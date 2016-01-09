using UnityEngine;
using System.Collections;

[QuestCategory("Location Of Interest", QuestCategory.UI)]
public class LocationOfInterestQuestComponent : QuestComponent
{
    [SerializeField]
    private bool _registerPOI = true;
    [SerializeField]
    private bool _registerQuestArrow = true;

    private POIOverlay _poiOverlay;
    private ArrowTaskOverlay _arrow;

    protected override void OnQuestSelected()
    {
        base.OnQuestSelected();
        if (_registerQuestArrow)
        {
            _arrow = QuestOverlayFactory.CreateQuestArrow(transform, Color.red, Quest);
        }
        if (_registerPOI)
        {
            _poiOverlay = QuestOverlayFactory.CreatePointOfInterest(transform, Quest);
        }
    }

    protected override void OnQuestObjectiveCompleted(QuestObjective objective)
    {
        base.OnQuestObjectiveCompleted(objective);
        DeRegister();
    }

    protected override void OnQuestCompleted()
    {
        base.OnQuestCompleted();
        DeRegister();
    }

    private void DeRegister()
    {
        if (_arrow != null) _arrow.DeRegisterQuestTaskOverlay();
        if (_poiOverlay != null) _poiOverlay.DeRegisterQuestTaskOverlay();
    }

}
