using UnityEngine;
using System.Collections;
[QuestCategory("Barrier State Value", QuestCategory.Misc)]
public class BarrierStateChangeQuestComponent : StandardQuestComponent
{
    [SerializeField]
    private bool _stateValue;
    private Barrier[] _barriers;

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        _barriers = GetComponentsInChildren<Barrier>(true);
    }

    protected override void DoEventTriggered(QuestComponentStandardEventAgrs args)
    {
        base.DoEventTriggered(args);
        foreach(var b in _barriers)
        {
            b.BarrierActivityState = _stateValue;
        }
    }

}
