using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Hunting
{
    public class HuntingOutlineGuidance : MonoBehaviour
    {
        [Header("References")]
        public XRGrabInteractable gunGrab;
        public Outline gunOutline;
        public AnimalHealth deerHealth;
        public Outline deerOutline;           

        [Header("Behavior")]
        public bool removeGunOutlineAfterKill = true;

        private bool deerKilled;

        void Reset()
        {
            gunGrab = FindFirstObjectByType<XRGrabInteractable>();
        }

        void OnEnable()
        {
            if (gunGrab != null)
            {
                gunGrab.selectEntered.AddListener(OnGunGrabbed);
                gunGrab.selectExited.AddListener(OnGunReleased);
            }

            if (deerHealth != null)
            {
                deerHealth.Killed += OnDeerKilled;
            }
        }

        void OnDisable()
        {
            if (gunGrab != null)
            {
                gunGrab.selectEntered.RemoveListener(OnGunGrabbed);
                gunGrab.selectExited.RemoveListener(OnGunReleased);
            }

            if (deerHealth != null)
            {
                deerHealth.Killed -= OnDeerKilled;
            }
        }

        void Start()
        {
            // Gun highlighted, deer not highlighted
            SetGunOutline(true);
            SetDeerOutline(false);
        }

        private void OnGunGrabbed(SelectEnterEventArgs args)
        {
            if (deerKilled) return;

            // Once gun is held: gun outline off, deer outline on (red)
            SetGunOutline(false);
            SetDeerOutline(true);
        }

        private void OnGunReleased(SelectExitEventArgs args)
        {
            if (deerKilled) return;

            // If gun dropped: deer outline off, gun outline back on
            SetDeerOutline(false);
            SetGunOutline(true);
        }

        private void OnDeerKilled(AnimalHealth deer)
        {
            deerKilled = true;
            SetDeerOutline(false);
            
            if (removeGunOutlineAfterKill)
                SetGunOutline(false);
        }

        private void SetGunOutline(bool on)
        {
            if (gunOutline != null)
                gunOutline.enabled = on;
        }

        private void SetDeerOutline(bool on)
        {
            if (deerOutline != null)
                deerOutline.enabled = on;
        }
    }
}
