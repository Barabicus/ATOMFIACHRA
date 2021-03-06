﻿using UnityEngine;
using System.Collections;

public class TowerEntity : EntityComponent {

    public Transform rotPoint;
    public Spell attackSpell;
    public float rotateSpeed = 5f;
    public float attackRange = 50f;
    private PlayerController player;

    public override void OnStart()
    {
        base.OnStart();
        player = GameMainReferences.Instance.PlayerController;
    }

    protected override void Update()
    {
        base.LivingUpdate();
        Attack();
    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            rotPoint.transform.rotation = Quaternion.Lerp(rotPoint.transform.rotation, Quaternion.LookRotation(player.transform.position - rotPoint.transform.position), Time.deltaTime * rotateSpeed);

            Spell spell;
            if (Entity.CastSpell(attackSpell, out spell))
            {
                //spell.SpellTarget = player.transform;
                //spell.SpellTargetPosition = player.transform.position;
            }
        }
    }

    public bool KeepBeamAlive()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= attackRange && Entity.LivingState == EntityLivingState.Alive;
    }
}
