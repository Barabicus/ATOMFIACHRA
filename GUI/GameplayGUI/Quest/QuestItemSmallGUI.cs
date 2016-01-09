using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestItemSmallGUI : MonoBehaviour {

    [SerializeField]
    private Text _descriptionText;
    [SerializeField]
    private Toggle _completionToggle;
    [SerializeField]
    private Color _completionColor;

    private QuestObjective _questObjective;

    public void SetQuestObjective(QuestObjective objective)
    {
        _questObjective = objective;
        _questObjective.OnObjectiveCompleted += ObjectiveComplete;
        SetDescription(objective);
        objective.OnDescriptionChanged += SetDescription;
    }

    private void SetDescription(QuestObjective objective)
    {
        _descriptionText.text = objective.QuestDescription;
    }

    private void ObjectiveComplete(QuestObjective obj)
    {
        _completionToggle.isOn = true;
        _descriptionText.color = _completionColor;
    }
}
