using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class StartWithBottle : MonoBehaviour
{
    [SerializeField] private XRBaseInteractor interactor;
    [SerializeField] private XRGrabInteractable bottleToGrab;
    [SerializeField] private InputActionReference selectActionReference;
    [SerializeField] private ThrowBottleHandler throwBottleHandler;

    private bool forceHold = true;
    private bool buttonPressed = false;

    private void Start()
    {
        interactor.startingSelectedInteractable = null;
        StartCoroutine(GrabBottleAfterDelay());
    }

    private IEnumerator GrabBottleAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        interactor.interactionManager.SelectEnter(interactor as IXRSelectInteractor, bottleToGrab);
    }

    private void Update()
    {
        // Check if the select button is actually being pressed
        bool isPressed = false;
        if (selectActionReference != null && selectActionReference.action != null)
        {
            isPressed = selectActionReference.action.ReadValue<float>() > 0.1f;
        }

        // If button was pressed and now released, stop forcing
        if (buttonPressed && !isPressed)
        {
            forceHold = false;
            throwBottleHandler.OnBottleThrown(bottleToGrab);
            Debug.Log("Button released - normal grab/drop enabled");
        }

        if (isPressed)
        {
            buttonPressed = true;
        }

        // Keep forcing the selection until user has pressed and released
        if (forceHold && interactor.hasSelection == false && bottleToGrab != null)
        {
            interactor.interactionManager.SelectEnter(interactor as IXRSelectInteractor, bottleToGrab);
        }
    }
}
