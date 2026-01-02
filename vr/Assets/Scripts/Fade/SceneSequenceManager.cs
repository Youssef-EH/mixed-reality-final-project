using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance { get; private set; }

    public string[] sceneOrder;

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentIndex++;
    }

    public void NextScene()
    {
        if (currentIndex >= sceneOrder.Length) return;

        StartCoroutine(TransitionToNextScene());
    }

    private IEnumerator TransitionToNextScene()
    {
        // Fade out
        Fade_Screen.Instance.fadeOut();
        yield return new WaitForSeconds(Fade_Screen.Instance.fadeDuration);

        // Load next scene
        SceneManager.LoadScene(sceneOrder[currentIndex]);
        currentIndex++;
        yield return null; // wait one frame

        // Fade in
        Fade_Screen.Instance.fadeIn();
        yield return new WaitForSeconds(Fade_Screen.Instance.fadeDuration);
    }
}
