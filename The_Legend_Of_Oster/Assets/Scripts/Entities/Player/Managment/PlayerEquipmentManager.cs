using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    InputHandler inputHandler;
    PlayerInventory playerInventory;

    [Header("Equipment Model Changers")]
    HelmetModelChanger helmetModelChanger;
    TorsoModelChanger torsoModelChanger;


    //LEG EQUIPMENT
    //HAND EQUIPMENT

    [Header("Default Naked Models")]
    public GameObject nakedHeadModel;
    public GameObject nakedTorsoModel;


    // public BlockingCollider blockingCollider;

    private void Awake()
    {
        inputHandler = GetComponentInParent<InputHandler>();
        playerInventory = GetComponentInParent<PlayerInventory>();
        helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
        torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
    }

    private void Start()
    {
        EquipAllEquipmentModelsOnStart();
    }

    private void EquipAllEquipmentModelsOnStart()
    {
        helmetModelChanger.UnEquipAllHelmetModels();

        if (playerInventory.currentHelmetEquipment != null)
        {
            nakedHeadModel.SetActive(false);
            helmetModelChanger.EquipHelmetModelByName(playerInventory.currentHelmetEquipment.helmetModelName);
        }
        else
        {
            nakedHeadModel.SetActive(true);
        }

        torsoModelChanger.UnEquipAllTorsoModels();

        if (playerInventory.currentTorsoEquipment != null)
        {
            nakedTorsoModel.SetActive(false);
            torsoModelChanger.EquipTorsoModelByName(playerInventory.currentTorsoEquipment.torsoModelName);
        }
        else
        {
            nakedTorsoModel.SetActive(true);
        }
    }

    /*  public void OpenBlockingCollider()
      {
          if (inputHandler.twoHandFlag)
          {
              blockingCollider.SetColliderDamageAbsorption(playerInventory.rightWeapon);
          }
          else
          {
              blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);
          }

          blockingCollider.EnableBlockingCollider();
      }

      public void CloseBlockingCollider()
      {
          blockingCollider.DisableBlockingCollider();
      }*/
}