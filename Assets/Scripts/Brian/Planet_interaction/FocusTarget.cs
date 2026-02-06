using UnityEditor.SearchService;
using UnityEngine;

public class FocusTarget : MonoBehaviour
{ 
    public float cameraPadding = 1.5f;
    public float splitDistance = 10f;
    
    public Vector2 zoomMaxMin = new Vector2(0.5f, 2f);
    private float zoomDistance;

    private bool isFocus = false;
    private Vector3 originalLocalPosition;

    void Awake()
    {
        originalLocalPosition = transform.localPosition;
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
        zoomDistance = 1; // reset the zoom
        
        return isFocus;
    }

    public void SplitFrom(Vector3 focusPoint)
    {
        Vector3 dir = (transform.position - focusPoint).normalized;
        Vector3 targetWorldPos = transform.position + dir * splitDistance;

        transform.position = Vector3.Lerp(
            transform.position,
            targetWorldPos,
            Time.deltaTime * 1.5f
        );
    }

    public void ReturnToOrbit()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalLocalPosition,
            Time.deltaTime * 1.5f
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