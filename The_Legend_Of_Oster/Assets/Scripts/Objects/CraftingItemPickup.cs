using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingItemPickup : PickUpItem
{
   /* public override void PickUp(PlayerManager playerManager)
    {
        base.PickUp(playerManager);
        playerInventory.craftingItemInventory.Add((CraftingItem)item);
        playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = item.itemName;
        playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture;
        playerManager.itemInteractableGameObject.SetActive(true);
        Destroy(gameObject);

    }*/
}
