using UnityEngine;
using System.Collections;

[QuestCategory("Unlock Spell", QuestCategory.Player)]
public class UnlockSpellQuestComponent : StandardQuestComponent {

    [SerializeField]
    private Spell _spell;
    [SerializeField]
    private bool _quietUnlock;

    protected override void DoEventTriggered(QuestComponentStandardEventAgrs args)
    {
        base.DoEventTriggered(args);
        var pc = GameMainReferences.Instance.PlayerCharacter;
        pc.UnlockSpell(_spell, _quietUnlock);
    }

    protected override void Reset()
    {
        base.Reset();
        TriggerEvent = QuestComponentTriggerEvent.Completed;
    }

}
