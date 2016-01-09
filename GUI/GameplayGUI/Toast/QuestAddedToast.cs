using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class QuestAddedToast : ToastObject<Quest>
{
    [SerializeField]
    private Text _questNameText;

    protected override void DoSetupToast(Quest obj)
    {
        _questNameText.text = obj.QuestName;
    }
}
