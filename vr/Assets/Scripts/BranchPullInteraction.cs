using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class BranchPullInteraction : MonoBehaviour
{
    [Header("Pull Settings")]
    [SerializeField] private float pullForceThreshold = 0.2f;
    [SerializeField] private float pullDuration = 2f;
    [SerializeField] private bool destroyJointOnDetach = true;
    [SerializeField] private float grabStartDelay = 0.1f;
    [SerializeField] private float jointSpring = 500f;
    [SerializeField] private float jointDamper = 50f;

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
    private ConfigurableJoint attachmentJoint;
    private bool _isAttached = true;
    private float pullStartTime;
    private float grabTime;
    private Vector3 lastPosition;
    private Vector3 jointAnchorWorldPos;
    private Vector3 grabAnchorPos;
    private IXRSelectInteractor currentInteractor;
    private bool isPulling;
    private bool hasSetGrabAnchor;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        branchRigidbody = GetComponent<Rigidbody>();

        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.retainTransformParent = false;
        grabInteractable.smoothPosition = false;
        grabInteractable.smoothRotation = false;

        branchRigidbody.mass = detachedMass;
        branchRigidbody.linearDamping = detachedLinearDamping;

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
        grabTime = Time.time;
        pullStartTime = Time.time;
        lastPosition = transform.position;
        isPulling = false;
        hasSetGrabAnchor = false;

        if (showDebugLogs)
        {
            Debug.Log($"<color=cyan>[BranchPull] Branch GRABBED! Pull force threshold: {pullForceThreshold}, Duration: {pullDuration}s</color>");
            Debug.Log($"<color=cyan>[BranchPull] Joint exists: {attachmentJoint != null}, Is Attached: {_isAttached}</color>");
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        currentInteractor = null;
        isPulling = false;
        hasSetGrabAnchor = false;
        
        if (showDebugLogs && _isAttached)
        {
            Debug.Log("[BranchPull] Branch released (still attached)");
        }
    }

    private void FixedUpdate()
    {
        if (!_isAttached || currentInteractor == null || attachmentJoint == null) return;

        float timeSinceGrab = Time.time - grabTime;
        
        if (timeSinceGrab < grabStartDelay) return;

        if (!hasSetGrabAnchor)
        {
            grabAnchorPos = transform.position;
            hasSetGrabAnchor = true;
            
            if (showDebugLogs)
            {
                Debug.Log($"<color=green>[BranchPull] Grab anchor set at: {grabAnchorPos}</color>");
            }
        }

        Vector3 currentPosition = transform.position;
        Vector3 pullDirection = currentPosition - grabAnchorPos;
        float pullDistance = pullDirection.magnitude;

        if (pullDistance > pullForceThreshold)
        {
            if (!isPulling)
            {
                isPulling = true;
                pullStartTime = Time.time;
                
                if (showDebugLogs)
                {
                    Debug.Log($"<color=yellow>[BranchPull] Started pulling! Distance: {pullDistance:F3}m (threshold: {pullForceThreshold}m)</color>");
                }
            }
            
            float pullingTime = Time.time - pullStartTime;
            
            if (showDebugLogs && Time.frameCount % 30 == 0)
            {
                Debug.Log($"[BranchPull] Pull distance: {pullDistance:F3}m (need {pullForceThreshold}m) | Time: {pullingTime:F2}s (need {pullDuration}s)");
            }

            if (pullingTime >= pullDuration)
            {
                DetachBranch(pullDirection.normalized);
            }
        }
        else
        {
            if (isPulling)
            {
                isPulling = false;
                
                if (showDebugLogs)
                {
                    Debug.Log($"<color=cyan>[BranchPull] Pull released - distance dropped to {pullDistance:F3}m</color>");
                }
            }
        }
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
        branchRigidbody.useGravity = true;
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
            attachmentJoint = gameObject.AddComponent<ConfigurableJoint>();
        }
        
        Rigidbody parentRb = parentTransform.GetComponent<Rigidbody>();
        if (parentRb != null)
        {
            attachmentJoint.connectedBody = parentRb;
        }
        
        attachmentJoint.xMotion = ConfigurableJointMotion.Limited;
        attachmentJoint.yMotion = ConfigurableJointMotion.Limited;
        attachmentJoint.zMotion = ConfigurableJointMotion.Limited;
        
        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = pullForceThreshold * 2f;
        linearLimit.bounciness = 0f;
        linearLimit.contactDistance = 0f;
        attachmentJoint.linearLimit = linearLimit;
        
        SoftJointLimitSpring linearSpring = new SoftJointLimitSpring();
        linearSpring.spring = jointSpring;
        linearSpring.damper = jointDamper;
        attachmentJoint.linearLimitSpring = linearSpring;
        
        attachmentJoint.angularXMotion = ConfigurableJointMotion.Locked;
        attachmentJoint.angularYMotion = ConfigurableJointMotion.Locked;
        attachmentJoint.angularZMotion = ConfigurableJointMotion.Locked;
        
        attachmentJoint.breakForce = Mathf.Infinity;
        attachmentJoint.breakTorque = Mathf.Infinity;
        attachmentJoint.enableCollision = false;
        attachmentJoint.enablePreprocessing = true;

        branchRigidbody.isKinematic = false;
        branchRigidbody.useGravity = false;
        branchRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        _isAttached = true;
        jointAnchorWorldPos = transform.position;

        if (showDebugLogs)
        {
            Debug.Log($"<color=green>[BranchPull] ✓ Branch attached to '{parentTransform.name}' with ConfigurableJoint</color>");
            Debug.Log($"<color=green>[BranchPull] ✓ Joint connected: {attachmentJoint.connectedBody != null}</color>");
            Debug.Log($"<color=green>[BranchPull] ✓ Joint anchor position: {jointAnchorWorldPos}</color>");
            Debug.Log($"<color=green>[BranchPull] ✓ Spring: {jointSpring}, Damper: {jointDamper}</color>");
        }
    }
}
