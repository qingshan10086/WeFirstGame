using System.Collections;
using UnityEngine;

namespace CoinSystem
{
    public class CollectedCoin : MonoBehaviour
    {
        public int coinsToGive; // Number of coins this collectible gives to the player
        public ParticleSystem CoinParticule; // Particle effect to play when the coin is collected
        public float Distance; // Vertical movement range for floating effect

        [SerializeField] private AudioClip coinSound; // Sound to play on collection
        [SerializeField] private AudioSource audioSource; // Audio source used to play the sound

        public float moveSpeed = 1.0f; // Speed of vertical movement
        public float originalY; // Original Y position of the coin

        private SpriteRenderer spriteRenderer; // Used to hide the coin after collection
        private Collider2D coinCollider; // Used to disable collisions after collection

        private void Start()
        {
            originalY = transform.position.y;

            // Get required components
            spriteRenderer = GetComponent<SpriteRenderer>();
            coinCollider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            // Floating up and down effect using sine wave
            float newY = originalY + Mathf.Sin(Time.time * moveSpeed) * Distance;
            transform.position = new Vector2(transform.position.x, newY);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {

                // Play sound and particle effects
                audioSource.PlayOneShot(coinSound);
                CreateCoinParticule(transform.position);

                // Add coins to the player
                Coin.instance.AddCoins(coinsToGive);

                // Handle collection
                Collect();
            }
        }

        private void Collect()
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;

            if (coinCollider != null)
                coinCollider.enabled = false;

            // Destroy the coin after a short delay
            StartCoroutine(DestroyAfterDelay(1f));
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

        private void CreateCoinParticule(Vector2 position)
        {
            // Set the particle system's position to the coin's position and play it
            Vector3 particlePosition = new Vector3(position.x, position.y, 0f);
            CoinParticule.transform.position = particlePosition;
            CoinParticule.Play();
        }
    }
}
