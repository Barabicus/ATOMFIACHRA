using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CanvasGroup))]
public class PauseOverlay : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve _fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField]
    private AnimationCurve _fadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private CanvasGroup _canvasGroup;
    private float _currentTime;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        GameMainController.Instance.OnPauseChange += OnPauseChange;
    }

    private void OnPauseChange(bool b)
    {
        _currentTime = 0f;
    }

    private void Update()
    {
        if (GameMainController.Instance.IsPaused)
        {
            _canvasGroup.alpha = _fadeInCurve.Evaluate(_currentTime);
        }
        else
        {
            _canvasGroup.alpha = _fadeOutCurve.Evaluate(_currentTime);

        }
        _currentTime += Time.unscaledDeltaTime;
    }

}
