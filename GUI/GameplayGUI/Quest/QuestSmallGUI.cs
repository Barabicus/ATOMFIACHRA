using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.UI.Tweens;
using UnityEngine.Events;

public class QuestSmallGUI : MonoBehaviour
{
    [Serializable]
    public class TrackedQuestChangedEvent : UnityEvent<Quest> { }

    [SerializeField]
    private Text _questTitle;
    [SerializeField]
    private Transform _objectivesContainer;
    [SerializeField]
    private QuestItemSmallGUI _questObjectivePrefab;
    [SerializeField]
    private float _hideDuration = 0.5f;
    [SerializeField]
    private TrackedQuestChangedEvent _onTrackedQuestChanged;

    [NonSerialized]
    private TweenRunner<FloatTween> m_FloatTweenRunner;

    private Quest m_targetQuest;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        if (this.m_FloatTweenRunner == null)
            this.m_FloatTweenRunner = new TweenRunner<FloatTween>();

        this.m_FloatTweenRunner.Init(this);

        _canvasGroup = GetComponent<CanvasGroup>();
        // Set to invisible initially.
        _canvasGroup.alpha = 0f;
        // Listen for tracked quest changes
        QuestManager.Instance.OnTrackedQuestChanged += TrackedQuestChanged;
    }

    private void TrackedQuestChanged(Quest quest)
    {
        if (m_targetQuest == null)
        {
            // Invoke when a new quest has been added when the user previously had none.
            _onTrackedQuestChanged.Invoke(quest);
        }
        m_targetQuest = quest;
        HideObjectives();
    }

    private void RebuildQuestObjectives()
    {
        ClearList();
        if (m_targetQuest != null)
        {
            _questTitle.text = m_targetQuest.QuestName;
            foreach (var objective in m_targetQuest.QuestObjectives)
            {
                var item = Instantiate(_questObjectivePrefab);
                item.SetQuestObjective(objective);
                item.gameObject.transform.SetParent(_objectivesContainer.transform);
            }
            ShowObjectives();
        }
    }

    private void HideObjectives()
    {
        var floatTween = new FloatTween { duration = _hideDuration, startFloat = this._canvasGroup.alpha, targetFloat = 0f };
        floatTween.AddOnChangedCallback(SetCanvasAlpha);
        floatTween.AddOnFinishCallback(OnHideTweenFinished);
        this.m_FloatTweenRunner.StartTween(floatTween);
    }

    private void ShowObjectives()
    {
        var floatTween = new FloatTween { duration = _hideDuration, startFloat = this._canvasGroup.alpha, targetFloat = 1f };
        floatTween.AddOnChangedCallback(SetCanvasAlpha);
        //  floatTween.AddOnFinishCallback(OnHideTweenFinished);
        this.m_FloatTweenRunner.StartTween(floatTween);
    }

    /// <summary>
    /// Sets the canvas alpha.
    /// </summary>
    /// <param name="alpha">Alpha.</param>
    protected void SetCanvasAlpha(float alpha)
    {
        if (this._canvasGroup == null)
            return;

        // Set the alpha
        this._canvasGroup.alpha = alpha;
    }

    /// <summary>
    /// Raises the hide tween finished event.
    /// </summary>
    protected virtual void OnHideTweenFinished()
    {
        RebuildQuestObjectives();
    }

    private void ClearList()
    {
        foreach (Transform t in _objectivesContainer.transform)
        {
            Destroy(t.gameObject);
        }
    }
}
