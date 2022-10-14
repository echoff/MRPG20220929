using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface: MonoBehaviour
{
    public interface IPatrolling
    {
        void Patrolling();
    }
    public interface ISightCheck
    {
        void SightCheck();
    }
    public interface IChasing
    {
        void Chasing();
    }
    public interface IEnemyAttack
    {
        void EnemyAttacking();
    }
    public interface IEnemyDied
    {
        void EnemyDied();
    }
    public interface IEnemyRunAway
    {
        void EnemyRunAway();
    }
}
