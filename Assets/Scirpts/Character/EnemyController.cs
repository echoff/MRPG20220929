using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, Interface.IChasing,Interface.IPatrolling,Interface.IEnemyAttack,Interface.IEnemyDied,Interface.IEnemyRunAway
{
    public float patrolSpeed = 3f;//Ѳ���ٶ�
    public float patrolWaitTime = 2f;//Ѳ�ߵ���ֹͣʱ��
    public Transform patrolWayPoints;//Ŀ���
    public NavMeshAgent agent;
    private float patrolTimer = 0f;//����Ŀ���ͣ����ʱ��
    private int wayPointIndex = 0;//��ǰ������ĵڼ���������

    public float chaseSpeed = 6f;//׷���ٶ�
    public float chaseWaitTime = 5f;//׷��ʱ��
    private float chaseTimer = 0f;//�Ѿ�׷���˶��
    public float sqrPlayerDist = 4f;//��ʼ�����ľ����ƽ��
    public bool chase = false;//��ǰ�Ƿ���׷��״̬

    public float EnemyTurnToPlayerSpeed=6f;//����ת������ٶ�
    public float EnemyAttackCD=2f;//���˹���cd
    private float EnemyAttackPass = 2f;//���˹������˶��

    public Enemy1 enemy1;
    public EnemySight enemySight;
    private Transform player;
    public float playerHP = 100f;//player������ֵ����������һ�кϲ�ΪPlayer���ʵ��
    public float playerExp = 0;//player�ľ���ֵ������ͬ��
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
            Debug.Log("������");
            EnemyRunAway();
        }
        else if (enemySight.playerInSight)
        {
            Chasing();
        }
        else if(!enemySight.isWaiting&& !enemySight.playerInSight)
        {
            Debug.Log("������Ѳ��״̬");
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
        //ʵ�ֻ����������
        Debug.Log("�����˹�������");
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        Vector3 targetDir = lookPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Mathf.Min(Time.deltaTime * EnemyTurnToPlayerSpeed));
        agent.isStopped = true;//ͣ��Ѳ�߹���תΪ����
        agent.speed = 0f;
        if(EnemyAttackPass>EnemyAttackCD)
        {
                playerHP -= enemy1.EnemyAttack;
                EnemyAttackPass = 0;
         }
        EnemyAttackPass += Time.deltaTime;
        Debug.Log("cdʱ��" + EnemyAttackPass);

    }
    public void Chasing()
    {
        //ʵ�ֻ����������
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        Vector3 targetDir = lookPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Mathf.Min(Time.deltaTime * EnemyTurnToPlayerSpeed));
        agent.isStopped = false;
        Vector3 sightDeltPos = enemySight.playerLastSight - transform.position;
        if (sightDeltPos.sqrMagnitude > sqrPlayerDist)//׷�پ����Ƿ����Ҫ��ľ���
        {
            agent.SetDestination(enemySight.playerLastSight);
            agent.speed = chaseSpeed;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)//����Ѿ����������ҵ�λ�ã�����δ������ң��ȴ����������δ���֣��ͷ���
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
        /*if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)//����Ѿ����������ҵ�λ�ã�����δ������ң��ȴ����������δ���֣��ͷ���
        {
            Debug.Log("�����������ҵ�λ�ã����еȴ�");
            chaseTimer += Time.deltaTime;
            if (chaseTimer > chaseWaitTime)
            {
                Debug.Log("δ������ң�����Ѳ��״̬");
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
