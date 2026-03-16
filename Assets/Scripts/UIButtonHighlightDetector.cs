using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIButtonHighlightDetector : MonoBehaviour
{
    GameObject lastSelected;

    FmodHandler Highlight;
    FmodHandler Select;

    

    public void Highlighted()
    {
        if(Highlight == null)
        {
            Highlight = new FmodHandler("event:/UIHighlight");
        }
        Highlight.StartEventSound();
    }

    public void Clicked()
    {
        if(Select == null)
            {
                Select = new FmodHandler("event:/ClickButton");
            }
        Select.StartEventSound();
    }




    void Update()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current != lastSelected)
        {
            lastSelected = current;

            if (current != null && current.GetComponent<Button>())
            {
                Debug.Log("Selected Button: " + current.name);
                
               
            }
        }
    }
}
