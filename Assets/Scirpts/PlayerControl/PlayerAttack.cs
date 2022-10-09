using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : AttackBase
{
    private AtkWeapon atkWeapon;
    private PlayerBehavior playerBehavior;
    private AnimatorStateInfo stateInfo;
    private bool isAtk;

    private Animator animator;
    private PlayerAnimControl animControl;

    public void MyUpdate(GameObject player, int id)
    {
        playerBehavior = player.GetComponent<PlayerBehavior>();
        if (playerBehavior.playerState == PlayerStates.attack)
        {
            atkWeapon = atkWeapons[id];
            Act();
        }
    }

    public PlayerAttack()
    {
        AtkWeapon weapon = new AtkWeapon();
        weapon.weaponID = 0;
        weapon.action = 1;
        atkWeapons.Add(0, weapon);
        animControl = new PlayerAnimControl();
    }

    public override void Act()
    {
       animator.SetInteger(animControl.animIDWeapon, atkWeapon.weaponID);
        animator.SetInteger(animControl.animIDAction, atkWeapon.action);
        if (!isAtk)
        {
            animator.SetTrigger(animControl.animIDTrigger);
            isAtk = true;
        }

        Debug.Log("aaaa");
        
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Idle"))
        {
            if (stateInfo.normalizedTime >= 0.99f)
            {
                playerBehavior.playerState = PlayerStates.move;
            }
        }
    }
}