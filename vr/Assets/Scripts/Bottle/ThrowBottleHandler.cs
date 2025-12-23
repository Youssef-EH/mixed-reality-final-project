using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ThrowBottleHandler : MonoBehaviour
{
    public RainController rainController;
    public ImageListPopup imageListPopup;
    public GameObject popupParent;
    public XRGrabInteractable bottle;
    void Start()
    {
        bottle.gameObject.TryGetComponent<DrinkableBottle>(out _);
        bottle.selectExited.AddListener(OnBottleThrown);
    }
    //weerverandering en fotos
    private void OnBottleThrown(SelectExitEventArgs args)
    {
        XRGrabInteractable thrownBottle = args.interactableObject as XRGrabInteractable;

        if(!thrownBottle.gameObject.TryGetComponent<BottleThrownMarker>(out _))
        {
            thrownBottle.gameObject.AddComponent<BottleThrownMarker>();
            rainController.ToggleRain();
            StartCoroutine(imageListPopup.ShowCanvasesCoroutine(popupParent));
        }
    }
    private void OnDestroy()
    {
        bottle.selectExited.RemoveListener(OnBottleThrown);
    }
}
public class BottleThrownMarker : MonoBehaviour { }