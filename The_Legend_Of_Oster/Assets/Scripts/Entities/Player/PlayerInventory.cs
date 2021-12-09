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

    //This may be the grossest bit of code ive ever wrote,but "It just works" ~ Vince Comaroto 1:44 am 12/8/21
    public void AddToCraftingStack(CraftingItemStack itemToAdd)
    {
        if (craftingItemInventory.Count <= 0)
        {
            craftingItemInventory.Add((CraftingItemStack)itemToAdd.GetCopy());
            itemToAdd.currentSize = 0;
            return;
        }
        else
        {
            for (int i = craftingItemInventory.Count - 1; i >= 0; i--)
            {
                CraftingItemStack craft = craftingItemInventory[i];
                if (!craft.IsFull())
                {
                    if (craft.itemType == itemToAdd.itemType)
                    {
                        if (craft.currentSize + itemToAdd.currentSize > craft.itemType.stacksTo)
                        {
                            int difference = (itemToAdd.currentSize + craft.currentSize) - craft.itemType.stacksTo;
                            craft.currentSize = craft.itemType.stacksTo;
                            CraftingItemStack diffStack = (CraftingItemStack)itemToAdd.GetCopy();
                            diffStack.currentSize = difference;
                            craftingItemInventory.Add(diffStack);
                            return;
                        }
                        else
                        {
                            craft.currentSize += itemToAdd.currentSize;
                            return;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < craftingItemInventory.Count; j++)
                        {
                            if (craftingItemInventory[j].itemType == itemToAdd.itemType)
                            {
                                if (craftingItemInventory[j].currentSize + itemToAdd.currentSize > craftingItemInventory[j].itemType.stacksTo)
                                {
                                    int difference = (itemToAdd.currentSize + craftingItemInventory[j].currentSize) - craftingItemInventory[j].itemType.stacksTo;
                                    craftingItemInventory[j].currentSize = craftingItemInventory[j].itemType.stacksTo;
                                    CraftingItemStack diffStack = (CraftingItemStack)itemToAdd.GetCopy();
                                    diffStack.currentSize = difference;
                                    craftingItemInventory.Add(diffStack);
                                    return;
                                }
                                else
                                {
                                    craftingItemInventory[j].currentSize += itemToAdd.currentSize;
                                    return;
                                }
                            }
                        }
                        craftingItemInventory.Add((CraftingItemStack)itemToAdd.GetCopy());
                        return;
                    }
                }
                else
                {
                    craftingItemInventory.Add((CraftingItemStack)itemToAdd.GetCopy());
                    return;
                }
            }
        }
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
