using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuEnter : MonoBehaviour
{
    /*void Start()
    {
        gameObject.SetActive(false);
    }*/

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
    }

    IEnumerator OptionsWorkaround()
    {
        yield return new WaitForSeconds(0.01f);
        gameObject.SetActive(false);
    }
}
    
