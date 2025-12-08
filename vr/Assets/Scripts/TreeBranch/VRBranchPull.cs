using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class VRBranchPull : MonoBehaviour
{
    [Header("Pull Settings")]
    [SerializeField] private float pullDistanceThreshold = 0.15f;
    [SerializeField] private float pullTimeRequired = 1.5f;
    [SerializeField] private float maxStretchDistance = 0.3f;
    
    [Header("Physics")]
    [SerializeField] private float jointSpringStrength = 500f;
    [SerializeField] private float jointDamping = 50f;
    [SerializeField] private float detachForce = 5f;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip snapSound;
    [SerializeField] private AudioClip detachSound;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool showGizmos = true;
    
    private XRGrabInteractable grabInteractable;
    private Rigidbody branchRigidbody;
    private ConfigurableJoint joint;
    private AudioSource audioSource;
    
    private bool isAttachedToTree = true;
    private bool isBeingPulled = false;
    private float pullStartTime;
    private Vector3 initialGrabPosition;
    private Vector3 jointConnectionPoint;
    
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        branchRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null && (snapSound != null || detachSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
        }
        
        ConfigureGrabInteractable();
    }
    
    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnBranchGrabbed);
        grabInteractable.selectExited.AddListener(OnBranchReleased);
        grabInteractable.hoverEntered.AddListener(OnBranchHoverEntered);
        grabInteractable.hoverExited.AddListener(OnBranchHoverExited);
    }
    
    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnBranchGrabbed);
        grabInteractable.selectExited.RemoveListener(OnBranchReleased);
        grabInteractable.hoverEntered.RemoveListener(OnBranchHoverEntered);
        grabInteractable.hoverExited.RemoveListener(OnBranchHoverExited);
    }
    
    private void Start()
    {
        VerifySetup();
        
        if (transform.parent != null)
        {
            CreateJointToTree();
        }
        else
        {
            Debug.LogError("[VRBranchPull] Branch must be a child of the tree object!");
        }
    }
    
    private void VerifySetup()
    {
        XRInteractionManager interactionManager = FindFirstObjectByType<XRInteractionManager>();
        
        if (interactionManager == null)
        {
            Debug.LogError("[VRBranchPull] NO XR INTERACTION MANAGER FOUND IN SCENE! Add one to enable grabbing.");
            Debug.LogError("[VRBranchPull] Create an empty GameObject and add XRInteractionManager component.");
        }
        else
        {
            grabInteractable.interactionManager = interactionManager;
            if (enableDebugLogs)
            {
                Debug.Log($"[VRBranchPull] XR Interaction Manager assigned: {interactionManager.name}");
            }
        }
        
        Collider[] colliders = GetComponents<Collider>();
        if (colliders.Length == 0)
        {
            Debug.LogError("[VRBranchPull] NO COLLIDER found on Branch! XRGrabInteractable needs a collider.");
        }
        else
        {
            grabInteractable.colliders.Clear();
            foreach (Collider col in colliders)
            {
                grabInteractable.colliders.Add(col);
                if (enableDebugLogs)
                {
                    Debug.Log($"[VRBranchPull] Collider registered: {col.GetType().Name}, IsTrigger: {col.isTrigger}");
                }
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[VRBranchPull] Branch setup complete. Layer: {LayerMask.LayerToName(gameObject.layer)}, Interaction Layers: {grabInteractable.interactionLayers.value}");
        }
    }
    
    private void OnBranchHoverEntered(HoverEnterEventArgs args)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"<color=cyan>[VRBranchPull] HOVER ENTERED by {args.interactorObject.transform.name}!</color>");
        }
    }
    
    private void OnBranchHoverExited(HoverExitEventArgs args)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"<color=cyan>[VRBranchPull] HOVER EXITED by {args.interactorObject.transform.name}</color>");
        }
    }
    
    private void ConfigureGrabInteractable()
    {
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;
        grabInteractable.smoothPosition = false;
        grabInteractable.smoothRotation = false;
        grabInteractable.retainTransformParent = false;
    }
    
    private void CreateJointToTree()
    {
        Transform treeParent = transform.parent;
        jointConnectionPoint = transform.position;
        
        joint = gameObject.AddComponent<ConfigurableJoint>();
        
        Rigidbody parentRigidbody = treeParent.GetComponent<Rigidbody>();
        if (parentRigidbody != null)
        {
            joint.connectedBody = parentRigidbody;
        }
        
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        
        SoftJointLimit limit = new SoftJointLimit
        {
            limit = maxStretchDistance,
            bounciness = 0f,
            contactDistance = 0.01f
        };
        joint.linearLimit = limit;
        
        SoftJointLimitSpring spring = new SoftJointLimitSpring
        {
            spring = jointSpringStrength,
            damper = jointDamping
        };
        joint.linearLimitSpring = spring;
        
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
        
        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;
        joint.enableCollision = false;
        joint.enablePreprocessing = true;
        
        branchRigidbody.isKinematic = false;
        branchRigidbody.useGravity = false;
        branchRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[VRBranchPull] Branch joint created. Pull distance: {pullDistanceThreshold}m, Time: {pullTimeRequired}s");
        }
    }
    
    private void OnBranchGrabbed(SelectEnterEventArgs args)
    {
        if (!isAttachedToTree) return;
        
        initialGrabPosition = transform.position;
        isBeingPulled = false;
        pullStartTime = 0f;
        
        if (enableDebugLogs)
        {
            Debug.Log($"[VRBranchPull] Branch grabbed by {args.interactorObject.transform.name}");
        }
        
        if (snapSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(snapSound);
        }
    }
    
    private void OnBranchReleased(SelectExitEventArgs args)
    {
        if (isAttachedToTree)
        {
            isBeingPulled = false;
            
            if (enableDebugLogs)
            {
                Debug.Log("[VRBranchPull] Branch released while still attached");
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (!isAttachedToTree || !grabInteractable.isSelected || joint == null) return;
        
        CheckPullProgress();
    }
    
    private void CheckPullProgress()
    {
        float currentDistance = Vector3.Distance(transform.position, jointConnectionPoint);
        
        if (currentDistance >= pullDistanceThreshold)
        {
            if (!isBeingPulled)
            {
                isBeingPulled = true;
                pullStartTime = Time.time;
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[VRBranchPull] Pull started! Distance: {currentDistance:F3}m");
                }
            }
            
            float pullDuration = Time.time - pullStartTime;
            
            if (enableDebugLogs && Time.frameCount % 30 == 0)
            {
                Debug.Log($"[VRBranchPull] Pulling: {pullDuration:F2}s / {pullTimeRequired:F2}s (Distance: {currentDistance:F3}m)");
            }
            
            if (pullDuration >= pullTimeRequired)
            {
                DetachBranchFromTree();
            }
        }
        else
        {
            if (isBeingPulled)
            {
                isBeingPulled = false;
                
                if (enableDebugLogs)
                {
                    Debug.Log("[VRBranchPull] Pull stopped - not pulling hard enough");
                }
            }
        }
    }
    
    private void DetachBranchFromTree()
    {
        if (!isAttachedToTree) return;
        
        isAttachedToTree = false;
        isBeingPulled = false;
        
        if (enableDebugLogs)
        {
            Debug.Log("[VRBranchPull] Branch detached from tree!");
        }
        
        Vector3 pullDirection = (transform.position - jointConnectionPoint).normalized;
        
        if (joint != null)
        {
            Destroy(joint);
        }
        
        transform.SetParent(null);
        
        branchRigidbody.useGravity = true;
        branchRigidbody.linearDamping = 0.5f;
        branchRigidbody.angularDamping = 0.5f;
        branchRigidbody.AddForce(pullDirection * detachForce, ForceMode.Impulse);
        branchRigidbody.AddTorque(Random.onUnitSphere * 2f, ForceMode.Impulse);
        
        if (detachSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(detachSound);
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        if (isAttachedToTree && joint != null)
        {
            Gizmos.color = isBeingPulled ? Color.red : Color.green;
            Gizmos.DrawLine(jointConnectionPoint, transform.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(jointConnectionPoint, pullDistanceThreshold);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(jointConnectionPoint, maxStretchDistance);
        }
        
        Gizmos.color = grabInteractable != null && grabInteractable.isSelected ? Color.cyan : Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }
}
