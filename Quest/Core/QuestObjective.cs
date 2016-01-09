using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class QuestObjective : QuestComponent
{
    [SerializeField]
    [Tooltip("How long the objective will take to disable. Can be useful for effects and such.")]
    private float _disableTimeDelay = 0f;
    [SerializeField]
    private string _objectiveDescription;
    [Tooltip("If true this will not be shown in the task section in the quest journal")]
    [SerializeField]
    private bool _invisibleObjective;

    private Timer _disableTimer;

    public event Action<QuestObjective> OnObjectiveCompleted;
    public event Action<QuestObjective> OnDescriptionChanged;

    public virtual string QuestDescription
    {
        get
        {
            return _objectiveDescription;
        }
        set
        {
            _objectiveDescription = value;
        }
    }

    public bool IsInvisibleObjective { get { return _invisibleObjective; } set { _invisibleObjective = value; } }

    public bool ObjectiveComplete { get; private set; }


    protected PlayerController Player
    {
        get { return GameMainReferences.Instance.PlayerController; }
    }


    #region Events
    /// <summary>
    /// Called when the Quest Objective has been completed. This should be used for any objective specific cleanup.
    /// </summary>
    protected virtual void ObjectiveCompleted() { }
    /// <summary>
    /// Called when an objective should update. Note an objective should be sure to handle updates via this method and typically not
    /// via the OnQuestUpdate. OnObjectiveUpdate ensure an update only takes place while the objective is active and not the quest
    /// in it's entirety.
    /// </summary>
    protected virtual void OnObjectiveUpdate() { }
    /// <summary>
    /// Ensure the QuestObjective is Setup.
    /// </summary>
    /// <param name="quest"></param>
    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        Setup();
    }
    /// <summary>
    /// Called when a quest update has occured. Child classe should be careful when overloading this method
    /// to include a base call. If not the quest objective may behave in undesirable means. Also note that
    /// for a quest objective it would be unusual to overload this method as updates should typically be handled
    /// using OnObjectiveUpdate to ensure correct behaviour.
    /// </summary>
    /// <param name="quest"></param>
    protected override void OnQuestUpdate()
    {
        base.OnQuestUpdate();
        if (!ObjectiveComplete)
            OnObjectiveUpdate();
        else if (_disableTimer.CanTick)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Triggers
    /// <summary>
    /// Triggers a quest objective description changed event. This is to notify all listeners that they should update any text
    /// that corresponds to the quest description.
    /// </summary>
    public void TriggerDescriptionChanged()
    {
        if (OnDescriptionChanged != null)
        {
            OnDescriptionChanged(this);
        }
    }
    /// <summary>
    /// Triggers the objective to complete itself and fires the appropriate events.
    /// </summary>
    public void TriggerObjectiveComplete()
    {
        if (ObjectiveComplete)
        {
            Debug.LogErrorFormat("Tried to re-complete objective {0} for quest {1} when it is already complete", name, Quest.QuestName);
            return;
        }
        ObjectiveComplete = true;

        ObjectiveCompleted();

        if (OnObjectiveCompleted != null)
            OnObjectiveCompleted(this);

        _disableTimer.Reset();
    }
    #endregion

    #region Checks & Handles
    /// <summary>
    /// Returns true if the passed in Entity is an Entity associated with this quest objective.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual bool IsEntityOfInterest(Entity entity)
    {
        return false;
    }
    /// <summary>
    /// This is called on objective initialise to setup some core Quest Objective values.
    /// </summary>
    private void Setup()
    {
        _disableTimer = _disableTimeDelay;
        var questComps = gameObject.GetComponentsInChildren<QuestComponent>(true);
        foreach (var qc in questComps)
        {
            qc.QuestObjective = this;
        }
    }
    #endregion
}
