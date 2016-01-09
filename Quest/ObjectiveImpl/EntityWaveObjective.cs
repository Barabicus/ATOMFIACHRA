using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[QuestCategory("Entity Wave", QuestCategory.Entity)]
public class EntityWaveObjective : QuestObjective
{
    [SerializeField]
    private Transform _waveContainer;
    [SerializeField]
    private bool _registerQuestArrows = false;
    [SerializeField]
    private ObjectTransitionFx spawnInFx;
    [SerializeField]
    [Tooltip("If true the entity wave will automatically progress. If false the progression of the waves will have to be controlled by external means")]
    private bool _autoProgess = true;
    [SerializeField]
    [Tooltip("The time offset between spawning each entity")]
    private float _spawnEntitiesOffset = 0f;

    private WaveProperty[] _entityWaveGroups;
    private int _currentWaveIndex = -1;
    private List<Entity> _currentWaveEntities;
    // How many entities need to be killed for the selected wave
    private int _killAmount;
    private Dictionary<Entity, ArrowTaskOverlay> _taskOverlays;

    /// <summary>
    /// Fired when a wave is a completed. The wave index number is passed in.
    /// </summary>
    public event Action<int> OnWaveAdvance;

    public override string QuestDescription
    {
        get { return string.Format(base.QuestDescription, "(" + _currentWaveEntities.Count + " / " + _killAmount + ")"); }
    }

    public bool AutoProgress
    {
        get { return _autoProgess; }
        set
        {
            _autoProgess = value;
            if (value)
            {
                CheckWave();
            }
        }
    }

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        _entityWaveGroups = _waveContainer.GetComponentsInChildren<WaveProperty>(true);
        // Construct Wave details
        foreach (var w in _entityWaveGroups)
        {
            w.ConstructEntityDetails();
        }
        _currentWaveEntities = new List<Entity>();
        _taskOverlays = new Dictionary<Entity, ArrowTaskOverlay>();
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        StartWave();
    }

    protected override void OnQuestReset()
    {
        base.OnQuestReset();
        StartWave();
    }

    private void StartWave()
    {
        // Reset the wave index
        _currentWaveIndex = -1;
        // Ensure all leftover entities are cleaned up
        foreach (var e in _currentWaveEntities)
        {
            e.OnKilled -= OnEntityKilled;
            e.PoolEntity();
        }
        _currentWaveEntities.Clear();

        // Set all waves as disabled
        foreach (var entityWaveGroup in _entityWaveGroups)
        {
            entityWaveGroup.gameObject.SetActive(false);
        }

        if (AutoProgress)
        {
            CheckWave();
        }
        // Ensure description is updated
        TriggerDescriptionChanged();
    }

    private void OnEntityKilled(Entity entity)
    {
        // DeRegister the kill event
        // By the time all the entities are killed all the events should have deregistered
        entity.OnKilled -= OnEntityKilled;
        DeRegisterEntityArrow(entity);
        _currentWaveEntities.Remove(entity);
        TriggerDescriptionChanged();
        if (HasTaskCompleted())
        {
            return;
        }
        if (AutoProgress)
        {
            CheckWave();
        }
    }
    /// <summary>
    /// Checks the wave to see if it should advance onto the next wave
    /// </summary>
    private void CheckWave()
    {
        if (_currentWaveEntities.Count == 0)
        {
            AdvanceWave();
        }
    }

    private bool HasTaskCompleted()
    {
        if (_currentWaveIndex == _entityWaveGroups.Length - 1 && _currentWaveEntities.Count == 0)
        {
            TriggerObjectiveComplete();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Advances the wave to spawn in the next set of wave entities
    /// </summary>
    public void AdvanceWave()
    {
        _currentWaveIndex++;
        if (_currentWaveIndex == _entityWaveGroups.Length)
        {
            return;
        }

        // Load all entities from the selected transform
        _currentWaveEntities = _entityWaveGroups[_currentWaveIndex].GenerateEntitiesFromWave().ToList();
        _killAmount = _currentWaveEntities.Count;

        foreach (var currentWaveEntity in _currentWaveEntities)
        {
            // Listen for each entity's death
            currentWaveEntity.OnKilled += OnEntityKilled;
        }

        StartCoroutine(DoSpawnEntities());

        // Enable the actual gameobject now that everything has been properly set
        _entityWaveGroups[_currentWaveIndex].gameObject.SetActive(true);

        // Send the wave advance event but ignore the initial wave
        if (_currentWaveIndex != 0 && OnWaveAdvance != null)
        {
            OnWaveAdvance(_currentWaveIndex);
        }
        
        // Try and register all quest arrows only if the quest is selected
        // This is a recheck of quest arrows since after the wave has finished
        // the next wave has not been checked to see if they should have quest arrows
        if(QuestManager.Instance.TrackedQuest == Quest)
        {
            RegisterAllEntityArrows();
        }
    }

    private IEnumerator DoSpawnEntities()
    {
        var entityList = _currentWaveEntities.ToList();
        // Use a transition if it is not null
        if (spawnInFx != null)
        {
            foreach (var entity in entityList)
            {
                // entity.gameObject.SetActive(false);
                var transition = ObjectTransitionPool.Instance.GetObjectFromPool(spawnInFx.PoolID, (o) =>
                {
                    o.FXTransitionMethod = ObjectTransitionFx.TransitionMethod.Activate;
                    o.TargetObject = entity.gameObject;
                });
                transition.gameObject.SetActive(true);
                yield return new WaitForSeconds(_spawnEntitiesOffset);
            }
        }
        else
        {
            // No transition force a spawn.
            foreach (var entity in entityList)
            {
                entity.ShouldTryEnableEntity = true;
                yield return new WaitForSeconds(_spawnEntitiesOffset);
            }
        }
    }

    protected override void OnQuestSelected()
    {
        base.OnQuestSelected();
        if (_registerQuestArrows)
        {
            RegisterAllEntityArrows();
        }
    }

    private void RegisterAllEntityArrows()
    {
        foreach (var currentWaveEntity in _currentWaveEntities)
        {
            RegisterEntityArrow(currentWaveEntity);
        }
    }

    protected override void OnSelectedQuestChanged(Quest newQuest)
    {
        base.OnSelectedQuestChanged(newQuest);
        _taskOverlays.Clear();
    }

    private void RegisterEntityArrow(Entity entity)
    {
        if (!_registerQuestArrows)
        {
            return;
        }
        if (IsEntityOfInterest(entity))
        {
            if (!_taskOverlays.ContainsKey(entity))
            {
                _taskOverlays.Add(entity, QuestOverlayFactory.CreateQuestArrow(entity.transform, Color.yellow, Quest));
            }
        }
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

    public override bool IsEntityOfInterest(Entity entity)
    {
        return _currentWaveEntities.Contains(entity);
    }

    private void Reset()
    {
        if (_waveContainer == null)
        {
            _waveContainer = new GameObject("WaveContainer").transform;
            _waveContainer.SetParent(transform);
            _waveContainer.localPosition = Vector3.zero;

            // Add an initial wave
            var wave = new GameObject("Wave").transform;
            wave.SetParent(_waveContainer);
            wave.localPosition = Vector3.zero;
            wave.gameObject.AddComponent<WaveProperty>();
        }
    }
}
