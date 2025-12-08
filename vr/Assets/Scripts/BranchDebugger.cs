using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BranchDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool drawDebugGizmos = true;
    [SerializeField] private float gizmoSize = 0.1f;

    private XRGrabInteractable grabInteractable;
    private BranchPullInteraction branchPull;
    private Rigidbody rb;
    private Vector3 lastPosition;
    private Vector3 attachmentPosition;
    private bool wasGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        branchPull = GetComponent<BranchPullInteraction>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnDebugGrabbed);
            grabInteractable.selectExited.AddListener(OnDebugReleased);
            grabInteractable.hoverEntered.AddListener(OnDebugHover);
            grabInteractable.hoverExited.AddListener(OnDebugHoverExit);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnDebugGrabbed);
            grabInteractable.selectExited.RemoveListener(OnDebugReleased);
            grabInteractable.hoverEntered.RemoveListener(OnDebugHover);
            grabInteractable.hoverExited.RemoveListener(OnDebugHoverExit);
        }
    }

    private void OnDebugHover(HoverEnterEventArgs args)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[BranchDebugger] HOVER ENTERED - Interactor: {args.interactorObject.transform.name}");
        }
    }

    private void OnDebugHoverExit(HoverExitEventArgs args)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[BranchDebugger] HOVER EXITED - Interactor: {args.interactorObject.transform.name}");
        }
    }

    private void OnDebugGrabbed(SelectEnterEventArgs args)
    {
        wasGrabbed = true;
        lastPosition = transform.position;
        
        if (showDebugLogs)
        {
            Debug.Log($"[BranchDebugger] GRABBED - Interactor: {args.interactorObject.transform.name}");
            if (branchPull != null)
            {
                Debug.Log($"[BranchDebugger] - Pull Threshold: {branchPull.PullForceThreshold}m");
                Debug.Log($"[BranchDebugger] - Pull Duration: {branchPull.PullDuration}s");
                Debug.Log($"[BranchDebugger] - Is Attached: {branchPull.IsAttached}");
            }
            Debug.Log($"[BranchDebugger] - Rigidbody IsKinematic: {rb.isKinematic}");
        }
    }

    private void OnDebugReleased(SelectExitEventArgs args)
    {
        wasGrabbed = false;
        
        if (showDebugLogs)
        {
            Debug.Log($"[BranchDebugger] RELEASED - Interactor: {args.interactorObject.transform.name}");
        }
    }

    private void Update()
    {
        if (!wasGrabbed || !grabInteractable.isSelected) return;

        if (branchPull != null && branchPull.IsAttached)
        {
            attachmentPosition = transform.position - (transform.position - lastPosition).normalized * 0.1f;
        }

        lastPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        if (!drawDebugGizmos) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = wasGrabbed ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, gizmoSize);
            
            if (col is CapsuleCollider capsule)
            {
                Gizmos.DrawWireSphere(transform.position, capsule.radius);
            }
        }

        if (wasGrabbed && branchPull != null && branchPull.IsAttached)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(attachmentPosition, transform.position);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attachmentPosition, 0.05f);
        }
    }
}
