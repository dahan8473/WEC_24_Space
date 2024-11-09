using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject reloadItemPrefab;  // Reference to the reload ammo item prefab
    public float spawnInterval = 5f;     // Time between each spawn
    public Vector2 spawnAreaMin;         // Minimum spawn area (e.g., bottom-left corner)
    public Vector2 spawnAreaMax;         // Maximum spawn area (e.g., top-right corner)
    public float itemLifetime = 10f;     // Time until the ammo item is destroyed (in seconds)
    
    void Start()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;

        // Calculate the screen bounds in world space
        Vector3 screenBoundsMin = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenBoundsMax = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        // Set the spawn area to the camera bounds
        spawnAreaMin = new Vector2(screenBoundsMin.x, screenBoundsMin.y);
        spawnAreaMax = new Vector2(screenBoundsMax.x, screenBoundsMax.y);

        // Start periodic spawning
        InvokeRepeating("SpawnReloadItem", 0f, spawnInterval);
    }

    void SpawnReloadItem()
    {
        // Randomly select a position within the defined spawn area
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f); // Z is 0 for 2D

        // Spawn the reload item at the random position
        GameObject ammoItem = Instantiate(reloadItemPrefab, spawnPosition, Quaternion.identity);

        // Destroy the ammo item after a set amount of time (itemLifetime)
        Destroy(ammoItem, itemLifetime);
    }
}