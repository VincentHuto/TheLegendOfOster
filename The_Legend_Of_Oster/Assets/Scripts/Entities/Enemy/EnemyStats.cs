using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{

    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        maxHealth = SetMaxHealthFromLevel();
        maxStamina = SetMaxStaminaFromLevel();
        maxBreath = SetMaxBreathFromLevel();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentBreath = maxBreath;
 

    }

    private float SetMaxHealthFromLevel()
    {
        maxHealth = level * 10;
        return maxHealth;
    }

    private float SetMaxStaminaFromLevel()
    {
        maxStamina = level * 5;
        return maxStamina;
    }

    private float SetMaxBreathFromLevel()
    {
        maxBreath = level * 15;
        return maxBreath;
    }


    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        
        anim.Play("Damage_1");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            anim.Play("Death_1");
            //HANDLE ENEMY DEATH
        }
    }
}
