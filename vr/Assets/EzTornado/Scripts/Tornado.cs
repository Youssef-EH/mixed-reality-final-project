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


        // Ensure the object has a Rigidbody
        if (other.CompareTag("HOUSE"))
        {
            AddRigidbodiesToChildren(other.transform);
            return; // we stop here, don't apply tornado forces yet
        }
        else if(other.CompareTag("NATUUR"))
        {
            if(other.GetComponent<Rigidbody>() == null) 
                other.gameObject.AddComponent<Rigidbody>();
        }
        if (other.CompareTag("OBJ") || other.CompareTag("NATUUR"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb == null)
                return;
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
    void RemoveParentRigidbodies(Transform t)
    {
        Transform current = t.parent;
        while (current != null)
        {
            Rigidbody rb = current.GetComponent<Rigidbody>();
            if (rb != null)
            {
                GameObject.Destroy(rb);
            }
            current = current.parent;
        }
    }
    void AddRigidbodiesToChildren(Transform houseRoot)
    {
        foreach (Transform child in houseRoot.GetComponentsInChildren<Transform>())
        {
            if (child == houseRoot) continue; // skip the root object

            // Skip if already has RB
            if (child.GetComponent<Rigidbody>() != null)
                continue;

            // Remove any RBs on parent chain to prevent auto-deletion
            RemoveParentRigidbodies(child);

            child.gameObject.AddComponent<Rigidbody>();
            Destroy(child.gameObject, 7f);
        }
    }
}
