using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private GameManager gameManager;
    public float fallSpeed;  // Base fall speed
    public float speedMultiplier = 0.5f;  // Multiplier to adjust speed
    public float baseDamage = 10f;              // Base damage value
    public float damageMultiplier = 0.5f;       // Increase in damage per second of elapsed time

    public float baseFallSpeed = 2f;            // Initial fall speed
    public float speedIncreaseRate = 0.1f;      // Rate at which fall speed increases per second

    public float baseSize = 1f;                 // Initial size of the meteor
    public float sizeIncreaseRate = 0.05f;      // Rate at which the size increases per second

    private float currentFallSpeed;             // Current fall speed
    private int currentDamage;                // Current damage based on time
    private Vector3 initialScale;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();  // Get the GameManager instance
        initialScale = transform.localScale;            // Store the initial scale of the meteor
        currentFallSpeed = baseFallSpeed;
    }

    void Update()
    {
        // Calculate elapsed time
        float elapsedTime = gameManager.elapsedTime;

        // Update current damage
        currentDamage = (int) (baseDamage + (elapsedTime * damageMultiplier));

        // Increase speed over time
        currentFallSpeed = baseFallSpeed + (elapsedTime * speedIncreaseRate);

        // Increase size over time
        float sizeMultiplier = baseSize + (elapsedTime * sizeIncreaseRate);
        transform.localScale = initialScale * sizeMultiplier;

        // Apply movement downwards
        transform.Translate(Vector3.down * currentFallSpeed * Time.deltaTime);

        // Deactivate if the meteor falls off the screen
        if (transform.position.y < -10f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.TakeDamage(currentDamage);
        }
    }
}
