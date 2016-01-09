using UnityEngine;
using System.Collections;

public static class QuestOverlayFactory
{

    public static ArrowTaskOverlay CreateQuestArrow(Transform point, Color color, Quest quest)
    {
        var arrow = QuestTasksPool.Instance.GetObjectFromPool<ArrowTaskOverlay>();
        arrow.Quest = quest;
        arrow.TargetTransform = point;
        arrow.gameObject.SetActive(true);
        return arrow;
    }

    public static ArrowTaskOverlay CreateQuestArrow(Transform point, Quest quest)
    {
        return CreateQuestArrow(point, Color.white, quest);
    }

    public static POIOverlay CreatePointOfInterest(Transform point, Quest quest)
    {
        var poi = QuestTasksPool.Instance.GetObjectFromPool<POIOverlay>((o) =>
        {
            o.Quest = quest;
            o.TargetTransform = point;
        });
        poi.gameObject.SetActive(true);
        return poi;
    }

}
