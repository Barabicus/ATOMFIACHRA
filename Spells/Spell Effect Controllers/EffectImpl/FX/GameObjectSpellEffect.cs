using UnityEngine;
using System.Collections;
[SpellEffectStandard(false, false,"This Spell Effect disables or activates the gameobject on an event triggered.")]
[SpellCategory("GameObject", SpellEffectCategory.FX)]
public class GameObjectSpellEffect : SpellEffectStandard {
    [SerializeField]
    private bool _setActiveValue;

    private bool r_active;

    public override void Initialize(EffectSetting effectSetting)
    {
        base.Initialize(effectSetting);
        r_active = gameObject.activeSelf;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        gameObject.SetActive(r_active);
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        gameObject.SetActive(_setActiveValue);
    }

}
