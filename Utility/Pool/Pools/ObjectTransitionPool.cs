using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectTransitionPool : IDListPool<ObjectTransitionFx>
{
    protected override System.Collections.Generic.Dictionary<string, ObjectTransitionFx> ConstructedPrefabs
    {
        get
        {
            var objs = Resources.LoadAll<ObjectTransitionFx>("Prefabs/Fx/TransitionFx");
            return objs.ToDictionary(obj => obj.PoolID);
        }
    }
}
