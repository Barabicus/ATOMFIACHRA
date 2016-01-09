using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestItemGUI : MonoBehaviour
{
    [SerializeField]
    private Text _questText;
    [SerializeField]
    private Text _questNumberText;
    [SerializeField]
    private Transform _container;
    [SerializeField]
    private RectTransform _expandedContainer;
    [SerializeField]
    private RectTransform _descriptionContainer;
    [SerializeField]
    private Text _descriptionText;
    [SerializeField]
    private TaskItemGUI _taskPrefab;
    [SerializeField]
    private Color _questSelectedColor;

    private Animator _anim;
    private Color _startColor;

    public Quest Quest { get; set; }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _startColor = _questText.color;
    }

    private void Start()
    {
        _questText.text = Quest.QuestName;
        Quest.OnQuestCompleted += OnQuestCompleted;
        Quest.OnQuestNumberChanged += AssignQuestNumber;
        GenerateTaskObjectives();
        AssignQuestNumber(Quest.QuestNumber);
        // Listen for selection events
        var toggle = gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((v) =>
        {
            if (v)
            {
                _questText.color = _questSelectedColor;
            }
            else
            {
                _questText.color = _startColor;
            }
        }
        );
        if(Quest.QuestDescription.Length == 0)
        {
            _descriptionContainer.gameObject.SetActive(false);
        }
        else
        {
            _descriptionText.text = Quest.QuestDescription;
            _descriptionContainer.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Assigns the quest number in the Quest Journal. This method is called everytime
    /// a quest has completed and will reassign the quest number to fix the current amount
    /// of quests displayed.
    /// </summary>
    /// <param name="number"></param>
    private void AssignQuestNumber(int number)
    {
        _questNumberText.text = number.ToString();
    }
    /// <summary>
    /// Generates all the task objectives associated for this quest item.
    /// </summary>
    private void GenerateTaskObjectives()
    {
        foreach (var objective in Quest.QuestObjectives)
        {
            var obj = Instantiate(_taskPrefab);
            obj.transform.SetParent(_container);
            obj.QuestObjective = objective;
        }
    }

    private void OnQuestCompleted(Quest quest)
    {
        SetExpandedContainer(false);
        _anim.SetTrigger("questCompleted");
    }
    /// <summary>
    /// Called by the animation to delete this quest item
    /// </summary>
    public void RemoveQuestItem()
    {
        Destroy(gameObject);
    }

    public void ToggleTasks()
    {
        //_expandedContainer.gameObject.SetActive(!_expandedContainer.gameObject.activeSelf);
        SetExpandedContainer(!_expandedContainer.gameObject.activeSelf);
    }
    public void SetExpandedContainer(bool value)
    {
        if (Quest.IsQuestComplete)
        {
            // Ensure the expanded container cannot expand when the quest is completed.
            _expandedContainer.gameObject.SetActive(false);
        }
        _expandedContainer.gameObject.SetActive(value);
    }
}
