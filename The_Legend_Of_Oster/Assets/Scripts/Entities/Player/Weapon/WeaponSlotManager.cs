using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    PlayerInventory playerInventory;
    PlayerManager playerManager;

    public WeaponHolderSlot leftHandSlot, rightHandSlot;
    public DamageCollider leftDamageCollider;
    public DamageCollider rightDamageCollider;

    Animator animator;
    QuickSlotsUI quickSlotsUI;
    PlayerStats playerStats;
    PlayerEffectsManager playerEffectsManager;
    public WeaponItem attackingWeapon;
    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        playerInventory = GetComponentInParent<PlayerInventory>();
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

    public void LoadBothWeaponsOnSlots()
    {
        LoadWeaponOnSlot(playerInventory.rightWeapon,false);
        LoadWeaponOnSlot(playerInventory.leftWeapon, true);

    }
    public void LoadWeaponOnSlot(WeaponItemStack weaponItem, bool isLeft)
    {
        if (weaponItem.itemType is WeaponItem)
        {
            WeaponItem weapon = (WeaponItem)weaponItem.itemType;
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
                quickSlotsUI.UpdateWeaponQuickSlotsUI(isLeft, weaponItem);
                if (weaponItem != null)
                {
                    animator.CrossFade(weapon.left_Hand_Idle, 0.2f);
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
                    animator.CrossFade(weapon.right_Hand_Idle, 0.2f);
                }
                else
                {
                    animator.CrossFade("Right Arm Empty", 0.2f);
                }
            }
        }
    }
    //All these functions are to be used on animation events
    public void DrainStaminaJump()
    {
        playerStats.TakeStaminaDamage(playerStats.maxStamina * 0.05f);
    }

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
       // WeaponItem leftWeapon = (WeaponItem)playerInventory.leftWeapon.itemType;
     //   leftDamageCollider.currentWeaponDamage = leftWeapon.baseDamage;
        playerEffectsManager.leftWeaponFX = leftHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();
    }
    public void LoadRightWeaponDamageCollider()
    {
        rightDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        //WeaponItem rightWeapon = (WeaponItem)playerInventory.rightWeapon.itemType;
        //rightDamageCollider.currentWeaponDamage = rightWeapon.baseDamage;
        playerEffectsManager.rightWeaponFx = rightHandSlot.currentWeaponModel.GetComponentInChildren<WeaponFX>();

    }
    public void OpenDamageCollider()
    {
        if (playerManager.isUsingRightHand)
        {
            rightDamageCollider.EnableDamageCollider();
        }
        else if (playerManager.isUsingLeftHand)
        {
            leftDamageCollider.EnableDamageCollider();
        }
    }

    public void CloseDamageCollider()
    {
        if(rightDamageCollider != null)
        {
            rightDamageCollider.DisableDamageCollider();
        }
        if (leftDamageCollider != null)
        {
            leftDamageCollider.DisableDamageCollider();
        }
    }
}

