using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : ItemStack
{
    [Header("Item Model")]
    public GameObject itemModel;

    [Header("Item Animations")]
    public string consumeAnimation;
    public bool isInteracting;


    public virtual void AtteptToConsumeItem(PlayerAnimatorManager playerAnimatorManager)
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
