using UnityEngine;

[RequireComponent(typeof(VRBranchPull))]
public class BranchVisualFeedback : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Renderer branchRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pullingColor = Color.yellow;
    [SerializeField] private Color aboutToBreakColor = Color.red;
    [SerializeField] private float colorTransitionSpeed = 5f;
    
    [Header("Scale Effect")]
    [SerializeField] private bool enableStretchEffect = false;
    [SerializeField] private float maxStretchScale = 1.2f;
    
    [Header("Particles (Optional)")]
    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private ParticleSystem pullParticles;
    
    private VRBranchPull branchPull;
    private Material branchMaterial;
    private Color currentColor;
    private Vector3 originalScale;
    private bool hasDetached = false;
    
    private void Awake()
    {
        branchPull = GetComponent<VRBranchPull>();
        originalScale = transform.localScale;
        
        if (branchRenderer == null)
        {
            branchRenderer = GetComponentInChildren<Renderer>();
        }
        
        if (branchRenderer != null)
        {
            branchMaterial = branchRenderer.material;
            currentColor = normalColor;
        }
    }
    
    private void Update()
    {
        if (hasDetached) return;
        
        UpdateVisualFeedback();
    }
    
    private void UpdateVisualFeedback()
    {
        if (branchMaterial == null) return;
        
        Color targetColor = normalColor;
        
        if (pullParticles != null)
        {
            if (!pullParticles.isPlaying)
            {
                pullParticles.Stop();
            }
        }
        
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorTransitionSpeed);
        
        if (branchMaterial.HasProperty("_Color"))
        {
            branchMaterial.color = currentColor;
        }
        else if (branchMaterial.HasProperty("_BaseColor"))
        {
            branchMaterial.SetColor("_BaseColor", currentColor);
        }
    }
    
    public void OnBranchDetached()
    {
        hasDetached = true;
        
        if (branchMaterial != null)
        {
            branchMaterial.color = normalColor;
        }
        
        if (enableStretchEffect)
        {
            transform.localScale = originalScale;
        }
        
        if (breakParticles != null)
        {
            breakParticles.transform.SetParent(null);
            breakParticles.Play();
            Destroy(breakParticles.gameObject, 5f);
        }
        
        if (pullParticles != null)
        {
            pullParticles.Stop();
        }
    }
    
    private void OnDestroy()
    {
        if (branchMaterial != null)
        {
            Destroy(branchMaterial);
        }
    }
}
