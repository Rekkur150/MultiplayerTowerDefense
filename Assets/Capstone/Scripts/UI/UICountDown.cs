using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICountDown : MonoBehaviour
{
    [Tooltip("Where the text will be replaced for the countdown")]
    public TextMeshProUGUI CountDownText;

    [Tooltip("Text before number")]
    public string Prefix;
    [Tooltip("Text after number")]
    public string Postfix;

    public float CountdownStartTime = 10;

    public bool StartCountdownOnStart = false;

    // Start is called before the first frame update
    void Start()
    {
        CountDownText.text = "";

        if (StartCountdownOnStart)
            StartCountdown();
    }

    public void StartCountdown()
    {
        StartCoroutine("CountdownCoroutine");    
    }

    private IEnumerator CountdownCoroutine()
    {
        for (float i = CountdownStartTime; i > 0; i--)
        {
            CountDownText.text = Prefix + (int)i + Postfix;
            yield return new WaitForSeconds(1f);
        }
    }
}
