using UnityEngine;

namespace Player
{
    public class PlayerFallback : MonoBehaviour
    {
        [Header("Fallback Settings")]
        public float fallThreshold = -15f;
        public Vector3 spawnPosition;

        private void Start()
        {
            if (spawnPosition == Vector3.zero)
                spawnPosition = transform.position;
        }

        private void Update()
        {
            // Check if player has fallen below threshold
            if (transform.position.y < fallThreshold)
            {
                // Teleport back
                transform.position = spawnPosition;
                Debug.Log("Player fell through ground—teleported back to spawn!");
            }
        }
    }
}