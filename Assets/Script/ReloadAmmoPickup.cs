using UnityEngine;

public class ReloadAmmoPickup : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController player;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();  // Correct way to initialize
        player = gameManager.player; // Reference to PlayerController
    }
    

    // This function is triggered when another collider enters the trigger collider of this object
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that collided is the player
        if (other.CompareTag("Player"))
        {
            // Call the function in PlayerController to reload ammo
            player.ReloadAmmo(); // This assumes the ReloadAmmo function is defined in PlayerController
            
            // Destroy the reload ammo item after it is picked up
            Destroy(gameObject);
        }
    }
}