using UnityEngine;
using UnityEngine.SceneManagement;

public class TerugNaarHelal : MonoBehaviour
{
    public GameObject infoPanel;
    public Animator animator;

    public void Terug()
    {
        SceneManager.LoadScene("Pan");
    }

    public void CloseMenus()
    {
        if(infoPanel.active == true)
        {
            animator.SetBool("Tween", true);
        }
        else
        {
            animator.SetBool("Tween", false);
        }
        
    }


}
