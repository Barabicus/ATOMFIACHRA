using UnityEngine;
using System.Collections;

public class UnlockSpellGeneralEvent : StandardGeneralEvent
{
    [SerializeField]
    private Spell _spell;
    [SerializeField]
    private bool _quietUnlock = false;

    protected override void DoEventTriggered(GeneralEventArgs args)
    {
        base.DoEventTriggered(args);
        GameMainReferences.Instance.PlayerCharacter.UnlockSpell(_spell, _quietUnlock);
    }

}
