using UnityEngine;
using System.Collections;
[QuestCategory("Set Spawn Point", QuestCategory.Player)]
public class UnlockRespawnPointQuestComponent : StandardQuestComponent
{
    [SerializeField]
    private PlayerSpawnPoint _targetSpawnPoint;

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        if(_targetSpawnPoint == null)
        {
            Debug.LogErrorFormat("Player spawn point was null for quest {0}", Quest.QuestName);
            Destroy(this);
            return;
        }
    }

    protected override void DoEventTriggered(QuestComponentStandardEventAgrs args)
    {
        base.DoEventTriggered(args);
        _targetSpawnPoint.SetAsActiveSpawnPoint();
    }
}
