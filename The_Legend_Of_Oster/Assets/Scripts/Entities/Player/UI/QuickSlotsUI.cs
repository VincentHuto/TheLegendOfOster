using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotsUI : MonoBehaviour
{
    public Image leftWeaponIcon, rightWeaponIcon;
    public Image bottomConsumableIcon;
    public Text bottomConsumableText;

    public void UpdateWeaponQuickSlotsUI(bool isLeft, WeaponItemStack weapon)
    {
        if (isLeft == false)
        {
            if (weapon.itemType.itemIcon != null)
            {
                rightWeaponIcon.sprite = weapon.itemType.itemIcon;
                rightWeaponIcon.enabled = true;
            }
            else
            {
                rightWeaponIcon.sprite = null;
                rightWeaponIcon.enabled = false;
            }
        }
        else
        {
            if (weapon.itemType.itemIcon != null)
            {
                leftWeaponIcon.sprite = weapon.itemType.itemIcon;
                leftWeaponIcon.enabled = true;
            }
            else
            {
                leftWeaponIcon.sprite = null;
                leftWeaponIcon.enabled = false;
            }
        }
    }

    public void UpdateConsumableQuickSlotsUi(ConsumableItemStack consumableItem)
    {
        if (consumableItem.itemType.itemIcon != null)
        {
            bottomConsumableIcon.sprite = consumableItem.itemType.itemIcon;
            bottomConsumableIcon.enabled = true;

            bottomConsumableText.text = consumableItem.name;
            bottomConsumableText.enabled = true;

        }
        else
        {
            bottomConsumableIcon.sprite = null;
            bottomConsumableIcon.enabled = false;
            bottomConsumableText.enabled = false;
        }
    }
}
