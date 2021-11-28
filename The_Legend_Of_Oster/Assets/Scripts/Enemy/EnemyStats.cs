using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int level = 10;
    public int maxHealth, currentHealth;
    public int maxStamina, currentStamina;
    public int maxBreath, currentBreath;


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
        Debug.Log("I SHOULD BE TAKING DAMAGE: " + currentHealth);
        
        anim.Play("Damage_1");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            anim.Play("Death_1");
            //HANDLE ENEMY DEATH
        }
    }
}
