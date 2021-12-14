using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeRowInputController : MonoBehaviour
{
    RecipeRow recipeRow;
    RecipeInputInventorySlot[] recipeInputSlots;
    public GameObject inputSlotPrefab;
    public Transform inputSlotsParent;
    private void Start()
    {
        recipeRow = GetComponentInParent<RecipeRow>();
        recipeInputSlots = GetComponentsInChildren<RecipeInputInventorySlot>();
        if (recipeRow.recipe != null)
        {
            BlenderizerRecipe recipe = recipeRow.recipe;
            for (int i = 0; i < recipeInputSlots.Length; i++)
            {
                if (i < recipe.neededItems.Count)
                {
                    if (recipeInputSlots.Length < recipe.neededItems.Count)
                    {
                        Instantiate(inputSlotPrefab, inputSlotsParent);
                        recipeInputSlots = GetComponentsInChildren<RecipeInputInventorySlot>();
                    }
                     recipeInputSlots[i].AddItem(recipe.neededItems[i]);
                     recipeInputSlots[i].count.text = recipe.neededItems[i].currentSize.ToString();
                }
            }
        }
    }
  
}