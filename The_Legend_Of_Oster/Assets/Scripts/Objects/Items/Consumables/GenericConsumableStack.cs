using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Generic")]
public class GenericConsumableStack : ConsumableItemStack
{
    public override void AtteptToConsumeItem(AnimatorManager playerAnimatorManager, WeaponSlotManager weaponSlotManager, PlayerEffectsManager playerEffectsManager)
    {
        base.AtteptToConsumeItem(playerAnimatorManager, weaponSlotManager, playerEffectsManager);

    }
}