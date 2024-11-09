using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;

    private int playerHealth = 100;
    private int playerAmmo = 10;

    void Start()
    {
        UpdateHUD();
    }

    public void SetHealth(int health)
    {
        playerHealth = health;
        UpdateHUD();
    }

    public void SetAmmo(int ammo)
    {
        ammoText.text = "Ammo: " + ammo.ToString();
    }

    public int GetAmmo()
    {
        return playerAmmo; // Return the current ammo count
    }

    private void UpdateHUD()
    {
        healthText.text = "Health: " + playerHealth;
        ammoText.text = "Ammo: " + playerAmmo;
    }
}