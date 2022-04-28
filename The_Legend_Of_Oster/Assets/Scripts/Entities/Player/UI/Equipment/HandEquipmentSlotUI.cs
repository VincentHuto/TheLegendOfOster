using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandEquipmentSlotUI : MonoBehaviour
{
    UIManager uIManager;
    public Image icon;
    WeaponItemStack weapon;

    public bool rightHandSlot01, rightHandSlot02;
    public bool leftHandSlot01, leftHandSlot02;

    private void Awake()
    {
        uIManager = FindObjectOfType<UIManager>();
    }

    public void AddItem(WeaponItemStack weaponItem)
    {
        weapon = weaponItem;
        icon.sprite = weaponItem.itemType.itemIcon;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        weapon = null;
        icon.sprite = null;
        icon.enabled = false;
        gameObject.SetActive(false);
    }


    public void SelectThisSlot()
    {
        if (rightHandSlot01)
        {
            uIManager.rightHandSlot01Selected = true;
        }
        else if (rightHandSlot02)

        {
            uIManager.rightHandSlot02Selected = true;

        }
        else if (leftHandSlot01)

        {
            uIManager.leftHandSlot01Selected = true;

        }
        else 
        {
            uIManager.leftHandSlot02Selected = true;

        }
    }
}
