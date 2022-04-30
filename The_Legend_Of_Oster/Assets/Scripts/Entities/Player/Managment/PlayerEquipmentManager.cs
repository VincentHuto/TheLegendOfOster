using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    InputHandler inputHandler;
    PlayerInventory playerInventory;

    [Header("Equipment Model Changers")]
    //Head Equipment
    HelmetModelChanger helmetModelChanger;
    //Torso Equipment
    TorsoModelChanger torsoModelChanger;
    UpperLeftArmModelChanger upperLeftArmModelChanger;
    UpperRightArmModelChanger upperRightArmModelChanger;
    //Leg Equipment
    HipModelChanger hipModelChanger;
    LeftLegModelChanger leftLegModelChanger;
    RightLegModelChanger rightLegModelChanger;
    //Hand Equipment
    LowerLeftArmModelChanger lowerLeftArmModelChanger;
    LowerRightArmModelChanger lowerRightArmModelChanger;
    LeftHandModelChanger leftHandModelChanger;
    RightHandModelChanger rightHandModelChanger;

    //LEG EQUIPMENT
    //HAND EQUIPMENT

    [Header("Default Naked Models")]
    public GameObject nakedHead;
    public GameObject nakedTorso;
    public GameObject nakedUpperLeftArm;
    public GameObject nakedUpperRightArm;
    public GameObject nakedLowerLeftArm;
    public GameObject nakedLowerRightArm;
    public GameObject nakedLeftHand;
    public GameObject nakedRightHand;
    public GameObject nakedHipModel;
    public GameObject nakedLeftLeg;
    public GameObject nakedRightLeg;


    // public BlockingCollider blockingCollider;

    private void Awake()
    {
        inputHandler = GetComponentInParent<InputHandler>();
        playerInventory = GetComponentInParent<PlayerInventory>();
        helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
        torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
        hipModelChanger = GetComponentInChildren<HipModelChanger>();
        leftLegModelChanger = GetComponentInChildren<LeftLegModelChanger>();
        rightLegModelChanger = GetComponentInChildren<RightLegModelChanger>();
        upperLeftArmModelChanger = GetComponentInChildren<UpperLeftArmModelChanger>();
        upperRightArmModelChanger = GetComponentInChildren<UpperRightArmModelChanger>();
        lowerLeftArmModelChanger = GetComponentInChildren<LowerLeftArmModelChanger>();
        lowerRightArmModelChanger = GetComponentInChildren<LowerRightArmModelChanger>();
        leftHandModelChanger = GetComponentInChildren<LeftHandModelChanger>();
        rightHandModelChanger = GetComponentInChildren<RightHandModelChanger>();
    }

    private void Start()
    {
        EquipAllEquipmentModelsOnStart();
    }

    private void EquipAllEquipmentModelsOnStart()
    {
        //HELMET EQUIPMENT
        helmetModelChanger.UnEquipAllHelmetModels();

        if (playerInventory.currentHelmetEquipment != null)
        {
            nakedHead.SetActive(!playerInventory.currentHelmetEquipment.hidesBody);
            helmetModelChanger.EquipHelmetModelByName(playerInventory.currentHelmetEquipment.helmetModelName);
        }
        else
        {
            nakedHead.SetActive(true);
        }

        //TORSO EQUIPMENT
        torsoModelChanger.UnEquipAllTorsoModels();
        upperLeftArmModelChanger.UnEquipAllModels();
        upperRightArmModelChanger.UnEquipAllModels();

        if (playerInventory.currentTorsoEquipment != null)
        {
            nakedTorso.SetActive(!playerInventory.currentTorsoEquipment.hidesBody);
            torsoModelChanger.EquipTorsoModelByName(playerInventory.currentTorsoEquipment.torsoModelName);
            upperLeftArmModelChanger.EquipModelByName(playerInventory.currentTorsoEquipment.upperLeftArmModelName);
            upperRightArmModelChanger.EquipModelByName(playerInventory.currentTorsoEquipment.upperRightArmModelName);
        }
        else
        {
            nakedTorso.SetActive(true);
            nakedUpperLeftArm.SetActive(true);
            nakedUpperRightArm.SetActive(true);
        }

        //LEG EQUIPMENT
        hipModelChanger.UnEquipAllHipModels();
        leftLegModelChanger.UnEquipAllLegModels();
        rightLegModelChanger.UnEquipAllLegModels();

        if (playerInventory.currentLegEquipment != null)
        {
            nakedHipModel.SetActive(!playerInventory.currentLegEquipment.hidesBody);
            nakedLeftLeg.SetActive(!playerInventory.currentLegEquipment.hidesBody);
            nakedRightLeg.SetActive(!playerInventory.currentLegEquipment.hidesBody);


            hipModelChanger.EquipHipModelByName(playerInventory.currentLegEquipment.hipModelName);
            leftLegModelChanger.EquipLegModelByName(playerInventory.currentLegEquipment.leftLegName);
            rightLegModelChanger.EquipLegModelByName(playerInventory.currentLegEquipment.rightLegName);
        }
        else
        {
            nakedHipModel.SetActive(true);
            nakedLeftLeg.SetActive(true);
            nakedRightLeg.SetActive(true);
        }

        //HAND EQUIPMENT
        lowerLeftArmModelChanger.UnEquipAllModels();
        lowerRightArmModelChanger.UnEquipAllModels();
        leftHandModelChanger.UnEquipAllModels();
        rightHandModelChanger.UnEquipAllModels();

        if (playerInventory.currentHandEquipment != null)
        {
            nakedLowerLeftArm.SetActive(!playerInventory.currentHandEquipment.hidesBody);
            nakedLowerRightArm.SetActive(!playerInventory.currentHandEquipment.hidesBody);
            nakedLeftHand.SetActive(!playerInventory.currentHandEquipment.hidesBody);
            nakedRightHand.SetActive(!playerInventory.currentHandEquipment.hidesBody);

            lowerLeftArmModelChanger.EquipModelByName(playerInventory.currentHandEquipment.lowerLeftArmModelName);
            lowerRightArmModelChanger.EquipModelByName(playerInventory.currentHandEquipment.lowerRightArmModelName);
            leftHandModelChanger.EquipModelByName(playerInventory.currentHandEquipment.leftHandModelName);
            rightHandModelChanger.EquipModelByName(playerInventory.currentHandEquipment.rightHandModelName);
        }
        else
        {
            nakedLowerLeftArm.SetActive(true);
            nakedLowerRightArm.SetActive(true);
            nakedLeftHand.SetActive(true);
            nakedRightHand.SetActive(true);
        }
    }


}