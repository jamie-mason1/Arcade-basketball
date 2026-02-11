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

public class FmodHandlerWithParameters : FmodHandler
{
    private readonly Dictionary<string, (float min, float max)> parameterRanges =
        new Dictionary<string, (float min, float max)>();

    public FmodHandlerWithParameters(string fmodEvent) : base(fmodEvent) { }

    public FmodHandlerWithParameters(string fmodEvent, bool loops) : base(fmodEvent, loops) { }

    public FmodHandlerWithParameters(FmodHandler other) : base(other) { }

    public FmodHandlerWithParameters(FmodHandlerWithParameters other) : base(other)
    {
        foreach (var kvp in other.parameterRanges)
        {
            parameterRanges[kvp.Key] = kvp.Value;
        }
    }

    public void AddParameter(string name, float min = 0f, float max = 100f)
    {
        parameterRanges[name] = (min, max);
    }

    public void SetDiscreteParameter(string name, int value)
    {
        SetParameterValue(name, (float)value);
    }

    public void SetContinuousParameter(string name, float value)
    {
        SetParameterValue(name, value);
    }

    public void SetLabeledParameter(string name, string label)
    {
        if (eventInstance.isValid())
        {
            FMOD.RESULT result = eventInstance.setParameterByNameWithLabel(name, label);
            if (result != FMOD.RESULT.OK)
                Debug.LogError($"Failed to set labeled parameter {name}: {result}");
        }
    }

    public int GetDiscreteParameterValue(string name)
    {
        return Mathf.RoundToInt(GetContinuousParameterValue(name));
    }

    public float GetContinuousParameterValue(string name)
    {
        if (eventInstance.isValid())
        {
            float value;
            FMOD.RESULT result = eventInstance.getParameterByName(name, out value);
            if (result == FMOD.RESULT.OK) return value;
        }
        return 0f;
    }

    private void SetParameterValue(string name, float value)
    {
        if (!parameterRanges.TryGetValue(name, out var range)) return;

        value = Mathf.Clamp(value, range.min, range.max);

        if (eventInstance.isValid())
        {
            FMOD.RESULT result = eventInstance.setParameterByName(name, value);
            if (result != FMOD.RESULT.OK)
                Debug.LogError($"Failed to set parameter {name}: {result}");
        }
    }

    public void UpdateParameters(Dictionary<string, float> newValues)
    {
        foreach (var kvp in newValues)
            SetContinuousParameter(kvp.Key, kvp.Value);
    }
}