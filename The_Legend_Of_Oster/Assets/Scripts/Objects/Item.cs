using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
  /*  [SerializeField] string id;
    public string ID { get { return id; } }*/
    [Header("Item Information")]
    public Sprite itemIcon;
    public string itemName;
    public int stacksTo;
}


