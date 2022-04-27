using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : NetworkBehaviour
{
    public float radius = 0.6f;
    public string interactionText;
    public UIManager uIManager;


    public void Update()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, radius, transform.up, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("FOUND PLAYER");

                uIManager = hit.collider.GetComponentInParent<UIManager>();
                if (uIManager != null)
                {
                    Debug.Log("FOUND UI");
                }
                else
                {
                    Debug.Log("LOST UI");

                }
            }
        }
    }

    /*    public void Awake()
        {
            uIManager = FindObjectOfType<UIManager>();
        }*/

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public virtual void Interact(PlayerManager playerManager)
    {
        //Called when player interacts
        //  Debug.Log("You interacted with an object");

    }

}
