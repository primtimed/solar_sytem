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
        FocusMovement();
        ReturnToBase();
    }

    void OnClick(InputAction.CallbackContext ctx)
    {
        if (hasFocus) return;
        
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, celestialLayer))
        {
            focusTarget = hit.transform.GetComponentInParent<FocusTarget>();
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
        if (currentTarget == null) return;

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
    
    public void ReturnBoolSwitch() { returningToBase = true; } // Used by button
}