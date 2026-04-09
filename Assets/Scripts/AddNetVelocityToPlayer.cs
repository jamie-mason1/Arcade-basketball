using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNetVelocityToPlayer : MonoBehaviour
{
     private Rigidbody rb; // This object's Rigidbody

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Rigidbody playerRb = collision.rigidbody;

        if (playerRb != null && rb != null)
        {
            // Add this object's velocity to the player
            //playerRb.velocity += rb.velocity;
        }

    }
    
}
}
