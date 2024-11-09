using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed at which the player moves")]
    public float moveSpeed = 5f;
    [Tooltip("How close to screen edge player can get (0-1)")]
    public float screenBoundary = 0.05f;
    [Tooltip("Limit how high the player can move up the screen (0-1)")]
    public float maxHeightPercentage = 0.7f;
    [Tooltip("Limit how low the player can move down the screen (0-1)")]
    public float minHeightPercentage = 0.1f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;
    public Vector2 bulletSpawnOffset = new Vector2(0f, 0.5f);

    [Header("Effects")]
    public ParticleSystem shootEffect;
    public AudioClip shootSound;

    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        // Cache components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && shootSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        mainCamera = Camera.main;
        
        // Calculate screen bounds
        CalculateScreenBounds();
    }

    void CalculateScreenBounds()
    {
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        var spriteRenderer = GetComponent<SpriteRenderer>();
        objectWidth = spriteRenderer.bounds.extents.x;
        objectHeight = spriteRenderer.bounds.extents.y;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {
        // Get both horizontal and vertical input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement vector
        Vector3 movement = new Vector3(
            horizontalInput,
            verticalInput,
            0
        ) * moveSpeed * Time.deltaTime;
        
        // Calculate new position
        Vector3 newPosition = transform.position + movement;
        
        // Convert to viewport point for boundary checking
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(newPosition);
        
        // Clamp position within boundaries
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, screenBoundary, 1 - screenBoundary);
        viewportPoint.y = Mathf.Clamp(viewportPoint.y, minHeightPercentage, maxHeightPercentage);
        
        // Convert back to world position and apply
        transform.position = mainCamera.ViewportToWorldPoint(viewportPoint);
    }

    void HandleShooting()
    {
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

    // Visual debugging of movement boundaries
    void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            // Draw movement boundaries
            Vector3 topLeft = mainCamera.ViewportToWorldPoint(new Vector3(screenBoundary, maxHeightPercentage, 0));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1 - screenBoundary, maxHeightPercentage, 0));
            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(screenBoundary, minHeightPercentage, 0));
            Vector3 bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1 - screenBoundary, minHeightPercentage, 0));

            Gizmos.color = Color.yellow;
            // Draw top and bottom boundaries
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(bottomLeft, bottomRight);
            // Draw side boundaries
            Gizmos.DrawLine(topLeft, bottomLeft);
            Gizmos.DrawLine(topRight, bottomRight);
        }
    }
}