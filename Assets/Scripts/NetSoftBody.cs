using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSoftBody : MonoBehaviour
{
    // Start is called before the first frame update
   
    public float dragMultiplier = 2f;   // higher = slows more
    public float exitForce = 2f;        // downward force when leaving net
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.attachedRigidbody;
          
            rb.velocity *= (1f - Time.deltaTime * dragMultiplier);
        }
    }
 

  

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.attachedRigidbody;

            // Push it downward slightly to finish the swish
            rb.AddForce(Vector3.down * exitForce, ForceMode.Impulse);
        }
    }
}
