using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Healing Spell")]
public class HealingSpell : SpellItem
{
    public float healAmount;
    public override void AttemptToCastSpell(AnimatorManager animatorHandler, PlayerStats playerStats, WeaponSlotManager weaponSlotManager,
            bool isLeftHanded)
    {
        base.AttemptToCastSpell(animatorHandler, playerStats, weaponSlotManager, isLeftHanded);
        GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, animatorHandler.transform);
        animatorHandler.PlayTargetAnimation(spellAnimation, false);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorHandler, PlayerStats playerStats,CameraHandler cameraHandler, WeaponSlotManager weaponSlotManager,
            bool isLeftHanded)
    {
        base.SuccessfullyCastSpell(animatorHandler, playerStats, cameraHandler, weaponSlotManager, isLeftHanded);
        GameObject instantiatedSpellFX = Instantiate(spellCastFX, animatorHandler.transform);
        playerStats.HealPlayerHealth(healAmount);
    }
}