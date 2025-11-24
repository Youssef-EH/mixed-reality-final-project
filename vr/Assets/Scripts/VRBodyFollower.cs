using UnityEngine;

public class VRBodyFollower : MonoBehaviour
{
    [Header("References")]
    public Transform head;        // Main Camera
    public Animator animator;     // Leonard's Animator

    [Header("Offsets")]
    public float heightOffset = -1.7f;
    public float forwardOffset = 0.0f;

    [Header("Animation")]
    public string speedParam = "Speed";
    public float walkThreshold = 0.1f;

    private Vector3 lastPos;

    void Start()
    {
        if (head == null)
            Debug.LogWarning("VRBodyFollower: Head not assigned.");

        var flat = new Vector3(transform.position.x, 0f, transform.position.z);
        lastPos = flat;
    }

    void LateUpdate()
    {
        if (head == null) return;

        // 1. Position body under the head (with offset)
        Vector3 headPos = head.position;
        Vector3 bodyPos = headPos;

        // FLATTEN forward so Y = 0 (no vertical movement from looking up/down)
        Vector3 flatForward = new Vector3(head.forward.x, 0f, head.forward.z);
        if (flatForward.sqrMagnitude > 0.0001f)
            flatForward.Normalize();

        bodyPos.y += heightOffset;                 // drop to feet
        bodyPos += flatForward * forwardOffset;    // use FLAT forward here

        transform.position = bodyPos;

        // 2. Rotate only with head yaw (same flatForward)
        if (flatForward.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
        }

        // 3. Drive animation based on horizontal speed (can stay as you had it)
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            Vector3 flat = new Vector3(transform.position.x, 0f, transform.position.z);
            float speed = (flat - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            animator.SetFloat(speedParam, speed);

            lastPos = flat;
        }
    }

}
