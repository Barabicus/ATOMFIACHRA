using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The wave property is used a marker to indictate what game objects should be used as wave containers.
/// </summary>
public class WaveProperty : MonoBehaviour
{
    private List<WaveEntityDetails> _entityDetails;

    /// <summary>
    /// Called by the wave objective for the WaveProperty to handle what entities it 
    /// has. This will take all the entities within it, substitute them with an WaveEntityWrapper
    /// and repool them. When the wave is called they will be pulled from the pool.
    /// </summary>
    public void ConstructEntityDetails()
    {
        _entityDetails = new List<WaveEntityDetails>();
        var entities = GetComponentsInChildren<Entity>(true);
        foreach(var e in entities)
        {
            GameObject subTransform = new GameObject("EntityPoint: " + e.EntityID);
            subTransform.transform.position = e.transform.position;
            subTransform.transform.rotation = e.transform.rotation;
            _entityDetails.Add(new WaveEntityDetails(subTransform.transform, e.EntityID));
            subTransform.transform.SetParent(transform);
            e.PoolEntity();
        }
    }

    public Entity[] GenerateEntitiesFromWave()
    {
        var ents = new Entity[_entityDetails.Count];
        int index = 0;
        foreach(var ed in _entityDetails)
        {
            var entity = EntityPool.Instance.GetObjectFromPool(ed.TargetEntityID, (e) =>
            {
                // Position the Entity
                e.transform.position = ed.TargetTransform.position;
                e.transform.rotation = ed.TargetTransform.rotation;
                // Ensure the Entity is not enabled
                e.ShouldTryEnableEntity = false;
            });
            entity.transform.SetParent(transform);
            ents[index] = entity;
            index++; 
        }
        return ents;
    }
}

public struct WaveEntityDetails
{
    public Transform TargetTransform { get; set; }
    public string TargetEntityID { get; set; }

    public WaveEntityDetails(Transform transform, string entityID)
    {
        TargetTransform = transform;
        TargetEntityID = entityID;
    }
}