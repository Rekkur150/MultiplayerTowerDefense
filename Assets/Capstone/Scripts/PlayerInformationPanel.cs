using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInformationPanel : MonoBehaviour
{
    public static PlayerInformationPanel singleton;

    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        } else if (singleton != this)
        {
            Destroy(this);
        }

        if (TryGetComponent(out TextMeshProUGUI textMesh))
            this.textMesh = textMesh;
    }


    public void UpdateText(string newText)
    {
        textMesh.text = newText;
    }

}
