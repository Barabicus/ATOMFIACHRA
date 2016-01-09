using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EntityCastingBar : MonoBehaviour
{
    [SerializeField]
    private Image _Fill;
    [SerializeField]
    private Text _TitleLabel;

    [SerializeField]
    private Image _IconImage;

    [SerializeField]
    private float _fadeOutDelay = 0.5f;

    private Entity _entity;
    private CanvasGroup _canvasGroup;
    private Timer _keepOpenTimer;
    private bool _isCasting;
    private Spell _castingSpell;

    public Entity Entity {
        get
        {
            return _entity;
        }
        set
        {
            _entity = value;
            _entity.OnSpellCast += EntityCastSpell;
        }
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        // Start faded out
        _canvasGroup.alpha = 0f;
        _keepOpenTimer = _fadeOutDelay;
    }

    private IEnumerator FadeInAnimation()
    {
        //Ensure fade out is not playing
        StopCoroutine(FadeOutAnimation());
        while(_canvasGroup.alpha < 1f)
        {
            _canvasGroup.alpha = Mathf.Clamp01(_canvasGroup.alpha + Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FadeOutAnimation()
    {
        //Ensure fade in is not playing
        StopCoroutine(FadeInAnimation());
        while (_canvasGroup.alpha > 0)
        {
            _canvasGroup.alpha = Mathf.Clamp01(_canvasGroup.alpha - Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SpellCastingAnimation()
    {
        _isCasting = true;
        StartCoroutine(FadeInAnimation());
        while (!_keepOpenTimer.CanTick)
        {
            _IconImage.sprite = _castingSpell.SpellIcon;
            _TitleLabel.text = _castingSpell.SpellName;
            yield return null;
        }
        StartCoroutine(FadeOutAnimation());
        _isCasting = false;
    }

    private void EntityCastSpell(Spell spell)
    {
        _keepOpenTimer.Reset();
        _castingSpell = spell;
        if (!_isCasting)
        {
            StartCoroutine(SpellCastingAnimation());
        }
    }
}
