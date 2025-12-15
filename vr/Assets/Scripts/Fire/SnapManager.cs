using System.Collections.Generic;
using UnityEngine;


public class SnapManager : MonoBehaviour
{
    public bool allFilled;
    [HideInInspector]
    public List<SnapPoint> snapPoints = new List<SnapPoint>();

    public void RegisterSnapPoint(SnapPoint sp)
    {
        if (!snapPoints.Contains(sp))
            snapPoints.Add(sp);
    }
    public void CheckSnapPoints()
    {
        int number = 0;
        foreach (SnapPoint sp in snapPoints)
        {
            number++;
            if (!sp.isFilled && number<=3)
            {
                allFilled = false;
                return;
            }
        }

        allFilled = true;
        Debug.Log("All branches snapped! allFilled = TRUE");
    }
}

