using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemStack/Crafting Item Stack")]
public class CraftingItemStack : ItemStack
{
    public new CraftingItem itemType;
}
