using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{

    //Attach this script to any object you want to be able to be picked up
    public Transform theDest;
    //Idk 5 seems to be a good sweet spot for most objects
    public float pickDistance = 5;

    void OnMouseDown()
    {

        float distance = Vector3.Distance(this.transform.position, GameObject.Find("ObjectHeld").transform.position);
        if(distance < pickDistance)
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = theDest.position;
            this.transform.parent = GameObject.Find("ObjectHeld").transform;
        }

    }
    void OnMouseUp()
    {
        float distance = Vector3.Distance(this.transform.position, GameObject.Find("ObjectHeld").transform.position);
        if (distance < pickDistance)
        {
            this.transform.parent = null;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<BoxCollider>().enabled = true;
        }
    }
}
