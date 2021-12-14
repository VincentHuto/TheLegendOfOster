using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public PlayerInventory playerInventory;
    public EquipmentWindowUI equipmentWindowUI;

    [Header("UI Windows")]
    public GameObject selectWindow, hudWindow, weaponInventoryWindow, equipmentWindow,
        keyItemInventoryWindow, craftingItemInventoryWindow, descriptionWindow, blenderWindow;

    public bool IsBlenderOpen = false;

    [Header("Equipment Window Slot Selected")]
    public bool rightHandSlot01Selected, rightHandSlot02Selected;
    public bool leftHandSlot01Selected, leftHandSlot02Selected;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPrefab;
    public GameObject keyItemInventorySlotPrefab;
    public GameObject craftingInventorySlotPrefab;
    public GameObject recipeSlotPrefab;

    public Transform weaponInventorySlotsParent;
    public Transform keyItemInventorySlotsParent;
    public Transform craftingItemInventorySlotsParent;
    WeaponInventorySlot[] weaponInventorySlots;
    KeyItemInventorySlot[] keyItemInventorySlots;
    CraftingItemInventorySlot[] craftingItemInventorySlots;


    private void Start()
    {
        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
        keyItemInventorySlots = keyItemInventorySlotsParent.GetComponentsInChildren<KeyItemInventorySlot>();
        craftingItemInventorySlots = craftingItemInventorySlotsParent.GetComponentsInChildren<CraftingItemInventorySlot>();
        equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
        blenderWindow.SetActive(false);
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
                weaponInventorySlots[i].count.text = playerInventory.weaponsInventory[i].currentSize.ToString();
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

                keyItemInventorySlots[i].count.gameObject.SetActive(true);
                keyItemInventorySlots[i].count.text = playerInventory.keyItemsInventory[i].currentSize.ToString();

            }
            else
            {
                keyItemInventorySlots[i].ClearInventorySlot();
            }
        }
        //Update Crafting inventory
        for (int i = 0; i < craftingItemInventorySlots.Length; i++)
        {

            if (i < playerInventory.craftingItemInventory.Count)
            {
                if (craftingItemInventorySlots.Length < playerInventory.craftingItemInventory.Count)
                {
                    Instantiate(craftingInventorySlotPrefab, craftingItemInventorySlotsParent);
                    craftingItemInventorySlots = craftingItemInventorySlotsParent.GetComponentsInChildren<CraftingItemInventorySlot>();
                }

                craftingItemInventorySlots[i].AddItem(playerInventory.craftingItemInventory[i].GetCopy());
                craftingItemInventorySlots[i].count.text = playerInventory.craftingItemInventory[i].currentSize.ToString();
            }
            else
            {
                craftingItemInventorySlots[i].ClearInventorySlot();
            }
        }
    }

    public void OpenSelectWindow()
    {
        if (!IsBlenderOpen)
            selectWindow.SetActive(true);
    }
    public void CloseSelectWindow()
    {
        if (!IsBlenderOpen)
            selectWindow.SetActive(false);
    }


    public void OpenBlenderWindow()
    {
        UpdateUI();
        IsBlenderOpen = true;
        blenderWindow.SetActive(true);
        craftingItemInventoryWindow.SetActive(true);
        hudWindow.SetActive(false);
    }
    public void CloseBlenderWindow()
    {
        UpdateUI();
        IsBlenderOpen = false;
        blenderWindow.SetActive(false);
        craftingItemInventoryWindow.SetActive(false);
        hudWindow.SetActive(true);
    }

    public void CloseAllInventoryWindows()
    {
        ResetAllSelectedSlots();
        weaponInventoryWindow.SetActive(false);
        equipmentWindow.SetActive(false);
        keyItemInventoryWindow.SetActive(false);
        craftingItemInventoryWindow.SetActive(false);
        descriptionWindow.SetActive(false);
        blenderWindow.SetActive(false);
        IsBlenderOpen = false;

    }

    public void ResetAllSelectedSlots()
    {
        rightHandSlot01Selected = false;
        rightHandSlot02Selected = false;
        leftHandSlot01Selected = false;
        leftHandSlot02Selected = false;
    }
}
