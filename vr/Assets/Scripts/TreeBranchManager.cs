using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TreeBranchManager : MonoBehaviour
{
    [Header("Branch Prefab Setup")]
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private List<Transform> branchSpawnPoints = new List<Transform>();

    [Header("Branch Settings")]
    [SerializeField] private float branchColliderRadius = 0.05f;
    [SerializeField] private float branchColliderHeight = 0.3f;

    private List<GameObject> spawnedBranches = new List<GameObject>();

    private void Start()
    {
        if (branchPrefab == null)
        {
            Debug.LogWarning("Branch prefab not assigned. Create branch prefab first.");
            return;
        }

        SpawnBranches();
    }

    private void SpawnBranches()
    {
        foreach (Transform spawnPoint in branchSpawnPoints)
        {
            if (spawnPoint == null) continue;

            GameObject branch = Instantiate(branchPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            
            BranchPullInteraction pullInteraction = branch.GetComponent<BranchPullInteraction>();
            if (pullInteraction != null)
            {
                pullInteraction.SetupAttachment(transform);
            }

            spawnedBranches.Add(branch);
        }
    }

    public void CreateBranchFromChild(Transform childTransform)
    {
        if (childTransform == null) return;

        SetupBranchInteraction(childTransform.gameObject);
    }

    private void SetupBranchInteraction(GameObject branchObject)
    {
        if (branchObject.GetComponent<XRGrabInteractable>() == null)
        {
            XRGrabInteractable grabInteractable = branchObject.AddComponent<XRGrabInteractable>();
            grabInteractable.throwOnDetach = true;
            grabInteractable.trackRotation = true;
        }

        if (branchObject.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = branchObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.mass = 0.5f;
        }

        if (branchObject.GetComponent<Collider>() == null)
        {
            CapsuleCollider collider = branchObject.AddComponent<CapsuleCollider>();
            collider.radius = branchColliderRadius;
            collider.height = branchColliderHeight;
            collider.direction = 1;
        }

        if (branchObject.GetComponent<BranchPullInteraction>() == null)
        {
            BranchPullInteraction pullInteraction = branchObject.AddComponent<BranchPullInteraction>();
            pullInteraction.SetupAttachment(transform);
        }
    }

    public int GetActiveBranchCount()
    {
        int count = 0;
        foreach (GameObject branch in spawnedBranches)
        {
            if (branch != null && branch.activeSelf)
            {
                count++;
            }
        }
        return count;
    }
}
