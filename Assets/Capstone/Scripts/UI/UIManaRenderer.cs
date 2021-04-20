using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

[RequireComponent(typeof(UISliderFloatIndicator))]
public class UIManaRenderer : MonoBehaviour
{
    public UISliderFloatIndicator manaSlider;
    public TextMeshProUGUI manaValue;

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out UISliderFloatIndicator manaSlider))
            this.manaSlider = manaSlider;

        if (manaValue == null)
            Debug.LogError("No text area here", this);

        StartCoroutine("WaitForClientMoneyController");
    }

    void MoneyUpdate(float money)
    {
        manaSlider.SetValueOverTime(money, 0.2f);
        manaValue.text = money + " / " + ClientMoneyController.singleton.MaxValue;
    }

    private void UpdateInformation()
    {
        manaSlider.SetMaxValue(ClientMoneyController.singleton.MaxValue);
        MoneyUpdate(ClientMoneyController.singleton.Money);
        ClientMoneyController.singleton.UpdatedMoney += new ClientMoneyController.MoneyUpdateHandler(MoneyUpdate);
    }

    private IEnumerator WaitForClientMoneyController()
    {
        while (true)
        {
            if (ClientMoneyController.singleton != null)
            {
                UpdateInformation();
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
