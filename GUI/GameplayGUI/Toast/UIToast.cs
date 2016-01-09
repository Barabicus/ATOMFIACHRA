using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(AudioSource))]
public class UIToast : MonoBehaviour
{
    [SerializeField]
    private Transform _toastContainer;
    [SerializeField]
    private AnimationCurve _alphaAnimation;
    [SerializeField]
    private SpellUnlockedToast _spellUnlockedPrefab;
    [SerializeField]
    private QuestAddedToast _questAddedPrefab;
    [SerializeField]
    private GeneralToast _spawnPointToastPrefab;

    private IToastable _activeToast;
    private Queue<IToastable> _toastQueue;
    private float _queueTime;
    private float _finishTime;
    private CanvasGroup _contCanvasGroup;
    private AudioSource _audioSource;

    public static UIToast Instance { get; set; }

    private IToastable ActiveToast
    {
        get
        {
            return _activeToast;
        }
        set
        {
            _activeToast = value;

            if (value == null)
            {
                return;
            }
            _queueTime = 0f;
            value.GameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        Instance = this;
        _toastQueue = new Queue<IToastable>();
        _contCanvasGroup = _toastContainer.GetComponent<CanvasGroup>();
        _audioSource = GetComponent<AudioSource>();
        // Calculate finish time
        _finishTime = _alphaAnimation.keys[_alphaAnimation.length - 1].time;
    }

    private void Start()
    {
        // Register to spell unlocked events
        //  GameMainReferences.Instance.PlayerCharacter.OnSpellUnlocked += OnSpellUnlocked;
        // Register to quest added events
        QuestManager.Instance.OnQuestAdded += OnQuestAdded;
    }

    public void DoSpellUnlocked(Spell spell, RectTransform overlayEndRect = null)
    {
        var toast = Instantiate(_spellUnlockedPrefab) as SpellUnlockedToast;
        toast.OverlayEndRect = overlayEndRect;
        toast.SetupToast(spell);
        QueueToast(toast);
    }

    public void DoSpawnPointToast(string text)
    {
        var toast = Instantiate(_spawnPointToastPrefab) as GeneralToast;
        toast.SetupToast(text);
        QueueToast(toast);
    }

    private void OnQuestAdded(Quest quest)
    {
        if (quest.IsSilentAdd)
        {
            return;
        }
        var toast = Instantiate(_questAddedPrefab) as QuestAddedToast;
        toast.SetupToast(quest);
        QueueToast(toast);
    }

    private void QueueToast(IToastable toast)
    {
        toast.GameObject.SetActive(false);
        toast.GameObject.transform.SetParent(_toastContainer);
        toast.GameObject.transform.localPosition = Vector3.zero;
        _toastQueue.Enqueue(toast);
        if(ActiveToast == null)
        {
            AdvanceToast();
        }
    }

    private void AdvanceToast()
    {
        // Remove the current active toast
        if (ActiveToast != null)
        {
            Destroy(ActiveToast.GameObject);
        }
        ActiveToast = null;
        // Check if there is one inline and advance it
        if (_toastQueue.Count > 0)
        {
            ActiveToast = _toastQueue.Dequeue();
            if(ActiveToast.ToastAppearedSound != null)
            {
                _audioSource.PlayOneShot(ActiveToast.ToastAppearedSound);
            }
        }
    }

    private void Update()
    {
        if (ActiveToast != null)
        {
            _queueTime += Time.deltaTime;
            // play the animation
            _contCanvasGroup.alpha = _alphaAnimation.Evaluate(_queueTime);
            if (_queueTime >= _finishTime)
            {
                AdvanceToast();
            }
        }
    }

    #region Register Toasts

    public void RegisterTextToast(string text)
    {

    }

    #endregion
}
