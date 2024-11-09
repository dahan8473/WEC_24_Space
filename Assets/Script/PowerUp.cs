using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [Tooltip("How much to decrease fire rate (0.5 = twice as fast)")]
    [SerializeField] private float fireRateMultiplier = 0.5f;
    [Tooltip("How long the power-up lasts in seconds (0 = permanent)")]
    [SerializeField] private float duration = 5f;
    [Tooltip("Should the power-up be destroyed on pickup?")]
    [SerializeField] private bool destroyOnPickup = true;

    [Header("Effects")]
    [Tooltip("Sound to play when collected")]
    [SerializeField] private AudioClip pickupSound;
    [Tooltip("Visual effect to show on pickup")]
    [SerializeField] private GameObject pickupEffect;
    [Tooltip("Effect to show while power-up is active")]
    [SerializeField] private GameObject activeEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPower playerPower = other.GetComponent<PlayerPower>();

            if (playerPower != null)
            {
                // Apply the fire rate boost
                playerPower.ApplyFireRateBoost(fireRateMultiplier);

                // If duration is greater than 0, make it temporary
                if (duration > 0)
                {
                    StartCoroutine(RemoveBoostAfterDelay(playerPower));  // Correctly pass PlayerPower
                }

                // Play pickup sound
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Show pickup effect
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                // Show active effect if it's a temporary power-up
                if (duration > 0 && activeEffect != null)
                {
                    GameObject effect = Instantiate(activeEffect, other.transform);
                    Destroy(effect, duration);
                }

                // Destroy the power-up if specified
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private System.Collections.IEnumerator RemoveBoostAfterDelay(PlayerPower playerPower)  // Change to PlayerPower
    {
        yield return new WaitForSeconds(duration);

        // Only remove boost if the player still exists
        if (playerPower != null)
        {
            playerPower.RemoveFireRateBoost(fireRateMultiplier);  // Correctly call method on PlayerPower
        }
    }
}