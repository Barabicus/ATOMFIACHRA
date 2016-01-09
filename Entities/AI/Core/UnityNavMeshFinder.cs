using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
public class UnityNavMeshFinder : EntityComponent, IPathFinder
{
    private NavMeshAgent _agent;
    private Vector3? _target;
    private bool _isDirty = false;
    private Timer _updateTimer;
    private bool _canPathFind = true;
    private NavMeshPath _pathCache;

    public override void Initialise()
    {
        base.Initialise();
        _agent = GetComponent<NavMeshAgent>();
        _pathCache = new NavMeshPath();
    }

    public float Speed
    {
        get { return _agent.speed; }
        set { _agent.speed = value; }
    }

    public float StoppingDistance
    {
        get
        {
            return _agent.stoppingDistance;
        }
    }

    public bool TargetReached
    {
        get
        {

            //if (!Target.HasValue)
            //{
            //    return !_agent.hasPath;
            //}
            //return Vector3.Distance(Target.Value, transform.position) <= _agent.stoppingDistance + 1f;
            //return false;

            if (!_agent.pathPending)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        // Done
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public void SetDestination(Vector3 target)
    {
        try
        {
            Target = target;
            _isDirty = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        Repath();
    }

    protected override void OnKilled(Entity e)
    {
        base.OnKilled(e);
        _agent.Stop();
    }

    private void Repath()
    {
        if (Target.HasValue && Target.Value != _agent.destination && _isDirty)
        {
            _agent.SetDestination(Target.Value);
            if (TargetReached)
            {
                Target = null;
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (_agent.enabled)
            _agent.Warp(position);
        else
        {
            Entity.transform.position = position;
        }
        Target = position;
    }

    private void Reset()
    {
        var col = GetComponent<CapsuleCollider>();
        col.center = new Vector3(0, 1, 0);
        col.height = 2f;

        var navMesh = GetComponent<NavMeshAgent>();
        navMesh.acceleration = 1000f;
        navMesh.angularSpeed = 1000f;
        navMesh.stoppingDistance = 1f;
    }

    public Vector3[] CalculatePath(Vector3 target)
    {
        _agent.CalculatePath(target, _pathCache);
        return _pathCache.corners;
    }

    public Vector3? Target
    {
        get { return _target; }
        private set
        {
            _target = value;
        }
    }

    public bool CanPathFind
    {
        get
        {
            return _canPathFind;
        }
        set
        {
            _canPathFind = value;
            _agent.SetDestination(transform.position);
            if (value)
            {
                _agent.Resume();
            }
            else
            {
                _agent.Stop();
            }
        }
    }
}
