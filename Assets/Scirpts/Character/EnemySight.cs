using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour ,Interface.ISightCheck
{
    public float fov = 110;//��Ұ���
    public bool playerInSight;//����Ƿ������Ұ
    public bool playerInAttactSight;//����Ƿ���빥������;
    public Vector3 playerLastSight;//�������Ұ������λ��
    public bool isWarning=false;//�����Ƿ��ھ���
    public bool isWaiting=false;//�Ƿ��ڹ���
    public static Vector3 resetPos = Vector3.back;
    private GameObject player;
    private SphereCollider col;//��Ұ�����ײ��
    WaitForSeconds waitForSeconds = new WaitForSeconds(5f);
    private Coroutine SerchingCor;
    private bool SerchingCorActive=false;//SerchingCor״̬���
    public Coroutine movingToLastSight;//�ƶ������������λ�õ�Э��

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
            Vector3 direction = other.transform.position - transform.position;//directionΪ���˳���ҵķ���
            float angle = Vector3.Angle(direction, transform.forward);//angleΪ���˳���ҵķ�����������ļнǵĽǶ�
            if (angle < fov * 0.5f)//��fov��,�������Ұ
            {
                if((other.transform.position-transform.position).sqrMagnitude<4)
                {
                    playerInAttactSight = true;
                }
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))//���߼��
                {
                    if (hit.collider.gameObject == player)//�����⵽������player
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
                        Debug.Log("���λ��:" + player.transform.position);
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