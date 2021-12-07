using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : InventorySlot
{
    WeaponSlotManager weaponSlotManager;
    public override void Awake()
    {
        base.Awake();
        weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
    }

 

    public void EquipThisItem()
    {
        if (uIManager.rightHandSlot01Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
            playerInventory.weaponsInRightHandSlots[0] = (WeaponItemStack)item;
            playerInventory.weaponsInventory.Remove((WeaponItemStack)item);
        }
        else if (uIManager.rightHandSlot02Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
            playerInventory.weaponsInRightHandSlots[1] = (WeaponItemStack)item;
            playerInventory.weaponsInventory.Remove((WeaponItemStack)item);

        }
        else if (uIManager.leftHandSlot01Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
            playerInventory.weaponsInLeftHandSlots[0] = (WeaponItemStack)item;
            playerInventory.weaponsInventory.Remove((WeaponItemStack)item);

        }
        else if (uIManager.leftHandSlot02Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
            playerInventory.weaponsInLeftHandSlots[1] = (WeaponItemStack)item;
            playerInventory.weaponsInventory.Remove((WeaponItemStack)item);

        }
        else
        {
            return;
        }
        playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];
        playerInventory.leftWeapon = playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex];
        weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
        weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
        uIManager.equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
        uIManager.ResetAllSelectedSlots();
        uIManager.hudWindow.SetActive(true);
        inputHandler.invFlag = false;


    }
}
