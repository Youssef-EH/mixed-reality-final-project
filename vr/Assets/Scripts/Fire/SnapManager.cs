using System.Collections.Generic;
using UnityEngine;


public class SnapManager : MonoBehaviour
{
    public RainController rainController;
    public GameObject popupParent;
    public ImageListPopup imageListPopup;

    public bool allFilled = false;
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
            if (!sp.isFilled && number<=0) //zet naar 3 voor alle takken, wordt nu sowieso geskipt
            {
                allFilled = false;
                return;
            }
        }
        if(allFilled==false)
        {
            allFilled = true;
            fire.SetActive(true);
            rainController.ToggleRain();
            StartCoroutine(imageListPopup.ShowCanvasesCoroutine(popupParent));
            Debug.Log("All branches snapped! allFilled = TRUE");
        }
    }
}

