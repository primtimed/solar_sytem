using System;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    private FocusTarget[] allTargets;

    public GameObject rotation;
    public GameObject stat;
    public bool rotating = false;

    public Transform rotateFiewLoc;
    public Transform statFiewLoc;
    
    public float cameraLerpSpeed = 2f;

    void Awake()
    {
        allTargets = FindObjectsOfType<FocusTarget>();
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
    }


    public void RotateSwitch() // Used by button
    {
        rotating = !rotating;

        Camera.main.fieldOfView = 60; // reset fov
    }
}