using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Checks if all the objects are cleared from the target area. 
/// </summary>
[QuestCategory("Clear Objects", QuestCategory.Location)]
public class ObjectsClearedObjective : QuestObjective
{
    [SerializeField]
    [Tooltip("The target container that is storing all the objects we will be comparing against")]
    private Transform _objectsContainer;
    [SerializeField]
    private float _radius = 5f;
    [SerializeField]
    private bool _disablesContainerOnCompletion = true;

    private List<Transform> _containedObjects;

    public override string QuestDescription
    {
        get
        {
            return string.Format(base.QuestDescription, _containedObjects.Count);
        }
    }

    protected override void OnQuestInitialise()
    {
        base.OnQuestInitialise();
        if (_objectsContainer == null)
        {
            Debug.LogErrorFormat("Object container for quest {0} was null. Please ensure this objective has a reference to an object container", Quest.QuestName);
            return;
        }
        _containedObjects = _objectsContainer.gameObject.GetComponentsInChildren<Transform>().ToList();
        // Ensure the container is removed
        _containedObjects.Remove(_objectsContainer);
    }

    protected override void OnQuestUpdate()
    {
        base.OnQuestUpdate();
        for(int i = _containedObjects.Count - 1; i >= 0; i--)
        {
            if(Vector3.Distance(_containedObjects[i].transform.position, transform.position) >= _radius)
            {
                _containedObjects.RemoveAt(i);
            }
        }
        CompletionCheck();
    }

    private void CompletionCheck()
    {
        if(_containedObjects.Count == 0)
        {
            TriggerObjectiveComplete();
        }
    }

    protected override void ObjectiveCompleted()
    {
        base.ObjectiveCompleted();
        if (_disablesContainerOnCompletion)
        {
            _objectsContainer.gameObject.SetActive(false);
        }
    }

    protected override void Reset()
    {
        base.Reset();
        GameObject cont = new GameObject("Container");
        cont.transform.SetParent(transform);
        _objectsContainer = cont.transform;
        QuestDescription = "Remaining Objects {0}";
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
