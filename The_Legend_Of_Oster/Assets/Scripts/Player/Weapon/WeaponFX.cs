using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFX : MonoBehaviour
{
    [Header("Weapon FX")]
    public ParticleSystem normalWeaponTrail;
    //add other buffed weapon trails here

    public void PlayWeaponFX()
    {
        normalWeaponTrail.Stop();
        if (normalWeaponTrail.isStopped)
        {
            normalWeaponTrail.Play();
        }
    }
}
