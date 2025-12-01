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
    private SimpleBranch simpleBranch;
    private Rigidbody rb;
    private Vector3 lastPosition;
    private bool wasGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        simpleBranch = GetComponent<SimpleBranch>();
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
            Debug.Log($"[BranchDebugger] ✓ GRABBED! - Interactor: {args.interactorObject.transform.name}");
            Debug.Log($"[BranchDebugger] - SimpleBranch Pull Strength Required: {(simpleBranch != null ? simpleBranch.pullStrengthRequired.ToString() : "N/A")}");
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

        Vector3 currentPos = transform.position;
        float pullDistance = Vector3.Distance(currentPos, lastPosition);
        float pullSpeed = pullDistance / Time.deltaTime;

        if (showDebugLogs && pullDistance > 0.001f)
        {
            Debug.Log($"[BranchDebugger] Pull Speed: {pullSpeed:F2} (Required: {(simpleBranch != null ? simpleBranch.pullStrengthRequired : 0)})");
        }

        lastPosition = currentPos;
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

        if (wasGrabbed)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(lastPosition, transform.position);
        }
    }
}
