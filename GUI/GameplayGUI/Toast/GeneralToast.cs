using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GeneralToast : ToastObject<string>
{
    [SerializeField]
    private Text _toastText;

    protected override void DoSetupToast(string targetToastText)
    {
        _toastText.text = targetToastText;
    }
}
