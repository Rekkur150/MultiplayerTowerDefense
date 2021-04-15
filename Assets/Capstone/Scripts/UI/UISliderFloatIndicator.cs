using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderFloatIndicator : MonoBehaviour
{
    [Tooltip("The other that will be moved in order to indicate the value")]
    public Slider slider;

    [Tooltip("Starts the slider initally at zero, instead of full value")]
    public bool StartAtZero = false;

    private IEnumerator ChangeValueCoroutine;

    // Start is called before the first frame update
    public void Awake()
    {
        if (slider == null)
            Debug.LogError("No Slider in this UISlider!", this);

        slider.minValue = 0;
        slider.maxValue = 100;

        if (StartAtZero)
            SetValue(0f);
        else SetValue(slider.maxValue);

    }
    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SetMaxValue(float newMaxValue)
    {
        slider.maxValue = newMaxValue;
    }

    public void SetValueOverTime(float newValue, float time)
    {

        if (gameObject.activeInHierarchy == false)
        {
            SetValue(newValue);
            return;
        }

        if (ChangeValueCoroutine != null)
            StopValueOverTime();

        ChangeValueCoroutine = ChangeValueOverTime(newValue, time);
        StartCoroutine(ChangeValueCoroutine);

    }

    public void StopValueOverTime()
    {
        if (ChangeValueCoroutine != null)
            StopCoroutine(ChangeValueCoroutine);
    }

    private IEnumerator ChangeValueOverTime(float newValue, float time)
    {
        float rate = (newValue - slider.value) / time;
        float originalValue = slider.value;

        for (float i = 0.01f; i < time; i = i + 0.01f)
        {
            SetValue(originalValue + (rate * i));
            yield return new WaitForSeconds(0.01f);
        }

        SetValue(newValue);
    }
}
