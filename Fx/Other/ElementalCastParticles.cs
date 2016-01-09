using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class ElementalCastParticles : MonoBehaviour
{
    [SerializeField]
    private PlaygroundParticlesC _airParticles;
    [SerializeField]
    private PlaygroundParticlesC _earthParticles;
    [SerializeField]
    private PlaygroundParticlesC _waterParticles;
    [SerializeField]
    private PlaygroundParticlesC _fireParticles;

    [SerializeField]
    private float radius = 2f;
    [SerializeField]
    private int emitMax = 50;
    [SerializeField]
    private float emitIncrement = 1;
    [SerializeField]
    private float emitDecrement = 2;

    private Entity _entity;

    private float _fireEmitAmount = 0;

    private void Start()
    {
        _entity = transform.parent.GetComponent<Entity>();
        if (_entity == null)
        {
            Debug.LogError("Elemental Cast Particles has null entity. Make sure the parent has the Entity component");
        }
        _entity.OnElementalSubtract += ElementalSubtract;
    }

    void ElementalSubtract(ElementalStats obj)
    {
        if (obj.fire > 0)
            _fireParticles.Emit(transform.position);
    }


}
