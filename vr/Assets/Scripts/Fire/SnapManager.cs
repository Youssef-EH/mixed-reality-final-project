using UnityEngine;


public class SnapManager : MonoBehaviour
{
    public SnapPoint[] snapPoints;
    public bool allFilled;   // Will become true when all snap points are filled

    public void CheckSnapPoints()
    {
        foreach (SnapPoint sp in snapPoints)
        {
            if (!sp.isFilled)
            {
                allFilled = false;
                return;
            }
        }

        allFilled = true;
        Debug.Log("All branches snapped! allFilled = TRUE");
    }
}

