using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class QuestManager : GameController
{

    public static QuestManager Instance { get; set; }

    public List<Quest> Quests { get; set; }


    private Quest _trackedQuest;

    #region Events
    public event Action<Quest> OnQuestAdded;
    public event Action<Quest> OnTrackedQuestChanged;

    #endregion

    public override void OnAwake()
    {
        base.OnAwake();
        Quests = new List<Quest>();
        Instance = this;
    }

    public Quest TrackedQuest
    {
        get { return _trackedQuest; }
        set
        {
            _trackedQuest = value;
            if (OnTrackedQuestChanged != null)
            {
                OnTrackedQuestChanged(value);
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        EntityManager.Instance.OnEntityEnabled += EntityEvent;
        EntityManager.Instance.OnEntityAdded += EntityEvent;
        // Check all entities loaded
        CheckAllEntitiesForQuestAssociation();
    }

    private void EntityEvent(Entity obj)
    {
        SetEntityQuestAssociation(obj);
    }

    private void CheckAllEntitiesForQuestAssociation()
    {
        foreach (var entity in EntityManager.Instance.LoadedEntities)
        {
            SetEntityQuestAssociation(entity);
        }
    }

    private void SetEntityQuestAssociation(Entity entity)
    {
        foreach (var quest in Quests)
        {
            if (quest.IsEntityOfInterest(entity))
            {
                if (entity.HealthBar != null)
                {
                    entity.HealthBar.AssociatedQuest = quest;
                }
                return;
            }
        }
        if (entity.HealthBar != null)
        {
            entity.HealthBar.AssociatedQuest = null;
        }
    }

    public void AddQuest(Quest quest)
    {
        if (Quests.Contains(quest))
        {
            Debug.LogErrorFormat("Quest {0} has already been added!", quest.name);
        }
        Quests.Add(quest);
        quest.IsQuestAdded = true;
        quest.OnQuestCompleted += OnQuestComplete;
        quest.TriggerQuestAdded();
        if (OnQuestAdded != null)
            OnQuestAdded(quest);
        // A new quest has been added recheck all associations
        CheckAllEntitiesForQuestAssociation();
        // Check all entities when an objective has been completed
        quest.OnQuestObjectiveCompleted += QuestObjectiveCompleted;

        TryAutoSetTrackedQuest();
        AssignAllQuestNumbers();
    }
    /// <summary>
    /// Each quest has an associated number with it. This is meta info about the quest and it's position in the quest journal. 
    /// This is useful for displaying various things in game that we want to highlight about specific quests, by associating them with numbers.
    /// When a quest ia added we must update all quests to reflect their numbers accordingly.
    /// </summary>
    private void AssignAllQuestNumbers()
    {
        int i = 1;
        foreach (var quest in Quests)
        {
            quest.QuestNumber = i;
            i++;
        }
    }


    private void QuestObjectiveCompleted(QuestObjective questObjective)
    {
        CheckAllEntitiesForQuestAssociation();
        TryAutoSetTrackedQuest();
    }

    private void OnQuestComplete(Quest quest)
    {
        Quests.Remove(quest);
        quest.IsQuestAdded = false;
        CheckAllEntitiesForQuestAssociation();
        if (quest == TrackedQuest)
        {
            TrackedQuest = null;
        }
    }

    private void TryAutoSetTrackedQuest()
    {
        // Quick check to ensure the tracked quest is not complete
        if (TrackedQuest != null && TrackedQuest.IsQuestComplete)
        {
            TrackedQuest = null;
        }
        // If we have not tracked quest try and auto set one
        if (TrackedQuest == null)
        {
            if (Quests.Count > 0)
            {
                TrackedQuest = Quests[0];
            }
        }
    }
}
