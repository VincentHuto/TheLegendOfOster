using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;
    public int currentWeaponDamage = 25;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }
    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider colllision)
    {
        if(colllision.tag == "Player")
        {
           PlayerStats playerStats =  colllision.GetComponent<PlayerStats>();
            if(playerStats != null)
            {
                playerStats.TakeDamage(currentWeaponDamage);
            }
        }
        if(colllision.tag == "Enemy")
        {
            EnemyStats enemyStats = colllision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentWeaponDamage);
            }
        }
    }

}
