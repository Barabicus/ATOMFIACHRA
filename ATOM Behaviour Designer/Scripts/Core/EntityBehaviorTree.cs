using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("Spell Game")]
[TaskIcon("BehaviorTreeReferenceIcon.png")]
public class EntityBehaviorTree : BehaviorTreeReference
{
    [InheritedField]
    public SharedEntity entity;
    [InheritedField]
    public SharedEntity targetEntity;
    [InheritedField]
    public SharedTransform targetTransform;

}
