using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyWeaponSlotManager : MonoBehaviour
{
    public WeaponItemStack rightHandWeapon;
    public WeaponItemStack leftHandWeapon;

    WeaponHolderSlot rightHandSlot;
    WeaponHolderSlot leftHandSlot;

    DamageCollider leftHandDamageCollider;
    DamageCollider rightHandDamageCollider;

    private void Awake()
    {
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

    private void Start()
    {
        LoadWeaponsOnBothHands();
    }

    public void LoadWeaponOnSlot(WeaponItemStack weapon, bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.currentWeapon = weapon;
            leftHandSlot.LoadWeaponModel(weapon);
            LoadWeaponsDamageCollider(true);
        }
        else
        {
            rightHandSlot.currentWeapon = weapon;
            rightHandSlot.LoadWeaponModel(weapon);
            LoadWeaponsDamageCollider(false);
        }
    }

    public void LoadWeaponsOnBothHands()
    {
        if (rightHandWeapon != null)
        {
            LoadWeaponOnSlot(rightHandWeapon, false);
        }
        if (leftHandWeapon != null)
        {
            LoadWeaponOnSlot(leftHandWeapon, true);
        }
    }

    public void LoadWeaponsDamageCollider(bool isLeft)
    {
        if (isLeft)
        {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }
        else
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }
    }

    public void OpenDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void DrainStaminaLightAttack()
    {

    }

    public void DrainStaminaHeavyAttack()
    {

    }

    public void EnableCombo()
    {
        //anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        //anim.SetBool("canDoCombo", false);
    }
}
