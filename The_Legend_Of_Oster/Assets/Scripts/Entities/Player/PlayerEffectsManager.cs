using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    public GameObject currentParticleFX; // Particles that play when the play has an effect going on like poison, healing, etc
    PlayerStats playerStats;
    WeaponSlotManager weaponSlotManager;
    protected override void Awake()
    {
        base.Awake();
        playerStats = GetComponentInParent<PlayerStats>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }
}
