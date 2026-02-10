using System;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    private FocusTarget[] allTargets;
    
    public FocusTarget[] rotatingPlanets;
    public Vector2[] size;

    public GameObject rotation;
    public GameObject stat;
    public bool rotating = false;
    public bool realSize = false;

    public Transform rotateFiewLoc;
    public Transform statFiewLoc;
    
    public float cameraLerpSpeed = 2f;

    public GameObject ScaleButton;

    void Awake()
    {
        allTargets = FindObjectsOfType<FocusTarget>();
        rotatingPlanets = rotation.GetComponentsInChildren<FocusTarget>();
    }

    public void SplitPlanets(FocusTarget focused)
    {
        foreach (FocusTarget target in allTargets)
        {
            if (target != focused)
            {
                target.SplitFrom(focused.transform.position);
            }
        }
    }

    public void ReturnPlanets()
    {
        foreach (FocusTarget target in allTargets)
        {
            target.ReturnToOrbit();
        }
    }
    
    public void Update()
    {
        rotation.gameObject.SetActive(rotating);     
        stat.gameObject.SetActive(!rotating);

        Camera cam = Camera.main;
        Vector3 targetPos = rotating ? rotateFiewLoc.position : statFiewLoc.position;
        Quaternion targetRot = rotating ? rotateFiewLoc.rotation : statFiewLoc.rotation;

        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            targetPos,
            Time.deltaTime * cameraLerpSpeed
        );

        cam.transform.rotation = Quaternion.Slerp(
            cam.transform.rotation,
            targetRot,
            Time.deltaTime * cameraLerpSpeed
        );

        if (rotating)
        {
            ScaleButton.SetActive(true);
        }
        else
        {
            ScaleButton.SetActive(false);
        }
    }


    public void RotateSwitch() // Used by button
    {
        rotating = !rotating;
        
        realSize = true;
        ScaleSwitch();

        Camera.main.fieldOfView = 60; // reset fov
    }

    public void ScaleSwitch()
    {
        if (rotating)
        {
            if (realSize)
            {
                for (int i = 0; i < rotatingPlanets.Length; i++)
                {
                    rotatingPlanets[i].transform.localScale = new Vector3(size[i].x, size[i].x, size[i].x);
                }

            }
            else
            {
                for (int i = 0; i < rotatingPlanets.Length; i++)
                {
                    rotatingPlanets[i].transform.localScale = new Vector3(size[i].y, size[i].y, size[i].y);                
                }            
            }
            
            realSize = !realSize;
        }
    }
    
    
}