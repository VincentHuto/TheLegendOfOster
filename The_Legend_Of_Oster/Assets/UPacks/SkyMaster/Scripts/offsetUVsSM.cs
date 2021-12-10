using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.ma

public class offsetUVsSM : MonoBehaviour
{

    public Material material;
    float offset = 0;
    public float speed = 0.06f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (material != null)
        {
            offset = 0.08f + Mathf.Abs(speed * Mathf.Cos(Time.fixedTime * 0.5f));// offset + 0.0001f;
            material.SetFloat("_NormalScale", offset);
            //material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
            //material.SetTextureOffset("_BumpMap", new Vector2(offset, 0));
        }
    }
}
