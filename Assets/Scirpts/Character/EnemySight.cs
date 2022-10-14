using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour ,Interface.ISightCheck
{
    public float fov = 110;//视野广度
    public bool playerInSight;//玩家是否进入视野
    public bool playerInAttactSight;//玩家是否进入攻击距离;
    public Vector3 playerLastSight;//玩家在视野中最后的位置
    public bool isWarning=false;//敌人是否在警戒
    public bool isWaiting=false;//是否在观望
    public static Vector3 resetPos = Vector3.back;
    private GameObject player;
    private SphereCollider col;//视野球的碰撞体
    WaitForSeconds waitForSeconds = new WaitForSeconds(5f);
    private Coroutine SerchingCor;
    private bool SerchingCorActive=false;//SerchingCor状态检测
    public Coroutine movingToLastSight;//移动到玩家最后出现位置的协程

    void Start()
    {
        SightCheck();

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInSight = false;
            playerInAttactSight = false;
            Vector3 direction = other.transform.position - transform.position;//direction为敌人朝玩家的方向
            float angle = Vector3.Angle(direction, transform.forward);//angle为敌人朝玩家的方向与自身朝向的夹角的角度
            if (angle < fov * 0.5f)//在fov内,则进入视野
            {
                if((other.transform.position-transform.position).sqrMagnitude<4)
                {
                    playerInAttactSight = true;
                }
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))//射线检测
                {
                    if (hit.collider.gameObject == player)//如果检测到物体是player
                    {
                        playerInSight = true;
                        isWarning = true;
                        playerLastSight = player.transform.position;
                        if(isWaiting&& SerchingCorActive)
                        {
                            StopCoroutine(SerchingCor);
                            SerchingCorActive = false;
                        }
                    }
                }
            }
            if(isWarning&&!playerInSight)
            {
                if (SerchingCorActive==false)
                {
                    SerchingCor = StartCoroutine(Serching());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject == player)
        {
            playerInSight = false;
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            if (angle < fov * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.size.z))
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                        Debug.Log("playerInSight has been true");
                        playerLastSight = player.transform.position;
                        Debug.Log("玩家位置:" + player.transform.position);
                    }
                }
            }
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            /*isWaiting = true;*/
            if(!SerchingCorActive)
                SerchingCor = StartCoroutine(Serching());
            isWarning = false;
            /*isWaiting = false;
            playerInSight = false;*/
        }
    }

    public void SightCheck()
    {
        col = GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerLastSight = resetPos;
    }
    IEnumerator Serching()
    {
        SerchingCorActive = true;
        isWaiting = true;
        yield return waitForSeconds;
        isWarning = false;
        playerInSight = false;
        isWaiting = false;
        SerchingCorActive = false;
    }
}