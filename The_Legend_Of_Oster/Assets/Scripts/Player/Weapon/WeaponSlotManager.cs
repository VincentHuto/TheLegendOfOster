using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot, rightHandSlot;
    DamageCollider leftDamageCollider, rightDamageCollider;

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
    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.LoadWeaponModel(weaponItem);
            LoadLeftWeaponDamageCollider();
        }
        else
        {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();
        }
    }
    public void LoadLeftWeaponDamageCollider()
    {
        leftDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }
    public void LoadRightWeaponDamageCollider()
    {
        rightDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
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

