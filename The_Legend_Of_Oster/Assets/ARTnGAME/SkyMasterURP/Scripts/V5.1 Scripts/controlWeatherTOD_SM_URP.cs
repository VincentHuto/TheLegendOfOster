using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artngame.SKYMASTER;
//using ARTNGAME.Skymaster;
//using BrunetonsAtmosphere;

//NOTES
//https://forum.unity.com/threads/hdrp-material-sorting-priority-c-scripting.1034344/ - Cloud sorting priority
//material.renderQueue = 3000 + sortingPriority;
//    material.SetFloat("_TransparentSortPriority", sortingPriority);
//https://www.turiyaware.com/blog/unity-ui-blur-in-hdrp - HDRP blur

namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class controlWeatherTOD_SM_URP : MonoBehaviour
    {

        //v0.2
        public Transform water;
        public PlanarRefractionsSM_LWRP refraction;
        public PlanarReflectionsSM_LWRP reflection;
        public DepthRendererSM_LWRP depthRenderer;

        //v0.1
        public CloudHandlerSM_SRP shaderVolClouds;

        // public Sky skyDome;
        public lightningCameraVolumeCloudsSM_URP lightningController;

        //If sky manager exsit, control weather and TOD
        public SkyMasterManager skyManager;

        //Control TOD coloratino for full volume clouds
        public connectSuntoFullVolumeCloudsURP fullVolumeClouds;

        public connectSuntoVolumeFogURP etherealVolFog;

        //Control water TODparams
        public WaterHandlerSM waterManager;

        //choose density per weather type
        public List<float> weatherCloudDensities = new List<float>();
        public float densitiesOffset = 0;

        public bool ApplyPresetDensities = true;
        public int cloudType = 0;
        //0 = full volume clouds, 1=background volume clouds, 2=dome background vol clouds, 3= InfiniCLOUD shader volume clouds 

        // Start is called before the first frame update
        void Start()
        {
            if (temporalSunMoon == null)//if (Application.isPlaying && temporalSunMoon == null)
            {
                temporalSunMoon = new GameObject();
                temporalSunMoon.name = "SunMoonTransformLerp";
                temporalSunMoon.transform.rotation = skyManager.SUN_LIGHT.transform.rotation;
                temporalSunMoon.transform.position = skyManager.SUN_LIGHT.transform.position;
                Light temp = temporalSunMoon.AddComponent<Light>();
                temp = fullVolumeClouds.sunLight;
                fullVolumeClouds.sunLight = temporalSunMoon.GetComponent<Light>();
            }
            else
            {
                temporalSunMoon.transform.rotation = skyManager.SUN_LIGHT.transform.rotation;
                temporalSunMoon.transform.position = skyManager.SUN_LIGHT.transform.position;
            }

            //set mooon based on time of day - X = vertical (minus goes up, 180 to 360), Y = horizontal
            //      Vector4 moonSettings = skyDome.m_skyMaterial.GetVector("_moonPos");
            //       skyDome.m_skyMaterial.SetVector("_moonPos", new Vector4(270 + Mathf.Cos(skyManager.Current_Time * 2) * 90,180+Mathf.Cos(Time.fixedTime*2)*180, moonSettings.z, moonSettings.w));

            //set cloud density at game start based on weather
            if (fullVolumeClouds != null)
            {
                setFullVolumeCloudsDensity(false);
                setFullVolumeCloudsColors();
            }



            //URP
            //v3.3 - init galaxy
            //v3.3 - shader based stars fade
            if (skyManager.skyboxMat != null)
            {
                //Color StarsCol = StarsMaterial.GetColor ("_Color");
                if (
                    (!skyManager.AutoSunPosition && (skyManager.Current_Time >= (9 + skyManager.Shift_dawn) & skyManager.Current_Time <= (skyManager.NightTimeMax + skyManager.Shift_dawn)))
                    |
                    (skyManager.AutoSunPosition && skyManager.Rot_Sun_X > 0))
                {
                    if (skyManager.Current_Time < 1f)
                    { //initialize at game start
                        skyManager.skyboxMat.SetColor("_Color", new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.y));
                    }
                    else
                    {
                        skyManager.skyboxMat.SetColor("_Color", new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.y));
                    }
                }
                else
                {
                    if (skyManager.Current_Time < 1f)
                    { //initialize at game start
                        skyManager.skyboxMat.SetColor("_Color", new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.x));
                    }
                    else
                    {
                        skyManager.skyboxMat.SetColor("_Color", new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.x));
                    }
                }
                //float StarsCover = StarsMaterial.GetFloat ("_Light");
                skyManager.skyboxMat.SetFloat("_Light", skyManager.StarsIntensity);
            }


            //v0.3 - Apply GUI
            if (enableGUI && Application.isPlaying)
            {
                volumeLightPower = etherealVolFog.blendVolumeLighting;
                volumeFogColor = etherealVolFog._FogColor;
                lightNoiseControl = etherealVolFog.lightNoiseControl;
                volumeFogNoise = etherealVolFog.noiseThickness;
                volumeFogHeight = etherealVolFog._fogHeight;
                volumeFogDensity = etherealVolFog._fogDensity;
                heigthNoiseDensities.x = etherealVolFog.heightDensity;
                heigthNoiseDensities.y = etherealVolFog.noiseDensity;
                volumeFogNoisePower = etherealVolFog.stepsControl.w;
                etherealVolFog.noiseScale = volumeNoiseScale;
                noiseSpeed = etherealVolFog.noiseSpeed.x;
                clearskyFactor =etherealVolFog.ClearSkyFac;
            }

        }

        //Lighting
        public GameObject temporalSunMoon;

        //v0.3
        public void OnGUI()
        {

            if (GUI.Button(new Rect(10 + 0 * 100, 10 + 1 * 30, 100, 30), "GUI Toggle"))
            {
                if (enableGUI)
                {
                    enableGUI = false;
                }
                else
                {
                    enableGUI = true;
                }
            }
            if (enableGUI && Application.isPlaying)
            {
                if (GUI.Button(new Rect(10 + 0 * 100, 10 + 0 * 30, 100, 30), "Dust Storm"))
                {
                    volumeLightPower = 0.65f;
                    volumeFogNoise = 0.1f;
                    volumeFogHeight = 60;
                    volumeFogColor = new Color(67.0f/255.0f, 34.0f / 255.0f, 17.0f / 255.0f); //Color.red * 0.5f + Color.yellow * 0.25f;
                    lightNoiseControl = new Vector4(0.7f, 0, 0.5f, 6);
                    volumeFogDensity = 0.3f;
                    heigthNoiseDensities = new Vector2(15, 0.5f);
                    volumeFogNoisePower = 0.25f;
                    volumeNoiseScale = 0.002f;
                    noiseSpeed = 100;
                    clearskyFactor = 0.07f;
                    water.gameObject.SetActive(false);
                    refraction.enabled = false;
                    reflection.enabled = false;
                    depthRenderer.enabled = false;
                }
                if (GUI.Button(new Rect(10 + 1 * 100, 10 + 0 * 30, 100, 30), "Dust Light"))
                {
                    volumeLightPower = 0.01f;
                    volumeFogNoise = 0.12f; //noiseThickness
                    volumeFogHeight = 105;
                    volumeFogColor = new Color(248.0f / 255.0f, 117.0f / 255.0f, 50.0f / 255.0f); //Color.white * 0.12f;
                    lightNoiseControl = new Vector4(0.7f, 0, 0.5f, 6);
                    volumeFogDensity = 0.056f; //_fogDensity
                    heigthNoiseDensities = new Vector2(8f, 1.0f);
                    volumeFogNoisePower = 0.55f; //stepsControl.w 
                    volumeNoiseScale = 0.18f;//noiseScale
                    noiseSpeed = 40;
                    clearskyFactor = 0.07f;
                    water.gameObject.SetActive(false);
                    refraction.enabled = false;
                    reflection.enabled = false; 
                    depthRenderer.enabled = false;
                }
                if (GUI.Button(new Rect(10 + 2 * 100, 10 + 0 * 30, 100, 30), "Heavy Fog"))
                {
                    volumeLightPower = 1.05f;
                    volumeFogNoise = 0.1f;
                    volumeFogHeight = 60;
                    volumeFogColor = Color.white * 0.15f;
                    lightNoiseControl = new Vector4(0.75f,0,0.5f,3);
                    volumeFogDensity = 0.3f;
                    heigthNoiseDensities = new Vector2(15, 0.5f);
                    volumeFogNoisePower = 0.25f; volumeNoiseScale = 0.002f;
                    noiseSpeed = 100;
                    clearskyFactor = 0.07f;
                    water.gameObject.SetActive(false);
                    refraction.enabled = false;
                    reflection.enabled = false; 
                    depthRenderer.enabled = false;
                }
                if (GUI.Button(new Rect(10 + 3 * 100, 10 + 0 * 30, 100, 30), "Light Fog"))
                {
                    volumeLightPower = 1.05f;
                    volumeFogNoise = 0.1f;
                    volumeFogHeight = 60;
                    volumeFogColor = Color.white * 0.55f;
                    lightNoiseControl = new Vector4(0.75f, 0, 0.5f, 3);
                    volumeFogDensity = 0.12f;
                    heigthNoiseDensities = new Vector2(15, 0.5f);
                    volumeFogNoisePower = 0.55f; volumeNoiseScale = 0.15f;
                    noiseSpeed = 70;
                    clearskyFactor = 0.07f;
                    water.gameObject.SetActive(false);
                    refraction.enabled = false;
                    reflection.enabled = false; 
                    depthRenderer.enabled = false;
                }
                if (GUI.Button(new Rect(10 + 4 * 100, 10 + 0 * 30, 100, 30), "Snow Fog"))
                {
                    volumeLightPower = 0.05f;
                    volumeFogNoise = 0.1f;
                    volumeFogHeight = 460;
                    volumeFogColor = new Color(148.0f / 255.0f, 148.0f / 255.0f, 148.0f / 255.0f); //Color.white * 0.12f;
                    lightNoiseControl = new Vector4(1.0f, 0, 0.5f, 3);
                    volumeFogDensity = 0.01f;
                    heigthNoiseDensities = new Vector2(5.5f, 1.35f);
                    volumeFogNoisePower = 14f; volumeNoiseScale = 0.01f;
                    noiseSpeed = 100;
                    clearskyFactor = 0.07f;
                    water.gameObject.SetActive(false);
                    refraction.enabled = false;
                    reflection.enabled = false; 
                    depthRenderer.enabled = false;
                }
                if (GUI.Button(new Rect(10 + 5 * 100, 10 + 0 * 30, 100, 30), "Snow Heavy"))
                {
                    volumeLightPower = 0.05f;
                    volumeFogNoise = 0.1f; //noiseThickness
                    volumeFogHeight = 620;
                    volumeFogColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f); //Color.white * 0.12f;
                    lightNoiseControl = new Vector4(1.0f, -12, 0.35f, 2);
                    volumeFogDensity = 0.015f; //_fogDensity
                    heigthNoiseDensities = new Vector2(5.5f, 1.35f);
                    volumeFogNoisePower = 14f; //stepsControl.w 
                    volumeNoiseScale = 0.18f;//noiseScale
                    noiseSpeed = 30;
                    clearskyFactor = 0.07f;
                    water.gameObject.SetActive(false);
                    refraction.enabled = false;
                    reflection.enabled = false; 
                    depthRenderer.enabled = false;
                }
                if (GUI.Button(new Rect(10 + 6 * 100, 10 + 0 * 30, 100, 30), "Sun Shafts"))
                {
                    volumeLightPower = 0.75f;
                    volumeFogNoise = 0.01f;
                    volumeFogHeight = 0;
                    volumeFogColor = new Color(29.0f / 255.0f, 29.0f / 255.0f, 29.0f / 255.0f); //Color.white * 0.05f;
                    lightNoiseControl = new Vector4(0.75f, 0, 0.5f, 3);
                    volumeFogDensity = 0.01f;
                    heigthNoiseDensities = new Vector2(0, 0);
                    volumeFogNoisePower = 0.05f;
                    volumeNoiseScale = 0;
                    noiseSpeed = 0;
                    clearskyFactor = 0.43f;
                    water.gameObject.SetActive(false);
                    refraction.enabled = false;
                    reflection.enabled = false;
                    depthRenderer.enabled = false;
                }
                if (GUI.Button(new Rect(10 + 7 * 100, 10 + 0 * 30, 100, 30), "Clear Fog"))
                {
                    volumeLightPower = 0.5f;
                    volumeFogNoise = 0.01f;
                    volumeFogHeight = 0;
                    volumeFogColor = new Color(29.0f / 255.0f, 29.0f / 255.0f, 29.0f / 255.0f); //Color.white * 0.05f;
                    lightNoiseControl = new Vector4(0.75f, 0, 0.5f, 3);
                    volumeFogDensity = 0.01f;
                    heigthNoiseDensities = new Vector2(0, 0);
                    volumeFogNoisePower = 0.05f;
                    volumeNoiseScale = 0;
                    noiseSpeed = 0;
                    clearskyFactor = 0.43f;
                    water.gameObject.SetActive(true);
                    refraction.enabled = true;
                    reflection.enabled = true;
                    depthRenderer.enabled = true;
                }



                if (GUI.Button(new Rect(10 + 0 * 100, 10 + 2 * 30, 100, 30), "Sunny"))
                {
                    skyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.Sunny;
                    skyManager.WeatherSeverity = 1;
                }
                if (GUI.Button(new Rect(10 + 1 * 100, 10 + 2 * 30, 100, 30), "Cloudy"))
                {
                    skyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.Cloudy;
                    skyManager.WeatherSeverity = 1;
                }
                if (GUI.Button(new Rect(10 + 2 * 100, 10 + 2 * 30, 100, 30), "Light Rain"))
                {
                    skyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.Rain;
                    skyManager.WeatherSeverity = 10;
                }
                if (GUI.Button(new Rect(10 + 3 * 100, 10 + 2 * 30, 100, 30), "Heavy Rain"))
                {
                    skyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.Rain;
                    skyManager.WeatherSeverity = 20;
                }
                if (GUI.Button(new Rect(10 + 4 * 100, 10 + 2 * 30, 100, 30), "Light Snow"))
                {
                    skyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.SnowStorm;
                    skyManager.WeatherSeverity = 10;
                }
                if (GUI.Button(new Rect(10 + 5 * 100, 10 + 2 * 30, 100, 30), "Heavy Snow"))
                {
                    skyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.SnowStorm;
                    skyManager.WeatherSeverity = 20;
                }
                if (GUI.Button(new Rect(10 + 6 * 100, 10 + 2 * 30, 100, 30), "Lightning"))
                {
                    skyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.LightningStorm;
                    skyManager.WeatherSeverity = 20;
                }

                //volumeLightPower = 1;
                //public float volumeFogPower = 1;
                // public float volumeFogNoise = 0;
                //public Color volumeFogColor = Color.white * 0.15f;
            }
        }
        //v0.3 - GUI
        public Vector2 heigthNoiseDensities = new Vector2(0,0);
        public float clearskyFactor = 0.43f;
        public float noiseSpeed = 0;
        public float volumeFogHeight = 0;
        public float volumeNoiseScale = 0;
        public float volumeFogDensity = 0;
        public float volumeLightPower = 1;
        public float volumeFogPower = 1;
        public float volumeFogNoise = 0;
        public float volumeFogNoisePower = 1;
        public Color volumeFogColor = Color.white * 0.15f;
        public Vector4 lightNoiseControl = new Vector4(1,1,1,1);
        public bool enableGUI = false;
        // Update is called once per frame
        void Update()
        {

            //v0.3 - Apply GUI
            if (enableGUI && Application.isPlaying)
            {
                etherealVolFog.blendVolumeLighting = volumeLightPower;
                etherealVolFog._FogColor = volumeFogColor;
                etherealVolFog.lightNoiseControl = lightNoiseControl;
                etherealVolFog.noiseThickness = volumeFogNoise;
                etherealVolFog._fogHeight = volumeFogHeight;
                etherealVolFog._fogDensity = volumeFogDensity;
                etherealVolFog.heightDensity = heigthNoiseDensities.x;
                etherealVolFog.noiseDensity = heigthNoiseDensities.y;
                etherealVolFog.stepsControl.w = volumeFogNoisePower;
                etherealVolFog.noiseScale = volumeNoiseScale;
                etherealVolFog.noiseSpeed.x = noiseSpeed;
                etherealVolFog.ClearSkyFac = clearskyFactor;
            }

            //URP
            //v3.3 - shader based stars fade
            if (skyManager.skyboxMat != null)
            {
                Color StarsCol = skyManager.skyboxMat.GetColor("_Color");
                if (
                    (!skyManager.AutoSunPosition && (skyManager.Current_Time >= (9 + skyManager.Shift_dawn) & skyManager.Current_Time <= (skyManager.NightTimeMax + skyManager.Shift_dawn)))
                    |
                    (skyManager.AutoSunPosition && skyManager.Rot_Sun_X > 0))
                {
                    if (skyManager.Current_Time < 1f)
                    { //initialize at game start
                        skyManager.skyboxMat.SetColor("_Color", new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.y));
                    }
                    else
                    {
                        skyManager.skyboxMat.SetColor("_Color", Color.Lerp(StarsCol,
                            new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.y), Time.deltaTime));
                    }
                }
                else
                {
                    if (skyManager.Current_Time < 1f)
                    { //initialize at game start
                        skyManager.skyboxMat.SetColor("_Color", new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.x));
                    }
                    else
                    {
                        skyManager.skyboxMat.SetColor("_Color", Color.Lerp(StarsCol,
                            new Color(skyManager.StarsColor.r, skyManager.StarsColor.g, skyManager.StarsColor.b, 1 - skyManager.MinMaxStarsTransp.x), Time.deltaTime));
                    }
                }
                float StarsCover = skyManager.skyboxMat.GetFloat("_Light");
                skyManager.skyboxMat.SetFloat("_Light", Mathf.Lerp(StarsCover, skyManager.StarsIntensity, Time.deltaTime));
            }




            if (temporalSunMoon == null)//if (Application.isPlaying && temporalSunMoon == null)
            {
                temporalSunMoon = new GameObject();
                temporalSunMoon.name = "SunMoonTransformLerp";
                temporalSunMoon.transform.rotation = skyManager.SUN_LIGHT.transform.rotation;
                temporalSunMoon.transform.position = skyManager.SUN_LIGHT.transform.position;
                Light temp = temporalSunMoon.AddComponent<Light>();
                temp = fullVolumeClouds.sunLight;
                fullVolumeClouds.sunLight = temporalSunMoon.GetComponent<Light>();
            }

            //Play mode only
            if (Application.isPlaying)
            {
                //set mooon based on time of day - X = vertical (minus goes up, 180 to 360), Y = horizontal
                //          Vector4 moonSettings = skyDome.m_skyMaterial.GetVector("_moonPos");
                //         skyDome.m_skyMaterial.SetVector("_moonPos", new Vector4(270 + Mathf.Cos(skyManager.Current_Time * 0.2f) * 90,0.1f* (Time.fixedTime % 360), moonSettings.z, moonSettings.w));
            }

            //v0.1
            if (shaderVolClouds != null)
            {
                //Pass time from sky master
                shaderVolClouds.Current_Time = skyManager.Current_Time;
                shaderVolClouds.WeatherSeverity = skyManager.WeatherSeverity;
                shaderVolClouds.WeatherDensity = true;
            }


            //set clouds based on weather
            //set cloud density at game start based on weather
            if (Application.isPlaying)
            {
                setFullVolumeCloudsDensity(true);
            }
            else
            {
                setFullVolumeCloudsDensity(false);
            }
            setFullVolumeCloudsColors();

            //apply a set of predefined densities
            if (ApplyPresetDensities)
            {
                applyPresetDens();
                ApplyPresetDensities = false;
            }
        }

        public void applyPresetDens()
        {

            //Full volume clouds with fly through, fog of war - planetary - vortex cloud options
            weatherCloudDensities.Clear();

            if (cloudType == 0)
            {
                weatherCloudDensities.Add(0);
                weatherCloudDensities.Add(0.4f);
                weatherCloudDensities.Add(1.2f);
                weatherCloudDensities.Add(1.4f);//tornado (to do - change material here to faciliate vortex)
                weatherCloudDensities.Add(1.5f);
                weatherCloudDensities.Add(1.5f);
                weatherCloudDensities.Add(1.1f);//flat clouds - also regulate darness density
                weatherCloudDensities.Add(2f);//lightning
                weatherCloudDensities.Add(2f);//heavy storm - also regulate darkness density
                weatherCloudDensities.Add(2f);
                weatherCloudDensities.Add(1f);//CLOUDY
                weatherCloudDensities.Add(0.4f);
                weatherCloudDensities.Add(0.4f);
                weatherCloudDensities.Add(0.9f);
            }
        }

        //0 Sunny
        //1 Foggy
        //2 Heavy fog
        //3 tornado
        //4 snow storm
        //5 freeze storm
        //6 flat clouds
        //7 lightning storm
        //8 heavy storm
        //9 heavy storm dark
        //10 cloudy
        //11 rolling fog
        //12 volcano erupt
        //13 Rain
        public float cloudTransitionSpeed = 1;
        public void setFullVolumeCloudsDensity(bool lerp)
        {
            if (weatherCloudDensities.Count < 14)
            {
                applyPresetDens();//fill with preset if nothing is declared 
            }

            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.Sunny)//0
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[0] + densitiesOffset, 0, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.Foggy)//1
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[1] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyFog)//2
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[2] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.Tornado)//3
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[3] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.SnowStorm)//4
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[4] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.FreezeStorm)//5
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[5] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.FlatClouds)//6
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[6] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStorm)//8
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[8] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStormDark)//9
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[9] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.RollingFog)//11
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[11] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.VolcanoErupt)//12
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[12] + densitiesOffset, 1, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.Rain)//13
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[13] + densitiesOffset, 1, lerp);
            }

            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.Cloudy)//10
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[10] + densitiesOffset, 1.32f, lerp);
            }
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.LightningStorm)//7
            {
                setFullVolumeCloudsDensityA(weatherCloudDensities[7] + densitiesOffset, 1.65f, lerp);
                //fullVolumeClouds.lightning.enable
                lightningController.EnableLightning = true;
            }
            else
            {
                lightningController.EnableLightning = false;
            }
        }
        public void setFullVolumeCloudsDensityA(float density, float darkness, bool lerp)
        {
            if (lerp)
            {
                fullVolumeClouds.coverage = Mathf.Lerp(fullVolumeClouds.coverage, density, cloudTransitionSpeed * Time.deltaTime * 0.05f);
                fullVolumeClouds.density = Mathf.Lerp(fullVolumeClouds.density, darkness, cloudTransitionSpeed * Time.deltaTime * 0.05f);

                //v0.2
                fullVolumeClouds._NoiseAmp1 = Mathf.Lerp(fullVolumeClouds._NoiseAmp1, density, cloudTransitionSpeed * Time.deltaTime * 0.05f);
                fullVolumeClouds._NoiseBias = Mathf.Lerp(fullVolumeClouds._NoiseBias, darkness, cloudTransitionSpeed * Time.deltaTime * 0.05f);
            }
            else
            {
                fullVolumeClouds.coverage = density;
                fullVolumeClouds.density = darkness;

                //v0.2
                fullVolumeClouds._NoiseAmp1 = density;
                fullVolumeClouds._NoiseBias = darkness;
            }
        }

        //Set cloud colors
        public void setFullVolumeCloudsColors()
        {
            setFullVolumeCloudsColorsA();
        }
        public Color cloudColor;
        public Color cloudsAmbientColor;
        public float cloudColorSpeed = 1;
        public float cloudColorSpeedD = 1;
        public float cloudColorSpeedN = 1;
        public Color Day_Sun_Color = new Color(0.933f, 0.933f, 0.933f, 1);
        public Color Day_Ambient_Color = new Color(0.4f, 0.4f, 0.4f, 1);//
        public Color Day_Tint_Color = new Color(0.2f, 0.2f, 0.2f, 1);//
        public Color Dusk_Sun_Color = new Color(0.933f, 0.596f, 0.443f, 1);
        public Color Dusk_Ambient_Color = new Color(0.2f, 0.2f, 0.2f, 0);
        public Color Dusk_Tint_Color = new Color(0.386f, 0, 0, 0);
        public Color Dawn_Sun_Color = new Color(0.933f, 0.933f, 0.933f, 1);
        public Color Dawn_Ambient_Color = new Color(0.35f, 0.35f, 0.35f, 1);//
        public Color Dawn_Tint_Color = new Color(0.2f, 0.05f, 0.1f, 0);//
        public Color Night_Sun_Color = new Color(0.01f, 0.01f, 0.01f, 1);
        public Color Night_Ambient_Color = new Color(0.03f, 0.03f, 0.03f, 0);
        public Color Night_Tint_Color = new Color(0, 0, 0, 0);
        public Color cloudStormColor = new Color(0.13f, 0.12f, 0.11f, 0);
        public void setFullVolumeCloudsColorsA()
        {
            if (skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStorm
                || skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStormDark
                || skyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.LightningStorm
                )
            {
                if (Application.isPlaying)
                {
                    cloudColor = Color.Lerp(cloudColor, cloudStormColor, 0.05f * Time.deltaTime * cloudColorSpeed);
                }
                else
                {
                    if (cloudColor != cloudStormColor)
                    {
                        cloudColor = cloudStormColor;
                    }
                }
            }
            else
            {
                //v3.0
                if (// IF DAY TIME
                    (!skyManager.AutoSunPosition && (skyManager.Current_Time < (18 + skyManager.Shift_dawn)
                    && skyManager.Current_Time > (9 + skyManager.Shift_dawn)))
                        ||
                    (skyManager.AutoSunPosition && skyManager.Rot_Sun_X > 0)
                )
                {
                    //if(fullVolumeClouds.sunLight.transform != skyManager.SUN_LIGHT.transform)
                    //{
                    //    fullVolumeClouds.sunLight = skyManager.SUN_LIGHT.GetComponent<Light>();
                    //}
                    if (temporalSunMoon != null)
                    {
                        temporalSunMoon.transform.rotation = Quaternion.Lerp(temporalSunMoon.transform.rotation, skyManager.SUN_LIGHT.transform.rotation, cloudColorSpeedD * 0.1f * Time.deltaTime);
                        temporalSunMoon.transform.position = Vector3.Lerp(temporalSunMoon.transform.position, skyManager.SUN_LIGHT.transform.position, cloudColorSpeedD * 0.1f * Time.deltaTime);
                    }

                    if ((skyManager.AutoSunPosition && skyManager.Rot_Sun_X < 25)
                        || (!skyManager.AutoSunPosition && skyManager.Current_Time > (17 + skyManager.Shift_dawn)))
                    { //v3.0

                        float UP_rate = 11f;

                        if (Application.isPlaying)
                        {
                            cloudColor = Color.Lerp(cloudColor, Dusk_Sun_Color, 0.05f * UP_rate * Time.deltaTime * cloudColorSpeed);
                            cloudsAmbientColor = Color.Lerp(cloudsAmbientColor, Dusk_Ambient_Color, 0.05f * UP_rate * Time.deltaTime);//v1.2.1
                        }
                        else
                        {
                            if (cloudColor != Dusk_Sun_Color)
                            {
                                cloudColor = Dusk_Sun_Color;
                            }
                            if (cloudsAmbientColor != Dusk_Ambient_Color)
                            {
                                cloudsAmbientColor = Dusk_Ambient_Color;//v1.5
                            }
                        }
                    }
                    else if ((skyManager.AutoSunPosition && skyManager.Rot_Sun_X < 65) ||
                        (!skyManager.AutoSunPosition && skyManager.Current_Time > (12 + skyManager.Shift_dawn)))
                    {
                        //raise to max
                        float UP_rate = 3;
                        UP_rate = 0.7f;

                        if (Application.isPlaying)
                        {
                            cloudColor = Color.Lerp(cloudColor, Day_Sun_Color, 0.05f * UP_rate * Time.deltaTime * cloudColorSpeed);
                            cloudsAmbientColor = Color.Lerp(cloudsAmbientColor, Day_Ambient_Color, 0.5f * UP_rate * Time.deltaTime);//v1.2.1
                        }
                        else
                        {
                            if (cloudColor != Day_Sun_Color)
                            {
                                cloudColor = Day_Sun_Color;
                            }
                            if (cloudsAmbientColor != Day_Ambient_Color)
                            {
                                cloudsAmbientColor = Day_Ambient_Color;//v1.5
                            }
                        }
                    }
                    else
                    {
                        //raise to max
                        float UP_rate = 3;

                        if (Application.isPlaying)
                        {
                            cloudColor = Color.Lerp(cloudColor, Dawn_Sun_Color, 0.05f * UP_rate * Time.deltaTime * cloudColorSpeed);
                            cloudsAmbientColor = Color.Lerp(cloudsAmbientColor, Dawn_Ambient_Color, 0.5f * UP_rate * Time.deltaTime);//v1.2.1
                        }
                        else
                        {
                            if (cloudColor != Dawn_Sun_Color)
                            {
                                cloudColor = Dawn_Sun_Color;
                            }
                            if (cloudsAmbientColor != Dawn_Ambient_Color)
                            {
                                cloudsAmbientColor = Dawn_Ambient_Color;//v1.5
                            }
                        }
                    }
                }
                else if ((skyManager.AutoSunPosition && skyManager.Rot_Sun_X < 15)//NIGHT TIME
                    || (!skyManager.AutoSunPosition && skyManager.Current_Time <= (9 + skyManager.Shift_dawn)
                    && skyManager.Current_Time > (1 + skyManager.Shift_dawn)))
                {
                    float UP_rate = 0.5f;
                    //Debug.Log("Night Time " + fullVolumeClouds.SunLight.gameObject.name +"," + skyManager.MOON_LIGHT.name);
                    //if (fullVolumeClouds.sunLight.gameObject != skyManager.MOON_LIGHT)
                    //{
                    //    fullVolumeClouds.sunLight = skyManager.MOON_LIGHT.GetComponent<Light>();
                    //    //Debug.Log("Night Time Assign Loon Light");
                    //}
                    if (temporalSunMoon != null)
                    {
                        temporalSunMoon.transform.rotation = Quaternion.Lerp(temporalSunMoon.transform.rotation, skyManager.MOON_LIGHT.transform.rotation, cloudColorSpeedD * 0.1f * Time.deltaTime);
                        temporalSunMoon.transform.position = Vector3.Lerp(temporalSunMoon.transform.position, skyManager.MOON_LIGHT.transform.position, cloudColorSpeedD * 0.1f * Time.deltaTime);
                    }

                    //v2.0.1
                    if ((skyManager.AutoSunPosition && skyManager.Rot_Sun_X < 5)
                        || (!skyManager.AutoSunPosition && skyManager.Current_Time < (8.7f + skyManager.Shift_dawn)))
                    {
                        if (Application.isPlaying)
                        {
                            cloudColor = Color.Lerp(cloudColor, Night_Sun_Color, 1.5f * UP_rate * Time.deltaTime * cloudColorSpeed);
                        }
                        else
                        {
                            if (cloudColor != Night_Sun_Color)
                            {
                                cloudColor = Night_Sun_Color;
                            }
                        }
                    }
                    else
                    {
                        if (Application.isPlaying)
                        {
                            cloudColor = Color.Lerp(cloudColor, Dawn_Sun_Color, UP_rate * Time.deltaTime * cloudColorSpeed);
                        }
                        else
                        {
                            if (cloudColor != Dawn_Sun_Color)
                            {
                                cloudColor = Dawn_Sun_Color;
                            }
                        }
                    }
                    if (Application.isPlaying)
                    {
                        cloudsAmbientColor = Color.Lerp(cloudsAmbientColor, Night_Ambient_Color, 2f * UP_rate * Time.deltaTime);//v1.2.1
                    }
                    else
                    {
                        if (cloudsAmbientColor != Night_Ambient_Color)
                        {
                            cloudsAmbientColor = Night_Ambient_Color;//v1.5
                        }
                    }
                }
                else
                { //18 evening up to 1 at night 
                  //if (fullVolumeClouds.sunLight.transform != skyManager.SUN_LIGHT.transform)
                  //{
                  //    fullVolumeClouds.sunLight = skyManager.SUN_LIGHT.GetComponent<Light>();
                  //}


                    float UP_rate = 0.5f;
                    //GLOBAL TINT
                    if ((skyManager.AutoSunPosition && skyManager.Rot_Sun_X < 0)
                        || (!skyManager.AutoSunPosition && skyManager.Current_Time > (22 + skyManager.Shift_dawn)
                        || (skyManager.Current_Time >= 0 & skyManager.Current_Time <= (1 + skyManager.Shift_dawn))))
                    {//v2.0.1

                        if (Application.isPlaying)
                        {
                            cloudColor = Color.Lerp(cloudColor, Night_Sun_Color, 1.5f * UP_rate * Time.deltaTime * cloudColorSpeed); //v2.0.1
                            cloudsAmbientColor = Color.Lerp(cloudsAmbientColor, Night_Ambient_Color, 2f * UP_rate * Time.deltaTime);//v1.2.1
                        }
                        else
                        {
                            if (cloudColor != Night_Sun_Color)
                            {
                                cloudColor = Night_Sun_Color;
                            }
                            if (cloudsAmbientColor != Night_Ambient_Color)
                            {
                                cloudsAmbientColor = Night_Ambient_Color;//v1.5
                            }
                        }

                        if (skyManager.Current_Time >= 0.45f)
                        {
                            temporalSunMoon.transform.rotation = Quaternion.Lerp(temporalSunMoon.transform.rotation, skyManager.MOON_LIGHT.transform.rotation, cloudColorSpeedN * 0.1f * Time.deltaTime);
                            temporalSunMoon.transform.position = Vector3.Lerp(temporalSunMoon.transform.position, skyManager.MOON_LIGHT.transform.position, cloudColorSpeedN * 0.1f * Time.deltaTime);
                        }
                        else
                        {
                            temporalSunMoon.transform.rotation = Quaternion.Lerp(temporalSunMoon.transform.rotation, skyManager.SUN_LIGHT.transform.rotation, cloudColorSpeedN * 0.1f * Time.deltaTime);
                            temporalSunMoon.transform.position = Vector3.Lerp(temporalSunMoon.transform.position, skyManager.SUN_LIGHT.transform.position, cloudColorSpeedN * 0.1f * Time.deltaTime);
                        }

                    }
                    else
                    {

                        if (temporalSunMoon != null)
                        {
                            temporalSunMoon.transform.rotation = Quaternion.Lerp(temporalSunMoon.transform.rotation, skyManager.SUN_LIGHT.transform.rotation, cloudColorSpeedD * 0.1f * Time.deltaTime);
                            temporalSunMoon.transform.position = Vector3.Lerp(temporalSunMoon.transform.position, skyManager.SUN_LIGHT.transform.position, cloudColorSpeedD * 0.1f * Time.deltaTime);
                        }

                        if (Application.isPlaying)
                        {
                            cloudColor = Color.Lerp(cloudColor, Dusk_Sun_Color, 0.5f * UP_rate * Time.deltaTime * cloudColorSpeed);
                            cloudsAmbientColor = Color.Lerp(cloudsAmbientColor, Dusk_Ambient_Color, 0.5f * UP_rate * Time.deltaTime);//v1.2.1
                        }
                        else
                        {
                            if (cloudColor != Dusk_Sun_Color)
                            {
                                cloudColor = Dusk_Sun_Color;
                            }
                            if (cloudsAmbientColor != Dusk_Ambient_Color)
                            {
                                cloudsAmbientColor = Dusk_Ambient_Color;//v1.5
                            }
                        }
                    }
                }//END COLOR SET
            }//END STORM CHECK

            if (fullVolumeClouds != null)
            {
                fullVolumeClouds.highSunColor = cloudColor;
                fullVolumeClouds.cloudBaseColor = cloudsAmbientColor;
                fullVolumeClouds.sunColor = cloudColor;

                //v0.2
                fullVolumeClouds._GroundColor = new Vector3(cloudsAmbientColor.r, cloudsAmbientColor.g, cloudsAmbientColor.b);
                fullVolumeClouds._SkyTint = new Vector3(cloudColor.r, cloudColor.g, cloudColor.b);
            }
        }//END SET CLOUD COLOR Function
    }
}