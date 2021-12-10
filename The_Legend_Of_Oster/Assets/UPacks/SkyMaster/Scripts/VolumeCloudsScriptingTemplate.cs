using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artngame.SKYMASTER
{
    public class VolumeCloudsScriptingTemplate : MonoBehaviour
    {
        public CloudHandlerSM_SRP cloudScript;
        public bool affectTimeOfDay = true;
        public bool changeCloudCoverage = false;
        public bool changeWeather = true;
        public bool changeCloudSpeed = true;
        public bool changeCloudScaling = true;

        public float updateSunEvery = 30; //move sun every 30 frames
        public float effectsRate = 0.1f;
        float lastWeatherChangeTime = 0;
        public float weatherStayTime = 16;

        int frameCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            frameCount = 0;
        }

        // Update is called once per frame
        void Update()
        {

            //CHANGE CLOUD SCALE IN SHADER
            if (changeCloudScaling)
            {
                cloudScript.CloudDensity = 0.00024f + 0.00022f * Mathf.Sin(Time.fixedTime * effectsRate * 0.42f);
            }

            //CHANGE CLOUD SPEED
            if (changeCloudSpeed)
            {
                cloudScript.WindStrength = 10 + 10 * Mathf.Sin(Time.fixedTime * effectsRate * 0.1f);
                cloudScript.WindParallaxFactor = 1.5f + Mathf.Abs(3 * Mathf.Sin(Time.fixedTime * effectsRate * 0.2f));
            }

            //CHANGE CLOUD COVERAGE AMOUNT
            if (changeCloudCoverage)
            {
                cloudScript.Coverage = 0.03f - 0.16f * Mathf.Abs(Mathf.Cos(Time.fixedTime * effectsRate));
            }

            //CHANGE WEATHER
            frameCount++;
            if (affectTimeOfDay && frameCount > updateSunEvery)
            {
                frameCount = 0;
                //Rotate directional light -12 to 188 and adapt time of day accordingly
                Vector3 angles = cloudScript.Sun.transform.eulerAngles;
                cloudScript.Sun.transform.eulerAngles = new Vector3((-12+200/2) - 100 * Mathf.Sin(Time.fixedTime * effectsRate), -20 + 1*40 * Mathf.Cos(Time.fixedTime * effectsRate * 4), 89);
                cloudScript.Current_Time = 14 - 8 * Mathf.Sin(Time.fixedTime * effectsRate);

                //cloudScript.Sun.transform.eulerAngles = new Vector3((-12 + 200 / 2) + 100 * Mathf.Cos(Time.fixedTime * effectsRate), -61, 89);
            }
            if (changeWeather)
            {
                cloudScript.WeatherDensity = true;

                if (Time.fixedTime - lastWeatherChangeTime > weatherStayTime)
                {
                    if(cloudScript.cloudType == CloudHandlerSM_SRP.CloudPreset.ClearDay)
                    {
                        cloudScript.cloudType = CloudHandlerSM_SRP.CloudPreset.Storm;

                        //set lighting higher to get a lit clouds effect - optional
                        cloudScript.IntensityDiffOffset = 15;
                        cloudScript.coverageSpeed = 30;

                        lastWeatherChangeTime = Time.fixedTime;
                    }
                    else if (cloudScript.cloudType == CloudHandlerSM_SRP.CloudPreset.Storm)
                    {
                        cloudScript.cloudType = CloudHandlerSM_SRP.CloudPreset.ClearDay;

                        //restore lighting
                        cloudScript.IntensityDiffOffset = -0.85f;
                        cloudScript.coverageSpeed = 1;

                        lastWeatherChangeTime = Time.fixedTime; //storm stay less

                        //randomize lighnting rate
                        cloudScript.lightning_every = Random.Range(1, 4);
                    }                   
                }
            }
            else
            {
                cloudScript.WeatherDensity = false;
            }
            //END CHANGE WEATHER
        }
    }
}
