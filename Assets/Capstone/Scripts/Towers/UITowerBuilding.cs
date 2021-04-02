using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITowerBuilding : MonoBehaviour
{
    public UISliderFloatIndicator UISliderFloatIndicator;
    public GameObject UIGameObject;

    public float Time;
    public float Rate;


    public void UITimer(float Time, float Rate, float MaxValue)
    {
        this.Time = Time;
        this.Rate = Rate;

        UIGameObject.SetActive(true);
        UISliderFloatIndicator.SetMaxValue(MaxValue);

        StartCoroutine(TimerIncrease());
    }

    private IEnumerator TimerIncrease()
    {

        for (float i = 0.1f; i < Time; i = i + 0.1f)
        {
            UISliderFloatIndicator.SetValue(Rate * i);
            yield return new WaitForSeconds(0.1f);
        }

        UIGameObject.SetActive(false);

    }

}
