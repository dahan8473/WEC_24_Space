using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject reloadItemPrefab; // Reference to the ReloadAmmo item prefab
    public float spawnInterval = 5f; // Time between each spawn
    public Vector2 spawnAreaMin; // Minimum position for spawn (e.g., bottom-left corner)
    public Vector2 spawnAreaMax; // Maximum position for spawn (e.g., top-right corner)

    void Start()
    {
        // Start spawning the reload item at periodic intervals
        InvokeRepeating("SpawnReloadItem", 0f, spawnInterval);
    }

    void SpawnReloadItem()
    {
        // Randomly select a position within the defined spawn area
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f); // Z value set to 0 for 2D

        // Spawn the reload item at the random position
        Instantiate(reloadItemPrefab, spawnPosition, Quaternion.identity);
    }
}