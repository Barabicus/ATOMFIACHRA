using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Spell Game/Basic")]
public class GetSpellFromSpellBook : EntityAction
{
    public SharedSpell spellOutput;
    public SharedTransform target;
    public SharedFloat castTimeOutput;

    private EntitySpellBook _spellBook;

    public override void OnAwake()
    {
        base.OnAwake();
        _spellBook = Entity.GetComponent<EntitySpellBook>();
    }

    public override void OnStart()
    {
        base.OnStart();
        if (_spellBook != null)
        {
            var distance = target.Value == null ? 0f : Vector3.Distance(Entity.transform.position, target.Value.position);
            float ct;
            spellOutput.Value = _spellBook.GetSpell(distance, Entity.CurrentHealthNormalised, out ct);
            castTimeOutput.Value = ct;
        }
    }
}
