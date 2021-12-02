using Assets.Scripts.Player.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 10;
    public float maxHealth, currentHealth;
    public float maxStamina, currentStamina;
    public float staminaRegenMult;
    public float maxBreath, currentBreath;
    public string text;
    public HealthBar healthbar;
    public StaminaBar staminaBar;
    public BreathBar breathBar;
    public LevelText levelText;

    AnimatorHandler animatorHandler;
    PlayerManager playerManager;
    private WaitForSeconds regenTicks = new WaitForSeconds(0.1f);
    private Coroutine regen;

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
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
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
        BeginStaminaRegen();

        currentStamina = currentStamina - damage;
        staminaBar.SetCurrentStamina(currentStamina);
    }

    public void BeginStaminaRegen()
    {
        if (regen != null)
        {
            StopCoroutine(regen);
        }
        regen = StartCoroutine(StaminaRegen());
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
