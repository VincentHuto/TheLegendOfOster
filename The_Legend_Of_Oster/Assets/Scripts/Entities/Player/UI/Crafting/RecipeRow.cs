using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeRow : MonoBehaviour
{
    public BlenderizerRecipe recipe;
   public Image icon;

    private void Start()
    {
        icon.gameObject.SetActive(false);
        if (recipe.outputItemIcon != null)
        {
            icon.sprite = recipe.outputItemIcon;
            icon.gameObject.SetActive(true);
        }
    }

    public void ClickRecipeButton()
    {
        Debug.Log("CLICKED RECIPE FOR: " + recipe.outputStack.name);
    }
}
