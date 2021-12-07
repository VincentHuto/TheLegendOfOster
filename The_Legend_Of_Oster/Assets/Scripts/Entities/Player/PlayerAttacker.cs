using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{

    AnimatorHandler animatorHandler;
    InputHandler inputHandler;
    WeaponSlotManager weaponSlotManager;
    PlayerStats playerStats;
    public string lastAttack;

    private void Awake()
    {
        playerStats = GetComponentInChildren<PlayerStats>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
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
                Debug.Log("HEAVY" + weaponItem.GetWeaponStaminaCost(true) + playerStats.currentStamina);

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
}
