using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

//using UnityEngine.Rendering.LWRP;
using UnityEngine.Rendering.Universal;

namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class connectSuntoVolumeCloudsURP : MonoBehaviour
    {
        public Camera reflectCamera;

        public bool enableFog = true;//v0.1 off by default
        public bool allowHDR = false;
        public Transform sun;

        public bool blendBackground = false;
        public Camera backgroundCam;
        public Material backgroundMat;

        public Texture2D testTex;
        public Texture2D testTex2;

        ////// VOLUME CLOUDS 
        public float farCamDistFactor = 1;
        //Texture2D colourPalette;
        //bool Made_texture = false;
        public Gradient DistGradient = new Gradient();
        public Vector2 GradientBounds = Vector2.zero;
        //v4.8.6
        public bool adjustNightLigting = true;
        public float backShadeNight = 0.5f; //use this at night for more dense clouds
        public float turbidityNight = 2f;
        public float extinctionNight = 0.01f;
        public float shift_dawn = 0; //add shift to when cloud lighting changes vs the TOD of sky master
        public float shift_dusk = 0;
        //v4.8.4
        //public bool adjustNightLigting = true;
        public Vector3 groundColorNight = new Vector3(0.5f, 0.5f, 0.5f);
        public float scatterNight = 0.12f; //use this at night
        public float reflectFogHeight = 1;
        //v2.1.20
        public bool WebGL = false;
        //v2.1.19
        public bool fastest = false;
        public Light localLight;
        public float localLightFalloff = 2;
        //public float localLightIntensity = 1;
        public float currentLocalLightIntensity = 0;
        public Vector3 _SkyTint = new Vector3(0.5f, 0.5f, 0.5f);
        public float _AtmosphereThickness = 1.0f;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        ////////// CLOUDS
        public bool isForReflections = false;
        //v4.8
        public float _invertX = 0;
        public float _invertRay = 1;
        public Vector3 _WorldSpaceCameraPosC;
        public float varianceAltitude1 = 0;
        //v4.1f
        public float _mobileFactor;
        public float _alphaFactor;
        //v3.5.3
        public Texture2D _InteractTexture;
        public Vector4 _InteractTexturePos;
        public Vector4 _InteractTextureAtr;
        public Vector4 _InteractTextureOffset; //v4.0
        public float _NearZCutoff;
        public float _HorizonYAdjust;
        public float _HorizonZAdjust;
        public float _FadeThreshold;
        //v3.5 clouds	
        public float _BackShade;
        public float _UndersideCurveFactor;
        public Matrix4x4 _WorldClip;
        public float _SampleCount0 = 2;
        public float _SampleCount1 = 3;
        public int _SampleCountL = 4;
        public Texture3D _NoiseTex1;
        public Texture3D _NoiseTex2;
        public float _NoiseFreq1 = 3.1f;
        public float _NoiseFreq2 = 35.1f;
        public float _NoiseAmp1 = 5;
        public float _NoiseAmp2 = 1;
        public float _NoiseBias = -0.2f;
        public Vector3 _Scroll1 = new Vector3(0.01f, 0.08f, 0.06f);
        public Vector3 _Scroll2 = new Vector3(0.01f, 0.05f, 0.03f);
        public float _Altitude0 = 1500;
        public float _Altitude1 = 3500;
        public float _FarDist = 30000;
        public float _Scatter = 0.008f;
        public float _HGCoeff = 0.5f;
        public float _Extinct = 0.01f;
        public float _SunSize;
        public Vector3 _GroundColor; //v4.0
        public float _ExposureUnder; //v4.0
        public float frameFraction = 0;
        //v2.1.19
        //public bool _fastest;
        public Vector4 _LocalLightPos;
        public Vector4 _LocalLightColor;

        /// <summary>
        /// /////////////// PERFORMANCE HANDLE
        /// </summary>
        public int splitPerFrames = 1; //v2.1.19
        public bool cameraMotionCompensate = true;//v2.1.19    
        public float updateRate = 0.3f;
        // public int resolution = 256;
        public float downScaleFactor = 1;
        public float lowerRefrReflResFactor = 3; //v0.1
        public bool downScale = false;
        public bool _needsReset = true;
        public bool enableReproject = false;
        public bool autoReproject = false;
        ///////// END CLOUDS
        ////// END VOLUME CLOUDS

        void OnGUI()
        {
            if (testTex != null && testTex2 != null && Event.current.type.Equals(EventType.Repaint))
            {
                Graphics.DrawTexture(new Rect(10, 10, testTex.width, testTex.height), testTex);
                Graphics.DrawTexture(new Rect(testTex.width + 10, 10, testTex2.width, testTex2.height), testTex2);
            }
        }

        //FOG
        public Light localLightA;
        public float localLightIntensity;
        public float localLightRadius;

        //FOG URP /////////////
        //FOG URP /////////////
        //FOG URP /////////////
        //public float blend =  0.5f;
        public Color _FogColor = Color.white / 2;
        //fog params
        public Texture2D noiseTexture;
        public float _startDistance = 30f;
        public float _fogHeight = 0.75f;
        public float _fogDensity = 1f;
        public float _cameraRoll = 0.0f;
        public Vector4 _cameraDiff = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public float _cameraTiltSign = 1;
        public float heightDensity = 1;
        public float noiseDensity = 1;
        public float noiseScale = 1;
        public float noiseThickness = 1;
        public Vector3 noiseSpeed = new Vector4(1f, 1f, 1f);
        public float occlusionDrop = 1f;
        public float occlusionExp = 1f;
        public int noise3D = 1;
        public float startDistance = 1;
        public float luminance = 1;
        public float lumFac = 1;
        public float ScatterFac = 1;
        public float TurbFac = 1;
        public float HorizFac = 1;
        public float turbidity = 1;
        public float reileigh = 1;
        public float mieCoefficient = 1;
        public float mieDirectionalG = 1;
        public float bias = 1;
        public float contrast = 1;
        public Color TintColor = new Color(1, 1, 1, 1);
        public Vector3 TintColorK = new Vector3(0, 0, 0);
        public Vector3 TintColorL = new Vector3(0, 0, 0);
        public Vector4 Sun = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public Vector4 SunFOG = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public bool FogSky = true;
        public float ClearSkyFac = 1f;
        public Vector4 PointL = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public Vector4 PointLParams = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        public bool _useRadialDistance = false;
        public bool _fadeToSkybox = true;
        //END FOG URP //////////////////
        //END FOG URP //////////////////
        //END FOG URP //////////////////

        //SUN SHAFTS
        public BlitVolumeCloudsSRP.BlitSettings.ShaftsScreenBlendMode screenBlendMode = BlitVolumeCloudsSRP.BlitSettings.ShaftsScreenBlendMode.Screen;
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
            _needsReset = true;
            //postProfile = GetComponent<PostProcessVolume>().profile;
        }

        void Awake()
        {
            _needsReset = true;
            //postProfile = GetComponent<PostProcessVolume>().profile;
        }

        // Update is called once per frame
        void Update()
        {
            if (sun != null)
            {
                UpdateFOG();
                //URP v0.1
                if (Time.fixedTime < 1)
                {
                    _needsReset = true;
                }
            }
        }


        //FOG

        // Update is called once per frame
        //float _cameraRoll;
        // Vector4 _cameraDiff;
        // Vector4 PointLParams;
        // Vector4 Sun;
        // Vector4 PointL;
        // int _cameraTiltSign;
        void UpdateFOG()
        {
            var volFog = this; //The custom forward renderer will read variables from this script

            //var volFog = postProfile.GetSetting<VolumeFogSM_SRP>();
            if (volFog != null)
            {
                if (localLightA != null)
                {

                    //volFog.sunTransform.value = sun.transform.position;
                }
                Camera cam = Camera.current;
                if (cam == null)
                {
                    cam = Camera.main;
                }
                volFog._cameraRoll = cam.transform.eulerAngles.z;

                volFog._cameraDiff = cam.transform.eulerAngles;// - prevRot;

                if (cam.transform.eulerAngles.y > 360)
                {
                    volFog._cameraDiff.y = cam.transform.eulerAngles.y % 360;
                }
                if (cam.transform.eulerAngles.y > 180)
                {
                    volFog._cameraDiff.y = -(360 - volFog._cameraDiff.y);
                }

                //slipt in 90 degs, 90 to 180 mapped to 90 to zero
                //volFog._cameraDiff.value.w = 1;
                if (volFog._cameraDiff.y > 90 && volFog._cameraDiff.y < 180)
                {
                    volFog._cameraDiff.y = 180 - volFog._cameraDiff.y;
                    volFog._cameraDiff.w = -1;
                    //volFog._cameraDiff.value.w = Mathf.Lerp(volFog._cameraDiff.value.w ,- 1, Time.deltaTime * 20);
                }
                else if (volFog._cameraDiff.y < -90 && volFog._cameraDiff.y > -180)
                {
                    volFog._cameraDiff.y = -180 - volFog._cameraDiff.y;
                    volFog._cameraDiff.w = -1;
                    //volFog._cameraDiff.value.w = Mathf.Lerp(volFog._cameraDiff.value.w, -1, Time.deltaTime * 20);
                    //Debug.Log("dde");
                }
                else
                {
                    //volFog._cameraDiff.value.w = Mathf.Lerp(volFog._cameraDiff.value.w, 1, Time.deltaTime * 20);
                    volFog._cameraDiff.w = 1;
                }

                //vertical fix
                if (cam.transform.eulerAngles.x > 360)
                {
                    volFog._cameraDiff.x = cam.transform.eulerAngles.x % 360;
                }
                if (cam.transform.eulerAngles.x > 180)
                {
                    volFog._cameraDiff.x = 360 - volFog._cameraDiff.x;
                }
                //Debug.Log(cam.transform.eulerAngles.x);
                if (cam.transform.eulerAngles.x > 0 && cam.transform.eulerAngles.x < 180)
                {
                    volFog._cameraTiltSign = 1;
                }
                else
                {
                    // Debug.Log(cam.transform.eulerAngles.x);
                    volFog._cameraTiltSign = -1;
                }
                if (sun != null)
                {
                    Vector3 sunDir = -sun.transform.forward;// -sun.transform.forward;
                                                            //sunDir = Quaternion.AngleAxis(-cam.transform.eulerAngles.y, Vector3.up) * -sunDir;
                                                            //sunDir = Quaternion.AngleAxis(cam.transform.eulerAngles.x, Vector3.left) * sunDir;
                                                            //sunDir = Quaternion.AngleAxis(-cam.transform.eulerAngles.z, Vector3.forward) * sunDir;
                                                            // volFog.Sun.value = -new Vector4(sunDir.x, sunDir.y, sunDir.z, 1);
                    volFog.Sun = new Vector4(sunDir.x, sunDir.y, sunDir.z, 1);
                    //volFog.sun.position = new Vector3(sunDir.x, sunDir.y, sunDir.z);////// vector 4 to vector3

                    //FOG SUN
                    Vector3 sunDiFOGr = sun.transform.forward;// -sun.transform.forward;
                    sunDiFOGr = Quaternion.AngleAxis(-cam.transform.eulerAngles.y, Vector3.up) * -sunDiFOGr;
                    sunDiFOGr = Quaternion.AngleAxis(cam.transform.eulerAngles.x, Vector3.left) * sunDiFOGr;
                    sunDiFOGr = Quaternion.AngleAxis(-cam.transform.eulerAngles.z, Vector3.forward) * sunDiFOGr;
                    // volFog.Sun.value = -new Vector4(sunDir.x, sunDir.y, sunDir.z, 1);
                    volFog.SunFOG = new Vector4(sunDiFOGr.x, sunDiFOGr.y, sunDiFOGr.z, 1);
                }
                else
                {
                    volFog.Sun = new Vector4(15, 0, 1, 1);
                }
                if (localLightA != null)
                {
                    volFog.PointL = new Vector4(localLightA.transform.position.x, localLightA.transform.position.y, localLightA.transform.position.z, localLightIntensity);
                    volFog.PointLParams = new Vector4(localLightA.color.r, localLightA.color.g, localLightA.color.b, localLightRadius);
                }
                //Debug.Log(volFog._cameraDiff.value);
                //prevRot = cam.transform.eulerAngles;
            }
        }

        //END FOG

    }
}