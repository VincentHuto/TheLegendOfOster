using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class ToolTipManager : MonoBehaviour
{
    public bool BlockedByUI;
    private EventTrigger eventTrigger;
    Image icon;
    public ItemDescriptionWindow descriptionWindow;

    private void Start()
    {
        icon = GetComponentInChildren<Image>();

        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry();
            // Pointer Enter


            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);
            if (eventTrigger.gameObject.GetComponentInChildren<Image>().sprite != null && descriptionWindow.GetComponentInChildren<Image>() != null)
            {
                Text descText = descriptionWindow.gameObject.GetComponentInChildren<Text>();
                Image descImage = descriptionWindow.gameObject.GetComponentInChildren<Image>();
                ItemStack hoveredStack = eventTrigger.gameObject.GetComponent<InventorySlot>().item;


                descImage.sprite = hoveredStack.itemType.itemIcon;
                descText.text = hoveredStack.itemType.name + "\n" + hoveredStack.itemType.itemDesc;
            }
            //Pointer Exit
            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry();
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);
        }
    }


    public void EnterUI()
    {
        BlockedByUI = true;

        if (eventTrigger.gameObject.GetComponentInChildren<Image>().sprite != null && descriptionWindow.GetComponentInChildren<Image>() != null)
        {
            Text descText = descriptionWindow.gameObject.GetComponentInChildren<Text>();
            Image descImage = descriptionWindow.gameObject.GetComponentInChildren<Image>();
            ItemStack hoveredStack = eventTrigger.gameObject.GetComponent<InventorySlot>().item;
            descImage.sprite = hoveredStack.itemType.itemIcon;
            descText.text = hoveredStack.itemType.name + "\n" + hoveredStack.itemType.itemDesc;

        }
    }
    public void ExitUI()
    {
        BlockedByUI = false;
    }
}
