// BreakablePart.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakablePart : MonoBehaviour
{
    [Header("Auto Rigidbody on break")]
    public float mass = 1f;
    public bool addRigidbodyOnBreak = true;
    public float randomTorque = 20f;

    Rigidbody rb;

    public void Break(Vector3 explosionOrigin, float explosionForce, float upwardsModifier = 0.5f)
    {
        if (rb == null && addRigidbodyOnBreak)
        {
            rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.isKinematic = false;
        }

        // optional explosion impulse
        if (rb != null)
        {
            rb.AddExplosionForce(explosionForce, explosionOrigin, 10f, upwardsModifier, ForceMode.Impulse);
            rb.AddTorque(Random.onUnitSphere * randomTorque, ForceMode.Impulse);
        }

        // remove any fixed joints so pieces separate
        //var joints = GetComponents<Joint>();
        //foreach (var j in joints) j.breakForce=1;
        //foreach (var j in joints) j.breakTorque=1;
    }
}
