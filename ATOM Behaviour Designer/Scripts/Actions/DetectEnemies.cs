using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Detect nearby enemies.")]
[TaskCategory("Spell Game")]
public class DetectEnemies : EntityConditional
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance we will test against")]
    public SharedFloat distance = 10f;
    public SharedFloat loseTargetDistance = 10f;

    public SharedTransform target;

    private Entity _targetEntity;
    private Timer _checkTimer;

    public override void OnAwake()
    {
        base.OnAwake();
        if(loseTargetDistance.Value < distance.Value)
        {
            Debug.LogErrorFormat("Behavior tree for Entity {0} is trying to check for targets with a lower lose distance than detect distance!", gameObject.name);
        }
        _checkTimer = Random.Range(0.5f, 1f);
    }

    public override TaskStatus OnUpdate()
    {
        if (_checkTimer.CanTickAndReset())
        {
            TryFindNewTarget();
        }
        CheckCurrentTarget();
        return target.Value == null ? TaskStatus.Failure : TaskStatus.Success;
    }

    private void TryFindNewTarget()
    {
        float nearest = distance.Value + 10f;
        Entity nearestEnt = null;
        // Setup for speed for checking against the player.
        foreach (var e in EntityManager.Instance.FactionOne)
        {
            if (e == Entity || !Entity.IsEnemy(e) || e.LivingState != EntityLivingState.Alive)
                continue;

            float dis = Vector3.Distance(e.transform.position, Entity.transform.position);
            if (dis <= distance.Value && dis <= nearest)
            {
                nearest = dis;
                nearestEnt = e;
            }
        }
        if (nearestEnt != null)
        {
            _targetEntity = nearestEnt;
            target.Value = nearestEnt.transform;
        }

        // Randomise check time again.
        // Don't let randomnly chosen entities get a unfair check advantage.
        _checkTimer.TickTime = Random.Range(0.5f, 1f);
    }

    private void CheckCurrentTarget()
    {
        // Lose the target if the Entity is not alive or out of distance
        if (_targetEntity != null && (_targetEntity.LivingState != EntityLivingState.Alive || Vector3.Distance(Entity.transform.position, target.Value.position) >= loseTargetDistance.Value))
        {
            target.Value = null;
            _targetEntity = null;
        }
    }
}