using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting
{
    public class MeatGustBurst : MonoBehaviour
    {
        [Header("Prefab (use SteakRaw_Gust)")]
        public GameObject meatPrefab;

        [Header("Burst")]
        [Range(1, 100)] public int count = 20;
        public float spawnRadius = 0.20f;

        [Header("Motion")]
        public Vector3 windDirection = new Vector3(1.4f, 1.2f, 0.05f);
        public float windForce = 3.5f;
        public float initialImpulse = 1.2f;
        public float turbulence = 1.2f;
        public float turbulenceFrequency = 0.6f;

        [Header("Motion - Clamp (keeps it visible)")]
        public float maxSpeed = 3f;

        [Header("Lifetime / Performance")]
        public float lifetime = 7f;
        public int maxPoolSize = 50;

        [Header("Audio (wind cue)")]
        public AudioSource windAudio;
        public AudioClip gustClip;
        [Range(0f, 1f)] public float gustVolume = 0.6f;
        public float audioFadeOut = 0.7f;

        private Coroutine audioRoutine;

        private class ActiveMeat
        {
            public GameObject go;
            public Rigidbody rb;
            public float dieTime;
            public Vector3 noiseSeed;
        }

        private readonly Queue<GameObject> pool = new();
        private readonly List<ActiveMeat> active = new();

        public void PlayAt(Vector3 position)
        {
            if (meatPrefab == null) return;

            // Start wind audio and guarantee it stops when the gust ends.
            StartWindAudio();

            Vector3 dir = windDirection.sqrMagnitude > 0.0001f ? windDirection.normalized : Vector3.up;

            for (int i = 0; i < count; i++)
            {
                GameObject go = GetFromPoolOrCreate();
                go.transform.position = position + Random.insideUnitSphere * spawnRadius;
                go.transform.rotation = Random.rotation;
                go.SetActive(true);

                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb == null) rb = go.AddComponent<Rigidbody>();

                // Gust behavior
                rb.useGravity = false;
                rb.linearDamping = 1.8f;     // higher damping = more "floaty"
                rb.angularDamping = 1.2f;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // Gentle kick upward (avoid "explosion")
                rb.AddForce(dir * initialImpulse, ForceMode.VelocityChange);

                active.Add(new ActiveMeat
                {
                    go = go,
                    rb = rb,
                    dieTime = Time.time + lifetime + Random.Range(-0.25f, 0.25f),
                    noiseSeed = new Vector3(Random.value * 100f, Random.value * 100f, Random.value * 100f)
                });
            }
        }

        private void Update()
        {
            if (active.Count == 0) return;

            Vector3 dir = windDirection.sqrMagnitude > 0.0001f ? windDirection.normalized : Vector3.up;

            for (int i = active.Count - 1; i >= 0; i--)
            {
                var m = active[i];

                if (m.go == null || m.rb == null)
                {
                    active.RemoveAt(i);
                    continue;
                }

                if (Time.time >= m.dieTime)
                {
                    ReturnToPool(m.go);
                    active.RemoveAt(i);
                    continue;
                }

                // Wind + slow turbulence
                float t = Time.time * turbulenceFrequency;
                Vector3 noise = new Vector3(
                    Mathf.PerlinNoise(m.noiseSeed.x, t) - 0.5f,
                    Mathf.PerlinNoise(m.noiseSeed.y, t + 10f) - 0.5f,
                    Mathf.PerlinNoise(m.noiseSeed.z, t + 20f) - 0.5f
                ) * turbulence;

                m.rb.AddForce((dir * windForce) + noise, ForceMode.Acceleration);
                m.rb.AddTorque(noise * 0.15f, ForceMode.Acceleration);

                // Clamp speed so it doesn't rocket away
                Vector3 v = m.rb.linearVelocity;
                float sp = v.magnitude;
                if (sp > maxSpeed)
                    m.rb.linearVelocity = v * (maxSpeed / sp);
            }
        }

        private void StartWindAudio()
        {
            if (windAudio == null || gustClip == null) return;

            // Restart fade routine if gust triggers again
            if (audioRoutine != null) StopCoroutine(audioRoutine);

            windAudio.loop = false;
            windAudio.clip = gustClip;
            windAudio.volume = gustVolume;
            windAudio.time = 0f;
            windAudio.Play();

            audioRoutine = StartCoroutine(FadeOutAndStop(lifetime, audioFadeOut));
        }

        private IEnumerator FadeOutAndStop(float playSeconds, float fadeSeconds)
        {
            yield return new WaitForSeconds(Mathf.Max(0f, playSeconds - fadeSeconds));

            if (windAudio == null) yield break;

            float startVol = windAudio.volume;
            float t = 0f;

            while (t < fadeSeconds)
            {
                t += Time.deltaTime;
                float k = fadeSeconds <= 0.0001f ? 1f : (t / fadeSeconds);
                windAudio.volume = Mathf.Lerp(startVol, 0f, k);
                yield return null;
            }

            windAudio.Stop();
            windAudio.volume = startVol;
            audioRoutine = null;
        }

        private GameObject GetFromPoolOrCreate()
        {
            if (pool.Count > 0)
                return pool.Dequeue();

            return Instantiate(meatPrefab);
        }

        private void ReturnToPool(GameObject go)
        {
            if (go == null) return;

            go.SetActive(false);

            if (pool.Count < maxPoolSize)
                pool.Enqueue(go);
            else
                Destroy(go);
        }
    }
}
