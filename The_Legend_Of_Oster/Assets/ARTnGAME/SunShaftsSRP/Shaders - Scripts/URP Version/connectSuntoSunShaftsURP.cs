using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

//using UnityEngine.Rendering.LWRP;
using UnityEngine.Rendering.Universal;

namespace Artngame.SKYMASTER
{

    [ExecuteInEditMode]
    public class connectSuntoSunShaftsURP : MonoBehaviour
    {
        public bool enableShafts = true;
        public Transform sun;
        public BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode screenBlendMode = BlitSunShaftsSRP.BlitSettings.ShaftsScreenBlendMode.Screen;
        //public Vector3 sunTransform = new Vector3(0f, 0f, 0f); 
        public int radialBlurIterations = 2;
        public Color sunColor = Color.white;
        public Color sunThreshold = new Color(0.87f, 0.74f, 0.65f);
        public float sunShaftBlurRadius = 2.5f;
        public float sunShaftIntensity = 1.15f;
        public float maxRadius = 0.75f;
        public bool useDepthTexture = true;
        //PostProcessProfile postProfile;

        // Start is called before the first frame update
        void Start()
        {
            //postProfile = GetComponent<PostProcessVolume>().profile;
        }

        // Update is called once per frame
        void Update()
        {
            if (sun != null)
            {

            }
        }
    }
}