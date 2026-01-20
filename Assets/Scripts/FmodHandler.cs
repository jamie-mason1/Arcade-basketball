using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class FmodHandler
{
    [EventRef]
    protected string fmodEvent;
    protected FMOD.Studio.EventInstance eventInstance;
    protected bool loops;
    public FmodHandler(string fmodEvent)
    {
        this.loops = false;
        this.fmodEvent = fmodEvent;
        CreateInstance();
    }
    public FmodHandler(string fmodEvent, bool loops)
    {
        this.loops = false;
        this.loops = loops;
        this.fmodEvent = fmodEvent;
        CreateInstance();
    }
    public FmodHandler(FmodHandler fmodCarSoundManager)
    {
        this.loops = fmodCarSoundManager.loops;
        this.fmodEvent = fmodCarSoundManager.fmodEvent;
        CreateInstance();
    }


    public string GetFmodEvent()
    {
        return fmodEvent;
    }
    public bool getLoops() => loops;

    protected void SetFmodEvent(string fmodEvent)
    {
        this.fmodEvent = fmodEvent;
    }

    private void CreateInstance()
    {
        if (!eventInstance.isValid())
        {
            eventInstance = RuntimeManager.CreateInstance(fmodEvent);
        }
    }

    public void StartEventSound()
    {
        if (!eventInstance.isValid())
        {
            CreateInstance();
        }
        eventInstance.start();
        
    }

    public void setSoundPlayPosition(Vector3 position)
    {
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();
        eventInstance.get3DAttributes(out attributes);
        Vector3 pos = new Vector3(attributes.position.x, attributes.position.y, attributes.position.z);

    }
    public void PauseEventSound()
    {
        if (eventInstance.isValid() && IsEventPlaying())
        {
            eventInstance.setPaused(true);
        }
    }

    public void ResumeEventSound()
    {

        if (eventInstance.isValid() && IsEventPlaying())
        {
            eventInstance.setPaused(false);
        }
        
    }
    public void stopSound()
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
    public bool IsEventPlaying()
    {
        if (eventInstance.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            eventInstance.getPlaybackState(out playbackState);
            return playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING;
        }
        return false;
    }
    public void EndSoundInstance()
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
    ~FmodHandler()
    {
        if (eventInstance.isValid())
        {
            eventInstance.release();
        }
    }
}
