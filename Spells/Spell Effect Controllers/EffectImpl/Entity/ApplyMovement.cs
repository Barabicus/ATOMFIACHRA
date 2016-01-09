using UnityEngine;
using System.Collections;

namespace SpellGame.SpellEffects
{
    [SpellCategory("Apply Movement", SpellEffectCategory.Entity)]
    public class ApplyMovement : SpellEffectStandard
    {

        [Tooltip("The direction to move in. This is a local direction")]
        [SerializeField]
        private Vector3 _moveDirection = Vector3.forward;

        [SerializeField] private float _speed = 1f;

        private bool _triggered;

        protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
        {
            base.DoEventTriggered(args);
            _triggered = true;
        }

        protected override void UpdateSpell()
        {
            base.UpdateSpell();
            if (_triggered)
            {
                effectSetting.spell.CastingEntity.transform.position += transform.TransformDirection(_moveDirection) *_speed*Time.deltaTime;
            }

        }

        protected override void effectSetting_OnSpellDestroy()
        {
            base.effectSetting_OnSpellDestroy();
        }
    }
}
