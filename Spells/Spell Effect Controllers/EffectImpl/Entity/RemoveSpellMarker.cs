using UnityEngine;
using System.Collections;

[SpellCategory("Remove Spell Marker", SpellEffectCategory.Entity)]
[SpellEffectStandard(true, false, "Removes a spell marker to the Entity when the event is triggered. Ensure a marker is added first for this to actually be effective.")]
public class RemoveSpellMarker : SpellEffectStandard
{
    [SerializeField]
    private string _markerID = AddSpellMarker.NOTSETID;

    protected override void DoEventTriggered(SpellEffectStandardEventArgs args)
    {
        base.DoEventTriggered(args);
        if(!_markerID.Equals(AddSpellMarker.NOTSETID) && args.TargetEntity != null && args.TargetEntity.HasSpellMarker(_markerID))
        {
            args.TargetEntity.RemoveSpellMarker(_markerID);
        }
    }

    public void Reset()
    {
        // Set default trigger event
        TriggerEvent = SpellEffectTriggerEvent.SpellDestroy;
    }

}
