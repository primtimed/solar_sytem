using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteractionHotspot : MonoBehaviour
{
    private PlayerInputActions input;
    private Camera cam;

    public void Awake()
    {
        cam = this.GetComponent<Camera>();
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Camera.Enable();
        input.Camera.Click.performed += HotspotInteract;
    }

    void OnDisable()
    {
        input.Camera.Click.performed -= HotspotInteract;
        input.Camera.Disable();
    }

    public void HotspotInteract(InputAction.CallbackContext ctx)
    {

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Debug.Log(mousePos);

        Ray ray = cam.ScreenPointToRay(mousePos);

        Debug.Log(ray);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        { 
            string sceneName = hit.collider.GetComponent<FlagInfo>().scenename;
            Debug.Log("Doet het");
            SceneManager.LoadScene(sceneName);
        }
    }
}
