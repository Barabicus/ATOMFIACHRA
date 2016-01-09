using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public abstract class EntityConditional : Conditional {

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
