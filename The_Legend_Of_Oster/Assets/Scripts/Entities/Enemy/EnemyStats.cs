using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{

    EnemyAnimatorManager anim;
    public UIEnemyHealthBar enemyHealthBar;
    EnemyBossManager enemyBossManager;

    public int soulsAwardedOnDeath = 50;
    public bool isBoss;

    private void Awake()
    {
        anim = GetComponentInChildren<EnemyAnimatorManager>();
        maxHealth = SetMaxHealthFromLevel();
        maxStamina = SetMaxStaminaFromLevel();
        maxBreath = SetMaxBreathFromLevel();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentBreath = maxBreath;
        enemyBossManager = GetComponent<EnemyBossManager>();


    }

    void Start()
    {
        if (!isBoss)
        {
            enemyHealthBar.SetMaxHealth(maxHealth);
        }
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


    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);


        if (!isBoss)
        {
            enemyHealthBar.SetHealth(currentHealth);
        }
        else if (isBoss && enemyBossManager != null)
        {
            enemyBossManager.UpdateBossHealthBar(currentHealth);
        }


        enemyHealthBar.SetHealth(currentHealth);

        anim.PlayTargetAnimation("Damage_1", true);

        if (currentHealth <= 0)
        {
            anim.PlayTargetAnimation("Death_1", true);
        }
    }

}
