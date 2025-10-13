using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class Teleporter1 : MonoBehaviour
{
    [Header("Destination")]
    public Transform targetTeleportLocation;

    [Header("FX (optional)")]
    public GameObject effectPrefab;
    public AudioClip teleportSound;

    [Header("Portal View (optional)")]
    public Camera portalCamera;     // camera that renders to the portal screen
    public Renderer portalScreen;   // mesh renderer of the quad

    [Header("Tuning")]
    [Range(0f,1f)] public float minApproachDot = 0.25f; // require front-side entry
    public float cooldown = 0.25f;

    private AudioSource audioSource;
    private ActionBasedContinuousMoveProvider moveProvider; // may be null in debug
    private bool busy;

    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (portalCamera && portalScreen)
        {
            var rt = new RenderTexture(Screen.width, Screen.height, 24);
            portalCamera.targetTexture = rt;
            portalScreen.material.mainTexture = rt;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        Vector3 playerToPortal = (transform.position - root.position).normalized;
        float dot = Vector3.Dot(transform.forward, playerToPortal);
        if (dot < minApproachDot || busy) return;

        moveProvider = root.GetComponentInParent<ActionBasedContinuousMoveProvider>();
        StartCoroutine(TeleportPlayer(root.gameObject));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator TeleportPlayer(GameObject player)
    {
        busy = true;

        if (moveProvider) moveProvider.enabled = false;
        if (effectPrefab) Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
        if (teleportSound) audioSource.PlayOneShot(teleportSound);

        if (targetTeleportLocation)
            player.transform.SetPositionAndRotation(targetTeleportLocation.position, targetTeleportLocation.rotation);
        else
            Debug.LogWarning("[Portal] No targetTeleportLocation set.");

        yield return new WaitForSeconds(cooldown);

        if (moveProvider) moveProvider.enabled = true;
        busy = false;
    }
}
