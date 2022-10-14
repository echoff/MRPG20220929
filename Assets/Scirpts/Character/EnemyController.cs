using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, Interface.IChasing,Interface.IPatrolling,Interface.IEnemyAttack,Interface.IEnemyDied,Interface.IEnemyRunAway
{
    public float patrolSpeed = 3f;//巡逻速度
    public float patrolWaitTime = 2f;//巡逻到点停止时间
    public Transform patrolWayPoints;//目标点
    public NavMeshAgent agent;
    private float patrolTimer = 0f;//到达目标点停留的时间
    private int wayPointIndex = 0;//当前父物体的第几个子物体

    public float chaseSpeed = 6f;//追踪速度
    public float chaseWaitTime = 5f;//追踪时间
    private float chaseTimer = 0f;//已经追踪了多久
    public float sqrPlayerDist = 4f;//开始攻击的距离的平方
    public bool chase = false;//当前是否处于追踪状态

    public float EnemyTurnToPlayerSpeed=6f;//敌人转向玩家速度
    public float EnemyAttackCD=2f;//敌人攻击cd
    private float EnemyAttackPass = 2f;//敌人攻击过了多久

    public Enemy1 enemy1;
    public EnemySight enemySight;
    private Transform player;
    public float playerHP = 100f;//player的生命值，后续和上一行合并为Player类的实例
    public float playerExp = 0;//player的经验值，后续同上
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        enemySight = transform.Find("EnemySight").GetComponent<EnemySight>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemySight.playerInAttactSight && enemy1.EnemyHealth>=20)
        {
            EnemyAttacking();
        }
        else if (enemy1.EnemyHealth < 20)
        {
            Debug.Log("在逃跑");
            EnemyRunAway();
        }
        else if (enemySight.playerInSight)
        {
            Chasing();
        }
        else if(!enemySight.isWaiting&& !enemySight.playerInSight)
        {
            Debug.Log("进入了巡逻状态");
            Patrolling();
        }
        else if(enemySight.isWaiting)
        {
            /*movingToLastSight = StartCoroutine(MoveToLastSight());
            StopCoroutine(movingToLastSight);*/
            agent.isStopped = true;
        }
        else
        {
            agent.SetDestination(enemySight.playerLastSight);
        }
    }
    public void EnemyAttacking()
    {
        //实现基本朝向玩家
        Debug.Log("进入了攻击方法");
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        Vector3 targetDir = lookPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Mathf.Min(Time.deltaTime * EnemyTurnToPlayerSpeed));
        agent.isStopped = true;//停下巡逻功能转为攻击
        agent.speed = 0f;
        if(EnemyAttackPass>EnemyAttackCD)
        {
                playerHP -= enemy1.EnemyAttack;
                EnemyAttackPass = 0;
         }
        EnemyAttackPass += Time.deltaTime;
        Debug.Log("cd时间" + EnemyAttackPass);

    }
    public void Chasing()
    {
        //实现基本朝向玩家
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        Vector3 targetDir = lookPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Mathf.Min(Time.deltaTime * EnemyTurnToPlayerSpeed));
        agent.isStopped = false;
        Vector3 sightDeltPos = enemySight.playerLastSight - transform.position;
        if (sightDeltPos.sqrMagnitude > sqrPlayerDist)//追踪距离是否大于要求的距离
        {
            agent.SetDestination(enemySight.playerLastSight);
            agent.speed = chaseSpeed;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)//如果已经到最后发现玩家的位置，并且未发现玩家，等待，如果还是未发现，就返回
            {
                chaseTimer += Time.deltaTime;
                if (chaseTimer > chaseWaitTime)
                {
                    chase = false;
                    agent.speed = 0f;
                }
            }
            else
            {
                chaseTimer = 0;
            }
        }
        /*if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)//如果已经到最后发现玩家的位置，并且未发现玩家，等待，如果还是未发现，就返回
        {
            Debug.Log("到达最后发现玩家的位置，进行等待");
            chaseTimer += Time.deltaTime;
            if (chaseTimer > chaseWaitTime)
            {
                Debug.Log("未发现玩家，返回巡逻状态");
                chase = false;
                chaseTimer = 0f;
            }
        }*/
    }
    public void Patrolling()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer > patrolWaitTime)
            {
                if (wayPointIndex == patrolWayPoints.childCount - 1)
                    wayPointIndex = 0;
                else
                    wayPointIndex++;
                patrolTimer = 0;
            }
        }
        else
            patrolTimer = 0;
        agent.destination = patrolWayPoints.GetChild(wayPointIndex).position;
    }

    public void EnemyDied()
    {
        playerExp += 10;
        Destroy(this);
    }
    public void EnemyRunAway()
    {
        agent.speed = 2;
        agent.isStopped = false;
    }
}
