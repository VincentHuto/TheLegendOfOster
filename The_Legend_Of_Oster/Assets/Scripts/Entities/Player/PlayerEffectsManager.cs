using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    public GameObject currentParticleFX; // Particles that play when the play has an effect going on like poison, healing, etc
    public GameObject instantiatedFXModel;
    PlayerStats playerStats;
    WeaponSlotManager weaponSlotManager;
    PlayerInventory playerInventory;
    public float healAmount;
    protected override void Awake()
    {
        base.Awake();
        playerStats = GetComponentInParent<PlayerStats>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
        playerInventory = GetComponentInParent<PlayerInventory>();
    }

    public void HealPlayerFromEffect()
    {
        if(playerInventory.currentConsumable is FlaskItem flask)
        {
            Debug.Log("IS FLASK: " + flask.name);
            if (flask.healthFlask)
            {
                playerStats.HealPlayerHealth(healAmount);
            }
            else if (flask.staminaFlask)
            {
                playerStats.HealPlayerStamina(healAmount);

            }
            else if (flask.breathFlask)
            {
                playerStats.HealPlayerBreath(healAmount);
            }
            else
            {

            }
        }

        DestroyAndReload();
    }
    public void DestroyAndReload()
    {
        GameObject healParticles = Instantiate(currentParticleFX, playerStats.transform);
        Destroy(instantiatedFXModel.gameObject);
        weaponSlotManager.LoadBothWeaponsOnSlots();
    }
}
