using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{

    FmodHandler InGameMusic;
    string musicEventPath = "event:/Menu Background Music";


    void Start()
    {
        InGameMusic = new FmodHandler(musicEventPath);
        InGameMusic.StartEventSound();
    }

    
    void OnDestroy()
    {
        InGameMusic.EndSoundInstance();

    }
    void OnDisable()
    {
        InGameMusic.EndSoundInstance();

    }
}
