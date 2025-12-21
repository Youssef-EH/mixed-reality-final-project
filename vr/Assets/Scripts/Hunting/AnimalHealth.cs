using UnityEngine;

namespace Hunting
{
    public class AnimalHealth : MonoBehaviour
    {
        public GameObject meatPrefab;
        public Transform meatSpawnPoint;

        bool dead;

        public void KillOneShot()
        {
            if (dead) return;
            dead = true;

            Debug.Log($"[Animal] Killed: {name}");

            if (meatPrefab != null)
            {
                Vector3 pos = meatSpawnPoint != null ? meatSpawnPoint.position : transform.position;
                Instantiate(meatPrefab, pos, Quaternion.identity);
            }
        
            gameObject.SetActive(false);
        }
    }
}