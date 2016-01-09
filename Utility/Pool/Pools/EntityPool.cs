using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EntityPool : IDListPool<Entity>
{

    protected override System.Collections.Generic.Dictionary<string, Entity> ConstructedPrefabs
    {
        get
        {
            EntityPool.Instance.PoolObject(entityObj);
            var objs = Resources.LoadAll<Entity>("Prefabs/Entities");
            return objs.ToDictionary(e => e.EntityID);
        }
    }
}
