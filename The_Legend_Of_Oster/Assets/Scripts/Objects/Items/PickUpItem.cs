using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : Interactable
{
    public ItemStack stack;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        PickUp(playerManager);
    }

    public virtual void PickUp(PlayerManager playerManager)
    {

        PlayerInventory playerInventory;
        PlayerLocomotion playerLocomotion;
        PlayerAnimatorManager playerAnimatorManager;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
        playerAnimatorManager = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

        playerLocomotion.rigidbody.velocity = Vector3.zero; //Stops the player from moving whilst picking up item
        playerAnimatorManager.PlayTargetAnimation("Pick Up Item", true); //Plays the animation of looting the item

        if (stack.itemType is WeaponItem)
        {
            playerInventory.weaponsInventory.Add(((WeaponItemStack)stack).GetCopy());

        }
        else if (stack.itemType is KeyItem)
        {
            playerInventory.keyItemsInventory.Add(((KeyItemStack)stack).GetCopy());

        }
        else if (stack.itemType is CraftingItem)
        {
            playerInventory.AddToCraftingStack(((CraftingItemStack)stack).GetCopy());
        }
        else
        {

        }
        playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = stack.itemType.itemName;
        playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = stack.itemType.itemIcon.texture;
        playerManager.itemInteractableGameObject.SetActive(true);
        Destroy(gameObject);
    }

}
