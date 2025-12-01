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

    [Header("Dynamic foot correction")]
    public bool enableDynamicFootCorrection = true;
    [Tooltip("How many frames between expensive skinned-mesh bakes (1 = every frame)")]
    public int dynamicCorrectionInterval = 2;

    private Vector3 _lastFlatPos;
    private bool _footOffsetAutoApplied;
    private int _frameCounter;
    private float _dynamicOffset;
    private Mesh _bakeMesh;

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

        // If user left footOffset at ~0, run accurate auto-detect (bakes skinned meshes).
        if (Mathf.Approximately(footOffset, 0f) && !_footOffsetAutoApplied)
        {
            yield return StartCoroutine(AutoDetectFootOffset());
            if (_footOffsetAutoApplied)
                Debug.Log("VRBodyFollower: auto-detected footOffset = " + footOffset.ToString("F3"));
            else
                Debug.Log("VRBodyFollower: no renderers found for auto-detect; leaving footOffset = 0");
        }

        // initialize dynamic offset to static or computed value, and attempt one immediate accurate sample
        _dynamicOffset = Mathf.Max(0f, footOffset);

        if (enableDynamicFootCorrection && animator != null)
        {
            float minY;
            if (TryComputeCurrentLowestRendererY(out minY))
            {
                // one-time accurate sample
                float needed = transform.position.y - minY;
                _dynamicOffset = Mathf.Max(_dynamicOffset, Mathf.Max(0f, needed));
            }
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
        Vector3 headPos = head.position;
        Vector3 bodyPos = headPos;

        // apply height offset
        bodyPos.y = headPos.y - heightOffset;

        RaycastHit hit;
        if (Physics.Raycast(headPos, Vector3.down, out hit, heightOffset + 2f, groundMask))
        {
            if (enableDynamicFootCorrection && animator != null)
            {
                if (_frameCounter % Mathf.Max(1, dynamicCorrectionInterval) == 0)
                {
                    float minY;
                    if (TryComputeCurrentLowestRendererY(out minY))
                    {
                        float needed = transform.position.y - minY;
                        _dynamicOffset = Mathf.Max(_dynamicOffset, Mathf.Max(0f, needed));
                    }
                }

                bodyPos.y = hit.point.y + _dynamicOffset;
            }
            else
            {
                bodyPos.y = hit.point.y + footOffset;
            }
        }

        Vector3 flatForward = new Vector3(head.forward.x, 0f, head.forward.z);
        if (flatForward.sqrMagnitude > 0.0001f)
        {
            flatForward.Normalize();
            bodyPos += flatForward * forwardOffset;
            transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
        }

        transform.position = bodyPos;
    }

    // Auto-detect foot offset by baking skinned meshes and finding lowest Y.
    private IEnumerator AutoDetectFootOffset()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        float minY;
        if (!TryComputeCurrentLowestRendererY(out minY))
            yield break; // no renderers

        float needed = transform.position.y - minY;
        needed = Mathf.Max(0f, needed);
        footOffset = needed;
        _footOffsetAutoApplied = true;
        yield break;
    }

    // Compute current lowest renderer Y using baked skinned meshes and Renderer.bounds fallback.
    private bool TryComputeCurrentLowestRendererY(out float outMinY)
    {
        outMinY = float.MaxValue;
        if (_bakeMesh == null) _bakeMesh = new Mesh();

        var skinned = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (skinned != null && skinned.Length > 0)
        {
            for (int si = 0; si < skinned.Length; si++)
            {
                var s = skinned[si];
                if (s == null) continue;
                s.BakeMesh(_bakeMesh);
                var verts = _bakeMesh.vertices;
                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 world = s.transform.TransformPoint(verts[i]);
                    if (world.y < outMinY) outMinY = world.y;
                }
                _bakeMesh.Clear();
            }
        }

        var rends = GetComponentsInChildren<Renderer>(true);
        if (rends != null && rends.Length > 0)
        {
            for (int ri = 0; ri < rends.Length; ri++)
            {
                var r = rends[ri];
                if (r == null) continue;
                if (r.bounds.min.y < outMinY) outMinY = r.bounds.min.y;
            }
        }

        return outMinY != float.MaxValue;
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
