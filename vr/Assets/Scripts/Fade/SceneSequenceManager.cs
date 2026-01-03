using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequenceManager : MonoBehaviour
{
    public static SceneSequenceManager Instance { get; private set; }

    public string[] sceneOrder;

    private int currentSceneIndex = 0;
    private AsyncOperation preloadOperation;

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

    public void PreloadNextScene()
    {
        if (preloadOperation != null)
            return;

        if (currentSceneIndex + 1 >= sceneOrder.Length)
            return;

        preloadOperation = SceneManager.LoadSceneAsync(sceneOrder[currentSceneIndex + 1]);
        preloadOperation.allowSceneActivation = false;
    }

    public void NextScene()
    {
        if (preloadOperation == null)
        {
            Debug.LogError("NextScene called before preload!");
            return;
        }

        StartCoroutine(ActivateNextScene());
    }

    private System.Collections.IEnumerator ActivateNextScene()
    {
        Fade_Screen.Instance.fadeOut();
        yield return new WaitForSeconds(Fade_Screen.Instance.fadeDuration);

        Fade_Screen.Instance.DetachFromCamera();

        currentSceneIndex++;
        preloadOperation.allowSceneActivation = true;
        preloadOperation = null;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
                               UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Fade_Screen.Instance.AttachToCamera(Camera.main);

        Fade_Screen.Instance.ForceTransparent();
    }
}
