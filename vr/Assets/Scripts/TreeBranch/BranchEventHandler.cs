using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BranchEventHandler : MonoBehaviour
{
    public RainController rainController;
    public XRGrabInteractable branch1;
    public XRGrabInteractable branch2;
    public XRGrabInteractable branch3;
    public ImageListPopup imageListPopup;
    public GameObject popupParent;

    [Header("Audio (Branch Pull Once)")] public AudioClip branchPullClip;
    [Range(0f, 1f)] public float pullVolume = 1f;
    [Range(0f, 1f)] public float spatialBlend = 1f; // 1 = 3D, 0 = 2D

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
        if (takenBranch == null) return;

        // Only first time ever: no marker yet
        if (!takenBranch.gameObject.TryGetComponent<BranchTakenMarker>(out _))
        {
            takenBranch.gameObject.AddComponent<BranchTakenMarker>();
            branchesTaken++;

            // ✅ Play pull sound ONLY once (first time branch is taken)
            PlayBranchPullSoundOnce(takenBranch.gameObject);

            if (branchesTaken >= 1 && !rainTriggered) //zet naar 3 voor alle takken
            {
                rainTriggered = true;
                rainController.ToggleRain();
                StartCoroutine(imageListPopup.ShowCanvasesCoroutine(popupParent.gameObject));
            }
        }
    }

    private void PlayBranchPullSoundOnce(GameObject branchGO)
    {
        if (branchPullClip == null) return;

        // Put the AudioSource on the branch so it feels “in-world”.
        var src = branchGO.GetComponent<AudioSource>();
        if (src == null) src = branchGO.AddComponent<AudioSource>();

        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = spatialBlend;
        src.volume = pullVolume;

        src.PlayOneShot(branchPullClip);
    }

    private void OnDestroy()
    {
        branch1.selectEntered.RemoveListener(OnBranchTaken);
        branch2.selectEntered.RemoveListener(OnBranchTaken);
        branch3.selectEntered.RemoveListener(OnBranchTaken);
    }
}

public class BranchTakenMarker : MonoBehaviour
{
}