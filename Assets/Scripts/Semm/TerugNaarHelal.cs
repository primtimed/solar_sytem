using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerugNaarHelal : MonoBehaviour
{
    public GameObject infoPanel;
    public Animator animator;
    public GameObject canvas;
    public GameObject hyperBeam;

    public void Terug()
    {
        StartCoroutine(Hyperbeam());
        canvas.SetActive(false);
        hyperBeam.SetActive(true);
    }

    public IEnumerator Hyperbeam()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("LiveRotation");
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
