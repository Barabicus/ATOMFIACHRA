using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class TextToast : ToastObject<string>
{
    [SerializeField]
    private Text _text;

    protected override void DoSetupToast(string obj)
    {
        _text.text = obj;
    }
}
