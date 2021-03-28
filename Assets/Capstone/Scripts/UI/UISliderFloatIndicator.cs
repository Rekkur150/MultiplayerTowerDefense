using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISliderFloatIndicator : MonoBehaviour
{

    [Tooltip("The inital maximum value of the indicated value")]
    public float InitalMaxValue;

    [Tooltip("The other that will be moved in order to indicate the value")]
    public RectTransform Slider;

    private Vector3 OriginalPosition;
    private float PositionMovementPerValueChange;
    private float MaxValue;
    private float Value;

    // Start is called before the first frame update
    void Start()
    {
        if (Slider == null)
            Debug.LogError("No Slider in this UISlider!", this);

        MaxValue = InitalMaxValue;
        Value = InitalMaxValue;

        OriginalPosition = Slider.localPosition;
        PositionMovementPerValueChange = Slider.rect.width / MaxValue;
        if (PositionMovementPerValueChange == Mathf.Infinity)
            PositionMovementPerValueChange = Slider.rect.width;

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
        MaxValue = newMaxValue;
        PositionMovementPerValueChange = Slider.rect.width / newMaxValue;
    }
}
