using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class PlayerMovePositionOverlay : GameController
{

    [SerializeField]
    private RectTransform _container;
    [SerializeField]
    private Transform _particleField;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _fadeDistance = 5f;
    [SerializeField]
    private float _fadeSpeed = 5f;

    /// <summary>
    /// Player entity reference
    /// </summary>
    private Entity _player;
    /// <summary>
    /// Camera Transform
    /// </summary>
    private Transform _camera;
    private CanvasGroup _containerCanavasGroup;
    private PlaygroundParticlesC _particles;

    public override void OnStart()
    {
        base.OnStart();
        _camera = GameMainReferences.Instance.RTSCamera.transform;
        _player = GameMainReferences.Instance.PlayerController.Entity;
        _containerCanavasGroup = _container.GetComponent<CanvasGroup>();
        _particles = _particleField.GetComponentInChildren<PlaygroundParticlesC>();
    }

    private void Update()
    {
        if (_player.EntityPathFinder.Target.HasValue && !GameMainController.Instance.IsCinematic)
        {
            _container.position = _player.EntityPathFinder.Target.Value + new Vector3(0f, 0.25f, 0f);
            float fadeAlpha = Vector3.Distance(_container.position, _player.transform.position) <= _fadeDistance ? 0f : 1f;
            HandleFade(fadeAlpha);
        }
        else
        {
            HandleFade(0f);
        }
        //if(_player.EntityPathFinder.Target.HasValue && !GameMainController.Instance.IsCinematic)
        //{
        //    _particles.emit = true;
        //    _particleField.transform.position = _player.EntityPathFinder.Target.Value;
        //    _particleField.transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    _particles.emit = false;
        //}
    }

    private void HandleFade(float value)
    {
        _containerCanavasGroup.alpha = Mathf.Lerp(_containerCanavasGroup.alpha, value, Time.deltaTime * _fadeSpeed);

    }


}
