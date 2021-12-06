using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;
    //Make sure to set this within the prefab so it propagates to all instances of this weapon
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
        if (colllision.tag == "Player")
        {
            PlayerStats playerStats = colllision.GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(currentWeaponDamage);
            }
        }
        //TAG THE COLLIDER AS ENEMY NOT THE PREFAB, SAME AS PLAYER
        if (colllision.tag == "Enemy")
        {
            EnemyStats enemyStats = colllision.GetComponentInParent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentWeaponDamage);
            }
        }
    }

}
