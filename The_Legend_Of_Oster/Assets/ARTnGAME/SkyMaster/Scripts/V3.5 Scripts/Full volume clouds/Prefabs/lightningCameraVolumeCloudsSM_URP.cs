using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artngame.SKYMASTER
{
    public class lightningCameraVolumeCloudsSM_URP : MonoBehaviour
    {
        public bool useSecondLightning = false;
        public GameObject LightningBoxPrefab;
        public bool setupLightning = false;
        public bool weatherTransition = true;
        public SkyMasterManager SkyManager;

        //public Light localLight; //find local light in camera volume clouds script       
        public connectSuntoFullVolumeCloudsURP fullvolumeCloudsScript;
        public connectSuntoVolumeCloudsURP volumeCloudsScript;

        // Start is called before the first frame update
        void Start()
        {
            setupLightning = true;
        }

        // Update is called once per frame
        void Update()
        {
            createLightningBox();

            if (Application.isPlaying)
            {
                LightningUpdate();
            }



            if (weatherTransition)
            {
                if (SkyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.LightningStorm)
                {
                    
                    EnableLightning = true;
                }
                else
                {
                    EnableLightning = false;
                }
            }
        }

        public void createLightningBox()
        {
            if (setupLightning)
            {
                if (LightningBox == null)
                {
                    GameObject lightningDome = Instantiate(LightningBoxPrefab);
                    LightningBox = lightningDome.transform;
                }
                setupLightning = false;
            }
        }


        //v2.1.24 - lightning
        public GameObject LightningPrefab; //Prefab to instantiate for lighting, use only 1-2 prefabs and move them around
        public bool EnableLightning = false;
        public bool useLocalLightLightn = false;
        float last_lightning_time = 0;
        public float lightning_every = 15;
        public float max_lightning_time = 2;
        public float lightning_rate_offset = 5;
        Transform LightningOne;
        Transform LightningTwo;
        public Transform LightningBox;
        Light LightA;
        Light LightB; //keep lights here and update them in the script local light as needed
        public void LightningUpdate()
        {

            if (Application.isPlaying)
            {
                if (EnableLightning)
                {
                    if (LightningOne == null)
                    {
                        LightningOne = Instantiate(LightningPrefab).transform;
                        LightA = LightningOne.GetComponentInChildren<ChainLightning_SKYMASTER>().startLight;
                        LightningOne.gameObject.SetActive(false);
                    }
                    if (LightningTwo == null)
                    {
                        LightningTwo = Instantiate(LightningPrefab).transform;
                        LightB = LightningTwo.GetComponentInChildren<ChainLightning_SKYMASTER>().startLight;//v4.0 - 2.1.24 IG
                        LightningTwo.gameObject.SetActive(false);
                    }

                    //move around
                    if (LightningBox != null)
                    {
                        if (Time.fixedTime - last_lightning_time > lightning_every - UnityEngine.Random.Range(-lightning_rate_offset, lightning_rate_offset))
                        {

                            Vector2 MinMaXLRangeX = LightningBox.position.x * Vector2.one + (LightningBox.localScale.x / 2) * new Vector2(-1, 1);
                            Vector2 MinMaXLRangeY = LightningBox.position.y * Vector2.one + (LightningBox.localScale.y / 2) * new Vector2(-1, 1);
                            Vector2 MinMaXLRangeZ = LightningBox.position.z * Vector2.one + (LightningBox.localScale.z / 2) * new Vector2(-1, 1);

                            LightningOne.position = new Vector3(UnityEngine.Random.Range(MinMaXLRangeX.x, MinMaXLRangeX.y),
                                UnityEngine.Random.Range(MinMaXLRangeY.x, MinMaXLRangeY.y), UnityEngine.Random.Range(MinMaXLRangeZ.x, MinMaXLRangeZ.y));
                            if (UnityEngine.Random.Range(0, SkyManager.WeatherSeverity + 1) == 1)
                            {
                                //do nothing
                            }
                            else
                            {
                                LightningOne.gameObject.SetActive(true);
                                //localLight = LightA;
                                if(fullvolumeCloudsScript != null)
                                {
                                    fullvolumeCloudsScript.localLight = LightA;
                                }
                                if (volumeCloudsScript != null)
                                {
                                    volumeCloudsScript.localLight = LightA;
                                }
                            }

                            if (useSecondLightning)
                            {
                                LightningTwo.position = new Vector3(UnityEngine.Random.Range(MinMaXLRangeX.x, MinMaXLRangeX.y),
                                    UnityEngine.Random.Range(MinMaXLRangeY.x, MinMaXLRangeY.y), UnityEngine.Random.Range(MinMaXLRangeZ.x, MinMaXLRangeZ.y));
                                if (UnityEngine.Random.Range(0, SkyManager.WeatherSeverity + 1) == 1)
                                {
                                    //do nothing
                                }
                                else
                                {
                                    LightningTwo.gameObject.SetActive(true);
                                    // localLight = LightB;
                                    if (fullvolumeCloudsScript != null)
                                    {
                                        fullvolumeCloudsScript.localLight = LightB;
                                    }
                                    if (volumeCloudsScript != null)
                                    {
                                        volumeCloudsScript.localLight = LightB;
                                    }
                                }
                            }

                            last_lightning_time = Time.fixedTime;
                        }
                        else
                        {
                            if (Time.fixedTime - last_lightning_time > max_lightning_time)
                            {
                                if (LightningOne != null)
                                {
                                    if (LightningOne.gameObject.activeInHierarchy)
                                    {
                                        LightningOne.gameObject.SetActive(false);

                                        //localLight = null; //v4.0
                                        if (fullvolumeCloudsScript != null)
                                        {
                                            fullvolumeCloudsScript.localLight = null;
                                        }
                                        if (volumeCloudsScript != null)
                                        {
                                            volumeCloudsScript.localLight = null;
                                        }
                                    }
                                }
                                if (LightningTwo != null)
                                {
                                    if (LightningTwo.gameObject.activeInHierarchy)
                                    {
                                        LightningTwo.gameObject.SetActive(false);

                                        //localLight = null; //v4.0
                                        if (fullvolumeCloudsScript != null)
                                        {
                                            fullvolumeCloudsScript.localLight = null;
                                        }
                                        if (volumeCloudsScript != null)
                                        {
                                            volumeCloudsScript.localLight = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (LightningOne != null)
                    {
                        if (LightningOne.gameObject.activeInHierarchy)
                        {
                            LightningOne.gameObject.SetActive(false);
                            //localLight = null; //v4.0
                            if (fullvolumeCloudsScript != null)
                            {
                                fullvolumeCloudsScript.localLight = null;
                            }
                            if (volumeCloudsScript != null)
                            {
                                volumeCloudsScript.localLight = null;
                            }
                        }
                    }
                    if (LightningTwo != null)
                    {
                        if (LightningTwo.gameObject.activeInHierarchy)
                        {
                            LightningTwo.gameObject.SetActive(false);
                            //localLight = null; //v4.0
                            if (fullvolumeCloudsScript != null)
                            {
                                fullvolumeCloudsScript.localLight = null;
                            }
                            if (volumeCloudsScript != null)
                            {
                                volumeCloudsScript.localLight = null;
                            }
                        }
                    }
                }
            }

        }

    }
}