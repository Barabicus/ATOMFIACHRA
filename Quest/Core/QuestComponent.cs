using UnityEngine;
using System.Collections;
using System;

public abstract class QuestComponent : MonoBehaviour
{
    private Quest _quest;
    private QuestObjective _questObjective;
    public Quest Quest
    {
        get { return _quest; }
        set
        {
            _quest = value;
            value.OnQuestCompleted += TriggerQuestCompleted;
            value.OnQuestSelected += TriggerQuestSelected;
            value.OnQuestAdded += TriggerQuestAdded;
            value.OnQuestInitialise += TriggerQuestInitialise;
            value.OnQuestReset += TriggerQuestReset;
            value.OnSpecialEvent += TriggerSpecialEvent;
            // Create an automatic event unsubscribe for when the quest has been completed
            Action<Quest> complAction = null;
            complAction = (q) =>
            {
                value.OnQuestCompleted -= TriggerQuestCompleted;
                value.OnQuestSelected -= TriggerQuestSelected;
                value.OnQuestAdded -= TriggerQuestAdded;
                value.OnQuestInitialise -= TriggerQuestInitialise;
                value.OnQuestReset -= TriggerQuestReset;
                value.OnSpecialEvent -= TriggerSpecialEvent;

                // Ensure to desub the complete action
                value.OnQuestCompleted -= complAction;

            };
            // Listen for quest completion and unsubscribe
            value.OnQuestCompleted += complAction;
        }
    }
    /// <summary>
    /// The parent QuestObjective associated with this quest component. This will not necessarily always have
    /// to have a value. It will only be not null when the questcomponent has been parented to a quest objective.
    /// Doing this allows the questcomponent to behave in response to the quest objective.
    /// </summary>
    public QuestObjective QuestObjective
    {
        get
        {
            return _questObjective;
        }
        set
        {
            value.OnObjectiveCompleted += OnQuestObjectiveCompleted;
            _questObjective = value;
        }
    }

    #region Events
    /// <summary>
    /// This is called as the quest has been initialised. This should be used to setup initial variables and other such references or state handling.
    /// </summary>
    /// <param name="quest"></param>
    protected virtual void OnQuestInitialise() { }
    /// <summary>
    /// This is called when the quest has been started
    /// </summary>
    protected virtual void OnQuestAdded() { }
    /// <summary>
    /// This is called when the quest is completed
    /// </summary>
    /// <param name="quest"></param>
    protected virtual void OnQuestCompleted() { }
    /// <summary>
    /// This is called by the controlling quest of this component to trigger an update.
    /// </summary>
    protected virtual void OnQuestUpdate() { }
    /// <summary>
    /// Called when a new quest has been selected from the quest GUI. Use this to deregister any quest related
    /// GUI elements associated with this quest.
    /// </summary>
    /// <param name="newQuest">The quest that was selected. Can be null.</param>
    protected virtual void OnSelectedQuestChanged(Quest newQuest) { }
    /// <summary>
    /// Called when this quest has been selected via the quest GUI. Use this to setup any quest
    /// related GUI elements associated with this quest.
    /// </summary>
    protected virtual void OnQuestSelected() { }
    /// <summary>
    /// This is called if the quest component has a parent quest objective and the Objective has just been completed
    /// This is useful for dealing with objective specific needs.
    /// </summary>
    /// <param name="objective"></param>
    protected virtual void OnQuestObjectiveCompleted(QuestObjective objective) { }
    /// <summary>
    /// This is called if the quest has been reset. In doing so all subsequent components should properly
    /// reset themself to reflect their start state.
    /// </summary>
    protected virtual void OnQuestReset() { }
    /// <summary>
    /// The standard unity reset method call.
    /// </summary>
    protected virtual void Reset() { }
    /// <summary>
    /// Called on Special Event. This can be called to notify components of a non general specific event.
    /// </summary>
    /// <param name="eventID">The ID of the event called</param>
    protected virtual void OnSpecialEvent(string eventID) { }
    #endregion

    #region Triggers
    /// <summary>
    /// Called when the quest component should register a quest update
    /// </summary>
    public void TriggerQuestUpdate()
    {
        OnQuestUpdate();
    }
    public void TriggerSelectedQuestChanged(Quest newQuest)
    {
        OnSelectedQuestChanged(newQuest);
    }
    private void TriggerQuestCompleted(Quest quest)
    {
        OnQuestCompleted();
    }
    private void TriggerQuestSelected(Quest quest)
    {
        OnQuestSelected();
    }
    private void TriggerQuestAdded(Quest quest)
    {
        OnQuestAdded();
    }
    private void TriggerQuestInitialise(Quest quest)
    {
        OnQuestInitialise();
    }
    private void TriggerQuestReset(Quest quest)
    {
        OnQuestReset();
    }
    private void TriggerSpecialEvent(string eventID)
    {
        OnSpecialEvent(eventID);
    }
    #endregion

}
