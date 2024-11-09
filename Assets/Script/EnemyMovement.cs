// This line imports Unity's basic functionality
using UnityEngine;

// Main class definition - must match the filename
public class EnemyPatrol : MonoBehaviour
{
    // SerializeField makes these variables editable in Unity's Inspector
    [SerializeField] private float moveSpeed = 2f;    // How fast enemy moves
    [SerializeField] private float moveDistance = 5f; // How far enemy moves left/right

    // Variables to track position and direction
    private Vector3 startPosition;         // Stores initial position of enemy
    private SpriteRenderer spriteRenderer; // Reference to handle sprite flipping
    private bool movingRight = true;       // Tracks movement direction

    // Called once when the game starts
    void Start()
    {
        // Save the starting position
        startPosition = transform.position;
        // Get the SpriteRenderer component attached to this object
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called every frame
    void Update()
    {
        // Calculate the furthest points the enemy can move to
        float rightBoundary = startPosition.x + moveDistance;
        float leftBoundary = startPosition.x - moveDistance;

        if (movingRight)
        {
            // Move right by: direction * speed * frame time
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            spriteRenderer.flipX = false;  // Make sprite face right

            // If we've reached the right boundary
            if (transform.position.x >= rightBoundary)
            {
                movingRight = false; // Switch direction
            }
        }
        else
        {
            // Move left by: direction * speed * frame time
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            spriteRenderer.flipX = true;   // Make sprite face left

            // If we've reached the left boundary
            if (transform.position.x <= leftBoundary)
            {
                movingRight = true; // Switch direction
            }
        }
    }

    // This is only for visualization in the Unity Editor
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw red spheres to show patrol boundaries
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPosition + Vector3.right * moveDistance, 0.3f);
            Gizmos.DrawWireSphere(startPosition - Vector3.right * moveDistance, 0.3f);
        }
    }
}