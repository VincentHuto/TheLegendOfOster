using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    PlayerManager playerManager;
    PlayerInventory playerInventory;
    AnimatorManager animatorHandler;
    InputHandler inputHandler;
    WeaponSlotManager weaponSlotManager;
    PlayerStats playerStats;
    public string lastAttack;

    private void Awake()
    {
        playerStats = GetComponentInChildren<PlayerStats>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerInventory = GetComponentInParent<PlayerInventory>();
        animatorHandler = GetComponentInChildren<AnimatorManager>();
        inputHandler = GetComponentInChildren<InputHandler>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    public void HandleWeaponCombo(WeaponItemStack weapon)
    {
        if (weapon.itemType is WeaponItem)
        {
            WeaponItem weaponItem = (WeaponItem)weapon.itemType;

            if (inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);
                if (lastAttack == weaponItem.OH_Right_Light_Attack_1)
                {
                    animatorHandler.PlayTargetAnimation("OH_Right_Light_Attack_2", true);
                }
                if (lastAttack == weaponItem.OH_Left_Light_Attack_1)
                {
                    animatorHandler.PlayTargetAnimation("OH_Left_Light_Attack_2", true);
                }
            }
        }
    }

    public void HandleLightAttack(WeaponItemStack weapon, bool isLeft)
    {
        if (weapon.itemType is WeaponItem)
        {
            WeaponItem weaponItem = (WeaponItem)weapon.itemType;

            if (weapon != null && !weaponItem.isUnarmed)
            {


                if (playerStats.currentStamina > weaponItem.GetWeaponStaminaCost(false))
                {
                    if (isLeft)
                    {
                        weaponSlotManager.attackingWeapon = weaponItem;
                        animatorHandler.PlayTargetAnimation(weaponItem.OH_Left_Light_Attack_1, true);
                        lastAttack = weaponItem.OH_Left_Light_Attack_1;
                    }
                    else
                    {
                        weaponSlotManager.attackingWeapon = weaponItem;
                        animatorHandler.PlayTargetAnimation(weaponItem.OH_Right_Light_Attack_1, true);
                        lastAttack = weaponItem.OH_Right_Light_Attack_1;

                    }
                }
            }
        }
    }

    public void HandleHeavyAttack(WeaponItemStack weapon, bool isLeft)
    {
        if (weapon.itemType is WeaponItem)
        {
            WeaponItem weaponItem = (WeaponItem)weapon.itemType;

            if (weapon != null && !weaponItem.isUnarmed)
            {
                if (playerStats.currentStamina > weaponItem.GetWeaponStaminaCost(true))
                {

                    if (isLeft)
                    {
                        weaponSlotManager.attackingWeapon = weaponItem;
                        animatorHandler.PlayTargetAnimation(weaponItem.OH_Left_Heavy_Attack_1, true);
                        lastAttack = weaponItem.OH_Left_Heavy_Attack_1;
                    }
                    else
                    {
                        weaponSlotManager.attackingWeapon = weaponItem;
                        animatorHandler.PlayTargetAnimation(weaponItem.OH_Right_Heavy_Attack_1, true);
                        lastAttack = weaponItem.OH_Right_Heavy_Attack_1;
                    }
                }

            }
        }
    }


    public void HandleRBAction()
    {
        if (((WeaponItem)playerInventory.rightWeapon.itemType).isMeleeWeapon)
        {
            PerformRBMeleeAction();
        }
        else if ((((WeaponItem)playerInventory.rightWeapon.itemType)).isSpellCaster 
            || (((WeaponItem)playerInventory.rightWeapon.itemType)).isFaithCaster 
            || (((WeaponItem)playerInventory.rightWeapon.itemType)).isPyroCaster)
        {
            PerformRBMagicAction((((WeaponItem)playerInventory.rightWeapon.itemType)));
        }
    }

    private void PerformRBMeleeAction()
    {
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventory.rightWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;

            if (playerManager.canDoCombo)
                return;

            animatorHandler.anim.SetBool("isUsingRightHand", true);
            HandleLightAttack(playerInventory.rightWeapon, false);
        }
    }

    private void PerformRBMagicAction(WeaponItem weapon)
    {
        if (weapon.isFaithCaster)
        {
            if (playerInventory.currentSpell != null && playerInventory.currentSpell.isFaithSpell)
            {
                //CHECK FOR FP
                //ATTEMPT TO CAST SPELL
            }
        }
    }

}
