using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float startTime;       // Tracks the start time
    public float elapsedTime;      // Public variable to expose elapsed time to other scripts

    void Start()
    {
        startTime = Time.time;     // Record the time when the game starts
    }

    void Update()
    {
        elapsedTime = Time.time - startTime;   // Calculate elapsed time
    }
}
