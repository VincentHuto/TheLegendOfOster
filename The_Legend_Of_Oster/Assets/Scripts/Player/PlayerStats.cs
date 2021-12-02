using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 10;
    public float maxHealth,currentHealth;
    public float maxStamina, currentStamina;
    public float staminaRegenMult;
    public float maxBreath, currentBreath;

    public HealthBar healthbar;
    public StaminaBar staminaBar;
    public BreathBar breathBar;

    AnimatorHandler animatorHandler;

    private WaitForSeconds regenTicks = new WaitForSeconds(0.1f);
    private Coroutine regen;

    private void Awake()
    {
        maxHealth = SetMaxHealthFromLevel();
        maxStamina = SetMaxStaminaFromLevel();
        maxBreath = SetMaxBreathFromLevel();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentBreath = maxBreath;
        staminaRegenMult = level * 0.05f;
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    void Start()
    {

        healthbar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
        breathBar.SetMaxBreath(maxBreath);

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

    public void TakeDamage(float damage)
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

    public void TakeStaminaDamage(float damage)
    {
        if (regen != null)
        {
            StopCoroutine(regen);
        }
        regen = StartCoroutine(StaminaRegen());

        currentStamina = currentStamina - damage;
        staminaBar.SetCurrentStamina(currentStamina);
    }

    private IEnumerator StaminaRegen()
    {
        yield return new WaitForSeconds(2);

        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 50 * staminaRegenMult;
            staminaBar.SetCurrentStamina(currentStamina);
            yield return regenTicks;
        }

        regen = null;
    }
}
