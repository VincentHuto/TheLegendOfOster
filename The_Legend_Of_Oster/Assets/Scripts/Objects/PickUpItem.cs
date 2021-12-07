using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : Interactable
{
    public Item item;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        PickUp(playerManager);
    }

    public virtual void PickUp(PlayerManager playerManager)
    {

        PlayerInventory playerInventory;
        PlayerLocomotion playerLocomotion;
        AnimatorHandler animatorHandler;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
        animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();

        playerLocomotion.rigidbody.velocity = Vector3.zero; //Stops the player from moving whilst picking up item
        animatorHandler.PlayTargetAnimation("Pick Up Item", true); //Plays the animation of looting the item

        if (item is WeaponItem)
        {
            playerInventory.weaponsInventory.Add((WeaponItem)item);

        }
        else if (item is KeyItem)
        {
            playerInventory.keyItemsInventory.Add((KeyItem)item);

        }
        else if (item is CraftingItem)
        {
            playerInventory.craftingItemInventory.Add((CraftingItem)item);
        }
        else
        {

        }
        playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = item.itemName;
        playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture;
        playerManager.itemInteractableGameObject.SetActive(true);
        Destroy(gameObject);
    }
}
