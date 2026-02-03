using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    private FocusTarget[] allTargets;

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
}