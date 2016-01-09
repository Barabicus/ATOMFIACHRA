using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EntityMinimapIcon : MonoBehaviour
{

    [SerializeField]
    private Color _playerColor;
    [SerializeField]
    private Color _friendlyColor;
    [SerializeField]
    private Color _enemyColor;
    [SerializeField]
    private Color _deadColor;
    [SerializeField]
    private Color _ignoreColor;
    [SerializeField]
    private Color _questEntityColor;

    [SerializeField]
    private Transform _container;
    [SerializeField]
    private Image _image;

    private Camera _camera;
    private Entity _player;

    public Entity Entity { get; set; }

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerController.Entity;
        _camera = MinimapCamera.Instance.GetComponent<Camera>();
        transform.SetParent(Entity.transform);
        transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        SetMiniMapColour();
        transform.rotation = _camera.transform.rotation;
    }

    private void SetMiniMapColour()
    {
        switch (Entity.LivingState)
        {
            case EntityLivingState.Alive:
                if (QuestGUI.Instance.SelectedQuest != null && QuestGUI.Instance.SelectedQuest.IsEntityOfInterest(Entity))
                {
                    _image.color = _questEntityColor;
                    return;
                }
                if ((Entity.EntityFactionFlags & FactionFlags.Ignore) == FactionFlags.Ignore)
                {
                    _image.color = _ignoreColor;
                    return;
                }

                if (Entity != _player)
                {
                    _image.color = _player.IsEnemy(Entity) ? _enemyColor : _friendlyColor;
                }
                else
                {
                    _image.color = _playerColor;
                }
                break;
            case EntityLivingState.Dead:
                _image.color = _deadColor;
                break;
        }
    }
}
