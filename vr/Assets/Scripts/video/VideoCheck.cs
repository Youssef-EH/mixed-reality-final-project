using UnityEngine;
using UnityEngine.Video;

public class VideoCheck : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool finished = false;
    private bool hasStarted = false;

    // Update is called once per frame
    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            hasStarted = true;
        }
        if (hasStarted) 
        {
            if (!videoPlayer.isPlaying && !finished)
            {
                finished = true;
                SceneSequenceManager.Instance.NextScene();
            }
        }
    }
}
