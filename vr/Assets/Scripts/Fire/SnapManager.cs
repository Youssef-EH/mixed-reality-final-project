using System.Collections.Generic;
using UnityEngine;


public class SnapManager : MonoBehaviour
{
    public RainController rainController;
    public GameObject popupParent;
    public ImageListPopup imageListPopup;

    private bool allFilled;
    public GameObject fire;
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
        fire.SetActive(true);
        rainController.ToggleRain();
        StartCoroutine(imageListPopup.ShowCanvasesCoroutine(popupParent));
        Debug.Log("All branches snapped! allFilled = TRUE");
    }
}

