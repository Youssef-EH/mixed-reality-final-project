// csharp
using System.Collections;
using UnityEngine;

public class VRBodyFollower : MonoBehaviour
{
    [Header("References")]
    public Transform head;
    public Animator animator;

    [Header("Offsets")]
    public float heightOffset = 1.7f;
    public float forwardOffset = 0.0f;
    public float footOffset = 0.0f; // inspector/static fallback
    public LayerMask groundMask = ~0;

    [Header("Animation")]
    public string speedParam = "Speed";
    public float walkThreshold = 0.1f;

    [Header("Dynamic foot correction")]
    public bool enableDynamicFootCorrection = true; // bake skinned meshes to follow animations
    [Tooltip("How many frames between expensive skinned-mesh bakes (1 = every frame)")]
    public int dynamicCorrectionInterval = 2;

    private Vector3 lastPos;
    private bool footOffsetAutoApplied = false;

    // dynamic correction state
    private int _frameCounter = 0;
    private float _dynamicOffset = 0f; // distance from root to current lowest renderer point
    private Mesh _bakeMesh;

    void Start()
    {
        heightOffset = Mathf.Abs(heightOffset);
        StartCoroutine(EnsureHeadAndInit());
        if (animator == null) Debug.LogWarning("VRBodyFollower: Animator not assigned.");
    }

    private IEnumerator EnsureHeadAndInit()
    {
        if (head != null && head.gameObject.activeInHierarchy)
        {
            Debug.Log("VRBodyFollower: using inspector head (active).");
        }
        else
        {
            if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
            {
                head = Camera.main.transform;
                Debug.Log("VRBodyFollower: assigned head = Camera.main");
            }
            else
            {
                Camera[] cams = Camera.allCameras;
                Camera found = null;
                for (int i = 0; i < cams.Length; i++)
                {
                    if (cams[i] != null && cams[i].gameObject.activeInHierarchy)
                    {
                        found = cams[i];
                        break;
                    }
                }

                if (found != null)
                {
                    head = found.transform;
                    Debug.Log("VRBodyFollower: assigned head = first active Camera found: " + head.name);
                }
                else
                {
                    Debug.LogWarning("VRBodyFollower: no active Camera found yet. Will wait a few frames.");
                }
            }
        }

        // give XR systems / animator a couple frames to settle
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (head == null || head.gameObject.activeInHierarchy == false)
        {
            if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
            {
                head = Camera.main.transform;
                Debug.Log("VRBodyFollower: late-assigned head = Camera.main");
            }
            else
            {
                Camera[] cams = Camera.allCameras;
                foreach (var cam in cams)
                {
                    if (cam != null && cam.gameObject.activeInHierarchy)
                    {
                        head = cam.transform;
                        Debug.Log("VRBodyFollower: late-assigned head = active camera: " + cam.name);
                        break;
                    }
                }
            }
        }

        if (head == null)
        {
            Debug.LogWarning("VRBodyFollower: Head not assigned and no active Camera found. Script will be idle until a head is set.");
            yield break;
        }

        // If user left footOffset at 0, run a more accurate auto-detect that bakes skinned meshes.
        if (Mathf.Approximately(footOffset, 0f) && !footOffsetAutoApplied)
        {
            yield return StartCoroutine(AutoDetectFootOffset());
            if (footOffsetAutoApplied)
                Debug.Log("VRBodyFollower: auto-detected footOffset = " + footOffset.ToString("F3"));
            else
                Debug.Log("VRBodyFollower: no renderers found for auto-detect; leaving footOffset = 0");
        }

        // initialize dynamic offset to static or computed value
        _dynamicOffset = Mathf.Max(0f, footOffset);

        // final wait for camera pose, then snap once to avoid initial clipping
        yield return new WaitForEndOfFrame();
        SnapToHead();

        lastPos = new Vector3(transform.position.x, 0f, transform.position.z);
        if (animator != null) animator.SetFloat(speedParam, 0f);
        Debug.Log("VRBodyFollower: initial snap complete. Body position set to " + transform.position);
    }

    void LateUpdate()
    {
        if (head == null) return;

        // update frame counter for dynamic correction sampling
        _frameCounter++;

        SnapToHead();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            Vector3 flat = new Vector3(transform.position.x, 0f, transform.position.z);
            float speed = (flat - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            animator.SetFloat(speedParam, speed);
            lastPos = flat;
        }
    }

