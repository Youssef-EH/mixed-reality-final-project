using UnityEngine;

public class WindmillRotation : MonoBehaviour
{
    public Transform wieken;
    public Transform sphere;
    public float rotationSpeed = -100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        wieken.transform.Rotate(0f, 0f, rotationSpeed);
        rotationSpeed++;
    }
}
