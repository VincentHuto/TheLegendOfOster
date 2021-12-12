using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemStack/KeyItemStack")]
public class KeyItemStack : ItemStack
{

    public KeyItemStack GetCopy()
    {
        return Instantiate(this);
    }

}

