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
    private Button backButton;
    
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
    
    //===================Pan solar system==============================//
    public float panSpeed = 250f;
    private Vector2 panInput;
    public Vector2 maxpan;
    
    //===================Interaction hotspot==============================//
    private InteractionHotspot interactionHotspot;
    
    //===================Zoom solar system============================//
    public Vector2 zoomMaxMin;


    void Awake()
    {
        cam = Camera.main;
        zoomScrollbar = cam.GetComponentInChildren<Scrollbar>();
        backButton = cam.GetComponentInChildren<Button>();
        interactionHotspot = cam.GetComponent<InteractionHotspot>();
        input = new PlayerInputActions();

        basePosition = transform.position;
        baseRotation = transform.rotation;
        
        zoomScrollbar.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        interactionHotspot.enabled = false;
    }

    void OnEnable()
    {
        input.Camera.Enable();

        input.Camera.Click.performed += OnClick;
        input.Camera.Scroll.performed += Scroll;

        input.Camera.Click.performed += OnRotateStart;
        input.Camera.Click.canceled += OnRotateEnd;
        input.Camera.RotateDelta.performed += OnRotateDelta;

        input.Camera.Pan.performed += OnPan;
        input.Camera.Pan.canceled += OnPanCancel;
    }

    void OnDisable()
    {
        input.Camera.Click.performed -= OnClick;
        input.Camera.Scroll.performed -= Scroll;

        input.Camera.Click.performed -= OnRotateStart;
        input.Camera.Click.canceled -= OnRotateEnd;
        input.Camera.RotateDelta.performed -= OnRotateDelta;

        input.Camera.Pan.performed -= OnPan;
        input.Camera.Pan.canceled -= OnPanCancel;

        input.Camera.Disable();
    }

    void FixedUpdate()
    {
        MouseMovement();
        FocusMovement();
        ReturnToBase();
        RotatePlanet();
        PanSolarSystem();
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
            interactionHotspot.enabled = true;
            cam.fieldOfView = 60; // set field of view to default for planet zoom
            zoomScrollbar.gameObject.SetActive(true);
            backButton.gameObject.SetActive(true);
            
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
        interactionHotspot.enabled = false;
        
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
        backButton.gameObject.SetActive(false);
    }

    //===================Zoom on planet==============================//

    public void Scroll(InputAction.CallbackContext ctx)
    {
        Vector2 scroll = ctx.ReadValue<Vector2>();

        if (!focusTarget)
        {
            cam.fieldOfView += scroll.y;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, zoomMaxMin.x, zoomMaxMin.y);
        }
        else
        {
            focusTarget.ZoomDistance(-scroll.y / 10f);

            float safeDistance = focusTarget.GetSafeDistance();
            Vector3 dir = (transform.position - currentTarget.position).normalized;
            targetPosition = currentTarget.position + dir * safeDistance;

            zoomScrollbar.value = Mathf.InverseLerp(
                focusTarget.zoomMaxMin.x,
                focusTarget.zoomMaxMin.y,
                focusTarget.GetZoomDistance()
            );
        }
    }


    public void ZoomUi(Scrollbar sliderValue) // Used by zoom slider
    {
        float zoomDistance = Mathf.Lerp(focusTarget.zoomMaxMin.x, focusTarget.zoomMaxMin.y, sliderValue.value);
        
        if (!focusTarget)
        {
            
        }
        
        else
        {
            focusTarget.SetZoomDistance(zoomDistance);

            float safeDistance = focusTarget.GetSafeDistance();
            Vector3 dir = (transform.position - currentTarget.position).normalized;
            targetPosition = currentTarget.position + dir * safeDistance;
        }
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
    
    //===================Pan solar system==============================//
    void OnPan(InputAction.CallbackContext ctx)
    {
        panInput = ctx.ReadValue<Vector2>();
    }

    void OnPanCancel(InputAction.CallbackContext ctx)
    {
        panInput = Vector2.zero;
    }

    void PanSolarSystem()
    {
        if (hasFocus) return;

        Vector3 right = transform.right;
        Vector3 up = transform.up;

        Vector3 move = (right * panInput.x + up * panInput.y) * panSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + move;

        float clampedX = Mathf.Clamp(
            newPos.x,
            basePosition.x - maxpan.x,
            basePosition.x + maxpan.x
        );
        float clampedY = Mathf.Clamp(
            newPos.y,
            basePosition.y - maxpan.y,
            basePosition.y + maxpan.y
        );

        transform.position = new Vector3(
            clampedX,
            clampedY,
            newPos.z
        );
    }


}