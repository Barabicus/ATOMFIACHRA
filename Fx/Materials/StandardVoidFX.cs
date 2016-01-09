using UnityEngine;
using System.Collections;

public class StandardVoidFX : MonoBehaviour
{
    private bool _doDropFX = true;
    private float _dropDistance = 20f;
    private float _dropMinSpeed = 4f;
    private float _dropMaxSpeed = 6f;

    private Material _matCache;

    private float _dropAmount = 1f;
    private bool _doDrop = false;
    private float _dropSpeed;

    private void Awake()
    {
        _matCache = GetComponent<Renderer>().material;
        if(_matCache == null)
        {
            Destroy(this);
            return;
        }
        // Get the drop speed
        _dropSpeed = Random.Range(_dropMinSpeed, _dropMaxSpeed);
        if (!_doDropFX)
        {
            _matCache.SetFloat("_PositionOffset", 0f);
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(_doDropFX)
        DropUpdate();
    }

    private void DropUpdate()
    {
        if (_doDrop)
        {
            _dropAmount = Mathf.Lerp(_dropAmount, 0f, Time.deltaTime * _dropSpeed);
        }
        else if (Vector3.Distance(GameMainReferences.Instance.PlayerController.transform.position, transform.position) <= _dropDistance)
        {
            _doDrop = true;
        }

        _matCache.SetFloat("_PositionOffset", _dropAmount);

    }
}
