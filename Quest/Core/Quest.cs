using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;

public class Quest : MonoBehaviour
{
    [Serializable]public class QuestCompleteEvent : UnityEvent<Quest> { }

    [SerializeField]
    private string _questName;
    [SerializeField]
    private string _questDescription;
    [SerializeField]
    private bool _autoDisables = true;
    [SerializeField]
    [Tooltip("If true the quest won't register a quest toast when the quest is added")]
    private bool _silentAdd = false;
    [SerializeField]
    [Tooltip("How long will in seconds will pass before the quest is automatically deactivated after it is completed")]
    private float _deactivationDelay = 1f;
    [SerializeField]
    private float _completionExperienceReward;
    [SerializeField]
    [Tooltip("Should the quest automatically reset itself when the player dies.")]
    private bool _resetOnPlayerDeath;
    [SerializeField]
    private QuestCompleteEvent _onQuestComplete;

    private Timer _deactivateTimer;
    private int _questNumber;
    private bool _isQuestCompleted;
    private int _objectivesCompleted;

    #region Properties
    public string QuestDescription
    {
        get
        {
            return _questDescription;
        }
    }
    public List<QuestObjective> QuestObjectives { get; set; }
    public List<QuestComponent> QuestComponents { get; set; }
    public string QuestName { get { return _questName; } }
    public bool IsQuestComplete
    {
        get
        {
            return _isQuestCompleted;
        }
        private set
        {
            _isQuestCompleted = value;
            // Unsubscribe all listener events when the quest has completed
            OnQuestObjectiveCompleted = null;
            OnQuestNumberChanged = null;
            OnQuestSelected = null;
            OnQuestCompleted = null;
        }
    }
    public bool IsQuestAdded { get; set; }
    public bool IsSilentAdd { get { return _silentAdd; } }
    /// <summary>
    /// This is a position that the player will warp to when debugging this quest.
    /// This may or may not be set as it is not a required component so check for null.
    /// </summary>
    public PlayerDebugPositionQuestComponent PlayerDebugPosition
    {
        get
        {
            var comp = GetComponentsInChildren<PlayerDebugPositionQuestComponent>(true);
            if (comp.Length > 0)
            {
                return comp[0];
            }
            return null;
        }
    }
    /// <summary>
    /// Meta data about the quest. This is the quest number within the quest journal.
    /// </summary>
    public int QuestNumber
    {
        get
        {
            return _questNumber;
        }
        set
        {
            _questNumber = value;
            if (OnQuestNumberChanged != null)
            {
                OnQuestNumberChanged(value);
            }
        }
    }
    public int NumberOfObjectives
    {
        get { return QuestObjectives.Count; }
    }
    public PathLocationQuestComponent QuestPathLocation { get; private set; }
    #endregion

    #region Events
    /// <summary>
    /// Fired when the quest has been completed
    /// </summary>
    public event Action<Quest> OnQuestCompleted;
    /// <summary>
    /// Fires when the quest has been selected via the user gui
    /// </summary>
    public event Action<Quest> OnQuestSelected;
    /// <summary>
    /// Fired when the quest number value changed. The quest number is the number of the quest associated in the quest journal.
    /// </summary>
    public event Action<int> OnQuestNumberChanged;
    /// <summary>
    /// Fired when an objective of the quest has been completed. This is fired prior to checking the completion status of the quest.
    /// </summary>
    public event Action<QuestObjective> OnQuestObjectiveCompleted;
    /// <summary>
    /// Called when the quest has been reset
    /// </summary>
    public event Action<Quest> OnQuestReset;
    /// <summary>
    /// Called when the quest has beeen initialised
    /// </summary>
    public event Action<Quest> OnQuestInitialise;
    /// <summary>
    /// Called when the quest has been added to the QuestManager
    /// </summary>
    public event Action<Quest> OnQuestAdded;
    public event Action<string> OnSpecialEvent;
    #endregion

    private void Start()
    {
        IntialiseQuest();
    }

