using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActivationOrDeactivation : MonoBehaviour
{
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
    }

    //only works when game is built
    public void Exit()
    {
        Application.Quit();
    }
}
