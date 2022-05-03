using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldEventManager : MonoBehaviour
{
    //Fog Wall
    public UIBossHealthBar bossHealthBar;
    public EnemyBossManager boss;

    public bool bossFightIsActive;  //Is currently fighting boss
    public bool bossHasBeenAwakened; //Woke the boss/watched cutscene but died during fight
    public bool bossHasBeenDefeated; //Boss has been defeated

    private void Awake()
    {
        bossHealthBar = FindObjectOfType<UIBossHealthBar>();
    }

    public void ActivateBossFight()
    {
        bossFightIsActive = true;
        bossHasBeenAwakened = true;
        bossHealthBar.SetUIHealthBarToActive();
        //Activate Fog Wall(s)
    }

    public void BossHasBeenDefeated()
    {
        bossHasBeenDefeated = true;
        bossFightIsActive = false;

        //Deactivate Fog Walls
    }
}
