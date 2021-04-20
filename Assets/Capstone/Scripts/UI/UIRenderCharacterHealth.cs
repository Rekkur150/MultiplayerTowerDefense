using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRenderCharacterHealth: MonoBehaviour
{
    public Character Character;

    public UISliderFloatIndicator UISliderFloatIndicator;

    private void Start()
    {
        if (Character == null)
            Debug.LogError("Character Missing!", this);

        UISliderFloatIndicator.SetMaxValue(Character.MaxHealth);
        UISliderFloatIndicator.SetValue(Character.GetHealth());
        Character.OnHealthChange += new Character.ChangeHealthHandler(HealthUpdated);

    }

    private void HealthUpdated(float newHealth)
    {
        UISliderFloatIndicator.SetValueOverTime(newHealth, 0.2f);
    }
}
