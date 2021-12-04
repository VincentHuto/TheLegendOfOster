using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    PlayerInventory playerInventory;
    WeaponSlotManager weaponSlotManager;
    UIManager uIManager;
    InputHandler inputHandler;
    public Image icon;
    WeaponItem item;

    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        uIManager = FindObjectOfType<UIManager>();
        weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
        inputHandler = FindObjectOfType<InputHandler>();

    }

    public void AddItem(WeaponItem weaponItem)
    {
        item = weaponItem;
        icon.sprite = weaponItem.itemIcon;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        gameObject.SetActive(false);
    }

    public void EquipThisItem()
    {
        if (uIManager.rightHandSlot01Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
            playerInventory.weaponsInRightHandSlots[0] = item;
            playerInventory.weaponsInventory.Remove(item);
        }
        else if (uIManager.rightHandSlot02Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
            playerInventory.weaponsInRightHandSlots[1] = item;
            playerInventory.weaponsInventory.Remove(item);

        }
        else if (uIManager.leftHandSlot01Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
            playerInventory.weaponsInLeftHandSlots[0] = item;
            playerInventory.weaponsInventory.Remove(item);

        }
        else if (uIManager.leftHandSlot02Selected)
        {
            playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
            playerInventory.weaponsInLeftHandSlots[1] = item;
            playerInventory.weaponsInventory.Remove(item);

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
