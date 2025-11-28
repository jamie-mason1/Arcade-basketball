using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public string SceneName;
    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
