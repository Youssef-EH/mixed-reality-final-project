using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance { get; private set; }

    public string[] sceneOrder;

    private int currentIndex = 0;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentIndex++;

        Fade_Screen.Instance.AttachToCamera(Camera.main);

        // Fade in
        Fade_Screen.Instance.fadeIn();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

        Fade_Screen.Instance.DetachFromCamera();

        // Load next scene
        SceneManager.LoadScene(sceneOrder[currentIndex]);
    }
}
