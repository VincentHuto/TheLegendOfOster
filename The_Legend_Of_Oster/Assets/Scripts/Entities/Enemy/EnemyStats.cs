using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{

    EnemyAnimatorManager anim;
    public UIEnemyHealthBar enemyHealthBar;
    private void Awake()
    {
        anim = GetComponentInChildren<EnemyAnimatorManager>();
    }

    void Start()
    {
        maxHealth = SetMaxHealthFromLevel();
        maxStamina = SetMaxStaminaFromLevel();
        maxBreath = SetMaxBreathFromLevel();
        currentHealth = maxHealth;
        enemyHealthBar.SetMaxHealth(maxHealth);
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
        enemyHealthBar.SetHealth(currentHealth);

        anim.PlayTargetAnimation("Damage_1",true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            anim.PlayTargetAnimation("Death_1", true);
            //HANDLE ENEMY DEATH
        }
    }
}
