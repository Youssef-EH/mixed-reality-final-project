using UnityEngine;

public class AutoJoint : MonoBehaviour
{
    public float breakForce = 100f;
    public float breakTorque = 100f;

    void Start()
    {
        // find closest piece below
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            Rigidbody rbBelow = hit.collider.attachedRigidbody;
            if (rbBelow != null)
            {
                FixedJoint joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = rbBelow;
                joint.breakForce = breakForce;
                joint.breakTorque = breakTorque;
            }
        }
    }
}
