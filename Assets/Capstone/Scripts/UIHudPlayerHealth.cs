using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHudPlayerHealth : MonoBehaviour
{
    public Character Character;

    public ClientPlayerManager clientPlayerManger;

    public UISliderFloatIndicator UISliderFloatIndicator;
    public TextMeshProUGUI HealthValue;

    private Character.ChangeHealthHandler EventCall;

    private void Start()
    {
        if (TryGetComponent(out UISliderFloatIndicator UISliderFloatIndicator))
            this.UISliderFloatIndicator = UISliderFloatIndicator;

        if (HealthValue == null)
            Debug.LogError("No text area here", this);

        if (ClientPlayerManager.singleton != null)
        {
            ClientPlayerManager.singleton.OnPlayerCharacterUpdate += new ClientPlayerManager.PlayerCharacterChangedHandler(CharacterChanged);
            EventCall = new Character.ChangeHealthHandler(HealthUpdated);
        } else
        {
            clientPlayerManger.OnPlayerCharacterUpdate += new ClientPlayerManager.PlayerCharacterChangedHandler(CharacterChanged);
            EventCall = new Character.ChangeHealthHandler(HealthUpdated);
        }


    }

    private void CharacterChanged(Character newCharacter)
    {

        if (Character != null)
            Character.OnHealthChange -= EventCall;

        Character = newCharacter;
        UISliderFloatIndicator.SetMaxValue(Character.MaxHealth);
        Character.OnHealthChange += EventCall;

        HealthUpdated(newCharacter.GetHealth());

    }

    private void HealthUpdated(float newHealth)
    {
        UISliderFloatIndicator.SetValueOverTime(newHealth, 0.2f);
        HealthValue.text = newHealth + " / " + Character.MaxHealth;
    }
}

