using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scored : MonoBehaviour
{
    AudioSource ding;
    private void Awake()
    {
        ding = GameObject.Find("Ding").GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.position.y > transform.position.y + 0.1f) 
        {
            if (other.CompareTag("Player"))
            {
               ding.Play();
            }
        }
    }
}
