using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashScreenGUI : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve _fadeCurve;
    [SerializeField]
    private string _loadLevel;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    private float _currentTime;


    private void Update()
    {
        _canvasGroup.alpha = _fadeCurve.Evaluate(_currentTime);
        _currentTime += Time.deltaTime;
        if(_currentTime > _fadeCurve.keys[_fadeCurve.length - 1].time)
        {
            LoadLevel();
        }
    }

    private void LoadLevel()
    {
        Application.LoadLevel(_loadLevel);
    }

}
