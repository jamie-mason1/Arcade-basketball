using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedirectionScript : MonoBehaviour
{
    [SerializeField] private Transform targetPoint; // Child object
    [SerializeField] private float boostForce = 15f;
    [SerializeField] private float AccelForce = 15f;
    [SerializeField] private bool useUpwardBoost = true;
    [SerializeField] private float upwardForce = 3f;

    private void Awake()
    {
        // Optional: auto-assign first child
        if (targetPoint == null && transform.childCount > 0)
        {
            targetPoint = transform.GetChild(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Direction toward the child target
        Vector3 direction = (targetPoint.position - other.transform.position).normalized;

        // Optional arc (adds a bit of lift)
        if (useUpwardBoost)
        {
            direction += Vector3.up * upwardForce;
        }

        // Normalize again after adding upward force
        direction = direction.normalized;

        // Reset velocity if you want consistent boosts
        //rb.velocity = Vector3.zero; // (Unity 6 uses linearVelocity)

        // Apply impulse
        rb.AddForce(direction * boostForce, ForceMode.Impulse);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Direction toward the child target
        Vector3 direction = (targetPoint.position - other.transform.position).normalized;

        

        // Normalize again after adding upward force
        direction = direction.normalized;

        // Reset velocity if you want consistent boosts
        //rb.velocity = Vector3.zero; // (Unity 6 uses linearVelocity)

        // Apply impulse
        rb.AddForce(direction * AccelForce, ForceMode.Force);
    }
}
