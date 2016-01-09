using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravitationalVortex : MonoBehaviour
{

    [SerializeField]
    private float _checkRadius;

    private List<Rigidbody> _rigidbodies;

    private void Start()
    {
        _rigidbodies = new List<Rigidbody>();
        // Get all bodies on start
        var rb = Physics.OverlapSphere(transform.position, _checkRadius, 1 << LayerMask.NameToLayer("Environment"));
       foreach(var r in rb)
        {
            var body = r.GetComponent<Rigidbody>();
            if (body != null)
            {
                _rigidbodies.Add(body);
            }
        }
    }

    private void Update()
    {

    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _checkRadius);
    }
}
