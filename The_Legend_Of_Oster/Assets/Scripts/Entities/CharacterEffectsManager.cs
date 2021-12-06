using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    public WeaponFX rightWeaponFx, leftWeaponFX;

    [Header("Poison")]
    public bool isPoisoned;
    public float poisonBuildup = 0;
    public float poisonAmount = 100;
    public float defaultPoisonAmount;
        
    protected virtual void Awake()
    {

    }

    public virtual void PlayWeaponFX(bool isLeft)
    {
        if(isLeft == false)
        {
            if(rightWeaponFx != null)
            {
                rightWeaponFx.PlayWeaponFX(); 
            }
        }
        else
        {
            if(leftWeaponFX != null)
            {
                leftWeaponFX.PlayWeaponFX();
            }
        }
    }

    protected virtual void HandlePoisonBuildup()
    {
        if (isPoisoned)
        {
            return;
        }
        if(poisonBuildup > 0 && poisonBuildup < poisonAmount)
        {
            poisonBuildup = poisonBuildup - 1 * Time.deltaTime;
        }
        else if(poisonBuildup >= poisonAmount)
        {
            poisonBuildup = 0;
        }
    }

    protected virtual void HandleIsPosionedEffect()
    {
        if (isPoisoned)
        {
            if (poisonAmount > 0)
            {
                //Damage over time
                poisonAmount = poisonAmount - 1 * Time.deltaTime;
            }
            else
            {
                isPoisoned = false;
                poisonAmount = defaultPoisonAmount;
            }
        }
    }
}
