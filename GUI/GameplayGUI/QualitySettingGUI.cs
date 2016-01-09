using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QualitySettingGUI : MonoBehaviour
{
    private UISliderExtended _slider;
    private int _targetPresetIndex;

    private void Awake()
    {
        _slider = GetComponent<UISliderExtended>();
        _slider.value = QualityController.Instance.QualityLevel;
        _slider.onValueChanged.AddListener((f) => SetPresetQualityLevel((int)f));
    }

    public void SetPresetQualityLevel(int index)
    {
        //QualityController.Instance.SetPresetQualitySetting(index);
        _targetPresetIndex = index;
    }

    public void ApplyQualitySetting()
    {
        QualityController.Instance.SetPresetQualitySetting(_targetPresetIndex);
    }

}
