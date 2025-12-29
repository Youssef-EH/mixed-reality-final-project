using PixeLadder.EasyTransition;
using PixeLadder.EasyTransition.Effects;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartTransition : MonoBehaviour
{
    public FadeEffect Fade;
    private static int currentSceneIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && currentSceneIndex == 0)
        {
            SceneTransitioner.Instance.LoadScene("MapDaan", Fade);
            currentSceneIndex++;
        } else if (Keyboard.current.enterKey.wasPressedThisFrame && currentSceneIndex == 1)
        {
            SceneTransitioner.Instance.LoadScene("EndScene", Fade);
            currentSceneIndex++;
        }
    }
}
