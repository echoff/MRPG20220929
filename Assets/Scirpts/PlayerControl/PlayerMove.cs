using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PlayerMove
{
    private float speed;
    private float animationBlend;
    private float targetRotation;
    private bool Grounded = true;

    private GameObject mainCamera;
    private float rotationVelocity;
    private float verticalVelocity;

    private GameObject player;
    private Animator animator;

    private PlayerBehavior playerBehavior;
    private CharacterController controller;
    private double jumpTimeoutDelta;
    private float fallTimeoutDelta;
    private float terminalVelocity = 53.0f;

    private PlayerStates myState;

    private PlayerAnimControl animControl;

    public PlayerMove(GameObject player)
    {
        this.player = player;
        playerBehavior = player.GetComponent<PlayerBehavior>();
        animator = player.GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        controller = player.GetComponent<CharacterController>();
        animControl = new PlayerAnimControl();
    }

    public void MyUpdate()
    {
        if (playerBehavior.playerState == PlayerStates.move)
        {
            GroundedCheck();
            JumpAndGravity();
            Move();
        }
    }

    public void Move()
    {
        float targetSpeed = StarterAssetsInputs.Instance.sprint ? playerBehavior.sprintSpeed : playerBehavior.moveSpeed;

        if (StarterAssetsInputs.Instance.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = StarterAssetsInputs.Instance.analogMovement ? StarterAssetsInputs.Instance.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * playerBehavior.speedChangeRate);

            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * playerBehavior.speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        Vector3 inputDirection = new Vector3(StarterAssetsInputs.Instance.move.x, 0.0f, StarterAssetsInputs.Instance.move.y).normalized;

        if (StarterAssetsInputs.Instance.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                             mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation,
                ref rotationVelocity,
                playerBehavior.rotationSmoothTime);
            player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                        new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        if (animator != null)
        {
            animator.SetFloat(animControl.animIDSpeed, animationBlend);
            animator.SetFloat(animControl.animIDMotionSpeed, inputMagnitude);
        }
    }

    public void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = playerBehavior.fallTimeout;

            // update animator if using character
            if (animator)
            {
                animator.SetBool(animControl.animIDJump, false);
                animator.SetBool(animControl.animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (StarterAssetsInputs.Instance.jump && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                // H(高度) * -2 * G(重力) 的平方根 = 达到所需高度所需的速度
                verticalVelocity = Mathf.Sqrt(playerBehavior.jumpHeight * -2f * playerBehavior.gravity);

                // update animator if using character
                if (animator)
                {
                    animator.SetBool(animControl.animIDJump, true);
                }
            }

            // jump timeout
            // 跳跃超时
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = playerBehavior.jumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (animator)
                {
                    animator.SetBool(animControl.animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            StarterAssetsInputs.Instance.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        // 如果在终端下，则随时间应用重力（乘以增量时间两次，以随时间线性加速）
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += playerBehavior.gravity * Time.deltaTime;
        }
    }

    public void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(player.transform.position.x,
            player.transform.position.y - playerBehavior.groundedOffset,
            player.transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, playerBehavior.groundedRadius, playerBehavior.groundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (animator)
        {
            animator.SetBool(animControl.animIDGrounded, Grounded);
        }
    }
}