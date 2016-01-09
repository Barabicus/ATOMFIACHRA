using UnityEngine;
using System.Collections;

[QuestCategory("Player Level Reached", QuestCategory.Entity)]
public class LevelReachedObjective : QuestObjective
{
    [SerializeField] private int targetLevel;

    protected override void OnObjectiveUpdate()
    {
        base.OnObjectiveUpdate();
        if (GameMainReferences.Instance.PlayerCharacter.Entity.LevelHandler.CurrentLevel >= targetLevel)
        {
            TriggerObjectiveComplete();
        }
    }
}
