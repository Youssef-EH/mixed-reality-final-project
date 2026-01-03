using UnityEngine;
using UnityEngine.Video;

public class VideoCheck : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneSequenceManager.Instance.NextScene();
    }
}
