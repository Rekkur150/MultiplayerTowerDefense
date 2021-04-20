using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuEnter : MonoBehaviour
{

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
    }

    //onyl works when game is built
    public void fullScreenOnOff()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log("Toggled Fullscreen");
    }
}
    
