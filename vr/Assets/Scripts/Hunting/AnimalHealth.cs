using UnityEngine;

namespace Hunting
{
    public class AnimalHealth : MonoBehaviour
    {
        [Header("Meat")] public GameObject meatPrefab;
        public Transform meatSpawnPoint;

        [Header("Death VFX")] public GameObject deathVfxPrefab;
        public float vfxLifetime = 3f;

        private bool dead;

        public void KillOneShot()
        {
            if (dead) return;
            dead = true;

            Debug.Log($"[Animal] Killed: {name}");

            Vector3 pos = meatSpawnPoint != null ? meatSpawnPoint.position : transform.position;

            // Spawn smoke VFX
            if (deathVfxPrefab != null)
            {
                var vfx = Instantiate(deathVfxPrefab, pos, Quaternion.identity);
                vfx.GetComponentInChildren<ParticleSystem>()?.Play(true);
                Destroy(vfx, Mathf.Max(0.1f, vfxLifetime));
            }

            // Spawn meat
            if (meatPrefab != null)
            {
                Instantiate(meatPrefab, pos, Quaternion.identity);
            }

            // Then remove the animal
            gameObject.SetActive(false);
        }
    }
}