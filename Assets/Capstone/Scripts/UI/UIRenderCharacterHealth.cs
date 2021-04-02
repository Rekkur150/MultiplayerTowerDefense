using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRenderCharacterHealth: MonoBehaviour
{
    public Character Character;

    public UISliderFloatIndicator UISliderFloatIndicator;

    private void Start()
    {
        UISliderFloatIndicator.SetMaxValue(Character.MaxHealth);
    }

    private void FixedUpdate()
    {
        UISliderFloatIndicator.SetValue(Character.GetHealth());
    }
}
