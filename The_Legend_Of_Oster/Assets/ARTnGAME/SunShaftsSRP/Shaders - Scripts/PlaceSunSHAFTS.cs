using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaceSunSHAFTS : MonoBehaviour
{
    public Transform Sun;//any object can be used or empty transform
    public float distance = 1000; //place sun object at this distance
    public Light SunLight; //actual sun light
    public float objectScaling = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Sun != null && SunLight != null)
        {
            //place Sun object in sun position in skybox
            Sun.transform.position = -SunLight.transform.forward * distance;
            Sun.transform.localScale = SunLight.transform.localScale * objectScaling * distance * 0.1f;
        }
    }
}
