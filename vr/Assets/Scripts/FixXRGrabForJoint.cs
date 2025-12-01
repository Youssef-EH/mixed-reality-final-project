using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class FixXRGrabForJoint : MonoBehaviour
{
    private void Awake()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        
        if (grab.movementType == XRBaseInteractable.MovementType.Kinematic)
        {
            Debug.Log("<color=orange>[FixXRGrab] Changing XRGrabInteractable movement type from Kinematic to VelocityTracking for FixedJoint compatibility</color>");
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }
    }
}
