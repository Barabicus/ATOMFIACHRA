using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages entities for gameplay. This keeps all references to them and reports events. This manages activating and deactivating entities when out of 
/// range of the player for perfomance issues. An Entity's gameobject should never be set active or inactive outside of this class. The EntityManager should
/// handle this. There are special cases in which an Entity's active state can be manipulated. Such as ticking "Ignore Disable Distance" or the
/// "ShouldTryEnable" property on the Entity. The ShouldTryEnable property allows Entities to decide whether or not they should be
///  activated when they are eligible to do so.
/// </summary>
public class EntityManager : GameController
{

    public const float chunkSize = 10f;

    private List<Entity> _loadedEntities;
    private Entity _player;
    public const float updateDistance = 60f;

    public static EntityManager Instance { get; set; }

    public Entity Player
    {
        get
        {
            // Cache the player in this manner to avoid method call loading issues
            if (_player == null)
            {
                _player = GameMainReferences.Instance.PlayerController.Entity;
            }
            return _player;
        }
    }

    public event Action<Entity> OnEntityKilled;
    public event Action<Entity> OnEntityEnabled;
    public event Action<Entity> OnEntityDisabled;
    public event Action<Entity> OnEntityAdded;
    public event Action<Entity> OnEntityRemoved;

    public List<Entity> LoadedEntities
    {
        get { return _loadedEntities; }
        private set { _loadedEntities = value; }
    }

    public List<Entity> FactionOne { get; set; }

    public override void OnAwake()
    {
        Instance = this;
        LoadedEntities = new List<Entity>();
        FactionOne = new List<Entity>();

        // Find all entities currently exsiting in the scene, i.e. they were preplaced and not loaded
        // From the Entity Pool directly. Attempt to load them as the Entity pool would have.
        var entities = GameObject.FindObjectsOfType<Entity>();
        foreach (var entity in entities)
        {
            if (!entity.gameObject.activeInHierarchy)
            {
                continue;
            }
            if (!entity.HasInitialised)
            {
                entity.Initialise();
                entity.PoolStart();
                if (entity.gameObject.activeSelf)
                {
                    AddEntity(entity);
                }
            }
        }
    }

    private void Update()
    {
        HandleEntityActiveState();
    }

    private void HandleEntityActiveState()
    {
        foreach (var loadedEntity in LoadedEntities)
        {
            if (IsEligibleToActivate(loadedEntity))
            {
                // Allow active state to be set via entity preference
                SetEntityActive(loadedEntity, loadedEntity.ShouldTryEnableEntity);
            }
            else
            {
                // Entity does is not eligible, force deactivation
                SetEntityActive(loadedEntity, false);
            }
        }
    }
    /// <summary>
    /// Returns true if the Entity is eligible to activate. Eligibility is determined via certain conditions. 
    /// The Enity may choose not to activated based on Eligbility by setting the ShouldTryEnableEntity property.
    /// Combining these two elements will activate or deactivate an Entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsEligibleToActivate(Entity entity)
    {
        return (entity.IgnoreDisableDistance || entity == Player) || Vector3.Distance(entity.transform.position, Player.transform.position) <= updateDistance;
    }

    private void SetEntityActive(Entity entity, bool value)
    {
        if (value)
        {
            if (!entity.gameObject.activeSelf)
            {
                entity.gameObject.SetActive(true);
                if (OnEntityEnabled != null)
                {
                    OnEntityEnabled(entity);
                }
            }
        }
        else
        {
            if (entity.gameObject.activeSelf)
            {
                entity.gameObject.SetActive(false);
                if (OnEntityDisabled != null)
                {
                    OnEntityDisabled(entity);
                }
            }
        }
    }

    public void TriggerEntityKilled(Entity entity)
    {
        if (OnEntityKilled != null)
            OnEntityKilled(entity);

        RemoveEntity(entity);
    }
    /// <summary>
    /// Removes the Entity from the Entity Manager.
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveEntity(Entity entity)
    {
        // Quick check to determain if our entity list contains this entity
        if (!entity.EntityMetaInfo.Has<EntityMetaInfo>(EntityMetaInfo.IsAddedToEntityManager))
        {
            Debug.LogError("Tried to remove " + entity + " that did not have IsAddedToEntityManager flags");
            return;
        }
        LoadedEntities.Remove(entity);
        if (entity.EntityFactionFlags == FactionFlags.One)
        {
            FactionOne.Remove(entity);
        }
        if (OnEntityRemoved != null)
        {
            OnEntityRemoved(entity);
        }
        entity.EntityMetaInfo = entity.EntityMetaInfo.Remove<EntityMetaInfo>(EntityMetaInfo.IsAddedToEntityManager);
    }

    public void AddEntity(Entity entity)
    {
        // Quick check to determain if our entity list contains this entity
        if (entity.EntityMetaInfo.Has<EntityMetaInfo>(EntityMetaInfo.IsAddedToEntityManager))
        {
            Debug.LogWarning("Tried to add " + entity + " that has IsAddedToEntityManager flags");
            return;
        }
        LoadedEntities.Add(entity);
        if (entity.EntityFactionFlags == FactionFlags.One)
        {
            FactionOne.Add(entity);
        }
        if (OnEntityAdded != null)
        {
            OnEntityAdded(entity);
        }
        entity.EntityMetaInfo = entity.EntityMetaInfo.Add<EntityMetaInfo>(EntityMetaInfo.IsAddedToEntityManager);
    }

    public Entity SpawnEntity(string entityID, string fxID)
    {
        Entity ent = EntityPool.Instance.GetObjectFromPool(entityID);
        return ent;
    }

}
