// HouseBreakController.cs
using UnityEngine;

public class HouseBreakController : MonoBehaviour
{
    public BreakablePart[] parts;
    public float baseExplosionForce = 100f;
    public Transform tornadoRoot;      // optionally point to tornado position
    public float breakDelay = 0f;

    void Start()
    {
        if (parts == null || parts.Length == 0)
            parts = GetComponentsInChildren<BreakablePart>(true);
    }

    // Call this to break the house (can from collision with tornado or when health hits 0)
    public void BreakHouse()
    {
        foreach(var part in parts)
        {
            var joint = part.GetComponent<Joint>();
            joint.breakForce = 50;
            joint.breakTorque = 50;
        }
        StartCoroutine(BreakRoutine());
    }

    System.Collections.IEnumerator BreakRoutine()
    {
        yield return new WaitForSeconds(breakDelay);

        Vector3 origin = tornadoRoot ? tornadoRoot.position : transform.position;
        foreach (var p in parts)
        {
            float force = baseExplosionForce * (1f + Random.value);
            p.Break(origin, force, Random.Range(0.25f, 1.2f));
        }
    }
}
