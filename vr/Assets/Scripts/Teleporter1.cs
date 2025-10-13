using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class Teleporter1 : MonoBehaviour
{
    public Transform targetTeleportLocation;
    public GameObject effectPrefab;
    public AudioClip teleportSound;
    private AudioSource audioSource;
    private ActionBasedContinuousMoveProvider moveProvider;
    public GameObject[] objectsToDisableDuringTeleport;

    public Camera portalCamera; // Camera at the destination
    public Renderer portalScreen; // Screen to display the portal view

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set up the portal camera render texture
        if (portalCamera != null && portalScreen != null)
        {
            RenderTexture portalTexture = new RenderTexture(Screen.width, Screen.height, 24);
            portalCamera.targetTexture = portalTexture;
            portalScreen.material.mainTexture = portalTexture;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the ActionBasedContinuousMoveProvider on the XR Rig
            moveProvider = other.GetComponentInParent<ActionBasedContinuousMoveProvider>();

            if (moveProvider != null)
            {
                StartCoroutine(TeleportPlayer(other.gameObject));
            }
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        // Disable specified GameObjects
        foreach (var obj in objectsToDisableDuringTeleport)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Disable movement
        if (moveProvider != null) moveProvider.enabled = false;

        // Play the teleport effect and sound
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
        }
        if (teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // Teleport the player to the target location
        player.transform.position = targetTeleportLocation.position;
        player.transform.rotation = targetTeleportLocation.rotation;

        // Wait for a frame to ensure the player is in the correct position
        yield return null;

        // Re-enable specified GameObjects
        foreach (var obj in objectsToDisableDuringTeleport)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Re-enable movement
        if (moveProvider != null) moveProvider.enabled = true;
    }
}