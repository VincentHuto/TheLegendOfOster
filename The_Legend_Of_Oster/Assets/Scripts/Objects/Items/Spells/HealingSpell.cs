using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Healing Spell")]
public class HealingSpell : SpellItem
{
    public float healAmount;
    public override void AttemptToCastSpell(AnimatorManager animatorHandler, PlayerStats playerStats)
    {
        base.AttemptToCastSpell(animatorHandler, playerStats);
        GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, animatorHandler.transform);
        animatorHandler.PlayTargetAnimation(spellAnimation, false);
    }

    public override void SuccessfullyCastSpell(AnimatorManager animatorHandler, PlayerStats playerStats)
    {
        base.SuccessfullyCastSpell(animatorHandler, playerStats);
        GameObject instantiatedSpellFX = Instantiate(spellCastFX, animatorHandler.transform);
        playerStats.HealPlayerHealth(healAmount);
    }
}