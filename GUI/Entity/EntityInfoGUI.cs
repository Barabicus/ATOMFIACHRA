using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// The health bars that appear above entities heads. The health bars display text representing the health of the associated entity.
/// It may also display the quest this entity is associated with by displaying the number of the quest key. Updates to the quest
/// keys are performed in the QuestManager.
/// </summary>
public class EntityInfoGUI : MonoBehaviour
{

    [SerializeField]
    private Transform _container;
    [SerializeField]
    private RectTransform _elementsContainer;
    private Image _healthbarBackground;
    [SerializeField]
    private Image _healthImage;
    [SerializeField]
    private float _disableDistance = 50f;
    [SerializeField]
    private Transform _associateContainer;
    [SerializeField]
    private Text _associateText;
    [SerializeField]
    private float _healthAdjustSpeed = 10f;
    [SerializeField]
    private EntityCastingBar _entityCastingBar;

    private PlayerController _player;
    private Quest _associatedQuest;

    public Entity Entity { get; set; }

    public Quest AssociatedQuest
    {
        get { return _associatedQuest; }
        set
        {
            if (_associatedQuest != null)
            {
                _associatedQuest.OnQuestCompleted -= AssociatedQuestCompleted;
            }
            if (value != null)
            {
                value.OnQuestCompleted += AssociatedQuestCompleted;
                _associateText.text = value.QuestNumber.ToString();
            }
            _associatedQuest = value;
        }
    }

    private void Start()
    {
        if (_entityCastingBar != null)
        {
            _entityCastingBar.Entity = Entity;
        }
        _player = GameMainReferences.Instance.PlayerController;
        transform.localPosition = new Vector3(0, 2f, 0);
        if (Entity == null)
        {
            Debug.Log("Entity was null: " + gameObject);
            Destroy(this);
            return;
        }
        DoUpdate();
    }

    private void LateUpdate()
    {
        DoUpdate();
    }

    private void DoUpdate()
    {
        SetObjects();
        if (Entity.GUIHealthPoint != null)
            UpdateHealthBar();
    }

    private void SetObjects()
    {
        if (Entity.LivingState != EntityLivingState.Alive || Vector3.Distance(_player.transform.position, Entity.transform.position) >= _disableDistance || GameMainController.Instance.IsCinematic || !Entity.gameObject.activeInHierarchy)
            _container.gameObject.SetActive(false);
        else
        {
            _container.gameObject.SetActive(true);
        }
        // If the Entity is associated with a quest, display a quest icon.
        if(AssociatedQuest != null)
        {
            _associateText.text = AssociatedQuest.QuestNumber.ToString();
            _associateContainer.gameObject.SetActive(true);
        }
        else
        {
            _associateContainer.gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        _container.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Entity.GUIHealthPoint.position);
        _healthImage.fillAmount = GetPercent(Entity.CurrentHp, Entity.StatHandler.MaxHp);
    }

    private float GetPercent(float value, float max)
    {
        return (value / max);
    }

    private void AssociatedQuestCompleted(Quest obj)
    {
        obj.OnQuestCompleted -= AssociatedQuestCompleted;
        AssociatedQuest = null;
    }

}
