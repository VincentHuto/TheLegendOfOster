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

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (inputHandler.comboFlag)
        {
            animatorHandler.anim.SetBool("canDoCombo", false);
            if (lastAttack == weapon.OH_Right_Light_Attack_1)
            {
                animatorHandler.PlayTargetAnimation("OH_Right_Light_Attack_2", true);
            }
            if (lastAttack == weapon.OH_Left_Light_Attack_1)
            {
                animatorHandler.PlayTargetAnimation("OH_Left_Light_Attack_2", true);
            }
        }
    }

    public void HandleLightAttack(WeaponItem weapon, bool isLeft)
    {
        if (weapon != null && !weapon.isUnarmed)
        {
            if (playerStats.currentStamina > weapon.GetWeaponStaminaCost(false))
            {
                if (isLeft)
                {
                    weaponSlotManager.attackingWeapon = weapon;
                    animatorHandler.PlayTargetAnimation(weapon.OH_Left_Light_Attack_1, true);
                    lastAttack = weapon.OH_Left_Light_Attack_1;
                }
                else
                {
                    weaponSlotManager.attackingWeapon = weapon;
                    animatorHandler.PlayTargetAnimation(weapon.OH_Right_Light_Attack_1, true);
                    lastAttack = weapon.OH_Right_Light_Attack_1;
                }
            }
        }
    }

    public void HandleHeavyAttack(WeaponItem weapon, bool isLeft)
    {
        if (weapon != null && !weapon.isUnarmed)
        {
            if (playerStats.currentStamina > weapon.GetWeaponStaminaCost(true))
            {
                if (isLeft)
                {
                    weaponSlotManager.attackingWeapon = weapon;
                    animatorHandler.PlayTargetAnimation(weapon.OH_Left_Heavy_Attack_1, true);
                    lastAttack = weapon.OH_Left_Heavy_Attack_1;
                }
                else
                {
                    weaponSlotManager.attackingWeapon = weapon;
                    animatorHandler.PlayTargetAnimation(weapon.OH_Right_Heavy_Attack_1, true);
                    lastAttack = weapon.OH_Right_Heavy_Attack_1;
                }
            }

        }

    }
}
