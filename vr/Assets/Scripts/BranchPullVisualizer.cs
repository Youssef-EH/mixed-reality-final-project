using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BranchPullVisualizer : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Renderer branchRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color grabbedColor = Color.yellow;
    [SerializeField] private Color pullingColor = Color.red;
    [SerializeField] private float colorTransitionSpeed = 5f;

    [Header("Pull Indicators")]
    [SerializeField] private bool showPullDirection = true;
    [SerializeField] private float pullArrowLength = 0.5f;
    [SerializeField] private Color pullArrowColor = Color.cyan;

    private XRGrabInteractable grabInteractable;
    private SimpleBranch simpleBranch;
    private BranchPullInteraction advancedBranch;
    private Material branchMaterial;
    private Color currentColor;
    private Vector3 lastPosition;
    private bool isGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        simpleBranch = GetComponent<SimpleBranch>();
        advancedBranch = GetComponent<BranchPullInteraction>();

        if (branchRenderer == null)
        {
            branchRenderer = GetComponentInChildren<Renderer>();
        }

        if (branchRenderer != null)
        {
            branchMaterial = branchRenderer.material;
            currentColor = normalColor;
        }
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        lastPosition = transform.position;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void Update()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (branchMaterial == null) return;

        Color targetColor = normalColor;

        if (isGrabbed)
        {
            Vector3 currentPos = transform.position;
            float pullDistance = Vector3.Distance(currentPos, lastPosition);

            if (pullDistance > 0.02f)
            {
                targetColor = pullingColor;
            }
            else
            {
                targetColor = grabbedColor;
            }

            lastPosition = currentPos;
        }

        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorTransitionSpeed);
        branchMaterial.color = currentColor;
    }

    private void OnDrawGizmos()
    {
        if (!isGrabbed || !showPullDirection) return;

        Vector3 pullDirection = transform.position - lastPosition;
        if (pullDirection.magnitude > 0.001f)
        {
            Gizmos.color = pullArrowColor;
            Gizmos.DrawRay(transform.position, pullDirection.normalized * pullArrowLength);
            Gizmos.DrawWireSphere(transform.position + pullDirection.normalized * pullArrowLength, 0.02f);
        }
    }
}
