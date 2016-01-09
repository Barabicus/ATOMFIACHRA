using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class EntityComponent : MonoBehaviour
{

    public Entity Entity
    {
        get;
        set;
    }

    public virtual void Initialise()
    {
        Entity = GetComponent<Entity>();
        Entity.OnKilled += OnKilled;
    }

    public virtual void OnStart()
    {
        
    }

    protected virtual void Update()
    {
        switch (Entity.LivingState)
        {
            case EntityLivingState.Alive:
                LivingUpdate();
                break;
            case EntityLivingState.Dead:
                DeadUpdate();
                break;
        }
    }

    /// <summary>
    /// Called while the entity is Living
    /// </summary>
    protected virtual void LivingUpdate() { }
    /// <summary>
    /// Called while the entity is Dead
    /// </summary>
    protected virtual void DeadUpdate() { }

    protected virtual void OnKilled(Entity e)
    {

    }

}
