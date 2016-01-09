using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class POIOverlay : QuestTaskOverlayItem
{
    [SerializeField]
    private bl_MiniMapItem _minimapItem;

    private UGUIMinimapUtilityWrapper _utilityWrapper;

    public Transform TargetTransform { get; set; }

    public override void Initialise()
    {
        base.Initialise();
        _utilityWrapper = new UGUIMinimapUtilityWrapper(_minimapItem);
    }

    public override void PoolStart()
    {
        base.PoolStart();
        _utilityWrapper.Target = TargetTransform;
        _utilityWrapper.IconCanvasGroup.alpha = 1f;
        // Renable the icon
        _utilityWrapper.IconItem.gameObject.SetActive(true);
    }


    public override void Recycle()
    {
        base.Recycle();
        // Disable the minimap icon
        _utilityWrapper.IconItem.gameObject.SetActive(false);
    }

}
