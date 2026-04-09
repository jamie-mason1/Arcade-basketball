using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject objectToEnable;
    public MonoBehaviour[] controlsToDisable;


    public void ChangeScene(string SceneName)

    {
        Time.timeScale = 4f;
        SceneManager.LoadScene(SceneName);
    }
    
    public void SetGameOverConditions()
    {
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        foreach (MonoBehaviour control in controlsToDisable)
        {
            if (control != null)
            {
                control.enabled = false;
            }
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
}
