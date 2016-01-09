using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

/// <summary>
/// An Action class containing inherited references to essential entity components.
/// </summary>
public abstract class EntityAction : Action
{
    //[InheritedField]
    //public SharedEntity entity;
    //[InheritedField]
    //public SharedEntity targetEntity;
    //[InheritedField]
    //public SharedTransform targetTransform;

    public Entity Entity { get; private set; }

    public override void OnAwake()
    {
        base.OnAwake();
        Entity = gameObject.GetComponent<Entity>();
    }
}
