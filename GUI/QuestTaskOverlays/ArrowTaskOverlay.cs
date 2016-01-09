using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArrowTaskOverlay : QuestTaskOverlayItem
{
    [SerializeField]
    private Transform _locationOverlayPointer;
    [SerializeField]
    private Transform _nearPlayerPointer;
    [SerializeField]
    private Vector3 _playerArrowOffset;
    [SerializeField]
    private float _directionOffset = 5f;
    [SerializeField]
    private float _playerDistanceDisable = 10f;

    private float _worldOffset;
    private float _rotTime;

    private Transform _player;
    private IPathFinder _playerPathFinderCache;

    public Transform TargetTransform { get; set; }
    public float PlayerToTargetDistance { get { return Vector3.Distance(TargetTransform.position, _player.transform.position); } }

    public override void Initialise()
    {
        base.Initialise();
        _playerPathFinderCache = GameMainReferences.Instance.PlayerCharacter.Entity.EntityPathFinder;
        _rotTime = Random.Range(0f, 2000f);
        _player = GameMainReferences.Instance.PlayerCharacter.transform;
    }

    public override void PoolStart()
    {
        base.PoolStart();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        UpdateWorldOverlay();
    }

    private void UpdateWorldOverlay()
    {
       // _worldImage.transform.position = RectTransformUtility.WorldToScreenPoint(MainGameCamera, TargetTransform.position + new Vector3(0, _worldOffset + 2f, 0));
       _locationOverlayPointer.position = TargetTransform.position + new Vector3(0, _worldOffset + 2f, 0);
        _rotTime += Time.deltaTime * 5f;
        _worldOffset = Mathf.Sin(_rotTime) * 0.15f;
        _locationOverlayPointer.LookAt(TargetTransform);
        if (PlayerToTargetDistance < _playerDistanceDisable)
        {
            _nearPlayerPointer.gameObject.SetActive(false);
        }
        else
        {
            // Find the next point in the path and aim towards it
            var targetPoint = FindArrowTargetPoint(TargetTransform.position);
            if(targetPoint == null)
            {
                _nearPlayerPointer.gameObject.SetActive(false);
                return;
            }

            _nearPlayerPointer.gameObject.SetActive(true);
            var nearPointerDirection = (targetPoint.Value - _nearPlayerPointer.position).normalized * _directionOffset;
            // Move the point towards the direction.
            _nearPlayerPointer.transform.position = Vector3.Lerp(_nearPlayerPointer.transform.position, _player.position + nearPointerDirection + new Vector3(0, 2f, 0), Time.deltaTime * 5f);
            // Look at the target
            _nearPlayerPointer.LookAt(targetPoint.Value);
        }
    }

    private Vector3? FindArrowTargetPoint(Vector3 target)
    {

        var path = _playerPathFinderCache.CalculatePath(target);
        if(path != null && path.Length > 1)
        {
            for(int i =0; i < path.Length; i++)
            {
                if(Vector3.Distance(path[i], _player.position) >= _playerDistanceDisable + 1f)
                {
                    return path[i];
                }
            }
        }
        return null;
    }


}