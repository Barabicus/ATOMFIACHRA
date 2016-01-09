using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("Spell Game/Entity Behaviour")]
public class CastSpell : EntityAction
{
    public SharedSpell castSpell;
    public SharedTransform targetTransform;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("If true the target position will be set to the position associated with the target transform")]
    public SharedBool positionIsTarget;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Target position in local space")]
    public SharedVector3 targetPosition;
    public CastSpellMethod spellCastMethod = CastSpellMethod.Time;
    public SharedFloat castTime;
    public SharedInt castAmount = 1;

    private Timer _timer;
    private int _spellsCast;

    public enum CastSpellMethod
    {
        Time,
        Amount
    }

    public override void OnStart()
    {
        _timer = new Timer(castTime.Value);
        _spellsCast = 0;
        // Ensure the cast amount is atleast 1
        castAmount.Value = Mathf.Max(1, castAmount.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if (castSpell.Value == null)
        {
            return TaskStatus.Failure;
        }

        // Keep trying to cast a spell. If the spell cast method is Amount
        // increment the amount of spells cast, check it and return true if it matches
        // the desired cast amount.
        if(DoCastSpell(castSpell.Value) && spellCastMethod == CastSpellMethod.Amount)
        {
            _spellsCast++;
            if(_spellsCast == castAmount.Value)
            {
                return TaskStatus.Success;
            }
        }

        if(spellCastMethod == CastSpellMethod.Time)
        {
            if (_timer.CanTick)
            {
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Running;

    }

    private bool DoCastSpell(Spell castSpell)
    {
        Spell spell;
        Vector3 vec = positionIsTarget.Value ? targetTransform.Value.position : Owner.transform.TransformPoint(targetPosition.Value);
        return Entity.CastSpell(castSpell, out spell, targetTransform.Value, vec);
    }

}
