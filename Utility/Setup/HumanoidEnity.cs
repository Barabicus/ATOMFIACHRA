using UnityEngine;
using System.Collections;

public class HumanoidEnity : MonoBehaviour
{
    public void Reset()
    {
        var ent = CheckAndAdd<Entity>();
        CheckAndAdd<HumanoidAnimatorController>();
        CheckAndAdd<UnityNavMeshFinder>();
        if(ent != null)
        {
            ent.EntityID = gameObject.name;
        }
     //   DestroyImmediate(this);
    }

    protected T CheckAndAdd<T>() where T : Component
    {
        if(gameObject.GetComponent<T>() == null)
        {
            return gameObject.AddComponent<T>();
        }
        return null;
    }
}
