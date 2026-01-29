using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ApplicationQuit : MonoBehaviour
{
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();

#endif
    }
}
