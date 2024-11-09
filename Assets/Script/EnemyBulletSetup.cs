using UnityEngine;

public class EnemyBulletSetup : MonoBehaviour
{
    void Awake()
    {
        // Ensure bullet has required components
        if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;  // Disable gravity
        }

        if (GetComponent<CircleCollider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }
    }
}