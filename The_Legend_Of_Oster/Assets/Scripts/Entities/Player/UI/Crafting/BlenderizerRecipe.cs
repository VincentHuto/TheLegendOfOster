using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Blenderizer/Recipe")]
public class BlenderizerRecipe : ScriptableObject
{
    public List<CraftingItemStack> neededItems;
    public ItemStack outputStack;
    public Sprite outputItemIcon;


    public int GetTotalItemsNeeded()
    {
        if(neededItems.Count > 0 && neededItems.Count <= 1)
        {
            return 1;
        }
        else
        {
            int totalItems = 0;
            for (int i = 0; i < neededItems.Count; i++)
            {
                Debug.Log(outputStack.itemType.name + " " + neededItems[i]);
                totalItems++;
            }
            return totalItems;
        }
    }

}
