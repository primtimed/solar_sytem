using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SolarSystemCamera : MonoBehaviour
{
    //=======================General=================================//

    private PlayerInputActions input;
    private Camera cam;
    public LayerMask celestialLayer;
    
    private GameObject sellected;
    
    //===================Focus on planet=============================//
    public float moveSpeed = 1f;
    public float rotateSpeed = 1f;
    
    private Transform currentTarget;
    private FocusTarget focusTarget;
    private Vector3 targetPosition;
    private bool hasFocus = false;
    
    private Vector3 basePosition;
    private Quaternion baseRotation;
    private bool returningToBase = false;

    public SolarSystemManager solarSystemManager;
    
    //===================Zoom on planet============================//

    private Scrollbar zoomScrollbar;
    
    //===================Rotation planet==============================//

    public float planetRotateSpeed = 0.1f;

    private bool isRotatingPlanet = false;
    private Vector2 rotateDelta;


    void Awake()
    {
        cam = Camera.main;
        zoomScrollbar = cam.GetComponentInChildren<Scrollbar>();
        input = new PlayerInputActions();

        basePosition = transform.position;
        baseRotation = transform.rotation;

        zoomScrollbar.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        input.Camera.Enable();

        input.Camera.Click.performed += OnClick;
        input.Camera.Scroll.performed += Scroll;

        input.Camera.Click.performed += OnRotateStart;
        input.Camera.Click.canceled += OnRotateEnd;
        input.Camera.RotateDelta.performed += OnRotateDelta;
    }

    void OnDisable()
    {
        input.Camera.Click.performed -= OnClick;
        input.Camera.Scroll.performed -= Scroll;

        input.Camera.Click.performed -= OnRotateStart;
        input.Camera.Click.canceled -= OnRotateEnd;
        input.Camera.RotateDelta.performed -= OnRotateDelta;

        input.Camera.Disable();
    }


    void FixedUpdate()
    {
        MouseMovement();
        FocusMovement();
        ReturnToBase();
        RotatePlanet();
    }

    
    //===================Focus on planet=============================//

    public void MouseMovement()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, celestialLayer))
        {
            sellected = hit.transform.gameObject;
            
            if (hasFocus) return;
            sellected.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        else if (sellected)
        {
            //Here will be the hover over planate shader disable
            
            sellected.GetComponent<MeshRenderer>().material.color = Color.white;
            sellected = null;
        }
    }

    void OnClick(InputAction.CallbackContext ctx)
    {
        if (hasFocus) return;

        if (sellected)
        {
            zoomScrollbar.gameObject.SetActive(true);
            
            focusTarget = sellected.transform.GetComponentInParent<FocusTarget>();
            if (focusTarget != null)
            {
               returningToBase = false;

                currentTarget = focusTarget.transform;
                hasFocus = focusTarget.FocusSwitch();

                float safeDistance = focusTarget.GetSafeDistance();

                Vector3 dir = (transform.position - currentTarget.position).normalized;

                targetPosition = currentTarget.position + dir * safeDistance;
            }
        }
    }

    void FocusMovement()
    { 
        if (!currentTarget) return;
        
        solarSystemManager.SplitPlanets(focusTarget);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * moveSpeed
        );

        Quaternion lookRot = Quaternion.LookRotation(
            currentTarget.position - transform.position
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            lookRot,
            Time.deltaTime * rotateSpeed
        );
    }
    
    public void ReturnToBase()
    {
        if (!returningToBase) return;
        
        solarSystemManager.ReturnPlanets();

        zoomScrollbar.value = .2f;
        targetPosition = Vector3.zero;
        focusTarget = null;
        currentTarget = null;
        hasFocus = false;
        
        transform.position = Vector3.Lerp(
            transform.position,
            basePosition,
            Time.deltaTime * moveSpeed
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            baseRotation,
            Time.deltaTime * rotateSpeed
        );
        
        if(Vector3.Distance(transform.position, basePosition) < 5f) returningToBase = false;
    }
    
    public void ReturnBoolSwitch() // Used by button
    {
        returningToBase = true;
        zoomScrollbar.gameObject.SetActive(false);
    }

    //===================Zoom on planet==============================//

    public void Scroll(InputAction.CallbackContext ctx)
    {
        if (!focusTarget) return;
        
        Vector2 scroll = ctx.ReadValue<Vector2>();
        
        focusTarget.ZoomDistance(-scroll.y / 10);
        
        float safeDistance = focusTarget.GetSafeDistance();
        Vector3 dir = (transform.position - currentTarget.position).normalized;
        targetPosition = currentTarget.position + dir * safeDistance;
        
        zoomScrollbar.value = Mathf.InverseLerp(focusTarget.zoomMaxMin.x, focusTarget.zoomMaxMin.y, focusTarget.GetZoomDistance());
    }

    public void ZoomUi(Scrollbar sliderValue) // Used by zoom slider
    {
        if (!focusTarget) return;

        float zoomDistance = Mathf.Lerp(focusTarget.zoomMaxMin.x, focusTarget.zoomMaxMin.y, sliderValue.value);
        
        focusTarget.SetZoomDistance(zoomDistance);

        float safeDistance = focusTarget.GetSafeDistance();
        Vector3 dir = (transform.position - currentTarget.position).normalized;
        targetPosition = currentTarget.position + dir * safeDistance;
    }
    
    //===================Rotation planet==============================//

    void OnRotateStart(InputAction.CallbackContext ctx)
    {
        if (!hasFocus || !currentTarget || !sellected) return;
        
        if (sellected.transform == currentTarget || sellected.transform.IsChildOf(currentTarget))
        {
            isRotatingPlanet = true;
        }
    }
    
    void OnRotateEnd(InputAction.CallbackContext ctx)
    {
        isRotatingPlanet = false;
        rotateDelta = Vector2.zero;
    }
    
    void OnRotateDelta(InputAction.CallbackContext ctx)
    {
        if (!isRotatingPlanet) return;
        rotateDelta = ctx.ReadValue<Vector2>();
    }
    
    void RotatePlanet()
    {
        if (!isRotatingPlanet || !currentTarget) return;

        Vector2 rotation = rotateDelta * planetRotateSpeed;

        currentTarget.Rotate(Vector3.up, -rotation.x, Space.World);
        currentTarget.Rotate(Vector3.right, rotation.y, Space.World);
    }
}