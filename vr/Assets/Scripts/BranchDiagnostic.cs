using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BranchDiagnostic : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("<color=cyan>========== BRANCH DIAGNOSTIC ==========</color>");
        
        var rb = GetComponent<Rigidbody>();
        var grab = GetComponent<XRGrabInteractable>();
        var pullInteraction = GetComponent<BranchPullInteraction>();
        var joint = GetComponent<FixedJoint>();
        
        Debug.Log($"Branch: {name}");
        Debug.Log($"Parent: {(transform.parent != null ? transform.parent.name : "NO PARENT!")}");
        
        if (rb != null)
        {
            Debug.Log($"✓ Rigidbody found - IsKinematic: {rb.isKinematic}, Mass: {rb.mass}");
        }
        else
        {
            Debug.LogError("✗ NO RIGIDBODY!");
        }
        
        if (grab != null)
        {
            Debug.Log($"✓ XRGrabInteractable found - MovementType: {grab.movementType}");
            if (grab.movementType == XRBaseInteractable.MovementType.Kinematic)
            {
                Debug.LogWarning("⚠️ WARNING: Movement type is Kinematic - this BREAKS FixedJoint!");
                Debug.LogWarning("⚠️ The XRGrabInteractable will override the joint and detach immediately!");
            }
        }
        else
        {
            Debug.LogError("✗ NO XRGrabInteractable!");
        }
        
        if (pullInteraction != null)
        {
            Debug.Log($"✓ BranchPullInteraction found - Threshold: {pullInteraction.PullForceThreshold}, Duration: {pullInteraction.PullDuration}s");
        }
        else
        {
            Debug.LogError("✗ NO BranchPullInteraction!");
        }
        
        if (joint != null)
        {
            Debug.Log($"✓ FixedJoint found - Connected to: {(joint.connectedBody != null ? joint.connectedBody.name : "NOTHING!")}");
            Debug.Log($"  Break Force: {joint.breakForce}, Break Torque: {joint.breakTorque}");
        }
        else
        {
            Debug.LogWarning("⚠️ NO FixedJoint yet - will be created by BranchJointSetup");
        }
        
        if (transform.parent != null)
        {
            var parentRb = transform.parent.GetComponent<Rigidbody>();
            if (parentRb != null)
            {
                Debug.Log($"✓ Parent Rigidbody found on '{transform.parent.name}' - IsKinematic: {parentRb.isKinematic}, Mass: {parentRb.mass}");
            }
            else
            {
                Debug.LogError($"✗ NO Rigidbody on parent '{transform.parent.name}'!");
            }
        }
        
        Debug.Log("<color=cyan>======================================</color>");
    }
    
    private void Update()
    {
        var joint = GetComponent<FixedJoint>();
        if (joint != null && !hasLoggedJointCreation)
        {
            Debug.Log($"<color=green>[Diagnostic] FixedJoint created! Connected to: {(joint.connectedBody != null ? joint.connectedBody.name : "nothing")}</color>");
            hasLoggedJointCreation = true;
        }
    }
    
    private bool hasLoggedJointCreation = false;
}
