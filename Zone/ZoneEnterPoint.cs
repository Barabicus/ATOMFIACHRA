using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ZoneEnterPoint : MonoBehaviour
{
    private const string NOT_SET = "NOT_SET";

    [SerializeField]
    private string _targetScene = NOT_SET;
    [SerializeField]
    private Vector3 _boundsExtents = new Vector3(2f, 2f, 2f);
    [SerializeField]
    private bool _instantLoad = false;

    private Bounds _bounds;

    private Transform _player;

    private void Awake()
    {
        _bounds = new Bounds(transform.position, _boundsExtents);
        if (_targetScene.Equals(NOT_SET))
        {
            Debug.LogError("Zone Enter Point has invalid target scene!");
        }
    }

    private void Start()
    {
        _player = GameMainReferences.Instance.PlayerCharacter.transform;
    }

    private void Update()
    {
        if (_bounds.Contains(_player.position))
        {
            DoZoneTrigger();
        }
    }

    private void DoZoneTrigger()
    {
        if (!_instantLoad)
        {
            GUILoad.LoadLevel(_targetScene);
        }
        else
        {
            SceneManager.LoadScene(_targetScene);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _boundsExtents);
    }

    private void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("EnterPoint");
    }
}
