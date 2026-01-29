using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboardSound : MonoBehaviour
{
    [SerializeField] AudioSource Backboard;
     FmodHandler backboardImpact;
    string eventPath = "event:/Backboard";

    private void OnCollisionEnter(Collision collision)
    {

       

        if (collision.collider.CompareTag("Player"))
        {
            Backboard.Play();
        }
        if(backboardImpact == null)
            {
                backboardImpact= new FmodHandler(eventPath);
            } 
            backboardImpact.setSoundPlayPosition(collision.contacts[0].point);
            backboardImpact.StartEventSound();

    }
}
