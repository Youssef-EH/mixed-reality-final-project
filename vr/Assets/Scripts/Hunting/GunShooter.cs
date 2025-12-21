using Hunting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GunShooter : MonoBehaviour
{
    public Transform muzzle;
    public float range = 50f;
    public LayerMask hitMask = ~0;

    XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.activated.AddListener(OnActivated);
    }

    void OnDestroy()
    {
        if (grab != null)
            grab.activated.RemoveListener(OnActivated);
    }

    void OnActivated(ActivateEventArgs args)
    {
        if (muzzle == null) return;

        Ray ray = new Ray(muzzle.position, muzzle.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"[Gun] Hit: {hit.collider.name}");

            var animal = hit.collider.GetComponentInParent<AnimalHealth>();
            if (animal != null)
                animal.KillOneShot();
        }
        else
        {
            Debug.Log("[Gun] Miss");
        }
    }
}