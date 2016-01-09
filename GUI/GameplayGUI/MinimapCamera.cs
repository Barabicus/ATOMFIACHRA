using UnityEngine;
using System.Collections;

public class MinimapCamera : MonoBehaviour
{
    private PlayerController _player;

    public static MinimapCamera Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start ()
	{
	    _player = GameMainReferences.Instance.PlayerController;
	}
	
	// Update is called once per frame
    private void Update()
    {
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 15f, _player.transform.position.z);
    }
}
