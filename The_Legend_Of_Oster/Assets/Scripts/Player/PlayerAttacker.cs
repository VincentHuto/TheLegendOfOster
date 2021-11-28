using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{

    AnimatorHandler animatorHandler;


    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        if (weapon != null && !weapon.isUnarmed)
        {
            animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
        }
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (weapon != null && !weapon.isUnarmed)
        {
            animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
        }

    }
}
