using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl 
{
    [HideInInspector] public int animIDSpeed;
    [HideInInspector] public int animIDGrounded;
    [HideInInspector] public int animIDJump;
    [HideInInspector] public int animIDFreeFall;
    [HideInInspector] public int animIDMotionSpeed;
    [HideInInspector] public int animIDAction;
    [HideInInspector] public int animIDWeapon;
    [HideInInspector] public int animIDTrigger;
    [HideInInspector] public int animIDIsAttack;

    public PlayerAnimControl()
    {
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDAction = Animator.StringToHash("Action");
        animIDWeapon = Animator.StringToHash("Weapon");
        animIDTrigger = Animator.StringToHash("Trigger");
        animIDIsAttack = Animator.StringToHash("IsAttack");
    }
    
    
}