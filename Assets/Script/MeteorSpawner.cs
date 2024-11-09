using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MeteorSpawner : MonoBehaviour
{
    public ObjectPooling objectPool;      // Reference to the object pool
    public float spawnInterval = 1f;   // Time between spawns
    public float spawnRangeX = 10f;    // Horizontal range for random spawn positions
    public float fallSpeed = 5f;       // Speed of meteor fall
    public float resetPositionY = -10f; // Y position to reset meteors
    public float startPositionY = 10f;  // Initial Y position for meteor spawn

    private float spawnTimer;

    void Update()
    {
        // Spawn meteors at intervals
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnMeteor();
            spawnTimer = 0f;
        }

        // Update meteors
        UpdateMeteors();
    }

    void SpawnMeteor()
    {
        // Get a meteor from the pool and set its position
        GameObject meteor = objectPool.GetPooledObject();
        meteor.transform.position = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), startPositionY, 0);
        meteor.SetActive(true);
    }

    void UpdateMeteors()
    {
        // Move each active meteor downward
        foreach (GameObject meteor in objectPool.pool)
        {
            if (meteor.activeInHierarchy)
            {
                meteor.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

                // Deactivate meteor if it goes below the reset position
                if (meteor.transform.position.y <= resetPositionY)
                {
                    meteor.SetActive(false);
                }
            }
        }
    }
}
