using UnityEngine;

public class ReloadItem : MonoBehaviour
{
    public PlayerController playerController; // Reference to the PlayerController to access the ammo
    public AudioClip pickupSound; // Optional: sound for pickup
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Detect player collision with the item
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Reload the player's ammo
            if (playerController != null)
            {
                playerController.ReloadAmmo();
            }

            // Play the pickup sound (if any)
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            // Destroy the item after it has been picked up
            Destroy(gameObject);
        }
    }
}