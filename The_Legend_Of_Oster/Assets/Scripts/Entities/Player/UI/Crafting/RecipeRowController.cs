using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeRowController : MonoBehaviour
{
    public List<BlenderizerRecipe> knownRecipes;
    RecipeRow[] recipeRowSlots;
    public GameObject inputSlotPrefab;
    public Transform inputSlotsParent;
    private void Start()
    {
        recipeRowSlots = GetComponentsInChildren<RecipeRow>();
        for (int i = 0; i < recipeRowSlots.Length; i++)
        {
            if (i < knownRecipes.Count)
            {
                if (recipeRowSlots.Length < knownRecipes.Count)
                {
                    Instantiate(inputSlotPrefab, inputSlotsParent);
                    recipeRowSlots = GetComponentsInChildren<RecipeRow>();
                }
                recipeRowSlots[i].recipe = knownRecipes[i];
            }
        }
    }
}
