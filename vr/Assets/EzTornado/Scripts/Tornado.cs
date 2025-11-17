using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Tornado : MonoBehaviour
{
    [Header("Tornado Settings")]
    public Transform tornadoCenter;
    public float pullForce;
    public float rotationForce; // Optional, for spinning effect
    public float upward;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("OBJ")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;
        // Direction toward tornado center
        Vector3 toCenter = (tornadoCenter.position - other.transform.position).normalized;

        // Add pulling force
        Vector3 lift = Vector3.up * upward * Time.fixedDeltaTime;
        rb.AddForce(toCenter * pullForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        rb.AddForce(lift, ForceMode.Acceleration);

        // Optional swirl (makes the object orbit the tornado)
        Vector3 swirl = Vector3.Cross(Vector3.up, toCenter) * rotationForce;
        rb.AddForce(swirl * Time.fixedDeltaTime, ForceMode.Acceleration);
    }
}
