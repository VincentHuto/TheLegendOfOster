using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlaskItem : ConsumableItem
{
    [Header("Flask type")]
    public bool healthFlask, staminaFlask, breathFlask;
    public int healthAmount, staminaAmount, breathAmount;
    [Header("Recovery FX")]
    public GameObject recoveryFX;

    public override void AtteptToConsumeItem(PlayerAnimatorManager playerAnimatorManager)
    {

    }

}
