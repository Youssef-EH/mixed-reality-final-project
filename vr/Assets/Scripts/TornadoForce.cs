
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TornadoForce : MonoBehaviour
{
    [Header("Tornado shape")]
    public float radius = 20f;
    public float height = 40f; // vertical influence
    public float coreRadius = 2f; // strongest core

    [Header("Forces")]
    public float maxSuction = 500f;      // inward pull
    public float maxTangential = 1000f;   // spin force
    public float maxUpward = 80f;        // lift
    public float liftFalloff = 2f;        // how fast lift falls off with height

    [Header("Layer / Filtering")]
    public LayerMask affectedLayers = ~0; // which layers to affect
    public bool use2DDistance = false;

    void Reset()
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = radius;
    }

    void OnValidate()
    {
        var sc = GetComponent<SphereCollider>();
        if (sc) { sc.radius = radius; sc.isTrigger = true; }
    }
    //void OnTriggerEnter(Collider other)
    //{
    //    house.BreakHouse();
    //    BreakablePart breakable = other.GetComponent<BreakablePart>();
    //    if (breakable)
    //    {
    //        breakable.Break(transform.position, 100f, 1f);
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        var hb = other.GetComponentInParent<HouseBreakController>();
        if (hb != null) hb.BreakHouse();
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("TORNADO HIT: " + other.name);
        if (((1 << other.gameObject.layer) & affectedLayers) == 0) return;
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        Vector3 localPos = other.transform.position - transform.position;
        // compute normalized vertical position (0 at base -> 1 at top)
        float verticalFactor = Mathf.InverseLerp(0, height, Mathf.Clamp(localPos.y + height * 0.5f, 0, height));
        // radial distance ignoring height if desired
        Vector3 toCenter = new Vector3(localPos.x, 0, localPos.z);
        float dist = toCenter.magnitude;
        float distNorm = Mathf.InverseLerp(coreRadius, radius, dist);
        float suction = Mathf.Lerp(maxSuction, 0, distNorm);
        float tangential = Mathf.Lerp(maxTangential, 0, distNorm);
        float upward = Mathf.Lerp(maxUpward, 0, Mathf.Pow(distNorm, liftFalloff)) * (0.5f + verticalFactor);

        // inward force (towards central axis)
        Vector3 inwardDir = (new Vector3(-localPos.x, 0, -localPos.z)).normalized;
        Vector3 inward = inwardDir * suction * Time.fixedDeltaTime;

        // tangential (spin) force: perpendicular to inwardDir (use cross)
        Vector3 up = Vector3.up;
        Vector3 tangentialDir = Vector3.Cross(inwardDir, up).normalized;
        Vector3 tangentialForce = tangentialDir * tangential * Time.fixedDeltaTime;

        // upward lift
        Vector3 lift = Vector3.up * upward * Time.fixedDeltaTime;

        // apply as force (use ForceMode.Acceleration or Force)
        rb.AddForce(inward + tangentialForce + lift, ForceMode.Acceleration);

        // optional: add torque for chaotic spin
        rb.AddTorque(Random.insideUnitSphere * (tangential * 0.005f), ForceMode.Acceleration);
    }
}
