using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] RawImage _background;
    [SerializeField] TMP_Text _text;
    // Start is called before the first frame update
    public void CallSetMessage(string text, Color color)
    {
        _background.color = color;
        _text.text = text;
    }

    public void CallUIDestroy() => GeneralUtil.Ui.SetDoneText();

}
