using UnityEngine;
using System.Collections;

[SpellCategory("Add Spell Marker", SpellEffectCategory.Entity)]
[SpellEffectStandard(true, false, "Adds a spell marker to the Entity when the event is triggered. This can let a missile hit a target and add a marker on doing so, preventing any further missile from targeting that target. To remove the marker use the Remove Spell Marker method but it is important to use the same ID in order to remove it.")]
public class AddSpellMarker : SpellEffectStandard
{
    public const string NOTSETID = "NOTSET";

    [SerializeField]
    private string _markerID = NOTSETID;
    [SerializeField]
    private float _removeTime = 1f;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        if(!_markerID.Equals(NOTSETID) && args.TargetEntity != null && !args.TargetEntity.HasSpellMarker(_markerID))
        {
            args.TargetEntity.AddSpellMarker(_markerID, _removeTime);
        }
    }

    public void Reset()
    {
        // Set default trigger event
        TriggerEvent = SpellEffectTriggerEvent.SpellApply;
    }

}
