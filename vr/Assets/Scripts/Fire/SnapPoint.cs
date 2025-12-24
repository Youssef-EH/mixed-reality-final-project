using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnapPoint : MonoBehaviour
{
    public string targetTag = "Branch";
    public GameObject outlineIndicator;
    public bool isFilled;                 // true when something is snapped here
    public Transform snapPosition;        // where the object should snap
    public SnapManager manager;           // reference to manager

    private void Awake()
    {
        if (manager == null)
            manager = FindFirstObjectByType<SnapManager>();
        if (manager != null)
            manager.RegisterSnapPoint(this);

        if (snapPosition == null)
        {
            snapPosition = transform.GetChild(0); // first child
            if (snapPosition == null)
                Debug.LogError($"{name}: No snapPosition assigned or child found!");
        }
        if (outlineIndicator != null)
            outlineIndicator.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        XRGrabInteractable grab = other.GetComponentInParent<XRGrabInteractable>();
        if (grab == null || grab.GetComponent<SnapMarker>() != null)
            return;
        if (isFilled)
            return; 

        if (!other.CompareTag(targetTag))
            return;

        outlineIndicator.SetActive(true);
        SnapObject(grab);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(targetTag))
            return;

        if (!isFilled) 
            outlineIndicator.SetActive(false);
    }

    private void SnapObject(XRGrabInteractable grab)
    {
        if (snapPosition == null)
        {
            Debug.LogError($"{name}: snapPosition is not assigned! Assign a child transform in the inspector.");
            return;
        }
        if (grab.GetComponent<SnapMarker>() != null)
            return;
        // Prevent multiple snaps in the same frame
        if (grab.gameObject.TryGetComponent<SnapMarker>(out _))
            return;
        grab.gameObject.AddComponent<SnapMarker>();
        // Disable grabbing
        grab.enabled = false;

        // Stop physics
        Rigidbody rb = grab.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Snap to exact transform
        grab.transform.position = snapPosition.position;
        grab.transform.rotation = snapPosition.rotation;

        // After snapping succeeds: add marker to root object
        var root = rb != null ? rb.gameObject : grab.gameObject;
        if (root.GetComponent<SnapMarker>() == null)
            root.AddComponent<SnapMarker>();

        outlineIndicator.SetActive(false);

        // Mark as filled
        isFilled = true;
        GetComponent<Collider>().enabled = false;
        print("filled");
        // Notify manager
        print(manager);
        if (manager != null)
            manager.CheckSnapPoints();
    }
}


