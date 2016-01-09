using UnityEngine;
using System.Collections;
[QuestCategory("Help Box", QuestCategory.Misc)]
public class HelpBoxObjective : QuestObjective
{
    [SerializeField]
    private StandardHelpBoxInstruction _helpBoxInfo;

    public override string QuestDescription
    {
        get
        {
            return string.Format(base.QuestDescription, _helpBoxInfo.OutputText);
        }
    }

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        if (_helpBoxInfo == null)
        {
            Debug.LogErrorFormat("Help box info was null for quest objective {0}", name);
        }
        _helpBoxInfo.OnHelpBoxCompleted += HelpBoxCompleted;
    }

    protected override void OnQuestCompleted()
    {
        base.OnQuestCompleted();
        // Ensure help box is closed
        if (!_helpBoxInfo.IsHelpBoxCompleted)
            _helpBoxInfo.IsHelpBoxCompleted = true;
    }

    private void HelpBoxCompleted()
    {
        _helpBoxInfo.OnHelpBoxCompleted -= HelpBoxCompleted;
        TriggerObjectiveComplete();
    }

    protected override void OnQuestAdded()
    {
        base.OnQuestAdded();
        _helpBoxInfo.DisplayHelpBox();
    }

    protected override void OnQuestUpdate()
    {
        base.OnQuestUpdate();
        TriggerDescriptionChanged();
    }

}
