using UnityEngine;
using TMPro; // Ensure you're using TextMeshPro for the HUD

public class PlayerController : MonoBehaviour
{
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
    public int currentHealth;

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

    private void Die()
    {
        Debug.Log("Player has died.");
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
}