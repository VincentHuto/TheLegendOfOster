using Assets.Scripts.Player.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public float staminaRegenMult;
    public string text;
    public HealthBar healthbar;
    public StaminaBar staminaBar;
    public BreathBar breathBar;
    public LevelText levelText;
    
    PlayerAnimatorManager playerAnimatorManager;
    PlayerManager playerManager;
    public float staminaRegenerationAmount = 1;
    public float staminaRegenTimer = 0;

    private void Awake()
    {
        levelText = GetComponentInChildren<LevelText>();
        maxHealth = SetMaxHealthFromLevel();
        maxStamina = SetMaxStaminaFromLevel();
        maxBreath = SetMaxBreathFromLevel();
        text = SetTextFromLevel();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentBreath = maxBreath;
        staminaRegenMult = level * 0.05f;
        playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        playerManager = GetComponent<PlayerManager>();

    }

    void Start()
    {

        healthbar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
        breathBar.SetMaxBreath(maxBreath);
        levelText.SetText(text);

    }

    private string SetTextFromLevel()
    {
        text = "Level: " + level.ToString();
        return text;
    }

    private float SetMaxHealthFromLevel()
    {
        maxHealth = level * 10;
        return maxHealth;
    }

    private float SetMaxStaminaFromLevel()
    {
        maxStamina = level * 10;
        return maxStamina;
    }

    private float SetMaxBreathFromLevel()
    {
        maxBreath = level * 15;
        return maxBreath;
    }

    public void TakeDamage(float damage)
    {
        if (playerManager.isInvulnerable)
            return;

        if (isDead)
            return;

        currentHealth = currentHealth - damage;

        healthbar.SetCurrentHealth(currentHealth);

        playerAnimatorManager.PlayTargetAnimation("Damage_1", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            playerAnimatorManager.PlayTargetAnimation("Death_1", true);
            isDead = true;
            //HANDLE PLAYER DEATH
        }
    }

    public void HealPlayerHealth(float healAmount)
    {
        currentHealth = currentHealth + healAmount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthbar.SetCurrentHealth(currentHealth);
    }

    public void TakeStaminaDamage(float damage)
    {
        currentStamina = currentStamina - damage;
        staminaBar.SetCurrentStamina(currentStamina);
    }

    public void RegenerateStamina()
    {
        if (playerManager.isInteracting)
        {
            staminaRegenTimer = 0;
        }
        else
        {
            staminaRegenTimer += Time.deltaTime;

            if (currentStamina < maxStamina && staminaRegenTimer > 1f)
            {
                currentStamina += staminaRegenerationAmount * Time.deltaTime;
                staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
            }
        }
    }

    public void HealPlayerStamina(float healAmount)
    {
        currentStamina = currentStamina + healAmount;

        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        staminaBar.SetCurrentStamina(currentStamina);
    }


    public void HealPlayerBreath(float healAmount)
    {
        currentBreath = currentBreath + healAmount;

        if (currentBreath > maxBreath)
        {
            currentBreath = maxBreath;
        }
        breathBar.SetCurrentBreath(currentBreath);
    }

}
