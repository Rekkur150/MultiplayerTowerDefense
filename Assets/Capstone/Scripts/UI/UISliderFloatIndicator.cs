using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISliderFloatIndicator : MonoBehaviour
{

    [Tooltip("The inital maximum value of the indicated value")]
    public float InitalMaxValue;

    [Tooltip("The other that will be moved in order to indicate the value")]
    public RectTransform Slider;

    [Tooltip("Starts the slider initally at zero, instead of full value")]
    public bool StartAtZero = false;

    private Vector3 OriginalPosition;
    private float PositionMovementPerValueChange;
    private float MaxValue;
    private float Value;
    private IEnumerator ChangeValueCoroutine;

    // Start is called before the first frame update
    public void Start()
    {
        if (Slider == null)
            Debug.LogError("No Slider in this UISlider!", this);

        MaxValue = InitalMaxValue;
        Value = InitalMaxValue;

        if (OriginalPosition == null)
            OriginalPosition = Slider.localPosition;

        PositionMovementPerValueChange = Slider.rect.width / MaxValue;
        if (PositionMovementPerValueChange == Mathf.Infinity)
            PositionMovementPerValueChange = Slider.rect.width;

        if (StartAtZero)
            SetValue(0);

    }
    public void SetValue(float value)
    {
        Value = value;
        Vector3 newPosition = OriginalPosition;
        newPosition.x = OriginalPosition.x + PositionMovementPerValueChange * (value - MaxValue);

        Slider.localPosition = newPosition;
    }

    public void SetMaxValue(float newMaxValue)
    {
        if (newMaxValue <= 0f)
        {
            SetValue(0f);
            return;
        }
            

        MaxValue = newMaxValue;
        PositionMovementPerValueChange = Slider.rect.width / newMaxValue;
    }

    public void SetValueOverTime(float newValue, float time)
    {
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
        float rate = (newValue - Value) / time;

        for (float i = 0.1f; i < time; i = i + 0.1f)
        {
            SetValue(rate * i);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
