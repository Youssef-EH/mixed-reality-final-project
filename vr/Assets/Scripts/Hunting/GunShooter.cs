using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Hunting
{
    public class GunShooter : MonoBehaviour
    {
        public RainController rainController;
        public ImageListPopup imageListPopup;
        public GameObject popupParent;

        public Transform muzzle;
        public float range = 50f;
        public LayerMask hitMask = ~0;

        [Header("Audio")]
        public AudioSource shotAudio;

        private XRGrabInteractable grab;

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

        private void OnActivated(ActivateEventArgs args)
        {
            if (muzzle == null) return;

            // Play shot sound every time we fire
            if (shotAudio != null)
                shotAudio.Play();

            Ray ray = new Ray(muzzle.position, muzzle.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"[Gun] Hit: {hit.collider.name}");

                var health = hit.collider.GetComponentInParent<AnimalHealth>();
                if (health != null)
                {
                    health.KillOneShot();
                    rainController.ToggleRain();
                    StartCoroutine(imageListPopup.ShowCanvasesCoroutine(popupParent));
                }
            }
            else
            {
                Debug.Log("[Gun] Miss");
            }
        }
    }
}