using UnityEngine;
using System.Collections;
[QuestCategory("PlayMaker", QuestCategory.Misc)]
public class PlayMakerObjective : QuestObjective
{
    public void TriggerPlayerMakerObjectiveComplete()
    {
        TriggerObjectiveComplete();
    }

    private void Reset()
    {
        // Invisible objective by default
        IsInvisibleObjective = true;
    }
}
