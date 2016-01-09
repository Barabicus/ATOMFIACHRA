using UnityEngine;
using System.Collections;

public interface IPathFinder
{
    Vector3? Target { get; }
    float Speed { get; set; }
    bool TargetReached { get; }
    float StoppingDistance { get; }
    bool CanPathFind { get; set; }
    void SetDestination(Vector3 target);
    void SetPosition(Vector3 position);
    Vector3[] CalculatePath(Vector3 target);

}
