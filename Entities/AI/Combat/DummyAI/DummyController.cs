using UnityEngine;
using System.Collections;

public class DummyController : EntityComponent
{

    public bool autoRevive = true;

    protected override void OnKilled(Entity e)
    {
        base.OnKilled(e);
        if (autoRevive)
            Invoke("Resurrect", 5f);
    }

    private void Resurrect()
    {
        Entity.CurrentHp = Entity.StatHandler.MaxHp;
        Entity.LivingState = EntityLivingState.Alive;
        EntityManager.Instance.AddEntity(Entity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Entity.ResetEntity();
        }
    }

}
