using UnityEngine;
using System.Collections;

public class StandardQuestComponent : QuestComponent
{
    [SerializeField]
    private QuestComponentTriggerEvent triggerEvent;
    [SerializeField]
    private bool _singleShot = true;
    [SerializeField]
    private float _timeEventDelay = 1f;

    private Timer _eventTimer;
    private bool _hasFired;

    public QuestComponentTriggerEvent TriggerEvent { get { return triggerEvent; } set { triggerEvent = value; } }
    /// <summary>
    /// Event occurs when the quest has been added and trigger event is set to QuestAdded
    /// </summary>
    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();

        switch (triggerEvent)
        {
            case QuestComponentTriggerEvent.Timed:
                _eventTimer = new Timer(_timeEventDelay);
                break;
            case QuestComponentTriggerEvent.Added:
                DoEventTriggered(new QuestComponentStandardEventAgrs(QuestComponentTriggerEvent.Added));
                break;
        }
    }
    /// <summary>
    /// Event occurs when the quest has been Completed and trigger event is set to QuestCompleted
    /// </summary>
    protected override void OnQuestCompleted()
    {
        base.OnQuestCompleted();
        if(triggerEvent == QuestComponentTriggerEvent.Completed)
        {
            DoEventTriggered(new QuestComponentStandardEventAgrs(QuestComponentTriggerEvent.Completed));
        }
    }

    protected override void OnQuestUpdate()
    {
        base.OnQuestUpdate();
        // Event triggered on Timed Event
        if(triggerEvent == QuestComponentTriggerEvent.Timed && !_hasFired && _eventTimer.CanTickAndReset())
        {
            DoEventTriggered(new QuestComponentStandardEventAgrs(QuestComponentTriggerEvent.Timed));
            if (_singleShot)
            {
                _hasFired = true;
            }
        }else if(triggerEvent == QuestComponentTriggerEvent.Update)
        {
            // Event triggered on quest update.
            DoEventTriggered(new QuestComponentStandardEventAgrs(QuestComponentTriggerEvent.Update));
        }
    }
    /// <summary>
    /// Event triggered when the quest has been selected via the QuestGUI interface
    /// </summary>
    protected override void OnQuestSelected()
    {
        base.OnQuestSelected();
        if(triggerEvent == QuestComponentTriggerEvent.Selected)
        {
            DoEventTriggered(new QuestComponentStandardEventAgrs(QuestComponentTriggerEvent.Selected));
        }
    }
    /// <summary>
    /// Event triggered when a special event occurs
    /// </summary>
    /// <param name="eventID"></param>
    protected override void OnSpecialEvent(string eventID)
    {
        base.OnSpecialEvent(eventID);
        if(triggerEvent == QuestComponentTriggerEvent.SpecialEvent)
        {
            var args = new QuestComponentStandardEventAgrs(QuestComponentTriggerEvent.SpecialEvent);
            args.EventID = eventID;
            DoEventTriggered(args);
        }
    }
    /// <summary>
    /// Called when the quest has been reset. 
    /// </summary>
    protected override void OnQuestReset()
    {
        base.OnQuestReset();
        if (triggerEvent == QuestComponentTriggerEvent.Selected)
        {
            DoEventTriggered(new QuestComponentStandardEventAgrs(QuestComponentTriggerEvent.Reset));
        }
    }
    protected virtual void DoEventTriggered(QuestComponentStandardEventAgrs args) { }

}

public struct QuestComponentStandardEventAgrs
{
    public QuestComponentTriggerEvent TriggerEvent { get; private set;}
    public string EventID { get; set; }

    public QuestComponentStandardEventAgrs(QuestComponentTriggerEvent triggerEvent)
    {
        TriggerEvent = triggerEvent;
        EventID = "";
    }
}

public enum QuestComponentTriggerEvent
{
    Added,
    Completed,
    Selected,
    Timed,
    SpecialEvent,
    Update,
    Reset
}
