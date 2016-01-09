using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
public abstract class HelpBoxInstruction : MonoBehaviour {

    [SerializeField]
    private AnimationCurve _fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField]
    private AnimationCurve _fadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    private float _fadeTime = 1;
   
    private CanvasGroup _canvasGroup;
    private float _currentTime;
    private bool _isHelpBoxCompleted;

    public event Action OnHelpBoxCompleted;

    public bool IsHelpBoxCompleted
    {
        get { return _isHelpBoxCompleted; }
        set
        {
            _isHelpBoxCompleted = value;
            if(OnHelpBoxCompleted != null)
            {
                OnHelpBoxCompleted();
            }
            // Remove all subscribers
            OnHelpBoxCompleted = null;
        }
    }
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        // Ensure it is invisible
        _canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Display the help box
    /// </summary>
    public void DisplayHelpBox()
    {
        if (IsHelpBoxCompleted)
        {
            Debug.LogErrorFormat("Help box {0} is already completed!", gameObject.name);
            return;
        }
        // Started, fade in
        OnDisplay();
        StartCoroutine(DoCheck());
    }
    /// <summary>
    /// Called when the HelpBox has been triggered to display. 
    /// This acts as a Start method.
    /// </summary>
    protected virtual void OnDisplay() { }
    /// <summary>
    /// Called as part of the HelpBox completion check
    /// </summary>
    protected virtual void CheckUpdate() { }

    private IEnumerator DoCheck()
    {
        StartCoroutine(Fade(_fadeInCurve));
        while (true)
        {
            CheckUpdate();
            if (IsHelpBoxCompleted)
            {
                // Finished, fade out
                StartCoroutine(Fade(_fadeOutCurve));
                yield break;
            }
            yield return null;
        }   
    }

    private IEnumerator Fade(AnimationCurve curve)
    {
        _currentTime = 0f;
        while(_currentTime < curve.keys[curve.length - 1].time)
        {
            _currentTime += Time.deltaTime;
            _canvasGroup.alpha = curve.Evaluate(_currentTime);
            yield return null;
        }
    }

}
