using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class BranchPullInteraction : MonoBehaviour
{
    [Header("Pull Settings")]
    [SerializeField] private float pullForceThreshold;
    [SerializeField] private float pullDuration;
    [SerializeField] private bool destroyJointOnDetach = true;

    [Header("Detached Settings")]
    [SerializeField] private float detachedMass = 0.5f;
    [SerializeField] private float detachedLinearDamping = 0.5f;
    [SerializeField] private float breakTorque = 10f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip detachSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    public float PullForceThreshold => pullForceThreshold;
    public float PullDuration => pullDuration;
    public bool IsAttached => _isAttached;

    private XRGrabInteractable grabInteractable;
    private Rigidbody branchRigidbody;
    private FixedJoint attachmentJoint;
    private bool _isAttached = true;
    private float pullStartTime;
    private Vector3 lastPosition;
    private IXRSelectInteractor currentInteractor;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        branchRigidbody = GetComponent<Rigidbody>();

        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;

        branchRigidbody.isKinematic = true;
        branchRigidbody.mass = detachedMass;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!_isAttached)
        {
            if (showDebugLogs)
                Debug.Log("[BranchPull] Branch grabbed but already detached");
            return;
        }

        currentInteractor = args.interactorObject;
        pullStartTime = Time.time;
        lastPosition = transform.position;

        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>[BranchPull] Branch GRABBED! Pull force threshold: {pullForceThreshold}, Duration: {pullDuration}s</color>");
            Debug.Log($"<color=cyan>[BranchPull] Joint exists: {attachmentJoint != null}, Is Attached: {_isAttached}</color>");
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        currentInteractor = null;
        
        if (showDebugLogs && _isAttached)
        {
            Debug.Log("[BranchPull] Branch released (still attached)");
        }
    }

    private void FixedUpdate()
    {
        if (!_isAttached || currentInteractor == null) return;

        Vector3 currentPosition = transform.position;
        Vector3 pullDirection = currentPosition - lastPosition;
        float pullForce = pullDirection.magnitude / Time.fixedDeltaTime;

        float timePulling = Time.time - pullStartTime;

        if (showDebugLogs && pullForce > 1f)
        {
            Debug.Log($"[BranchPull] Pull force: {pullForce:F1} (need {pullForceThreshold}) | Time: {timePulling:F2}s (need {pullDuration}s)");
        }

        if (pullForce > pullForceThreshold && timePulling > pullDuration)
        {
            DetachBranch(pullDirection.normalized);
        }

        lastPosition = currentPosition;
    }

    private void DetachBranch(Vector3 pullDirection)
    {
        if (!_isAttached) return;

        _isAttached = false;

        if (showDebugLogs)
        {
            Debug.Log($"<color=yellow>[BranchPull] 🌿 Branch DETACHED! Breaking from tree...</color>");
        }

        if (attachmentJoint != null && destroyJointOnDetach)
        {
            Destroy(attachmentJoint);
        }

        branchRigidbody.isKinematic = false;
        branchRigidbody.linearDamping = detachedLinearDamping;
        branchRigidbody.AddForce(pullDirection * pullForceThreshold * 0.5f, ForceMode.Impulse);
        branchRigidbody.AddTorque(Random.onUnitSphere * breakTorque, ForceMode.Impulse);

        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;

        if (detachSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(detachSound);
        }

        transform.SetParent(null);
    }

    public void SetupAttachment(Transform parentTransform)
    {
        transform.SetParent(parentTransform);
        
        if (attachmentJoint == null)
        {
            attachmentJoint = gameObject.AddComponent<FixedJoint>();
        }
        
        Rigidbody parentRb = parentTransform.GetComponent<Rigidbody>();
        if (parentRb != null)
        {
            attachmentJoint.connectedBody = parentRb;
        }
        
        attachmentJoint.breakForce = Mathf.Infinity;
        attachmentJoint.breakTorque = Mathf.Infinity;

        _isAttached = true;
        branchRigidbody.isKinematic = true;

        if (showDebugLogs)
        {
            Debug.Log($"<color=green>[BranchPull] ✓ Branch attached to '{parentTransform.name}' with FixedJoint</color>");
            Debug.Log($"<color=green>[BranchPull] ✓ Joint connected: {attachmentJoint.connectedBody != null}</color>");
        }
    }
}