    private void SnapToHead()
    {
        Vector3 headPos = head.position;
        Vector3 bodyPos = headPos;

        // vertical: place body so feet sit at (headY - heightOffset)
        bodyPos.y = headPos.y - heightOffset;

        // raycast down from head to snap to ground if available
        RaycastHit hit;
        if (Physics.Raycast(headPos, Vector3.down, out hit, heightOffset + 2f, groundMask))
        {
            // if dynamic correction enabled, occasionally bake SkinnedMeshRenderers to compute the
            // current lowest renderer Y, then compute a corrected offset = rootY - minY.
            if (enableDynamicFootCorrection && animator != null)
            {
                // sample only every N frames to reduce cost
                if (_frameCounter % Mathf.Max(1, dynamicCorrectionInterval) == 0)
                {
                    float minY;
                    if (TryComputeCurrentLowestRendererY(out minY))
                    {
                        // distance from current root world Y to lowest renderer point
                        float needed = transform.position.y - minY;
                        _dynamicOffset = Mathf.Max(0f, needed);
                    }
                }

                // apply dynamic offset so the lowest renderer point sits at the hit point
                bodyPos.y = hit.point.y + _dynamicOffset;
            }
            else
            {
                // static inspector/simple auto-detected offset path
                bodyPos.y = hit.point.y + footOffset;
            }
        }

        Vector3 flatForward = new Vector3(head.forward.x, 0f, head.forward.z);
        if (flatForward.sqrMagnitude > 0.0001f) flatForward.Normalize();

        bodyPos += flatForward * forwardOffset;
        transform.position = bodyPos;

        if (flatForward.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
    }

    // Accurate auto-detect: wait a couple frames, bake skinned meshes if present,
    // compute lowest world-space Y across all renderers, and set footOffset so lowest point sits at ground.
    private IEnumerator AutoDetectFootOffset()
    {
        // give animator/skinned meshes one more frame to update bones
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        float minY = float.MaxValue;

        // First handle SkinnedMeshRenderers by baking their current posed mesh.
        var skinned = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        Mesh bakeMesh = null;
        if (skinned != null && skinned.Length > 0)
        {
            bakeMesh = new Mesh();
            foreach (var s in skinned)
            {
                if (s == null) continue;
                s.BakeMesh(bakeMesh);
                var verts = bakeMesh.vertices;
                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 world = s.transform.TransformPoint(verts[i]);
                    minY = Mathf.Min(minY, world.y);
                }
                bakeMesh.Clear();
            }
        }

        // Fallback: use Renderer.bounds for MeshRenderers / other renderers
        var rends = GetComponentsInChildren<Renderer>(true);
        if (rends != null && rends.Length > 0)
        {
            foreach (var r in rends)
            {
                if (r == null) continue;
                minY = Mathf.Min(minY, r.bounds.min.y);
            }
        }

        if (minY == float.MaxValue)
        {
            // no renderers found
            yield break;
        }

        // distance from current root world Y down to lowest renderer point
        float needed = transform.position.y - minY;
        needed = Mathf.Max(0f, needed); // non-negative
        footOffset = needed;
        footOffsetAutoApplied = true;

        yield break;
    }

    // Compute current lowest renderer Y using baked skinned meshes and Renderer.bounds fallback.
    // Returns true if any renderer found.
    private bool TryComputeCurrentLowestRendererY(out float outMinY)
    {
        outMinY = float.MaxValue;

        // create bake mesh if needed (reused to avoid allocations)
        if (_bakeMesh == null) _bakeMesh = new Mesh();

        var skinned = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (skinned != null && skinned.Length > 0)
        {
            foreach (var s in skinned)
            {
                if (s == null) continue;
                s.BakeMesh(_bakeMesh);
                var verts = _bakeMesh.vertices;
                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 world = s.transform.TransformPoint(verts[i]);
                    outMinY = Mathf.Min(outMinY, world.y);
                }
                _bakeMesh.Clear();
            }
        }

        var rends = GetComponentsInChildren<Renderer>(true);
        if (rends != null && rends.Length > 0)
        {
            foreach (var r in rends)
            {
                if (r == null) continue;
                outMinY = Mathf.Min(outMinY, r.bounds.min.y);
            }
        }

        if (outMinY == float.MaxValue) return false;
        return true;
    }
}
