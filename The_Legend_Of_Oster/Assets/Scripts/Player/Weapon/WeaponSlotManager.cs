using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot, rightHandSlot;
    DamageCollider leftDamageCollider, rightDamageCollider;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();

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

            if(weaponItem != null)
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

