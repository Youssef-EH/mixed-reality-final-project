using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SimpleBranchGrab : MonoBehaviour
{
    private ConfigurableJoint joint;
    private Rigidbody branchRigidbody;
    private XRGrabInteractable grabInteractable;
    
    private void Awake()
    {
        branchRigidbody = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }
    
    private void Start()
    {
        AttachToTree();
    }
    
    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }
    
    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }
    
    private void AttachToTree()
    {
        if (transform.parent == null)
        {
            Debug.LogWarning("Branch has no parent tree!");
            return;
        }
        
        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = transform.parent.GetComponent<Rigidbody>();
        
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        
        branchRigidbody.isKinematic = false;
        branchRigidbody.useGravity = false;
    }
    
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        DetachFromTree();
    }
    
    private void DetachFromTree()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
        
        transform.SetParent(null);
        branchRigidbody.useGravity = true;
    }
}
