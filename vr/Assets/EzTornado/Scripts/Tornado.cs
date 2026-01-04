using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Tornado : MonoBehaviour
{
    [Header("Tornado Settings")]
    public Transform tornadoCenter;
    public float pullForce;
    public float rotationForce; // Optional, for spinning effect
    public float upward;

    HashSet<Transform> fracturedHouses = new HashSet<Transform>();

    private void OnTriggerStay(Collider other)
    {


        // Ensure the object has a Rigidbody
        if (other.CompareTag("HOUSE"))
        {
            if (!fracturedHouses.Contains(other.transform))
            {
                fracturedHouses.Add(other.transform);
                AddRigidbodiesToChildren(other.transform);
            }
            return;
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
            if (child == houseRoot) continue;

            // Skip tiny debris
            Renderer r = child.GetComponent<Renderer>();
            if (r != null && r.bounds.size.magnitude < 0.5f)
            {
                Destroy(child.gameObject);
                continue;
            }

            // 50% chance to skip
            if (Random.value < 0.6f)
            {
                Destroy(child.gameObject);
                continue;
            }

            if (child.GetComponent<Rigidbody>() != null)
                continue;

            RemoveParentRigidbodies(child);

            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
            rb.mass = 2f;
            rb.drag = 0.5f;
        }
    }
}
