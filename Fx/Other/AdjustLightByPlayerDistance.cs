using UnityEngine;
using System.Collections;
[DisallowMultipleComponent]
public class AdjustLightByPlayerDistance : MonoBehaviour
{

    private float _startIntensity;
    private Light _light;
    private float _lerpInDistance = 25f;
    private float _lerpOutDistance = 30f;
    private bool _isWithinDistance;
    private float _updateFreq;
    private Timer _timer;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _startIntensity = _light.intensity;
        _updateFreq = Random.Range(0.2f, 0.75f);
        _timer = _updateFreq;
    }

    private void Update()
    {
        if (_timer.CanTickAndReset())
        {
            DoCheck();
        }
        LerpLight();
    }

    private void DoCheck()
    {
        var distance = Vector3.Distance(GameMainReferences.Instance.PlayerController.transform.position, transform.position);
        if (_isWithinDistance && distance >= _lerpOutDistance)
        {
            _isWithinDistance = false;
        }
        else if (!_isWithinDistance && distance < _lerpInDistance)
        {
            _isWithinDistance = true;
        }
    }

    private void LerpLight()
    {
        _light.intensity = Mathf.Lerp(_light.intensity, _isWithinDistance ? _startIntensity : 0f, Time.deltaTime * 5f);
    }
}
