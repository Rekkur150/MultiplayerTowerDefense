using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuEnter : MonoBehaviour
{
    int initial = 1;
    int current;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
    }

    //only works when game is built
    public void fullScreenOnOff()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log("Toggled Fullscreen");

        if(current == 1)
        {
            current = 0;
            PlayerPrefs.SetInt("Fullscreen", current);
            PlayerPrefs.Save();
        }
        else
        {
            current = 1;
            PlayerPrefs.SetInt("Fullscreen", current);
            PlayerPrefs.Save();
        }

    }


}
    
