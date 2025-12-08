using Unity.VRTemplate;
using UnityEngine;
using UnityEngine;
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
        if (isFilled)
            return; 
        XRGrabInteractable grab = other.GetComponentInParent<XRGrabInteractable>();
        if (grab == null || grab.GetComponent<SnapMarker>() != null)
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

        outlineIndicator.SetActive(false);

        // Mark as filled
        isFilled = true;
        print("filled");
        // Notify manager
        if (manager != null)
            manager.CheckSnapPoints();
    }
}


