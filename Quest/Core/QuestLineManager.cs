using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestLineManager : MonoBehaviour
{
    [SerializeField]
    private bool _autoStart; 

    private Queue<Quest> _quests;
    private int _currentQuestIndex;

    public Quest CurrentQuest
    {
        get
        {
            if(_quests.Count == 0)
            {
                return null;
            }
            else
            {
                return _quests.Peek();
            }
        }
    }
    public bool HasStarted { get; set; }

    private void Awake()
    {
        _quests = new Queue<Quest>(GetComponentsInChildren<Quest>(true));
    }

    private void Start()
    {
        if (_autoStart)
        {
            AdvanceQuestLine();
        }
    }
    /// <summary>
    /// Starts the initial advance quest line which will keep chaining until all quests have been completed
    /// </summary>
    public void StartQuestLine()
    {
        if (!HasStarted)
        {
            AdvanceQuestLine();
        }
    }
    /// <summary>
    /// Adds the current quest. This will handle the quest in such a way it will automatically chain
    /// to add the next one when the current one completes.
    /// </summary>
    private void AdvanceQuestLine()
    {
        HasStarted = true;
        var q = CurrentQuest;
        if(q != null)
        {
            QuestManager.Instance.AddQuest(q);
            q.OnQuestCompleted += QuestCompleted;
        }
        else
        {
            return;
        }
        // Advance the queue
        _quests.Dequeue();
    }

    private void QuestCompleted(Quest quest)
    {
        Debug.Log("Quest completed: " + quest);
        // Unsubscribe
        quest.OnQuestCompleted -= QuestCompleted;
        // Automatically advance when a quest has been completed
        AdvanceQuestLine();
    }

}
