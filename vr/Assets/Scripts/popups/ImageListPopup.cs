using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ImageListPopup : MonoBehaviour
{
    public List<GameObject> popupParents;

    public float startingDelay = 0.5f;
    public float delayDecrease = 0.1f;
    public float minimumDelay = 0.1f;
    public float stayVisibleTime = 5f;

    private int currentParentIndex = 0;
    private bool isPlaying = false;

    private void Start()
    {
        foreach (GameObject parent in popupParents)
        {
            Canvas[] canvases = parent.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas canvas in canvases)
            {
                canvas.gameObject.SetActive(false);
            }
        }
        StartCoroutine(PreloadAudio());
    }
    private IEnumerator PreloadAudio()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject parent in popupParents)
        {
            AudioSource audio = parent.GetComponent<AudioSource>();
            if (audio != null && audio.clip != null)
            {
                // Force load the audio clip
                audio.volume = 0f;
                audio.Play();
                yield return null; // Wait one frame for audio to load
                audio.Stop();
            }
        }
    }

    public IEnumerator ShowCanvasesCoroutine(GameObject parent)
    {
        isPlaying = true;
        AudioSource audio = parent.GetComponent<AudioSource>();

        if (audio != null)
        {
            audio.volume = 0f;
            audio.loop = false;
            audio.Play();
            StartCoroutine(FadeInAudio(audio, 0.5f, 5f));
        }

        Canvas[] canvases = parent.GetComponentsInChildren<Canvas>(true);
        float currentDelay = startingDelay;

        foreach (Canvas canvas in canvases)
        {
            // Spread activation across multiple frames
            yield return null; // Wait one frame
            canvas.gameObject.SetActive(true);

            // Force layout rebuild in chunks if needed
            if (canvas.GetComponent<CanvasGroup>() == null)
            {
                var cg = canvas.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 1f;
            }

            yield return null; // Another frame for Canvas to rebuild
            yield return new WaitForSeconds(currentDelay);

            currentDelay = Mathf.Max(minimumDelay, currentDelay - delayDecrease);
        }

        yield return new WaitForSeconds(stayVisibleTime);

        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
            yield return null; // Spread deactivation too
        }

        if (audio != null)
        {
            yield return StartCoroutine(FadeOutAudio(audio, 1f));
            audio.Stop();
        }

        isPlaying = false;
    }
    private IEnumerator FadeInAudio(AudioSource audio, float targetVolume, float duration)
    {
        float startVolume = audio.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        audio.volume = targetVolume;
    }
    private IEnumerator FadeOutAudio(AudioSource audio, float duration)
    {
        float startVolume = audio.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audio.volume = 0f;
    }
}
