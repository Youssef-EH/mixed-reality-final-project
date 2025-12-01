using UnityEngine;

[RequireComponent(typeof(BranchPullInteraction))]
public class BranchJointSetup : MonoBehaviour
{
    private void Start()
    {
        BranchPullInteraction branchPull = GetComponent<BranchPullInteraction>();
        
        if (transform.parent != null)
        {
            branchPull.SetupAttachment(transform.parent);
            Debug.Log($"<color=green>[BranchJoint] ✓ Branch attached to '{transform.parent.name}' with FixedJoint</color>");
        }
        else
        {
            Debug.LogError("[BranchJoint] Branch has no parent! Cannot create joint.");
        }
    }
}
