using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemStack/Weapon Item Stack")]
public class WeaponItemStack : ItemStack
{
    public WeaponItemStack GetCopy()
    {
        return Instantiate(this);
    }

}
