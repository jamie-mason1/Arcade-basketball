using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceSound : MonoBehaviour
{
    public FmodHandlerWithParameters BounceSoundFmod;  
    string eventBouncePath = "event:/Bounce";
    string parameterNameBounce = "BounceImpact";
 
   void Start()
    {
        BounceSoundFmod = new FmodHandlerWithParameters(eventBouncePath);
        BounceSoundFmod.AddParameter(parameterNameBounce, 0f, 1f);
        
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            if(BounceSoundFmod == null)
            {
                BounceSoundFmod = new FmodHandlerWithParameters(eventBouncePath);
                BounceSoundFmod.AddParameter(parameterNameBounce, 0f, 1f);
            }
            //Debug.Log(collision.relativeVelocity.magnitude);
            float vol = Mathf.Clamp01(collision.relativeVelocity.magnitude / 100f);
            BounceSoundFmod.SetContinuousParameter(parameterNameBounce, vol);
            BounceSoundFmod.setSoundPlayPosition(collision.contacts[0].point);


            BounceSoundFmod.StartEventSound();
        }

    }

    
}
