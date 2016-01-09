using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class EntityStatusText : MonoBehaviour, IPoolable
{
    [SerializeField]
    private Text _statusText;
    [SerializeField]
    private float _liveTime = 2f;
    [SerializeField]
    private AnimationCurve _lifeTimeAlpha;
    [SerializeField]
    private AnimationCurve _lifeTimeYOffset;

    private CanvasGroup _canvasGroup;
    private Timer _livingTimer;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    /// <summary>
    /// The target this text should move to
    /// </summary>
    public Transform Target { get; set; }
    /// <summary>
    /// The text that should be displayed
    /// </summary>
    public string StatusText
    {
        get { return _statusText.text; }
        set { _statusText.text = value; }
    }
    /// <summary>
    /// The color of the text
    /// </summary>
    public Color TextColor
    {
        get { return _statusText.color; }
        set { _statusText.color = value; }
    }
    public Vector3 Offset
    {
        get { return new Vector3(0, _lifeTimeYOffset.Evaluate(_livingTimer.CurrentTimeNormal), 0); }
    }

    public void Initialise()
    {
        _livingTimer = _liveTime;
        transform.rotation = GameMainReferences.Instance.RTSCamera.transform.rotation;
    }

    public void PoolStart()
    {
        _livingTimer.Reset();
    }

    public void Recycle()
    {
    }

    private void LateUpdate()
    {
        transform.position = Target.position + Offset;
        _canvasGroup.alpha = _lifeTimeAlpha.Evaluate(_livingTimer.CurrentTimeNormal);
    }

}
