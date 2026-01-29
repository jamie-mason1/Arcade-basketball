using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class RimSound : MonoBehaviour
{
    FmodHandler Rim;



    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            if(Rim == null)
            {
                Rim = new FmodHandler("event:/Rim");
            }
            Rim.setSoundPlayPosition(collision.contacts[0].point);
            Rim.StartEventSound();
        }

    }

}
