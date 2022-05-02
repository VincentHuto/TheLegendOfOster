using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager : NetworkBehaviour
{
    public Transform lockOnTransform;
    public SpriteRenderer spriteRenderer;

    public int pendingCriticalDamage;

    [Header("Movement Flags")]
    public bool isRotatingWithRootMotion;
    public bool canRotate;

    [Header("Combat Flags")]
    public bool canDoCombo;

   



}
