using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[QuestCategory("Kill Entities", QuestCategory.Entity)]
public class KillEntitiesObjective : QuestObjective
{
    [SerializeField]
    private int _killAmount = 1;
    [SerializeField]
    private Entity[] _targetEntityIDs;
    [SerializeField]
    private bool _generateQuestArrows = false;

    private List<string> _targetIDs;
    private List<Entity> _autoEntities;
    private int _currentKillAmount = 0;
    private Dictionary<Entity, ArrowTaskOverlay> _taskOverlays;

    public int AmountToKill
    {
        get
        {
            return _killAmount;
        }
    }

    public override string QuestDescription
    {
        get { return String.Format(base.QuestDescription, "(" + (_killAmount - _currentKillAmount) + " / " + _killAmount + ")"); }
    }

    public override bool IsEntityOfInterest(Entity entity)
    {
        return _targetIDs.Contains(entity.EntityID);
    }

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        _taskOverlays = new Dictionary<Entity, ArrowTaskOverlay>();

        _targetIDs = new List<string>();
        foreach (var ent in _targetEntityIDs)
        {
            if (!_targetIDs.Contains(ent.EntityID) && ent.EntityID != "")
                _targetIDs.Add(ent.EntityID);
        }

    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        EntityManager.Instance.OnEntityEnabled += OnEntityEnabled;
        EntityManager.Instance.OnEntityDisabled += OnEntityDisabled;
        EntityManager.Instance.OnEntityKilled += OnEntityKilled;
        CheckForCompletion();
    }

    protected override void OnQuestSelected()
    {
        base.OnQuestSelected();
        // Register quest arrows
        foreach (var loadedEntity in EntityManager.Instance.LoadedEntities)
        {
            RegisterEntityArrow(loadedEntity);
        }
    }

    protected override void ObjectiveCompleted()
    {
        base.ObjectiveCompleted();
        EntityManager.Instance.OnEntityEnabled -= OnEntityEnabled;
        EntityManager.Instance.OnEntityDisabled -= OnEntityDisabled;
        EntityManager.Instance.OnEntityKilled -= OnEntityKilled;
    }

    private void RegisterEntityArrow(Entity entity)
    {
        if (!_generateQuestArrows || true)
        {
            return;
        }
        if (IsEntityOfInterest(entity))
            _taskOverlays.Add(entity, QuestOverlayFactory.CreateQuestArrow(entity.transform, Color.yellow, Quest));
    }

    private void DeRegisterEntityArrow(Entity entity)
    {
        ArrowTaskOverlay overlay;
        if (_taskOverlays.TryGetValue(entity, out overlay))
        {
            _taskOverlays.Remove(entity);
            overlay.DeRegisterQuestTaskOverlay();
        }
    }

    private void OnEntityDisabled(Entity obj)
    {
        DeRegisterEntityArrow(obj);
    }

    private void OnEntityEnabled(Entity obj)
    {
        RegisterEntityArrow(obj);
    }

    private void OnEntityKilled(Entity e)
    {
        if (_targetIDs.Contains(e.EntityID))
        {
            DeRegisterEntityArrow(e);
            _currentKillAmount++;
            CheckForCompletion();
        }
        TriggerDescriptionChanged();
    }

    private void CheckForCompletion()
    {
        if (_killAmount == _currentKillAmount)
        {
            TriggerObjectiveComplete();
        }
    }
}
