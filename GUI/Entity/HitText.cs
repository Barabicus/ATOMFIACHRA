using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HitText : MonoBehaviour, IPoolable
{
    [SerializeField]
    private RectTransform _container;
    [SerializeField]
    private Text _hitText;
    [SerializeField]
    private Color _positiveColor;
    [SerializeField]
    private Color _negativeColor;
    [SerializeField]
    private float _liveTime;
    [SerializeField]
    [Tooltip("The alpha of the text. Normalised from lifetime")]
    private AnimationCurve _lifeTimeAlpha = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [SerializeField]
    private Vector3 _minVelocity;
    [SerializeField]
    private Vector3 _maxVelocity;
    [SerializeField]
    private float _gravity = -8f;
    [Tooltip("How long the Enity will keep the reference to hit text for and incrementally add onto the hit amount")]
    [SerializeField]
    private float _keepReferenceTime;

    private Color _currentColor;
    private Timer _liveTimer;
    private Timer _keepRefTimer;

    /// <summary>
    /// The Enity that called this HitText
    /// </summary>
    public Entity Owner { get; set; }
    /// <summary>
    /// How much Health difference the Entity encountered
    ///  </summary>
    public float HitAmount { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 CurrentOffset { get; set; }

    public bool ShouldDropReference
    {
        get
        {
            if (_keepRefTimer.CanTick)
            {
                return true;
            }
            return false;
        }
    }

    public void Initialise()
    {
        _liveTimer = new Timer(_liveTime);
        _keepRefTimer = new Timer(_keepReferenceTime);
    }

    public void PoolStart()
    {
        HitAmount = 0;
        _liveTimer.Reset();
        _keepRefTimer.Reset();

        if (Owner == null)
        {
            HitTextPool.Instance.PoolObject(this);
            return;
        }
        UpdateText();
        CurrentOffset = Vector3.zero;
        Velocity = new Vector3(Random.Range(_minVelocity.x, _maxVelocity.x), Random.Range(_minVelocity.y, _maxVelocity.y), 0);
        transform.rotation = Camera.main.transform.rotation;
        transform.position = Owner.GUIHealthPoint.position;
        gameObject.SetActive(true);
    }

    public void Recycle()
    {
        Owner = null;
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        if (_liveTimer.CanTick)
        {
            HitTextPool.Instance.PoolObject(this);
            return;
        }

        UpdateText();
        CurrentOffset += Velocity * Time.deltaTime;
        transform.position += CurrentOffset;
        Velocity += new Vector3(0f, _gravity, 0) * Time.deltaTime;
    }

    private void UpdateText()
    {
        if (HitAmount < 0)
        {
            _currentColor = _negativeColor;
        }
        else if (HitAmount == 0)
        {
            _currentColor = Color.clear;
        }
        else
        {
            _currentColor = _positiveColor;
        }

        Color c = _currentColor;
        c.a = c.a * _lifeTimeAlpha.Evaluate(_liveTimer.CurrentTimeNormal);

        _hitText.color = c;
        _hitText.text = Mathf.Floor(HitAmount).ToString();
    }
}


