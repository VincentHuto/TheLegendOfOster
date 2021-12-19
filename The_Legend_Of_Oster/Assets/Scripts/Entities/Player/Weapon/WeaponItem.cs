using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("Weapon Type")]
    public bool isSpellCaster;
    public bool isFaithCaster;
    public bool isPyroCaster;
    public bool isMeleeWeapon;


    [Header("Stamina Costs")]
    public float baseStamina;
    public float lightAttackMultiplier, heavyAttackMultiplier;

    [Header("Idle Animations")]
    public string right_Hand_Idle, left_Hand_Idle;

    [Header("One Handed Attack Animations")]
    public string OH_Right_Light_Attack_1, OH_Right_Light_Attack_2;
    public string OH_Left_Light_Attack_1, OH_Left_Light_Attack_2;

    public string OH_Right_Heavy_Attack_1;
    public string OH_Left_Heavy_Attack_1;

    public float GetWeaponStaminaCost(bool isHeavy)
    {
        if (isHeavy)
        {
            return baseStamina * heavyAttackMultiplier;
        }
        else
        {
            return baseStamina * lightAttackMultiplier;
        }
    }


}
