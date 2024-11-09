using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public GameObject meteorPrefab; // Meteor prefab to pool
    public int poolSize = 10;       // Number of meteors to pre-instantiate

    public List<GameObject> pool;  // List to store pooled meteors

    void Start()
    {
        // Initialize the pool
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(meteorPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        // Return an inactive meteor if available
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Optionally, you can increase pool size dynamically
        GameObject newObj = Instantiate(meteorPrefab);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }
}
