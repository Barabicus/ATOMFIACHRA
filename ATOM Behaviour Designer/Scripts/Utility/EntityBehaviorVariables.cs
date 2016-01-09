using UnityEngine;
using System.Collections;

public class EntityBehaviorVariables : EntityComponent
{
    public Entity TargetEntity { get; set; }

    public Transform TargetTransform
    {
        get { return TargetEntity != null ? TargetEntity.transform : null; }
    }
}
