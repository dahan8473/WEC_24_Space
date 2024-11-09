using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public float scrollSpeed = 2f;  // Speed of the scrolling
    public float resetPosition = -10f;  // Y position at which the map will reset
    public float startPosition = 10f;  // Starting Y position of the map

    void Update()
    {
        // Move the map down
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // Check if the map has passed the reset position
        if (transform.position.y <= resetPosition)
        {
            // Reset the map to the start position
            Vector3 newPosition = transform.position;
            newPosition.y = startPosition;
            transform.position = newPosition;
        }
    }
}
