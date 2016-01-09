using UnityEngine;
using System.Collections;
[RequireComponent(typeof(bl_MiniMapItem))]
public class UGUIEntityMinimapIcon : MonoBehaviour
{
    [SerializeField]
    private bl_MiniMapItem _minimapItem;
    [SerializeField]
    private Sprite _livingIcon;
    [SerializeField]
    private Sprite _deathIcon;
    [SerializeField]
    private Color _friendlyColor;
    [SerializeField]
    private Color _enemyColor;
    [SerializeField]
    private Color _ignoreColor;
    [SerializeField]
    private Color _questEntityColor;

    private Entity _player;
    private Entity _targetEntity;
    /// <summary>
    /// The graphical icon associated with the minimap.
    /// </summary>
    private UGUIMinimapUtilityWrapper _utilityWrapper;

    /// <summary>
    /// The Entity this minimap icon will be associated with.
    /// </summary>
    public Entity Entity
    {
        get { return _targetEntity; }
        set
        {
            _targetEntity = value;
            _minimapItem.Target = value.transform;
        }
    }

    private void Start()
    {
        // Ensure the icon has been created
        _utilityWrapper = new UGUIMinimapUtilityWrapper(_minimapItem);
        _player = GameMainReferences.Instance.PlayerController.Entity;
        UpdateIcon();
        _targetEntity.OnEntityReset += OnEntityReset;
        _utilityWrapper.IconCanvasGroup.alpha = 1f;
    }
    /// <summary>
    /// Listen for target Entity reset events so we can ensure the alpha of the canvas group properly starts as 1 i.e. visible.
    /// </summary>
    /// <param name="obj"></param>
    private void OnEntityReset(Entity obj)
    {
        _utilityWrapper.IconCanvasGroup.alpha = 1f;
    }

    private void LateUpdate()
    {
        SetIconEnabled();
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        switch (Entity.LivingState)
        {
            case EntityLivingState.Alive:
                _utilityWrapper.IconSprite = _livingIcon;
                if (QuestManager.Instance.TrackedQuest != null && QuestManager.Instance.TrackedQuest.IsEntityOfInterest(Entity))
                {
                    _minimapItem.GraphicImage.color = _questEntityColor;
                    return;
                }
                if ((Entity.EntityFactionFlags & FactionFlags.Ignore) == FactionFlags.Ignore)
                {
                    _minimapItem.GraphicImage.color = _ignoreColor;
                    return;
                }

                if (Entity != _player)
                {
                    _minimapItem.GraphicImage.color = _player.IsEnemy(Entity) ? _enemyColor : _friendlyColor;
                }
                break;
            case EntityLivingState.Dead:
                // If dead ensure the icon is disabled
                _utilityWrapper.IconSprite = _deathIcon;
                break;
        }
    }

    private void SetIconEnabled()
    {
        _utilityWrapper.IconItem.gameObject.SetActive(Entity.gameObject.activeInHierarchy);
    }

}
