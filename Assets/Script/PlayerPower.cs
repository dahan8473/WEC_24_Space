using UnityEngine;

public class PlayerPower : MonoBehaviour
{
    [Header("Power Settings")]
    [SerializeField] private float currentPower = 0f;
    [SerializeField] private float maxPower = 100f;

    [Header("Fire Rate Settings")]
    [SerializeField] private float baseFireRate = 0.5f;         // Default fire rate
    [SerializeField] private float minFireRate = 0.1f;         // Maximum speed (minimum time between shots)
    [SerializeField] private float currentFireRateBonus = 1f;  // Current multiplier (1 = normal speed)

    // Reference to the PlayerController
    private PlayerController playerController;

    private void Start()
    {
        // Get reference to PlayerController
        playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Set initial fire rate
            baseFireRate = playerController.fireRate;
        }
    }

    // Method to increase power
    public void IncreasePower(float amount)
    {
        currentPower = Mathf.Min(currentPower + amount, maxPower);
        Debug.Log($"Power increased! Current Power: {currentPower}");
    }

    // Method to get current power level
    public float GetCurrentPower()
    {
        return currentPower;
    }

    // Method to apply fire rate boost
    public void ApplyFireRateBoost(float multiplier)
    {
        if (playerController != null)
        {
            // Update the current bonus multiplier
            currentFireRateBonus *= multiplier;

            // Calculate new fire rate (ensuring it doesn't go below minimum)
            float newFireRate = Mathf.Max(baseFireRate * multiplier, minFireRate);

            // Apply to player controller
            playerController.fireRate = newFireRate;

            Debug.Log($"Fire Rate modified! New fire rate: {newFireRate:F3} seconds");
        }
    }

    // Method to remove fire rate boost
    public void RemoveFireRateBoost(float multiplier)
    {
        if (playerController != null)
        {
            // Remove the multiplier from current bonus
            currentFireRateBonus /= multiplier;

            // Calculate new fire rate
            float newFireRate = Mathf.Max(baseFireRate * currentFireRateBonus, minFireRate);

            // Apply to player controller
            playerController.fireRate = newFireRate;

            Debug.Log($"Fire Rate boost removed! New fire rate: {newFireRate:F3} seconds");
        }
    }

    // Method to reset fire rate to base value
    public void ResetFireRate()
    {
        if (playerController != null)
        {
            currentFireRateBonus = 1f;
            playerController.fireRate = baseFireRate;
            Debug.Log("Fire Rate reset to base value!");
        }
    }

    // Method to get current fire rate
    public float GetCurrentFireRate()
    {
        return playerController != null ? playerController.fireRate : baseFireRate;
    }
}