using UnityEngine;
using System.Collections;
[QuestCategory("Help Box Component", QuestCategory.Misc)]
public class HelpBoxQuestComponent : StandardQuestComponent {
    [SerializeField]
    private StandardHelpBoxInstruction _helpBox;

    protected override void DoEventTriggered(QuestComponentStandardEventAgrs args)
    {
        base.DoEventTriggered(args);
        _helpBox.DisplayHelpBox();
    }

}
