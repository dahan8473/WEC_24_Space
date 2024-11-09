using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float startTime;          // Tracks the start time
    public float elapsedTime;         // Public variable to expose elapsed time to other scripts
    public TextMeshProUGUI score;
    private bool isGameOver = false;  // Flag to check if the game is over
    public PlayerController player;

    void Start()
    {
        startTime = Time.time;        // Record the time when the game starts
    }

    void Update()
    {
        if (!isGameOver)
        {
            elapsedTime = Time.time - startTime;
            score.text = Mathf.FloorToInt(elapsedTime).ToString();
        }
    }

    // Call this method when the player dies
    public void GameOver()
    {
        isGameOver = true;
    }
}