using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/StandardAI")]
public class SetStandardAIVariables : Action
{
    [InheritedField]
    private SharedFloat detectDistance;

    public SharedFloat localDistance;

    public override void OnStart()
    {
        localDistance.Value = detectDistance.Value;
    }
}
