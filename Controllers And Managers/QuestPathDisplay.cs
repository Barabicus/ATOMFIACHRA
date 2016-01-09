using UnityEngine;
using System.Collections;

public class QuestPathDisplay : GameController
{
    public Transform target;

    private NavMeshAgent _agent;
    private NavMeshPath path;


    private void Start()
    {
        _agent = GameMainReferences.Instance.PlayerController.Entity.GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    private void Update()
    {
        _agent.CalculatePath(target.position, path);

        for(int i = 0; i < path.corners.Length; i++)
        {
            if(i + 1 != path.corners.Length)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }
            else
            {
                Debug.DrawRay(path.corners[i], Vector3.up * 5f, Color.red);
            }
        }
    }

}
