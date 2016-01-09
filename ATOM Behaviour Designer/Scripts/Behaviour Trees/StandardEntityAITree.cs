using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/StandardAI")]
[TaskIcon("BehaviorTreeReferenceIcon.png")]
public class StandardEntityAITree : EntityBehaviorTree
{
    [InheritedField]
    public SharedFloat detectDistance;
}

