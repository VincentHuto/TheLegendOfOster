using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    public float horizontal, vertical, moveAmount, mouseX, mouseY, rollInputTimer;

    public bool jump_Input, b_Input, rb_Input, rt_Input, lb_Input, lt_Input,
        d_Pad_Up, d_Pad_Down, d_Pad_Left, d_Pad_Right, pickup_Input, inv_Input,
        lockOnInput, right_Stick_Right_Input, right_Stick_Left_Input, x_Input;

    public bool rollFlag, sprintFlag, comboFlag, invFlag, lockOnFlag;

    PlayerControls inputActions;
    PlayerLocomotion playerLocomotion;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;
    WeaponSlotManager weaponSlotManager;
    PlayerManager playerManager;
    PlayerStats playerStats;
    PlayerEffectsManager playerEffectsManager;
    AnimatorManager animatorManager;
    UIManager uIManager;
    CameraHandler cameraHandler;

    Vector2 movementInput;
    Vector2 cameraInput;


    public void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAttacker = GetComponentInChildren<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        uIManager = FindObjectOfType<UIManager>();
        playerEffectsManager = GetComponentInChildren<PlayerEffectsManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        cameraHandler = FindObjectOfType<CameraHandler>();
        animatorManager = GetComponentInChildren<PlayerAnimatorManager>();

    }

    public void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputActions.PlayerActions.Roll.performed += i => b_Input = true;
            inputActions.PlayerActions.Roll.canceled += i => b_Input = false;
            inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i => rt_Input = true;
            inputActions.PlayerActions.LB.performed += i => lb_Input = true;
            inputActions.PlayerActions.LT.performed += i => lt_Input = true;
            inputActions.PlayerActions.X.performed += i => x_Input = true;
            inputActions.PlayerActions.DPadRight.performed += i => d_Pad_Right = true;
            inputActions.PlayerActions.DPadLeft.performed += i => d_Pad_Left = true;
            inputActions.PlayerActions.DPadDown.performed += i => d_Pad_Down = true;
            inputActions.PlayerActions.DPadUp.performed += i => d_Pad_Up = true;
            inputActions.PlayerActions.Interact.performed += i => pickup_Input = true;
            inputActions.PlayerActions.Inventory.performed += i => inv_Input = true;
            inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
            inputActions.PlayerActions.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
            inputActions.PlayerActions.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
        }
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void HandleAllInputs(float delta)
    {
        HandleMoveInput(delta);
        HandleRollInput(delta);
        HandleAttackInput(delta);
        HandleJumpInput();
        HandleQuickSlotInput();
        HandleInventoryInput();
        HandleLockOnInput();
        HandleUseConsumableInput();

    }

    private void HandleMoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }
    private void HandleRollInput(float delta)
    {
        if (b_Input)
        {
            rollInputTimer += delta;

            if (playerStats.currentStamina <= 0)
            {
                b_Input = false;
                sprintFlag = false;
            }

            if (moveAmount > 0.5f && playerStats.currentStamina > 0)
            {
                sprintFlag = true;
            }
        }
        else
        {
            sprintFlag = false;

            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                rollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    private void HandleJumpInput()
    {
        if (playerManager.isInteracting)
            return;

        if (jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }

    }
    private void HandleAttackInput(float delta)
    {
        if (rb_Input)
        {
            playerAttacker.HandleRBAction();
        }

        if (rt_Input)
        {
            playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon, false);
        }

        if (lb_Input)
        {
            playerAttacker.HandleLBAction();
        }

        if (lt_Input)
        {
            playerAttacker.HandleHeavyAttack(playerInventory.leftWeapon, true);
        }

    }
    private void HandleQuickSlotInput()
    {

        if (d_Pad_Right)
        {
            playerInventory.ChangeRightWeapon();
        }
        else if (d_Pad_Left)
        {
            playerInventory.ChangeLeftWeapon();
        }
        else if (d_Pad_Down)
        {
            playerInventory.ChangeCurrentConsumable();

        }
        else if (d_Pad_Up)
        {
            playerInventory.ChangeCurrentSpell();

        }


    }
    private void HandleInventoryInput()
    {
        if (inv_Input)
        {
            invFlag = !invFlag;

            if (invFlag)
            {
                uIManager.OpenSelectWindow();
                uIManager.UpdateUI();
            }
            else
            {
                uIManager.CloseSelectWindow();
                uIManager.CloseAllInventoryWindows();
                uIManager.hudWindow.SetActive(true);
            }
        }
    }
    private void HandleUseConsumableInput()
    {
        if (x_Input)
        {
            x_Input = false;
            playerInventory.currentConsumable.AtteptToConsumeItem(animatorManager, weaponSlotManager, playerEffectsManager);
        }
    }
    private void HandleLockOnInput()
    {
        if (lockOnInput && lockOnFlag == false)
        {

            lockOnInput = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                cameraHandler.currentLockOnTarget.spriteRenderer.gameObject.SetActive(true);
                lockOnFlag = true;
            }
        }
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraHandler.currentLockOnTarget.spriteRenderer.gameObject.SetActive(false);
            cameraHandler.ClearLockOnTargets();
        }

        if (lockOnFlag && right_Stick_Left_Input)
        {
            right_Stick_Left_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.leftLockTarget != null)
            {
                cameraHandler.currentLockOnTarget.spriteRenderer.gameObject.SetActive(false);
                cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                cameraHandler.currentLockOnTarget.spriteRenderer.gameObject.SetActive(true);
            }
        }

        if (lockOnFlag && right_Stick_Right_Input)
        {
            right_Stick_Right_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.rightLockTarget != null)
            {
                cameraHandler.currentLockOnTarget.spriteRenderer.gameObject.SetActive(false);
                cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                cameraHandler.currentLockOnTarget.spriteRenderer.gameObject.SetActive(true);
            }
        }

        cameraHandler.SetCameraHeight();
    }


}
