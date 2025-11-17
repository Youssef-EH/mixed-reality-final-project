using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DebugFPController : MonoBehaviour
{
    public Camera viewCam;
    public float cameraHeight = 0.9f;     // meters above feet
    public float moveSpeed = 4f;
    public float fastMultiplier = 2f;
    public float lookSensitivityDegPerSec = 120f;
    public bool lockCursor = true;

    CharacterController cc;
    float yaw, pitch;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!viewCam) viewCam = Camera.main;
        yaw = transform.eulerAngles.y;
        pitch = 0f;

        if (lockCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    }

    void Update()
    {
        // keep camera locked to head position under the player
        if (viewCam)
        {
            viewCam.transform.localPosition = new Vector3(0f, cameraHeight, 0f);
        }

        // mouse look
        float mx = Input.GetAxis("Mouse X") * lookSensitivityDegPerSec * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * lookSensitivityDegPerSec * Time.deltaTime;

        yaw   += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -85f, 85f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (viewCam) viewCam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // WASD + Q/E up/down
        Vector3 input = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            (Input.GetKey(KeyCode.E) ? 1 : 0) + (Input.GetKey(KeyCode.Q) ? -1 : 0),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastMultiplier : 1f);
        Vector3 move = transform.TransformDirection(input) * speed;

        cc.Move(move * Time.deltaTime);
    }

    void OnDisable(){ Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
}