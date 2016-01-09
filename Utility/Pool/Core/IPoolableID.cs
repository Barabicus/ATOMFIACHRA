using UnityEngine;
using System.Collections;

public interface IPoolableID
{
    /// <summary>
    /// Called when the pooled object has been created initially
    /// </summary>
    void Initialise();
    /// <summary>
    /// Called when the object has been retrieved from the pool. Essentially the same as Start
    /// </summary>
    void PoolStart();
    /// <summary>
    /// Called when the object has been returned to the pool. This is not a means to pool the object. Call Pool Object on the associated Pooling class.
    /// </summary>
    void Recycle();
    /// <summary>
    /// This is the ID associated with the pooled object
    /// </summary>
    string PoolID { get; }
}
