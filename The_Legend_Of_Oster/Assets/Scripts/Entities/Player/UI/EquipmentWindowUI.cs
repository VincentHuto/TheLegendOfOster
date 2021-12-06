using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWindowUI : MonoBehaviour
{
    public bool rightHandSlot01Selected, rightHandSlot02Selected;
    public bool leftHandSlot01Selected, leftHandSlot02Selected;
    public HandEquipmentSlotUI[] handEquipmentSlotUI;


    public void LoadWeaponsOnEquipmentScreen(PlayerInventory playerInventory)
    {
        for(int i = 0; i < handEquipmentSlotUI.Length; i++)
        {
            if (handEquipmentSlotUI[i].rightHandSlot01)
            {
                handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);

            }else if (handEquipmentSlotUI[i].rightHandSlot02)
            {
                handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
            }
            else if (handEquipmentSlotUI[i].leftHandSlot01)
            {
                handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
            }
            else if (handEquipmentSlotUI[i].leftHandSlot02)
            {
                handEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
            }

        }
    }

    public void SelectRightHandSlot01()
    {
        rightHandSlot01Selected = true;
    }
    public void SelectRightHandSlot02()
    {
        rightHandSlot02Selected = true;
    }
    public void SelectLeftHandSlot01()
    {
        leftHandSlot01Selected = true;
    }
    public void SelectLeftHandSlot02()
    {
        leftHandSlot02Selected = true;
    }
}
