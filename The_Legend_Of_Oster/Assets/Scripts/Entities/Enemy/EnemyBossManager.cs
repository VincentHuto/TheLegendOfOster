using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossManager : MonoBehaviour
{
    public string bossName;
    public UIBossHealthBar bossHealthBar;
    public EnemyStats bossStats;
    //HANDLE SWITCHING PHASE
    //HANDLE SWITCHING ATTACKS

    private void Awake()
    {
        bossHealthBar.SetBossName(bossName);
        bossHealthBar.SetBossMaxHealth(bossStats.maxHealth);
    }

    public void UpdateBossHealthBar(float currentHealth)
    {
        bossHealthBar.SetBossCurrentHealth(currentHealth);
    }

}
