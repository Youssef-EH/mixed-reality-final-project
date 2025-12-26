using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FireWoodOutlineGuidance : MonoBehaviour
{
    [Header("Branches (3x)")]
    public XRGrabInteractable[] branchGrabs;
    public Outline[] branchOutlines;        

    [Header("Fire")]
    public Outline fireOutline;          

    [Header("Snap State")]
    public SnapManager snapManager;
    public bool disableAllOutlinesWhenComplete = true;

    private void OnEnable()
    {
        HookBranchEvents(true);
        Refresh();
    }

    private void OnDisable()
    {
        HookBranchEvents(false);
    }

    private void Update()
    {
        Refresh();
    }

    private void HookBranchEvents(bool hook)
    {
        if (branchGrabs == null) return;

        foreach (var grab in branchGrabs)
        {
            if (grab == null) continue;

            if (hook)
            {
                grab.selectEntered.AddListener(OnBranchGrabbed);
                grab.selectExited.AddListener(OnBranchReleased);
            }
            else
            {
                grab.selectEntered.RemoveListener(OnBranchGrabbed);
                grab.selectExited.RemoveListener(OnBranchReleased);
            }
        }
    }

    private void OnBranchGrabbed(SelectEnterEventArgs args) => Refresh();
    private void OnBranchReleased(SelectExitEventArgs args) => Refresh();

    private void Refresh()
    {
        bool allFilled = false;

        // If your SnapManager has "allFilled" (your debug log suggests it does), use it.
        if (snapManager != null)
            allFilled = snapManager.allFilled;

        if (disableAllOutlinesWhenComplete && allFilled)
        {
            SetAllBranchOutlines(false);
            SetFireOutline(false);
            return;
        }

        bool holdingUnsappedBranch = false;

        for (int i = 0; i < branchGrabs.Length; i++)
        {
            var grab = branchGrabs[i];
            var outline = (branchOutlines != null && i < branchOutlines.Length) ? branchOutlines[i] : null;

            if (grab == null || outline == null)
                continue;

            // "Snapped" detection:
            // We’ll mark snapped branches using SnapMarker (added by SnapPoint in Step 4 below).
            bool isSnapped = grab.GetComponent<SnapMarker>() != null;

            if (isSnapped)
            {
                outline.enabled = false;
                continue;
            }

            bool isHeld = grab.isSelected;

            if (isHeld)
            {
                outline.enabled = false;            // grabbed branch outline OFF
                holdingUnsappedBranch = true;       // fire outline ON while holding
            }
            else
            {
                outline.enabled = true;             // not grabbed + not snapped => outline ON
            }
        }

        SetFireOutline(holdingUnsappedBranch);
    }

    private void SetAllBranchOutlines(bool on)
    {
        if (branchOutlines == null) return;
        foreach (var o in branchOutlines)
            if (o != null) o.enabled = on;
    }

    private void SetFireOutline(bool on)
    {
        if (fireOutline != null)
            fireOutline.enabled = on;
    }
}
