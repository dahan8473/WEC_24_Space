using UnityEngine;

public class HealthAndAmmo : MonoBehaviour
{
    public int maxHealth = 100;        // Maximum health
    public int currentHealth;          // Current health value
    public int maxAmmo = 10;           // Maximum ammo
    public int currentAmmo;            // Current ammo count

    void Start()
    {
        currentHealth = maxHealth;     // Initialize health to max health
        currentAmmo = maxAmmo;         // Initialize ammo to max ammo
    }

    // Method to apply damage to health
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to use ammo when shooting
    public void UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            // Add shooting logic here if needed
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }

    // Method to reload ammo
    public void Reload()
    {
        currentAmmo = maxAmmo;
    }

    // Method to handle death
    private void Die()
    {
        // Logic for what happens when the entity dies
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }
}