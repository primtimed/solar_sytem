using Unity.VisualScripting;
using UnityEngine;

public class FocusTarget : MonoBehaviour
{ 
    public float cameraPadding = 1.5f;
    public float splitDistance = 10f;
    
    public Vector2 zoomMaxMin;
    private float zoomDistance;

    private bool isFocus = false;
    public bool orbiting = false;

    private Vector3 originalLocalPosition;

    public float rotationSpeed = 20f;

    public Transform center;
    public Transform target;
    
    private TrailRenderer trail;
    public Material trailMat;

    void Awake()
    {
        originalLocalPosition = transform.localPosition;

        if (orbiting)
        {
            trail = this.AddComponent<TrailRenderer>();
            trail.startWidth = 10f;
            trail.endWidth = 10f;
            trail.time = 100f;
            
            trail.material = trailMat;
        }
    }

    void Update()
    {
        if (orbiting && center != null)
        {
            target.transform.RotateAround(
                center.position,
                Vector3.up,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public float GetSafeDistance()
    {
        Renderer r = GetComponentInChildren<Renderer>();
        if (r == null) return 2f;

        float radius = r.bounds.extents.magnitude;
        return radius * cameraPadding * zoomDistance;
    }

    public bool FocusSwitch()
    {
        isFocus = !isFocus;
        zoomDistance = 1;
        
        return isFocus;
    }

    public void SplitFrom(Vector3 focusPoint)
    {
        Vector3 dir = (transform.position - focusPoint).normalized;
        Vector3 targetWorldPos = transform.position + dir * splitDistance;

        transform.position = Vector3.Lerp(
            transform.position,
            targetWorldPos,
            Time.deltaTime * 1f
        );
    }

    public void ReturnToOrbit()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalLocalPosition,
            Time.deltaTime * 50f
        );
    }

    public void ZoomDistance(float zoom)
    {
        zoomDistance = Mathf.Clamp(zoomDistance + zoom, zoomMaxMin.x, zoomMaxMin.y);
    }

    public void SetZoomDistance(float zoom)
    {
        zoomDistance = zoom;
    }
    
    public float GetZoomDistance()
    {
        return zoomDistance;
    }

    public bool GetFocus()
    {
        return isFocus;
    }
}
