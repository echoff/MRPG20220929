using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum PlayerStates
{
    move,
    attack,
    hurt,
    die
}

public class PlayerBehavior : MonoBehaviour
{
    [HideInInspector] public PlayerStates playerState = PlayerStates.move;

    /// <summary>
    /// 行走速度
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// 冲刺速度
    /// </summary>
    public float sprintSpeed = 5.335f;

    /// <summary>
    /// 加速与减速的速率
    /// </summary>
    public float speedChangeRate = 10f;

    /// <summary>
    /// 角色转向移动方向的速度
    /// </summary>
    public float rotationSmoothTime;

    /// <summary>
    /// 提前判断是否触碰地面(适用于粗糙的地面)
    /// </summary>
    public float groundedOffset = -0.14f;

    /// <summary>
    /// 接触地面时检查的半径(判断是否是地面)
    /// </summary>
    public float groundedRadius = 0.28f;

    /// <summary>
    /// 地面的层
    /// </summary>
    public LayerMask groundLayers;

    /// <summary>
    /// 进入下落状态之前需要经过的时间(用于判断是否在走楼梯)
    /// </summary>
    public float fallTimeout = 0.15f;

    /// <summary>
    /// 角色跳跃的高度
    /// </summary>
    public float jumpHeight = 1.2f;

    /// <summary>
    /// 设置角色自己的重力(其默认值是 -9.81f)
    /// </summary>
    public float gravity = -15;

    /// <summary>
    /// 跳跃后再次跳跃的间隔时间
    /// </summary>
    public float jumpTimeout = 0.50f;

    public AudioClip landingAudioClip;

    public AudioClip[] footstepAudioClips;
    [Range(0, 1)] public float footstepAudioVolume = 0.5f;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;

    private CharacterController controller;

    private void Start()
    {
        playerMove = new PlayerMove(gameObject);
        playerAttack = new PlayerAttack();
        
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        playerMove.MyUpdate();
        // playerAttack.MyUpdate(gameObject,0);
        // if (StarterAssetsInputs.Instance.attack)
        // {
        //     playerState = PlayerStates.attack;
        // }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(controller.center),
                    footstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(controller.center),
                footstepAudioVolume);
        }
    }
}