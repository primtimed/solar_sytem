using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SolarSystemCamera : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotateSpeed = 1f;
    public LayerMask celestialLayer;

    private PlayerInputActions input;
    private Camera cam;

    private Transform currentTarget;
    private FocusTarget focusTarget;
    private Vector3 targetPosition;
    private bool hasFocus = false;
    
    private Vector3 basePosition;
    private Quaternion baseRotation;
    private bool returningToBase = false;

    public SolarSystemManager solarSystemManager;

    void Awake()
    {
        cam = Camera.main;
        input = new PlayerInputActions();

        basePosition = transform.position;
        baseRotation = transform.rotation;
    }

    void OnEnable()
    {
        input.Camera.Enable();
        input.Camera.Click.performed += OnClick;
    }

    void OnDisable()
    {
        input.Camera.Click.performed -= OnClick;
        input.Camera.Disable();
    }

    void FixedUpdate()
    {
        MouseMovement();
        FocusMovement();
        ReturnToBase();
    }

    GameObject sellected;
    public void MouseMovement()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, celestialLayer))
        {
            if (hasFocus) return;

            sellected = hit.transform.gameObject;
            
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
    }
}