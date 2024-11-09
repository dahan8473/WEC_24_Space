using UnityEngine;
using TMPro; // Ensure you're using TextMeshPro for the HUD

public class PlayerController : MonoBehaviour
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

    // Add references to the TextMeshProUGUI components
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float screenBoundary = 0.05f;
    public float maxHeightPercentage = 0.7f;
    public float minHeightPercentage = 0.1f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;
    public Vector2 bulletSpawnOffset = new Vector2(0f, 0.5f);

    [Header("Effects")]
    public ParticleSystem shootEffect;
    public AudioClip shootSound;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth = 0;

    [Header("HUD")]
    public PlayerHUD playerHUD; // Reference to the PlayerHUD script for ammo display
    public int maxAmmo = 10; // Maximum ammo limit
    private int currentAmmo; // Use this variable to track current ammo

    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Initialize ammo
        currentAmmo = maxAmmo;

        // Cache components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && shootSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        mainCamera = Camera.main;
        
        // Calculate screen bounds
        CalculateScreenBounds();

        // Set initial HUD values
        UpdateHealthText();
        UpdateAmmoText();

        // Get reference to PlayerController
        playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Set initial fire rate
            baseFireRate = playerController.fireRate;
        }
    }

    void CalculateScreenBounds()
    {
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        var spriteRenderer = GetComponent<SpriteRenderer>();
        objectWidth = spriteRenderer.bounds.extents.x;
        objectHeight = spriteRenderer.bounds.extents.y;
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            Debug.Log("it is updating");
            healthText.text = "Health: " + currentHealth.ToString();
        }
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo.ToString(); // Use currentAmmo here
        }
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(
            horizontalInput,
            verticalInput,
            0
        ) * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + movement;
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(newPosition);
        
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, screenBoundary, 1 - screenBoundary);
        viewportPoint.y = Mathf.Clamp(viewportPoint.y, minHeightPercentage, maxHeightPercentage);
        
        transform.position = mainCamera.ViewportToWorldPoint(viewportPoint);
    }

    void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Calculate spawn position
        Vector3 spawnPosition = transform.position + new Vector3(bulletSpawnOffset.x, bulletSpawnOffset.y, 0);

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Set bullet velocity
        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = Vector2.up * bulletSpeed;
        }

        // Play effects
        if (shootEffect != null)
        {
            shootEffect.Play();
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Destroy bullet after time
        Destroy(bullet, 3f);

        // Update ammo count
        UpdateAmmoCount();
    }

    // Method to update the ammo count on the HUD
    void UpdateAmmoCount()
    {
        // Decrease ammo by 1
        currentAmmo--; // Directly update currentAmmo here

        // Update ammo display
        UpdateAmmoText();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }

        // Update health UI text
        UpdateHealthText();
    }
    public void ReloadAmmo()
{
    currentAmmo = maxAmmo; // Reload ammo to the max value
    UpdateAmmoText(); // Update the HUD to reflect the new ammo count
}

    private void Die()
    {
        Debug.Log("Player has died.");
        FindObjectOfType<GameManager>().GameOver();
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            Vector3 topLeft = mainCamera.ViewportToWorldPoint(new Vector3(screenBoundary, maxHeightPercentage, 0));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1 - screenBoundary, maxHeightPercentage, 0));
            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(screenBoundary, minHeightPercentage, 0));
            Vector3 bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1 - screenBoundary, minHeightPercentage, 0));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(topLeft, bottomLeft);
            Gizmos.DrawLine(topRight, bottomRight);
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