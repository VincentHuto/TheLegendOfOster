using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Consumables/Flask")]
public class FlaskItem : ConsumableItem
{
    [Header("Flask type")]
    public bool healthFlask, staminaFlask, breathFlask;
    public int healthAmount, staminaAmount, breathAmount;
    [Header("Recovery FX")]
    public GameObject recoveryFX;

    public override void AtteptToConsumeItem(PlayerAnimatorManager playerAnimatorManager, WeaponSlotManager weaponSlotManager, PlayerEffectsManager playerEffectsManager)
    {
        base.AtteptToConsumeItem(playerAnimatorManager, weaponSlotManager, playerEffectsManager);
        GameObject flask = Instantiate(itemModel, weaponSlotManager.leftHandSlot.transform);
        playerEffectsManager.currentParticleFX = recoveryFX;
        playerEffectsManager.healAmount = healthAmount;
        playerEffectsManager.instantiatedFXModel = flask;
        weaponSlotManager.leftHandSlot.UnloadWeapon();
    }

}
