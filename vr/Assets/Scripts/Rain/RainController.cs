using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RainController : MonoBehaviour
{
    [Header("Rain Settings")]
    public ParticleSystem rainParticleSystem;

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
    private ParticleSystem.EmissionModule emissionModule;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
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
        }

        if (directionalLight != null)
        {
            normalLightIntensity = directionalLight.intensity;
        }

        if (rainParticleSystem != null)
        {
            emissionModule = rainParticleSystem.emission;
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
    }

    private void ToggleRain()
    {
        if (rainParticleSystem == null)
        {
            return;
        }

        isRaining = !isRaining;

        if (isRaining)
        {
            rainParticleSystem.Play();
        }
    }

    private void UpdateRainIntensity()
    {
        if (rainParticleSystem == null)
        {
            return;
        }

        float targetIntensity = isRaining ? 1f : 0f;
        float duration = isRaining ? rampUpDuration : rampDownDuration;

        currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, Time.deltaTime / duration);

        float emissionRate = Mathf.Lerp(minEmissionRate, maxEmissionRate, currentIntensity);
        emissionModule.rateOverTime = emissionRate;

        if (!isRaining && currentIntensity <= 0f)
        {
            rainParticleSystem.Stop();
        }
    }

    private void UpdateLighting()
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = Mathf.Lerp(normalLightIntensity, rainyLightIntensity, currentIntensity);
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = Mathf.Lerp(0f, rainyExposure, currentIntensity);
        }
    }
}
