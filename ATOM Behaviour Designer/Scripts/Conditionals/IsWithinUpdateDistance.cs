using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game")]
public class IsWithinUpdateDistance : Conditional
{

    private Entity _player;

    private const float _updateDistance = 80f;

    public override void OnAwake()
    {
        _player = GameMainReferences.Instance.PlayerController.Entity;
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.Distance(_player.transform.position, gameObject.transform.position) <= _updateDistance)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
