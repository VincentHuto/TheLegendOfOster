using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    public float horizontal, vertical, moveAmount, mouseX, mouseY, rollInputTimer;

    public bool jump_Input, b_Input, rb_Input, rt_Input, lb_Input, lt_Input,
        d_Pad_Up, d_Pad_Down, d_Pad_Left, d_Pad_Right, pickup_Input,inv_Input;

    public bool rollFlag, sprintFlag, comboFlag,invFlag;

    PlayerControls inputActions;
    PlayerLocomotion playerLocomotion;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;
    PlayerManager playerManager;
    PlayerStats playerStats;
    PlayerEffectsManager playerEffectsManager;
    UIManager uIManager;
    Vector2 movementInput;
    Vector2 cameraInput;


    public void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        playerStats = GetComponent<PlayerStats>();
        uIManager = FindObjectOfType<UIManager>();
        playerEffectsManager = GetComponentInChildren<PlayerEffectsManager>();

    }

    public void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i => rt_Input = true;
            inputActions.PlayerActions.LB.performed += i => lb_Input = true;
            inputActions.PlayerActions.LT.performed += i => lt_Input = true;
            inputActions.PlayerActions.DPadRight.performed += i => d_Pad_Right = true;
            inputActions.PlayerActions.DPadLeft.performed += i => d_Pad_Left = true;
            inputActions.PlayerActions.Interact.performed += i => pickup_Input = true;
            inputActions.PlayerActions.Inventory.performed += i => inv_Input = true;


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
        b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        sprintFlag = b_Input;

        if (b_Input)
        {
            rollInputTimer += delta;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
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
     

        //RB handles the right hands light attack
        if (rb_Input)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                if (playerManager.canDoCombo)
                    return;
                playerAttacker.HandleLightAttack(playerInventory.rightWeapon, false);
            }
        }

        //RT handles the right hands heavy attack
        if (rt_Input)
        {
            playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon, false);
        }

        //LB handles the left hands light attack
        if (lb_Input)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.leftWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                if (playerManager.canDoCombo)
                    return;
                playerAttacker.HandleLightAttack(playerInventory.leftWeapon, true);
            }
        }

        //LT handles the left hands heavy attack
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
}
