using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage((int)damage);
            }
            Destroy(gameObject);
        }
    }
}