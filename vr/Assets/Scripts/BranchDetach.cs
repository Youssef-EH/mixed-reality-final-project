using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class BranchDetach : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private Transform branchParent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        branchParent = transform.parent;
        
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (branchParent != null)
        {
            branchParent.SetParent(null);
        }
        
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
