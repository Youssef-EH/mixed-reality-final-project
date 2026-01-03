using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Fade_Screen : MonoBehaviour
{
    public static Fade_Screen Instance { get; private set; }

    public float fadeDuration = 2;
    public Color fadeColor;
    private Renderer rend;

    private static readonly Vector3 FADE_LOCAL_POSITION = new Vector3(0f, 0f, 0.3f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        rend = GetComponent<Renderer>();

        Color c = fadeColor;
        c.a = 0f;
        rend.material.SetColor("_BaseColor", c);
    }

    public void AttachToCamera(Camera cam)
    {
        if (cam == null) return;

        transform.SetParent(cam.transform);
        transform.localPosition = FADE_LOCAL_POSITION;
        transform.localRotation = Quaternion.identity;
    }

    public void DetachFromCamera()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void fadeIn()
    {
        rend.enabled = true;
        fade(1, 0);
    }

    public void fadeOut()
    {
        rend.enabled = true;
        fade(0, 1);
    }

    private void fade(float startAlpha, float endAlpha)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(startAlpha, endAlpha));
    }

    public IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        float timer = 0;
        Color newColor = fadeColor;
        newColor.a = startAlpha;
        rend.material.SetColor("_BaseColor", newColor);
        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            newColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            rend.material.SetColor("_BaseColor", newColor);

            timer += Time.deltaTime;
            yield return null;
        }

        newColor.a = endAlpha;
        rend.material.SetColor("_BaseColor", newColor);
    }
    public void ForceTransparent()
    {
        StopAllCoroutines();

        Color c = fadeColor;
        c.a = 0f;
        rend.material.SetColor("_BaseColor", c);

        // Optional but recommended
        rend.enabled = false;
    }
}
