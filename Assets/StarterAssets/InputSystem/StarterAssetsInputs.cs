using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using RPGCharacterAnims.Actions;
using UnityEngine.InputSystem;
#endif
public class StarterAssetsInputs : SingletonMonoBase<StarterAssetsInputs>
    {
        [Header("Character Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool attack;

        [Header("Movement Settings")] public bool analogMovement;

        [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

        public void OnMove(InputAction.CallbackContext value)
        {
            MoveInput(value.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.ReadValue<Vector2>());
            }
        }

        public void OnAttack(InputAction.CallbackContext value)
        {
            AttackInput(value.ReadValueAsButton());
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            JumpInput(value.ReadValueAsButton());
        }

        public void OnSprint(InputAction.CallbackContext value)
        {
            SprintInput(value.ReadValueAsButton());
        }
        // public void OnMove(InputValue value)
        // {
        // 	MoveInput(value.Get<Vector2>());
        // }
        //
        // public void OnLook(InputValue value)
        // {
        // 	if(cursorInputForLook)
        // 	{
        // 		LookInput(value.Get<Vector2>());
        // 	}
        // }
        //
        // public void OnAttack(InputValue value)
        // {
        // 	AttackInput(value.isPressed);
        // }
        //
        // public void OnJump(InputValue value)
        // {
        // 	JumpInput(value.isPressed);
        // }
        //
        // public void OnSprint(InputValue value)
        // {
        // 	SprintInput(value.isPressed);
        // }
#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
