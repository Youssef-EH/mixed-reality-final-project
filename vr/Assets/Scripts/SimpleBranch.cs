using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SimpleBranch : MonoBehaviour
{
    [Header("Branch Properties")]
    [SerializeField] public float pullStrengthRequired = 30f;
    [SerializeField] private float detachDelay = 0.3f;
    
    [Header("Physics")]
    [SerializeField] private float mass = 0.3f;
    [SerializeField] private float dragAfterDetach = 0.5f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private FixedJoint joint;
    private Transform originalParent;
    private Vector3 previousPosition;
    private float grabTime;
    private bool isDetached = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.mass = mass;
        }

        originalParent = transform.parent;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnBranchGrabbed);
            grabInteractable.selectExited.AddListener(OnBranchReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnBranchGrabbed);
            grabInteractable.selectExited.RemoveListener(OnBranchReleased);
        }
    }

    private void OnBranchGrabbed(SelectEnterEventArgs args)
    {
        if (isDetached) return;

        grabTime = Time.time;
        previousPosition = transform.position;
    }

    private void OnBranchReleased(SelectExitEventArgs args)
    {
    }

    private void FixedUpdate()
    {
        if (isDetached || grabInteractable == null || !grabInteractable.isSelected) return;

        Vector3 currentPosition = transform.position;
        float pullDistance = Vector3.Distance(currentPosition, previousPosition);
        float timeHeld = Time.time - grabTime;

        if (pullDistance > 0.01f && timeHeld > detachDelay)
        {
            float pullSpeed = pullDistance / Time.fixedDeltaTime;
            
            if (pullSpeed > pullStrengthRequired)
            {
                DetachFromTree(currentPosition - previousPosition);
            }
        }

        previousPosition = currentPosition;
    }

    private void DetachFromTree(Vector3 pullDirection)
    {
        isDetached = true;

        transform.SetParent(null);

        if (joint != null)
        {
            Destroy(joint);
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearDamping = dragAfterDetach;
            rb.AddForce(pullDirection.normalized * 5f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }

        Debug.Log($"Branch {gameObject.name} detached from tree!");
    }

    public void AttachToTree(Transform parent)
    {
        transform.SetParent(parent);
        originalParent = parent;
        
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        isDetached = false;
    }
}
