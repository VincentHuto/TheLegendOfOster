using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;

    public WeaponItemStack rightWeapon, leftWeapon, unarmedWeapon;
    public WeaponItemStack[] weaponsInRightHandSlots = new WeaponItemStack[1];
    public WeaponItemStack[] weaponsInLeftHandSlots = new WeaponItemStack[1];

    public int currentRightWeaponIndex = 0;
    public int currentLeftWeaponIndex = 0;

    public List<WeaponItemStack> weaponsInventory;
    public List<KeyItemStack> keyItemsInventory;
    public List<CraftingItemStack> craftingItemInventory;

    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start()
    {
        rightWeapon = weaponsInRightHandSlots[0];
        leftWeapon = weaponsInLeftHandSlots[0];
        weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);

    }

    public void AddToCraftingStack(CraftingItemStack itemToAdd)
    {

        if (craftingItemInventory.Count <= 0)
        {
            craftingItemInventory.Add((CraftingItemStack)itemToAdd.GetCopy());
        }
        else
        {

            for (int i = craftingItemInventory.Count - 1; i >= 0; i--)
            {
                CraftingItemStack craft = craftingItemInventory[i];
                if (craft.itemType == itemToAdd.itemType && itemToAdd.currentSize < craft.itemType.stacksTo)
                {
                    craft.currentSize += itemToAdd.currentSize;
                }
                else
                {
                    craftingItemInventory.Add((CraftingItemStack)itemToAdd.GetCopy());
                }
            }
        }

        /*   foreach (CraftingItemStack i in craftingItemInventory)
           {
               if (i.itemType == itemToAdd && itemToAdd.currentSize < i.itemType.stacksTo)
               {
                   Debug.Log("INCREMENTING STACK");
                   i.currentSize += itemToAdd.currentSize;
               }
               else
               {
                   Debug.Log("CREATING NEW STACK");
                   craftingItemInventory.Add(itemToAdd);
               }
           }*/
    }



    public void ChangeRightWeapon()
    {
        currentRightWeaponIndex = currentRightWeaponIndex + 1;

        if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null)
        {
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
        }
        else if (currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null)
        {
            currentRightWeaponIndex = currentRightWeaponIndex + 1;
        }

        else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null)
        {
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
        }
        else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] == null)
        {
            currentRightWeaponIndex = currentRightWeaponIndex + 1;
        }

        if (currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
        {
            currentRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
        }
    }

    public void ChangeLeftWeapon()
    {
        currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

        if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null)
        {
            leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
        }
        else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] == null)
        {
            currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
        }

        else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] != null)
        {
            leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
        }
        else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] == null)
        {
            currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
        }

        if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
        {
            currentLeftWeaponIndex = -1;
            leftWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
        }
    }
}
