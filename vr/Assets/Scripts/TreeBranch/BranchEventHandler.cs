using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BranchEventHandler : MonoBehaviour
{
    public RainController rainController;
    public XRGrabInteractable branch1;
    public XRGrabInteractable branch2;
    public XRGrabInteractable branch3;
    public ImageListPopup imageListPopup;
    public GameObject popupParent;

    private int branchesTaken = 0;
    private bool rainTriggered = false;
    void Start()
    {
        branch1.selectEntered.AddListener(OnBranchTaken);
        branch2.selectEntered.AddListener(OnBranchTaken);
        branch3.selectEntered.AddListener(OnBranchTaken);
    }

    private void OnBranchTaken(SelectEnterEventArgs args)
    {
        XRGrabInteractable takenBranch = args.interactableObject as XRGrabInteractable;

        if (!takenBranch.gameObject.TryGetComponent<BranchTakenMarker>(out _))
        {
            takenBranch.gameObject.AddComponent<BranchTakenMarker>();
            branchesTaken++;

            if (branchesTaken >= 3 && !rainTriggered)
            {
                rainTriggered = true;
                rainController.ToggleRain();
                StartCoroutine(imageListPopup.ShowCanvasesCoroutine(popupParent.gameObject));
            }
        }
    }
    private void OnDestroy()
    {
        branch1.selectEntered.RemoveListener(OnBranchTaken);
        branch2.selectEntered.RemoveListener(OnBranchTaken);
        branch3.selectEntered.RemoveListener(OnBranchTaken);
    }
}
public class BranchTakenMarker : MonoBehaviour { }
