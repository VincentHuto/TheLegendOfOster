using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemStack : ItemStack
{
    [Header("Item Model")]
    public GameObject itemModel;

    [Header("Item Animations")]
    public string consumeAnimation;
    public bool isInteracting;


    public virtual void AtteptToConsumeItem(AnimatorManager playerAnimatorManager,WeaponSlotManager weaponSlotManager, PlayerEffectsManager playerEffectsManager)
    {
        if (currentSize > 0)
        {
            playerAnimatorManager.PlayTargetAnimation(consumeAnimation, isInteracting);
        }
        else
        {
            playerAnimatorManager.PlayTargetAnimation("Shrug", true);

        }

    }
}
