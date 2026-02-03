using UnityEngine;

public class FocusTarget : MonoBehaviour
{
    public float cameraPadding = 1.5f;
    private bool isFocus = false;

    public float GetSafeDistance()
    {
        Renderer r = GetComponentInChildren<Renderer>();
        if (r == null) return 2f;
        FocusSwitch();

        float radius = r.bounds.extents.magnitude;
        return radius * cameraPadding;
    }

    public bool FocusSwitch()
    {
        isFocus = !isFocus; 
        return isFocus;
    }
}