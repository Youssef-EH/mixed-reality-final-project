using UnityEngine;

namespace Fire
{
    [DisallowMultipleComponent]
    public class FireLoopSound : MonoBehaviour
    {
        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip fireLoopClip;
        [Range(0f, 1f)]
        public float volume = 0.7f;

        [Range(0f, 1f)]
        public float spatialBlend = 1f;

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = spatialBlend;
            audioSource.volume = volume;
        }

        private void OnEnable()
        {
            if (audioSource == null || fireLoopClip == null)
                return;

            audioSource.clip = fireLoopClip;
            audioSource.loop = true;
            audioSource.spatialBlend = spatialBlend;
            audioSource.volume = volume;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        private void OnDisable()
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}