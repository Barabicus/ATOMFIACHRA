using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class TextPlayer : MonoBehaviour, IPoolable
{

    public Text talkText;
    public float textBaseTime = 1f;
    public float characterTimeGain = 0.1f;
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;
    public event Action TextFinished;

    private Queue<string> textQueue;
    private string currentText;
    private Timer timer;
    private CanvasGroup _canvasGroup;

    private float _aVel;

    protected int MaxCharacterCount { get { return 200; } }


    /// <summary>
    /// The speech bubble has finished talking and has faded out
    /// </summary>
    public event Action OnFinishedPlaying;
    /// <summary>
    /// The speech bubble has finished talking and has not yet faded out
    /// </summary>
    public event Action OnFinishedTalking;

    public bool HasFinishedPlaying { get; set; }
    public bool IsBannerFaded { get; set; }

    public virtual void Initialise()
    {
        textQueue = new Queue<string>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void PoolStart()
    {
        HasFinishedPlaying = false;
        timer = new Timer(textBaseTime);
        textQueue.Clear();
        currentText = null;
        TextFinished = null;
        gameObject.SetActive(true);
        IsBannerFaded = false;
    }

    public void Recycle()
    {
        // Ensure events are reset
        OnFinishedPlaying = null;
        OnFinishedTalking = null;
    }

    public void QueueText(string text)
    {
        string playText;
        if(text.Length > MaxCharacterCount)
        {
            text.Substring(0, MaxCharacterCount);
            text += "...";

        }
        textQueue.Enqueue(text);
        HasFinishedPlaying = false;
    }

    public void PlayNextText()
    {
        if (textQueue.Count == 0 && currentText != null && TextFinished != null)
            TextFinished();

        if (textQueue.Count > 0)
        {
            currentText = textQueue.Dequeue();
        }
        else
        {
            if (currentText != null)
            {
                if (OnFinishedTalking != null)
                {
                    OnFinishedTalking();
                }
                FinishedTalking();
            }
            currentText = null;
            HasFinishedPlaying = true;
        }
        talkText.text = currentText;
        if (currentText != null)
        {
            var charAdd = (currentText.Length * characterTimeGain);
            timer = charAdd + ((charAdd < textBaseTime) ? textBaseTime : 0f);
        }

    }

    protected virtual void Update()
    {
        if (currentText == null)
        {
            PlayNextText();
        }
        else if (timer.CanTick)
        {
            PlayNextText();
        }

        FadeBanner();

        if (IsBannerFaded && HasFinishedPlaying)
        {
            if (OnFinishedPlaying != null)
                OnFinishedPlaying();

            FinishedPlaying();
        }
    }

    protected virtual void FinishedPlaying() { }

    protected virtual void FinishedTalking() { }

    private void FadeBanner()
    {
        if (currentText != null)
        {
            // panelImage.color = Color.Lerp(panelImage.color, panelStartColor, Time.deltaTime*2f);
            // panelImage.color = SmoothColor(panelImage.color, panelStartColor, fadeInTime);
            _canvasGroup.alpha = Mathf.SmoothDamp(_canvasGroup.alpha, 1f, ref _aVel, Time.deltaTime * 2f);
        }
        else
        {
            // panelImage.color = Color.Lerp(panelImage.color, Color.clear, Time.deltaTime*5f);
            // panelImage.color = SmoothColor(panelImage.color, Color.clear, fadeOutTime);
            _canvasGroup.alpha = Mathf.SmoothDamp(_canvasGroup.alpha, 0f, ref _aVel, Time.deltaTime * 5f);
        }
        if (_canvasGroup.alpha == 0)
        {
            IsBannerFaded = true;
        }
    }

}
