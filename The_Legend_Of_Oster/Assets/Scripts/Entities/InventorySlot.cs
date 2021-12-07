using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public UIManager uIManager;
    public InputHandler inputHandler;
    public Image icon;
    public Item item;

    public virtual void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        uIManager = FindObjectOfType<UIManager>();
        inputHandler = FindObjectOfType<InputHandler>();
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = newItem.itemIcon;
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
