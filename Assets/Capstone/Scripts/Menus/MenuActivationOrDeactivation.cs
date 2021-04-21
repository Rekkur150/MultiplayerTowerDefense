using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActivationOrDeactivation : MonoBehaviour
{
    int FSUpdate;
    int firstTime=0;

    void Start()
    {

        FSUpdate = PlayerPrefs.GetInt("Fullscreen");
        if (FSUpdate == 0 || FSUpdate == 1)
        {
            FullscreenOrNah();
        }
        firstTime++;
        PlayerPrefs.SetInt("firstTime", firstTime);
        PlayerPrefs.Save();

    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
    }

    void FullscreenOrNah()
    {
        if(FSUpdate == 0)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    //only works when game is built
    public void Exit()
    {
        Application.Quit();
    }
}
