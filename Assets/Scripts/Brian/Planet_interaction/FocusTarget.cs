using UnityEngine;

public class FocusTarget : MonoBehaviour
{
    public float cameraPadding = 1.5f;
    public float splitDistance = 10f;

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
        return radius * cameraPadding;
    }

    public bool FocusSwitch()
    {
        isFocus = !isFocus;
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
}