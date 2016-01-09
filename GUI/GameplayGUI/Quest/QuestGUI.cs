using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class QuestGUI : MonoBehaviour
{
    [SerializeField]
    private Transform _questItemContainer;
    [SerializeField]
    private QuestItemGUI questItemPrefab;
    /// <summary>
    /// The quest item that is currently selected. This is the GUI item
    /// </summary>
    private QuestItemGUI _selectedQuestItem;
    /// <summary>
    /// The selected quest. This is a reference to the actual quest
    /// </summary>
    private Quest _selectedQuest;
    private ToggleGroup _toggleGroup;
    /// <summary>
    ///  a list of all added quest items
    /// </summary>
    private List<QuestItemGUI> _questItems;
    private LineRenderer _lineRend;
    private NavMeshAgent _playerAgentCache;
    private NavMeshPath _path;

    public event Action<Quest> OnSelectedQuestChanged;

    public static QuestGUI Instance { get; set; }

    public QuestItemGUI SelectedQuestItem
    {
        get { return _selectedQuestItem; }
        set
        {
            // Minimize the quest objectives on the previously selected quest item
            if (_selectedQuestItem != null)
            {
                _selectedQuestItem.SetExpandedContainer(false);
            }
            _selectedQuestItem = value;

            if (value != null)
            {
                // Toggle task objectives on the current quest item.
                value.SetExpandedContainer(true);
                SelectedQuest = value.Quest;
            }
            else
            {
                SelectedQuest = null;
            }
        }
    }

    public Quest SelectedQuest
    {
        get { return _selectedQuest; }
        private set
        {
            _selectedQuest = value;
            if (OnSelectedQuestChanged != null)
            {
                OnSelectedQuestChanged(value);
            }
            if (value != null)
            {
                value.TriggerQuestSelected();
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Two QuestGUIS!");
            return;
        }
        QuestManager.Instance.OnQuestAdded += OnQuestAdded;
        _toggleGroup = GetComponent<ToggleGroup>();
        _questItems = new List<QuestItemGUI>();
        _lineRend = GetComponent<LineRenderer>();
        _path = new NavMeshPath();
    }

    private void Start()
    {
        _playerAgentCache = GameMainReferences.Instance.PlayerController.GetComponent<NavMeshAgent>();
    }

    private void OnQuestAdded(Quest quest)
    {
        var questItem = Instantiate(questItemPrefab);
        questItem.Quest = quest;
        questItem.transform.SetParent(_questItemContainer.transform);

        var toggle = questItem.GetComponent<Toggle>();
        toggle.group = gameObject.GetComponent<ToggleGroup>();
        // Listen for value changing. 
        // If the quest is selected set it as the selected quest
        toggle.onValueChanged.AddListener((v) =>
       {
           if (v)
           {
               SelectedQuestItem = questItem;
           }
       });

        _questItems.Add(questItem);
        // Remove the quest item when the quest is completed
        questItem.Quest.OnQuestCompleted += (q) =>
        {
            if (SelectedQuest == q)
            {
                SelectedQuestItem = null;
            }
            _questItems.Remove(questItem);
        };
        TryAutoSelect();
    }

    private void Update()
    {
        //if (SelectedQuest != null)
        //{
        //    DisplayPath();
        //}
        if (LevelMetaInfo.Instance.IsDebugging)
        {
            DebugQuestCompletion();
        }
    }

    private void TryAutoSelect()
    {
        if (SelectedQuest != null && !SelectedQuest.IsQuestComplete)
        {
            return;
        }
        if (_questItems.Count > 0)
        {
            var toggle = _questItems[0].GetComponent<Toggle>();
            //  toggle.Select();
            //  SelectedQuestItem = _questItems[0];
            toggle.isOn = true;
        }
        else
        {
            SelectedQuest = null;
        }
    }

    private void DisplayPath()
    {
        if (SelectedQuest.QuestPathLocation == null)
        {
            _lineRend.SetVertexCount(0);
            return;
        }

        _playerAgentCache.CalculatePath(SelectedQuest.QuestPathLocation.transform.position, _path);

        _lineRend.SetVertexCount(_path.corners.Length);

        for (int i = 0; i < _path.corners.Length; i++)
        {
            _lineRend.SetPosition(i, _path.corners[i]);
        }
    }

    private void DebugQuestCompletion()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1) && _questItems.Count > 0)
        {
            var q = _questItems[0];
            var dp = q.Quest.GetComponentsInChildren<PlayerDebugPositionQuestComponent>(true);
            Vector3 debugPos;
            if (dp != null && dp.Length > 0)
            {
                debugPos = dp[0].transform.position;
                GameMainReferences.Instance.PlayerCharacter.Entity.EntityPathFinder.SetPosition(debugPos);
                GameMainReferences.Instance.PlayerCharacter.Entity.EntityPathFinder.SetDestination(debugPos);
            }
            foreach(var obj in q.Quest.QuestObjectives)
            {
                obj.TriggerObjectiveComplete();
            }
        }
    }

}
