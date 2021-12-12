using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    public GameObject currentParticleFX; // Particles that play when the play has an effect going on like poison, healing, etc
    public GameObject instantiatedFXModel;
    PlayerStats playerStats;
    WeaponSlotManager weaponSlotManager;
    public float healAmount;
    protected override void Awake()
    {
        base.Awake();
        playerStats = GetComponentInParent<PlayerStats>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    public void HealPlayerFromEffect()
    {
        Debug.Log("FEWFJNIWEFJWEF");
        Debug.Log(instantiatedFXModel);
        Debug.Log(instantiatedFXModel);

        playerStats.HealPlayer(healAmount);
        GameObject healParticles = Instantiate(currentParticleFX, playerStats.transform);
        Destroy(instantiatedFXModel.gameObject);
        weaponSlotManager.LoadBothWeaponsOnSlots();
        Destroy(healParticles.gameObject);
    
    }

}
