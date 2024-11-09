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
            Debug.Log("we have a contact");
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.isActiveAndEnabled)
            {
                // Apply the fire rate boost
                player.ApplyFireRateBoost(fireRateMultiplier);

                // If duration is greater than 0, make it temporary
                if (duration > 0)
                {
                    StartCoroutine(RemoveBoostAfterDelay(player));
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

                Debug.Log("it should distroy");
            }
        }
    }

    private System.Collections.IEnumerator RemoveBoostAfterDelay(PlayerController player)
    {
        yield return new WaitForSeconds(duration);

        // Only remove boost if the player still exists
        if (player.isActiveAndEnabled)
        {
            player.RemoveFireRateBoost(fireRateMultiplier);
        }
    }
}