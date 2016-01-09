using UnityEngine;
using System.Collections;

public class StartQuestLine : MonoBehaviour
{
    [SerializeField]
    private QuestLineManager _questLineManager;
    [SerializeField]
    private StartQuestlineMethod _startQuestLineMethod;
    [SerializeField]
    private float _areaRadius = 5f;
    [SerializeField]
    private Quest _questCompleted;

    private Entity _player;

    public enum StartQuestlineMethod
    {
        None,
        Area,
        QuestCompleted
    }

    /// <summary>
    /// If this is called it will start the quest line manager associated with this.
    /// This can be called by an external means. For example a button.
    /// </summary>
    public void TriggerStartQuestLine()
    {
        _questLineManager.StartQuestLine();
    }

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerCharacter.Entity;

        switch (_startQuestLineMethod)
        {
            case StartQuestlineMethod.Area:
                StartCoroutine(CheckPlayerDistance());
                break;
            case StartQuestlineMethod.QuestCompleted:
                if (_questCompleted != null)
                {
                    _questCompleted.OnQuestCompleted += OnQuestCompleted;
                }
                break;
        }
    }
    /// <summary>
    /// Only called when the selected quest has been completed and the start method is QuestCompleted
    /// </summary>
    /// <param name="obj"></param>
    private void OnQuestCompleted(Quest obj)
    {
        TriggerStartQuestLine();
        obj.OnQuestCompleted -= OnQuestCompleted;
    }

    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if (Vector3.Distance(_player.transform.position, transform.position) <= _areaRadius)
            {
                TriggerStartQuestLine();
                yield break;
            }
            // Wait a bit before rechecking
            yield return null;
        }
    }

    public void OnDrawGizmosSelected()
    {
        if(_startQuestLineMethod == StartQuestlineMethod.Area)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _areaRadius);
        }
    }
}
