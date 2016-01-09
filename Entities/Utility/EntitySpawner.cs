using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField]
    private Entity[] _spawnTypes;
    [SerializeField]
    private int _minLevel = 1;
    [SerializeField]
    private int _maxLevel = 1;
    [SerializeField]
    private int _spawnAmount;
    [Tooltip("How often the entity spawner will check the current amount of spawned entities to the max spawn amount. If it can it will then attempt to spawn another Entity")]
    [SerializeField]
    private float _spawnCheckFrequency;
    [SerializeField]
    private SpawnMethod _spawnMethod = SpawnMethod.Area;
    [SerializeField]
    private float _spawnRadius;
    [SerializeField]
    private Vector3 _boxBounds;
    [SerializeField]
    private bool _enableBoundsDespawning = false;
    [SerializeField]
    [Tooltip("If any entities are outside the bounds of this box and bound despawning is turned on the entities will get despawned")]
    private Vector3 _despawnBounds;
    [SerializeField]
    private bool _preWarm;
    [Tooltip("If this is true the entity spawner will keep trying to spawn. If this is false it will only spawn in the maximum amount of entities before it stops spawning")]
    [SerializeField]
    private bool _keepSpawning = true;
    [Tooltip("If not continiously spawning this is how many additional entities will be spawned")]
    [SerializeField]
    private int _spawnOverflow = 1;
    [SerializeField]
    public ObjectTransitionFx _spawnInFx;
    [SerializeField]
    private bool _preWarmUsesFX = true;
    [SerializeField]
    private bool _despawnOnCinematic = true;
    [SerializeField]
    private bool _followPlayer = false;
    [SerializeField]
    private bool _startSpawningOnStart = true;

    private List<Entity> _spawnedEntities;
    private Timer _spawnFreqTimer;
    private Transform _anchorPoint;
    // How many additional attempts we will give at spawning each entity
    private const int _spawnErrorMargin = 10;
    private int _spawnedTotalAmount;
    private int _overflow;
    private Bounds _despawnBoundingBox;

    public bool IsSpawning { get; set; }
    public Transform AnchorPoint { get { return _anchorPoint; } set { _anchorPoint = value; } }

    public enum SpawnMethod
    {
        Area,
        RandomSpecified,
        Box
    }

    private void Awake()
    {
        if (_spawnTypes == null || _spawnTypes.Length == 0)
        {
            Debug.LogErrorFormat("Entity Spawner {0} has no spawn types!");
            Destroy(this);
            return;
        }
        _overflow = _spawnAmount + _spawnOverflow;
        _spawnedEntities = new List<Entity>();
        _spawnFreqTimer = new Timer(_spawnCheckFrequency);
        _despawnBoundingBox = new Bounds(transform.position, _despawnBounds);
    }

    private void Start()
    {
        if (_followPlayer)
        {
            _anchorPoint = GameMainReferences.Instance.PlayerCharacter.transform;
        }
        else
        {
            _anchorPoint = transform;
        }

        // Try and pre warm
        if (_preWarm)
        {
            int errorMargin = 0;
            bool _giveUp = false;
            while (_spawnedEntities.Count != _spawnAmount)
            {
                while (!TrySpawn(_spawnInFx != null && _preWarmUsesFX))
                {
                    errorMargin++;
                    if (errorMargin >= _spawnErrorMargin)
                    {
                        // Give up trying to spawn
                        _giveUp = true;
                        break;
                    }
                }
                if (_giveUp)
                {
                    break;
                }
            }
        }
        if (_enableBoundsDespawning)
        {
            StartCoroutine(DoDespawnCheck());
        }
        GameMainController.Instance.OnCinematicChange += OnCinematicChange;

        if (_startSpawningOnStart)
        {
            IsSpawning = true;
        }
    }

    private void OnCinematicChange(bool value)
    {
        if (_despawnOnCinematic)
        {
            foreach (var entity in _spawnedEntities)
            {
                entity.ShouldTryEnableEntity = !value;
            }
            _spawnFreqTimer.Reset();
        }
    }

    private IEnumerator DoDespawnCheck()
    {
        while (_enableBoundsDespawning)
        {
            for (int i = _spawnedEntities.Count - 1; i >= 0; i--)
            {
                if (!IsEntityWithinBounds(_spawnedEntities[i]))
                {
                    var entity = _spawnedEntities[i];
                    _spawnedEntities.Remove(entity);
                    entity.PoolEntity();
                    Debug.Log("Despawned: " + entity);
                }
            }
            // Wait a a bit before trying to despawn again
            // Mix it up a bit to avoid a bunch of spawners trying to despawn
            yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
        }
    }

    private void Update()
    {
        transform.position = _anchorPoint.position;

        if (IsSpawning && _spawnFreqTimer.CanTickAndReset())
        {
            TrySpawn(true);
        }
    }

    private bool TrySpawn(bool useFX)
    {
        if (!_keepSpawning && _spawnedTotalAmount >= _overflow || (_despawnOnCinematic && GameMainController.Instance.IsCinematic))
        {
            return false;
        }

        // Limit amount of spawned entities
        if (_spawnedEntities.Count == _spawnAmount)
        {
            return false;
        }
        switch (_spawnMethod)
        {
            case SpawnMethod.Area:
                return DoAreaSpawn(useFX);
            case SpawnMethod.RandomSpecified:
                return DoRandomSpecifiedSpawn(useFX);
            case SpawnMethod.Box:
                return DoBoxSpawn(useFX);
        }
        return false;
    }

    private void EntityKilled(Entity entity)
    {
        // If we can spawn more entities again after being full, reset the timer
        if (_spawnedEntities.Count == _spawnAmount)
        {
            _spawnFreqTimer.Reset();
        }

        _spawnedEntities.Remove(entity);
        entity.OnKilled -= EntityKilled;
    }

    private bool DoAreaSpawn(bool useFX)
    {
        Vector3 offset = Random.insideUnitSphere * _spawnRadius;
        offset.y = _anchorPoint.position.y + 500f;
        RaycastHit hit;
        if (Physics.Raycast(_anchorPoint.position + offset, Vector3.down, out hit, 1000f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Environment") | 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Entity")))
        {
            SpawnInEntity(hit.point, useFX);
            if (!_keepSpawning)
            {
                _spawnedTotalAmount++;
            }
            return true;
        }
        return false;
    }

    private bool DoBoxSpawn(bool useFX)
    {
        var offset = new Vector3(Random.Range(-(_boxBounds.x / 2f), (_boxBounds.x / 2f)), _boxBounds.y / 2f, Random.Range(-(_boxBounds.z / 2f), (_boxBounds.z / 2f)));
        RaycastHit hit;
        if (Physics.Raycast(_anchorPoint.position + offset, Vector3.down, out hit, 1000f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Environment") | 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Entity")))
        {
            SpawnInEntity(hit.point, useFX);
            if (!_keepSpawning)
            {
                _spawnedTotalAmount++;
            }
            return true;
        }
        return false;
    }

    private bool DoRandomSpecifiedSpawn(bool useFX)
    {
        return false;
    }

    private void SpawnInEntity(Vector3 position, bool useFX)
    {
        var ent = EntityPool.Instance.GetObjectFromPool(_spawnTypes[Random.Range(0, _spawnTypes.Length)].PoolID, (e) =>
        {
            e.OnKilled += EntityKilled;
            e.LevelHandler.CurrentLevel = Random.Range(_minLevel, _maxLevel);
            e.transform.position = position;
        });

        if (_spawnInFx != null && useFX)
        {
            var fx = ObjectTransitionPool.Instance.GetObjectFromPool(_spawnInFx.PoolID, (o) =>
            {
                o.FXTransitionMethod = ObjectTransitionFx.TransitionMethod.Activate;
                o.TargetObject = ent.gameObject;
            });
            fx.gameObject.SetActive(true);
        }
        else
        {
            //   ent.gameObject.SetActive(true);
            EntityManager.Instance.AddEntity(ent);
        }
        _spawnedEntities.Add(ent);
    }
    /// <summary>
    /// Check to see if the specified entity is within spawning range of this Entity Spawners.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsEntityWithinBounds(Entity entity)
    {
        // Update bounds position
        _despawnBoundingBox.center = transform.position;
        return _despawnBoundingBox.Contains(entity.transform.position);
    }

    private void Reset()
    {
        _spawnRadius = 10f;
        _boxBounds = new Vector3(5f, 5f, 5f);
        if (name.StartsWith("GameObject"))
        {
            name = "Entity Spawner";
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        switch (_spawnMethod)
        {
            case SpawnMethod.Area:
                Gizmos.DrawWireSphere(transform.position, _spawnRadius);
                break;
            case SpawnMethod.RandomSpecified:
                break;
            case SpawnMethod.Box:
                Gizmos.DrawWireCube(transform.position, _boxBounds);
                break;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, _despawnBounds);
    }
}
