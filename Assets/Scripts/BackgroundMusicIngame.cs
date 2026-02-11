using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicIngame : MonoBehaviour
{

    FmodHandler InGameMusic;
    string musicEventPath = "event:/In Game Background music";


    void Start()
    {
        InGameMusic = new FmodHandler(musicEventPath);
        InGameMusic.StartEventSound();
    }

    // Update is called once per frame
    void Update()
    {
        
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
