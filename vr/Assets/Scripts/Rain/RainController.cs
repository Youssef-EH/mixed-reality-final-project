using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RainController : MonoBehaviour
{
    public GameObject tornadoBase;

    private int intensityStage = 0;
    
    private Exposure autoExposure;
    private ColorAdjustments colorAdjustments;
    private Fog fog;

    [Header("Rain Audio")]
    public AudioSource lightRainAudio;
    public AudioSource heavyRainAudio;

    [Header("Rain Settings")]
    public ParticleSystem rainParticleSystem;

    [Header("Skybox Settings")]
    public Color normalSkyTint = new Color(0.5f, 0.7f, 1f);
    public Color rainySkyTint = new Color(0.5f, 0.5f, 0.5f);

    public Color normalGroundColor = new Color(0.369f, 0.349f, 0.341f);
    public Color rainyGroundColor = new Color(0.2f, 0.2f, 0.2f);

    public float normalSkyExposure = 1.3f;
    public float rainySkyExposure = 0.7f;

    private Material skyboxMaterial;

    [Header("Progression Settings")]
    [Tooltip("Minimum emission rate when rain starts")]
    public float minEmissionRate = 50f;

    [Tooltip("Maximum emission rate when rain is fully active")]
    public float maxEmissionRate = 1000f;

    [Tooltip("Time in seconds for rain to reach full intensity")]
    public float rampUpDuration = 3f;

    [Tooltip("Time in seconds for rain to fade out")]
    public float rampDownDuration = 2f;

    [Header("Sky Darkening Settings")]
    [Tooltip("Reference to the Directional Light (usually the sun)")]
    public Light directionalLight;

    [Tooltip("Reference to the Global Volume for post-processing")]
    public Volume globalVolume;

    [Tooltip("Normal light intensity when not raining")]
    public float normalLightIntensity = 1f;

    [Tooltip("Dark light intensity when fully raining")]
    public float rainyLightIntensity = 0.3f;

    [Tooltip("How much to darken the scene when raining (-1 to 1, negative = darker)")]
    public float rainyExposure = -1.5f;

    private bool isRaining = false;
    private float currentIntensity = 0f;
    public Image overlay;
    private void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out autoExposure);
            globalVolume.profile.TryGet(out colorAdjustments);
            globalVolume.profile.TryGet(out fog);

            if (autoExposure != null) autoExposure.active = true;
            if (colorAdjustments != null) colorAdjustments.active = true;
            if (fog != null) fog.active = true;
        }
        skyboxMaterial = RenderSettings.skybox;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.00f;
        RenderSettings.fogColor = Color.gray;
        if (rainParticleSystem == null)
        {
            rainParticleSystem = GetComponent<ParticleSystem>();
        }

        if (directionalLight == null)
        {
            directionalLight = FindFirstObjectByType<Light>();
        }

        if (globalVolume == null)
        {
            globalVolume = FindFirstObjectByType<Volume>();
        }

        if (globalVolume != null && globalVolume.profile != null)
        {
            if (!globalVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments = globalVolume.profile.Add<ColorAdjustments>();
            }
            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.postExposure.value = 0f;
            colorAdjustments.colorFilter.overrideState = true;
            colorAdjustments.saturation.overrideState = true;
        }

        if (directionalLight != null)
        {
            normalLightIntensity = directionalLight.intensity;
        }

        if (rainParticleSystem != null)
        {
            rainParticleSystem.Stop();
            isRaining = false;
            currentIntensity = 0f;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            ToggleRain();
        }

        UpdateRainIntensity();
        UpdateLighting();
        UpdateSkybox();
        UpdateRainAudio();
    }
    private void UpdateSkybox()
    {
        if (skyboxMaterial == null)
            return;
        Color skyTint = Color.Lerp(normalSkyTint, rainySkyTint, currentIntensity);
        Color groundColor = Color.Lerp(normalGroundColor, rainyGroundColor, currentIntensity);
        float exposure = Mathf.Lerp(normalSkyExposure, rainySkyExposure, currentIntensity);
        skyboxMaterial.SetColor("_SkyTint", skyTint);
        skyboxMaterial.SetColor("_GroundColor", groundColor);
        skyboxMaterial.SetFloat("_Exposure", exposure);
        DynamicGI.UpdateEnvironment();
    }
    public void ToggleRain()
    {
        if (rainParticleSystem == null)
        {
            return;
        }
        if(intensityStage>=4)
        {
            intensityStage = 0;
            isRaining = false;
        }
        else
        {
            intensityStage++;
            isRaining = true;
        }
        if (intensityStage == 4)
        {
            tornadoBase.SetActive(true);
            StartCoroutine(WaitCoroutine());
        }
        else
        {
            tornadoBase.SetActive(false);
        }
        if (isRaining)
        {
            rainParticleSystem.Play();

            if (!lightRainAudio.isPlaying)
                lightRainAudio.Play();

            if (!heavyRainAudio.isPlaying)
                heavyRainAudio.Play();
        }
    }
    private void UpdateRainAudio()
    {
        float heavyRainThreshold = 2f;
        float audioFadeSpeed = 1.5f;

        if (!isRaining)
        {
            lightRainAudio.volume = Mathf.MoveTowards(
                lightRainAudio.volume, 0f, Time.deltaTime * audioFadeSpeed);

            heavyRainAudio.volume = Mathf.MoveTowards(
                heavyRainAudio.volume, 0f, Time.deltaTime * audioFadeSpeed);

            return;
        }

        float lightTarget = Mathf.Clamp(intensityStage / (float)heavyRainThreshold, 0, 0.5f);

        float heavyTarget = intensityStage > heavyRainThreshold ? 0.5f : 0f;

        lightRainAudio.volume = Mathf.MoveTowards(
            lightRainAudio.volume, lightTarget, Time.deltaTime * audioFadeSpeed);

        heavyRainAudio.volume = Mathf.MoveTowards(
            heavyRainAudio.volume, heavyTarget, Time.deltaTime * audioFadeSpeed);
    }
    private void UpdateRainIntensity()
    {
        if (rainParticleSystem == null)
            return;

        var emission = rainParticleSystem.emission;

        float targetIntensity = isRaining ? (0.25f*intensityStage) : 0f;
        float duration = isRaining ? rampUpDuration : rampDownDuration;

        currentIntensity = Mathf.MoveTowards(
            currentIntensity,
            targetIntensity,
            Time.deltaTime / duration
        );

        float emissionRate =
            Mathf.Lerp(minEmissionRate, maxEmissionRate, currentIntensity);

        emission.rateOverTime = emissionRate;

        if (!isRaining && currentIntensity <= 0f)
        {
            rainParticleSystem.Stop();
        }
    }


    private void UpdateLighting()
    {
        float t = currentIntensity;

        // Sun
        if (directionalLight != null)
        {
            directionalLight.intensity =
                Mathf.Lerp(normalLightIntensity, rainyLightIntensity, t);
        }

        // HDRP auto exposure darkening
        if (autoExposure != null)
        {
            autoExposure.compensation.value =
                Mathf.Lerp(0f, -2.2f, t);
        }
        //NIEUW
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value =
                Color.Lerp(
                    Color.white,
                    new Color(0.8f, 0.8f, 0.8f), // rainy gray
                    t
                );
        }
        // Desaturate colors (key for gray sky)
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value =
                Mathf.Lerp(0f, -40f, t);
        }

        // Fog sells rain
        if (fog != null)
        {
            fog.meanFreePath.value =
                Mathf.Lerp(400f, 80f, t);

            fog.albedo.value =
                Color.Lerp(Color.white, Color.gray, t);
        }
    }
    
    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSecondsRealtime(35f);
        SceneSequenceManager.Instance.NextScene();
    }
}