    private void IntialiseQuest()
    {
        QuestObjectives = new List<QuestObjective>();
        _deactivateTimer = new Timer(_deactivationDelay);

        QuestObjectives = GetComponentsInChildren<QuestObjective>(true).ToList();
        QuestComponents = GetComponentsInChildren<QuestComponent>(true).ToList();
        if (QuestObjectives.Count == 0)
        {
            Debug.LogError("Quest: " + QuestDescription + " has no quest objectives");
        }

        foreach(var comp in QuestComponents)
        {
            comp.Quest = this;
        }

        if(OnQuestInitialise != null)
        {
            OnQuestInitialise(this);
        }
    }
    /// <summary>
    /// Called when the Quest has been added to the QuestManager. This essentially activates the quest.
    /// </summary>
    public void TriggerQuestAdded()
    {
        foreach (var objective in QuestObjectives)
        {
            // Listen for objective completed events.
            objective.OnObjectiveCompleted += ObjectiveCompleted;
        }

        foreach (var qc in QuestComponents)
        {
            if (qc is PathLocationQuestComponent)
            {
                QuestPathLocation = qc as PathLocationQuestComponent;
            }
        }

        CheckForCompletion();
        // QuestGUI.Instance.OnSelectedQuestChanged += OnSelectedQuestChanged;
        QuestManager.Instance.OnTrackedQuestChanged += OnSelectedQuestChanged;
        if (_resetOnPlayerDeath)
        {
            GameMainReferences.Instance.PlayerCharacter.Entity.OnKilled += PlayerKilled;
        }

        // Send Quest Added Event
        if(OnQuestAdded != null)
        {
            OnQuestAdded(this);
        }
        StartCoroutine(HandleQuestUpdate());
    }
    /// <summary>
    /// Triggers a special event to occur with the specified ID. 
    /// </summary>
    /// <param name="eventID"></param>
    public void TriggerSpecialEvent(string eventID)
    {
        if(OnSpecialEvent != null)
        {
            OnSpecialEvent(eventID);
        }
    }
    /// <summary>
    /// Event for when a new quest has been selected via the quest gui.
    /// </summary>
    /// <param name="newQuest"></param>
    private void OnSelectedQuestChanged(Quest newQuest)
    {
        foreach (var comp in QuestComponents)
        {
            comp.TriggerSelectedQuestChanged(newQuest);
        }
        if(newQuest == this)
        {
            TriggerQuestSelected();
        }
    }

    private IEnumerator HandleQuestUpdate()
    {
        while (!IsQuestComplete)
        {
            UpdateQuest();
            yield return null;
        }
    }

    /// <summary>
    /// Called when the quest should be updated. This will trigger everycomponent to update as well.
    /// </summary>
    public void UpdateQuest()
    {
        foreach(var comp in QuestComponents)
        {
            comp.TriggerQuestUpdate();
        }
    }
    /// <summary>
    /// Check to see if the quest is complete and triggers a quest completion if it is.
    /// This is automaticall called every time an objective is completed.
    /// </summary>
    private void CheckForCompletion()
    {
        if (QuestObjectives.Count == 0)
        {
            TriggerQuestComplete();
        }
    }
    /// <summary>
    /// Returns true if the passed in Entity is an Entity associated with this quest
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsEntityOfInterest(Entity entity)
    {
        foreach (var objective in QuestObjectives)
        {
            if (objective.IsEntityOfInterest(entity))
            {
                return true;
            }
        }
        return false;
    }
    #region Events

    /// <summary>
    /// Called when an objective of the quest has been completed.
    /// </summary>
    /// <param name="questObjective"></param>
    private void ObjectiveCompleted(QuestObjective questObjective)
    {
        QuestObjectives.Remove(questObjective);
        if (OnQuestObjectiveCompleted != null)
        {
            OnQuestObjectiveCompleted(questObjective);
        }
        questObjective.OnObjectiveCompleted -= ObjectiveCompleted;
        CheckForCompletion();
    }
    /// <summary>
    /// Only ever called if the quest should reset on the players death
    /// </summary>
    /// <param name="player"></param>
    private void PlayerKilled(Entity player)
    {
        TriggerQuestReset();
    }
    #endregion


    #region Triggers
    /// <summary>
    /// Triggers a reset of the quest and all its components and objectives
    /// </summary>
    public void TriggerQuestReset()
    {
        if (OnQuestReset != null)
        {
            OnQuestReset(this);
        }
    }
    /// <summary>
    /// This quest has been selected via the quest GUI. This is called from QuestGUI when the quest has been selected as the current selected quest.
    /// </summary>
    public void TriggerQuestSelected()
    {
        if (OnQuestSelected != null)
        {
            OnQuestSelected(this);
        }
    }
    /// <summary>
    /// Triggers quest completion. 
    /// </summary>
    public void TriggerQuestComplete()
    {
        if (IsQuestComplete)
        {
            Debug.LogFormat("Quest {0} tried to trigger a completion when it is already complete", QuestName);
            return;
        }
        if (OnQuestCompleted != null)
        {
            OnQuestCompleted(this);
        }
        _onQuestComplete.Invoke(this);
        IsQuestComplete = true;
        // Reward experience
        GameMainReferences.Instance.PlayerCharacter.CurrentExperience += _completionExperienceReward;
        _deactivateTimer.Reset();
        if (_resetOnPlayerDeath)
        {
            GameMainReferences.Instance.PlayerCharacter.Entity.OnKilled -= PlayerKilled;
        }
    }
    #endregion

    public void OnDrawGizmosSelected()
    {
        // Draw the debug position if it exists
        var pos = PlayerDebugPosition;
        if (pos != null)
        {
            var debugPos = pos.transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(debugPos, 1f);
        }
    }
}
