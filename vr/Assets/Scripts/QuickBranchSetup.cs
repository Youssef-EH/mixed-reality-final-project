using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class QuickBranchSetup : MonoBehaviour
{
    [Header("Quick Setup - Add this to any GameObject to make it a pullable branch")]
    [Tooltip("The tree or parent this branch is attached to")]
    public Transform treeParent;
    
    [Header("Branch Settings")]
    [Tooltip("How fast you need to pull to detach (lower = easier)")]
    public float pullSpeed = 25f;
    
    [Tooltip("How long to wait before checking pull (prevents accidental detach)")]
    public float pullDelay = 0.2f;

    [Header("Auto Setup")]
    [Tooltip("Automatically add required components")]
    public bool autoSetupComponents = true;

    private void Start()
    {
        if (autoSetupComponents)
        {
            SetupBranch();
        }
        
        if (treeParent == null)
        {
            treeParent = transform.parent;
        }
    }

    private void SetupBranch()
    {
        if (GetComponent<XRGrabInteractable>() == null)
        {
            XRGrabInteractable grab = gameObject.AddComponent<XRGrabInteractable>();
            grab.trackPosition = true;
            grab.trackRotation = true;
            grab.throwOnDetach = true;
            Debug.Log($"Added XRGrabInteractable to {gameObject.name}");
        }

        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.mass = 0.3f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.5f;
            Debug.Log($"Added Rigidbody to {gameObject.name}");
        }

        if (GetComponent<Collider>() == null)
        {
            CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
            col.radius = 0.05f;
            col.height = 0.3f;
            col.direction = 1;
            Debug.Log($"Added Collider to {gameObject.name}");
        }

        if (GetComponent<SimpleBranch>() == null)
        {
            SimpleBranch branch = gameObject.AddComponent<SimpleBranch>();
            Debug.Log($"Added SimpleBranch to {gameObject.name}");
        }
    }

    private void OnValidate()
    {
        SimpleBranch branch = GetComponent<SimpleBranch>();
        if (branch != null)
        {
            Debug.Log("QuickBranchSetup: Adjust SimpleBranch component values in the Inspector");
        }
    }
}
