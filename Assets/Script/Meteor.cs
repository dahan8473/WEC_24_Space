using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float fallSpeed;  // Base fall speed
    public float speedMultiplier = 0.5f;  // Multiplier to adjust speed

    void Update()
    {
        // Multiply fall speed by the speed multiplier to control the speed
        transform.Translate(Vector3.down * fallSpeed * speedMultiplier * Time.deltaTime);

        // Deactivate when the meteor falls below a certain point
        if (transform.position.y < -10f)
        {
            gameObject.SetActive(false);
        }
    }
}
