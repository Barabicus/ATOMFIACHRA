using UnityEngine;
using System.Collections;
[QuestCategory("Tutorial Mouse Button", QuestCategory.Misc)]
public class TutorialMouseObjective : QuestObjective
{
    [SerializeField]
    private int _button;

    protected override void OnQuestUpdate()
    {
        base.OnQuestUpdate();
        if (Input.GetMouseButton(_button))
        {
            TriggerObjectiveComplete();
        }
    }

}
