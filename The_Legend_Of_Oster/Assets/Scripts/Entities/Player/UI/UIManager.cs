using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public PlayerInventory playerInventory;
    public EquipmentWindowUI equipmentWindowUI;

    [Header("UI Windows")]
    public GameObject selectWindow, hudWindow, weaponInventoryWindow, equipmentWindow, keyItemInventoryWindow;

    [Header("Equipment Window Slot Selected")]
    public bool rightHandSlot01Selected;
    public bool rightHandSlot02Selected;
    public bool leftHandSlot01Selected, leftHandSlot02Selected;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPrefab;
    public GameObject keyItemInventorySlotPrefab;
    public Transform weaponInventorySlotsParent;
    public Transform keyItemInventorySlotsParent;
    WeaponInventorySlot[] weaponInventorySlots;
    KeyItemInventorySlot[] keyItemInventorySlots;


    private void Start()
    {
        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
        keyItemInventorySlots = keyItemInventorySlotsParent.GetComponentsInChildren<KeyItemInventorySlot>();
        equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
    }

    public void UpdateUI()
    {
        //Update Weapon inventory
        for (int i = 0; i < weaponInventorySlots.Length; i++)
        {
            if (i < playerInventory.weaponsInventory.Count)
            {
                if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count)
                {
                    Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent);
                    weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                }
                weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
            }
            else
            {
                weaponInventorySlots[i].ClearInventorySlot();
            }
        }
        //Update Key inventory
        for (int i = 0; i < keyItemInventorySlots.Length; i++)
        {
            if (i < playerInventory.keyItemsInventory.Count)
            {
                if (keyItemInventorySlots.Length < playerInventory.keyItemsInventory.Count)
                {
                    Instantiate(keyItemInventorySlotPrefab, keyItemInventorySlotsParent);
                    keyItemInventorySlots = keyItemInventorySlotsParent.GetComponentsInChildren<KeyItemInventorySlot>();
                }
                keyItemInventorySlots[i].AddItem(playerInventory.keyItemsInventory[i]);
            }
            else
            {
                keyItemInventorySlots[i].ClearInventorySlot();
            }
        }
    }

    public void OpenSelectWindow()
    {
        selectWindow.SetActive(true);
    }
    public void CloseSelectWindow()
    {
        selectWindow.SetActive(false);
    }

    public void CloseAllInventoryWindows()
    {
        ResetAllSelectedSlots();
        weaponInventoryWindow.SetActive(false);
        equipmentWindow.SetActive(false);
        keyItemInventoryWindow.SetActive(false);
    }

    public void ResetAllSelectedSlots()
    {
        rightHandSlot01Selected = false;
        rightHandSlot02Selected = false;
        leftHandSlot01Selected = false;
        leftHandSlot02Selected = false;
    }
}
