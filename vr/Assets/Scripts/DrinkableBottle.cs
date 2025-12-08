using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrinkableBottle : MonoBehaviour
{
    [Header("References")]
    public GameObject waterInside;          // Child mesh with water
    public AudioSource drinkSound;          // AudioSource on the bottle
    public XRGrabInteractable grabInteract; // XR Grab Interactable on this object
    public Transform head;                  // Player head (Main Camera)

    [Header("Settings")]
    [Tooltip("Max distance from head to allow drinking.")]
    public float drinkDistance = 0.3f;

    [Tooltip("Seconds before another drink is allowed.")]
    public float drinkCooldown = 1.0f;

    private bool isEmpty = false;
    private bool isOnCooldown = false;

    private void OnEnable()
    {
        if (grabInteract == null)
            grabInteract = GetComponent<XRGrabInteractable>();

        if (grabInteract != null)
            grabInteract.activated.AddListener(OnActivated);
    }

    private void OnDisable()
    {
        if (grabInteract != null)
            grabInteract.activated.RemoveListener(OnActivated);
    }

    // Called when trigger/activate is pressed while holding the bottle
    private void OnActivated(ActivateEventArgs args)
    {
        TryDrink();
    }

    private void TryDrink()
    {
        if (isEmpty || isOnCooldown)
            return;

        if (waterInside == null || head == null)
        {
            Debug.LogWarning("DrinkableBottle: Missing waterInside or head reference.", this);
            return;
        }

        // Check distance from bottle to head
        float distance = Vector3.Distance(transform.position, head.position);
        if (distance > drinkDistance)
        {
            // Too far from mouth, do nothing
            Debug.Log($"DrinkableBottle: Too far from head ({distance:F2} > {drinkDistance}).", this);
            return;
        }

        // Play sound
        if (drinkSound != null)
        {
            drinkSound.Play();
        }
        else
        {
            Debug.LogWarning("DrinkableBottle: No drinkSound assigned.", this);
        }

        // Make bottle look empty
        waterInside.SetActive(false);
        isEmpty = true;

        Debug.Log("DrinkableBottle: Drink triggered, bottle is now empty.", this);

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(drinkCooldown);
        isOnCooldown = false;
    }
}
