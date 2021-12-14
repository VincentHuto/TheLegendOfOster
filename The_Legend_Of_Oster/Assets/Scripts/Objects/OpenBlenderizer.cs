using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBlenderizer : Interactable
{
    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        UseBlender(playerManager);
    }

    public void UseBlender(PlayerManager playerManager)
    {

        //Debug.Log(uIManager.playerInventory.craftingItemInventory.Count);

        if (uIManager.IsBlenderOpen)
        {
            uIManager.CloseBlenderWindow();
        }
        else
        {
            uIManager.OpenBlenderWindow();
        }
    }


}
