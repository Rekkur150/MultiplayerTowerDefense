using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UICountDown))]
public class RespawnCountdown : MonoBehaviour
{
    private UICountDown Countdown;

    // Start is called before the first frame update
    void Start()
    {
        Countdown = GetComponent<UICountDown>();

        Countdown.CountdownStartTime = ClientPlayerManager.singleton.RespawnTime;
        Countdown.StartCountdown();
    }
}
