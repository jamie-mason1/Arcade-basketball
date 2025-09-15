using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class RimSound : MonoBehaviour
{
    [SerializeField] AudioSource Rim;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            Rim.Play();
        }

    }

}
