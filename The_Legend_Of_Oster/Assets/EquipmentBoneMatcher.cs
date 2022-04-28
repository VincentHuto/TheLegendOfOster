using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentBoneMatcher : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        GetAllSkinnedMeshRenderers(ref boneMap);

        SkinnedMeshRenderer myRenderer = GetComponent<SkinnedMeshRenderer>();
        Transform[] newBones = new Transform[myRenderer.bones.Length];

        for (int i = 0; i < myRenderer.bones.Length; ++i)
        {
            GameObject bone = myRenderer.bones[i].gameObject;
            if (!boneMap.TryGetValue(bone.name, out newBones[i]))
            {
                Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton.");
            }
        }

        myRenderer.bones = newBones;
    }


    void GetAllSkinnedMeshRenderers(ref Dictionary<string, Transform> map)
    {
        SkinnedMeshRenderer[] renderers = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

        foreach (SkinnedMeshRenderer smr in renderers)
        {
            foreach (Transform bone in smr.bones)
            {
                if (!boneMap.ContainsKey(bone.gameObject.name)) boneMap[bone.gameObject.name] = bone;
            }

        }

        map = boneMap;
    }
}