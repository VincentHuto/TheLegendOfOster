using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 10;
    public int maxHealth,currentHealth;
    public int maxStamina, currentStamina;
    public int maxBreath, currentBreath;

    public HealthBar healthbar;
    public StaminaBar staminaBar;
    public BreathBar breathBar;

    AnimatorHandler animatorHandler;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    void Start()
    {
        maxHealth = SetMaxHealthFromLevel();
        maxStamina = SetMaxStaminaFromLevel();
        maxBreath = SetMaxBreathFromLevel();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentBreath = maxBreath;
        healthbar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
        breathBar.SetMaxBreath(maxBreath);

    }

    private int SetMaxHealthFromLevel()
    {
        maxHealth = level * 10;
        return maxHealth;
    }

    private int SetMaxStaminaFromLevel()
    {
        maxStamina = level * 5;
        return maxStamina;
    }

    private int SetMaxBreathFromLevel()
    {
        maxBreath = level * 15;
        return maxBreath;
    }


    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;

        healthbar.SetCurrentHealth(currentHealth);

        animatorHandler.PlayTargetAnimation("Damage_1", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animatorHandler.PlayTargetAnimation("Death_1", true);
            //HANDLE PLAYER DEATH
        }
    }
}
