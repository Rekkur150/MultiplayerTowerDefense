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
        Character.OnHealthChange += new Character.ChangeHealthHandler(HealthUpdated);

    }

    private void HealthUpdated(float newHealth)
    {
        UISliderFloatIndicator.SetValue(newHealth);
    }
}
