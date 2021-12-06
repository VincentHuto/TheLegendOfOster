using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot, rightHandSlot;
    DamageCollider leftDamageCollider, rightDamageCollider;
    Animator animator;
    QuickSlotsUI quickSlotsUI;
    PlayerStats playerStats;
    PlayerEffectsManager playerEffectsManager;
    public WeaponItem attackingWeapon;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
        playerStats = GetComponentInParent<PlayerStats>();
        playerEffectsManager = GetComponent<PlayerEffectsManager>();

        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
        {
            if (weaponSlot.isLeftHandSlot)
            {
                leftHandSlot = weaponSlot;
            }
            else if (weaponSlot.isRightHandSlot)
            {
                rightHandSlot = weaponSlot;
            }
        }
    }
    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {

        if (isLeft)
        {
            leftHandSlot.LoadWeaponModel(weaponItem);
            LoadLeftWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(isLeft, weaponItem);
            if (weaponItem != null)
            {
                animator.CrossFade(weaponItem.left_Hand_Idle, 0.2f);
            }
            else
            {
                animator.CrossFade("Left Arm Empty", 0.2f);
            }

        }
        else
        {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(isLeft, weaponItem);

            if (weaponItem != null)
            {
                animator.CrossFade(weaponItem.right_Hand_Idle, 0.2f);
            }
            else
            {
                animator.CrossFade("Right Arm Empty", 0.2f);
            }
        }
    }
    //All these functions are to be used on animation events
    public void DrainStaminaRoll()
    {
        playerStats.TakeStaminaDamage(playerStats.maxStamina * 0.05f);
    }
    public void DrainStaminaBackstep()
    {
        playerStats.TakeStaminaDamage(playerStats.maxStamina * 0.025f);
    }
    public void DrainStaminaLightAttack()
    {
        playerStats.TakeStaminaDamage(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier);

    }
    public void DrainStaminaHeavyAttack()
    {
        playerStats.TakeStaminaDamage(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier);

    }
    public void LoadLeftWeaponDamageCollider()
    {
        leftDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        playerEffectsManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
    }
    public void LoadRightWeaponDamageCollider()
    {
        rightDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        playerEffectsManager.rightWeaponFx = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();

    }
    public void OpenLeftDamageCollider()
    {
        leftDamageCollider.EnableDamageCollider();
    }
    public void OpenRightDamageCollider()
    {
        rightDamageCollider.EnableDamageCollider();
    }
    public void CloseLeftDamageCollider()
    {
        leftDamageCollider.DisableDamageCollider();
    }
    public void CloseRightDamageCollider()
    {
        rightDamageCollider.DisableDamageCollider();
    }
}

