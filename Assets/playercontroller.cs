using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed at which the player moves left/right")]
    public float moveSpeed = 5f;
    [Tooltip("How close to screen edge player can get (0-1)")]
    public float screenBoundary = 0.05f;

    [Header("Shooting Settings")]
    [Tooltip("Prefab of the bullet to spawn")]
    public GameObject bulletPrefab;
    [Tooltip("Speed at which bullets travel")]
    public float bulletSpeed = 10f;
    [Tooltip("Time between shots")]
    public float fireRate = 0.5f;
    [Tooltip("Position offset for bullet spawn")]
    public Vector2 bulletSpawnOffset = new Vector2(0f, 0.5f);

    [Header("Effects")]
    [Tooltip("Particle system for shooting effect")]
    public ParticleSystem shootEffect;
    [Tooltip("Audio clip for shooting sound")]
    public AudioClip shootSound;

    // Private variables
    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;

    void Start()
    {
        // Cache components and calculate boundaries
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && shootSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        mainCamera = Camera.main;
        
        // Calculate screen bounds for player movement
        CalculateScreenBounds();
    }

    void CalculateScreenBounds()
    {
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        objectWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        // Get input and calculate movement
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;
        
        // Calculate new position
        Vector3 newPosition = transform.position + movement;
        
        // Keep player within screen bounds
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(newPosition);
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, screenBoundary, 1 - screenBoundary);
        
        // Apply movement
        transform.position = mainCamera.ViewportToWorldPoint(viewportPoint);
    }

    void HandleShooting()
    {
        // Check for input and fire rate
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
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
    }

    // Optional: Add collision handling
    void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collisions with enemies or power-ups
        if (other.CompareTag("Enemy"))
        {
            // Handle player death or damage
            Debug.Log("Player hit by enemy!");
        }
    }

    // Optional: Health system
    public void TakeDamage()
    {
        // Implement damage handling
        Debug.Log("Player took damage!");
    }
}