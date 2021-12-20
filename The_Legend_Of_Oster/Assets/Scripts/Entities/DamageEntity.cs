using System.Collections;
using UnityEngine;

public class DamageEntity : MonoBehaviour
{
    public int damage = 25;

    private void OnTriggerEnter(Collider other)
    {
        CharacterStats enemyStats = other.GetComponentInParent<CharacterStats>();

        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage);
        }
    }
}
