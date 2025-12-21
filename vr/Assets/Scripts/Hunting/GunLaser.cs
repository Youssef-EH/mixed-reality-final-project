using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Hunting
{
    [RequireComponent(typeof(LineRenderer))]
    public class GunLaser : MonoBehaviour
    {
        public Transform muzzle;
        public float range = 50f;
        public LayerMask hitMask = ~0;

        LineRenderer lr;
        XRGrabInteractable grab;
        bool held;

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            grab = GetComponent<XRGrabInteractable>();

            // basic line setup
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.startColor = Color.red;
            lr.endColor = Color.red;

            // Only show laser while held
            lr.enabled = false;

            grab.selectEntered.AddListener(_ => { held = true; lr.enabled = true; });
            grab.selectExited.AddListener(_ => { held = false; lr.enabled = false; });
        }

        void Update()
        {
            if (!held || muzzle == null) return;

            Vector3 start = muzzle.position;
            Vector3 dir = muzzle.forward;

            Vector3 end = start + dir * range;

            if (Physics.Raycast(start, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
                end = hit.point;

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }
}