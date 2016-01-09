using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TaskItemGUI : MonoBehaviour
{
    [SerializeField]
    private Text _questText;

    public QuestObjective QuestObjective { get; set; }

    private void Start()
    {
        _questText.text = QuestObjective.QuestDescription;
    }

    private void Update()
    {
        _questText.text = QuestObjective.QuestDescription;
        if (QuestObjective.ObjectiveComplete)
        {
            Debug.Log("objective completed");
            Destroy(gameObject);
        }
    }
}
