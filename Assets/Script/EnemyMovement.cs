using UnityEngine;
using UnityEngine.Events;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 5f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float bulletDamageToPlayer = 10f;
    [SerializeField] private Vector2 bulletSpawnOffset = new Vector2(0f, -0.5f);
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private GameObject shootEffectPrefab;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool showDamageNumbers = true;
    [SerializeField] private float bulletDamage = 20f;

    [Header("Death Settings")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private GameObject respawnEffectPrefab;

    [Header("Power-Up Settings")]
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private float dropChance = 0.3f;
    [SerializeField] private Vector2 dropOffset = new Vector2(0f, 0f);
    [SerializeField] private bool dropOnlyIfNoOtherPowerUps = true;

    [Header("Hit Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private Material hitFlashMaterial;
    [SerializeField] private float hitFlashDuration = 0.1f;

    [Header("Events")]
    public UnityEvent<float> OnDamaged;
    public UnityEvent OnDeath;
    public UnityEvent OnRespawn;
    public UnityEvent OnPowerUpDropped;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private bool movingRight = true;
    private Material originalMaterial;
    private bool isDead = false;
    private Collider2D enemyCollider;
    private float nextFireTime;
    private int powerUpLayer;

    void Awake()
    {
        powerUpLayer = LayerMask.NameToLayer("PowerUp");
    }

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }

        currentHealth = maxHealth;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            Color tempColor = spriteRenderer.color;
            tempColor.a = 1f;
            spriteRenderer.color = tempColor;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }

        isDead = false;
        movingRight = true;
        nextFireTime = Time.time + fireRate;
    }

    void Update()
    {
        if (!isDead)
        {
            HandlePatrol();
            HandleShooting();
        }
    }

    private void HandlePatrol()
    {
        float rightBoundary = startPosition.x + moveDistance;
        float leftBoundary = startPosition.x - moveDistance;

        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (spriteRenderer != null) spriteRenderer.flipX = false;

            if (transform.position.x >= rightBoundary)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (spriteRenderer != null) spriteRenderer.flipX = true;

            if (transform.position.x <= leftBoundary)
            {
                movingRight = true;
            }
        }
    }

    private void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(bulletSpawnOffset.x, bulletSpawnOffset.y, 0);
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.down * bulletSpeed;
            }

            // Set up bullet damage
            if (bullet.TryGetComponent<Bullet>(out var bulletComp))
            {
                bulletComp.damage = bulletDamageToPlayer;
            }

            if (shootEffectPrefab != null)
            {
                Instantiate(shootEffectPrefab, spawnPosition, Quaternion.identity);
            }

            if (shootSound != null)
            {
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
            }

            Destroy(bullet, 3f);
        }
    }

    private bool CanDropPowerUp()
    {
        if (dropOnlyIfNoOtherPowerUps)
        {
            GameObject[] existingPowerUps = GameObject.FindGameObjectsWithTag("PowerUp");
            return existingPowerUps.Length == 0;
        }
        return true;
    }

    private void TrySpawnPowerUp()
    {
        Debug.Log("Try power");
        if (powerUpPrefab != null &&  Random.Range(1, 3) == dropChance && CanDropPowerUp())
        {
            Debug.Log("powerup!");
            Vector3 dropPosition = transform.position + new Vector3(dropOffset.x, dropOffset.y, 0);
            GameObject powerUp = Instantiate(powerUpPrefab, dropPosition, Quaternion.identity);
            
            // Ensure the power-up has the correct layer
            powerUp.layer = powerUpLayer;
            
            OnPowerUpDropped?.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            if (other.TryGetComponent<Bullet>(out var bullet))
            {
                TakeDamage(bullet.damage);
            }
            else
            {
                TakeDamage(bulletDamage);
            }
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        OnDamaged?.Invoke(damage);

        if (showDamageNumbers)
        {
            ShowDamageNumber(damage);
        }

        PlayHitEffects();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashSprite());
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        TrySpawnPowerUp();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        StartCoroutine(RespawnAfterDelay());
    }

    private System.Collections.IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        transform.position = startPosition;

        if (respawnEffectPrefab != null)
        {
            Instantiate(respawnEffectPrefab, transform.position, Quaternion.identity);
        }

        Initialize();
        OnRespawn?.Invoke();
    }

    private void PlayHitEffects()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
    }

    private void ShowDamageNumber(float damage)
    {
        Debug.Log($"Enemy took {damage} damage! Health: {currentHealth}/{maxHealth}");
    }

    private System.Collections.IEnumerator FlashSprite()
    {
        if (spriteRenderer != null && hitFlashMaterial != null)
        {
            spriteRenderer.material = hitFlashMaterial;
            yield return new WaitForSeconds(hitFlashDuration);
            spriteRenderer.material = originalMaterial;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 gizmoCenter = Application.isPlaying ? startPosition : transform.position;
        
        // Draw patrol boundaries
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoCenter + Vector3.right * moveDistance, 0.3f);
        Gizmos.DrawWireSphere(gizmoCenter - Vector3.right * moveDistance, 0.3f);
        Gizmos.DrawLine(gizmoCenter + Vector3.right * moveDistance, gizmoCenter - Vector3.right * moveDistance);
        
        // Draw power-up drop position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gizmoCenter + new Vector3(dropOffset.x, dropOffset.y, 0), 0.2f);

        // Draw bullet spawn position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gizmoCenter + new Vector3(bulletSpawnOffset.x, bulletSpawnOffset.y, 0), 0.15f);
    }
}

// Simple Bullet class that can be used by both player and enemy bullets
public class Bullet : MonoBehaviour
{
    public float damage = 10f;
}