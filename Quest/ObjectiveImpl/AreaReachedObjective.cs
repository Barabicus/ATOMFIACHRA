using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[QuestCategory("Area Reached", QuestCategory.Location)]
public class AreaReachedObjective : QuestObjective
{
    [FormerlySerializedAs("triggerDistance")]
    [SerializeField]
    private float _radius = 5f;
    [SerializeField]
    private Vector3 _boxBounds = new Vector3(10, 10, 10);
    [SerializeField]
    private LocationChecking _locationChecking = LocationChecking.Radius;
    [SerializeField]
    [FormerlySerializedAs("_areaReachedMethod")]
    private BoundsChecking _boundsChecking = BoundsChecking.LessThanDistance;

    private Bounds _boundingBox;

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        if (_locationChecking == LocationChecking.Box)
        {
            _boundingBox = new Bounds(transform.position, _boxBounds);
        }
    }

    /// <summary>
    /// Returns true if the player is within the bounds of this area objective. 
    /// False if the player is outside the bounds.
    /// </summary>
    public bool IsPlayerContained
    {
        get
        {
            switch (_locationChecking)
            {
                case LocationChecking.Box:
                    return _boundingBox.Contains(Player.transform.position);
                case LocationChecking.Radius:
                    return Vector3.Distance(transform.position, Player.transform.position) <= _radius;
            }
            return false;
        }
    }

    public enum LocationChecking
    {
        Radius,
        Box
    }

    public enum BoundsChecking
    {
        LessThanDistance,
        GreaterThanDistance
    }

    private ArrowTaskOverlay areaArrow;

    protected override void OnObjectiveUpdate()
    {
        base.OnObjectiveUpdate();
        switch (_boundsChecking)
        {
            case BoundsChecking.LessThanDistance:
                if (IsPlayerContained)
                    TriggerObjectiveComplete();
                break;
            case BoundsChecking.GreaterThanDistance:
                if (!IsPlayerContained)
                    TriggerObjectiveComplete();
                break;
        }
    }

    protected override void ObjectiveCompleted()
    {
        base.ObjectiveCompleted();
        if (areaArrow != null)
        {
            areaArrow.DeRegisterQuestTaskOverlay();
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        switch (_locationChecking)
        {
            case LocationChecking.Radius:
                Gizmos.DrawWireSphere(transform.position, _radius);
                break;
            case LocationChecking.Box:
                Gizmos.DrawWireCube(transform.position, _boxBounds);
                break;
        }
    }

    protected override void Reset()
    {
        base.Reset();
        QuestDescription = "Area Reached";
    }


}
