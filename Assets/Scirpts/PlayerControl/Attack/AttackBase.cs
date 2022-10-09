using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkWeapon
{
    public int weaponID;
    public float atkRange;
    public int action;
    public float atkCD;
}

public abstract class AttackBase
{
    private int weaponID;
    protected Dictionary<int, AtkWeapon> atkWeapons = new Dictionary<int, AtkWeapon>();


    public void DoBeforeAtk()
    {
    }

    public abstract void Act();

    public void DoAfterAtk()
    {
    }
}