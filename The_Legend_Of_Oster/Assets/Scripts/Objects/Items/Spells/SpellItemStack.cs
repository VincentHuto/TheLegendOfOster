using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Spells/Spell Item Stack")]
public class SpellItemStack : ItemStack
{
   public bool isEmpty;

    public SpellItem getSpell()
    {
        return (SpellItem)itemType;

    }

    public SpellItemStack GetCopy()
    {
        return Instantiate(this);
    }
}
