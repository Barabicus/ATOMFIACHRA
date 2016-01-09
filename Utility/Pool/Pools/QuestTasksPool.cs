using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Quest task pool. Note this pool unlike the spell pool does not have the pool IDs serialised into the prefab.
/// This is because quest task overlays are more specific and do not need a large number of unique IDs. When 
/// pulled from the pool a reference to a const string ID will be called here for use with identifying what object it is
/// that should be pulled. 
/// </summary>
public class QuestTasksPool : TypeListPool<QuestTaskOverlayItem>
{
    private Dictionary<Type, QuestTaskOverlayItem> _overlays;

    protected override System.Collections.Generic.Dictionary<Type, QuestTaskOverlayItem> ConstructedPrefabs
    {
        get
        {
            Dictionary<Type,QuestTaskOverlayItem> overlays = new Dictionary<Type, QuestTaskOverlayItem>();
            foreach (var overlay in Resources.LoadAll<QuestTaskOverlayItem>("Prefabs/GUI/QuestTaskOverlays"))
            {
                overlays.Add(overlay.GetType(), overlay);
            }
            return overlays;
        }
    }
}
