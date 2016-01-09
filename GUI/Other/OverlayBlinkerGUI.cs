using UnityEngine;
using System.Collections;
using UnityEngine.UI.Tweens;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class OverlayBlinkerGUI : MonoBehaviour
{

    [SerializeField]
    private int _blinkAmount;
    [SerializeField]
    private float _blinkFrequency = 1f;
    [SerializeField]
    private float _alphaTarget;

    private CanvasGroup _canvasGroup;
    private bool _isBlinking;
    private TweenRunner<FloatTween> _floatTweenRunner;
    private int _currentBlinks;
    private bool _blinkLock;

    private void Awake()
    {
        _floatTweenRunner = new TweenRunner<FloatTween>();
        _floatTweenRunner.Init(this);
        _canvasGroup = GetComponent<CanvasGroup>();
        // Ensure it starts faded out
        _canvasGroup.alpha = 0f;
    }

    public void OnValidate()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public void TryBlink()
    {
        if (!_isBlinking)
        {
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        _isBlinking = true;
        while (_currentBlinks != _blinkAmount)
        {
            // Blink in
            _blinkLock = true;
            DoTween(_alphaTarget);
            // Wait for the blink to be finished
            while (_blinkLock) { yield return null; }
            // Fade out
            _blinkLock = true;
            DoTween(0f);
            // Wait for blink to be finished
            while (_blinkLock) { yield return null; }
            _currentBlinks++;
        }
        _isBlinking = false;

    }

    private void DoTween(float target)
    {
        var floatTween = new FloatTween { duration = _blinkFrequency, startFloat = _canvasGroup.alpha, targetFloat = target };
        floatTween.AddOnChangedCallback(SetCanvasAlpha);
        floatTween.AddOnFinishCallback(() => { _blinkLock = false; });
        floatTween.ignoreTimeScale = true;
        floatTween.easing = TweenEasing.Linear;
        this._floatTweenRunner.StartTween(floatTween);
    }

    private void SetCanvasAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
    }
}
