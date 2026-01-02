using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ThrowBottleHandler : MonoBehaviour
{
    public RainController rainController;
    public ImageListPopup imageListPopup;
    public GameObject popupParent;
    public XRGrabInteractable bottle;

    [SerializeField] private XRBaseInteractor interactor;
    [SerializeField] private InputActionReference selectAction;
    void Start()
    {
        bottle.gameObject.TryGetComponent<DrinkableBottle>(out _);
    }

    public void OnBottleThrown(XRGrabInteractable bottle)
    {
        XRGrabInteractable thrownBottle = bottle;

        if(!thrownBottle.gameObject.TryGetComponent<BottleThrownMarker>(out _))
        {
            thrownBottle.gameObject.AddComponent<BottleThrownMarker>();
            rainController.ToggleRain();
            StartCoroutine(imageListPopup.ShowCanvasesCoroutine(popupParent));
        }
    }
}
public class BottleThrownMarker : MonoBehaviour { }