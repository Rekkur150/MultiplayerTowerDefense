using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderFloatndicatorTest : MonoBehaviour
{

    private Slider slider;

    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent(out Slider slider))
            this.slider = slider;

        slider.minValue = 0;
        StartCoroutine(Slowcountup());
    }

    private IEnumerator Slowcountup()
    {
        Debug.Log("Is it running?");
        while (slider.value < slider.maxValue)
        {
            Debug.Log(slider.value);
            slider.value += 1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
