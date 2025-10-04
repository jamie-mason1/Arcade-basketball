using UnityEngine;
using System.Collections;

public class BackboardShake : MonoBehaviour
{
    [SerializeField] private BackboardVisualWhip visualWhip;

    void Start()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float impactForce = collision.relativeVelocity.magnitude;
            visualWhip?.TriggerWhip(impactForce, collision.contacts[0].point);
        }
    }
    
}
