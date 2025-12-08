using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInteractionManagerAutoSetup : MonoBehaviour
{
    private void Awake()
    {
        XRInteractionManager existingManager = FindFirstObjectByType<XRInteractionManager>();
        
        if (existingManager == null)
        {
            GameObject managerObject = new GameObject("XR Interaction Manager");
            XRInteractionManager manager = managerObject.AddComponent<XRInteractionManager>();
            Debug.Log("[XRInteractionManagerAutoSetup] Created XR Interaction Manager automatically!");
        }
        else
        {
            Debug.Log($"[XRInteractionManagerAutoSetup] XR Interaction Manager already exists: {existingManager.name}");
        }
    }
}
