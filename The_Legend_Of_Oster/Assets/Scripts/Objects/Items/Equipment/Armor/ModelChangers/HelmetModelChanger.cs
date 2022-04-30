using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetModelChanger : MonoBehaviour
{
    public List<GameObject> helmetModels;
    public GameObject attachedBone;

 /*   private void Update()
    {
        int childrenGameObjects = transform.childCount;

        for (int i = 0; i < childrenGameObjects; i++)
        {
            transform.GetChild(i).transform.parent = attachedBone.transform;
        }
    }*/

    private void Awake()
    {
        GetAllHelmetModels();
    }

    private void GetAllHelmetModels()
    {
        int childrenGameObjects = transform.childCount;

        for (int i = 0; i < childrenGameObjects; i++)
        {
            helmetModels.Add(transform.GetChild(i).gameObject);
        }
    }

    public void UnEquipAllHelmetModels()
    {
        foreach (GameObject helmetModel in helmetModels)
        {
            helmetModel.SetActive(false);
        }
    }

    public void EquipHelmetModelByName(string helmetName)
    {
        for (int i = 0; i < helmetModels.Count; i++)
        {
            if (helmetModels[i].name == helmetName)
            {
                helmetModels[i].SetActive(true);
            }
        }
    }
}
