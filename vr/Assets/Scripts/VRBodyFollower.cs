using System.Collections;
using UnityEngine;

public class VRBodyFollower : MonoBehaviour
{
    [Header("References")]
    public Transform head;
    public Animator animator;

    [Header("Offsets")]
    public float heightOffset = 1.7f;
    public float forwardOffset = 0f;
    public float footOffset = 0f;
    public LayerMask groundMask = ~0;

    [Header("Animation")]
    public string speedParam = "Speed";
    public float walkThreshold = 0.1f;

    private Vector3 _lastFlatPos;
    private int _frameCounter;
    private Mesh _bakeMesh;

    private bool _dynamicLocked;

    void Start()
    {
        heightOffset = Mathf.Abs(heightOffset);
        _bakeMesh = new Mesh();
        StartCoroutine(InitRoutine());
        if (animator == null) Debug.LogWarning("VRBodyFollower: Animator not assigned.");
    }

    private IEnumerator InitRoutine()
    {
        AssignHeadIfPossible();

        // let VR camera / bones pose for a couple frames
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        AssignHeadIfPossible(late: true);

        if (head == null)
        {
            Debug.LogWarning("VRBodyFollower: Head not assigned and no active Camera found. Script will be idle until a head is set.");
            yield break;
        }

        // final snap
        yield return new WaitForEndOfFrame();
        SnapToHead();

        _lastFlatPos = new Vector3(transform.position.x, 0f, transform.position.z);
        if (animator != null) animator.SetFloat(speedParam, 0f);
        Debug.Log("VRBodyFollower: initial snap complete. Body position set to " + transform.position);
    }

    void LateUpdate()
    {
        if (head == null) return;

        _frameCounter++;
        SnapToHead();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            Vector3 flat = new Vector3(transform.position.x, 0f, transform.position.z);
            float speed = (flat - _lastFlatPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            if (speed > walkThreshold)
            {
                _dynamicLocked = true;
            }
            animator.SetFloat(speedParam, speed);
            _lastFlatPos = flat;
        }
    }

    private void AssignHeadIfPossible(bool late = false)
    {
        if (head != null && head.gameObject.activeInHierarchy)
        {
            if (!late) Debug.Log("VRBodyFollower: using inspector head (active).");
            return;
        }

        if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
        {
            head = Camera.main.transform;
            if (late) Debug.Log("VRBodyFollower: late-assigned head = Camera.main");
            else Debug.Log("VRBodyFollower: assigned head = Camera.main");
            return;
        }

        var cams = Camera.allCameras;
        for (int i = 0; i < cams.Length; i++)
        {
            var c = cams[i];
            if (c != null && c.gameObject.activeInHierarchy)
            {
                head = c.transform;
                Debug.Log((late ? "VRBodyFollower: late-assigned head = " : "VRBodyFollower: assigned head = ") + c.name);
                return;
            }
        }

        if (!late)
            Debug.LogWarning("VRBodyFollower: no active Camera found yet. Will wait a few frames.");
    }

    private void SnapToHead()
    {
        Vector3 bodyPos = transform.position;

        // --- Y: ground only ---
        RaycastHit hit;
        if (Physics.Raycast(head.position, Vector3.down, out hit, 10f, groundMask))
        {
            bodyPos.y = hit.point.y + footOffset;
        }

        // --- XZ: follow head ---
        bodyPos.x = head.position.x;
        bodyPos.z = head.position.z;

        // --- rotation ---
        Vector3 flatForward = new Vector3(head.forward.x, 0f, head.forward.z);
        if (flatForward.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
        }

        transform.position = bodyPos;
    }

    void OnDestroy()
    {
        if (_bakeMesh != null)
        {
            Destroy(_bakeMesh);
            _bakeMesh = null;
        }
    }
}
