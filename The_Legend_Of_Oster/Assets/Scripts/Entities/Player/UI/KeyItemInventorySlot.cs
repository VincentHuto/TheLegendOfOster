using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyItemInventorySlot : MonoBehaviour
{
    PlayerInventory playerInventory;
    UIManager uIManager;
    InputHandler inputHandler;
    public Image icon;
    KeyItem item;

    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        uIManager = FindObjectOfType<UIManager>();
        inputHandler = FindObjectOfType<InputHandler>();
    }

    public void AddItem(KeyItem weaponItem)
    {
        item = weaponItem;
        icon.sprite = weaponItem.itemIcon;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        gameObject.SetActive(false);
    }

}
