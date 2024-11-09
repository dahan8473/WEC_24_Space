using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform position;
        public GameObject enemyPrefab;
        public float initialDelay = 0f;
        public float respawnDelay = 3f;
    }

    [Header("Spawn Settings")]
    [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    [SerializeField] private bool autoStart = true;

    private void Start()
    {
        if (autoStart)
        {
            InitializeSpawnPoints();
        }
    }

    public void InitializeSpawnPoints()
    {
        foreach (SpawnPoint point in spawnPoints)
        {
            StartCoroutine(SpawnEnemyWithDelay(point));
        }
    }

    private System.Collections.IEnumerator SpawnEnemyWithDelay(SpawnPoint point)
    {
        if (point.initialDelay > 0)
        {
            yield return new WaitForSeconds(point.initialDelay);
        }

        SpawnEnemy(point);
    }

    private void SpawnEnemy(SpawnPoint point)
    {
        if (point.enemyPrefab != null && point.position != null)
        {
            GameObject enemy = Instantiate(point.enemyPrefab, point.position.position, point.position.rotation);

            // Get the enemy patrol component
            EnemyPatrol enemyPatrol = enemy.GetComponent<EnemyPatrol>();

            if (enemyPatrol != null)
            {
                // Subscribe to the enemy's death event to handle respawning
                enemyPatrol.OnDeath.AddListener(() => StartCoroutine(RespawnEnemyAfterDelay(point)));
            }
        }
    }

    private System.Collections.IEnumerator RespawnEnemyAfterDelay(SpawnPoint point)
    {
        yield return new WaitForSeconds(point.respawnDelay);
        SpawnEnemy(point);
    }
}