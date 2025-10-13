using UnityEngine;

public class DebugFlyCam : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float fastMultiplier = 2.5f;
    public float lookSensitivity = 2.5f;
    public bool holdRightMouseToLook = true;

    float yaw, pitch;

    void Start()
    {
        var rot = transform.rotation.eulerAngles;
        yaw = rot.y; pitch = rot.x;
        Cursor.lockState = holdRightMouseToLook ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = holdRightMouseToLook;
    }

    void Update()
    {
        // Mouse look
        if (!holdRightMouseToLook || Input.GetMouseButton(1))
        {
            yaw   += Input.GetAxis("Mouse X") * lookSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * lookSensitivity;
            pitch = Mathf.Clamp(pitch, -85f, 85f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            if (holdRightMouseToLook) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        }
        else if (holdRightMouseToLook && Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        }

        // Movement (WASD), up/down (Q/E)
        Vector3 dir = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            (Input.GetKey(KeyCode.E) ? 1f : 0f) + (Input.GetKey(KeyCode.Q) ? -1f : 0f),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastMultiplier : 1f);
        transform.position += transform.TransformDirection(dir) * (speed * Time.deltaTime);
    }
}