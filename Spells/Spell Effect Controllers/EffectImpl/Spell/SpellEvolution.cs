using UnityEngine;
using System.Collections;

[SpellCategory("Spell Evolution", SpellEffectCategory.Spell)]
public class SpellEvolution : SpellEffectStandard
{
    [SerializeField]
    private int _evolveCost;
    [SerializeField]
    private Spell _evolveSpell;

    private int _currentEvolvePoints;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _currentEvolvePoints = 0;
    }

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        AddEvolutionPoints(1);
    }

    public void AddEvolutionPoints(int amount)
    {
        _currentEvolvePoints += amount;
        if(_currentEvolvePoints >= _evolveCost)
        {
            TriggerEvolve();
        }
    }

    private void TriggerEvolve()
    {

    }

    private void Reset()
    {
        TriggerEvent = SpellEffectTriggerEvent.SpellRefresh;
    }

}
