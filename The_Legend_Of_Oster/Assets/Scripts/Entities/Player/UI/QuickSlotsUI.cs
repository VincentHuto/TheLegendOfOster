using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotsUI : MonoBehaviour
{
    public Image leftWeaponIcon, rightWeaponIcon;

    public Image topSpellIcon;
    public Text topSpellText;
    public Image bottomConsumableIcon;
    public Text bottomConsumableText;

    public void UpdateWeaponQuickSlotsUI(bool isLeft, WeaponItemStack weapon)
    {
        if (weapon != null)
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
    }
    public void UpdateConsumableQuickSlotsUi(ConsumableItemStack consumableItem)
    {
        if (consumableItem != null)
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
    public void UpdateSpellQuickSlotsUi(SpellItemStack spellItem)
    {
        if (spellItem != null)
        {

            if (spellItem.itemType.itemIcon != null)
            {
                topSpellIcon.sprite = spellItem.itemType.itemIcon;
                topSpellIcon.enabled = true;

                topSpellText.text = spellItem.name;
                topSpellText.enabled = true;

            }
            else
            {
                topSpellIcon.sprite = null;
                topSpellIcon.enabled = false;
                topSpellText.enabled = false;
            }
        }
    }

}
