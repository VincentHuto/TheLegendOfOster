using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using ORKFramework;

namespace Artngame.SKYMASTER {

    [System.Serializable]
    public class WeatherEventSM {

        public SkyMasterManager SkyManager;
        public enum Volume_Weather_event_types { Sunny, Foggy, HeavyFog, Tornado, SnowStorm, FreezeStorm, FlatClouds, LightningStorm, HeavyStorm, HeavyStormDark, Cloudy, RollingFog, VolcanoErupt, Rain };
        //public enum Volume_Weather_event_types {Sunny, Foggy, SnowStorm, HeavyStorm, Cloudy, Rain};
        public Volume_Weather_event_types Weather_type;
        public GameObject VolumeCloudsPREFAB;//replacement for default clouds of weather type
        public int EventStartMonth;
        public int EventEndMonth;
        public int EventStartDay;//which day in the start month
        public int EventEndDay;
        public float EventStartHour;
        public float EventEndHour;
        public bool is_activated;
        public bool loop;
        public float Chance;
        public bool concurrent;
        public float VolCloudsHorScale;
        public float VolCloudHeight;
        public int seed;
        public Volume_Weather_event_types FollowUpWeather;// what will follow after the event is over
    }

    //v2.2 - New weather states system, define state when Volume_Weather_Type changes, with particle clouds, snow materials etc per type & season and transition in a coroutine
    [System.Serializable]
    public class WeatherSM {

        //CONTROLS
        public SkyMasterManager SkyManager;
        public enum Volume_Weather_State { Init, FadeIn, Steady, FadeOut }
        public Volume_Weather_State currentState = Volume_Weather_State.Init;//start at initialization in by default
        public int Season = 0;//spring by default
        public bool FadedOut;
        public bool FadingOut;
        public float StartTime;//time of weather front creation
        public float FadeInTime = 10;// max time fade in lasts
        public int seed;
        public float FadeOutTime = 10;
        public float state_start_time;

        //MATERIALS
        public Material CloudDomeL1Mat;//shader based cloud dome material - layer1
        public Material CloudDomeL2Mat;//shader based cloud dome material - layer2 - additive add to layer 1 for special FX and contrast
        public Material SnowMat;
        public Material SnowMatTerrain;

        //WIND
        public WindZoneMode windMode = WindZoneMode.Directional;
        //public float WindForce;
        //public Transform WindDirection;//can be changed in real time

        //RAIN - SNOW - ICE PARTICLES
        public List<Transform> SnowParticle = new List<Transform>();
        public List<ParticleSystem[]> SnowParticleP = new List<ParticleSystem[]>();
        public List<Transform> RainParticle = new List<Transform>();
        public List<ParticleSystem[]> RainParticleP = new List<ParticleSystem[]>();
        public List<Transform> RefractRainParticle = new List<Transform>();
        public List<ParticleSystem[]> RefractRainParticleP = new List<ParticleSystem[]>();

        public List<int> RainParticleRate = new List<int>();
        public List<int> RainParticleMax = new List<int>();
        public List<bool> RainParticleReset = new List<bool>();
        public List<float> RainTransp = new List<float>();

        public List<int> SnowParticleRate = new List<int>();
        public List<int> SnowParticleMax = new List<int>();
        public List<bool> SnowParticleReset = new List<bool>();
        public List<float> SnowTransp = new List<float>();
        //public Transform RefractRainParticle;
        //public ParticleSystem RefractRainParticleP;

        //CLOUD PARTICLES
        public List<Transform> ParticleClouds = new List<Transform>();
        public List<ParticleSystem[]> ParticlesCloudsP = new List<ParticleSystem[]>();//fill this if more than one particle systems are involved
        public List<int> CloudParticleRate = new List<int>();
        public List<int> CloudParticleMax = new List<int>();
        public List<bool> CloudParticleReset = new List<bool>();
        public List<float> CloudTranspMax = new List<float>();
        //public List<ParticleSystem> ParticleCloudsP = new List<ParticleSystem>();

        //VOLUME CLOUD SYSTEMS
        public GameObject VolumeCloud;
        public VolumeClouds_SM VolumeScript;
        //public ParticleEmitter VolumeCloudemitter;//v3.4.6

        //OPTIONS
        public bool Snow;
        //public float SnowCoverage;
        //public float SnowCoverageRate;
        //public float MaxSnowCoverage;
        public string SnowCoverageVariable;//global shader variable to change snow with setglobal
        public bool Rain;
        public bool Refractive_Rain;//
        public bool Volume_Rain;

        public bool has_fog;//Unity fog
        public bool has_volume_fog;//image effect fog 
        public int volume_fog_peset;//set SeasonalTerrainSKYMASTER script volume fog preset

        //v3.4
        public float prevSeverity;

        public void Update() {

            if (SkyManager != null) {
                if (currentState == Volume_Weather_State.Init) {

                    //v3.4
                    prevSeverity = SkyManager.WeatherSeverity;

                    //Start volumetric clouds - disabled by default for performance
                    if (VolumeCloud != null) { //&& !VolumeCloud.activeInHierarchy) {
                        VolumeCloud.SetActive(true);
                        //VolumeScript = VolumeCloud.GetComponent<VolumeClouds_SM> ();
                        VolumeScript = VolumeCloud.GetComponentsInChildren<VolumeClouds_SM>(true)[0];
                        //Debug.Log("name1="+VolumeScript.gameObject.name);

                        //start cloud fade in
                        VolumeScript.SmoothIn = true;
                        VolumeScript.FadeOutOnBoundary = true;
                        VolumeScript.destroy_on_end = true;
                        VolumeScript.clone_on_end = true;

                        //define wind
                        if (SkyManager.windZone != null) {
                            VolumeScript.Wind_holder = SkyManager.windZone.gameObject;
                            VolumeScript.wind = SkyManager.windZone.transform.forward * SkyManager.windZone.windMain * SkyManager.AmplifyWind;//get current wind
                        }
                    }
                    //move to next state
                    currentState = Volume_Weather_State.FadeIn;
                    state_start_time = Time.fixedTime;
                } else {
                    //assign new wind direction if it changes
                    if (SkyManager.windZone != null && VolumeScript != null) {
                        VolumeScript.wind = SkyManager.windZone.transform.forward * SkyManager.windZone.windMain * SkyManager.AmplifyWind;
                    }
                }

                if (currentState == Volume_Weather_State.FadeIn) {
                    for (int i = 0; i < ParticleClouds.Count; i++) {
                        Make_Particles_Appear(ParticleClouds[i].gameObject, ParticlesCloudsP[i], null, CloudParticleMax[i], CloudParticleReset[i], CloudParticleRate[i], CloudTranspMax[i]);
                    }
                    if (Snow) {
                        for (int i = 0; i < SnowParticle.Count; i++) {
                            Make_Particles_Appear(SnowParticle[i].gameObject, SnowParticleP[i], null, (int)(SkyManager.Snow_max_part * SkyManager.WeatherSeverity / 10), true, 10, 1);//v3.4
                        }
                        //if(SnowCoverageVariable < MaxSnowCoverage){
                        //if(SnowMat.HasProperty(SnowCoverageVariable) && SnowMat.GetFloat(SnowCoverageVariable) < MaxSnowCoverage){
                        if (SkyManager.SnowCoverage < SkyManager.MaxSnowCoverage) {
                            //SnowCoverageVariable += SnowCoverageRate*Time.deltaTime;
                            SkyManager.SnowCoverage += SkyManager.SnowCoverageRate * Time.deltaTime;
                            Shader.SetGlobalFloat(SnowCoverageVariable, SkyManager.SnowCoverage);
                        }
                        if (SkyManager.SnowCoverageTerrain < SkyManager.MaxSnowCoverage) {
                            SkyManager.SnowCoverageTerrain += SkyManager.SnowTerrainRate * Time.deltaTime * SkyManager.SnowTerrRateFactor;
                            if (SnowMatTerrain != null) {
                                SnowMatTerrain.SetFloat(SnowCoverageVariable, SkyManager.SnowCoverageTerrain / SkyManager.divideSnowTerrain);
                            }
                        }
                    } else {
                        for (int i = 0; i < SnowParticle.Count; i++) {
                            Make_Particles_Dissappear(SnowParticle[i].gameObject, SnowParticleP[i], null, 20, 10, false, true);
                        }
                        if (SkyManager.SnowCoverage > 0) {
                            SkyManager.SnowCoverage -= SkyManager.SnowCoverageRate * Time.deltaTime;
                            Shader.SetGlobalFloat(SnowCoverageVariable, SkyManager.SnowCoverage);
                            //						if(!Application.isPlaying){
                            //							Shader.SetGlobalFloat(SnowCoverageVariable, 0);
                            //						}
                        }
                        if (SkyManager.SnowCoverageTerrain > 0) {
                            SkyManager.SnowCoverageTerrain -= SkyManager.SnowTerrainRate * Time.deltaTime * SkyManager.SnowTerrRateFactor;
                            if (SnowMatTerrain != null) {
                                SnowMatTerrain.SetFloat(SnowCoverageVariable, SkyManager.SnowCoverageTerrain / SkyManager.divideSnowTerrain);
                            }
                        }
                    }//END SNOW
                    if (Rain) {
                        if (Refractive_Rain) {
                            for (int i = 0; i < RefractRainParticle.Count; i++) {
                                //v3.4
                                if (i < RefractRainParticleP.Count) {
                                    Make_Particles_Appear(RefractRainParticle[i].gameObject, RefractRainParticleP[i], null, (int)(SkyManager.Refract_Rain_max_part * SkyManager.WeatherSeverity / 10), true, 12, 1);//v3.2 - v3.4 severity
                                }
                            }
                        } else {
                            for (int i = 0; i < RainParticle.Count; i++) {
                                //v3.4
                                if (i < RainParticleP.Count) {
                                    Make_Particles_Appear(RainParticle[i].gameObject, RainParticleP[i], null, (int)(SkyManager.Refract_Rain_max_part * SkyManager.WeatherSeverity / 10), true, 10, 1);//v3.4
                                }
                            }
                        }
                    } else {
                        for (int i = 0; i < RefractRainParticle.Count; i++) {
                            //v3.4
                            if (i < RefractRainParticleP.Count) {
                                Make_Particles_Dissappear(RefractRainParticle[i].gameObject, RefractRainParticleP[i], null, 20, 10, false, true);
                            }
                        }
                        for (int i = 0; i < RainParticle.Count; i++) {
                            //v3.4
                            if (i < RainParticleP.Count) {
                                Make_Particles_Dissappear(RainParticle[i].gameObject, RainParticleP[i], null, 20, 10, false, true);
                            }
                        }
                    }

                    //TIMER
                    if (FadeInTime != 0) {
                        if (Time.fixedTime - state_start_time > FadeInTime) {
                            currentState = Volume_Weather_State.Steady;
                            state_start_time = Time.fixedTime;

                            FadedOut = false;
                        }
                    }

                    //Debug.Log("Weather fading in " + state_start_time);

                }

                if (currentState == Volume_Weather_State.FadeOut) {

                    //FadingOut = true;

                    if (VolumeScript != null) {
                        VolumeScript.FadeOutOnBoundary = true;//make sure clouds go out
                        VolumeScript.DestroyOnfadeOut = true;
                        VolumeScript.SmoothIn = false;
                        VolumeScript.SmoothOut = true;
                        //Debug.Log("name="+VolumeScript.gameObject.name);
                        //VolumeScript.cloned = true;//force cloned
                    } else {
                        ///Debug.Log("no volume script");
                    }

                    if (Rain) {
                        for (int i = 0; i < RefractRainParticle.Count; i++) {
                            //v3.4
                            if (i < RefractRainParticleP.Count) {
                                Make_Particles_Dissappear(RefractRainParticle[i].gameObject, RefractRainParticleP[i], null, 20, 10, false, true);
                            }
                        }
                        for (int i = 0; i < RainParticle.Count; i++) {
                            //v3.4
                            if (i < RainParticleP.Count) {
                                Make_Particles_Dissappear(RainParticle[i].gameObject, RainParticleP[i], null, 20, 10, false, true);
                            }
                        }
                    }
                    if (Snow) {

                        //			Debug.Log("A="+SnowParticle.Count);
                        //			Debug.Log("B="+SnowParticleP.Count);

                        for (int i = 0; i < SnowParticle.Count; i++) {
                            Make_Particles_Dissappear(SnowParticle[i].gameObject, SnowParticleP[i], null, 20, 10, false, true);
                        }
                    }
                    //TIMER
                    if (FadeOutTime != 0) {
                        if (Time.fixedTime - state_start_time > FadeOutTime) {
                            FadedOut = true;
                            currentState = Volume_Weather_State.Steady;
                            state_start_time = Time.fixedTime;
                        }
                    }

                    //Debug.Log("Weather fading out " + state_start_time);

                }

                if (currentState == Volume_Weather_State.Steady) {
                    state_start_time = Time.fixedTime;//keep time to pass to fade out when it happens
                                                      //Debug.Log("Weather steady ");

                    if (!FadedOut) {


                        //v3.4
                        if (SkyManager.WeatherSeverity != prevSeverity) {
                            if (Rain) {
                                if (Refractive_Rain) {
                                    for (int i = 0; i < RefractRainParticle.Count; i++) {
                                        //v3.4
                                        if (i < RefractRainParticleP.Count) {
                                            for (int n = 0; n < RefractRainParticleP[i].Length; n++) {
                                                //RefractRainParticleP [i][n].maxParticles = (int)(SkyManager.Refract_Rain_max_part * SkyManager.WeatherSeverity / 10);//v3.4
                                                ParticleSystem.MainModule MainMod = RefractRainParticleP[i][n].main; //v3.4.9
                                                MainMod.maxParticles = (int)(SkyManager.Refract_Rain_max_part * SkyManager.WeatherSeverity / 10);
                                            }
                                        }
                                    }
                                } else {
                                    for (int i = 0; i < RainParticle.Count; i++) {
                                        //v3.4
                                        if (i < RainParticleP.Count) {
                                            for (int n = 0; n < RainParticleP[i].Length; n++) {
                                                //RainParticleP [i][n].maxParticles = (int)(SkyManager.Refract_Rain_max_part * SkyManager.WeatherSeverity / 10);//v3.4
                                                ParticleSystem.MainModule MainMod = RainParticleP[i][n].main; //v3.4.9
                                                MainMod.maxParticles = (int)(SkyManager.Refract_Rain_max_part * SkyManager.WeatherSeverity / 10);
                                            }
                                        }
                                    }
                                }
                            }
                            prevSeverity = SkyManager.WeatherSeverity;
                        }


                        if (Snow) {
                            //						for (int i=0; i<SnowParticle.Count; i++) {
                            //							Make_Particles_Appear (SnowParticle [i].gameObject, SnowParticleP [i], null, SkyManager.Flat_cloud_max_part, true, 10, 1);
                            //						}
                            //if(SnowCoverageVariable < MaxSnowCoverage){
                            //if(SnowMat.HasProperty(SnowCoverageVariable) && SnowMat.GetFloat(SnowCoverageVariable) < MaxSnowCoverage){
                            if (SkyManager.SnowCoverage < SkyManager.MaxSnowCoverage) {
                                //SnowCoverageVariable += SnowCoverageRate*Time.deltaTime;
                                SkyManager.SnowCoverage += SkyManager.SnowCoverageRate * Time.deltaTime;
                                Shader.SetGlobalFloat(SnowCoverageVariable, SkyManager.SnowCoverage);
                            }
                            if (SkyManager.SnowCoverageTerrain < SkyManager.MaxSnowCoverage) {
                                SkyManager.SnowCoverageTerrain += SkyManager.SnowTerrainRate * Time.deltaTime * SkyManager.SnowTerrRateFactor;

                                //Debug.Log(SkyManager.SnowCoverageTerrain);

                                if (SnowMatTerrain != null) {
                                    SnowMatTerrain.SetFloat(SnowCoverageVariable, SkyManager.SnowCoverageTerrain / SkyManager.divideSnowTerrain);
                                }
                            }
                        } else {
                            //						for (int i=0; i<SnowParticle.Count; i++) {
                            //							Make_Particles_Dissappear(SnowParticle[i].gameObject, SnowParticleP[i], null, 20, 10, false, true);
                            //						}
                            //Debug.Log(VolumeCloud.gameObject.name);
                            if (SkyManager.SnowCoverage > 0) {
                                SkyManager.SnowCoverage -= SkyManager.SnowCoverageRate * Time.deltaTime;
                                Shader.SetGlobalFloat(SnowCoverageVariable, SkyManager.SnowCoverage);
                                //						if(!Application.isPlaying){
                                //							Shader.SetGlobalFloat(SnowCoverageVariable, 0);
                                //						}
                            }
                            if (SkyManager.SnowCoverageTerrain > 0) {
                                SkyManager.SnowCoverageTerrain -= SkyManager.SnowTerrainRate * Time.deltaTime * SkyManager.SnowTerrRateFactor;
                                if (SnowMatTerrain != null) {
                                    SnowMatTerrain.SetFloat(SnowCoverageVariable, SkyManager.SnowCoverageTerrain / SkyManager.divideSnowTerrain);
                                }
                            }
                        }//END SNOW
                    }

                }
            }
        }

        #region FUNCTIONS
        void Make_Fog_Dissappear(float speed) {
            //// FOG
            #region FOG
            if (RenderSettings.fog) {
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0, speed * Time.deltaTime);
            }
            #endregion
            //// END FOG
        }

        void Make_Fog_Appear(Color Fog_Color, float density, FogMode mode, float speed, int Season) {
            //// FOG
            #region FOG
            bool Enable_fog = true;
            float Color_divider = 255;
            if (Enable_fog) {
                if (!RenderSettings.fog) {
                    RenderSettings.fog = true;
                    RenderSettings.fogDensity = 0;
                }
                if (Season != 0) { //give color based on season, else get from outside script variable
                    if (Season == 3) {
                        Fog_Color = new Color(100 / Color_divider, 80 / Color_divider, 100 / Color_divider, 255 / Color_divider);
                        density = 0.01f;
                        mode = FogMode.ExponentialSquared;//fogMode;// FogMode.ExponentialSquared; //v4.5
                    }
                } else {
                    //Fog_Color = Color.white;					
                }
                Color LERP_TO = Fog_Color;
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, LERP_TO, speed * Time.deltaTime);//0.5f
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, density, speed * Time.deltaTime);//0.01f
                RenderSettings.fogMode = mode; //FogMode.ExponentialSquared;

            } else {
                if (RenderSettings.fog) {
                    RenderSettings.fog = false;
                }
            }
            #endregion
            //// END FOG
        }

        //Make particle clouds appear
        void Make_Particles_Appear(GameObject Particle_OBJ, ParticleSystem[] ParticleOBJ_Ps, ParticleSystem ParticleOBJ_P, int max_particles, bool reset_pcount, int rate, float Transp) {

            if (Particle_OBJ != null) {
                if (!Particle_OBJ.activeInHierarchy) {
                    Particle_OBJ.SetActive(true);
                    if (ParticleOBJ_Ps != null) {
                        for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                            ParticleOBJ_Ps[n].Stop();
                            ParticleOBJ_Ps[n].Clear();

                            ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9

                            if (reset_pcount) {
                                //ParticleOBJ_Ps[n].maxParticles=0;
                                MainMod.maxParticles = 0; //v3.4.9
                            }
                            //v2.2
                            //ParticleOBJ_Ps[n].startColor = new Color(ParticleOBJ_Ps[n].startColor.r,ParticleOBJ_Ps[n].startColor.g,ParticleOBJ_Ps[n].startColor.b,0);
                            MainMod.startColor = new Color(ParticleOBJ_Ps[n].main.startColor.color.r, ParticleOBJ_Ps[n].main.startColor.color.g, ParticleOBJ_Ps[n].main.startColor.color.b, 0); //v3.4.9
                        }
                    }
                    if (ParticleOBJ_P != null) {
                        ParticleOBJ_P.Stop();
                        ParticleOBJ_P.Clear();
                        if (reset_pcount) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                            MainMod.maxParticles = 0;
                            //ParticleOBJ_P.maxParticles=0;
                        }
                    }
                } else { //start increasing particles
                    if (reset_pcount) {
                        if (ParticleOBJ_Ps != null) {
                            for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                                ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9
                                if (MainMod.maxParticles < max_particles) { //v3.4.9
                                    MainMod.maxParticles += rate; // RESTORE max from spring elimination
                                                                  //v2.2									 
                                    if (MainMod.startColor.color.a < Transp) {
                                        MainMod.startColor = new Color(ParticleOBJ_Ps[n].main.startColor.color.r, ParticleOBJ_Ps[n].main.startColor.color.g, ParticleOBJ_Ps[n].main.startColor.color.b, ParticleOBJ_Ps[n].main.startColor.color.a + 0.08f * rate * Time.deltaTime);//v3.2 - changed from 0.02f
                                    }
                                }
                                if (!ParticleOBJ_Ps[n].isPlaying) {
                                    ParticleOBJ_Ps[n].Play();
                                }
                            }
                        }
                        if (ParticleOBJ_P != null) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                            if (MainMod.maxParticles < max_particles) {
                                MainMod.maxParticles += rate; // RESTORE max from spring elimination
                            }
                            if (!ParticleOBJ_P.isPlaying) {
                                ParticleOBJ_P.Play();
                            }
                        }
                    } else {
                        if (ParticleOBJ_Ps != null) {
                            for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                                ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9
                                MainMod.maxParticles = max_particles; // RESTORE max from spring elimination
                                ParticleOBJ_Ps[n].Play();
                            }
                        }
                        if (ParticleOBJ_P != null) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                            MainMod.maxParticles = max_particles; // RESTORE max from spring elimination
                            ParticleOBJ_P.Play();
                        }
                    }
                }
            }
        }
        //Make particle clouds dissappear
        void Make_Particles_Dissappear(GameObject Particle_OBJ, ParticleSystem[] ParticleOBJ_Ps, ParticleSystem ParticleOBJ_P, int min_particles, int rate, bool pull_downward, bool on_lowest) {

            if (Particle_OBJ != null) {
                if (Particle_OBJ.activeInHierarchy) {
                    if (ParticleOBJ_Ps != null) {
                        int count_finished = 0;

                        for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9
                            MainMod.maxParticles = MainMod.maxParticles - rate;
                            if (pull_downward) {
                                MainMod.gravityModifier = -10;
                            }
                            if (MainMod.maxParticles < min_particles || SkyManager.instaKillParticles) { //start increasing opaque //v4.8.9
                                ParticleOBJ_Ps[n].Stop();
                                ParticleOBJ_Ps[n].Clear();
                                if (on_lowest) {
                                    Particle_OBJ.SetActive(false);
                                }
                                count_finished++;
                            }
                        }
                        if (!on_lowest) {
                            if (count_finished >= ParticleOBJ_Ps.Length) {
                                Particle_OBJ.SetActive(false);
                            }
                        }
                    }
                    if (ParticleOBJ_P != null) {
                        ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                        MainMod.maxParticles = MainMod.maxParticles - rate;

                        if (pull_downward) {
                            MainMod.gravityModifier = -10;
                        }
                        if (MainMod.maxParticles < min_particles) { //start increasing opaque
                            ParticleOBJ_P.Stop();
                            ParticleOBJ_P.Clear();
                            Particle_OBJ.SetActive(false);
                        }
                    }
                }
            }
        }
        #endregion

    }

    [ExecuteInEditMode]
    public class SkyMasterManager : MonoBehaviour
    {
        //v4.9.5
        public bool instaChangeSkyColor = false;//change sky color instantly without lerp to next value, enable just before change TOD in gameplay

        //v4.8.9
        public bool instaKillParticles = false; //temporary fix for Unity 2019.1 bug - crash when lowering maxParticles gradually 

        //v4.8
        //public FullVolumeCloudsSkyMaster volumeClouds;

        //v4.5
        public FogMode fogMode = FogMode.ExponentialSquared;

        //v4.2 - disable sun at night time
        public bool disableSunAtNight = false;

        //v4.1d
        public bool gradAffectsFog = true;//choose to affect fog by gradients or color settings in the sky master manager colors

        //v3.4.2
        public float White_cutoffOffset = 5;
        //float initCamY=0;

        //v3.4 - volume shader clouds
        public Transform VolShaderClouds;
        public CloudHandlerSM VolShaderCloudsH;
        public bool SunFollowHero = false;
        //public AtmosphericScatteringSkyMaster VolLightingH;
        //public AtmosphericScatteringDeferredSkyMaster VolLightingDefH;

        [Range(1, 20)]
        public float WeatherSeverity = 10;

        //v3.3e - UTC zone
        [Range(-12, 12)]
        public float Time_zone = 0;
        [HideInInspector]
        public float MaxSunElevation;
        [HideInInspector]
        public Color gradSkyColor;
        [HideInInspector]
        public float calcColorTime;

        //v3.3e
        public bool UseGradients = false;
        public Gradient SkyColorGrad;
        public Gradient SkyTintGrad;
        public float FogColorPow = 1.7f;
        public float FogWaterPow = 1.4f;
        //		public AnimationCurve FexposureC;
        //		public AnimationCurve FscaleDiffC;
        //		public AnimationCurve FSunGC;
        //		public AnimationCurve FSunringC;



        public AnimationCurve FexposureC = new AnimationCurve(new Keyframe(0, 0.3f), new Keyframe(0.3f, 0.0f), new Keyframe(0.7f, 0.0f), new Keyframe(1, 0.3f));
        public AnimationCurve FscaleDiffC = new AnimationCurve(new Keyframe(0, 0.001f), new Keyframe(0.3f, 0.001f), new Keyframe(0.7f, 0.001f), new Keyframe(1, 0.001f));
        public AnimationCurve FSunGC = new AnimationCurve(new Keyframe(0, 0.95f), new Keyframe(0.3f, 0.99f), new Keyframe(0.7f, 0.99f), new Keyframe(1, 0.95f));
        public AnimationCurve FSunringC = new AnimationCurve(new Keyframe(0, 0.0f), new Keyframe(0.3f, 0.0f), new Keyframe(0.7f, 0.0f), new Keyframe(1, 0.0f));

        public bool VolCloudGradients = false;
        public Gradient VolCloudLitGrad;
        public Gradient VolCloudShadGrad;
        public Gradient VolCloudFogGrad;
        public float VolCloudTransp = 0.7f;
        [HideInInspector]
        public Color gradCloudLitColor;
        [HideInInspector]
        public Color gradCloudShadeColor;
        [HideInInspector]
        public Color gradCloudFogColor;


        [HideInInspector]
        public float VcloudSunIntensity;
        [HideInInspector]
        public float VcloudLightDiff;
        [HideInInspector]
        public float VcloudFog;

        //		public AnimationCurve IntensityDiff = new AnimationCurve (new Keyframe (0,0.756f), 
        //			new Keyframe (0.216f, 0.795f), new Keyframe (0.318f,0.566f), new Keyframe (0.374f,0.324f), new Keyframe (0.6f,0.308f),
        //			new Keyframe (0.758f,0.211f), new Keyframe (0.817f,0.290f), new Keyframe (0.899f,0.212f), new Keyframe (0.936f,0.246f),
        //			new Keyframe (1,0.756f));
        //
        //		public AnimationCurve IntensityFog = new AnimationCurve (new Keyframe (0,5f), 
        //			new Keyframe (0.75f,10f), new Keyframe (0.85f,40f), new Keyframe (0.9f,20f), 
        //			new Keyframe (1,5f));
        //
        //		public AnimationCurve IntensitySun = new AnimationCurve (new Keyframe (0,0.078f), 
        //			new Keyframe (0.75f,0.118f), new Keyframe (0.85f,0.04f), new Keyframe (0.90f,0.118f), 
        //			new Keyframe (1,0.078f));

        //v3.4.3
        [SerializeField]
        public AnimationCurve IntensityDiff = new AnimationCurve(new Keyframe(0, 0.4f),
            new Keyframe(0.374f, 0.293f), new Keyframe(0.6f, 0.2766f), new Keyframe(0.757f, 0.278f), new Keyframe(0.798f, 0.2713f),
            //new Keyframe (0.869f,0.204f), new Keyframe (0.916f,0.232f), new Keyframe (0.944f,0.280f),
            new Keyframe(0.8495f, 0.2752f), new Keyframe(0.887f, 0.249f), new Keyframe(0.944f, 0.280f),
            new Keyframe(1, 0.4f));
        [SerializeField]
        public AnimationCurve IntensityFog = new AnimationCurve(new Keyframe(0, 5f),
            new Keyframe(0.75f, 10f), new Keyframe(0.883f, 10.91f),
            new Keyframe(1, 5f));
        [SerializeField]
        public AnimationCurve IntensitySun = new AnimationCurve(new Keyframe(0, 0f),
            //new Keyframe (0.185f,0.127f), new Keyframe (0.668f,0.274f), new Keyframe (0.803f,0.365f),new Keyframe (0.827f,1.1f), new Keyframe (0.871f,0.666f),
            new Keyframe(0.186f, 0.148f), new Keyframe(0.71f, 0.13f), new Keyframe(0.84f, 0.30f), new Keyframe(0.9f, 0.13f),
            new Keyframe(1, 0.0f));

        //v3.4.3
        //		public AnimationCurve IntensityDiff = new AnimationCurve (new Keyframe (0,0.4f), 
        //			new Keyframe (0.374f,0.292f), new Keyframe (0.602f,0.255f), new Keyframe (0.757f,0.278f), new Keyframe (0.798f,0.271f),
        //			//new Keyframe (0.869f,0.204f), new Keyframe (0.916f,0.232f), new Keyframe (0.944f,0.280f),
        //			new Keyframe (0.869f,0.204f), new Keyframe (0.9f,0.5f), new Keyframe (0.944f,0.280f),
        //			new Keyframe (1,0.4f));
        //
        //		public AnimationCurve IntensityFog = new AnimationCurve (new Keyframe (0,5f), 
        //			new Keyframe (0.75f,10f), new Keyframe (0.88f,11f), new Keyframe (0.89f,10.58f), 
        //			new Keyframe (1,5f));
        //
        //		public AnimationCurve IntensitySun = new AnimationCurve (new Keyframe (0,0f), 
        //			//new Keyframe (0.185f,0.127f), new Keyframe (0.668f,0.274f), new Keyframe (0.803f,0.365f),new Keyframe (0.827f,1.1f), new Keyframe (0.871f,0.666f),
        //			new Keyframe (0.185f,0.127f), new Keyframe (0.668f,0.274f), new Keyframe (0.803f,0.365f),new Keyframe (0.827f,0.65f), new Keyframe (0.9f,-0.1f),
        //			new Keyframe (1,0.0f));

        //v3.4
        //		public AnimationCurve IntensityDiff = new AnimationCurve (new Keyframe (0,0.4f), 
        //			new Keyframe (0.374f, 0.292f), new Keyframe (0.6f,0.2766f), new Keyframe (0.757f,0.278f), new Keyframe (0.798f,0.271f),
        //			new Keyframe (0.849f,0.275f), new Keyframe (0.887f,0.248f), new Keyframe (0.944f,0.280f),
        //			new Keyframe (1,0.4f));
        //
        //		public AnimationCurve IntensityFog = new AnimationCurve (new Keyframe (0,5f), 
        //			new Keyframe (0.75f,10f), new Keyframe (0.88f,11f), new Keyframe (0.89f,10.58f), 
        //			new Keyframe (1,5f));
        //
        //		public AnimationCurve IntensitySun = new AnimationCurve (new Keyframe (0,0.078f), 
        //			new Keyframe (0.1864f,0.148f), new Keyframe (0.71f,0.129f), new Keyframe (0.839f,0.303f), new Keyframe (0.90f,0.13f),
        //			new Keyframe (1,0.078f));

        //v3.4.2
        //		public AnimationCurve IntensityDiff = new AnimationCurve (new Keyframe (0,0.4f), 
        //			new Keyframe (0.35f, 0.326f), new Keyframe (0.396f,0.584f), new Keyframe (0.59f,0.433f),new Keyframe (0.65f,0.652f), new Keyframe (0.755f,0.584f), new Keyframe (0.80f,0.616f),
        //			new Keyframe (0.85f,0.506f), new Keyframe (0.90f,0.58f), new Keyframe (0.947f,0.487f),
        //			new Keyframe (1,0.4f));
        //
        //		public AnimationCurve IntensityFog = new AnimationCurve (new Keyframe (0,5f), 
        //			new Keyframe (0.75f,10f), new Keyframe (0.88f,11f), new Keyframe (0.89f,10.58f), 
        //			new Keyframe (1,5f));
        //
        //		public AnimationCurve IntensitySun = new AnimationCurve (new Keyframe (0,-0.6f), 
        //			new Keyframe (0.217f,-0.625f), new Keyframe (0.662f,0.196f), new Keyframe (0.8f,0.36f), new Keyframe (0.83f,0.815f),new Keyframe (0.865f,0.683f), new Keyframe (0.90f,0.436f), 
        //			new Keyframe (0.949f,0.250f), 
        //			new Keyframe (1,-0.6f));



        //v3.3 - moon positioning
        public bool Lerp_sky_rot = false;
        public bool LatLonMoonPos = false;
        public bool AutoMoonLighting = false;//automatic sun lighting and day fade for moon shader
        public Transform MoonCenterObj;
        public bool StarsFollowMoon = false;
        public bool AutoMoonFade = false;//fade out moon shader when visible in day time
        public Color MoonColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
        public Color MoonAmbientColor = new Color(0.9f, 0.9f, 0.9f, 1);
        public Vector3 MinMaxEclMoonTransp = new Vector3(0.2f, 0.9f, 0.1f);
        public float MoonSunLight = 0.02f;
        public float MoonCoverage = 0.377f;
        public float MoonSize = 1;
        public Material StarsMaterial;
        public Color StarsColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
        public Vector2 MinMaxStarsTransp = new Vector2(0.5f, 1f);
        public float StarsIntensity = 1.4f;
        public float MoonPlaneRot = 0;
        public bool onEclipse = false;

        //v3.1
        public float DawnAppearSpeed = 1;
        public float StarsPFadeSpeed = 0.5f;
        public float StarsPMaxGlow = 0.8f;

        //v.3.0.2 - new TOD system
        public bool AutoSunPosition = false;//use a latitude, longitude system to define sun positioning
        public float RotateNorth = 0; //v4.2b
        public float Latitude = 15;
        public float Longitude = 45;
        public float NightTimeMax = 22.4f;
        public float NightAngleMax = 0f;//v3.4.8 controls the angle where sun and moon change in sky shader

        //v3.0.2
        public float SkyColorationOffset = 0; // offset coloration intensity over the preset value
        public float divideSnowTerrain = 14;

        //FOGS
        public List<int> VFogsPerVWeather = new List<int>();

        //v3.0 camera
        Transform Cam_tranform;
        public float AmplifyWind = 1;//multiple wind effect for volume clouds

        //SCALING
        public float WorldScale = 20;
        public float prevWorldScale = 20;

        //v3.0
        public bool MoonPhases = false;//add moon phases emulation 
        public float max_moon_intensity = 0.25f;//v3.3b
        public Material MoonPhasesMat;
        public bool ScreenRainDrops = false; //control rain drops on screen
        public Transform RainDropsPlane;
        public Material ScreenRainDropsMat;
        public bool ScreenFreezeFX = false;
        public bool FreezeInwards = false;

        public float MaxDropSpeed = 6;
        public float MaxWater = 1;
        public float MaxRefract = 4.5f;
        public float FreezeSpeed = 0.2f;

        //rain above hero distance and water handling
        public Transform water;
        public float RainDistAboveHero = 10;

        public GameObject SunSystem;

        //v2.2 - Weather states and volumetric weather
        public enum Volume_Weather_types { Sunny, Foggy, HeavyFog, Tornado, SnowStorm, FreezeStorm, FlatClouds, LightningStorm, HeavyStorm, HeavyStormDark, Cloudy, RollingFog, VolcanoErupt, Rain }
        //public enum Volume_Weather_types {Sunny, Foggy, SnowStorm, HeavyStorm, Cloudy, Rain};

        //v3.4
        //	[Range(0,4)]
        //	public int WeatherLevel = 0; //start level
        [SerializeField]
        public WeatherSM currentWeather;
        public WeatherSM prevWeather;
        public Volume_Weather_types currentWeatherName = Volume_Weather_types.Sunny;
        Volume_Weather_types prevWeatherName = Volume_Weather_types.Sunny;
        public WindZone windZone;

        public List<WeatherEventSM> WeatherEvents = new List<WeatherEventSM>();
        public float VolCloudsHorScale = 1000;//scale of cloud bed
        public float VolCloudHeight = 650;
        public float VCloudCoverFac = 1; //increase volumetric clouds
        public float VCloudSizeFac = 1; //scale volumetric cloud particles
        public float VCloudCSizeFac = 1; //cloud scale

        //v3.4.1
        public Vector2 VCloudXZOffset = new Vector2(0, 0);
        public float WindBasedOffsetFactor = 1;//volume clouds will be created opposite of the wind direction around the map center, to traverse the map on wind.
        public Transform VCloudCenter;//define other center than map one
        public bool VCloudCustomSize = false;

        public bool DefinePlanetScale = false;
        public float PlanetScale = 10000;
        public Transform MapCenter;
        public Transform SkyDomeSystem;//sky dome sphere

        public GameObject CloudDomeL1;
        public GameObject CloudDomeL2;
        public Color Dusk_L1_dome_color = new Color(230f / 255f, 212f / 255f, 226f / 255f, 174f / 255f); //1 cover
        public Color Dusk_L2_dome_color = new Color(90f / 255f, 54f / 255f, 28f / 255f, 174f / 255f); //1 cover
        public Color Dawn_L1_dome_color = new Color(250f / 255f, 190f / 255f, 170f / 255f, 200f / 255f); //0.775 cover
        public Color Dawn_L2_dome_color = new Color(100f / 255f, 5f / 255f, 5f / 255f, 200f / 255f); //0.705 cover

        //				public Color Day_L1_dome_color 		= new Color(240f/255f,230f/255f,230f/255f,255f/255f); //0.8
        //				public Color Day_L2_dome_color 		= new Color(216f/255f,216f/255f,216f/255f,184f/255f); //0.693 - best with new preset 8 !!!
        public Color Day_L1_dome_color = new Color(240f / 255f, 240f / 255f, 240f / 255f, 255f / 255f); //0.8
        public Color Day_L2_dome_color = new Color(216f / 255f, 216f / 255f, 216f / 255f, 184f / 255f); //0.693 - best with new preset 8 !!!

        public Color Night_L1_dome_color = new Color(70f / 255f, 70f / 255f, 70f / 255f, 255f / 255f); //0.8
        public Color Night_L2_dome_color = new Color(103f / 255f, 103f / 255f, 103f / 255f, 184f / 255f); //0.6

        public Color Dark_storm_L2CA = new Color(50f / 255f, 40f / 255f, 40f / 255f, 214f / 255f);//v3.0.4

        public GameObject StarDome;

        public Material CloudDomeL1Mat;//shader based cloud dome material - layer1
        public Material CloudDomeL2Mat;//shader based cloud dome material - layer2 - additive add to layer 1 for special FX and contrast

        public float L1CloudCoverOffset = 0;//offset the preset cloud coverage
        public float L1CloudDensOffset = 0;//shift cloud layer to cover more sky
        public float L1CloudSize = 1;//resize cloud layer
        public float L1Ambience = 1;
        public float L12SpeedOffset = 1;//offset speed over the windzone effect

        public Material SnowMat;
        public Material SnowMatTerrain;
        public float SnowCoverage = 0;
        public float SnowCoverageTerrain = 0;
        public float SnowCoverageRate = 0.1f;
        public float SnowTerrainRate = 0.0001f;
        public float SnowTerrRateFactor = 1f;
        public float MaxSnowCoverage = 1;
        public string SnowCoverageVariable = "_SnowCoverage";

        //VOLUME CLOUDS
        public GameObject HeavyStormVolumeClouds;//prefab to instantiate
        public Object DustyStormVolumeClouds;
        public Object DayClearVolumeClouds;
        public Object SnowStormVolumeClouds;
        public Object SnowVolumeClouds;
        public Object RainStormVolumeClouds;
        public Object RainVolumeClouds;
        public Object PinkVolumeClouds;
        public Object LightningVolumeClouds;

        void LateUpdate() {

            //			//v3.3e
            //			if (UseGradients) {
            //				if (SkyColorGrad != null) {					
            //					m_TintColor = SkyTintGrad.Evaluate(Current_Time/24);
            //					Color TMP1 = SkyColorGrad.Evaluate(Current_Time/24);
            //
            //					CloudDomeL1Mat.SetVector ("_Color", TMP1*TMP1);
            //					//float FogColorPow = 1.7f;FogWaterPow
            //					RenderSettings.fogColor = new Color(Mathf.Pow(TMP1.r,FogColorPow),Mathf.Pow(TMP1.g,FogColorPow),Mathf.Pow(TMP1.b,FogColorPow));
            //
            //					if (!Application.isPlaying) {
            //						DynamicGI.UpdateEnvironment ();
            //						RenderSettings.ambientIntensity = AmbientIntensity;
            //					}
            //					if (water != null) {
            //						Material WaterHandlerMat = water.GetComponent<WaterHandlerSM> ().oceanMat;
            //						WaterHandlerMat.SetColor ("_ReflectionColor", new Color(Mathf.Pow(TMP1.r,FogWaterPow),Mathf.Pow(TMP1.g,FogWaterPow),Mathf.Pow(TMP1.b,FogWaterPow)));
            //						Color OceanBase = WaterHandlerMat.GetColor ("_BaseColor");
            //						Color Basefinal = OceanBase * TMP1 * 0.5f + 0.5f * OceanBase;
            //						WaterHandlerMat.SetColor ("_BaseColor",new Color(Basefinal.r,Basefinal.g,Basefinal.b,OceanBase.a));
            //					}
            //
            //					if (TMP1.r > 0.118f) {
            //						TMP1.r = 1.118f - TMP1.r;
            //					}
            //					if (TMP1.g > 0.118f) {
            //						TMP1.g = 1.118f - TMP1.g;
            //					}
            //					if (TMP1.b > 0.118f) {
            //						TMP1.b = 1.118f - TMP1.b;
            //					}
            //					m_fWaveLength = new Vector3(TMP1.r,TMP1.g,TMP1.b);
            //				}
            //			}

            if (Application.isPlaying) {

                //HANDLE WEATHER EVENTS
                // Disable event
                for (int i = 0; i < WeatherEvents.Count; i++) {
                    if (WeatherEvents[i].is_activated) {

                        //v3.4
                        bool dayWithinBounds = (Current_Day >= WeatherEvents[i].EventStartDay && Current_Day <= WeatherEvents[i].EventEndDay);
                        bool monthWithinBounds = true;
                        bool timeWithinBounds = Current_Time >= WeatherEvents[i].EventStartHour && Current_Time <= WeatherEvents[i].EventEndHour;
                        if (WeatherEvents[i].loop) {
                            int modulatedMonth = (Current_Month + 1) - (Mathf.FloorToInt((Current_Month + 1) / 12) * 12);
                            dayWithinBounds = (WeatherEvents[i].EventStartDay >= (modulatedMonth - 1) * days_per_month && WeatherEvents[i].EventEndDay <= (modulatedMonth) * days_per_month);
                            monthWithinBounds = modulatedMonth >= WeatherEvents[i].EventStartMonth && modulatedMonth <= WeatherEvents[i].EventEndMonth;
                        }

                        if (!dayWithinBounds || !timeWithinBounds || !monthWithinBounds) {
                            WeatherEvents[i].is_activated = false;
                        }

                        //v3.4
                        //						if(Current_Day < WeatherEvents[i].EventStartDay | Current_Day > WeatherEvents[i].EventEndDay){
                        //							WeatherEvents[i].is_activated = false;
                        //						}
                        //						if(Current_Time < WeatherEvents[i].EventStartHour | Current_Time > WeatherEvents[i].EventEndHour){
                        //							WeatherEvents[i].is_activated = false;
                        //						}
                        //						if(Current_Month < WeatherEvents[i].EventStartMonth | Current_Month > WeatherEvents[i].EventEndMonth){
                        //							WeatherEvents[i].is_activated = false;
                        //						}

                        //v3.4a
                        if (currentWeather.VolumeScript != null) {
                            currentWeather.VolumeScript.FadeOutOnBoundary = true;//make sure clouds go out
                            currentWeather.VolumeScript.DestroyOnfadeOut = true;
                        }

                        //Enable new weather
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.HeavyStorm) {
                            currentWeatherName = Volume_Weather_types.HeavyStorm;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Sunny) {
                            currentWeatherName = Volume_Weather_types.Sunny;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Cloudy) {
                            currentWeatherName = Volume_Weather_types.Cloudy;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.FlatClouds) {
                            currentWeatherName = Volume_Weather_types.FlatClouds;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Foggy) {
                            currentWeatherName = Volume_Weather_types.Foggy;
                        }

                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.FreezeStorm) {
                            currentWeatherName = Volume_Weather_types.FreezeStorm;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.HeavyFog) {
                            currentWeatherName = Volume_Weather_types.HeavyFog;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.HeavyStormDark) {
                            currentWeatherName = Volume_Weather_types.HeavyStormDark;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.LightningStorm) {
                            currentWeatherName = Volume_Weather_types.LightningStorm;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Rain) {
                            currentWeatherName = Volume_Weather_types.Rain;
                        }

                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.RollingFog) {
                            currentWeatherName = Volume_Weather_types.RollingFog;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.SnowStorm) {
                            currentWeatherName = Volume_Weather_types.SnowStorm;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Tornado) {
                            currentWeatherName = Volume_Weather_types.Tornado;
                        }
                        if (WeatherEvents[i].FollowUpWeather == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.VolcanoErupt) {
                            currentWeatherName = Volume_Weather_types.VolcanoErupt;
                        }
                        ////// END WEATHER PATTERNS
                    }
                }

                bool enter = true;
                for (int i = 0; i < WeatherEvents.Count; i++) {
                    if (WeatherEvents[i].is_activated) {
                        enter = false;
                    }
                }
                if (enter) {
                    for (int i = 0; i < WeatherEvents.Count; i++) {
                        if (!WeatherEvents[i].is_activated) {
                            //enter = false;

                            //v3.4
                            bool dayWithinBounds = (Current_Day >= WeatherEvents[i].EventStartDay && Current_Day <= WeatherEvents[i].EventEndDay);
                            bool monthWithinBounds = true;
                            bool timeWithinBounds = Current_Time >= WeatherEvents[i].EventStartHour && Current_Time <= WeatherEvents[i].EventEndHour;
                            if (WeatherEvents[i].loop) {
                                int modulatedMonth = (Current_Month + 1) - (Mathf.FloorToInt((Current_Month + 1) / 12) * 12);
                                //int modulatedDayYEAR = Current_Day - (Mathf.FloorToInt (Current_Day / 364) * 364);
                                //int modulatedDayMONTH = Current_Day - (Mathf.FloorToInt (Current_Day / 30) * 30);
                                dayWithinBounds = (WeatherEvents[i].EventStartDay >= (modulatedMonth - 1) * days_per_month && WeatherEvents[i].EventEndDay <= (modulatedMonth) * days_per_month);
                                monthWithinBounds = modulatedMonth >= WeatherEvents[i].EventStartMonth && modulatedMonth <= WeatherEvents[i].EventEndMonth;
                                //timeWithinBounds = Current_Time >= WeatherEvents [i].EventStartHour & Current_Time <= WeatherEvents [i].EventEndHour;
                            }

                            if (dayWithinBounds) {
                                if (timeWithinBounds) {
                                    if (monthWithinBounds) {

                                        //							if(Current_Day >= WeatherEvents[i].EventStartDay & Current_Day <= WeatherEvents[i].EventEndDay){
                                        //								if(Current_Time >= WeatherEvents[i].EventStartHour & Current_Time <= WeatherEvents[i].EventEndHour){
                                        //									if(Current_Month >= WeatherEvents[i].EventStartMonth & Current_Month <= WeatherEvents[i].EventEndMonth){


                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.HeavyStorm) {
                                            //CHANGE WEATHER
                                            currentWeatherName = Volume_Weather_types.HeavyStorm;
                                            //VolCloudsHorScale = WeatherEvents[i].VolCloudsHorScale;//adjust bed scale
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Sunny) {
                                            currentWeatherName = Volume_Weather_types.Sunny;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Cloudy) {
                                            currentWeatherName = Volume_Weather_types.Cloudy;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.FlatClouds) {
                                            currentWeatherName = Volume_Weather_types.FlatClouds;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Foggy) {
                                            currentWeatherName = Volume_Weather_types.Foggy;
                                        }

                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.FreezeStorm) {
                                            currentWeatherName = Volume_Weather_types.FreezeStorm;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.HeavyFog) {
                                            currentWeatherName = Volume_Weather_types.HeavyFog;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.HeavyStormDark) {
                                            currentWeatherName = Volume_Weather_types.HeavyStormDark;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.LightningStorm) {
                                            currentWeatherName = Volume_Weather_types.LightningStorm;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Rain) {
                                            currentWeatherName = Volume_Weather_types.Rain;
                                        }

                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.RollingFog) {
                                            currentWeatherName = Volume_Weather_types.RollingFog;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.SnowStorm) {
                                            currentWeatherName = Volume_Weather_types.SnowStorm;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.Tornado) {
                                            currentWeatherName = Volume_Weather_types.Tornado;
                                        }
                                        if (WeatherEvents[i].Weather_type == Artngame.SKYMASTER.WeatherEventSM.Volume_Weather_event_types.VolcanoErupt) {
                                            currentWeatherName = Volume_Weather_types.VolcanoErupt;
                                        }
                                        ////// END WEATHER PATTERNS

                                        VolCloudsHorScale = WeatherEvents[i].VolCloudsHorScale;//adjust bed scale
                                        WeatherEvents[i].is_activated = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (currentWeatherName != prevWeatherName | currentWeather == null) {
                    //if ((currentWeatherName != prevWeatherName && ((currentWeather != null && currentWeather.currentState != WeatherSM.Volume_Weather_State.FadeIn) | currentWeather == null )) || currentWeather == null) {	
                    //Define new weather state
                    WeatherSM Weather = new WeatherSM();
                    Weather.SkyManager = this;
                    if (SnowMat != null) {
                        Weather.SnowMat = SnowMat;
                    }
                    if (SnowMatTerrain != null) {
                        Weather.SnowMatTerrain = SnowMatTerrain;
                    }
                    Weather.SnowCoverageVariable = SnowCoverageVariable;

                    Weather.windMode = WindZoneMode.Directional;
                    //Weather.WindDirection = Wind_OBJ.forward;

                    if (currentWeatherName == Volume_Weather_types.Sunny | currentWeatherName == Volume_Weather_types.VolcanoErupt |
                           currentWeatherName == Volume_Weather_types.Foggy | currentWeatherName == Volume_Weather_types.HeavyFog | currentWeatherName == Volume_Weather_types.RollingFog) {
                        Weather.Snow = false;
                    }

                    if (currentWeatherName == Volume_Weather_types.Cloudy | currentWeatherName == Volume_Weather_types.FlatClouds) {
                        //Weather.SkyManager = this;
                        Weather.has_fog = false;
                        Weather.Snow = false;

                        //Weather.VolumeCloud = HeavyStormVolumeClouds;
                        Vector3 Along_wind_vector = Vector3.forward;
                        if (windZone != null) {
                            Along_wind_vector = VolCloudsHorScale * windZone.transform.forward;
                        }

                        //v3.4.1
                        Vector3 CloudCenter = MapCenter.position;
                        if (VCloudCenter != null) {
                            CloudCenter = VCloudCenter.position;
                        }
                        Vector3 VolCloudStartPos = CloudCenter + new Vector3(VCloudXZOffset.x, VolCloudHeight, VCloudXZOffset.y) - Along_wind_vector * WindBasedOffsetFactor;

                        if (DayClearVolumeClouds != null) {
                            Weather.VolumeCloud = (GameObject)Instantiate(DayClearVolumeClouds, VolCloudStartPos, Quaternion.identity);
                            VolumeClouds_SM Cloud_Script = Weather.VolumeCloud.GetComponent<VolumeClouds_SM>();

                            if (Cloud_Script != null) {
                                if (windZone != null) {
                                    Cloud_Script.Wind_holder = windZone.gameObject;
                                }
                                Cloud_Script.sun_transf = SUN_LIGHT.transform;
                                Cloud_Script.moon_transf = MOON_LIGHT.transform.parent;
                                Cloud_Script.SkyManager = this;

                                //							//Scaling

                                //v3.4.1
                                if (VCloudCustomSize) {
                                    Cloud_Script.min_bed_corner = -VolCloudsHorScale * (20 / WorldScale);
                                    Cloud_Script.max_bed_corner = VolCloudsHorScale * (20 / WorldScale);
                                }

                                Cloud_Script.ScaleClouds(WorldScale, VCloudCoverFac, VCloudSizeFac, VCloudCSizeFac);

                                //							ParticleEmitter Cloud_Particles = Weather.VolumeCloud.GetComponent<ParticleEmitter>();	
                                //							if(Cloud_Particles!=null){
                                //								Cloud_Particles.minSize = Cloud_Particles.minSize*(WorldScale/20);
                                //								Cloud_Particles.maxSize = Cloud_Particles.maxSize*(WorldScale/20);
                                //							}
                                //
                                //							Cloud_Script.min_bed_corner = Cloud_Script.min_bed_corner*(WorldScale/20);
                                //							Cloud_Script.max_bed_corner = Cloud_Script.max_bed_corner*(WorldScale/20);
                                //							Cloud_Script.cloud_bed_heigh = Cloud_Script.cloud_bed_heigh*(WorldScale/20);
                                //
                                //							Cloud_Script.cloud_scale = Cloud_Script.cloud_scale*(WorldScale/20);
                                //							Cloud_Script.global_cloud_scale = Cloud_Script.global_cloud_scale*(WorldScale/20);
                                //
                                //								Cloud_Script.YScaleDiff = Cloud_Script.YScaleDiff*(WorldScale/20);
                                //								Cloud_Script.Yspread = Cloud_Script.Yspread*(WorldScale/20);
                                //								Cloud_Script.LODMinDist = Cloud_Script.LODMinDist*(WorldScale/20);
                                //								Cloud_Script.LodMaxHDiff = Cloud_Script.LodMaxHDiff*(WorldScale/20);
                                //								Cloud_Script.LodMaxYDiff = Cloud_Script.LodMaxYDiff*(WorldScale/20);
                                //
                                //							Cloud_Script.cloud_min_scale = Cloud_Script.cloud_min_scale*(WorldScale/20);
                                //							Cloud_Script.cloud_max_scale = Cloud_Script.cloud_max_scale *(WorldScale/20);

                            }
                        }
                    }

                    if (currentWeatherName == Volume_Weather_types.Rain) {
                        //Weather.SkyManager = this;
                        Weather.has_fog = false;
                        Weather.Snow = false;

                        //Weather.VolumeCloud = HeavyStormVolumeClouds;
                        Vector3 Along_wind_vector = Vector3.forward;
                        if (windZone != null) {
                            Along_wind_vector = VolCloudsHorScale * windZone.transform.forward;
                        }
                        //v3.4.1
                        Vector3 CloudCenter = MapCenter.position;
                        if (VCloudCenter != null) {
                            CloudCenter = VCloudCenter.position;
                        }
                        Vector3 VolCloudStartPos = CloudCenter + new Vector3(VCloudXZOffset.x, VolCloudHeight, VCloudXZOffset.y) - Along_wind_vector * WindBasedOffsetFactor;

                        if (DayClearVolumeClouds != null) {
                            Weather.VolumeCloud = (GameObject)Instantiate(DayClearVolumeClouds, VolCloudStartPos, Quaternion.identity);
                            VolumeClouds_SM Cloud_Script = Weather.VolumeCloud.GetComponent<VolumeClouds_SM>();

                            if (Cloud_Script != null) {
                                if (windZone != null) {
                                    Cloud_Script.Wind_holder = windZone.gameObject;
                                }
                                Cloud_Script.sun_transf = SUN_LIGHT.transform;
                                Cloud_Script.moon_transf = MOON_LIGHT.transform.parent;
                                Cloud_Script.SkyManager = this;

                                //							//Scaling

                                //v3.4.1
                                if (VCloudCustomSize) {
                                    Cloud_Script.min_bed_corner = -VolCloudsHorScale * (20 / WorldScale);
                                    Cloud_Script.max_bed_corner = VolCloudsHorScale * (20 / WorldScale);
                                }

                                Cloud_Script.ScaleClouds(WorldScale, VCloudCoverFac, VCloudSizeFac, VCloudCSizeFac);

                                //							ParticleEmitter Cloud_Particles = Weather.VolumeCloud.GetComponent<ParticleEmitter>();	
                                //							if(Cloud_Particles!=null){
                                //								Cloud_Particles.minSize = Cloud_Particles.minSize*(WorldScale/20);
                                //								Cloud_Particles.maxSize = Cloud_Particles.maxSize*(WorldScale/20);
                                //							}
                                //
                                //							Cloud_Script.min_bed_corner = Cloud_Script.min_bed_corner*(WorldScale/20);
                                //							Cloud_Script.max_bed_corner = Cloud_Script.max_bed_corner*(WorldScale/20);
                                //							Cloud_Script.cloud_bed_heigh = Cloud_Script.cloud_bed_heigh*(WorldScale/20);
                                //
                                //							Cloud_Script.cloud_scale = Cloud_Script.cloud_scale*(WorldScale/20);
                                //							Cloud_Script.global_cloud_scale = Cloud_Script.global_cloud_scale*(WorldScale/20);
                                //
                                //								Cloud_Script.YScaleDiff = Cloud_Script.YScaleDiff*(WorldScale/20);
                                //								Cloud_Script.Yspread = Cloud_Script.Yspread*(WorldScale/20);
                                //								Cloud_Script.LODMinDist = Cloud_Script.LODMinDist*(WorldScale/20);
                                //								Cloud_Script.LodMaxHDiff = Cloud_Script.LodMaxHDiff*(WorldScale/20);
                                //								Cloud_Script.LodMaxYDiff = Cloud_Script.LodMaxYDiff*(WorldScale/20);
                                //
                                //							Cloud_Script.cloud_min_scale = Cloud_Script.cloud_min_scale*(WorldScale/20);
                                //							Cloud_Script.cloud_max_scale = Cloud_Script.cloud_max_scale *(WorldScale/20);

                            }
                        }//v3.3 - dont check cloud exists for rain

                        //ADD RAIN
                        Weather.Rain = true;
                        Weather.Refractive_Rain = true;
                        //Weather.RefractRainParticle.Add(VolumeRain_Heavy.transform);
                        //Weather.RefractRainParticleP.Add(VolumeHeavy_Rain_P);
                        Weather.RefractRainParticle.Add(RefractRain_Heavy.transform);
                        Weather.RefractRainParticleP.Add(RefractHeavy_Rain_P);

                        //} //v3.3
                    }

                    if (currentWeatherName == Volume_Weather_types.HeavyStorm | currentWeatherName == Volume_Weather_types.HeavyStormDark
                       | currentWeatherName == Volume_Weather_types.Tornado | currentWeatherName == Volume_Weather_types.LightningStorm) {
                        //Weather.SkyManager = this;
                        Weather.has_fog = true;
                        Weather.Snow = false;

                        //Weather.VolumeCloud = HeavyStormVolumeClouds;
                        Vector3 Along_wind_vector = Vector3.forward;
                        if (windZone != null) {
                            Along_wind_vector = VolCloudsHorScale * windZone.transform.forward;
                        }
                        //v3.4.1
                        Vector3 CloudCenter = MapCenter.position;
                        if (VCloudCenter != null) {
                            CloudCenter = VCloudCenter.position;
                        }
                        Vector3 VolCloudStartPos = CloudCenter + new Vector3(VCloudXZOffset.x, VolCloudHeight, VCloudXZOffset.y) - Along_wind_vector * WindBasedOffsetFactor;

                        if (HeavyStormVolumeClouds != null) {
                            Weather.VolumeCloud = (GameObject)Instantiate(HeavyStormVolumeClouds, VolCloudStartPos, Quaternion.identity);
                            VolumeClouds_SM Cloud_Script = Weather.VolumeCloud.GetComponent<VolumeClouds_SM>();

                            if (Cloud_Script != null) {
                                if (windZone != null) {
                                    Cloud_Script.Wind_holder = windZone.gameObject;
                                }
                                Cloud_Script.sun_transf = SUN_LIGHT.transform;
                                Cloud_Script.moon_transf = MOON_LIGHT.transform.parent;
                                Cloud_Script.SkyManager = this;

                                //v3.4.1
                                if (VCloudCustomSize) {
                                    Cloud_Script.min_bed_corner = -VolCloudsHorScale * (20 / WorldScale);
                                    Cloud_Script.max_bed_corner = VolCloudsHorScale * (20 / WorldScale);
                                }

                                Cloud_Script.ScaleClouds(WorldScale, VCloudCoverFac, VCloudSizeFac, VCloudCSizeFac);
                            }
                        }

                        //Weather.SnowMat = SnowMat;
                        //Weather.SnowMatTerrain = SnowMatTerrain;
                        //Weather.MaxSnowCoverage = MaxSnowCoverage;
                        //Weather.SnowCoverageRate = SnowCoverageRate;
                        //Weather.SnowCoverageVariable = "_SnowCoverage";

                        //ADD RAIN
                        Weather.Rain = true;
                        Weather.Refractive_Rain = true;
                        //Weather.RefractRainParticle.Add(VolumeRain_Heavy.transform);
                        //Weather.RefractRainParticleP.Add(VolumeHeavy_Rain_P);
                        Weather.RefractRainParticle.Add(RefractRain_Heavy.transform);
                        Weather.RefractRainParticleP.Add(RefractHeavy_Rain_P);

                        //					if(1==0){
                        //						//CLOUD1
                        //						Weather.ParticleClouds.Add(Lower_Cloud_Bed.transform);	//Particle holder object
                        //						Weather.ParticlesCloudsP.Add(Cloud_bed_Dn_P);			//Particles
                        //						Weather.CloudParticleMax.Add(Flat_cloud_max_part);		//Max particle count
                        //						Weather.CloudParticleRate.Add(10);						//Particle increase rate
                        //						Weather.CloudParticleReset.Add(true);					//Reset particle on start
                        //						Weather.CloudTranspMax.Add(0.5f);
                        //						//CLOUD2
                        //						Weather.ParticleClouds.Add(Upper_Cloud_Bed.transform);
                        //						Weather.ParticlesCloudsP.Add(Cloud_bed_Up_P);
                        //						Weather.CloudParticleMax.Add(Flat_cloud_max_part);
                        //						Weather.CloudParticleRate.Add(10);
                        //						Weather.CloudParticleReset.Add(true);
                        //						Weather.CloudTranspMax.Add(0.5f);
                        //					}
                    }

                    if (currentWeatherName == Volume_Weather_types.SnowStorm | currentWeatherName == Volume_Weather_types.FreezeStorm) {
                        //Weather.SkyManager = this;
                        Weather.has_fog = true;
                        Weather.Snow = true;

                        //Weather.VolumeCloud = HeavyStormVolumeClouds;
                        Vector3 Along_wind_vector = Vector3.forward;
                        if (windZone != null) {
                            Along_wind_vector = VolCloudsHorScale * windZone.transform.forward;
                        }
                        //v3.4.1
                        Vector3 CloudCenter = MapCenter.position;
                        if (VCloudCenter != null) {
                            CloudCenter = VCloudCenter.position;
                        }
                        Vector3 VolCloudStartPos = CloudCenter + new Vector3(VCloudXZOffset.x, VolCloudHeight, VCloudXZOffset.y) - Along_wind_vector * WindBasedOffsetFactor;

                        if (SnowStormVolumeClouds != null) {
                            Weather.VolumeCloud = (GameObject)Instantiate(SnowStormVolumeClouds, VolCloudStartPos, Quaternion.identity);
                            VolumeClouds_SM Cloud_Script = Weather.VolumeCloud.GetComponent<VolumeClouds_SM>();

                            if (Cloud_Script != null) {
                                if (windZone != null) {
                                    Cloud_Script.Wind_holder = windZone.gameObject;
                                }
                                Cloud_Script.sun_transf = SUN_LIGHT.transform;
                                Cloud_Script.moon_transf = MOON_LIGHT.transform.parent;
                                Cloud_Script.SkyManager = this;

                                //v3.4.1
                                if (VCloudCustomSize) {
                                    Cloud_Script.min_bed_corner = -VolCloudsHorScale * (20 / WorldScale);
                                    Cloud_Script.max_bed_corner = VolCloudsHorScale * (20 / WorldScale);
                                }

                                Cloud_Script.ScaleClouds(WorldScale, VCloudCoverFac, VCloudSizeFac, VCloudCSizeFac);
                            }
                        }

                        Weather.volume_fog_peset = 7;

                        //ADD RAIN
                        Weather.Snow = true;
                        //Weather.SnowCoverageVariable = true;
                        //Weather.RefractRainParticle.Add(VolumeRain_Heavy.transform);
                        //Weather.RefractRainParticleP.Add(VolumeHeavy_Rain_P);
                        Weather.SnowParticle.Add(SnowStorm_OBJ.transform);
                        Weather.SnowParticleP.Add(SnowStorm_P);
                    }


                    //HANDLE VOLUME FOGS
                    if (VFogsPerVWeather.Count == 0) {
                        VFogsPerVWeather.Add(0);//sunny--
                        VFogsPerVWeather.Add(13);//foggy
                        VFogsPerVWeather.Add(14);//heavy fog
                        VFogsPerVWeather.Add(0);//tornado
                        VFogsPerVWeather.Add(7);//snow storm--

                        VFogsPerVWeather.Add(7);//freeze storm
                        VFogsPerVWeather.Add(0);//flat clouds
                        VFogsPerVWeather.Add(0);//lightning storm
                        VFogsPerVWeather.Add(7);//heavy storm--
                        VFogsPerVWeather.Add(7);//heavy storm dark--

                        VFogsPerVWeather.Add(12);//cloudy--
                        VFogsPerVWeather.Add(0);//rolling fog
                        VFogsPerVWeather.Add(0);//volcano erupt
                        VFogsPerVWeather.Add(14);//rain
                    }

                    if (water != null && Terrain_controller != null) {
                        if (Hero != null && Hero.position.y < water.position.y) { //v3.3d
                                                                                  //let water script define fog
                        } else {
                            //define fogs
                            if (currentWeatherName == Volume_Weather_types.Sunny) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[0];//0
                            }
                            if (currentWeatherName == Volume_Weather_types.Foggy) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[1];
                            }
                            if (currentWeatherName == Volume_Weather_types.HeavyFog) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[2];
                            }
                            if (currentWeatherName == Volume_Weather_types.Tornado) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[3];
                            }
                            if (currentWeatherName == Volume_Weather_types.SnowStorm) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[4];//7
                            }
                            if (currentWeatherName == Volume_Weather_types.FreezeStorm) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[5];
                            }
                            if (currentWeatherName == Volume_Weather_types.FlatClouds) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[6];
                            }
                            if (currentWeatherName == Volume_Weather_types.LightningStorm) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[7];
                            }
                            if (currentWeatherName == Volume_Weather_types.HeavyStorm) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[8];//7
                            }
                            if (currentWeatherName == Volume_Weather_types.HeavyStormDark) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[9];//7
                            }
                            if (currentWeatherName == Volume_Weather_types.Cloudy) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[10];//12
                            }
                            if (currentWeatherName == Volume_Weather_types.RollingFog) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[11];
                            }
                            if (currentWeatherName == Volume_Weather_types.VolcanoErupt) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[12];
                            }
                            if (currentWeatherName == Volume_Weather_types.Rain) {
                                Terrain_controller.FogPreset = VFogsPerVWeather[13];
                            }



                        }
                    } else if (Terrain_controller != null) {
                        //define fogs
                        if (currentWeatherName == Volume_Weather_types.Sunny) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[0];//0
                        }
                        if (currentWeatherName == Volume_Weather_types.Foggy) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[1];
                        }
                        if (currentWeatherName == Volume_Weather_types.HeavyFog) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[2];
                        }
                        if (currentWeatherName == Volume_Weather_types.Tornado) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[3];
                        }
                        if (currentWeatherName == Volume_Weather_types.SnowStorm) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[4];//7
                        }
                        if (currentWeatherName == Volume_Weather_types.FreezeStorm) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[5];
                        }
                        if (currentWeatherName == Volume_Weather_types.FlatClouds) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[6];
                        }
                        if (currentWeatherName == Volume_Weather_types.LightningStorm) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[7];
                        }
                        if (currentWeatherName == Volume_Weather_types.HeavyStorm) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[8];//7
                        }
                        if (currentWeatherName == Volume_Weather_types.HeavyStormDark) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[9];//7
                        }
                        if (currentWeatherName == Volume_Weather_types.Cloudy) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[10];//12
                        }
                        if (currentWeatherName == Volume_Weather_types.RollingFog) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[11];
                        }
                        if (currentWeatherName == Volume_Weather_types.VolcanoErupt) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[12];
                        }
                        if (currentWeatherName == Volume_Weather_types.Rain) {
                            Terrain_controller.FogPreset = VFogsPerVWeather[13];
                        }

                    }


                    if (water != null && Mesh_Terrain_controller != null) {
                        if (Hero != null && Hero.position.y < water.position.y) { //v3.3d
                                                                                  //let water script define fog
                        } else {
                            //define fogs
                            if (currentWeatherName == Volume_Weather_types.Sunny) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[0];//0
                            }
                            if (currentWeatherName == Volume_Weather_types.Foggy) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[1];
                            }
                            if (currentWeatherName == Volume_Weather_types.HeavyFog) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[2];
                            }
                            if (currentWeatherName == Volume_Weather_types.Tornado) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[3];
                            }
                            if (currentWeatherName == Volume_Weather_types.SnowStorm) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[4];//7
                            }
                            if (currentWeatherName == Volume_Weather_types.FreezeStorm) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[5];
                            }
                            if (currentWeatherName == Volume_Weather_types.FlatClouds) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[6];
                            }
                            if (currentWeatherName == Volume_Weather_types.LightningStorm) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[7];
                            }
                            if (currentWeatherName == Volume_Weather_types.HeavyStorm) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[8];//7
                            }
                            if (currentWeatherName == Volume_Weather_types.HeavyStormDark) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[9];//7
                            }
                            if (currentWeatherName == Volume_Weather_types.Cloudy) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[10];//12
                            }
                            if (currentWeatherName == Volume_Weather_types.RollingFog) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[11];
                            }
                            if (currentWeatherName == Volume_Weather_types.VolcanoErupt) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[12];
                            }
                            if (currentWeatherName == Volume_Weather_types.Rain) {
                                Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[13];
                            }

                        }
                    } else if (Mesh_Terrain_controller != null) {
                        //define fogs
                        if (currentWeatherName == Volume_Weather_types.Sunny) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[0];//0
                        }
                        if (currentWeatherName == Volume_Weather_types.Foggy) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[1];
                        }
                        if (currentWeatherName == Volume_Weather_types.HeavyFog) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[2];
                        }
                        if (currentWeatherName == Volume_Weather_types.Tornado) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[3];
                        }
                        if (currentWeatherName == Volume_Weather_types.SnowStorm) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[4];//7
                        }
                        if (currentWeatherName == Volume_Weather_types.FreezeStorm) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[5];
                        }
                        if (currentWeatherName == Volume_Weather_types.FlatClouds) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[6];
                        }
                        if (currentWeatherName == Volume_Weather_types.LightningStorm) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[7];
                        }
                        if (currentWeatherName == Volume_Weather_types.HeavyStorm) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[8];//7
                        }
                        if (currentWeatherName == Volume_Weather_types.HeavyStormDark) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[9];//7
                        }
                        if (currentWeatherName == Volume_Weather_types.Cloudy) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[10];//12
                        }
                        if (currentWeatherName == Volume_Weather_types.RollingFog) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[11];
                        }
                        if (currentWeatherName == Volume_Weather_types.VolcanoErupt) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[12];
                        }
                        if (currentWeatherName == Volume_Weather_types.Rain) {
                            Mesh_Terrain_controller.FogPreset = VFogsPerVWeather[13];
                        }

                    }


                    //Make sure to initialize weather (if previously used and is in steady state)
                    Weather.currentState = WeatherSM.Volume_Weather_State.Init;

                    //Asign after saving previous
                    prevWeatherName = currentWeatherName;

                    //if(prevWeather != null){
                    if (currentWeather != null) {
                        if (prevWeather == null) {
                            prevWeather = new WeatherSM();
                        }
                        currentWeather.currentState = WeatherSM.Volume_Weather_State.FadeOut;

                        if (currentWeather.VolumeScript != null) {
                            currentWeather.VolumeScript.cloned = true;//force cloned here to fade out
                        }

                        prevWeather = currentWeather;
                        prevWeather.currentState = WeatherSM.Volume_Weather_State.FadeOut; //set current system to fade out
                    }
                    currentWeather = Weather;
                }

                if (currentWeather != null) {
                    if (prevWeather != null) {
                        prevWeather.Update();
                    }
                    currentWeather.Update();
                }
            } else {
                //v2.2
                currentWeather = null;
            }
        }


        //v.2.0.1
        public float Max_sun_intensity = 1.3f;//0.67f; //v4.8.4
        public float Min_sun_intensity = 0.01f;
        public float To_max_intensity_speed = 0.5f; //speed towards the sun max intensity, between 9 morning and 23 dusk day times, slow to gradually come to max intensity
        public float To_min_intensity_speed = 1.5f; //fast to come at night sooner after sun goes down
        public bool Ortho_cam = false;
        public float Ortho_factor = -0.46f;
        [Range(-1.5f, 0)]
        public float Shift_dawn = 0;
        public float Rot_Sun_X;//v3.0 elevation
        public float Rot_Sun_Y = 170;
        public float Rot_Sun_Z = 180;
        float Previous_Rot_Y;
        float Previous_Rot_Z;
        float Previous_Rot_X;//v3.3

        //v1.7
        public float Moon_glow = 0.03f;
        public float Horizon_adj = 110;
        public float HorizonY = -60;
        public Color GroundColor = Color.grey * 1.4f;

        //v1.5
        public bool Unity5 = false;

        //v1.2.5
        public bool Mobile = false;
        public int Volume_fog_max_part = 150;
        public int Snow_cloud_max_part = 1000;
        public int Lightning_cloud_max_part = 1000;
        public int Real_cloud_max_part = 400;
        public int Storm_cloud_max_part = 600;
        public int Flat_cloud_max_part = 1500;

        public int Refract_Rain_max_part = 300;//v3.0.4
        public int Snow_max_part = 1200;//v3.0.4

        public GameObject SunObj;
        //[HideInInspector] //v2.1
        public GameObject SunObj2;
        public Vector3 DualSunsFactors = new Vector3(0.3f, 0.3f, 18);
        [HideInInspector]
        public bool alter_sun = false;
        //bool altered_sun=false;
        public GameObject MoonObj;

        //v3.0
        public Transform Hero;
        //PLAYER
        public bool Tag_based_player = false;//otherwise search for camera
        public string Player_tag = "Player";

        public Weather_types Weather = Weather_types.Sunny;
        Weather_types previous_weather = Weather_types.Sunny;
        public bool On_demand = false;

        public Material skyMat;

        //v1.5
        public Material skyboxMat; //added skybox mode, for full compatibility to Unity 5 and the IBL, GI features

        public enum Weather_types { Sunny, Foggy, HeavyFog, Tornado, SnowStorm, FreezeStorm, FlatClouds, LightningStorm, HeavyStorm, HeavyStormDark, Cloudy, RollingFog, VolcanoErupt, Rain }

        public Material cloud_upMaterial;
        public Material cloud_downMaterial;
        public Material flat_cloud_upMaterial;
        public Material flat_cloud_downMaterial;
        public Material cloud_dome_downMaterial;
        public Material star_dome_Material;
        public Material real_cloud_upMaterial;
        public Material real_cloud_downMaterial;
        public Material Surround_Clouds_Mat;

        public GameObject Sun_Ray_Cloud;
        public GameObject Upper_Dynamic_Cloud;
        public GameObject Lower_Dynamic_Cloud;
        public GameObject Upper_Cloud_Bed;
        public GameObject Lower_Cloud_Bed;
        public GameObject Upper_Cloud_Real;
        public GameObject Lower_Cloud_Real;
        public GameObject Upper_Static_Cloud;
        public GameObject Lower_Static_Cloud;
        public GameObject Cloud_Dome;
        public GameObject Surround_Clouds;
        public GameObject Surround_Clouds_Heavy;
        public GameObject SnowStorm_OBJ;
        public bool SnowStorm = false;//use to start a Snow Storm adjustements
        public GameObject[] FallingLeaves_OBJ;
        public GameObject Butterfly_OBJ;
        public GameObject[] Tornado_OBJs;
        public GameObject[] Butterfly3D_OBJ;
        public GameObject Ice_Spread_OBJ;
        public GameObject Ice_System_OBJ;
        public GameObject Lightning_System_OBJ;
        public GameObject Lightning_OBJ;//single lightning to instantiate 
        public GameObject Star_particles_OBJ;
        public GameObject[] Volcano_OBJ;
        public GameObject VolumeFog_OBJ;
        //public Transform Wind_OBJ;//v2.2

        public GameObject Rain_Heavy;
        public GameObject Rain_Mild;
        public bool Use_fog = false;

        //v3.0
        //Particle Clouds v3.0
        public GameObject VolumeRain_Heavy;
        public GameObject VolumeRain_Mild;
        public GameObject RefractRain_Heavy;
        public GameObject RefractRain_Mild;

        public bool Fog_local = false; // if local, parent and center systems to hero
        public bool Snow_local = false;
        public bool Mild_rain_local = false;
        public bool Heavy_rain_local = false;
        public bool Butterflies_local = false;

        public float m_fExposure = 0.8f;
        public Vector3 m_fWaveLength = new Vector3(0.65f, 0.57f, 0.475f); //light wave length
        public float m_ESun = 20.0f;            // Sun 
        public float m_Kr = 0.0025f;            // Rayleigh
        public float m_Km = 0.0010f;            // Mie
        public float m_g = -0.990f;             // Mie phase asymetry 
        public float scale_dif = 0.25f;
        public bool AltScale = false;
        public float OuterRadiusScaleOverInner;
        private float m_fInnerRadius;           // Radius of the sphere of the ground 
        private float m_fOuterRadius;           // Radius of the sphere of the sky 
        public float m_fSamples;
        public float m_fRayleighScaleDepth = 0.5f;  // Rayleigh scale

        public float m_Coloration = 0.28f;
        public Color m_TintColor = new Color(0, 0, 0, 0);

        public bool USE_ORK = false;
        public bool USE_SKYCUBE = false;
        public bool USE_SKYBOX = false;

        public SeasonalTerrainSKYMASTER Terrain_controller; //control terrain script
        public SeasonalTerrainSKYMASTER Mesh_Terrain_controller; //control terrain script
        public Transform Mesh_terrain;
        public Transform Unity_terrain;//v3.0

        public bool LegacySeasonalFX = false;//v3.0.3 - enable older Shuriken cloud based sample weather effects
        public bool Seasonal_change_auto = false;
        int Season_prev = 0;
        public int Season = 0;//0 for undefined (get from time), 1=spring, 2=summer, 3=autumn, 4=winter
        public float Seasonal = 0;
        public float Seasonal_prev = 0;
        public float Horizontal_factor = 0;
        public float Horizontal_factor_prev = 0;


        public float Sun_ring_factor = 0;
        public float Sun_halo_factor = 0; //0 - 0.015 range
        public float Sun_eclipse_factor = 0; // -1.6 (explosion - 0.4) Inverts at 0.004
        public float Glob_scale = 1;

        //SKYBOX
        float mSunSize;
        Vector4 mSunTint;
        float mSkyExponent;
        Vector4 mSkyTopColor;
        Vector4 mSkyMidColor;
        Vector4 mSkyEquatorColor;
        Vector4 mGroundColor;

        float Seasonal_factor1_add = 0; //Seasonal factors, change below and assign to sky and cloud color changes
        float Seasonal_factor2_add = 0;
        float Seasonal_factor3_add = 0;
        float Seasonal_factor4_add = 0;
        float Seasonal_factor5_add = 0;
        float Seasonal_factor6_add = 0;
        float Seasonal_factor7_add = 0;

        public float Fog_Density_Speed = 0.5f;

        //v3.0
        public float Fog_Density_Mult = 1f;

        public Color Autumn_fog_day = new Color(0.4f, 0.314f, 0.4f, 1);
        public Color Autumn_fog_dusk = new Color(0.4f, 0.314f, 0.4f, 1);
        public float Autumn_fog_day_density = 0.0001f;
        public float Autumn_fog_dusk_density = 0.0002f;
        public Color Autumn_fog_night = new Color(0.04f, 0.0314f, 0.04f, 1);
        public float Autumn_fog_night_density = 0.0001f;

        public Color Winter_fog_day = new Color(0.45f, 0.45f, 0.45f, 1);
        public Color Winter_fog_dusk = new Color(0.45f, 0.3514f, 0.45f, 1);
        public float Winter_fog_day_density = 0.0002f;
        public float Winter_fog_dusk_density = 0.0003f;
        public Color Winter_fog_night = new Color(0.045f, 0.03514f, 0.045f, 1);
        public float Winter_fog_night_density = 0.0002f;

        public Color Summer_fog_day = new Color(0.5f, 0.5f, 0.5f, 1);
        public Color Summer_fog_dusk = new Color(0.2f, 0.1f, 0.2f, 1);
        public float Summer_fog_day_density = 0.0001f;
        public float Summer_fog_dusk_density = 0.0002f;
        public Color Summer_fog_night = new Color(0.02f, 0.01f, 0.02f, 1);
        public float Summer_fog_night_density = 0.0001f;

        public Color Spring_fog_day = new Color(0.47f, 0.47f, 0.47f, 1);
        public Color Spring_fog_dusk = new Color(0.4f, 0.36f, 0.4f, 1);//v3.3c (0.1f,0.06f,0.1f,1)
        public float Spring_fog_day_density = 0.0001f;
        public float Spring_fog_dusk_density = 0.0002f;
        public Color Spring_fog_night = new Color(0.01f, 0.01f, 0.01f, 1);
        public float Spring_fog_night_density = 0.0001f;

        public Color Tree_Summer_Col = new Color(1, 0.69f, 0.035f, 1);
        public Color Terrain_Summer_Col = new Color(0.8f, 0.7f, 0.2f, 1);
        public Color Grass_Summer_Col = new Color(1, 0.71f, 0.004f, 1);

        public Color Tree_Spring_Col = new Color(0.52f, 0.773f, 0.27f, 1);
        public Color Terrain_Spring_Col = new Color(0.17f, 0.247f, 0, 1);
        public Color Grass_Spring_Col = new Color(0.243f, 0.27f, 0.157f, 1);

        public Color Tree_Winter_Col = new Color(0.028f, 0.028f, 0.028f, 1);
        public Color Terrain_Winter_Col = new Color(0.678f, 0.678f, 0.678f, 1);
        public Color Grass_Winter_Col = new Color(0.028f, 0.028f, 0.028f, 1);

        public Color Tree_Autumn_Col = new Color(0.866f, 0.03f, 0.105f, 1);
        public Color Terrain_Autumn_Col = new Color(0.27f, 0.055f, 0.04f, 1);
        public Color Grass_Autumn_Col = new Color(0.63f, 0.055f, 0.004f, 1);

        public Color Dusk_cloud_color = new Color(1, 0.4f, 0.2f, 0.5f);
        public Color Dusk_real_cloud_col_dn = new Color(1, 0.61f, 0.55f, 0.17f);
        public Color Dusk_real_cloud_col_up = new Color(0.855f, 0.384f, 0.384f, 0.192f);

        public Color Dusk_surround_cloud_col = new Color(0.921f, 0.47f, 0.57f, 0.51f);
        public Color Night_surround_cloud_col = new Color(0.494f, 0.533f, 1f, 0f);

        public Color Dusk_cloud_dome_color = new Color(1, 0.7f, 0.596f, 0.5f);

        public Color Night_Color = new Color(0, 0, 0, 0.1f);
        public Color Night_deep_Color = new Color(0, 0, 0, 0.05f);
        public Color Night_black_Color = new Color(0, 0, 0, 0.0f);
        public Color Night_lum_Color = new Color(0.05f, 0.05f, 0.05f, 0.7f);
        public Color Storm_cloud_Color = new Color(0.1f, 0.1f, 0.1f, 0.5f);

        public Color Day_Color = new Color(1, 1, 1, 0.5f);
        public Color Day_Sun_Color = new Color(0.933f, 0.933f, 0.933f, 1);
        public Color Day_Ambient_Color = new Color(0.4f, 0.4f, 0.4f, 1);//
        public Color Day_Tint_Color = new Color(0.2f, 0.2f, 0.2f, 1);//
        public Color Day_surround_cloud_col = new Color(0.11f, 0.1f, 0.1f, 0.6f);

        public Color Dusk_Sun_Color = new Color(0.933f, 0.596f, 0.443f, 1);
        public Color Dusk_Ambient_Color = new Color(0.2f, 0.2f, 0.2f, 0);
        public Color Dusk_Tint_Color = new Color(0.386f, 0, 0, 0);

        public Color Dawn_Sun_Color = new Color(0.933f, 0.933f, 0.933f, 1);
        public Color Dawn_Ambient_Color = new Color(0.35f, 0.35f, 0.35f, 1);//
        public Color Dawn_Tint_Color = new Color(0.2f, 0.05f, 0.1f, 0);//

        public Color Night_Sun_Color = new Color(0.01f, 0.01f, 0.01f, 1);
        public Color Night_Ambient_Color = new Color(0.03f, 0.03f, 0.03f, 0);
        public Color Night_Tint_Color = new Color(0, 0, 0, 0);

        //v1.7
        bool InitSeason = false;

        void Start()
        {

            //v3.4.2
            //initCamY = Camera.main.transform.position.y;

            //v3.3 - init galaxy
            //v3.3 - shader based stars fade
            if (StarsMaterial != null) {
                //Color StarsCol = StarsMaterial.GetColor ("_Color");
                if (
                    (!AutoSunPosition && (Current_Time >= (9 + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn)))
                    |
                    (AutoSunPosition && Rot_Sun_X > 0))
                {
                    if (Current_Time < 1f) { //initialize at game start
                        StarsMaterial.SetColor("_Color", new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.y));
                    } else {
                        StarsMaterial.SetColor("_Color", new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.y));
                    }
                } else {
                    if (Current_Time < 1f) { //initialize at game start
                        StarsMaterial.SetColor("_Color", new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.x));
                    } else {
                        StarsMaterial.SetColor("_Color", new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.x));
                    }
                }
                //float StarsCover = StarsMaterial.GetFloat ("_Light");
                StarsMaterial.SetFloat("_Light", StarsIntensity);
            }

            //////////////////////// INIT SCREEN WATER DROPS MATERIAL - v3.0
            if (ScreenRainDrops && RainDropsPlane != null && ScreenRainDropsMat != null) {
                if (currentWeatherName == Volume_Weather_types.HeavyStorm | currentWeatherName == Volume_Weather_types.HeavyStormDark
                   | currentWeatherName == Volume_Weather_types.Rain) {

                    if (ScreenFreezeFX) {
                        //decrease speed to zero
                        //			float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                        ScreenRainDropsMat.SetFloat("_Speed", 0);
                        //increase water from zero to MaxWater
                        //			float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                        ScreenRainDropsMat.SetFloat("_WaterAmount", MaxWater);
                        //increase refraction from zero to MaxRefract * 4
                        //			float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                        ScreenRainDropsMat.SetFloat("_BumpAmt", MaxRefract * 4);

                        if (FreezeInwards) {
                            //				float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorA", 0);
                            //				float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorB", 0);
                            //				float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorC", 1);
                        } else {
                            //				float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorA", 1);
                            //				float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorB", 1);
                            //				float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorC", 0);
                        }
                    } else {
                        //increase speed from zero to MaxDropSpeed, MaxWater, MaxRefract
                        //			float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                        ScreenRainDropsMat.SetFloat("_Speed", MaxDropSpeed);
                        //increase water from zero to MaxWater
                        //			float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                        ScreenRainDropsMat.SetFloat("_WaterAmount", MaxWater);
                        //increase refraction from zero to MaxRefract
                        //			float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                        ScreenRainDropsMat.SetFloat("_BumpAmt", MaxRefract);
                    }

                } else {//if not raining, fade out
                    if (ScreenFreezeFX | currentWeatherName == Volume_Weather_types.FreezeStorm) {
                        //decrease speed to zero
                        //			float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                        ScreenRainDropsMat.SetFloat("_Speed", 0);
                        //increase water from zero to MaxWater
                        //			float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                        ScreenRainDropsMat.SetFloat("_WaterAmount", MaxWater);
                        //increase refraction from zero to MaxRefract * 4
                        //			float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                        ScreenRainDropsMat.SetFloat("_BumpAmt", MaxRefract * 4);

                        if (FreezeInwards) {
                            //				float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorA", 0);
                            //				float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorB", 0);
                            //				float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorC", 1);
                        } else {
                            //				float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorA", 1);
                            //				float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorB", 1);
                            //				float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                            ScreenRainDropsMat.SetFloat("_FreezeFacrorC", 0);
                        }
                    } else {
                        //decrease speed to zero to MaxDropSpeed, MaxWater, MaxRefract
                        //float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                        //ScreenRainDropsMat.SetFloat("_Speed",Mathf.Lerp(Drops_speed, MaxDropSpeed, Time.deltaTime));
                        //increase speed from zero to MaxWater
                        //			float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                        ScreenRainDropsMat.SetFloat("_WaterAmount", 0);
                        //increase refraction from zero to MaxRefract
                        //			float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                        ScreenRainDropsMat.SetFloat("_BumpAmt", 0);
                    }
                }
            }

            //3.0
            //MOON PHASES Init
            if (MoonPhases && MoonPhasesMat != null) {
                Moon_glow = 0.43f;
            }

            //v2.2
            //v2.2 - Shader based cloud dome (2 layers)
            Color ShaderCloudDomeC1 = Color.white;
            Color ShaderCloudDomeC2 = Color.white;
            float ShaderCloudDomeCoverage1 = 0;
            float ShaderCloudDomeCoverage2 = 0;
            Color ShaderCloudDomeC1A = Color.white;
            Color ShaderCloudDomeC2A = Color.white;
            if (CloudDomeL1Mat != null & CloudDomeL2Mat != null) {
                ShaderCloudDomeC1 = CloudDomeL1Mat.GetVector("_Color");
                ShaderCloudDomeC2 = CloudDomeL2Mat.GetVector("_Color");
                ShaderCloudDomeCoverage1 = CloudDomeL1Mat.GetFloat("_CloudCover");
                ShaderCloudDomeCoverage2 = CloudDomeL2Mat.GetFloat("_CloudCover");
                ShaderCloudDomeC1A = CloudDomeL1Mat.GetVector("_Ambient");
                ShaderCloudDomeC2A = CloudDomeL2Mat.GetVector("_Ambient");

                //extra controls for dome L1
                CloudDomeL1Mat.SetFloat("_CloudDensity", L1CloudDensOffset);
                CloudDomeL1Mat.SetFloat("_CloudSize", L1CloudSize);
                CloudDomeL1Mat.SetFloat("_AmbientFactor", L1Ambience);

                //CloudDomeL1Mat.SetVector("_CloudSpeed",new Vector4(0, Mathf.Lerp(CloudDomeL1Mat.GetVector("_CloudSpeed").y, -windZone.transform.forward.z*windZone.windMain/50,Time.deltaTime*500),0,0));
                if (windZone != null) {
                    //v3.1
                    Vector3 Flatten_wind = new Vector3(windZone.transform.forward.x, 0, windZone.transform.forward.z);
                    CloudDomeL1.transform.forward = Flatten_wind;
                    //CloudDomeL1.transform.forward = windZone.transform.forward;
                }

            }
            if (currentWeatherName == Volume_Weather_types.Sunny) {
                Shader.SetGlobalFloat(SnowCoverageVariable, 0);
                if (SnowMatTerrain != null) {
                    SnowMatTerrain.SetFloat(SnowCoverageVariable, 0);
                }
            }

            //// RESER CLOUD DOME
            //v2.2 - Shader based cloud dome control - DAY
            float Trans_dome_color = 0.8f;
            float Color_devider2 = 1;
            if (CloudDomeL1Mat != null & CloudDomeL2Mat != null) {

                if (currentWeatherName == Volume_Weather_types.HeavyStorm) {

                    Color Dark_storm_L1C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                    Color Dark_storm_L1CA = new Color(98f / 255f, 98f / 255f, 98f / 255f, 234f / 255f);

                    Color Dark_storm_L2C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                    //Color Dark_storm_L2CA = new Color(155f/255f,150f/255f,161f/255f,254f/255f);

                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dark_storm_L1C.r, Dark_storm_L1C.g, Dark_storm_L1C.b, Trans_dome_color), 0.15f * Time.deltaTime));
                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dark_storm_L2C.r, Dark_storm_L2C.g, Dark_storm_L2C.b, Trans_dome_color), 0.15f * Time.deltaTime));
                    CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1.09f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                    CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1.08f, 0.5f * Time.deltaTime));

                    CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(Dark_storm_L1CA.r, Dark_storm_L1CA.g, Dark_storm_L1CA.b, Trans_dome_color * 0.55f), 0.15f * Time.deltaTime));
                    CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(Dark_storm_L2CA.r, Dark_storm_L2CA.g, Dark_storm_L2CA.b, Dark_storm_L2CA.a), 0.55f * Time.deltaTime));

                } else {

                    if (Season == 0 | Season == 1) {
                        CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Day_L1_dome_color.r, Day_L1_dome_color.g, Day_L1_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                        CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Day_L2_dome_color.r, Day_L2_dome_color.g, Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));

                        //v3.3d
                        if (currentWeatherName == Volume_Weather_types.Sunny) {
                            CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0, 1));
                            CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 1));
                        } else {
                            CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.82f + L1CloudCoverOffset, 1));
                            //CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2,1,0.5f*Time.deltaTime));
                            CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 1));
                        }

                        //CloudDomeL1Mat.SetFloat("_CloudCover",0);
                        //CloudDomeL2Mat.SetFloat("_CloudCover", 0);
                    } else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand)) {
                        CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                        CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                    } else {
                        CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Day_L1_dome_color.r, Day_L1_dome_color.g, Day_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                        CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Day_L2_dome_color.r, Day_L2_dome_color.g, Day_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.8f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0.693f, 0.5f * Time.deltaTime));
                    }

                    CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(Day_L1_dome_color.r, Day_L1_dome_color.g, Day_L1_dome_color.b, Trans_dome_color * 0.55f), 0.15f * Time.deltaTime));
                    CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(Day_L2_dome_color.r, Day_L2_dome_color.g, Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                }
            }


            if (currentWeatherName == Volume_Weather_types.HeavyStorm) {
                Shader.SetGlobalFloat(SnowCoverageVariable, SnowCoverage);
                if (SnowMatTerrain != null) {
                    SnowMatTerrain.SetFloat(SnowCoverageVariable, SnowCoverageTerrain / divideSnowTerrain);
                }
            }
            currentWeather = null;

            //v1.7
            InitSeason = false;

            //v1.2.5
            if (Terrain.activeTerrain != null) {
                Terrain_controller = Terrain.activeTerrain.gameObject.GetComponent(typeof(SeasonalTerrainSKYMASTER)) as SeasonalTerrainSKYMASTER;
            }
            if (Mesh_terrain != null) {
                Mesh_Terrain_controller = Mesh_terrain.gameObject.GetComponent(typeof(SeasonalTerrainSKYMASTER)) as SeasonalTerrainSKYMASTER;
            }

            float radius = transform.localScale.x;

            if (DefinePlanetScale) {
                radius = PlanetScale;
            }

            m_fInnerRadius = radius;
            OuterRadiusScaleOverInner = 1 + (scale_dif / 10);
            m_fOuterRadius = OuterRadiusScaleOverInner * radius;


            //ORK
            if (USE_ORK) {

                //UNCOMMENT FOR ORK FRAMEWORK !!!

                //			//v3.1
                //			//start time
                //			// if global variable not exist, create it
                //			if (ORK.Game.Variables.GetFloat("TIME") != null)
                //			//if (GameHandler.GetNumberVariable("TIME") != null) //ORK1
                //			{
                //				Current_Time = ORK.Game.Variables.GetFloat("TIME");
                //			}
                //			else{
                //				ORK.Game.Variables.Set("TIME",Current_Time);
                //				//GameHandler.SetNumberVariable("TIME",7);//ORK1
                //			}
                //			
                //				if (ORK.Game.Variables.GetFloat("TIME_MONTH") != null)
                //				//if (GameHandler.GetNumberVariable("TIME_DAY") != null)//ORK1
                //				{
                //					Current_Month = (int)ORK.Game.Variables.GetFloat("TIME_MONTH");
                //				}
                //				else{
                //					ORK.Game.Variables.Set("TIME_MONTH",Current_Month);
                //					//GameHandler.SetNumberVariable("TIME_DAY",0);//ORK1
                //				}	
                //
                //			if (ORK.Game.Variables.GetFloat("TIME_DAY") != null)
                //			//if (GameHandler.GetNumberVariable("TIME_DAY") != null)//ORK1
                //			{
                //				Current_Day = ORK.Game.Variables.GetFloat("TIME_DAY");
                //			}
                //			else{
                //				ORK.Game.Variables.Set("TIME_DAY",Current_Day);
                //				//GameHandler.SetNumberVariable("TIME_DAY",0);//ORK1
                //			}	
            }

            if (SUN_LIGHT != null) {//v3.4
                MAIN = SUN_LIGHT.GetComponent("Light") as Light;
            } else {
                Debug.Log("Please add a light to the 'SUN_LIGHT' variable");
            }


            //ORK
            //SUPPORT = SUPPORT_LIGHT.GetComponent("Light") as Light;
            //		KEEP_INITIAL_SUN_LIGHT_transform_eulerAngles = MAIN.transform.eulerAngles;
            //		KEEP_INITIAL_SUPPORT_LIGHT_transform_eulerAngles = SUPPORT.transform.eulerAngles;		
            //		KEEP_INITIAL_SUN_LIGHT_color= MAIN.color;
            //		KEEP_INITIAL_SUPPORT_LIGHT_color= SUPPORT.color;		
            //		KEEP_INITIAL_SUPPORT_LIGHT_intensity= SUPPORT.intensity;
            //		KEEP_INITIAL_SUN_LIGHT_intensity= MAIN.intensity;		
            //		//FOG		
            //		KEEP_START_FOG_COLOR = RenderSettings.fogColor;
            //		KEEP_START_FOG_DENSITY = RenderSettings.fogDensity;

            ///// TOD
            //	Current_Time =12;
            //	Keep_previous_time=12;
            Keep_previous_time = Current_Time;
            Keep_previous_dawn_shift = Shift_dawn; //v2.0.1
            Previous_Rot_Y = Rot_Sun_Y;
            Previous_Rot_Z = Rot_Sun_Z;
            Previous_Rot_X = Rot_Sun_X;//v3.3

            //SKYBOX
            if (USE_SKYCUBE) {
                //render to cubemap and apply to skybox and sphere to test

                //END SKYBOX
                CubeTexture = new Cubemap(256, TextureFormat.ARGB32, false);
                //Shader CUBE_Shader = Shader.Find("RenderFX/Skybox Cubed");  
                CUBE_Mat.SetTexture("_Tex", CubeTexture);

                //v3.3b
                if (SkyDomeSystem != null) {
                    RenderSettings.skybox = CUBE_Mat;
                }
            }

            //SEASONAL - GATHER PARTICLE SYSTEMS and CACHE them
            //BUTTERFLIES
            if (Butterfly_OBJ != null) {
                Butterfly_OBJ.SetActive(true);
                Component[] Butterflies2D = Butterfly_OBJ.GetComponentsInChildren(typeof(ParticleSystem));
                if (Butterflies2D != null) {
                    Butterflies2D_P = new ParticleSystem[Butterflies2D.Length];
                    for (int n = 0; n < Butterflies2D.Length; n++) {
                        Butterflies2D_P[n] = Butterflies2D[n].GetComponent<ParticleSystem>();
                    }
                }
                Butterfly_OBJ.SetActive(false);
            }

            //UPPER DYNAMIC CLOUDS
            if (Upper_Dynamic_Cloud != null) {
                Upper_Dynamic_Cloud.SetActive(true);
                Component[] Dynamic_Clouds_Up = Upper_Dynamic_Cloud.GetComponentsInChildren(typeof(ParticleSystem));
                if (Dynamic_Clouds_Up != null) {
                    Dynamic_Clouds_Up_P = new ParticleSystem[Dynamic_Clouds_Up.Length];
                    for (int n = 0; n < Dynamic_Clouds_Up.Length; n++) {
                        Dynamic_Clouds_Up_P[n] = Dynamic_Clouds_Up[n].GetComponent<ParticleSystem>();
                    }
                }
                Upper_Dynamic_Cloud.SetActive(false);
            }

            //LOWER DYNAMIC CLOUDS
            if (Lower_Dynamic_Cloud != null) {
                Lower_Dynamic_Cloud.SetActive(true);
                Component[] Dynamic_Clouds_Dn = Lower_Dynamic_Cloud.GetComponentsInChildren(typeof(ParticleSystem));
                if (Dynamic_Clouds_Dn != null) {
                    Dynamic_Clouds_Dn_P = new ParticleSystem[Dynamic_Clouds_Dn.Length];
                    for (int n = 0; n < Dynamic_Clouds_Dn.Length; n++) {
                        Dynamic_Clouds_Dn_P[n] = Dynamic_Clouds_Dn[n].GetComponent<ParticleSystem>();
                    }
                }
                Lower_Dynamic_Cloud.SetActive(false);
            }

            //REAL LOWER CLOUDS
            if (Lower_Cloud_Real != null) {
                Lower_Cloud_Real.SetActive(true);
                Component[] Real_Clouds_Dn = Lower_Cloud_Real.GetComponentsInChildren(typeof(ParticleSystem));
                if (Real_Clouds_Dn != null) {
                    Real_Clouds_Dn_P = new ParticleSystem[Real_Clouds_Dn.Length];
                    for (int n = 0; n < Real_Clouds_Dn.Length; n++) {
                        Real_Clouds_Dn_P[n] = Real_Clouds_Dn[n].GetComponent<ParticleSystem>();
                    }
                }
                Lower_Cloud_Real.SetActive(false);
            }

            //REAL UPPER CLOUDS
            if (Upper_Cloud_Real != null) {
                Upper_Cloud_Real.SetActive(true);
                Component[] Real_Clouds_Up = Upper_Cloud_Real.GetComponentsInChildren(typeof(ParticleSystem));
                if (Real_Clouds_Up != null) {
                    Real_Clouds_Up_P = new ParticleSystem[Real_Clouds_Up.Length];
                    for (int n = 0; n < Real_Clouds_Up.Length; n++) {
                        Real_Clouds_Up_P[n] = Real_Clouds_Up[n].GetComponent<ParticleSystem>();
                    }
                }
                Upper_Cloud_Real.SetActive(false);
            }
            /////// CLOUD BED ////////

            //LOWER CLOUD BED
            if (Lower_Cloud_Bed != null) {
                Lower_Cloud_Bed.SetActive(true);
                Component[] Bed_Clouds_Dn = Lower_Cloud_Bed.GetComponentsInChildren(typeof(ParticleSystem));
                if (Bed_Clouds_Dn != null) {
                    Cloud_bed_Dn_P = new ParticleSystem[Bed_Clouds_Dn.Length];
                    for (int n = 0; n < Bed_Clouds_Dn.Length; n++) {
                        Cloud_bed_Dn_P[n] = Bed_Clouds_Dn[n].GetComponent<ParticleSystem>();
                    }
                }
                Lower_Cloud_Bed.SetActive(false);
            }

            //UPPER CLOUD BED
            if (Upper_Cloud_Bed != null) {
                Upper_Cloud_Bed.SetActive(true);
                Component[] Bed_Clouds_Up = Upper_Cloud_Bed.GetComponentsInChildren(typeof(ParticleSystem));
                if (Bed_Clouds_Up != null) {
                    Cloud_bed_Up_P = new ParticleSystem[Bed_Clouds_Up.Length];
                    for (int n = 0; n < Bed_Clouds_Up.Length; n++) {
                        Cloud_bed_Up_P[n] = Bed_Clouds_Up[n].GetComponent<ParticleSystem>();
                    }
                }
                Upper_Cloud_Bed.SetActive(false);
            }
            /////// STATIC CLOUDS ///////

            //LOWER CLOUD BED
            if (Lower_Static_Cloud != null) {
                Lower_Static_Cloud.SetActive(true);
                Component[] Bed_Static_Dn = Lower_Static_Cloud.GetComponentsInChildren(typeof(ParticleSystem));
                if (Bed_Static_Dn != null) {
                    Cloud_Static_Dn_P = new ParticleSystem[Bed_Static_Dn.Length];
                    for (int n = 0; n < Bed_Static_Dn.Length; n++) {
                        Cloud_Static_Dn_P[n] = Bed_Static_Dn[n].GetComponent<ParticleSystem>();
                    }
                }
                Lower_Static_Cloud.SetActive(false);
            }

            //UPPER CLOUD BED
            if (Upper_Static_Cloud != null) {
                Upper_Static_Cloud.SetActive(true);
                Component[] Bed_Static_Up = Upper_Static_Cloud.GetComponentsInChildren(typeof(ParticleSystem));
                if (Bed_Static_Up != null) {
                    Cloud_Static_Up_P = new ParticleSystem[Bed_Static_Up.Length];
                    for (int n = 0; n < Bed_Static_Up.Length; n++) {
                        Cloud_Static_Up_P[n] = Bed_Static_Up[n].GetComponent<ParticleSystem>();
                    }
                }
                Upper_Static_Cloud.SetActive(false);
            }

            //SURROUND CLOUDS
            if (Surround_Clouds != null) {
                Surround_Clouds.SetActive(true);
                Component[] Surround_CloudsC = Surround_Clouds.GetComponentsInChildren(typeof(ParticleSystem));
                if (Surround_CloudsC != null) {
                    Surround_Clouds_P = new ParticleSystem[Surround_CloudsC.Length];
                    for (int n = 0; n < Surround_CloudsC.Length; n++) {
                        Surround_Clouds_P[n] = Surround_CloudsC[n].GetComponent<ParticleSystem>();
                    }
                }
                Surround_Clouds.SetActive(false);
            }

            //SURROUND CLOUDS HEAVY
            if (Surround_Clouds_Heavy != null) {
                Surround_Clouds_Heavy.SetActive(true);
                Component[] Surround_Clouds_HeavyC = Surround_Clouds_Heavy.GetComponentsInChildren(typeof(ParticleSystem));
                if (Surround_Clouds_HeavyC != null) {
                    Surround_Clouds_Heavy_P = new ParticleSystem[Surround_Clouds_HeavyC.Length];
                    for (int n = 0; n < Surround_Clouds_HeavyC.Length; n++) {
                        Surround_Clouds_Heavy_P[n] = Surround_Clouds_HeavyC[n].GetComponent<ParticleSystem>();
                    }
                }
                Surround_Clouds_Heavy.SetActive(false);
            }

            /////// TORNADOS ///////
            if (Tornado_OBJs != null) {
                Tornados_P = new ParticleSystem[Tornado_OBJs.Length];
                for (int n = 0; n < Tornado_OBJs.Length; n++) {
                    Tornado_OBJs[n].SetActive(true);
                    Component Tornado_Particles = Tornado_OBJs[n].GetComponent(typeof(ParticleSystem));
                    if (Tornado_Particles != null) {
                        Tornados_P[n] = Tornado_Particles.GetComponent<ParticleSystem>();
                    }
                    Tornado_OBJs[n].SetActive(false);
                }
            }

            if (Ice_Spread_OBJ != null) {
                /////// ICE SPREAD SYSTEMS ///////
                Ice_Spread_OBJ.SetActive(true);
                Component[] Ice_spreaders = Ice_Spread_OBJ.GetComponentsInChildren(typeof(ParticleSystem));
                if (Ice_spreaders != null) {
                    Freezer_P = new ParticleSystem[Ice_spreaders.Length];
                    for (int n = 0; n < Ice_spreaders.Length; n++) {
                        Freezer_P[n] = Ice_spreaders[n].GetComponent<ParticleSystem>();
                    }
                }
                Ice_Spread_OBJ.SetActive(false);
            }

            /////// SNOW STORM ///////
            if (SnowStorm_OBJ != null) {
                SnowStorm_OBJ.SetActive(true);
                Component[] Snow_Storm_C = SnowStorm_OBJ.GetComponentsInChildren(typeof(ParticleSystem));
                if (Snow_Storm_C != null) {
                    SnowStorm_P = new ParticleSystem[Snow_Storm_C.Length];
                    for (int n = 0; n < Snow_Storm_C.Length; n++) {
                        SnowStorm_P[n] = Snow_Storm_C[n].GetComponent<ParticleSystem>();
                    }
                }
                SnowStorm_OBJ.SetActive(false);
            }

            /////// VOLUME FOG ///////
            if (VolumeFog_OBJ != null) {
                VolumeFog_OBJ.SetActive(true);
                Component[] Vol_Fog_C = VolumeFog_OBJ.GetComponentsInChildren(typeof(ParticleSystem));
                if (Vol_Fog_C != null) {
                    VolumeFog_P = new ParticleSystem[Vol_Fog_C.Length];
                    for (int n = 0; n < Vol_Fog_C.Length; n++) {
                        VolumeFog_P[n] = Vol_Fog_C[n].GetComponent<ParticleSystem>();
                    }
                }
                VolumeFog_OBJ.SetActive(false);
            }

            /////// FALLING LEAVES ///////
            if (FallingLeaves_OBJ != null) {
                FallingLeaves_P = new ParticleSystem[FallingLeaves_OBJ.Length];
                for (int n = 0; n < FallingLeaves_OBJ.Length; n++) {
                    FallingLeaves_OBJ[n].SetActive(true);
                    Component Leaf_Particles = FallingLeaves_OBJ[n].GetComponent(typeof(ParticleSystem));
                    if (Leaf_Particles != null) {
                        FallingLeaves_P[n] = Leaf_Particles.GetComponent<ParticleSystem>();
                    }
                    FallingLeaves_OBJ[n].SetActive(false);
                }
            }

            /////// 3D BUTTERFLIES ///////
            if (Butterfly3D_OBJ != null) {
                if (Butterfly3D_OBJ.Length > 0) {
                    Butterfly3D_OBJ[0].SetActive(true);
                    Component[] Butterfly3D_OBJ_C = Butterfly3D_OBJ[0].GetComponentsInChildren(typeof(ParticleSystem));
                    if (Butterfly3D_OBJ_C != null) {
                        Butterflies3D_P = new ParticleSystem[Butterfly3D_OBJ_C.Length];
                        for (int n = 0; n < Butterfly3D_OBJ_C.Length; n++) {
                            Butterflies3D_P[n] = Butterfly3D_OBJ_C[n].GetComponent<ParticleSystem>();
                        }
                    }
                    Butterfly3D_OBJ[0].SetActive(false);
                }
            }

            /////// VOLCANOS ///////
            if (Volcano_OBJ != null) {
                Volcanos_P = new ParticleSystem[Volcano_OBJ.Length * 5];
                int counter_volc = 0;
                for (int n = 0; n < Volcano_OBJ.Length; n++) {
                    Volcano_OBJ[n].SetActive(true);
                    Component[] Volcanic_Particles = Volcano_OBJ[n].GetComponentsInChildren(typeof(ParticleSystem));
                    if (Volcanic_Particles != null) {
                        Volcanos_P[counter_volc] = Volcanic_Particles[0].GetComponent<ParticleSystem>();
                        Volcanos_P[counter_volc + 1] = Volcanic_Particles[1].GetComponent<ParticleSystem>();
                        Volcanos_P[counter_volc + 2] = Volcanic_Particles[2].GetComponent<ParticleSystem>();
                        Volcanos_P[counter_volc + 3] = Volcanic_Particles[3].GetComponent<ParticleSystem>();
                        Volcanos_P[counter_volc + 4] = Volcanic_Particles[4].GetComponent<ParticleSystem>();
                        counter_volc = counter_volc + 5;
                    }
                    Volcano_OBJ[n].SetActive(false);
                }
            }
            ////////// RAIN ////////////////
            Heavy_Rain_P = new ParticleSystem[1];
            Mild_Rain_P = new ParticleSystem[1];
            Sun_Ray_Cloud_P = new ParticleSystem[1];
            Lightning_System_P = new ParticleSystem[1];
            if (Rain_Heavy != null) {
                //Heavy_Rain_P[0] = Rain_Heavy.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                Heavy_Rain_P = Rain_Heavy.GetComponentsInChildren<ParticleSystem>(true);
            }
            if (VolumeRain_Heavy != null) {
                //VolumeHeavy_Rain_P[0] = VolumeRain_Heavy.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                VolumeHeavy_Rain_P = VolumeRain_Heavy.GetComponentsInChildren<ParticleSystem>(true);
            }
            if (VolumeRain_Mild != null) {
                //VolumeMild_Rain_P[0] = VolumeRain_Mild.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                VolumeMild_Rain_P = VolumeRain_Mild.GetComponentsInChildren<ParticleSystem>(true);
            }
            if (RefractRain_Heavy != null) {
                //RefractHeavy_Rain_P[0] = RefractRain_Heavy.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                RefractHeavy_Rain_P = RefractRain_Heavy.GetComponentsInChildren<ParticleSystem>(true);
            }
            if (RefractRain_Mild != null) {
                //RefractMild_Rain_P[0] = RefractRain_Mild.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                RefractMild_Rain_P = RefractRain_Mild.GetComponentsInChildren<ParticleSystem>(true);
            }
            if (Rain_Mild != null) {
                //Mild_Rain_P[0] = Rain_Mild.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                Mild_Rain_P = Rain_Mild.GetComponentsInChildren<ParticleSystem>(true);
            }
            if (Sun_Ray_Cloud != null) {
                //Sun_Ray_Cloud_P[0] = Sun_Ray_Cloud.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
                Sun_Ray_Cloud_P = Sun_Ray_Cloud.GetComponentsInChildren<ParticleSystem>(true);
            }
            if (Star_particles_OBJ != null) {
                Star_particles_OBJ_P = Star_particles_OBJ.GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
            }

            if (Lightning_System_OBJ != null) {
                Lightning_System_OBJ.SetActive(true);
                Component[] Lightning_System_P_C = Lightning_System_OBJ.GetComponentsInChildren(typeof(ParticleSystem));
                if (Lightning_System_P_C != null & Lightning_System_P_C.Length > 0) {
                    Lightning_System_P[0] = Lightning_System_P_C[0].GetComponent<ParticleSystem>();
                    //Lightning_System_P = Lightning_System_P_C.GetComponentsInChildren<ParticleSystem>(true);
                }
                Lightning_System_OBJ.SetActive(false);
            }
            //Debug.Log (Lightning_System_P.Length);

            //v1.7
            //if (Hero == null) {
            //	Hero = Camera.main.transform;
            //}

            //v3.0
            if (Tag_based_player) {
                if (Hero == null) {
                    //v3.3d
                    if (GameObject.FindGameObjectWithTag(Player_tag) != null) {
                        Hero = (GameObject.FindGameObjectWithTag(Player_tag)).transform;
                    }
                }
            } else {
                if (Hero == null) {
                    Hero = Camera.main.transform;
                }
            }

            ////////// HERO local weather 
            if (Hero != null & Application.isPlaying) {
                if (Butterflies_local) {
                    if (Butterfly_OBJ != null) {
                        Butterfly_OBJ.transform.parent = Hero;
                        Butterfly_OBJ.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                }
                if (Fog_local) {
                    //Butterfly_OBJ.transform.parent = Hero;
                    if (VolumeFog_OBJ != null) {
                        VolumeFog_OBJ.transform.parent = Hero;
                        VolumeFog_OBJ.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                }
                if (Heavy_rain_local) {
                    if (Rain_Heavy != null) {
                        Rain_Heavy.transform.parent = Hero;
                        Rain_Heavy.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                    if (VolumeRain_Heavy != null) {//v3.0
                        VolumeRain_Heavy.transform.parent = Hero;
                        VolumeRain_Heavy.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                    if (RefractRain_Heavy != null) {
                        RefractRain_Heavy.transform.parent = Hero;
                        //RefractRain_Heavy.transform.position = new Vector3(Hero.position.x,RefractRain_Heavy.transform.position.y,Hero.position.z);
                        RefractRain_Heavy.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                }
                if (Mild_rain_local) {
                    if (Rain_Mild != null) {
                        Rain_Mild.transform.parent = Hero;
                        Rain_Mild.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                    if (VolumeRain_Mild != null) {//v3.0
                        VolumeRain_Mild.transform.parent = Hero;
                        VolumeRain_Mild.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                    if (RefractRain_Mild != null) {
                        RefractRain_Mild.transform.parent = Hero;
                        RefractRain_Mild.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                }
                if (Snow_local) {
                    if (SnowStorm_OBJ != null) {
                        SnowStorm_OBJ.transform.parent = Hero;
                        SnowStorm_OBJ.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                    if (Ice_Spread_OBJ != null) {
                        Ice_Spread_OBJ.transform.parent = Hero;
                        Ice_Spread_OBJ.transform.position = new Vector3(Hero.position.x, Hero.position.y + RainDistAboveHero, Hero.position.z);
                    }
                }
            }

            //v1.6.5
            //InitCam = Camera.main.transform.position;
            InitCam = new Vector3(62.67624f, 10016, 107.306f);
            //cam_pos = Camera.main.transform.position;
            //InitCamPos = cam_pos;
            //Debug.Log (Hero.position);

            Update(); //v3.4.7
            Run_presets();


            /////////////////////////// CHANGE FOG PER SEASON ////////////////////
            if (!Seasonal_change_auto & !On_demand) {
                if (Season == 0 | Season == 1) {
                    if (Use_fog) {
                        if (!RenderSettings.fog) {
                            RenderSettings.fog = true;
                        }
                        Color Fog_color = Spring_fog_day;
                        float Fog_density = Spring_fog_day_density * Fog_Density_Mult;
                        if (Current_Time < 18 & Current_Time > 9) {
                            Fog_color = Spring_fog_day;
                            Fog_density = Spring_fog_day_density * Fog_Density_Mult;
                        } else if (Current_Time <= 9 & Current_Time > 1) {
                            Fog_color = Spring_fog_night;
                            Fog_density = Spring_fog_night_density * Fog_Density_Mult;
                        } else {
                            Fog_color = Spring_fog_dusk;
                            Fog_density = Spring_fog_dusk_density * Fog_Density_Mult;
                        }
                        FogMode Fog_Mode = fogMode;// FogMode.ExponentialSquared;
                        float Density_Speed = Fog_Density_Speed;
                        //Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                        Make_Fog_Appear(Fog_color, Fog_density, Fog_Mode, Density_Speed, 0);
                        //Debug.Log ("sasasa");
                    } else {
                        if (RenderSettings.fog) {
                            RenderSettings.fog = false;
                        }
                    }
                } else
                    if (Season == 2) {
                    if (Use_fog) {
                        if (!RenderSettings.fog) {
                            RenderSettings.fog = true;
                        }
                        Color Fog_color = Summer_fog_day;
                        float Fog_density = Summer_fog_day_density * Fog_Density_Mult;
                        if (Current_Time < 18 & Current_Time > 9) {
                            Fog_color = Summer_fog_day;
                            Fog_density = Summer_fog_day_density * Fog_Density_Mult;
                        } else if (Current_Time <= 9 & Current_Time > 1) {
                            Fog_color = Summer_fog_night;
                            Fog_density = Summer_fog_night_density * Fog_Density_Mult;
                        } else {
                            Fog_color = Summer_fog_dusk;
                            Fog_density = Summer_fog_dusk_density * Fog_Density_Mult;
                        }
                        FogMode Fog_Mode = fogMode;// FogMode.ExponentialSquared;
                        float Density_Speed = Fog_Density_Speed;
                        //Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                        Make_Fog_Appear(Fog_color, Fog_density, Fog_Mode, Density_Speed, 0);

                    } else {
                        if (RenderSettings.fog) {
                            RenderSettings.fog = false;
                        }
                    }
                } else
                        if (Season == 3) {
                    if (Use_fog) {
                        if (!RenderSettings.fog) {
                            RenderSettings.fog = true;
                        }
                        Color Fog_color = Autumn_fog_day;
                        float Fog_density = Autumn_fog_day_density * Fog_Density_Mult;
                        if (Current_Time < 18 & Current_Time > 9) {
                            Fog_color = Autumn_fog_day;
                            Fog_density = Autumn_fog_day_density * Fog_Density_Mult;
                        } else if (Current_Time <= 9 & Current_Time > 1) {
                            Fog_color = Autumn_fog_night;
                            Fog_density = Autumn_fog_night_density * Fog_Density_Mult;
                        } else {
                            Fog_color = Autumn_fog_dusk;
                            Fog_density = Autumn_fog_dusk_density * Fog_Density_Mult;
                        }
                        FogMode Fog_Mode = fogMode;//FogMode.ExponentialSquared;
                        float Density_Speed = Fog_Density_Speed;
                        //Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                        Make_Fog_Appear(Fog_color, Fog_density, Fog_Mode, Density_Speed, 0);

                    } else {
                        if (RenderSettings.fog) {
                            RenderSettings.fog = false;
                        }
                    }
                } else
                            if (Season == 4) {
                    if (!RenderSettings.fog) {
                        RenderSettings.fog = true;
                    }
                    Color Fog_color = Winter_fog_day;
                    float Fog_density = Winter_fog_day_density * Fog_Density_Mult;
                    if (Current_Time < 18 & Current_Time > 9) {
                        Fog_color = Winter_fog_day;
                        Fog_density = Winter_fog_day_density * Fog_Density_Mult;
                    } else if (Current_Time <= 9 & Current_Time > 1) {
                        Fog_color = Winter_fog_night;
                        Fog_density = Winter_fog_night_density * Fog_Density_Mult;
                    } else {
                        Fog_color = Winter_fog_dusk;
                        Fog_density = Winter_fog_dusk_density * Fog_Density_Mult;
                    }
                    FogMode Fog_Mode = fogMode;//FogMode.ExponentialSquared;
                    float Density_Speed = Fog_Density_Speed;
                    //Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                    Make_Fog_Appear(Fog_color, Fog_density, Fog_Mode, Density_Speed, 0);

                    //RenderSettings.fogColor = new Color(60/Color_devider,70/Color_devider,75/Color_devider,255/Color_devider);
                    //RenderSettings.fogDensity = 0.01f;

                } else {
                    if (RenderSettings.fog) {
                        RenderSettings.fog = false;
                    }
                }
            }


            init_presets = true;

            Horizontal_factor = 0; Horizontal_factor_prev = 0;//v2.1
            if (Shift_dawn != 0) {
                SunSystem.transform.eulerAngles = new Vector3(28.1412f - (Current_Time - 20.50f) * 15 + Shift_dawn * 15, Rot_Sun_Y, Rot_Sun_Z);
            } else {
                //v2.1
                SunSystem.transform.eulerAngles = new Vector3(Mathf.Abs((360 * 2) + 28.1412f - (Current_Time - 20.50f) * 15), Rot_Sun_Y, Rot_Sun_Z); //at 20.50f time, sun rot x at 28.1412f
            }

            //v3.0 - snow init
            if (currentWeatherName == Volume_Weather_types.HeavyStorm) {
                SnowCoverage = MaxSnowCoverage;
                SnowCoverageTerrain = MaxSnowCoverage;
                Shader.SetGlobalFloat(SnowCoverageVariable, MaxSnowCoverage / divideSnowTerrain);

                if (SnowMatTerrain != null) {
                    SnowMatTerrain.SetFloat(SnowCoverageVariable, MaxSnowCoverage / divideSnowTerrain);
                }

            } else {
                SnowCoverage = 0;
                SnowCoverageTerrain = 0;
                Shader.SetGlobalFloat(SnowCoverageVariable, 0);

                if (SnowMatTerrain != null) {
                    SnowMatTerrain.SetFloat(SnowCoverageVariable, 0);
                }
            }

            //v3.3b
            if (updateSkyAmbient) {
                //if (Time.fixedTime - lastAmbientUpdateTime > AmbientUpdateEvery) {
                DynamicGI.UpdateEnvironment();
                lastAmbientUpdateTime = Time.fixedTime;
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
                RenderSettings.ambientIntensity = AmbientIntensity;
                //}			
            }

            //v3.3c
            if (skyMat != null && Camera.main != null) {
                Assign_Material_Props(skyMat, Camera.main.transform.position);//transform);
                Assign_Material_PropsS(skyMat, Camera.main.transform.position);//transform);
            }
            init_scene = false;
            if (Camera.main != null) {
                prev_scene_cam_pos = Camera.main.transform.position;
            }
            if (skyboxMat != null && Camera.main != null) {
                Assign_Material_Props(skyboxMat, Camera.main.transform.position);
                Assign_Material_PropsS(skyboxMat, Camera.main.transform.position);
            }

        }//END START

        //v3.3c
        void Awake() {
            init_scene = false;
            if (Camera.main != null) {
                prev_scene_cam_pos = Camera.main.transform.position;
            }
        }
        void OnApplicationQuit() {
            //v3.3c
            init_scene = false;
            if (Camera.main != null) {
                prev_scene_cam_pos = Camera.main.transform.position;
            }
            //Debug.Log ("quited");
            if (skyMat != null && Camera.main != null) {
                Assign_Material_Props(skyMat, Camera.main.transform.position);//transform);
                Assign_Material_PropsS(skyMat, Camera.main.transform.position);//transform);
            }
            if (skyboxMat != null && Camera.main != null) {
                Assign_Material_Props(skyboxMat, Camera.main.transform.position);
                Assign_Material_PropsS(skyboxMat, Camera.main.transform.position);
            }
        }

        //v.2.0.1
        bool init_presets = false;

        ParticleSystem Star_particles_OBJ_P;

        ParticleSystem[] Butterflies2D_P;
        ParticleSystem[] Butterflies3D_P;

        ParticleSystem[] Dynamic_Clouds_Up_P;
        ParticleSystem[] Dynamic_Clouds_Dn_P;
        ParticleSystem[] Real_Clouds_Up_P;
        ParticleSystem[] Real_Clouds_Dn_P;
        ParticleSystem[] Cloud_bed_Up_P;
        ParticleSystem[] Cloud_bed_Dn_P;

        ParticleSystem[] Cloud_Static_Up_P;
        ParticleSystem[] Cloud_Static_Dn_P;
        ParticleSystem[] Tornados_P;//correspond to each tornado main particle system
        ParticleSystem[] Freezer_P;//control ice spreading spray from snow storm
        ParticleSystem[] SnowStorm_P;
        ParticleSystem[] FallingLeaves_P;
        ParticleSystem[] Volcanos_P;
        ParticleSystem[] VolumeFog_P;

        ParticleSystem[] Surround_Clouds_P;
        ParticleSystem[] Surround_Clouds_Heavy_P;

        ParticleSystem[] Heavy_Rain_P;
        ParticleSystem[] Mild_Rain_P;

        //v3.0
        ParticleSystem[] VolumeHeavy_Rain_P;
        ParticleSystem[] VolumeMild_Rain_P;
        ParticleSystem[] RefractHeavy_Rain_P;
        ParticleSystem[] RefractMild_Rain_P;

        ParticleSystem[] Sun_Ray_Cloud_P;
        ParticleSystem[] Lightning_System_P;

        //v1.7
        //Vector3 InitCamPos;

        //v1.6
        Vector3 InitCam;
        public float Cam_follow_factor = 0;
        public Vector3 Cam_offset = new Vector3(0, 0, 0);

        //v1.6.5
        //Vector3 cam_pos;

        //v3.3c - Update every frame
        void Assign_Material_PropsS(Material mat, Vector3 CameraTransform) {
            if (USE_SKYCUBE && SkyDomeSystem != null) {//v3.3b
                mat.SetVector("v3CameraPos", SkyCam_transform.localPosition);
            } else {
                //v1.6.5
                if (Unity5) {
                    InitCam = new Vector3(62.67624f, 10016, 107.306f);
                    Vector3 cam_pos = CameraTransform;
                    if (Application.isPlaying) {
                        //mat.SetVector ("v3CameraPos", new Vector3 (0, 14, 0) + (cam_pos - InitCam) + Cam_offset + Cam_follow_factor * new Vector3 (0, (SunTarget.transform.position.y - cam_pos.y) / 200, 0));
                        //mat.SetFloat ("flicker", 0);
                        mat.SetVector("v3CameraPos", new Vector3(0, 14, 0) - InitCam + Cam_offset + Cam_follow_factor * new Vector3(0, (SunTarget.transform.position.y - cam_pos.y) / 200));
                    } else {
                        //v3.4.2
                        //						float Adder = Camera.main.transform.position.y;
                        //						if (SunFollowHero) {
                        //							Adder = -Camera.main.transform.position.y;
                        //						}
                        //mat.SetFloat ("flicker", 0);
                        //mat.SetVector ("v3CameraPos", new Vector3 (0, 14+initCamY, 0) + (cam_pos - InitCam) + Cam_offset + (Cam_follow_factor+200) * new Vector3 (0, (SunTarget.transform.position.y - cam_pos.y) / 200, 0));
                        //mat.SetVector ("v3CameraPos", new Vector3 (0, 14, 0) + (cam_pos - InitCam) + Cam_offset +  new Vector3 (-cam_pos.x, -cam_pos.y, -cam_pos.z));
                        mat.SetVector("v3CameraPos", new Vector3(0, 14, 0) - InitCam + Cam_offset + Cam_follow_factor * new Vector3(0, (SunTarget.transform.position.y - cam_pos.y) / 200));
                    }
                } else {
                    mat.SetVector("v3CameraPos", transform.localPosition);
                }
            }
        }

        void Assign_Material_Props(Material mat, Vector3 CameraTransform)//v3.3c  //void Assign_Material_Props(Material mat, Transform CameraTransform)
        {
            //v4.9.3
            if (Camera.main != null && CameraTransform != Camera.main.transform.position)
            {
                return;
            }

            Vector3 invert_WaveLength = new Vector3(1 / Mathf.Pow(m_fWaveLength.x, 4), 1 / Mathf.Pow(m_fWaveLength.y, 4), 1 / Mathf.Pow(m_fWaveLength.z, 4));
            float scale = 1;
            scale = (1 / (m_fOuterRadius - m_fInnerRadius)) * Glob_scale;

            float fKr4PI = m_Kr * 4.0f * Mathf.PI;
            float fKm4PI = m_Km * 4.0f * Mathf.PI;

            Vector3 cam_pos1 = CameraTransform;
            if (Camera.current != null & Application.isEditor) {
                //cam_pos1 = Camera.current.transform.position;
            }

            float HOR_ADJ = Horizon_adj + 96 * (SunTarget.transform.position.y - cam_pos1.y) / 3000;


            mat.SetFloat("Horizon_adj", HOR_ADJ * 0.1f);
            mat.SetFloat("HorizonY", HorizonY / 1000);
            mat.SetColor("GroundColor", GroundColor);

            //v2.0.1
            if (Ortho_cam) {
                mat.SetColor("_Obliqueness", new Vector4(Ortho_factor, 0, 0, 0));
            } else {
                mat.SetColor("_Obliqueness", new Vector4(0, 0, 0, 0));
            }

            //		if(USE_SKYCUBE && SkyDomeSystem != null){//v3.3b
            //			mat.SetVector("v3CameraPos",  SkyCam_transform.localPosition);
            //		}else{
            //				//v1.6.5
            //				if(Unity5){
            //					InitCam = new Vector3(62.67624f,10016,107.306f);
            //					Vector3 cam_pos = CameraTransform;
            //					if(Camera.current != null & Application.isEditor){
            //						//cam_pos = Camera.current.transform.position;
            //					}
            //					//mat.SetVector("v3CameraPos",  new Vector3(0,14,0)+(cam_pos-InitCam)- Cam_follow_factor*(cam_pos - (InitCam+Cam_offset)) );
            //					//mat.SetVector("v3CameraPos",  new Vector3(0,14,0)+(cam_pos-InitCam)+ Cam_offset + Cam_follow_factor*new Vector3(0,(InitCamPos.y-cam_pos.y)/200,0) );
            //					mat.SetVector("v3CameraPos",  new Vector3(0,14,0)+(cam_pos-InitCam)+ Cam_offset + Cam_follow_factor*new Vector3(0,(SunTarget.transform.position.y-cam_pos.y)/200,0) );
            //				}else{
            //					mat.SetVector("v3CameraPos",  transform.localPosition);
            //				}
            //		}

            if (!alter_sun) {

                //v3.0
                bool Check_time = !AutoSunPosition && ((Current_Time > (NightTimeMax + Shift_dawn) && Current_Time <= 25) || (Current_Time >= 0 && Current_Time < (9 + Shift_dawn))); //v3.4.8 added nightimemax instead of 22.4
                bool Check_angle = AutoSunPosition && Rot_Sun_X <= NightAngleMax;  //v3.4.8 - added possibility to change sun to moon when sun is lower than horizon (nighttimemax instead of 0)
                                                                                   //v1.7
                                                                                   //if((Current_Time > (22.4 + Shift_dawn) & Current_Time <=25) | (Current_Time >= 0 & Current_Time < (9 + Shift_dawn) )){//v2.0.1 - 9 changed to 10
                if (Check_time | Check_angle) {
                    //mat.SetVector("v3LightDir", (Vector3.one*Sun_ring_factor)-MoonObj.transform.forward);
                    mat.SetVector("v3LightDir", (Vector3.one * Sun_ring_factor) - (CameraTransform - MoonObj.transform.position).normalized);

                    //mat.SetVector("v3LightDir", Vector3.Lerp(mat.GetVector("v3LightDir"),(Vector3.one*Sun_ring_factor)-(Camera.main.transform.position-MoonObj.transform.position).normalized,15.5f*Time.deltaTime));

                } else {
                    //mat.SetVector("v3LightDir", (Vector3.one*Sun_ring_factor)-SunObj.transform.forward);
                    //mat.SetVector("v3LightDir", (Vector3.one*Sun_ring_factor)-(CameraTransform-SunObj.transform.position).normalized);
                    if (!Application.isPlaying || (Application.isPlaying && Time.fixedTime > 0.01f)) { //v3.4.7
                        mat.SetVector("v3LightDir", (Vector3.one * Sun_ring_factor) - (CameraTransform - SunObj.transform.position).normalized);
                    }
                    //Debug.Log (Current_Time);
                }

                //v2.1
                if (SunObj2 != null) {
                    mat.SetVector("DualSunsFactors", DualSunsFactors);
                    mat.SetVector("v3LightDirMoon", (Vector3.one * Sun_ring_factor) - (CameraTransform - SunObj2.transform.position).normalized);
                }

            } else {
                //				if(!altered_sun){
                //					mat.SetVector("v3LightDir", (Vector3.one*Sun_ring_factor)-SunObj2.transform.forward);
                //					altered_sun = true;
                //				}else{
                //					mat.SetVector("v3LightDir", (Vector3.one*Sun_ring_factor)-SunObj.transform.forward);
                //					altered_sun = false;
                //				}
            }

            //mat.SetVector("v3LightDirMoon", (-Vector3.one*Sun_ring_factor)+SunObj.transform.forward); //v2.1

            mat.SetVector("v3InvWavelength", invert_WaveLength);

            mat.SetFloat("_Coloration", m_Coloration);
            mat.SetVector("_TintColor", m_TintColor);

            mat.SetFloat("fInnerRadius", m_fInnerRadius);
            mat.SetFloat("fInnerRadius2", m_fInnerRadius * m_fInnerRadius);
            mat.SetFloat("fOuterRadius", m_fOuterRadius);
            mat.SetFloat("fOuterRadius2", m_fOuterRadius * m_fOuterRadius);

            mat.SetFloat("fKrESun", m_Kr * m_ESun - (10 * Sun_halo_factor));
            mat.SetFloat("fKmESun", m_Km * m_ESun - (10 * Sun_eclipse_factor));
            mat.SetFloat("fKr4PI", fKr4PI);
            mat.SetFloat("fKm4PI", fKm4PI);
            mat.SetFloat("fScale", scale);
            if (!AltScale) {
                mat.SetFloat("fScaleDepth", scale_dif);
            } else {
                mat.SetFloat("fScaleDepth", m_fRayleighScaleDepth);
            }
            mat.SetFloat("fScaleOverScaleDepth", scale / m_fRayleighScaleDepth);
            mat.SetFloat("fExposure", m_fExposure);
            mat.SetFloat("g", m_g);
            mat.SetFloat("g2", m_g * m_g);
            mat.SetFloat("fSamples", m_fSamples);

            if (USE_SKYBOX) {
                mat.SetFloat("_SunSize", mSunSize);
                mat.SetVector("_SunTint", mSunTint);
                mat.SetFloat("_SkyExponent", mSkyExponent);
                mat.SetVector("_SkyTopColor", mSkyTopColor);
                mat.SetVector("_SkyMidColor", mSkyMidColor);
                mat.SetVector("_SkyEquatorColor", mSkyEquatorColor);
                mat.SetVector("_GroundColor", mGroundColor);
            }
        }

        public GameObject SunTarget; //  Sun looks there

        float Day_hours = 24f;

        [Range(0, 24)]
        public float Current_Time; //current time, update and define days, hours by it
        public float Current_Day; //current day
        public int Current_Month; //current day

        float days_since_last_month_inc = 0;
        public int days_per_month = 30;

        float Keep_previous_time;
        float Keep_previous_dawn_shift;
        public bool Auto_Cycle_Sky = false;

        //ORK
        //	private float KEEP_INITIAL_SUN_LIGHT_intensity;
        //	private float KEEP_INITIAL_SUPPORT_LIGHT_intensity;	
        //	private Color KEEP_START_FOG_COLOR;
        //	private float KEEP_START_FOG_DENSITY;
        //	private Vector3 KEEP_INITIAL_SUN_LIGHT_transform_eulerAngles;
        //	private Vector3 KEEP_INITIAL_SUPPORT_LIGHT_transform_eulerAngles;		
        //	private Color KEEP_INITIAL_SUN_LIGHT_color;
        //	private Color KEEP_INITIAL_SUPPORT_LIGHT_color;

        public GameObject SUN_LIGHT;
        public GameObject SUPPORT_LIGHT;
        public GameObject MOON_LIGHT;


        private Vector3 Keep_prev_sun_pos;

        public float SPEED = 1;

        public int Preset = 0;

        //v4.8.6
        public bool stopRainIfUnderWater = false;

        Light MAIN;
        //Light SUPPORT; //ORK
        #region FUNCTIONS
        void Make_Fog_Dissappear(float speed) {
            //// FOG
            #region FOG
            if (RenderSettings.fog) {
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0, speed * Time.deltaTime);
                //RenderSettings.fogMode = mode; 				
            }
            #endregion
            //// END FOG
        }

        void Make_Fog_Appear(Color Fog_Color, float density, FogMode mode, float speed, int Season) {
            //// FOG
            #region FOG
            bool Enable_fog = true;
            float Color_divider = 255;
            if (Enable_fog) {
                if (!RenderSettings.fog) {
                    RenderSettings.fog = true;
                    //RenderSettings.fogColor = Color.white;
                    RenderSettings.fogDensity = 0;
                }
                //RenderSettings.fogColor = new Color(100/Color_divider,80/Color_divider,100/Color_divider,255/Color_divider);
                //Color LERP_TO = new Color(100/Color_divider,80/Color_divider,100/Color_divider,255/Color_divider); // for AUTUMN
                if (Season != 0) { //give color based on season, else get from outside script variable
                    if (Season == 3) {
                        Fog_Color = new Color(100 / Color_divider, 80 / Color_divider, 100 / Color_divider, 255 / Color_divider);
                        density = 0.01f;
                        mode = FogMode.ExponentialSquared;
                    }
                } else {

                    //Fog_Color = Color.white;


                }
                Color LERP_TO = Fog_Color;

                //v3.3b
                if (init_presets) {
                    RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, LERP_TO, speed * Time.deltaTime);//0.5f
                    RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, density, speed * Time.deltaTime);//0.01f
                    RenderSettings.fogMode = mode; //FogMode.ExponentialSquared;
                } else {
                    RenderSettings.fogColor = LERP_TO;//0.5f
                    RenderSettings.fogDensity = density;//0.01f
                    RenderSettings.fogMode = mode; //FogMode.ExponentialSquared;
                }

            } else {
                if (RenderSettings.fog) {
                    RenderSettings.fog = false;
                }
            }
            #endregion
            //// END FOG
        }

        //Make particle clouds appear
        void Make_Appear(GameObject Particle_OBJ, ParticleSystem[] ParticleOBJ_Ps, ParticleSystem ParticleOBJ_P, int max_particles, bool reset_pcount, int rate) {

            if (Particle_OBJ != null) {
                if (!Particle_OBJ.activeInHierarchy) {
                    Particle_OBJ.SetActive(true);
                    if (ParticleOBJ_Ps != null) {
                        for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9
                            ParticleOBJ_Ps[n].Stop();
                            ParticleOBJ_Ps[n].Clear();
                            if (reset_pcount) {
                                MainMod.maxParticles = 0;
                            }
                        }
                    }
                    if (ParticleOBJ_P != null) {
                        ParticleOBJ_P.Stop();
                        ParticleOBJ_P.Clear();
                        if (reset_pcount) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                            MainMod.maxParticles = 0;
                        }
                    }
                } else { //start increasing particles
                    if (reset_pcount) {
                        if (ParticleOBJ_Ps != null) {
                            for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                                ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9
                                if (MainMod.maxParticles < max_particles) {
                                    MainMod.maxParticles += rate; // RESTORE max from spring elimination
                                }
                                if (!ParticleOBJ_Ps[n].isPlaying) {
                                    ParticleOBJ_Ps[n].Play();
                                }
                            }
                        }
                        if (ParticleOBJ_P != null) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                            if (MainMod.maxParticles < max_particles) {
                                MainMod.maxParticles += rate; // RESTORE max from spring elimination
                            }
                            if (!ParticleOBJ_P.isPlaying) {
                                ParticleOBJ_P.Play();
                            }
                        }
                    } else {
                        if (ParticleOBJ_Ps != null) {
                            for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                                ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9
                                MainMod.maxParticles = max_particles; // RESTORE max from spring elimination
                                ParticleOBJ_Ps[n].Play();
                            }
                        }
                        if (ParticleOBJ_P != null) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                            MainMod.maxParticles = max_particles; // RESTORE max from spring elimination
                            ParticleOBJ_P.Play();
                        }
                    }
                }
            }
        }
        //Make particle clouds dissappear
        void Make_Dissappear(GameObject Particle_OBJ, ParticleSystem[] ParticleOBJ_Ps, ParticleSystem ParticleOBJ_P, int min_particles, int rate, bool pull_downward, bool on_lowest) {

            if (Particle_OBJ != null) {
                if (Particle_OBJ.activeInHierarchy) {
                    if (ParticleOBJ_Ps != null) {
                        int count_finished = 0;
                        for (int n = 0; n < ParticleOBJ_Ps.Length; n++) {
                            ParticleSystem.MainModule MainMod = ParticleOBJ_Ps[n].main; //v3.4.9
                            MainMod.maxParticles = MainMod.maxParticles - rate;

                            if (pull_downward) {
                                MainMod.gravityModifier = -10;
                            }

                            if (MainMod.maxParticles < min_particles) { //start increasing opaque
                                ParticleOBJ_Ps[n].Stop();
                                ParticleOBJ_Ps[n].Clear();
                                if (on_lowest) {
                                    Particle_OBJ.SetActive(false);
                                }
                                count_finished++;
                            }
                        }
                        if (!on_lowest) {
                            if (count_finished >= ParticleOBJ_Ps.Length) {
                                Particle_OBJ.SetActive(false);
                            }
                        }
                    }
                    if (ParticleOBJ_P != null) {
                        ParticleSystem.MainModule MainMod = ParticleOBJ_P.main; //v3.4.9
                        MainMod.maxParticles = MainMod.maxParticles - rate;

                        if (pull_downward) {
                            MainMod.gravityModifier = -10;
                        }

                        if (MainMod.maxParticles < min_particles) { //start increasing opaque
                            ParticleOBJ_P.Stop();
                            ParticleOBJ_P.Clear();
                            Particle_OBJ.SetActive(false);
                        }
                    }
                }
            }
        }
        #endregion

        void Update()
        {
            //v3.4
            if (MAIN == null) {
                if (SUN_LIGHT != null) {
                    MAIN = SUN_LIGHT.GetComponent("Light") as Light;
                } else {
                    Debug.Log("Please add a light to the 'SUN_LIGHT' variable");
                }
            } else {
                //Debug.Log ("Please add a light to the 'SUN_LIGHT' variable");
            }

            //v3.3
            Previous_Rot_X = Rot_Sun_X;//v3.3

            //v3.0
            if (Cam_tranform == null && Camera.main != null) {
                Cam_tranform = Camera.main.transform;
            }

            //v3.0
            if (Tag_based_player) {
                if (Hero == null) {
                    //v3.3d
                    if (GameObject.FindGameObjectWithTag(Player_tag) != null) {
                        Hero = (GameObject.FindGameObjectWithTag(Player_tag)).transform;
                    }
                }
            } else {
                if (Hero == null) {
                    Hero = Camera.main.transform;
                }
            }

            ////////// HERO local weather 
            if (Hero != null & Application.isPlaying) {
                if (Butterflies_local) {
                    if (Butterfly_OBJ != null) {
                        //Butterfly_OBJ.transform.parent = Hero;
                        //Butterfly_OBJ.transform.position = new Vector3(Hero.position.x,Butterfly_OBJ.transform.position.y,Hero.position.z);
                        Butterfly_OBJ.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy) { //v4.8.6
                            if (Hero.position.y < water.position.y) {
                                //								if(Butterfly_OBJ.activeInHierarchy){
                                //									Butterfly_OBJ.SetActive(false);
                                //								}
                                for (int i = 0; i < Butterflies2D_P.Length; i++) {
                                    Butterflies2D_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!Butterfly_OBJ.activeInHierarchy){
                                //									Butterfly_OBJ.SetActive(true);
                                //								}
                                for (int i = 0; i < Butterflies2D_P.Length; i++) {
                                    Butterflies2D_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                }
                if (Fog_local) {
                    //Butterfly_OBJ.transform.parent = Hero;
                    if (VolumeFog_OBJ != null) {
                        //VolumeFog_OBJ.transform.parent = Hero;
                        //VolumeFog_OBJ.transform.position = new Vector3(Hero.position.x,VolumeFog_OBJ.transform.position.y,Hero.position.z);
                        VolumeFog_OBJ.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(VolumeFog_OBJ.activeInHierarchy){
                                //									VolumeFog_OBJ.SetActive(false);
                                //								}
                                for (int i = 0; i < VolumeFog_P.Length; i++) {
                                    VolumeFog_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!VolumeFog_OBJ.activeInHierarchy){
                                //									VolumeFog_OBJ.SetActive(true);
                                //								}
                                for (int i = 0; i < VolumeFog_P.Length; i++) {
                                    VolumeFog_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                }
                if (Heavy_rain_local) {
                    if (Rain_Heavy != null) {
                        //Rain_Heavy.transform.parent = Hero;
                        //Rain_Heavy.transform.position = new Vector3(Hero.position.x,Rain_Heavy.transform.position.y,Hero.position.z);
                        Rain_Heavy.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(Rain_Heavy.activeInHierarchy){
                                //									Rain_Heavy.SetActive(false);
                                //								}
                                for (int i = 0; i < Heavy_Rain_P.Length; i++) {
                                    Heavy_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!Rain_Heavy.activeInHierarchy){
                                //									if(currentWeatherName == Volume_Weather_types.Rain | Weather == Weather_types.Rain){
                                //										Rain_Heavy.SetActive(true);
                                //									}
                                //								}
                                for (int i = 0; i < Heavy_Rain_P.Length; i++) {
                                    Heavy_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                    if (VolumeRain_Heavy != null) {//v3.0
                                                   //VolumeRain_Heavy.transform.parent = Hero;
                                                   //VolumeRain_Heavy.transform.position = new Vector3(Hero.position.x,VolumeRain_Heavy.transform.position.y,Hero.position.z);
                        VolumeRain_Heavy.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(VolumeRain_Heavy.activeInHierarchy){
                                //									VolumeRain_Heavy.SetActive(false);
                                //								}
                                for (int i = 0; i < VolumeHeavy_Rain_P.Length; i++) {
                                    VolumeHeavy_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!VolumeRain_Heavy.activeInHierarchy){
                                //									if(currentWeatherName == Volume_Weather_types.Rain | Weather == Weather_types.Rain){
                                //										VolumeRain_Heavy.SetActive(true);
                                //									}
                                //								}
                                for (int i = 0; i < VolumeHeavy_Rain_P.Length; i++) {
                                    VolumeHeavy_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                    if (RefractRain_Heavy != null) {
                        //RefractRain_Heavy.transform.parent = Hero;
                        //RefractRain_Heavy.transform.position = new Vector3(Hero.position.x,RefractRain_Heavy.transform.position.y,Hero.position.z);
                        RefractRain_Heavy.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(RefractRain_Heavy.activeInHierarchy){
                                //									RefractRain_Heavy.SetActive(false);
                                //								}
                                for (int i = 0; i < RefractHeavy_Rain_P.Length; i++) {
                                    RefractHeavy_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!RefractRain_Heavy.activeInHierarchy){
                                //									if(currentWeatherName == Volume_Weather_types.HeavyStorm | Weather == Weather_types.Rain){
                                //										RefractRain_Heavy.SetActive(true);
                                //									}
                                //								}
                                for (int i = 0; i < RefractHeavy_Rain_P.Length; i++) {
                                    RefractHeavy_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                }
                if (Mild_rain_local) {
                    if (Rain_Mild != null) {
                        //Rain_Mild.transform.parent = Hero;
                        //Rain_Mild.transform.position = new Vector3(Hero.position.x,Rain_Mild.transform.position.y,Hero.position.z);
                        Rain_Mild.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(Rain_Mild.activeInHierarchy){
                                //									Rain_Mild.SetActive(false);
                                //								}
                                for (int i = 0; i < Mild_Rain_P.Length; i++) {
                                    Mild_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!Rain_Mild.activeInHierarchy){
                                //									Rain_Mild.SetActive(true);
                                //								}
                                for (int i = 0; i < Mild_Rain_P.Length; i++) {
                                    Mild_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                    if (VolumeRain_Mild != null) {//v3.0
                                                  //VolumeRain_Mild.transform.parent = Hero;
                                                  //VolumeRain_Mild.transform.position = new Vector3(Hero.position.x,VolumeRain_Mild.transform.position.y,Hero.position.z);
                        VolumeRain_Mild.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(VolumeRain_Mild.activeInHierarchy){
                                //									VolumeRain_Mild.SetActive(false);
                                //								}
                                for (int i = 0; i < VolumeMild_Rain_P.Length; i++) {
                                    VolumeMild_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!VolumeRain_Mild.activeInHierarchy){
                                //									VolumeRain_Mild.SetActive(true);
                                //								}
                                for (int i = 0; i < VolumeMild_Rain_P.Length; i++) {
                                    VolumeMild_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                    if (RefractRain_Mild != null) {
                        //RefractRain_Mild.transform.parent = Hero;
                        //RefractRain_Mild.transform.position = new Vector3(Hero.position.x,RefractRain_Mild.transform.position.y,Hero.position.z);
                        RefractRain_Mild.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(RefractRain_Mild.activeInHierarchy){
                                //									RefractRain_Mild.SetActive(false);
                                //								}
                                for (int i = 0; i < RefractMild_Rain_P.Length; i++) {
                                    RefractMild_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!RefractRain_Mild.activeInHierarchy){
                                //									RefractRain_Mild.SetActive(true);
                                //								}
                                for (int i = 0; i < RefractMild_Rain_P.Length; i++) {
                                    RefractMild_Rain_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                }
                if (Snow_local) {
                    if (SnowStorm_OBJ != null) {
                        //SnowStorm_OBJ.transform.parent = Hero;
                        //SnowStorm_OBJ.transform.position = new Vector3(Hero.position.x,SnowStorm_OBJ.transform.position.y,Hero.position.z);
                        SnowStorm_OBJ.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(SnowStorm_OBJ.activeInHierarchy){
                                //									SnowStorm_OBJ.SetActive(false);
                                //								}
                                for (int i = 0; i < SnowStorm_P.Length; i++) {
                                    SnowStorm_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!SnowStorm_OBJ.activeInHierarchy){
                                //									SnowStorm_OBJ.SetActive(true);
                                //								}
                                for (int i = 0; i < SnowStorm_P.Length; i++) {
                                    SnowStorm_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                    if (Ice_Spread_OBJ != null) {
                        //Ice_Spread_OBJ.transform.parent = Hero;
                        //Ice_Spread_OBJ.transform.position = new Vector3(Hero.position.x,Ice_Spread_OBJ.transform.position.y,Hero.position.z);
                        Ice_Spread_OBJ.transform.up = Vector3.up;
                        if (stopRainIfUnderWater && water != null && water.gameObject.activeInHierarchy)
                        {
                            if (Hero.position.y < water.position.y) {
                                //								if(Ice_Spread_OBJ.activeInHierarchy){
                                //									Ice_Spread_OBJ.SetActive(false);
                                //								}
                                for (int i = 0; i < Freezer_P.Length; i++) {
                                    Freezer_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = false;
                                }
                            } else {
                                //								if(!Ice_Spread_OBJ.activeInHierarchy){
                                //									Ice_Spread_OBJ.SetActive(true);
                                //								}
                                for (int i = 0; i < Freezer_P.Length; i++) {
                                    Freezer_P[i].transform.GetComponent<ParticleSystemRenderer>().enabled = true;
                                }
                            }
                        }
                    }
                }
            }




            //v1.7 - grab controllers as they come in play
            if (Terrain_controller == null) {
                if (Terrain.activeTerrain != null) {
                    Terrain_controller = Terrain.activeTerrain.gameObject.GetComponent(typeof(SeasonalTerrainSKYMASTER)) as SeasonalTerrainSKYMASTER;
                }
            }
            if (Mesh_Terrain_controller == null) {
                if (Mesh_terrain != null) {
                    Mesh_Terrain_controller = Mesh_terrain.gameObject.GetComponent(typeof(SeasonalTerrainSKYMASTER)) as SeasonalTerrainSKYMASTER;
                }
            }


            //ORK
            //TIME	
            bool Check_time = false;


            if (USE_ORK) {

                //UNCOMMENT FOR ORK FRAMEWORK !!!

                //			if (ORK.Game.Variables.GetFloat("TIME") != null){  	
                //				Check_time = true;
                //			}
            } else {

                Check_time = true;

            }

            //if (ORK.Game.Variables.GetFloat("TIME") != null)
            //if (GameHandler.GetNumberVariable("TIME") != null) //ORK1
            if (Check_time)
            {
                float CURRENT_TIME = 0; //ORK2			

                if (USE_ORK) {
                    //v3.1
                    CURRENT_TIME = Current_Time; //ORK2
                                                 //CURRENT_DAY = Current_Day; //ORK2
                    if (CURRENT_TIME < 24) {
                        //ORK.Game.Variables.Set("TIME",CURRENT_TIME+0.001f*SPEED); 
                        if (Auto_Cycle_Sky && Application.isPlaying && (!LimitSunUpdateRate || (LimitSunUpdateRate && Time.fixedTime - last_sun_update > Update_sun_every))) { // v3.4.5a
                            Current_Time = CURRENT_TIME + 0.02f * SPEED * Time.deltaTime;

                            last_sun_update = Time.fixedTime; //v3.4.5a
                        }
                    }
                    else {
                        //ORK.Game.Variables.Set("TIME",0);
                        //Current_Time = 0; //v3.3
                        //Keep_previous_time = 0.00001f;
                        //UPDATE DAY

                        if (CURRENT_TIME >= 24) {
                            //ORK.Game.Variables.Set("TIME_DAY",ORK.Game.Variables.GetFloat("TIME_DAY")+1); //ORK1
                            Current_Day = Current_Day + 1;
                            days_since_last_month_inc = days_since_last_month_inc + 1;
                            //Debug.Log("DAY is "+CURRENT_DAY);
                        }
                        Current_Time = 0; //v3.3

                    }

                    if (days_since_last_month_inc > days_per_month) {
                        Current_Month = Current_Month + 1;
                        days_since_last_month_inc = 0;
                    }

                    //UNCOMMENT FOR ORK FRAMEWORK !!!

                    //					//Set global variables to keep time between levels 		- UNCOMMENT FOR ORK FRAMEWORK !!!
                    //					if (ORK.Game.Variables.GetFloat("TIME") != null) {
                    //						//Debug.Log (ORK.Game.Variables.GetFloat ("TIME"));
                    //						ORK.Game.Variables.Set ("TIME", Current_Time); 
                    //						ORK.Game.Variables.Set ("TIME_DAY", Current_Day); 
                    //						ORK.Game.Variables.Set ("TIME_MONTH", Current_Month); 
                    //					}

                } else {


                    CURRENT_TIME = Current_Time; //ORK2
                                                 //CURRENT_DAY = Current_Day; //ORK2
                    if (CURRENT_TIME < 24) {
                        //ORK.Game.Variables.Set("TIME",CURRENT_TIME+0.001f*SPEED);
                        if (Auto_Cycle_Sky && Application.isPlaying && (!LimitSunUpdateRate || (LimitSunUpdateRate && Time.fixedTime - last_sun_update > Update_sun_every))) { // v3.4.5a
                                                                                                                                                                               //if(Auto_Cycle_Sky & Application.isPlaying){
                            Current_Time = CURRENT_TIME + 0.02f * SPEED * Time.deltaTime;

                            last_sun_update = Time.fixedTime; //v3.4.5a
                        }
                    }
                    else {
                        //ORK.Game.Variables.Set("TIME",0);
                        //Current_Time = 0; //v3.3
                        //Keep_previous_time = 0.00001f;
                        //UPDATE DAY

                        if (CURRENT_TIME >= 24) {
                            //ORK.Game.Variables.Set("TIME_DAY",ORK.Game.Variables.GetFloat("TIME_DAY")+1); //ORK1
                            Current_Day = Current_Day + 1;
                            days_since_last_month_inc = days_since_last_month_inc + 1;
                            //Debug.Log("DAY is "+CURRENT_DAY);
                        }
                        Current_Time = 0; //v3.3

                    }

                    if (days_since_last_month_inc > days_per_month) {
                        Current_Month = Current_Month + 1;
                        days_since_last_month_inc = 0;
                    }

                }


                ///TOD

                float DIST = (Current_Time - Keep_previous_time) * 0.04165041f; //0.04143041f //v1.7

                //v2.0.1 - reset dawn oriented sun rotation based on changes
                if (!Application.isPlaying) {
                    if (Current_Time != Keep_previous_time) {
                        if (Shift_dawn != 0) {
                            SunSystem.transform.eulerAngles = new Vector3(28.1412f - (Current_Time - 20.50f) * 15 + Shift_dawn * 15, Rot_Sun_Y, Rot_Sun_Z);
                        } else {
                            SunSystem.transform.eulerAngles = new Vector3(Mathf.Abs((360 * 2) + 28.1412f - (Current_Time - 20.50f) * 15), Rot_Sun_Y, Rot_Sun_Z); //at 20.50f time, sun rot x at 28.1412f
                        }
                    }
                    if (Keep_previous_dawn_shift != Shift_dawn) {
                        if (Shift_dawn != 0) {
                            SunSystem.transform.eulerAngles = new Vector3(28.1412f - (Current_Time - 20.50f) * 15 + Shift_dawn * 15, Rot_Sun_Y, Rot_Sun_Z);
                        } else {
                            SunSystem.transform.eulerAngles = new Vector3(28.1412f - (Current_Time - 20.50f) * 15, Rot_Sun_Y, Rot_Sun_Z); //at 20.50f time, sun rot x at 28.1412f
                        }
                    }
                }

                //TOD calcs - v3.0
                if (AutoSunPosition) {

                    //v3.3e - change time based on longitude
                    float Current_TimeF = Current_Time - Time_zone;//(24 - (Current_Time - Time_zone))%24;
                    if (Current_TimeF >= 24) {
                        Current_TimeF = Current_TimeF - 24;
                    }
                    if (Current_TimeF < 0) {
                        //Current_TimeF = 24-Current_TimeF;
                        Current_TimeF = 24 + Current_TimeF;
                    }
                    //Current_TimeF = (Current_Time + (int)((12*Latitude)/180))%24;
                    //Debug.Log (Current_TimeF);

                    float Day_numberN = (Current_Day + 0) % 365f;
                    //	Debug.Log("Day ="+Day_numberN);
                    float LonCoord = Longitude;//in degrees - negative west of greenwich, pos east
                    float LatCoord = Latitude * Mathf.Deg2Rad;
                    float Fraction_YearG = (360 / 365f) * (Day_numberN + (Current_TimeF) / 24) * Mathf.Deg2Rad;
                    //	Debug.Log("Fraction_YearG ="+(Fraction_YearG * Mathf.Rad2Deg).ToString());
                    float SunDeclination = (0.396372f + 4.02543f * Mathf.Sin(Fraction_YearG) - 22.91327f * Mathf.Cos(Fraction_YearG) - 0.387205f * Mathf.Cos(2 * Fraction_YearG) +
                                     0.051967f * Mathf.Sin(2 * Fraction_YearG) - 0.154627f * Mathf.Cos(3 * Fraction_YearG) + 0.084798f * Mathf.Sin(3 * Fraction_YearG)) * Mathf.Deg2Rad;
                    //	Debug.Log("SunDeclination = "+(SunDeclination* Mathf.Rad2Deg).ToString());
                    float TimeCorrecSolarAngTC = 0.004297f + 0.107029f * Mathf.Cos(Fraction_YearG) - 1.837877f * Mathf.Sin(Fraction_YearG) - 0.837378f * Mathf.Cos(2 * Fraction_YearG) - 2.340475f * Mathf.Sin(2 * Fraction_YearG);
                    //	Debug.Log("TimeCorrecSolarAngTC = "+TimeCorrecSolarAngTC);
                    float SolarHourAngleSHA = ((Current_TimeF) - 12) * 15 + LonCoord + TimeCorrecSolarAngTC;
                    if (SolarHourAngleSHA > 180) {
                        SolarHourAngleSHA = SolarHourAngleSHA - 360;
                    }
                    if (SolarHourAngleSHA < -180) {
                        SolarHourAngleSHA = SolarHourAngleSHA + 360;
                    }
                    //Debug.Log("SolarHourAngleSHA = "+SolarHourAngleSHA);
                    float CosSZA = Mathf.Sin(LatCoord) * Mathf.Sin(SunDeclination) + Mathf.Cos(LatCoord) * Mathf.Cos(SunDeclination) * Mathf.Cos(SolarHourAngleSHA * Mathf.Deg2Rad);
                    if (CosSZA > 1) { CosSZA = 1; }
                    if (CosSZA < -1) { CosSZA = -1; }
                    //	Debug.Log("CosSZA = "+CosSZA);
                    float SunZenithAngleSZA = Mathf.Acos(CosSZA);//in radians
                                                                 //	Debug.Log("SunZenithAngleSZA = "+SunZenithAngleSZA*Mathf.Rad2Deg);
                    float SunElevationAngle = 90 - (SunZenithAngleSZA * Mathf.Rad2Deg);

                    //v3.3e
                    MaxSunElevation = 90 - Latitude + SunDeclination * Mathf.Rad2Deg;
                    //Debug.Log ("SunElevation === "+SunElevation);

                    float Cos_AZ = (Mathf.Sin(SunDeclination) - Mathf.Sin(LatCoord) * Mathf.Cos(SunZenithAngleSZA)) / (Mathf.Cos(LatCoord) * Mathf.Sin(SunZenithAngleSZA));
                    //	Debug.Log("Cos_AZ = "+Cos_AZ);
                    float AzimuthAngle = Mathf.Acos(Cos_AZ) * Mathf.Rad2Deg;//degrees
                                                                            //	Debug.Log("AzimuthAngle = "+AzimuthAngle);
                    float CorrectAzimuth = AzimuthAngle;
                    if (SolarHourAngleSHA > 0) {//if(Current_Time > 12.22f){
                        CorrectAzimuth = 360 - AzimuthAngle;
                    }
                    Rot_Sun_Y = CorrectAzimuth - RotateNorth; //v4.2b     //Rot_Sun_Y = CorrectAzimuth;
                    //Rot_Sun_Z = SunElevationAngle;
                    Rot_Sun_X = SunElevationAngle;
                    //SunSystem.transform.Rotate(Vector3.right * (SunElevationAngle) * 1 );
                    //SunSystem.transform.rotation = Quaternion.AngleAxis(SunElevationAngle,Vector3.right);
                    if (Mathf.Abs(Rot_Sun_Y) < 360) {
                        SunSystem.transform.eulerAngles = new Vector3(SunElevationAngle, Rot_Sun_Y, Rot_Sun_Z);
                    }
                    //Debug.Log("Azimuth = "+AzimuthAngle);
                    //Debug.Log("360-Azimuth = "+(360-AzimuthAngle).ToString());
                    //Debug.Log("Sun Elevation = "+SunElevationAngle);

                    //v3.3
                    if (LatLonMoonPos) {
                        if (MoonCenterObj.transform.parent == this.transform) {

                            float MoonDay = Day_numberN;

                            //Parameters in degrees and converted to rad with *Mathf.Deg2Rad
                            float O_N = (125.1228f - 0.0529538083f * MoonDay) * Mathf.Deg2Rad;
                            float O_i = 5.1454f;//*Mathf.Deg2Rad;
                            float O_w = (318.0634f + 0.1643573223f * MoonDay) * Mathf.Deg2Rad;
                            float O_a = 60.2666f;
                            float O_e = 0.0549f;
                            float O_M = (115.3654f + 13.0649929509f * MoonDay) * Mathf.Deg2Rad;
                            //float Epsilon = O_M + O_e * (180/Mathf.PI) * Mathf.Sin(O_M) * (1.0f + O_e * Mathf.Cos(O_M));
                            float ECL = (23.4393f - (3.563E-7f * MoonDay)) * Mathf.Deg2Rad;
                            float Epsilon = O_M + O_e * Mathf.Sin(O_M) * (1.0f + O_e * Mathf.Cos(O_M));//if Epsilon and M in radians

                            float Moon_dist1 = O_a * (Mathf.Cos(Epsilon) - O_e);
                            float Moon_dist2 = O_a * (Mathf.Sqrt(1.0f - O_e * O_e) * Mathf.Sin(Epsilon));
                            float MoonV = Mathf.Atan2(Moon_dist2, Moon_dist1) * Mathf.Rad2Deg;

                            //							if (MoonV < 0) {
                            //								MoonV = 360 + MoonV;
                            //							}


                            float MoonR = Mathf.Sqrt(Moon_dist1 * Moon_dist1 + Moon_dist2 * Moon_dist2);
                            //Debug.Log ("MoonR="+MoonR);
                            //Debug.Log ("MoonV="+MoonV);

                            //return to rad
                            MoonV = Mathf.Deg2Rad * MoonV;

                            float Moon_xh = MoonR * (Mathf.Cos(O_N) * Mathf.Cos(MoonV + O_w) - Mathf.Sin(O_N) * Mathf.Sin(MoonV + O_w) * Mathf.Cos(O_i));
                            float Moon_yh = MoonR * (Mathf.Sin(O_N) * Mathf.Cos(MoonV + O_w) + Mathf.Cos(O_N) * Mathf.Sin(MoonV + O_w) * Mathf.Cos(O_i));
                            float Moon_zh = MoonR * (Mathf.Sin(MoonV + O_w) * Mathf.Sin(O_i));
                            //Debug.Log ("MoonPos1="+new Vector3(Moon_xh,Moon_yh,Moon_zh));
                            //float Moon_lonEcl = Mathf.Atan2 (Moon_yh, Moon_xh);
                            //float Moon_latEcl = Mathf.Atan2 (Moon_zh, Mathf.Sqrt(Moon_xh*Moon_yh + Moon_xh*Moon_yh));

                            float Moon_xe = Moon_xh;
                            float Moon_ye = Moon_yh * Mathf.Cos(ECL) - Moon_zh * Mathf.Sin(ECL);
                            float Moon_ze = Moon_yh * Mathf.Sin(ECL) + Moon_zh * Mathf.Cos(ECL);

                            float Moon_RA = Mathf.Atan2(Moon_ye, Moon_xe);
                            //Debug.Log ("Moon_RA="+Moon_RA);

                            float Moon_Declination = Mathf.Atan2(Moon_ze, Mathf.Sqrt(Moon_xe * Moon_xe + Moon_ye * Moon_ye));
                            //Debug.Log ("Moon_Declination="+Moon_Declination);

                            //							float SolarHourAngleSHA1 = ((Current_Time) - 12)*15f+ LonCoord + TimeCorrecSolarAngTC;
                            //							if(SolarHourAngleSHA1 > 180){
                            //								SolarHourAngleSHA1 = SolarHourAngleSHA1 - 360;
                            //							}
                            //							if(SolarHourAngleSHA1 < -180){
                            //								SolarHourAngleSHA1 = SolarHourAngleSHA1 + 360;
                            //							}

                            float fixed_SHA = SolarHourAngleSHA;
                            if (SolarHourAngleSHA < 0) {//if(Current_Time > 12.22f){
                                                        //fixed_SHA = 180+SolarHourAngleSHA;
                                                        //fixed_SHA = -SolarHourAngleSHA;

                            }

                            if (Moon_RA > 0) {
                                fixed_SHA = SolarHourAngleSHA + 180;///// 3
                            }

                            //fixed_SHA = fixed_SHA*1.2f;
                            float Moon_x = Mathf.Cos(fixed_SHA * Mathf.Deg2Rad) * Mathf.Cos(Moon_Declination);
                            float Moon_y = Mathf.Sin(fixed_SHA * Mathf.Deg2Rad) * Mathf.Cos(Moon_Declination);
                            float Moon_z = Mathf.Sin(Moon_Declination);
                            //	Debug.Log ("fixed_SHA="+fixed_SHA);

                            //							float Sun_W = (282.9404f + 4.70935E-5f * MoonDay)*Mathf.Deg2Rad;
                            //							float Sun_M = (356.0470f + 0.9856002585f * MoonDay)*Mathf.Deg2Rad;

                            float Moon_xhor = Moon_x * Mathf.Sin(LatCoord + MoonPlaneRot * Mathf.Deg2Rad) - Moon_z * Mathf.Cos(LatCoord + MoonPlaneRot * Mathf.Deg2Rad);
                            float Moon_yhor = Moon_y;
                            float Moon_zhor = Moon_x * Mathf.Cos(LatCoord + MoonPlaneRot * Mathf.Deg2Rad) + Moon_z * Mathf.Sin(LatCoord + MoonPlaneRot * Mathf.Deg2Rad);

                            //							float Moon_xhor = Moon_x * Mathf.Sin (LatCoord) - Moon_z * Mathf.Cos (LatCoord);
                            //							float Moon_yhor = Moon_y;
                            //							float Moon_zhor = Moon_x * Mathf.Cos (LatCoord) + Moon_z * Mathf.Sin (LatCoord);

                            float Moon_Azimuth = Mathf.Atan2(Moon_yhor, Moon_xhor) * Mathf.Rad2Deg - RotateNorth; //v4.2b
                            float Moon_Altitude = Mathf.Atan2(Moon_zhor, Mathf.Sqrt(Moon_xhor * Moon_xhor + Moon_yhor * Moon_yhor)) * Mathf.Rad2Deg;
                            //	Debug.Log ("Moon_zhor="+Moon_zhor);
                            //Debug.Log ("Moon_xhor="+Moon_xhor);
                            //Debug.Log ("Moon_yhor="+Moon_yhor);

                            if (SolarHourAngleSHA < 0) {
                                //Moon_Azimuth = 360 - Moon_Azimuth;
                            }

                            if (Mathf.Abs(Moon_Azimuth) < 360) {

                                //MoonCenterObj.transform.eulerAngles = new Vector3 (Moon_Altitude, Moon_Azimuth, Rot_Sun_Z);
                                //Debug.Log(Current_Time);

                                //								if (Current_Time < 23.5f) {
                                //									MoonCenterObj.transform.rotation = Quaternion.Euler (-Moon_Altitude, Moon_Azimuth, Rot_Sun_Z);
                                //								} else {
                                //									//MoonCenterObj.transform.rotation = Quaternion.Euler (Moon_Altitude-5.5f*Current_Time, Moon_Azimuth-5.5f*Current_Time, Rot_Sun_Z);
                                //									//MoonCenterObj.transform.rotation = Quaternion.Euler (Moon_Altitude-0.2f*Moon_Altitude*Current_Time, Moon_Azimuth-0.2f*Moon_Azimuth*Current_Time, Rot_Sun_Z);
                                //									if (Current_Day < 200) {
                                //										MoonCenterObj.transform.rotation = Quaternion.Euler (-Moon_Altitude - 15f * (Current_Time - 23.5f) * (Current_Time - 23.5f), Moon_Azimuth, Rot_Sun_Z);
                                //									} else {
                                //										MoonCenterObj.transform.rotation = Quaternion.Euler (-Moon_Altitude + 15f * (Current_Time - 23.5f) * (Current_Time - 23.5f), Moon_Azimuth, Rot_Sun_Z);
                                //									}
                                //								}

                                if (Current_TimeF <= 24f) {


                                    //if (Lerp_sky_rot && (Current_Time < 0.5f | Current_Time>23.7f)) {
                                    if (Lerp_sky_rot && (Current_TimeF < 3.64f)) {
                                        MoonCenterObj.transform.rotation = Quaternion.Lerp(MoonCenterObj.transform.rotation, Quaternion.Euler(-Moon_Altitude, Moon_Azimuth, Rot_Sun_Z), Time.deltaTime * (0.21f + (Current_TimeF * 1.6f)));
                                    }
                                    else {
                                        MoonCenterObj.transform.rotation = Quaternion.Euler(-Moon_Altitude, Moon_Azimuth, Rot_Sun_Z);
                                    }

                                } else {
                                    //MoonCenterObj.transform.rotation = Quaternion.Euler (Moon_Altitude-5.5f*Current_Time, Moon_Azimuth-5.5f*Current_Time, Rot_Sun_Z);
                                    //MoonCenterObj.transform.rotation = Quaternion.Euler (Moon_Altitude-0.2f*Moon_Altitude*Current_Time, Moon_Azimuth-0.2f*Moon_Azimuth*Current_Time, Rot_Sun_Z);
                                    //									if (Current_Day < 200) {
                                    //										MoonCenterObj.transform.rotation = Quaternion.Euler (-Moon_Altitude - 5f * (Current_Time - 23.9f) * (Current_Time - 23.9f), Moon_Azimuth, Rot_Sun_Z);
                                    //									} else {
                                    //										MoonCenterObj.transform.rotation = Quaternion.Euler (-Moon_Altitude + 5f * (Current_Time - 23.9f) * (Current_Time - 23.9f), Moon_Azimuth, Rot_Sun_Z);
                                    //									}
                                    //									if (Current_Day < 200) {
                                    //										MoonCenterObj.transform.rotation =Quaternion.Lerp(MoonCenterObj.transform.rotation, Quaternion.Euler (-Moon_Altitude - 1f * (Current_Time - 23.9f) * (Current_Time - 23.9f), Moon_Azimuth, Rot_Sun_Z),Time.deltaTime*2.9f);
                                    //									} else {
                                    //										MoonCenterObj.transform.rotation = Quaternion.Lerp(MoonCenterObj.transform.rotation, Quaternion.Euler (-Moon_Altitude + 1f * (Current_Time - 23.9f) * (Current_Time - 23.9f), Moon_Azimuth, Rot_Sun_Z),Time.deltaTime*2.9f);
                                    //									}
                                    MoonCenterObj.transform.rotation = Quaternion.Lerp(MoonCenterObj.transform.rotation, Quaternion.Euler(-Moon_Altitude, Moon_Azimuth, Rot_Sun_Z), Time.deltaTime * 3.9f);
                                }
                                //MoonCenterObj.transform.rotation = Quaternion.Euler (-Moon_Altitude, Moon_Azimuth, Rot_Sun_Z);

                                //MoonCenterObj.transform.eulerAngles = new Vector3 (Moon_Altitude, Moon_Azimuth, Rot_Sun_Z);
                                //MoonCenterObj.transform.eulerAngles = Vector3.Lerp(MoonCenterObj.transform.eulerAngles,new Vector3 (Moon_Altitude, Moon_Azimuth, Rot_Sun_Z),Time.deltaTime*10);
                            }
                            //Debug.Log ("Moon_Azimuth="+Moon_Azimuth);
                            //Debug.Log ("Moon_Altitude="+Moon_Altitude);

                            if (StarsFollowMoon) {
                                //if active, disable star particle motion and add moon motion, also handle more star layers here (check if dome exists and rotate, also check time and apply extra shader fades)
                                if (Star_particles_OBJ.transform.parent != MoonCenterObj) {
                                    Star_particles_OBJ.transform.parent = MoonCenterObj;
                                    Star_particles_OBJ.GetComponentsInChildren<Circle_Around_ParticleSKYMASTER>(true)[0].enabled = false;
                                }
                            } else {
                                if (Star_particles_OBJ.transform.parent == MoonCenterObj) {
                                    Star_particles_OBJ.transform.parent = this.transform;
                                    Star_particles_OBJ.GetComponentsInChildren<Circle_Around_ParticleSKYMASTER>(true)[0].enabled = true;
                                }
                            }
                        }
                    }

                }
                //

                if (!AutoSunPosition) {
                    if (Previous_Rot_Y != Rot_Sun_Y | Previous_Rot_Z != Rot_Sun_Z) {
                        if (Shift_dawn != 0) {
                            SunSystem.transform.eulerAngles = new Vector3(28.1412f - (Current_Time - 20.50f) * 15 + Shift_dawn * 15, Rot_Sun_Y, Rot_Sun_Z);
                        } else {
                            SunSystem.transform.eulerAngles = new Vector3(Mathf.Abs((360 * 2) + 28.1412f - (Current_Time - 20.50f) * 15), Rot_Sun_Y, Rot_Sun_Z); //at 20.50f time, sun rot x at 28.1412f
                        }
                    }
                }

                if (DIST != 0 && !AutoSunPosition) {
                    SunSystem.transform.Rotate(Vector3.right * (DIST) * (360 * 1));
                }

                if (Application.isPlaying) {//v.2.0.1 - Use these for real time, rot_y_z for editor
                    if (Seasonal_prev != Seasonal) {
                        Seasonal_prev = Seasonal;

                        if (Day_hours < 12) {

                            SunSystem.transform.Rotate(new Vector3(0, Seasonal, 0));
                        } else {

                            SunSystem.transform.Rotate(new Vector3(0, -Seasonal, 0));
                        }
                    }

                    //do ONLY if changes
                    if (Horizontal_factor_prev != Horizontal_factor) {

                        Horizontal_factor_prev = Horizontal_factor;

                        float Hor_factor = Horizontal_factor;

                        SunSystem.transform.eulerAngles = new Vector3(SunSystem.transform.eulerAngles.x, SunSystem.transform.eulerAngles.y + Hor_factor, SunSystem.transform.eulerAngles.z);

                    }
                }

                Keep_previous_time = Current_Time;
                Keep_previous_dawn_shift = Shift_dawn;//v2.0.1
                Previous_Rot_Y = Rot_Sun_Y;
                Previous_Rot_Z = Rot_Sun_Z;

                if (Keep_prev_sun_pos != SUN_LIGHT.transform.position) {
                    SUN_LIGHT.transform.LookAt(SunTarget.transform);
                    Keep_prev_sun_pos = SUN_LIGHT.transform.position;
                }

                //////////////////////// HANDLE WEATHER CHANGE ///////////////////////////////
                if ((Weather != previous_weather | 1 == 1) & On_demand) {

                    if (Weather != previous_weather) {
                        previous_weather = Weather;
                        if (!RenderSettings.fog) {
                            //check if disabled fog had left over density
                            RenderSettings.fogDensity = 0;
                        }
                    }


                    //FOGGY
                    if (Weather == Weather_types.Foggy | Weather == Weather_types.HeavyFog) {

                        FogMode Fog_Mode = FogMode.Exponential;
                        float Fog_density = 0.007f + Random.Range(-0.001f, 0.001f);
                        float Dark_divide = 1;
                        //Make_Fog_Dissappear(1.1f);
                        Color Fog_color = new Color(0.9f, 0.9f, 0.9f, 1);
                        //if(Current_Time < 16 & Current_Time > 11 ){
                        if (Current_Time < 18 & Current_Time > 9) {

                            if (Current_Time > 16) {
                                Fog_color = new Color(0.8f, 0.45f, 0.55f, 1);
                            } else if (Current_Time > 12) {
                                Fog_color = new Color(0.9f, 0.9f, 0.9f, 1);
                            } else {
                                Fog_color = new Color(0.5f, 0.15f, 0.45f, 1);
                            }

                            if (Season == 1 | Season == 0) {
                                Fog_color = Spring_fog_day;
                                Fog_density = Spring_fog_day_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 2) {
                                Fog_color = Summer_fog_day;
                                Fog_density = Summer_fog_day_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 3) {
                                Fog_color = Autumn_fog_day;
                                Fog_density = Autumn_fog_day_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 4) {
                                Fog_color = Winter_fog_day;
                                Fog_density = Winter_fog_day_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            }

                            //}else if(Current_Time <=11 & Current_Time >=0){
                        } else if (Current_Time <= 9 & Current_Time >= 0) {
                            Fog_color = new Color(0.2f, 0.2f, 0.2f, 1);
                            Dark_divide = 1;

                            if (Season == 1 | Season == 0) {
                                Fog_color = Spring_fog_night;
                                Fog_density = Spring_fog_night_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 2) {
                                Fog_color = Summer_fog_night;
                                Fog_density = Summer_fog_night_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 3) {
                                Fog_color = Autumn_fog_night;
                                Fog_density = Autumn_fog_night_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 4) {
                                Fog_color = Winter_fog_night;
                                Fog_density = Winter_fog_night_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            }

                        } else {//18 to 1

                            if (Current_Time > 22.5f) {
                                Fog_color = new Color(0.2f, 0.2f, 0.2f, 1);
                                Dark_divide = 1;
                            } else {
                                Fog_color = new Color(0.9f, 0.6f, 0.3f, 1);
                            }

                            if (Season == 1 | Season == 0) {
                                //Debug.Log("color = "+Fog_color);
                                Fog_color = Spring_fog_dusk;
                                Fog_density = Spring_fog_dusk_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 2) {
                                Fog_color = Summer_fog_dusk;
                                Fog_density = Summer_fog_dusk_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 3) {
                                Fog_color = Autumn_fog_dusk;
                                Fog_density = Autumn_fog_dusk_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            } else
                            if (Season == 4) {
                                Fog_color = Winter_fog_dusk;
                                Fog_density = Winter_fog_dusk_density * Fog_Density_Mult;
                                Fog_Mode = FogMode.ExponentialSquared;
                            }
                        }

                        float Fog_density2 = Fog_density;   // correction for heavy fog, no cloud dome, so has to be more mild 
                        float Density_Speed = Fog_Density_Speed;
                        if (Weather == Weather_types.HeavyFog) {
                            //Fog_density2 = Fog_density/2;
                            //Density_Speed = 0.015f;
                            Fog_Mode = FogMode.Exponential;
                        }

                        //Make_Fog_Appear(Fog_color/Dark_divide,Fog_density2,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                        Make_Fog_Appear(Fog_color / Dark_divide, Fog_density2, Fog_Mode, Density_Speed, 0);


                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        if (Weather == Weather_types.HeavyFog) {
                            Make_Appear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 200, true, 5);
                            Make_Appear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 200, true, 5);
                            Make_Appear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 200, true, 5);
                            Make_Appear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 200, true, 5);
                        } else {
                            Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                            Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);
                            Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                            Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);
                        }

                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);

                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }

                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                        // Wind_OBJ.
                        //var Wind_Zone = Wind_OBJ.GetComponent("WindZone");
                        //Wind_Zone.
                    }

                    //HEAVY STORM
                    if (Weather == Weather_types.HeavyStorm | Weather == Weather_types.HeavyStormDark) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        if (Weather == Weather_types.HeavyStormDark) {
                            Make_Appear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, Flat_cloud_max_part, true, 10);
                            Make_Appear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, Flat_cloud_max_part, true, 10);
                        } else {
                            Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 50, false, false);
                            Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 50, false, false);
                        }

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        //Make_Dissappear(Surround_Clouds,Surround_Clouds_P,null,5,50, false);
                        //Make_Dissappear(Surround_Clouds_Heavy,Surround_Clouds_Heavy_P,null,5,50, false);
                        Make_Appear(Surround_Clouds, Surround_Clouds_P, null, Storm_cloud_max_part, false, 10);
                        Make_Appear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, Storm_cloud_max_part, false, 10);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        //Make_Dissappear(Rain_Heavy,Heavy_Rain_P,null,5,50, false);
                        Make_Appear(Rain_Heavy, Heavy_Rain_P, null, 300, false, 10);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //SNOW STORM
                    if (Weather == Weather_types.SnowStorm | Weather == Weather_types.FreezeStorm) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        //Make_Dissappear(SnowStorm_OBJ,SnowStorm_P,null,5,50, false);
                        Make_Appear(SnowStorm_OBJ, SnowStorm_P, null, Snow_cloud_max_part, true, 10);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        if (Weather == Weather_types.SnowStorm) {
                            Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                            Make_Dissappear(Ice_System_OBJ, null, null, 5, 50, false, false);
                        } else {
                            Make_Appear(Ice_Spread_OBJ, Freezer_P, null, 250, true, 10);
                            Make_Appear(Ice_System_OBJ, null, null, 250, true, 10);
                        }

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //LIGHTNING STORM
                    if (Weather == Weather_types.LightningStorm) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 8, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 8, false, false);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        //	Make_Dissappear(Lightning_System_OBJ,null,null,5,50, false);
                        Make_Appear(Lightning_System_OBJ, Lightning_System_P, null, Lightning_cloud_max_part, false, 15);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //FLAT CLOUDS
                    if (Weather == Weather_types.FlatClouds) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        //						Make_Dissappear(Lower_Cloud_Bed,Cloud_bed_Dn_P,null,5,8, false);
                        //						Make_Dissappear(Upper_Cloud_Bed,Cloud_bed_Up_P,null,5,8, false);

                        Make_Appear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, Flat_cloud_max_part, true, 5);
                        Make_Appear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, Flat_cloud_max_part, true, 5);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);

                        //Make_Dissappear(Rain_Heavy,Heavy_Rain_P,null,5,50, false,false);
                        Make_Appear(Rain_Heavy, Heavy_Rain_P, null, 1000, true, 25);

                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //CLOUDY
                    if (Weather == Weather_types.Cloudy) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        //						Make_Dissappear(Lower_Dynamic_Cloud,Dynamic_Clouds_Dn_P,null,5,50, false);
                        //						Make_Dissappear(Upper_Dynamic_Cloud,Dynamic_Clouds_Up_P,null,5,50, false);
                        //						
                        //						Make_Dissappear(Lower_Static_Cloud,Cloud_Static_Dn_P,null,5,50, false);
                        //						Make_Dissappear(Upper_Static_Cloud,Cloud_Static_Up_P,null,5,50, false);

                        if (Season != 3) {
                            Make_Appear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 300, true, 15);
                            Make_Appear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 300, true, 15);
                            Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                            Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);
                        } else {
                            Make_Appear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, Real_cloud_max_part, true, 15);
                            Make_Appear(Upper_Cloud_Real, Real_Clouds_Up_P, null, Real_cloud_max_part, true, 15);
                            Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                            Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);
                        }

                        Make_Appear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 300, true, 15);
                        Make_Appear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 300, true, 15);




                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 8, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 8, false, false);



                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //VOLCANOS
                    if (Weather == Weather_types.VolcanoErupt) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //						int count_volc_p =0;
                        //						for (int n = 0; n < Volcano_OBJ.Length; n++){
                        //							//Make_Dissappear(Tornado_OBJs[n],null,Tornados_P[n],5,50, false);
                        //							Make_Appear(Volcano_OBJ[n],null,Volcanos_P[count_volc_p],1300,true,5);
                        //							Make_Appear(Volcano_OBJ[n],null,Volcanos_P[count_volc_p+1],1300,true,5);
                        //							Make_Appear(Volcano_OBJ[n],null,Volcanos_P[count_volc_p+2],1300,true,5);
                        //							Make_Appear(Volcano_OBJ[n],null,Volcanos_P[count_volc_p+3],1300,true,5);
                        //							Make_Appear(Volcano_OBJ[n],null,Volcanos_P[count_volc_p+4],1300,true,5);
                        //							count_volc_p = count_volc_p+5;
                        //						}

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Appear(Volcano_OBJ[n], Vocan, null, 1300, true, 5);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 8, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 8, false, false);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //VOLUMETRIC FOG
                    if (Weather == Weather_types.RollingFog) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //						int count_volc_p =0;
                        //						for (int n = 0; n < Volcanos_P.Length; n++){
                        //							if(Volcanos_P[n].maxParticles>0){
                        //								Volcanos_P[n].maxParticles -= 5;
                        //							}else{
                        //								Volcanos_P[n].maxParticles = 0;
                        //							}
                        //						}
                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);

                        //Make_Dissappear(VolumeFog_OBJ,VolumeFog_P,null,5,50, false);
                        Make_Appear(VolumeFog_OBJ, VolumeFog_P, null, Volume_fog_max_part, true, 5);

                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 8, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 8, false, false);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //TORNADOS
                    if (Weather == Weather_types.Tornado) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }
                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 8, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 8, false, false);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        //for (int n = 0; n < Tornado_OBJs.Length; n++){
                        for (int n = 0; n < 1; n++) {
                            //Make_Dissappear(Tornado_OBJs[n],null,Tornados_P[n],5,50, false,false);
                            Make_Appear(Tornado_OBJs[n], null, Tornados_P[n], 250, true, 10);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //RAIN
                    if (Weather == Weather_types.Rain) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        //						Make_Dissappear(Lower_Cloud_Bed,Cloud_bed_Dn_P,null,5,8, false);
                        //						Make_Dissappear(Upper_Cloud_Bed,Cloud_bed_Up_P,null,5,8, false);

                        Make_Appear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 700, true, 5);
                        Make_Appear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 700, true, 5);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);

                        //Make_Dissappear(Rain_Heavy,Heavy_Rain_P,null,5,50, false,false);
                        //Make_Appear(Rain_Heavy,Heavy_Rain_P,null,1000,true,25);
                        Make_Appear(Rain_Mild, Mild_Rain_P, null, 1000, true, 25);

                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }

                    //SUNNY
                    if (Weather == Weather_types.Sunny) {

                        if (!Use_fog) {
                            Make_Fog_Dissappear(1.1f);
                        }

                        //						int count_volc_p =0;
                        //						for (int n = 0; n < Volcanos_P.Length; n++){
                        //							if(Volcanos_P[n].maxParticles>0){
                        //								Volcanos_P[n].maxParticles -= 5;
                        //							}else{
                        //								Volcanos_P[n].maxParticles = 0;
                        //							}
                        //						}
                        //VOLCANO
                        int count_volc_p = 0;
                        for (int n = 0; n < Volcano_OBJ.Length; n++) {
                            int Array_size = (int)(Volcanos_P.Length / Volcano_OBJ.Length);
                            ParticleSystem[] Vocan = new ParticleSystem[Array_size];
                            for (int n0 = 0; n0 < Array_size; n0++) {
                                Vocan[n0] = Volcanos_P[count_volc_p];
                                count_volc_p += 1;
                            }
                            Make_Dissappear(Volcano_OBJ[n], Vocan, null, 5, 10, false, false);
                        }

                        Make_Dissappear(SnowStorm_OBJ, SnowStorm_P, null, 5, 50, false, false);
                        Make_Dissappear(VolumeFog_OBJ, VolumeFog_P, null, 1, 5, true, false);
                        Make_Dissappear(Lower_Dynamic_Cloud, Dynamic_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Dynamic_Cloud, Dynamic_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Static_Cloud, Cloud_Static_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Static_Cloud, Cloud_Static_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Lower_Cloud_Bed, Cloud_bed_Dn_P, null, 5, 8, false, false);
                        Make_Dissappear(Upper_Cloud_Bed, Cloud_bed_Up_P, null, 5, 8, false, false);

                        Make_Dissappear(Lower_Cloud_Real, Real_Clouds_Dn_P, null, 5, 50, false, false);
                        Make_Dissappear(Upper_Cloud_Real, Real_Clouds_Up_P, null, 5, 50, false, false);

                        Make_Dissappear(Surround_Clouds, Surround_Clouds_P, null, 1, 2, false, false);
                        Make_Dissappear(Surround_Clouds_Heavy, Surround_Clouds_Heavy_P, null, 1, 2, false, false);

                        Make_Dissappear(Ice_Spread_OBJ, Freezer_P, null, 5, 50, false, false);
                        Make_Dissappear(Ice_System_OBJ, Freezer_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly_OBJ, Butterflies2D_P, null, 5, 50, false, false);
                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 50, false, false);
                        }
                        Make_Dissappear(Lightning_System_OBJ, Lightning_System_P, null, 5, 50, false, false);

                        Make_Dissappear(Butterfly3D_OBJ[0], Butterflies3D_P, null, 2, 1, true, true);
                        Make_Dissappear(Rain_Heavy, Heavy_Rain_P, null, 5, 50, false, false);
                        Make_Dissappear(Rain_Mild, Mild_Rain_P, null, 5, 50, false, false);

                        Make_Dissappear(Sun_Ray_Cloud, Sun_Ray_Cloud_P, null, 5, 50, false, false);
                    }
                }

                //////////////////////// HANDLE AUTOMATIC SEASON CHANGE ///////////////////////////////
                if (Seasonal_change_auto & Application.isPlaying) {

                    float Color_divider = 255;

                    int Twelves = Current_Month / 12;
                    float CURRENT_YEAR_MONTHS = (Current_Month) - (Twelves * 12);

                    float CURRENT_YEAR_QUARTER = (float)(CURRENT_YEAR_MONTHS / 3);

                    if (CURRENT_YEAR_QUARTER >= 3) { //////////////////////////////////////////////// WINTER !!!!!!!!!!!! - make autumn clouds dissapear - start cloud bed and freeze
                        Season = 4;

                        if (CURRENT_YEAR_QUARTER > 3.1f & CURRENT_YEAR_QUARTER < 3.65f) {
                            //////////////////////////
                            //SNOW STORM APPEAR
                            if (SnowStorm_OBJ != null) {
                                if (!SnowStorm_OBJ.activeInHierarchy) {
                                    SnowStorm_OBJ.SetActive(true);
                                    Ice_Spread_OBJ.SetActive(true);
                                    Ice_System_OBJ.SetActive(true);
                                    if (SnowStorm_P != null) {
                                        for (int n = 0; n < SnowStorm_P.Length; n++) {
                                            SnowStorm_P[n].Stop();
                                            SnowStorm_P[n].Clear();
                                        }
                                    }
                                } else { //start increasing opaque
                                    if (SnowStorm_P != null) {
                                        for (int n = 0; n < SnowStorm_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = SnowStorm_P[n].main; //v3.4.9
                                            MainMod.maxParticles = Snow_cloud_max_part; // RESTORE max from spring elimination
                                            SnowStorm_P[n].Play();
                                        }
                                    }
                                }
                            }
                        } else if (CURRENT_YEAR_QUARTER >= 3.65f) {
                            //////////////////////////
                            //SNOW STORM DISSAPEAR
                            if (SnowStorm_OBJ != null) {
                                if (SnowStorm_OBJ.activeInHierarchy) {
                                    if (SnowStorm_P != null) {
                                        for (int n = 0; n < SnowStorm_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = SnowStorm_P[n].main; //v3.4.9
                                            MainMod.maxParticles = MainMod.maxParticles - 80;
                                            if (MainMod.maxParticles < 5) { //start increasing opaque
                                                SnowStorm_P[n].Stop();
                                                SnowStorm_P[n].Clear();
                                                SnowStorm_OBJ.SetActive(false);
                                                Ice_Spread_OBJ.SetActive(false);
                                                Ice_System_OBJ.SetActive(false);
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        //////////////////////////
                        //FALLING LEAVES DISSAPEAR - make dissapear in winter
                        for (int o = 0; o < FallingLeaves_OBJ.Length; o++) {
                            if (FallingLeaves_OBJ[o].activeInHierarchy) {
                                if (FallingLeaves_P != null) {
                                    for (int n = 0; n < FallingLeaves_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = FallingLeaves_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 10;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            FallingLeaves_P[n].Stop();
                                            FallingLeaves_P[n].Clear();
                                            FallingLeaves_OBJ[o].SetActive(false);
                                        }
                                    }
                                }
                            }
                        }

                        //////////////////////////
                        //TORNADOS from Autumn DISSAPEAR

                        for (int n = 0; n < Tornado_OBJs.Length; n++) {
                            Make_Dissappear(Tornado_OBJs[n], null, Tornados_P[n], 5, 11, false, false);
                        }

                        //						if(Tornado_OBJs[0].activeInHierarchy){
                        //							if(Tornados_P[0]!=null){
                        //								Tornados_P[0].maxParticles = Tornados_P[0].maxParticles-11;								
                        //								if(Tornados_P[0].maxParticles < 5){ //start increasing opaque
                        //									Tornados_P[0].Stop();
                        //									Tornados_P[0].Clear();
                        //									Tornado_OBJs[0].SetActive(false);
                        //								}
                        //							}
                        //						}

                        //// CLOUDS
                        #region CLOUDS

                        ///////////// HEAVY STORM SEQUENCE /////////////////////
                        if (CURRENT_YEAR_QUARTER >= 3.4f & CURRENT_YEAR_QUARTER < 3.9f) {

                            //HEAVY STORM CLOUDS APEAR
                            if (Surround_Clouds_Heavy != null) {
                                if (!Surround_Clouds_Heavy.activeInHierarchy) {
                                    Surround_Clouds_Heavy.SetActive(true);
                                    if (Surround_Clouds_Heavy_P != null) {
                                        for (int n = 0; n < Surround_Clouds_Heavy_P.Length; n++) {
                                            Surround_Clouds_Heavy_P[n].Stop();
                                            Surround_Clouds_Heavy_P[n].Clear();
                                        }
                                    }
                                } else { //start increasing opaque
                                    if (Surround_Clouds_Heavy_P != null) {
                                        for (int n = 0; n < Surround_Clouds_Heavy_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Surround_Clouds_Heavy_P[n].main; //v3.4.9
                                            MainMod.maxParticles = Storm_cloud_max_part; // RESTORE max from spring elimination
                                            Surround_Clouds_Heavy_P[n].Play();
                                        }
                                    }
                                }
                            }

                            //HEAVY RAIN APEAR
                            if (Rain_Heavy != null) {
                                if (!Rain_Heavy.activeInHierarchy) {
                                    Rain_Heavy.SetActive(true);
                                    if (Heavy_Rain_P != null) {
                                        for (int n = 0; n < Heavy_Rain_P.Length; n++) {
                                            Heavy_Rain_P[n].Stop();
                                            Heavy_Rain_P[n].Clear();
                                        }
                                    }
                                } else { //start increasing opaque
                                    if (Heavy_Rain_P != null) {
                                        for (int n = 0; n < Heavy_Rain_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Heavy_Rain_P[n].main; //v3.4.9
                                            MainMod.maxParticles = 300; // RESTORE max from spring elimination
                                            Heavy_Rain_P[n].Play();
                                        }
                                    }
                                }
                            }

                            /////////// LIGHTNING SINGLE appear
                            if (Lightning_OBJ != null) {
                                Lightning_OBJ.SetActive(true);
                            }
                            //////////////////////////
                            //LOWER BED CLOUDS DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                            if (Lower_Cloud_Bed != null) {
                                if (Lower_Cloud_Bed.activeInHierarchy) {
                                    if (Cloud_bed_Dn_P != null) {
                                        for (int n = 0; n < Cloud_bed_Dn_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Cloud_bed_Dn_P[n].main; //v3.4.9
                                            MainMod.maxParticles = MainMod.maxParticles - 110;
                                            if (MainMod.maxParticles < 5) { //start increasing opaque
                                                Cloud_bed_Dn_P[n].Stop();
                                                Cloud_bed_Dn_P[n].Clear();
                                                Lower_Cloud_Bed.SetActive(false);
                                            }
                                        }
                                    }
                                }
                            }

                        } else {

                            //////////////////////////
                            //CLOUD BED LOWER APPEAR
                            if (Lower_Cloud_Bed != null) {
                                if (!Lower_Cloud_Bed.activeInHierarchy) {
                                    Lower_Cloud_Bed.SetActive(true);
                                    if (Cloud_bed_Dn_P != null) {
                                        for (int n = 0; n < Cloud_bed_Dn_P.Length; n++) {
                                            Cloud_bed_Dn_P[n].Stop();
                                            Cloud_bed_Dn_P[n].Clear();
                                        }
                                    }
                                } else { //start increasing opaque
                                    if (Cloud_bed_Dn_P != null) {
                                        for (int n = 0; n < Cloud_bed_Dn_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Cloud_bed_Dn_P[n].main; //v3.4.9
                                            MainMod.maxParticles = Flat_cloud_max_part; // RESTORE max from spring elimination
                                            Cloud_bed_Dn_P[n].Play();
                                        }
                                    }
                                }
                            }
                        }
                        //////////////////////////
                        //CLOUD BED UPPER APPEAR 
                        if (Upper_Cloud_Bed != null) {
                            if (!Upper_Cloud_Bed.activeInHierarchy) {
                                Upper_Cloud_Bed.SetActive(true);
                                if (Cloud_bed_Up_P != null) {
                                    for (int n = 0; n < Cloud_bed_Up_P.Length; n++) {
                                        Cloud_bed_Up_P[n].Stop();
                                        Cloud_bed_Up_P[n].Clear();
                                    }
                                }
                            } else { //start increasing opaque
                                if (Cloud_bed_Up_P != null) {
                                    for (int n = 0; n < Cloud_bed_Up_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Cloud_bed_Up_P[n].main; //v3.4.9
                                        MainMod.maxParticles = Flat_cloud_max_part; // RESTORE max from spring elimination
                                        Cloud_bed_Up_P[n].Play();
                                    }
                                }
                            }
                        }


                        //////////////////////////
                        //LOWER REAL CLOUDS DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                        if (Lower_Cloud_Real != null) {
                            if (Lower_Cloud_Real.activeInHierarchy) {
                                if (Real_Clouds_Dn_P != null) {
                                    for (int n = 0; n < Real_Clouds_Dn_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Dn_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 10;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            Real_Clouds_Dn_P[n].Stop();
                                            Real_Clouds_Dn_P[n].Clear();
                                            Lower_Cloud_Real.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }
                        //////////////////////////
                        //UPPER REAL CLOUDS DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                        if (Upper_Cloud_Real != null) {
                            if (Upper_Cloud_Real.activeInHierarchy) {
                                if (Real_Clouds_Up_P != null) {
                                    for (int n = 0; n < Real_Clouds_Up_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Up_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 10;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            Real_Clouds_Up_P[n].Stop();
                                            Real_Clouds_Up_P[n].Clear();
                                            Upper_Cloud_Real.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        //// END CLOUDS

                        //// FOG
                        #region FOG
                        bool Enable_fog = true;
                        if (CURRENT_YEAR_QUARTER >= 3.8f) { Enable_fog = false; Lightning_System_OBJ.SetActive(true); }
                        if (Enable_fog) {
                            if (!RenderSettings.fog) {
                                RenderSettings.fog = true;
                            }
                            Color Fog_color = new Color(60 / Color_divider, 70 / Color_divider, 75 / Color_divider, 255 / Color_divider);
                            float Fog_density = 0.01f;

                            if (Current_Time < 18 & Current_Time > 9) {
                                Fog_color = Winter_fog_day;
                                Fog_density = Winter_fog_day_density * Fog_Density_Mult;
                            } else if (Current_Time <= 9 & Current_Time > 1) {
                                Fog_color = Winter_fog_night;
                                Fog_density = Winter_fog_night_density * Fog_Density_Mult;
                            } else {
                                Fog_color = Winter_fog_dusk;
                                Fog_density = Winter_fog_dusk_density * Fog_Density_Mult;
                            }

                            FogMode Fog_Mode = fogMode;//FogMode.ExponentialSquared;//v4.5

                            float Fog_density2 = Fog_density;   // correction for heavy fog, no cloud dome, so has to be more mild 
                            float Density_Speed = Fog_Density_Speed;
                            if (Weather == Weather_types.HeavyFog) {
                                Fog_density2 = Fog_density / 2;
                                Density_Speed = 0.015f;
                            }
                            //Make_Fog_Appear(Fog_color,Fog_density2,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                            Make_Fog_Appear(Fog_color, Fog_density2, Fog_Mode, Density_Speed, 0);


                        } else {
                            if (RenderSettings.fog) {
                                RenderSettings.fog = false;
                            }
                        }
                        #endregion
                        //// END FOG

                    } else if (CURRENT_YEAR_QUARTER >= 2) { /////////////////////////////////////// AUTUMN !!!!!!!!!!!!!!!!
                        Season = 3;

                        //////////////////////////
                        //FALLING LEAVES APEAR
                        if (CURRENT_YEAR_QUARTER >= 2.0f) {
                            for (int o = 0; o < FallingLeaves_OBJ.Length; o++) {
                                if (!FallingLeaves_OBJ[o].activeInHierarchy) {
                                    FallingLeaves_OBJ[o].SetActive(true);
                                    if (FallingLeaves_P != null) {
                                        for (int n = 0; n < FallingLeaves_P.Length; n++) {
                                            FallingLeaves_P[n].Stop();
                                            FallingLeaves_P[n].Clear();
                                        }
                                    }
                                } else { //start increasing opaque
                                    if (FallingLeaves_P != null) {
                                        for (int n = 0; n < FallingLeaves_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = FallingLeaves_P[n].main; //v3.4.9
                                            MainMod.maxParticles = 300; // RESTORE max from spring elimination
                                            FallingLeaves_P[n].Play();
                                        }
                                    }
                                }
                            }
                        }



                        //////////////////////////
                        //TORNADOS APPEAR in last third of season



                        if (CURRENT_YEAR_QUARTER >= 2.5f) {

                            for (int n = 0; n < Tornado_OBJs.Length; n++) {
                                Make_Appear(Tornado_OBJs[n], null, Tornados_P[n], 200, false, 25);
                            }

                            //							if(!Tornado_OBJs[0].activeInHierarchy){
                            //								Tornado_OBJs[0].SetActive(true);
                            //								if(Tornados_P[0]!=null){
                            //									Tornados_P[0].Stop();
                            //									Tornados_P[0].Clear();
                            //									Tornados_P[0].maxParticles = 0;
                            //								}
                            //							}else{ //start increasing opaque
                            //								if(Tornados_P[0]!=null){
                            //									if(Tornados_P[0].maxParticles < 200){
                            //										Tornados_P[0].maxParticles += 25; // RESTORE max from spring elimination
                            //									}
                            //									if(!Tornados_P[0].isPlaying){
                            //										Tornados_P[0].Play();
                            //									}
                            //								}
                            //							}
                        }

                        //// CLOUDS
                        #region CLOUDS
                        //////////////////////////
                        //LOWER REAL CLOUDS APEAR
                        if (Lower_Cloud_Real != null) {
                            if (!Lower_Cloud_Real.activeInHierarchy) {
                                Lower_Cloud_Real.SetActive(true);
                                if (Real_Clouds_Dn_P != null) {
                                    for (int n = 0; n < Real_Clouds_Dn_P.Length; n++) {
                                        Real_Clouds_Dn_P[n].Stop();
                                        Real_Clouds_Dn_P[n].Clear();
                                    }
                                }
                            } else { //start increasing opaque
                                if (Real_Clouds_Dn_P != null) {
                                    for (int n = 0; n < Real_Clouds_Dn_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Dn_P[n].main; //v3.4.9
                                        MainMod.maxParticles = Real_cloud_max_part; // RESTORE max from spring elimination
                                        Real_Clouds_Dn_P[n].Play();
                                    }
                                }
                            }
                        }
                        //////////////////////////
                        //UPPER REAL CLOUDS APEAR
                        if (Upper_Cloud_Real != null) {
                            if (!Upper_Cloud_Real.activeInHierarchy) {
                                Upper_Cloud_Real.SetActive(true);
                                if (Real_Clouds_Up_P != null) {
                                    for (int n = 0; n < Real_Clouds_Up_P.Length; n++) {
                                        Real_Clouds_Up_P[n].Stop();
                                        Real_Clouds_Up_P[n].Clear();
                                    }
                                }
                            } else { //start increasing opaque
                                if (Real_Clouds_Up_P != null) {
                                    for (int n = 0; n < Real_Clouds_Up_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Up_P[n].main; //v3.4.9
                                        MainMod.maxParticles = Real_cloud_max_part; // RESTORE max from spring elimination
                                        Real_Clouds_Up_P[n].Play();
                                    }
                                }
                            }
                        }

                        //////////////////////////
                        //SURROUND CLOUDS APEAR
                        if (CURRENT_YEAR_QUARTER >= 2.5f) {
                            if (Surround_Clouds != null) {
                                if (!Surround_Clouds.activeInHierarchy) {
                                    Surround_Clouds.SetActive(true);
                                    if (Surround_Clouds_P != null) {
                                        for (int n = 0; n < Surround_Clouds_P.Length; n++) {
                                            Surround_Clouds_P[n].Stop();
                                            Surround_Clouds_P[n].Clear();
                                        }
                                    }
                                } else { //start increasing opaque
                                    if (Surround_Clouds_P != null) {
                                        for (int n = 0; n < Surround_Clouds_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Surround_Clouds_P[n].main; //v3.4.9
                                            MainMod.maxParticles = 300; // RESTORE max from spring elimination
                                            Surround_Clouds_P[n].Play();
                                        }
                                    }
                                }
                            }
                        }

                        //v1.2.5
                        //////////////////////////
                        //MOVING CLOUDS DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                        if (Lower_Dynamic_Cloud != null) {
                            if (Lower_Dynamic_Cloud.activeInHierarchy) {
                                if (Dynamic_Clouds_Dn_P != null) {
                                    for (int n = 0; n < Dynamic_Clouds_Dn_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Dynamic_Clouds_Dn_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 10;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            Dynamic_Clouds_Dn_P[n].Stop();
                                            Dynamic_Clouds_Dn_P[n].Clear();
                                            Lower_Dynamic_Cloud.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }
                        //MOVING CLOUDS DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                        if (Upper_Dynamic_Cloud != null) {
                            if (Upper_Dynamic_Cloud.activeInHierarchy) {
                                if (Dynamic_Clouds_Up_P != null) {
                                    for (int n = 0; n < Dynamic_Clouds_Up_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Dynamic_Clouds_Up_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 10;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            Dynamic_Clouds_Up_P[n].Stop();
                                            Dynamic_Clouds_Up_P[n].Clear();
                                            Upper_Dynamic_Cloud.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                        ///// END CLOUDS
                        if (Sun_Ray_Cloud != null) {
                            Sun_Ray_Cloud.SetActive(false);
                        }
                        //// FOG
                        #region FOG
                        bool Enable_fog = true;
                        if (CURRENT_YEAR_QUARTER >= 2.4f) { Enable_fog = true; }
                        if (Enable_fog) {
                            if (!RenderSettings.fog) {
                                RenderSettings.fog = true;
                            }
                            //RenderSettings.fogColor = new Color(100/Color_divider,80/Color_divider,100/Color_divider,255/Color_divider);
                            Color Fog_color = new Color(100 / Color_divider, 80 / Color_divider, 100 / Color_divider, 255 / Color_divider);
                            float Fog_density = 0.01f;

                            if (Current_Time < 18 & Current_Time > 9) {
                                Fog_color = Autumn_fog_day;
                                Fog_density = Autumn_fog_day_density * Fog_Density_Mult;
                            } else if (Current_Time <= 9 & Current_Time > 1) {
                                Fog_color = Autumn_fog_night;
                                Fog_density = Autumn_fog_night_density * Fog_Density_Mult;
                            } else {
                                Fog_color = Autumn_fog_dusk;
                                Fog_density = Autumn_fog_dusk_density * Fog_Density_Mult;
                            }

                            FogMode Fog_Mode = fogMode;//FogMode.ExponentialSquared;
                            float Fog_density2 = Fog_density;   // correction for heavy fog, no cloud dome, so has to be more mild 
                            float Density_Speed = Fog_Density_Speed;
                            if (Weather == Weather_types.HeavyFog) {
                                Fog_density2 = Fog_density / 2;
                                Density_Speed = 0.015f;
                            }

                            //Make_Fog_Appear(Fog_color,Fog_density2,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                            Make_Fog_Appear(Fog_color, Fog_density2, Fog_Mode, Density_Speed, 0);


                        } else {
                            if (RenderSettings.fog) {
                                RenderSettings.fog = false;
                            }
                        }
                        #endregion
                        //// END FOG

                    } else if (CURRENT_YEAR_QUARTER >= 1) { /////////////////////////////////////// SUMMER !!!!!!!!!!!!!!!!!!!
                        Season = 2;

                        //// CLOUDS
                        #region CLOUDS
                        //////////////////////////
                        //LOWER CLOUD BED DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                        if (Lower_Cloud_Bed != null) {
                            if (Lower_Cloud_Bed.activeInHierarchy) {
                                if (Cloud_bed_Dn_P != null) {
                                    for (int n = 0; n < Cloud_bed_Dn_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Cloud_bed_Dn_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 10;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            Cloud_bed_Dn_P[n].Stop();
                                            Cloud_bed_Dn_P[n].Clear();
                                            Lower_Cloud_Bed.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }
                        //////////////////////////
                        //UPPER CLOUD BED DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                        if (Upper_Cloud_Bed != null) {
                            if (Upper_Cloud_Bed.activeInHierarchy) {
                                if (Cloud_bed_Up_P != null) {
                                    for (int n = 0; n < Cloud_bed_Up_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Cloud_bed_Up_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 10;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            Cloud_bed_Up_P[n].Stop();
                                            Cloud_bed_Up_P[n].Clear();
                                            Upper_Cloud_Bed.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        //// END CLOUDS

                        //////////////////////////
                        //BUTTERFLIES DISSAPEAR - make dissapear in summer
                        if (Butterfly_OBJ != null) {
                            if (Butterfly_OBJ.activeInHierarchy) {
                                if (Butterflies2D_P != null) {
                                    for (int n = 0; n < Butterflies2D_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Butterflies2D_P[n].main; //v3.4.9
                                        MainMod.maxParticles = MainMod.maxParticles - 40;
                                        if (MainMod.maxParticles < 5) { //start increasing opaque
                                            Butterflies2D_P[n].Stop();
                                            Butterflies2D_P[n].Clear();
                                            Butterfly_OBJ.SetActive(false);
                                        }
                                    }
                                }
                            }
                        }

                    } else if (CURRENT_YEAR_QUARTER >= 0) { ////////////////////////////////////// SPRING !!!!!!!!!!!!!!!!!!!
                        Season = 1;

                        //////////////////////////
                        //BUTTERFLIES APPEAR
                        if (Butterfly_OBJ != null) {
                            if (!Butterfly_OBJ.activeInHierarchy) {
                                Butterfly_OBJ.SetActive(true);
                                if (Butterflies2D_P != null) {
                                    for (int n = 0; n < Butterflies2D_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Butterflies2D_P[n].main; //v3.4.9
                                        MainMod.startColor = new Color(MainMod.startColor.color.r, MainMod.startColor.color.g, MainMod.startColor.color.b, 0);
                                    }
                                }
                            } else { //start increasing opaque
                                if (Butterflies2D_P != null) {
                                    for (int n = 0; n < Butterflies2D_P.Length; n++) {
                                        ParticleSystem.MainModule MainMod = Butterflies2D_P[n].main; //v3.4.9
                                        Color START_C = MainMod.startColor.color;
                                        MainMod.startColor = Color.Lerp(START_C, new Color(START_C.r, START_C.g, START_C.b, 1), 0.5f * Time.deltaTime);
                                    }
                                }
                            }
                        }

                        //// CLOUDS
                        #region CLOUDS
                        //////////////////////////
                        //LOWER DYNAMIC CLOUDS APPEAR
                        if (Lower_Dynamic_Cloud != null) {
                            if (!Lower_Dynamic_Cloud.activeInHierarchy) {
                                Lower_Dynamic_Cloud.SetActive(true);
                                if (Dynamic_Clouds_Dn_P != null) {
                                    for (int n = 0; n < Dynamic_Clouds_Dn_P.Length; n++) {
                                        Dynamic_Clouds_Dn_P[n].Stop();
                                        Dynamic_Clouds_Dn_P[n].Clear();
                                    }
                                }
                            } else { //start increasing opaque
                                if (Dynamic_Clouds_Dn_P != null) {
                                    for (int n = 0; n < Dynamic_Clouds_Dn_P.Length; n++) {
                                        Dynamic_Clouds_Dn_P[n].Play();
                                    }
                                }
                            }
                        }
                        //////////////////////////
                        //UPPER DYNAMIC CLOUDS APPEAR
                        if (Upper_Dynamic_Cloud != null) {
                            if (!Upper_Dynamic_Cloud.activeInHierarchy) {
                                Upper_Dynamic_Cloud.SetActive(true);
                                if (Dynamic_Clouds_Up_P != null) {
                                    for (int n = 0; n < Dynamic_Clouds_Up_P.Length; n++) {
                                        Dynamic_Clouds_Up_P[n].Stop();
                                        Dynamic_Clouds_Up_P[n].Clear();
                                    }
                                }
                            } else { //start increasing opaque
                                if (Dynamic_Clouds_Up_P != null) {
                                    for (int n = 0; n < Dynamic_Clouds_Up_P.Length; n++) {
                                        Dynamic_Clouds_Up_P[n].Play();
                                    }
                                }
                            }
                        }
                        //////////////////////////
                        //SURROUND CLOUDS DISSAPEAR at end of spring - make dissapear in spring- first reduce max particles, then check max and stop system
                        if (CURRENT_YEAR_QUARTER > 0.6f) {
                            //Debug.Log ("ASER1");
                            if (Surround_Clouds != null) {
                                if (Surround_Clouds.activeInHierarchy) {
                                    //Debug.Log ("ASER2");
                                    if (Surround_Clouds_P != null) {
                                        //Debug.Log ("ASER3");
                                        for (int n = 0; n < Surround_Clouds_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Surround_Clouds_P[n].main; //v3.4.9
                                            MainMod.maxParticles = MainMod.maxParticles - 20; //Debug.Log ("ASER4");								
                                            if (MainMod.maxParticles < 5) { //start increasing opaque
                                                Surround_Clouds_P[n].Stop();
                                                Surround_Clouds_P[n].Clear();
                                                Surround_Clouds.SetActive(false);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (CURRENT_YEAR_QUARTER >= 0.4f) {
                            //////////////////////////
                            //LOWER CLOUD BED DISSAPEAR - make dissapear in spring- first reduce max particles, then check max and stop system
                            if (Surround_Clouds_Heavy != null) {
                                if (Surround_Clouds_Heavy.activeInHierarchy) {
                                    if (Surround_Clouds_Heavy_P != null) {
                                        for (int n = 0; n < Surround_Clouds_Heavy_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Surround_Clouds_Heavy_P[n].main; //v3.4.9
                                            MainMod.maxParticles = MainMod.maxParticles - 10;
                                            if (MainMod.maxParticles < 5) { //start increasing opaque
                                                Surround_Clouds_Heavy_P[n].Stop();
                                                Surround_Clouds_Heavy_P[n].Clear();
                                                Surround_Clouds_Heavy.SetActive(false);
                                            }
                                        }
                                    }
                                }
                            }

                            if (Rain_Heavy != null) {
                                if (Rain_Heavy.activeInHierarchy) {
                                    if (Heavy_Rain_P != null) {
                                        for (int n = 0; n < Heavy_Rain_P.Length; n++) {
                                            ParticleSystem.MainModule MainMod = Heavy_Rain_P[n].main; //v3.4.9
                                            MainMod.maxParticles = MainMod.maxParticles - 10;
                                            if (MainMod.maxParticles < 5) { //start increasing opaque
                                                Heavy_Rain_P[n].Stop();
                                                Heavy_Rain_P[n].Clear();
                                                Rain_Heavy.SetActive(false);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                        //// END CLOUDS
                        if (Sun_Ray_Cloud != null) {
                            Sun_Ray_Cloud.SetActive(true);
                        }
                        //// FOG
                        #region FOG
                        bool Enable_fog = false;
                        //if(CURRENT_YEAR_QUARTER >= 2.4f){ Enable_fog = true; }
                        if (Enable_fog) {
                            if (!RenderSettings.fog) {
                                RenderSettings.fog = true;
                            }
                            Color Fog_color = Spring_fog_day;
                            float Fog_density = Spring_fog_day_density * Fog_Density_Mult;

                            if (Current_Time < 18 & Current_Time > 9) {
                                Fog_color = Spring_fog_day;
                                Fog_density = Spring_fog_day_density * Fog_Density_Mult;
                            } else if (Current_Time <= 9 & Current_Time > 1) {
                                Fog_color = Spring_fog_night;
                                Fog_density = Spring_fog_night_density * Fog_Density_Mult;
                            } else {
                                Fog_color = Spring_fog_dusk;
                                Fog_density = Spring_fog_dusk_density * Fog_Density_Mult;
                            }

                            FogMode Fog_Mode = fogMode;//FogMode.ExponentialSquared;
                            float Density_Speed = Fog_Density_Speed;
                            //Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
                            Make_Fog_Appear(Fog_color, Fog_density, Fog_Mode, Density_Speed, 0);


                        } else {
                            if (RenderSettings.fog) {
                                RenderSettings.fog = false;
                            }
                        }
                        #endregion
                        //// END FOG
                        /////////// end sping /////////////
                    }
                }



                //////////////////////// UPDATE MOON LIGHTING - PHASES - SCREEN WATER DROPS MATERIAL
                if (ScreenRainDrops && RainDropsPlane != null && ScreenRainDropsMat != null) {
                    if (currentWeatherName == Volume_Weather_types.HeavyStorm | currentWeatherName == Volume_Weather_types.HeavyStormDark
                       | currentWeatherName == Volume_Weather_types.Rain
                           | (water != null && water.GetComponent<WaterHandlerSM>() != null && water.GetComponent<WaterHandlerSM>().BelowWater)
                           ) {

                        if (ScreenFreezeFX) {
                            //decrease speed to zero
                            float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                            ScreenRainDropsMat.SetFloat("_Speed", Mathf.Lerp(Drops_speed, 0, Time.deltaTime * FreezeSpeed));
                            //increase water from zero to MaxWater
                            float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                            ScreenRainDropsMat.SetFloat("_WaterAmount", Mathf.Lerp(_WaterAmount, MaxWater, Time.deltaTime * FreezeSpeed));
                            //increase refraction from zero to MaxRefract * 4
                            float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                            ScreenRainDropsMat.SetFloat("_BumpAmt", Mathf.Lerp(RefractAmount, MaxRefract * 4, Time.deltaTime * FreezeSpeed));

                            if (FreezeInwards) {
                                float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorA", Mathf.Lerp(FreezeFactor1, 0, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorB", Mathf.Lerp(FreezeFactor2, 0, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorC", Mathf.Lerp(FreezeFactor3, 1, Time.deltaTime * FreezeSpeed));
                            } else {
                                float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorA", Mathf.Lerp(FreezeFactor1, 1, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorB", Mathf.Lerp(FreezeFactor2, 1, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorC", Mathf.Lerp(FreezeFactor3, 0, Time.deltaTime * FreezeSpeed));
                            }
                        } else {
                            //increase speed from zero to MaxDropSpeed, MaxWater, MaxRefract
                            float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                            ScreenRainDropsMat.SetFloat("_Speed", Mathf.Lerp(Drops_speed, MaxDropSpeed, Time.deltaTime * FreezeSpeed));
                            //increase water from zero to MaxWater
                            float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                            ScreenRainDropsMat.SetFloat("_WaterAmount", Mathf.Lerp(_WaterAmount, MaxWater, Time.deltaTime * FreezeSpeed));
                            //increase refraction from zero to MaxRefract
                            float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                            ScreenRainDropsMat.SetFloat("_BumpAmt", Mathf.Lerp(RefractAmount, MaxRefract, Time.deltaTime * FreezeSpeed));
                        }

                    } else {//if not raining, fade out
                        if (ScreenFreezeFX | currentWeatherName == Volume_Weather_types.FreezeStorm) {
                            //decrease speed to zero
                            float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                            ScreenRainDropsMat.SetFloat("_Speed", Mathf.Lerp(Drops_speed, 0, Time.deltaTime * FreezeSpeed));
                            //increase water from zero to MaxWater
                            float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                            ScreenRainDropsMat.SetFloat("_WaterAmount", Mathf.Lerp(_WaterAmount, MaxWater, Time.deltaTime * FreezeSpeed));
                            //increase refraction from zero to MaxRefract * 4
                            float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                            ScreenRainDropsMat.SetFloat("_BumpAmt", Mathf.Lerp(RefractAmount, MaxRefract * 4, Time.deltaTime * FreezeSpeed));

                            if (FreezeInwards) {
                                float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorA", Mathf.Lerp(FreezeFactor1, 0, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorB", Mathf.Lerp(FreezeFactor2, 0, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorC", Mathf.Lerp(FreezeFactor3, 1, Time.deltaTime * FreezeSpeed));
                            } else {
                                float FreezeFactor1 = ScreenRainDropsMat.GetFloat("_FreezeFacrorA");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorA", Mathf.Lerp(FreezeFactor1, 1, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor2 = ScreenRainDropsMat.GetFloat("_FreezeFacrorB");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorB", Mathf.Lerp(FreezeFactor2, 1, Time.deltaTime * FreezeSpeed));
                                float FreezeFactor3 = ScreenRainDropsMat.GetFloat("_FreezeFacrorC");
                                ScreenRainDropsMat.SetFloat("_FreezeFacrorC", Mathf.Lerp(FreezeFactor3, 0, Time.deltaTime * FreezeSpeed));
                            }
                        } else {
                            //decrease speed to zero to MaxDropSpeed, MaxWater, MaxRefract
                            //float Drops_speed = ScreenRainDropsMat.GetFloat("_Speed");
                            //ScreenRainDropsMat.SetFloat("_Speed",Mathf.Lerp(Drops_speed, MaxDropSpeed, Time.deltaTime));
                            //increase speed from zero to MaxWater
                            float _WaterAmount = ScreenRainDropsMat.GetFloat("_WaterAmount");
                            ScreenRainDropsMat.SetFloat("_WaterAmount", Mathf.Lerp(_WaterAmount, 0, Time.deltaTime * FreezeSpeed));
                            //increase refraction from zero to MaxRefract
                            float RefractAmount = ScreenRainDropsMat.GetFloat("_BumpAmt");
                            ScreenRainDropsMat.SetFloat("_BumpAmt", Mathf.Lerp(RefractAmount, 0, Time.deltaTime * FreezeSpeed));
                        }
                    }
                }
                //MOON LIGHT
                //v3.0
                if (
                        (!AutoSunPosition && (Current_Time >= (9 + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn)))
                        |
                        (AutoSunPosition && Rot_Sun_X > 0)
                       ) {
                    //if(Current_Time >= (9 + Shift_dawn) & Current_Time <= (22.4f + Shift_dawn)  ){
                    float intensity = SUPPORT_LIGHT.GetComponent<Light>().intensity;
                    if (intensity < 0.2f) {
                        SUPPORT_LIGHT.GetComponent<Light>().intensity = Mathf.Lerp(intensity, 0.2f, Time.deltaTime);
                    }
                    float intensity1 = MOON_LIGHT.GetComponent<Light>().intensity;
                    if (intensity1 > 0) {
                        MOON_LIGHT.GetComponent<Light>().intensity = Mathf.Lerp(intensity1, 0.0f, Time.deltaTime);
                    }

                    //v3.3 - add fading of moon if sun above horizon
                    //					if(LatLonMoonPos & AutoMoonFade){
                    //						
                    //					}

                } else {
                    float intensity = SUPPORT_LIGHT.GetComponent<Light>().intensity;
                    if (intensity > 0) {
                        SUPPORT_LIGHT.GetComponent<Light>().intensity = Mathf.Lerp(intensity, 0.0f, Time.deltaTime);
                    }
                    float intensity1 = MOON_LIGHT.GetComponent<Light>().intensity;
                    if (intensity1 != max_moon_intensity) {
                        MOON_LIGHT.GetComponent<Light>().intensity = Mathf.Lerp(intensity1, max_moon_intensity, Time.deltaTime * 0.05f); //v3.4.3
                    }

                    //v3.3 - add un-fading of moon if sun below horizon
                    //					if(LatLonMoonPos & AutoMoonFade){
                    //
                    //					}
                }
                //MOON PHASES
                if (MoonPhases && MoonPhasesMat != null) {

                    float Eclipse_factor = 0;

                    //if (LatLonMoonPos) {
                    if (AutoMoonLighting && Camera.main != null)
                    {
                        Vector3 Camera_to_moon = (MoonObj.transform.position - Camera.main.transform.position).normalized;
                        Vector3 Moon_to_sun = (SunObj.transform.position - MoonObj.transform.position).normalized;
                        Vector3 Normal = -Camera.main.transform.up;
                        Vector3 NormalY = Camera.main.transform.right;
                        float Angle_signed = Mathf.Atan2(Vector3.Dot(Normal, Vector3.Cross(Camera_to_moon, Moon_to_sun)), Vector3.Dot(Camera_to_moon, Moon_to_sun)) * Mathf.Rad2Deg;
                        float Angle_signedY = Mathf.Atan2(Vector3.Dot(NormalY, Vector3.Cross(Camera_to_moon, Moon_to_sun)), Vector3.Dot(Camera_to_moon, Moon_to_sun)) * Mathf.Rad2Deg;
                        //Debug.Log ("ANGLE=" + Vector3.Angle (Camera_to_moon, Moon_to_sun));
                        //Debug.Log ("ANGLES=" + Angle_signed);
                        //Debug.Log ("ANGLESY=" + Angle_signedY);

                        Vector3 Camera_to_sun = (SunObj.transform.position - Camera.main.transform.position).normalized;
                        float AngleA = Vector3.Angle(Camera_to_moon, Camera_to_sun);
                        float AngleSign = 10;
                        if (AngleA < 90) {
                            AngleSign = -1;
                        }

                        if (AngleA < 1.5f) { //0.83f) {
                            Eclipse_factor = -2000;
                            onEclipse = true;
                        } else {
                            onEclipse = false;
                        }
                        //Debug.Log (AngleA);
                        float DistBased = Eclipse_factor - 2200 + AngleSign * 20 * AngleA;

                        MoonPhasesMat.SetVector("_SunDir", Vector3.Lerp(MoonPhasesMat.GetVector("_SunDir"), new Vector3(Angle_signed * 200, Angle_signedY * 350, DistBased), Time.deltaTime * SPEED * 1.2f));

                        //v3.3 - add un-fading of moon if sun below horizon
                        if (AutoMoonFade) {
                            Color MoonCol = MoonPhasesMat.GetColor("_Color");

                            if (
                                (!AutoSunPosition && (Current_Time >= (9 + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn)))
                                |
                                (AutoSunPosition && Rot_Sun_X > 0))
                            {
                                if (onEclipse) {
                                    MoonPhasesMat.SetColor("_Color", Color.Lerp(MoonCol, new Color(MoonColor.r, MoonColor.g, MoonColor.b, 1 - MinMaxEclMoonTransp.z), Time.deltaTime));
                                } else {
                                    MoonPhasesMat.SetColor("_Color", Color.Lerp(MoonCol, new Color(MoonColor.r, MoonColor.g, MoonColor.b, 1 - MinMaxEclMoonTransp.y), Time.deltaTime));
                                }
                            } else {
                                MoonPhasesMat.SetColor("_Color", Color.Lerp(MoonCol, new Color(MoonColor.r, MoonColor.g, MoonColor.b, 1 - MinMaxEclMoonTransp.x), Time.deltaTime));
                            }

                        } else {
                            Color MoonCol = MoonPhasesMat.GetColor("_Color");
                            MoonPhasesMat.SetColor("_Color", Color.Lerp(MoonCol, MoonColor, Time.deltaTime));
                        }

                    } else {

                        //pass sun postion to material shader, add variant based on day of month
                        //float month = (Current_Month+1 % 12) -1;
                        float month1 = Current_Month - ((((int)((Current_Month + 0) / 12))) * 12);
                        //Debug.Log(month1);
                        //float day1 = Current_Day - (Current_Month * days_per_month);
                        float day1 = Current_Day - ((((int)((Current_Day + 0) / days_per_month))) * days_per_month);

                        float day2 = -30 + (day1) * 2;
                        //Debug.Log(day2);
                        MoonPhasesMat.SetVector("_SunDir", Vector3.Lerp(MoonPhasesMat.GetVector("_SunDir"), new Vector3(day2 * 350 * 2 + 500, month1 * 350 * 2 + 500, day2 * 800), Time.deltaTime * SPEED));

                        //v3.3 - set moon color
                        Color MoonCol = MoonPhasesMat.GetColor("_Color");
                        MoonPhasesMat.SetColor("_Color", Color.Lerp(MoonCol, MoonColor, Time.deltaTime));
                    }

                    //v3.3 - moon ambient, sun light and coverage
                    Color MoonACol = MoonPhasesMat.GetColor("_Ambient");
                    if (onEclipse) {
                        MoonPhasesMat.SetColor("_Ambient", Color.Lerp(MoonACol, Color.black, Time.deltaTime));
                    } else {
                        MoonPhasesMat.SetColor("_Ambient", Color.Lerp(MoonACol, MoonAmbientColor, Time.deltaTime));
                    }

                    float MoonLight = MoonPhasesMat.GetFloat("_Light");
                    MoonPhasesMat.SetFloat("_Light", Mathf.Lerp(MoonLight, MoonSunLight, Time.deltaTime));

                    float MoonCoverage1 = MoonCoverage;
                    if (Eclipse_factor != 0) {
                        MoonCoverage1 = 0.8f;
                    }

                    float MoonCover = MoonPhasesMat.GetFloat("_CloudCover");
                    MoonPhasesMat.SetFloat("_CloudCover", Mathf.Lerp(MoonCover, MoonCoverage1, Time.deltaTime));

                }

                //v3.3 - shader based stars fade
                if (StarsMaterial != null) {
                    Color StarsCol = StarsMaterial.GetColor("_Color");
                    if (
                        (!AutoSunPosition && (Current_Time >= (9 + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn)))
                        |
                        (AutoSunPosition && Rot_Sun_X > 0))
                    {
                        if (Current_Time < 1f) { //initialize at game start
                            StarsMaterial.SetColor("_Color", new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.y));
                        } else {
                            StarsMaterial.SetColor("_Color", Color.Lerp(StarsCol, new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.y), Time.deltaTime));
                        }
                    } else {
                        if (Current_Time < 1f) { //initialize at game start
                            StarsMaterial.SetColor("_Color", new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.x));
                        } else {
                            StarsMaterial.SetColor("_Color", Color.Lerp(StarsCol, new Color(StarsColor.r, StarsColor.g, StarsColor.b, 1 - MinMaxStarsTransp.x), Time.deltaTime));
                        }
                    }
                    float StarsCover = StarsMaterial.GetFloat("_Light");
                    StarsMaterial.SetFloat("_Light", Mathf.Lerp(StarsCover, StarsIntensity, Time.deltaTime));
                }

                //v3.3 - moon size
                if (MoonSize != 1) {
                    MoonObj.transform.localScale = 1800 * MoonSize * Vector3.one;
                }

                //v4.2 - disable sun at night time
                //public bool disableSunAtNight = false;
                if (disableSunAtNight)
                {
                    if (
                        (!AutoSunPosition && (Current_Time >= (9 + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn)))
                        |
                        (AutoSunPosition && Rot_Sun_X > 0))
                    {
                        if (!SUN_LIGHT.GetComponent<Light>().enabled)
                        {
                            SUN_LIGHT.GetComponent<Light>().enabled = true;
                        }
                    }
                    else
                    {
                        if (SUN_LIGHT.GetComponent<Light>().enabled)
                        {
                            SUN_LIGHT.GetComponent<Light>().enabled = false;
                        }
                    }
                }

                //////////////////////// HANDLE CLOUD MATERIALS Based on time of day //////////////////

                float Color_devider = 255;

                //Color Cloud1 = cloud_downMaterial.GetVector("_TintColor");
                //Color Cloud2 = cloud_upMaterial.GetVector("_TintColor");

                //Color FlatCloud1 = flat_cloud_downMaterial.GetVector("_EmisColor");
                //Color FlatCloud2 = flat_cloud_upMaterial.GetVector("_TintColor");

                //Color CloudDomeR = cloud_dome_downMaterial.GetVector("_TintColor");

                //v5.1.2
                Color Cloud1 = Color.white;
                if (cloud_downMaterial) {
                    Cloud1 = cloud_downMaterial.GetVector("_TintColor");
                }
                Color Cloud2 = Color.white;
                if (cloud_upMaterial)
                {
                    Cloud2 = cloud_upMaterial.GetVector("_TintColor");
                }
                Color FlatCloud1 = Color.white;
                if (flat_cloud_downMaterial)
                {
                    FlatCloud1 = flat_cloud_downMaterial.GetVector("_EmisColor");
                }
                Color FlatCloud2 = Color.white;
                if (flat_cloud_upMaterial)
                {
                    FlatCloud2 = flat_cloud_upMaterial.GetVector("_TintColor");
                }
                Color CloudDomeR = Color.white;
                if (cloud_dome_downMaterial)
                {
                    CloudDomeR = cloud_dome_downMaterial.GetVector("_TintColor");
                }

                //v2.2 - Shader based cloud dome (2 layers)
                Color ShaderCloudDomeC1 = Color.white;
                Color ShaderCloudDomeC2 = Color.white;
                Color ShaderCloudDomeC1A = Color.white;
                Color ShaderCloudDomeC2A = Color.white;
                float ShaderCloudDomeCoverage1 = 0;
                float ShaderCloudDomeCoverage2 = 0;
                if (CloudDomeL1Mat != null & CloudDomeL2Mat != null) {
                    ShaderCloudDomeC1 = CloudDomeL1Mat.GetVector("_Color");
                    ShaderCloudDomeC2 = CloudDomeL2Mat.GetVector("_Color");
                    ShaderCloudDomeCoverage1 = CloudDomeL1Mat.GetFloat("_CloudCover");
                    ShaderCloudDomeCoverage2 = CloudDomeL2Mat.GetFloat("_CloudCover");
                    ShaderCloudDomeC1A = CloudDomeL1Mat.GetVector("_Ambient");
                    ShaderCloudDomeC2A = CloudDomeL2Mat.GetVector("_Ambient");

                    //extra controls for dome L1
                    CloudDomeL1Mat.SetFloat("_CloudDensity", L1CloudDensOffset);
                    CloudDomeL1Mat.SetFloat("_CloudSize", L1CloudSize);
                    CloudDomeL1Mat.SetFloat("_AmbientFactor", L1Ambience);

                    //CloudDomeL1Mat.SetVector("_CloudSpeed",new Vector4(0, Mathf.Lerp(CloudDomeL1Mat.GetVector("_CloudSpeed").y, -windZone.transform.forward.z*windZone.windMain/50,Time.deltaTime*500),0,0));
                    if (windZone != null) {

                        //v3.1
                        Vector3 Flatten_wind = new Vector3(windZone.transform.forward.x, 0, windZone.transform.forward.z);

                        CloudDomeL1.transform.forward = Vector3.Lerp(CloudDomeL1.transform.forward, Flatten_wind, Time.deltaTime * 0.06f);
                        CloudDomeL2.transform.forward = Vector3.Lerp(CloudDomeL2.transform.forward, Flatten_wind, Time.deltaTime * 0.06f);//v3.4.3

                        //CloudDomeL1.transform.forward = Vector3.Lerp(CloudDomeL1.transform.forward, windZone.transform.forward,Time.deltaTime*0.06f);

                        //CloudDomeL1Mat.SetVector("_CloudSpeed",new Vector4(-windZone.transform.forward.x, -windZone.transform.forward.z,0,0)); //L12SpeedOffset
                        CloudDomeL1Mat.SetVector("_CloudSpeed", Vector4.Lerp(CloudDomeL1Mat.GetVector("_CloudSpeed"), new Vector4(0, -(windZone.windMain + 5.05f) * L12SpeedOffset * 0.4f * 0.01f, 0, 0), Time.deltaTime * 0.06f)); //v3.4.3
                        CloudDomeL2Mat.SetVector("_CloudSpeed", Vector4.Lerp(CloudDomeL2Mat.GetVector("_CloudSpeed"), new Vector4(0, -(windZone.windMain + 5.05f) * L12SpeedOffset * 0.1f * 0.01f, 0, 0), Time.deltaTime * 0.06f)); //v3.4.3
                    }
                }

                Color StarDomeColor = Color.white;
                if (star_dome_Material) {
                    StarDomeColor = star_dome_Material.GetVector("_TintColor");
                }

                Color RealCloud1 = Color.white;
                Color RealCloud2 = Color.white;

                if (Mobile) {
                    //change particle color transparency instead
                    //Real_Clouds_Dn_P
                } else {
                    if (real_cloud_downMaterial)
                    {
                        RealCloud1 = real_cloud_downMaterial.GetVector("_TintColor");
                    }
                    if (real_cloud_upMaterial)
                    {
                        RealCloud2 = real_cloud_upMaterial.GetVector("_TintColor");
                    }
                }

                float WeatherL12Speed = 0.1f;


                //v3.3 - Lat-Lon Handling
                //v3.0 - presets 0 and 7-11 if(){
                //bool is_DayLight  = (AutoSunPosition && Rot_Sun_X > 0 ) | (!AutoSunPosition && Current_Time >= ( 9.0f + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn));
                bool is_DayLight = (AutoSunPosition && Rot_Sun_X > 0) | (!AutoSunPosition && Current_Time >= (9.0f + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn));
                //bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 ) | (!AutoSunPosition && Current_Time >  (22.1f + Shift_dawn));
                //		bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 ) | (!AutoSunPosition && Current_Time >  (NightTimeMax-0.3f + Shift_dawn));
                //		bool is_after_17  = (AutoSunPosition && Rot_Sun_X > 65) | (!AutoSunPosition && Current_Time >  (17.1f + Shift_dawn));

                bool is_after_22 = (AutoSunPosition && Rot_Sun_X < 5 && Previous_Rot_X > Rot_Sun_X) | (!AutoSunPosition && Current_Time > (NightTimeMax - 0.3f + Shift_dawn) | (Current_Time < (9.0f + Shift_dawn)));//v3.3
                bool is_after_17 = (AutoSunPosition && Rot_Sun_X < 45 && Previous_Rot_X > Rot_Sun_X) | (!AutoSunPosition && Current_Time > (17.1f + Shift_dawn));

                //				bool is_before_10 = (AutoSunPosition && Rot_Sun_X < 10) | (!AutoSunPosition && Current_Time <  (10.0f + Shift_dawn));
                //				bool is_before_11 = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time <  (11.0f + Shift_dawn));
                //				bool is_before_16 = (AutoSunPosition && Rot_Sun_X < 60) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn));
                //				bool is_before_85 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time <  ( 8.5f + Shift_dawn));
                //				bool is_after_23  = (AutoSunPosition && Rot_Sun_X <  3) | (!AutoSunPosition && Current_Time >  (23.0f + Shift_dawn));
                //				bool is_after_224 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time >  (NightTimeMax + Shift_dawn));
                //				bool is_duskNight = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time >  (19.0f + Shift_dawn) & Current_Time < (23 + Shift_dawn));
                //				bool is_dayToDusk = (AutoSunPosition && Rot_Sun_X > 15 && Rot_Sun_X < 45) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn) & Current_Time > (11f + Shift_dawn));



                //DAY TIME CLOUDS
                //	if(Current_Time < 16 & Current_Time > 9 ){
                if (is_DayLight)
                {

                    if (AutoSunPosition && is_after_17)
                    {//if(Current_Time > 17){ //BREAK stars near morning //v3.3
                     //star_dome_Material.SetVector("_TintColor", Color.Lerp(StarDomeColor,Night_Color,1.0f*Time.deltaTime));

                        if (cloud_downMaterial)
                        {
                            cloud_downMaterial.SetVector("_TintColor", Color.Lerp(Cloud1, Dusk_cloud_color, 0.5f * Time.deltaTime));
                        }
                        if (cloud_upMaterial)
                        {
                            cloud_upMaterial.SetVector("_TintColor", Color.Lerp(Cloud2, Dusk_cloud_color, 0.5f * Time.deltaTime));
                        }

                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, new Color(0.05f, 0.02f, 0.01f, 0.5f), 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, new Color(0.05f, 0.02f, 0.01f, 0.5f), 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Dusk_cloud_color, 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Dusk_cloud_color, 0.5f * Time.deltaTime));
                            }
                        }

                        if (!Mobile)
                        {
                            if (real_cloud_downMaterial)
                            {
                                real_cloud_downMaterial.SetVector("_TintColor", Color.Lerp(RealCloud1, Dusk_real_cloud_col_dn, 0.5f * Time.deltaTime));
                            }
                            if (real_cloud_upMaterial)
                            {
                                real_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(RealCloud2, Dusk_real_cloud_col_up, 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (Application.isPlaying)
                            {
                                if (Real_Clouds_Dn_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Dn_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Dn_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, Dusk_real_cloud_col_dn, 0.5f * Time.deltaTime);
                                    }
                                }
                                if (Real_Clouds_Up_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Up_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Up_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, Dusk_real_cloud_col_up, 0.5f * Time.deltaTime);
                                    }
                                }
                            }
                        }

                        float Trans_dome_color = 0.5f;
                        if (Weather == Weather_types.Sunny | Weather == Weather_types.HeavyFog)
                        {
                            if (On_demand)
                            {
                                Trans_dome_color = 0;
                            }
                        }
                        float Color_devider2 = 1;
                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (On_demand)
                            {
                                Color_devider2 = 0.1f;
                            }
                        }

                        //v2.2 - Shader based cloud dome control
                        if (CloudDomeL1Mat != null & CloudDomeL2Mat != null)
                        {


                            if (currentWeatherName == Volume_Weather_types.HeavyStorm)
                            {

                                Color Dark_storm_L1C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                Color Dark_storm_L1CA = new Color(98f / 255f, 98f / 255f, 98f / 255f, 234f / 255f);

                                Color Dark_storm_L2C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                //Color Dark_storm_L2CA = new Color(155f/255f,150f/255f,161f/255f,254f/255f);

                                CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dark_storm_L1C.r, Dark_storm_L1C.g, Dark_storm_L1C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dark_storm_L2C.r, Dark_storm_L2C.g, Dark_storm_L2C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1.09f + L1CloudCoverOffset, WeatherL12Speed * 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1.08f, WeatherL12Speed * 0.5f * Time.deltaTime));

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(Dark_storm_L1CA.r, Dark_storm_L1CA.g, Dark_storm_L1CA.b, Trans_dome_color * 0.55f), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(Dark_storm_L2CA.r, Dark_storm_L2CA.g, Dark_storm_L2CA.b, Dark_storm_L2CA.a), WeatherL12Speed * 0.55f * Time.deltaTime));

                            }
                            else
                            {

                                if (Season == 0 | Season == 1)
                                {
                                    //CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),0.5f*Time.deltaTime));
                                    //CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),0.5f*Time.deltaTime));
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dusk_L1_dome_color.r, Dusk_L1_dome_color.g, Dusk_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dusk_L2_dome_color.r, Dusk_L2_dome_color.g, Dusk_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));

                                    if (currentWeatherName == Volume_Weather_types.Sunny)
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0, 0.5f * Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 0.5f * Time.deltaTime));
                                    }
                                    else
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.82f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                                        //CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2,1,0.5f*Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 0.5f * Time.deltaTime));
                                    }
                                }
                                else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                }
                                else
                                {
                                    //CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),2*0.5f*Time.deltaTime));
                                    //CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),2*0.5f*Time.deltaTime));
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dusk_L1_dome_color.r, Dusk_L1_dome_color.g, Dusk_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dusk_L2_dome_color.r, Dusk_L2_dome_color.g, Dusk_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));

                                    if (currentWeatherName == Volume_Weather_types.Sunny)
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0, 0.5f * Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 0.5f * Time.deltaTime));
                                    }
                                    else
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1, 0.5f * Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1, 0.5f * Time.deltaTime));
                                    }
                                }

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(0.75f * Day_L1_dome_color.r, 0.65f * Day_L1_dome_color.g, 0.75f * Day_L1_dome_color.b, Trans_dome_color * 0.55f), 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(0.75f * Day_L2_dome_color.r, 0.65f * Day_L2_dome_color.g, 0.75f * Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                            }
                        }

                        if (Season == 0 | Season == 1)
                        {
                            if (cloud_dome_downMaterial)
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                            }
                        }
                        else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                        {
                            if ((Weather == Weather_types.Sunny & On_demand))
                            {
                                if (cloud_dome_downMaterial)
                                {
                                    cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));

                                }
                            }
                            else
                            {
                                if (cloud_dome_downMaterial)
                                {
                                    cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                }
                            }
                        }
                        else
                        {
                            if (cloud_dome_downMaterial)
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 2 * 0.5f * Time.deltaTime));
                            }
                        }
                        Surround_Clouds_Mat.color = Color.Lerp(Surround_Clouds_Mat.color, Dusk_surround_cloud_col, 0.5f * Time.deltaTime);

                        star_dome_Material.SetVector("_TintColor", Color.Lerp(StarDomeColor, Night_black_Color, 0.5f * Time.deltaTime));
                    }
                    else
                    {
                        if (cloud_downMaterial)
                        {
                            cloud_downMaterial.SetVector("_TintColor", Color.Lerp(Cloud1, Day_Color, 0.5f * Time.deltaTime));
                        }
                        if (cloud_upMaterial)
                        {
                            cloud_upMaterial.SetVector("_TintColor", Color.Lerp(Cloud2, Day_Color, 0.5f * Time.deltaTime));
                        }
                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Storm_cloud_Color, 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Storm_cloud_Color, 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Day_Color, 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Day_Color, 0.5f * Time.deltaTime));
                            }
                        }

                        if (!Mobile)
                        {
                            if (real_cloud_downMaterial)
                            {
                                real_cloud_downMaterial.SetVector("_TintColor", Color.Lerp(RealCloud1, new Color(1, 1f, 1f, 0.05f), 0.5f * Time.deltaTime));
                            }
                            if (real_cloud_upMaterial)
                            {
                                real_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(RealCloud2, new Color(1, 1f, 1f, 0.05f), 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (Application.isPlaying)
                            {
                                if (Real_Clouds_Dn_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Dn_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Dn_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, new Color(1, 1f, 1f, 0.05f), 0.5f * Time.deltaTime);
                                    }
                                }
                                if (Real_Clouds_Up_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Up_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Up_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, new Color(1, 1f, 1f, 0.05f), 0.5f * Time.deltaTime);
                                    }
                                }
                            }
                        }

                        //cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDome,Day_Color,0.5f*Time.deltaTime));
                        float Trans_dome_color = 0.8f;
                        if (Weather == Weather_types.Sunny | Weather == Weather_types.HeavyFog)
                        {
                            if (On_demand)
                            {
                                Trans_dome_color = 0;
                            }
                        }
                        float Color_devider2 = 1;
                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (On_demand)
                            {
                                Color_devider2 = 0.1f;
                            }
                        }

                        //v2.2 - Shader based cloud dome control - DAY
                        if (CloudDomeL1Mat != null & CloudDomeL2Mat != null)
                        {


                            if (currentWeatherName == Volume_Weather_types.HeavyStorm)
                            {

                                Color Dark_storm_L1C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                Color Dark_storm_L1CA = new Color(98f / 255f, 98f / 255f, 98f / 255f, 234f / 255f);

                                Color Dark_storm_L2C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                //Color Dark_storm_L2CA = new Color(155f/255f,150f/255f,161f/255f,254f/255f);

                                CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dark_storm_L1C.r, Dark_storm_L1C.g, Dark_storm_L1C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dark_storm_L2C.r, Dark_storm_L2C.g, Dark_storm_L2C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1.09f + L1CloudCoverOffset, WeatherL12Speed * 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1.08f, WeatherL12Speed * 0.5f * Time.deltaTime));

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(Dark_storm_L1CA.r / 1.1f, Dark_storm_L1CA.g / 1.1f, Dark_storm_L1CA.b / 1.1f, Trans_dome_color * 0.55f), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(Dark_storm_L2CA.r, Dark_storm_L2CA.g, Dark_storm_L2CA.b, Dark_storm_L2CA.a), WeatherL12Speed * 0.55f * Time.deltaTime));

                            }
                            else
                            {


                                if (Season == 0 | Season == 1)
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Day_L1_dome_color.r, Day_L1_dome_color.g, Day_L1_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Day_L2_dome_color.r, Day_L2_dome_color.g, Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                                    CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.8f + L1CloudCoverOffset, 0.15f * Time.deltaTime));
                                    CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0.693f, 0.15f * Time.deltaTime));
                                }
                                else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                }
                                else
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Day_L1_dome_color.r, Day_L1_dome_color.g, Day_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Day_L2_dome_color.r, Day_L2_dome_color.g, Day_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.8f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0.693f, 0.5f * Time.deltaTime));
                                }

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(0.89f * Day_L1_dome_color.r, 0.89f * Day_L1_dome_color.g, 0.89f * Day_L1_dome_color.b, Trans_dome_color * 0.55f), 0.15f * Time.deltaTime));//0.79
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(0.85f * Day_L2_dome_color.r, 0.85f * Day_L2_dome_color.g, 0.85f * Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                            }
                        }


                        if (Season == 0 | Season == 1)
                        {
                            if (cloud_dome_downMaterial)
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                            }
                        }
                        else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                        {
                            if (cloud_dome_downMaterial)
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (cloud_dome_downMaterial)
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 2 * 0.5f * Time.deltaTime));
                            }
                        }

                        if (Surround_Clouds_Mat)
                        {
                            Surround_Clouds_Mat.color = Color.Lerp(Surround_Clouds_Mat.color, Day_surround_cloud_col, 0.5f * Time.deltaTime);
                        }

                        //				if(is_after_17){//if(Current_Time > 17){ //BREAK stars near morning //v3.3
                        //						//star_dome_Material.SetVector("_TintColor", Color.Lerp(StarDomeColor,Night_Color,1.0f*Time.deltaTime));
                        //				}else{

                        if (star_dome_Material) {
                            star_dome_Material.SetVector("_TintColor", Color.Lerp(StarDomeColor, Night_black_Color, 1.5f * Time.deltaTime));
                        }
                    }

                }
                else if (is_after_22)
                { //if(Current_Time <=9 & Current_Time >=0){ //v3.3

                    if (cloud_downMaterial)
                    {
                        cloud_downMaterial.SetVector("_TintColor", Color.Lerp(Cloud1, Night_Color, 2.9f * Time.deltaTime));
                    }
                    if (cloud_upMaterial)
                    {
                        cloud_upMaterial.SetVector("_TintColor", Color.Lerp(Cloud2, Night_Color, 2.9f * Time.deltaTime));
                    }

                    if (Weather == Weather_types.HeavyStormDark)
                    {
                        if (flat_cloud_downMaterial)
                        {
                            flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Night_lum_Color, 0.5f * Time.deltaTime));
                        }
                        if (flat_cloud_upMaterial)
                        {
                            flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Night_lum_Color, 0.5f * Time.deltaTime));
                        }
                    }
                    else
                    {
                        if (flat_cloud_downMaterial)
                        {
                            flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Night_Color, 0.5f * Time.deltaTime));
                        }
                        if (flat_cloud_upMaterial)
                        {
                            flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Night_Color, 0.5f * Time.deltaTime));
                        }
                    }

                    if (!Mobile)
                    {
                        if (real_cloud_downMaterial)
                        {
                            real_cloud_downMaterial.SetVector("_TintColor", Color.Lerp(RealCloud1, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime));
                        }
                        if (real_cloud_upMaterial)
                        {
                            real_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(RealCloud2, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime));
                        }
                    } else {
                        if (Application.isPlaying) {
                            if (Real_Clouds_Dn_P != null) {
                                for (int n = 0; n < Real_Clouds_Dn_P.Length; n++) {
                                    ParticleSystem.MainModule MainMod = Real_Clouds_Dn_P[n].main; //v3.4.9
                                    MainMod.startColor = Color.Lerp(MainMod.startColor.color, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime);
                                }
                            }
                            if (Real_Clouds_Up_P != null) {
                                for (int n = 0; n < Real_Clouds_Up_P.Length; n++) {
                                    ParticleSystem.MainModule MainMod = Real_Clouds_Up_P[n].main; //v3.4.9
                                    MainMod.startColor = Color.Lerp(MainMod.startColor.color, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime);
                                }
                            }
                        }
                    }

                    float Trans_dome_color = 0.1f;
                    if (Weather == Weather_types.Sunny | Weather == Weather_types.HeavyFog) {
                        if (On_demand) {
                            Trans_dome_color = 0;
                        }
                    }
                    float Color_devider2 = 1;
                    if (Weather == Weather_types.HeavyStormDark) {
                        if (On_demand) {
                            Color_devider2 = 0.1f;
                        }
                    }

                    //v2.2 - Shader based cloud dome control - DAY
                    if (CloudDomeL1Mat != null & CloudDomeL2Mat != null) {


                        if (currentWeatherName == Volume_Weather_types.HeavyStorm) {

                            Color Dark_storm_L1C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                            Color Dark_storm_L1CA = new Color(98f / 255f, 98f / 255f, 98f / 255f, 234f / 255f);

                            Color Dark_storm_L2C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                            //Color Dark_storm_L2CA = new Color(155f/255f,150f/255f,161f/255f,254f/255f);

                            CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dark_storm_L1C.r, Dark_storm_L1C.g, Dark_storm_L1C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                            CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dark_storm_L2C.r, Dark_storm_L2C.g, Dark_storm_L2C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                            CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1.09f + L1CloudCoverOffset, WeatherL12Speed * 0.5f * Time.deltaTime));
                            CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1.08f, WeatherL12Speed * 0.5f * Time.deltaTime));

                            CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(Dark_storm_L1CA.r, Dark_storm_L1CA.g, Dark_storm_L1CA.b, Trans_dome_color * 0.55f), WeatherL12Speed * 0.15f * Time.deltaTime));
                            CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(Dark_storm_L2CA.r, Dark_storm_L2CA.g, Dark_storm_L2CA.b, Dark_storm_L2CA.a), WeatherL12Speed * 0.55f * Time.deltaTime));

                        } else {

                            if (Season == 0 | Season == 1) {
                                CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Night_L1_dome_color.r, Night_L1_dome_color.g, Night_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Night_L2_dome_color.r, Night_L2_dome_color.g, Night_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.8f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0.6f, 0.5f * Time.deltaTime));
                            } else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand)) {
                                CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                            } else {
                                CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Night_L1_dome_color.r, Night_L1_dome_color.g, Night_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Night_L2_dome_color.r, Night_L2_dome_color.g, Night_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.8f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0.6f, 0.5f * Time.deltaTime));
                            }
                            //Debug.Log ("aaa");
                            CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(0.35f * Day_L1_dome_color.r, 0.35f * Day_L1_dome_color.g, 0.35f * Day_L1_dome_color.b, Trans_dome_color * 0.55f), 0.15f * Time.deltaTime));
                            CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(0.35f * Day_L2_dome_color.r, 0.35f * Day_L2_dome_color.g, 0.35f * Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                        }
                    }
                    if (cloud_dome_downMaterial)
                    {
                        if (Season == 0 | Season == 1)
                        {
                            cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(0, 0, 0, Trans_dome_color), 2 * 0.5f * Time.deltaTime));
                        }
                        else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                        {
                            cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(0, 0f, 0f, 0), 0.5f * Time.deltaTime));
                        }
                        else
                        {
                            cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(0, 0, 0, Trans_dome_color), 2 * 0.5f * Time.deltaTime));
                        }
                    }
                    if (Surround_Clouds_Mat)
                    {
                        Surround_Clouds_Mat.color = Color.Lerp(Surround_Clouds_Mat.color, Night_surround_cloud_col, 0.5f * Time.deltaTime);
                    }
                    if (!AutoSunPosition) {
                        if (star_dome_Material)
                        {
                            if (Current_Time > 10.5)
                            { //BREAK stars near morning
                                star_dome_Material.SetVector("_TintColor", Color.Lerp(StarDomeColor, Night_Color, 1.0f * Time.deltaTime));
                            }
                            else
                            {
                                star_dome_Material.SetVector("_TintColor", Color.Lerp(StarDomeColor, Day_Color, 0.02f * Time.deltaTime));
                            }
                        }
                    } else {
                        //Debug.Log ("a");
                    }

                } else {    //17 to 24	

                    if (!AutoSunPosition)
                    {
                        //Debug.Log ("b");

                        if (cloud_downMaterial)
                        {
                            cloud_downMaterial.SetVector("_TintColor", Color.Lerp(Cloud1, Dusk_cloud_color, 0.5f * Time.deltaTime));
                        }
                        if (cloud_upMaterial)
                        {
                            cloud_upMaterial.SetVector("_TintColor", Color.Lerp(Cloud2, Dusk_cloud_color, 0.5f * Time.deltaTime));
                        }
                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, new Color(0.05f, 0.02f, 0.01f, 0.5f), 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, new Color(0.05f, 0.02f, 0.01f, 0.5f), 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Dusk_cloud_color, 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Dusk_cloud_color, 0.5f * Time.deltaTime));
                            }
                        }

                        if (!Mobile)
                        {

                            if (real_cloud_downMaterial)
                            {
                                real_cloud_downMaterial.SetVector("_TintColor", Color.Lerp(RealCloud1, Dusk_real_cloud_col_dn, 0.5f * Time.deltaTime));
                            }
                            if (real_cloud_upMaterial)
                            {
                                real_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(RealCloud2, Dusk_real_cloud_col_up, 0.5f * Time.deltaTime));
                            }

                        }
                        else
                        {
                            if (Application.isPlaying)
                            {
                                if (Real_Clouds_Dn_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Dn_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Dn_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, Dusk_real_cloud_col_dn, 0.5f * Time.deltaTime);
                                    }
                                }
                                if (Real_Clouds_Up_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Up_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Up_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, Dusk_real_cloud_col_up, 0.5f * Time.deltaTime);
                                    }
                                }
                            }
                        }

                        float Trans_dome_color = 0.5f;
                        if (Weather == Weather_types.Sunny | Weather == Weather_types.HeavyFog)
                        {
                            if (On_demand)
                            {
                                Trans_dome_color = 0;
                            }
                        }
                        float Color_devider2 = 1;
                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (On_demand)
                            {
                                Color_devider2 = 0.1f;
                            }
                        }

                        //v2.2 - Shader based cloud dome control
                        if (CloudDomeL1Mat != null & CloudDomeL2Mat != null)
                        {


                            if (currentWeatherName == Volume_Weather_types.HeavyStorm)
                            {

                                Color Dark_storm_L1C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                Color Dark_storm_L1CA = new Color(98f / 255f, 98f / 255f, 98f / 255f, 234f / 255f);

                                Color Dark_storm_L2C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                //Color Dark_storm_L2CA = new Color(155f/255f,150f/255f,161f/255f,254f/255f);

                                CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dark_storm_L1C.r, Dark_storm_L1C.g, Dark_storm_L1C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dark_storm_L2C.r, Dark_storm_L2C.g, Dark_storm_L2C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1.09f + L1CloudCoverOffset, WeatherL12Speed * 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1.08f, WeatherL12Speed * 0.5f * Time.deltaTime));

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(Dark_storm_L1CA.r, Dark_storm_L1CA.g, Dark_storm_L1CA.b, Trans_dome_color * 0.55f), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(Dark_storm_L2CA.r, Dark_storm_L2CA.g, Dark_storm_L2CA.b, Dark_storm_L2CA.a), WeatherL12Speed * 0.55f * Time.deltaTime));

                            }
                            else
                            {

                                if (Season == 0 | Season == 1)
                                {
                                    //CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),0.5f*Time.deltaTime));
                                    //CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),0.5f*Time.deltaTime));
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dusk_L1_dome_color.r, Dusk_L1_dome_color.g, Dusk_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dusk_L2_dome_color.r, Dusk_L2_dome_color.g, Dusk_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));

                                    if (currentWeatherName == Volume_Weather_types.Sunny)
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0, 0.5f * Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 0.5f * Time.deltaTime));
                                    }
                                    else
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.82f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                                        //CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2,1,0.5f*Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 0.5f * Time.deltaTime));
                                    }
                                }
                                else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                }
                                else
                                {
                                    //CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),2*0.5f*Time.deltaTime));
                                    //CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2,Color_devider2*new Color(Dusk_cloud_dome_color.r,Dusk_cloud_dome_color.g,Dusk_cloud_dome_color.b,Trans_dome_color),2*0.5f*Time.deltaTime));
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dusk_L1_dome_color.r, Dusk_L1_dome_color.g, Dusk_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dusk_L2_dome_color.r, Dusk_L2_dome_color.g, Dusk_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));

                                    if (currentWeatherName == Volume_Weather_types.Sunny)
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0, 0.5f * Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0, 0.5f * Time.deltaTime));
                                    }
                                    else
                                    {
                                        CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1, 0.5f * Time.deltaTime));
                                        CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1, 0.5f * Time.deltaTime));
                                    }
                                }

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(0.75f * Day_L1_dome_color.r, 0.65f * Day_L1_dome_color.g, 0.75f * Day_L1_dome_color.b, Trans_dome_color * 0.55f), 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(0.75f * Day_L2_dome_color.r, 0.65f * Day_L2_dome_color.g, 0.75f * Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                            }
                        }

                        if (cloud_dome_downMaterial)
                        {
                            if (Season == 0 | Season == 1)
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                            }
                            else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                            {
                                if ((Weather == Weather_types.Sunny & On_demand))
                                {
                                    cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                }
                                else
                                {
                                    cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                }
                            }
                            else
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(Dusk_cloud_dome_color.r, Dusk_cloud_dome_color.g, Dusk_cloud_dome_color.b, Trans_dome_color), 2 * 0.5f * Time.deltaTime));
                            }
                        }
                        if (Surround_Clouds_Mat)
                        {
                            Surround_Clouds_Mat.color = Color.Lerp(Surround_Clouds_Mat.color, Dusk_surround_cloud_col, 0.5f * Time.deltaTime);
                        }
                        if (star_dome_Material)
                        {
                            star_dome_Material.SetVector("_TintColor", Color.Lerp(StarDomeColor, Night_black_Color, 0.5f * Time.deltaTime));
                        }
                    }
                    else
                    {
                        //do the after22

                        if (cloud_downMaterial)
                        {
                            cloud_downMaterial.SetVector("_TintColor", Color.Lerp(Cloud1, Night_Color, 2.9f * Time.deltaTime));
                        }
                        if (cloud_upMaterial)
                        {
                            cloud_upMaterial.SetVector("_TintColor", Color.Lerp(Cloud2, Night_Color, 2.9f * Time.deltaTime));
                        }

                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Night_lum_Color, 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Night_lum_Color, 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (flat_cloud_downMaterial)
                            {
                                flat_cloud_downMaterial.SetVector("_EmisColor", Color.Lerp(FlatCloud1, Night_Color, 0.5f * Time.deltaTime));
                            }
                            if (flat_cloud_upMaterial)
                            {
                                flat_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(FlatCloud2, Night_Color, 0.5f * Time.deltaTime));
                            }
                        }

                        if (!Mobile)
                        {
                            if (real_cloud_downMaterial)
                            {
                                real_cloud_downMaterial.SetVector("_TintColor", Color.Lerp(RealCloud1, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime));
                            }
                            if (real_cloud_upMaterial)
                            {
                                real_cloud_upMaterial.SetVector("_TintColor", Color.Lerp(RealCloud2, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime));
                            }
                        }
                        else
                        {
                            if (Application.isPlaying)
                            {
                                if (Real_Clouds_Dn_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Dn_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Dn_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime);
                                    }
                                }
                                if (Real_Clouds_Up_P != null)
                                {
                                    for (int n = 0; n < Real_Clouds_Up_P.Length; n++)
                                    {
                                        ParticleSystem.MainModule MainMod = Real_Clouds_Up_P[n].main; //v3.4.9
                                        MainMod.startColor = Color.Lerp(MainMod.startColor.color, new Color(0, 0.0f, 0.0f, 0.05f), 0.5f * Time.deltaTime);
                                    }
                                }
                            }
                        }

                        float Trans_dome_color = 0.1f;
                        if (Weather == Weather_types.Sunny | Weather == Weather_types.HeavyFog)
                        {
                            if (On_demand)
                            {
                                Trans_dome_color = 0;
                            }
                        }
                        float Color_devider2 = 1;
                        if (Weather == Weather_types.HeavyStormDark)
                        {
                            if (On_demand)
                            {
                                Color_devider2 = 0.1f;
                            }
                        }

                        //v2.2 - Shader based cloud dome control - DAY
                        if (CloudDomeL1Mat != null & CloudDomeL2Mat != null)
                        {


                            if (currentWeatherName == Volume_Weather_types.HeavyStorm)
                            {

                                Color Dark_storm_L1C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                Color Dark_storm_L1CA = new Color(98f / 255f, 98f / 255f, 98f / 255f, 234f / 255f);

                                Color Dark_storm_L2C = new Color(78f / 255f, 78f / 255f, 78f / 255f, 204f / 255f);
                                //Color Dark_storm_L2CA = new Color(155f/255f,150f/255f,161f/255f,254f/255f);

                                CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Dark_storm_L1C.r, Dark_storm_L1C.g, Dark_storm_L1C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Dark_storm_L2C.r, Dark_storm_L2C.g, Dark_storm_L2C.b, Trans_dome_color), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 1.09f + L1CloudCoverOffset, WeatherL12Speed * 0.5f * Time.deltaTime));
                                CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 1.08f, WeatherL12Speed * 0.5f * Time.deltaTime));

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(Dark_storm_L1CA.r, Dark_storm_L1CA.g, Dark_storm_L1CA.b, Trans_dome_color * 0.55f), WeatherL12Speed * 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(Dark_storm_L2CA.r, Dark_storm_L2CA.g, Dark_storm_L2CA.b, Dark_storm_L2CA.a), WeatherL12Speed * 0.55f * Time.deltaTime));

                            }
                            else
                            {

                                if (Season == 0 | Season == 1)
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Night_L1_dome_color.r, Night_L1_dome_color.g, Night_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Night_L2_dome_color.r, Night_L2_dome_color.g, Night_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.8f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0.6f, 0.5f * Time.deltaTime));
                                }
                                else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(1, 1f, 1f, 0), 0.5f * Time.deltaTime));
                                }
                                else
                                {
                                    CloudDomeL1Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC1, Color_devider2 * new Color(Night_L1_dome_color.r, Night_L1_dome_color.g, Night_L1_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetVector("_Color", Color.Lerp(ShaderCloudDomeC2, Color_devider2 * new Color(Night_L2_dome_color.r, Night_L2_dome_color.g, Night_L2_dome_color.b, Trans_dome_color), 0.5f * Time.deltaTime));
                                    CloudDomeL1Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage1, 0.8f + L1CloudCoverOffset, 0.5f * Time.deltaTime));
                                    CloudDomeL2Mat.SetFloat("_CloudCover", Mathf.Lerp(ShaderCloudDomeCoverage2, 0.6f, 0.5f * Time.deltaTime));
                                }

                                CloudDomeL1Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC1A, Color_devider2 * new Color(0.35f * Day_L1_dome_color.r, 0.35f * Day_L1_dome_color.g, 0.35f * Day_L1_dome_color.b, Trans_dome_color * 0.55f), 0.15f * Time.deltaTime));
                                CloudDomeL2Mat.SetVector("_Ambient", Color.Lerp(ShaderCloudDomeC2A, Color_devider2 * new Color(0.35f * Day_L2_dome_color.r, 0.35f * Day_L2_dome_color.g, 0.35f * Day_L2_dome_color.b, Trans_dome_color), 0.15f * Time.deltaTime));
                            }
                        }
                        if (cloud_dome_downMaterial)
                        {
                            if (Season == 0 | Season == 1)
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(0, 0, 0, Trans_dome_color), 2 * 0.5f * Time.deltaTime));
                            }
                            else if (Season == 2 | (Weather == Weather_types.Sunny & On_demand))
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(0, 0f, 0f, 0), 0.5f * Time.deltaTime));
                            }
                            else
                            {
                                cloud_dome_downMaterial.SetVector("_TintColor", Color.Lerp(CloudDomeR, Color_devider2 * new Color(0, 0, 0, Trans_dome_color), 2 * 0.5f * Time.deltaTime));
                            }
                        }
                        if (Surround_Clouds_Mat)
                        {
                            Surround_Clouds_Mat.color = Color.Lerp(Surround_Clouds_Mat.color, Night_surround_cloud_col, 0.5f * Time.deltaTime);
                        }
						if (!AutoSunPosition && star_dome_Material) {
							if (Current_Time > 10.5) { //BREAK stars near morning
								star_dome_Material.SetVector ("_TintColor", Color.Lerp (StarDomeColor, Night_Color, 1.0f * Time.deltaTime));
							} else {
								star_dome_Material.SetVector ("_TintColor", Color.Lerp (StarDomeColor, Day_Color, 0.02f * Time.deltaTime));
							}
						} else {
							//Debug.Log ("a");
						}
					}

			}
			//////////////////////// END HANDLE CLOUDS //////////////////


			//HANDLE SNOW STORM and HAZE	
			if(Application.isPlaying){

			///////////////////////// HANDLE SEASONS ////////////////////
			Color Tree_color = new Color(255/Color_devider,255/Color_devider,255/Color_devider,255/Color_devider);
			Color Terrain_color = new Color(0/Color_devider,0/Color_devider,0/Color_devider,255/Color_devider);
			Color Grass_color = new Color(105/Color_devider,105/Color_devider,105/Color_devider,255/Color_devider);

				/////////////////////////// CHANGE FOG PER SEASON ////////////////////
				if(!Seasonal_change_auto & !On_demand){
					if(Season == 0 | Season == 1){
							if(Use_fog){
								if(!RenderSettings.fog){
									RenderSettings.fog = true;
								}
								Color Fog_color = Spring_fog_day;
								float Fog_density = Spring_fog_day_density * Fog_Density_Mult;
								if(Current_Time < 18 & Current_Time > 9 ){
									 Fog_color = Spring_fog_day;
									Fog_density = Spring_fog_day_density * Fog_Density_Mult;									
								}else if(Current_Time <=9 & Current_Time > 1){
									 Fog_color = Spring_fog_night;
									Fog_density = Spring_fog_night_density * Fog_Density_Mult;
								}else{
									 Fog_color = Spring_fog_dusk;
									Fog_density = Spring_fog_dusk_density * Fog_Density_Mult;
								}
								FogMode Fog_Mode = fogMode;//FogMode.ExponentialSquared;
                                float Density_Speed = Fog_Density_Speed;
								//Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
								Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,Density_Speed,0);
							}else{
								if(RenderSettings.fog){
									RenderSettings.fog = false;
								}
							}
					}else
					if(Season == 2){
							if(Use_fog){
								if(!RenderSettings.fog){
									RenderSettings.fog = true;
								}
								Color Fog_color = Summer_fog_day;
								float Fog_density = Summer_fog_day_density * Fog_Density_Mult;	
								if(Current_Time < 18 & Current_Time > 9 ){
									 Fog_color = Summer_fog_day;
									Fog_density = Summer_fog_day_density * Fog_Density_Mult;									
								}else if(Current_Time <=9 & Current_Time > 1){
									 Fog_color = Summer_fog_night;
									Fog_density = Summer_fog_night_density * Fog_Density_Mult;
								}else{
									 Fog_color = Summer_fog_dusk;
									Fog_density = Summer_fog_dusk_density * Fog_Density_Mult;
								}
								FogMode Fog_Mode  = fogMode;//FogMode.ExponentialSquared;
                                float Density_Speed = Fog_Density_Speed;
								//Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
								Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,Density_Speed,0);

							}else{
								if(RenderSettings.fog){
									RenderSettings.fog = false;
								}
							}
					}else
					if(Season == 3){
						if(Use_fog){
							if(!RenderSettings.fog){
								RenderSettings.fog = true;
							}
								Color Fog_color = Autumn_fog_day;
								float Fog_density = Autumn_fog_day_density * Fog_Density_Mult;	
								if(Current_Time < 18 & Current_Time > 9 ){
									 Fog_color = Autumn_fog_day;
									Fog_density = Autumn_fog_day_density * Fog_Density_Mult;									
								}else if(Current_Time <=9 & Current_Time > 1){
									 Fog_color = Autumn_fog_night;
									Fog_density = Autumn_fog_night_density * Fog_Density_Mult;
								}else{
									 Fog_color = Autumn_fog_dusk;
									Fog_density = Autumn_fog_dusk_density * Fog_Density_Mult;
								}
								FogMode Fog_Mode  = fogMode;//FogMode.ExponentialSquared;
                                float Density_Speed = Fog_Density_Speed;
								//Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
								Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,Density_Speed,0);

						}else{
							if(RenderSettings.fog){
								RenderSettings.fog = false;
							}
						}
					}else
					if(Season ==4){
						if(!RenderSettings.fog){
							RenderSettings.fog = true;
						}
							Color Fog_color = Winter_fog_day;
							float Fog_density = Winter_fog_day_density * Fog_Density_Mult;	
							if(Current_Time < 18 & Current_Time > 9 ){
								 Fog_color = Winter_fog_day;
								Fog_density = Winter_fog_day_density * Fog_Density_Mult;									
							}else if(Current_Time <=9 & Current_Time > 1){
								 Fog_color = Winter_fog_night;
								Fog_density = Winter_fog_night_density * Fog_Density_Mult;
							}else{
								 Fog_color = Winter_fog_dusk;
								Fog_density = Winter_fog_dusk_density * Fog_Density_Mult;
							}
							FogMode Fog_Mode  = fogMode;//FogMode.ExponentialSquared;
                            float Density_Speed = Fog_Density_Speed;
							//Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,(Density_Speed*Fog_density*Fog_density)/(Mathf.Abs(Mathf.Pow(Fog_density-RenderSettings.fogDensity,2))+0.0000001f),0);
							Make_Fog_Appear(Fog_color,Fog_density,Fog_Mode,Density_Speed,0);

							//RenderSettings.fogColor = new Color(60/Color_devider,70/Color_devider,75/Color_devider,255/Color_devider);
							//RenderSettings.fogDensity = 0.01f;
						
					}else{
						if(RenderSettings.fog){
							RenderSettings.fog = false;
						}
					}
				}
			/////////////////////////// CHANGE TERRAIN PER SEASON ////////////////////
			if(Season_prev != Season | !InitSeason){

						InitSeason = true;

					//reset all extra params
					Seasonal_factor1_add = 0; //Seasonal factors, change below and assign to sky and cloud color changes
					Seasonal_factor2_add = 0;
					Seasonal_factor3_add = 0;
					Seasonal_factor4_add = 0;
					Seasonal_factor5_add = 0;
					Seasonal_factor6_add = 0;
					Seasonal_factor7_add = 0;

					//v3.0.3
					if(!Seasonal_change_auto && LegacySeasonalFX){
					//Enable falling leaves in autumn
					if(Season == 3){
						if(FallingLeaves_OBJ!=null){
								for (int n = 0; n < FallingLeaves_OBJ.Length; n++){
									ParticleSystem.MainModule MainMod = FallingLeaves_P[n].main; //v3.4.9
									FallingLeaves_OBJ[n].SetActive(true);// Later change to instantiate the effect and destroy it
									MainMod.maxParticles = 300;
								}
						}
						int RAND_CLOUD = Random.Range(1,4);
						if(RAND_CLOUD == 2){
									if(Lower_Dynamic_Cloud != null){
										Lower_Dynamic_Cloud.SetActive(true);}
									if(Upper_Dynamic_Cloud != null){
										Upper_Dynamic_Cloud.SetActive(true);}
									if(Lower_Static_Cloud != null){
										Lower_Static_Cloud.SetActive(true);}
						}else{
									if(Lower_Cloud_Real != null){
										Lower_Cloud_Real.SetActive(true);}
									if(Upper_Cloud_Real != null){
										Upper_Cloud_Real.SetActive(true);
									}
									if(Lower_Dynamic_Cloud != null){
										Lower_Dynamic_Cloud.SetActive(false);}
									if(Upper_Dynamic_Cloud != null){
										Upper_Dynamic_Cloud.SetActive(false);	}
									if(Lower_Static_Cloud != null){
										Lower_Static_Cloud.SetActive(false);
									}
						}

					}else{
								if(Lower_Cloud_Real != null){
									Lower_Cloud_Real.SetActive(false);}
								if(Upper_Cloud_Real != null){
									Upper_Cloud_Real.SetActive(false);
								}
						//FallingLeaves_OBJ.particleSystem.enableEmission
								if(FallingLeaves_OBJ!=null){
								for (int n = 0; n < FallingLeaves_OBJ.Length; n++){
						FallingLeaves_OBJ[n].SetActive(false);// Or stop particles before and re-start emit when active
								}
								}
					}

					//activate snow storm in winter
					if(Season ==4){
								if(SnowStorm_OBJ != null){
									SnowStorm_OBJ.SetActive(true);
								}
					}else{
								if(SnowStorm_OBJ != null){
									SnowStorm_OBJ.SetActive(false);
								}
					}

					//activate butterflies in spring
					if(Season == 1){
								if(Butterfly_OBJ != null){Butterfly_OBJ.SetActive(true);}
								if(Lower_Dynamic_Cloud != null){Lower_Dynamic_Cloud.SetActive(true);}
								if(Upper_Dynamic_Cloud != null){Upper_Dynamic_Cloud.SetActive(true);}
								if(Lower_Cloud_Bed != null){Lower_Cloud_Bed.SetActive(false);}
								if(Upper_Cloud_Bed != null){Upper_Cloud_Bed.SetActive(false);	}
								if(Lower_Static_Cloud != null){Lower_Static_Cloud.SetActive(false);}
								if(Upper_Static_Cloud != null){Upper_Static_Cloud.SetActive(false);}						
								if(Sun_Ray_Cloud != null){Sun_Ray_Cloud.SetActive(true);}
					}else{
						if(Butterfly_OBJ != null){
							Butterfly_OBJ.SetActive(false);
						}
					}
			}

			if(Season==0){
					//do changes based on time
							Tree_color = Tree_Spring_Col;
							Terrain_color = Terrain_Spring_Col;
							Grass_color = Grass_Spring_Col;
//					Tree_color = new Color(133/Color_devider,197/Color_devider,69/Color_devider,255/Color_devider);
//					Terrain_color = new Color(43/Color_devider,63/Color_devider,0/Color_devider,255/Color_devider);
//					Grass_color = new Color(65/Color_devider,69/Color_devider,40/Color_devider,255/Color_devider);
			}else if(Season ==1){ // Spring
							Tree_color = Tree_Spring_Col;
							Terrain_color = Terrain_Spring_Col;
							Grass_color = Grass_Spring_Col;
//					Tree_color = new Color(133/Color_devider,197/Color_devider,69/Color_devider,255/Color_devider);
//					Terrain_color = new Color(43/Color_devider,63/Color_devider,0/Color_devider,255/Color_devider);
//					Grass_color = new Color(65/Color_devider,69/Color_devider,40/Color_devider,255/Color_devider);
					//Terrain_controller.Enable_trasition = true;
			}else if(Season ==2){ // Summer
							Tree_color = Tree_Summer_Col;
							Terrain_color = Terrain_Summer_Col;
							Grass_color = Grass_Summer_Col;
//					Tree_color = new Color(255/Color_devider,176/Color_devider,9/Color_devider,255/Color_devider);
//					Terrain_color = new Color(28/Color_devider,30/Color_devider,4/Color_devider,255/Color_devider);
//					Grass_color = new Color(255/Color_devider,181/Color_devider,1/Color_devider,255/Color_devider);
					//Terrain_controller.Enable_trasition = true;
			}else if(Season ==3){ // Autumn
							Tree_color = Tree_Autumn_Col;
							Terrain_color = Terrain_Autumn_Col;
							Grass_color = Grass_Autumn_Col;
//					Tree_color = new Color(221/Color_devider,8/Color_devider,27/Color_devider,255/Color_devider);
//					Terrain_color = new Color(69/Color_devider,14/Color_devider,10/Color_devider,255/Color_devider);
//					Grass_color = new Color(161/Color_devider,14/Color_devider,1/Color_devider,255/Color_devider);
					//Terrain_controller.Enable_trasition = true;
			}else if(Season ==4){ // Winter
							Tree_color = Tree_Winter_Col;
							Terrain_color = Terrain_Winter_Col;
							Grass_color = Grass_Winter_Col;
//					Tree_color = new Color(7/Color_devider,7/Color_devider,7/Color_devider,255/Color_devider);
//					Terrain_color = new Color(173/Color_devider,173/Color_devider,173/Color_devider,255/Color_devider);
//					Grass_color = new Color(7/Color_devider,7/Color_devider,7/Color_devider,255/Color_devider);
					//Terrain_controller.Enable_trasition = true;
			}
						//v1.2.5
						if(Terrain_controller!=null){
							Terrain_controller.TreeA_color = Tree_color;
							Terrain_controller.Terrain_tint = Terrain_color;
							Terrain_controller.Grass_tint = Grass_color;
						}
						if(Mesh_Terrain_controller!=null){
							Mesh_Terrain_controller.TreeA_color = Tree_color;
							Mesh_Terrain_controller.Terrain_tint = Terrain_color;
							Mesh_Terrain_controller.Grass_tint = Grass_color;
						}

				Season_prev = Season;
			}	
			}

			//////////////////////// SET SKY MATERIAL /////////////////
			//if(Auto_Cycle_Sky && (!Application.isPlaying || (Application.isPlaying && (!LimitSunUpdateRate || (LimitSunUpdateRate && Time.fixedTime - last_sun_update > Update_sun_every) ))) ){// & Application.isPlaying){ //v3.4.5a
			if(Auto_Cycle_Sky){
					
					//last_sun_update = Time.fixedTime; //v3.4.5a

					//v3.1 - presets 0 and 7-11 if(){
//					bool is_DayLight  = (AutoSunPosition && Rot_Sun_X > 0 ) | (!AutoSunPosition && Current_Time >= ( 9.0f + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn));
//					//bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 ) | (!AutoSunPosition && Current_Time >  (22.1f + Shift_dawn));
//					bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 ) | (!AutoSunPosition && Current_Time >  (NightTimeMax-0.3f + Shift_dawn));
//					bool is_after_17  = (AutoSunPosition && Rot_Sun_X > 65) | (!AutoSunPosition && Current_Time >  (17.1f + Shift_dawn));
//					bool is_before_10 = (AutoSunPosition && Rot_Sun_X < 10) | (!AutoSunPosition && Current_Time <  (10.0f + Shift_dawn));
//					bool is_before_11 = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time <  (11.0f + Shift_dawn));
//					bool is_before_16 = (AutoSunPosition && Rot_Sun_X < 60) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn));
//					bool is_before_85 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time <  ( 8.5f + Shift_dawn));
//					bool is_after_23  = (AutoSunPosition && Rot_Sun_X <  3) | (!AutoSunPosition && Current_Time >  (23.0f + Shift_dawn));
//					bool is_after_224 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time >  (NightTimeMax + Shift_dawn));
//					bool is_duskNight = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time >  (19.0f + Shift_dawn) & Current_Time < (23 + Shift_dawn));
//					bool is_dayToDusk = (AutoSunPosition && Rot_Sun_X > 15 && Rot_Sun_X < 45) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn) & Current_Time > (11f + Shift_dawn));



					//reset all extra params
					Seasonal_factor1_add = 0; //Seasonal factors, change below and assign to sky and cloud color changes
					Seasonal_factor2_add = 0;
					Seasonal_factor3_add = 0;
					Seasonal_factor4_add = 0;
					Seasonal_factor5_add = 0;
					Seasonal_factor6_add = 0;
					Seasonal_factor7_add = 0;
					if(1==1){//v1.7
						//if(Current_Time < (18+Shift_dawn) & Current_Time > (9+Shift_dawn) ){
						//if(Current_Time < (18+Shift_dawn) & Current_Time > (9+Shift_dawn) ){

						//v3.0
						if(
							(!AutoSunPosition && ( Current_Time < (18+Shift_dawn) & Current_Time > (9+Shift_dawn) )) 
								|
							(AutoSunPosition && Rot_Sun_X > 0)
						){

//						if(Star_particles_OBJ != null){
//							if(Star_particles_OBJ.activeInHierarchy){
//								Star_particles_OBJ.SetActive(false);
//							}
//						}

							if(Star_particles_OBJ_P!=null){
								ParticleSystem.MainModule MainMod = Star_particles_OBJ_P.main; //v3.4.9
								if(MainMod.maxParticles>5){
									MainMod.maxParticles -=5;
								}else{
									if(Star_particles_OBJ_P.gameObject.activeInHierarchy){
										Star_particles_OBJ_P.gameObject.SetActive(false);
									}
									if(Star_particles_OBJ.activeInHierarchy){
										Star_particles_OBJ.SetActive(false);
									}
								}
							}

						if(Season == 4){//Winter
							Seasonal_factor1_add = -0.5f;//0.8 to 0.37
							Seasonal_factor2_add = -16f;// 26 to 10 //Esun
							Seasonal_factor3_add = -0.0004f;// 0.0004 to 0.002
							Seasonal_factor4_add = -0.014f;// 0.096 to 0.0325//Km
							Seasonal_factor5_add = 0.02f;// -0.98  to -0.96
							Seasonal_factor6_add = 0.2f;// 2.1 to 2.78 //Fsamples
							Seasonal_factor7_add = -0.3f;// 0.6 to 0.2
							m_Coloration = Mathf.Lerp(m_Coloration, 0.2f + Seasonal_factor1_add,0.5f*Time.deltaTime);
						}

							if(   (AutoSunPosition && Rot_Sun_X < 25  ) | (!AutoSunPosition &&   Current_Time > (17+Shift_dawn)    )){ //v3.0

						float UP_rate = 11f;

							if(Application.isPlaying){
									MAIN.color =  Color.Lerp (MAIN.color, Dusk_Sun_Color , 0.05f*UP_rate*Time.deltaTime*SPEED); 
							}else{
									if(MAIN.color != Dusk_Sun_Color){
										MAIN.color = Dusk_Sun_Color;
									}
							}

						//GLOBAL TINT
	//					m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , 0.05f*UP_rate*Time.deltaTime); 
						
							if(Application.isPlaying){
								RenderSettings.ambientLight = Color.Lerp (RenderSettings.ambientLight, Dusk_Ambient_Color , 0.05f*UP_rate*Time.deltaTime);//v1.2.1
							}else{
								if(RenderSettings.ambientLight != Dusk_Ambient_Color){
									RenderSettings.ambientLight =Dusk_Ambient_Color;//v1.5
								}
							}
						//fall from max
						UP_rate = 4f+2;
						UP_rate = 4.5f;

								if(!Unity5){//v1.7
							m_fExposure = Mathf.Lerp(m_fExposure, 0.8f + Seasonal_factor1_add,UP_rate*Time.deltaTime);
							m_ESun = Mathf.Lerp(m_ESun, 36.9f + Seasonal_factor2_add,UP_rate*Time.deltaTime); 			// Sun brightness 
							m_Kr = Mathf.Lerp(m_Kr, 0.0106587f + Seasonal_factor3_add,0.05f*UP_rate*Time.deltaTime); 			// Rayleigh 
							m_Km = Mathf.Lerp(m_Km, 0.09617826f + Seasonal_factor4_add,0.05f*UP_rate*Time.deltaTime);  			// Mie 
							m_g = Mathf.Lerp(m_g, -0.9802108f + Seasonal_factor5_add,UP_rate*Time.deltaTime); 
						Sun_ring_factor=0;
							m_fSamples=Mathf.Lerp(m_fSamples, 2.1f + Seasonal_factor6_add,0.05f*UP_rate*Time.deltaTime); 
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth, 0.61f + Seasonal_factor7_add,UP_rate*Time.deltaTime); 
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.7977272f,0.5472727f,0.5204545f),UP_rate*Time.deltaTime); // Wave length of light
								}
							}else if( (AutoSunPosition && Rot_Sun_X < 65  ) | (!AutoSunPosition && Current_Time > (12+Shift_dawn))){
						//raise to max
						float UP_rate = 3;
						UP_rate = 0.7f;

								if(Application.isPlaying){
									MAIN.color =  Color.Lerp (MAIN.color, Day_Sun_Color , 0.05f*UP_rate*Time.deltaTime*SPEED); 
								}else{
									if(MAIN.color != Day_Sun_Color){
										MAIN.color = Day_Sun_Color;
									}
								}

							if(Application.isPlaying){
								RenderSettings.ambientLight = Color.Lerp (RenderSettings.ambientLight, Day_Ambient_Color , 0.5f*UP_rate*Time.deltaTime);//v1.2.1
							}else{
								if(RenderSettings.ambientLight != Day_Ambient_Color){
									RenderSettings.ambientLight =Day_Ambient_Color;//v1.5
								}
							}

								if(!Unity5){//v1.7
							m_fExposure = Mathf.Lerp(m_fExposure, 2.2f + Seasonal_factor1_add,UP_rate*Time.deltaTime);
						//m_ESun = Mathf.Lerp(m_ESun, 46.19f,UP_rate*Time.deltaTime); 			// Sun brightness 

							m_ESun = Mathf.Lerp(m_ESun, 26.19f + Seasonal_factor2_add,UP_rate*Time.deltaTime); 

							m_Kr = Mathf.Lerp(m_Kr, 0.0004021739f + Seasonal_factor3_add,UP_rate*Time.deltaTime); 			// Rayleigh 
							m_Km = Mathf.Lerp(m_Km, 0.01445652f + Seasonal_factor4_add,UP_rate*Time.deltaTime);  			// Mie
							m_g = Mathf.Lerp(m_g, -0.9263152f + Seasonal_factor5_add,UP_rate*Time.deltaTime); 
						Sun_ring_factor=0;
							m_fSamples=Mathf.Lerp(m_fSamples, 2.76087f + Seasonal_factor6_add,UP_rate*Time.deltaTime); 
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth, 0.7f + Seasonal_factor7_add,UP_rate*Time.deltaTime); 
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.65f,0.57f,0.475f),UP_rate*Time.deltaTime); // Wave length of light
								}
							//GLOBAL TINT - v1.6.5
	//						m_TintColor =  Color.Lerp (m_TintColor, new Color(0f,0,0,0) , 0.55f*UP_rate*Time.deltaTime);
					}else {
						//raise to max
						float UP_rate = 3;
						
									if(Application.isPlaying){
									MAIN.color =  Color.Lerp (MAIN.color, Dawn_Sun_Color , 0.05f*UP_rate*Time.deltaTime*SPEED); 
									}else{
											if(MAIN.color != Dawn_Sun_Color){
												MAIN.color = Dawn_Sun_Color;
											}
									}

							if(Application.isPlaying){
								RenderSettings.ambientLight = Color.Lerp (RenderSettings.ambientLight, Dawn_Ambient_Color , 0.5f*UP_rate*Time.deltaTime);//v1.2.1
							}else{
								if(RenderSettings.ambientLight != Dawn_Ambient_Color){
									RenderSettings.ambientLight =Dawn_Ambient_Color;//v1.5
								}
							}
								if(!Unity5){//v1.7
							m_fExposure = Mathf.Lerp(m_fExposure, 1.163406f + Seasonal_factor1_add,UP_rate*Time.deltaTime);
							m_ESun = Mathf.Lerp(m_ESun, 52.37899f + Seasonal_factor2_add,UP_rate*Time.deltaTime); 			// Sun brightness
							m_Kr = Mathf.Lerp(m_Kr, 0.014f + Seasonal_factor3_add,UP_rate*Time.deltaTime); 			// Rayleigh 
							m_Km = Mathf.Lerp(m_Km, 0.030868f + Seasonal_factor4_add,UP_rate*Time.deltaTime);  			// Mie 
							m_g = Mathf.Lerp(m_g, -0.96f + Seasonal_factor5_add,UP_rate*Time.deltaTime); 
						Sun_ring_factor=0;
							m_fSamples=Mathf.Lerp(m_fSamples, 3f + Seasonal_factor6_add,UP_rate*Time.deltaTime); 
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth, 0.4173913f + Seasonal_factor7_add,UP_rate*Time.deltaTime); 
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.775f,0.5927f,0.5545f),UP_rate*Time.deltaTime); // light  wave length
								}
							//GLOBAL TINT - v1.6.5
	//						m_TintColor =  Color.Lerp (m_TintColor, new Color(0f,0,0,0) , 0.55f*UP_rate*Time.deltaTime);
					}	

						}else if( (AutoSunPosition && Rot_Sun_X < 15  ) | (!AutoSunPosition && Current_Time <=(9+Shift_dawn) & Current_Time > (1+Shift_dawn))){

					float UP_rate = 0.5f;

							//v2.0.1
							if((AutoSunPosition && Rot_Sun_X < 5  ) | (!AutoSunPosition &&  Current_Time <(8.7f+Shift_dawn))){
								if(Application.isPlaying){
									MAIN.color =  Color.Lerp (MAIN.color, Night_Sun_Color , 1.5f*UP_rate*Time.deltaTime*SPEED); 
								}else{
									if(MAIN.color !=  Night_Sun_Color){
										MAIN.color = Night_Sun_Color;
									}
								}
							}else{
									if(Application.isPlaying){
										//MAIN.color =  Color.Lerp (MAIN.color, new Color(208/Color_devider,208/Color_devider,208/Color_devider,255/Color_devider) , UP_rate*Time.deltaTime); 
									MAIN.color =  Color.Lerp (MAIN.color, Dawn_Sun_Color , UP_rate*Time.deltaTime*SPEED); 
									}else{
									if(MAIN.color != Dawn_Sun_Color){
										MAIN.color = Dawn_Sun_Color;
									}
								}
							}

						if(Application.isPlaying){
								RenderSettings.ambientLight = Color.Lerp (RenderSettings.ambientLight, Night_Ambient_Color , 2f*UP_rate*Time.deltaTime);//v1.2.1
						}else{
							if(RenderSettings.ambientLight != Night_Ambient_Color){
								RenderSettings.ambientLight =Night_Ambient_Color;//v1.5
							}
						}

						//GLOBAL TINT - v1.6.5
	//					m_TintColor =  Color.Lerp (m_TintColor, new Color(0f,0,0,0) , 0.55f*UP_rate*Time.deltaTime);

							if(!Unity5){//v1.7
					//if(Preset==0){
						m_fExposure = Mathf.Lerp(m_fExposure, 0.2655797f + Seasonal_factor1_add,0.5f*Time.deltaTime);
						m_ESun = Mathf.Lerp(m_ESun, 34.61015f + Seasonal_factor2_add,0.5f*Time.deltaTime); 			// 
						m_Kr = Mathf.Lerp(m_Kr, 0.0001f + Seasonal_factor3_add,0.5f*Time.deltaTime); 			// 
						m_Km = Mathf.Lerp(m_Km, 0.0003f + Seasonal_factor4_add,0.5f*Time.deltaTime);  			//
						m_g = Mathf.Lerp(m_g, -0.8993674f + Seasonal_factor5_add,0.5f*Time.deltaTime); 
					Sun_ring_factor=0.005113637f;
						m_fSamples=Mathf.Lerp(m_fSamples, 3.130435f + Seasonal_factor6_add,0.5f*Time.deltaTime); 
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth, 0.4036232f + Seasonal_factor7_add,0.5f*Time.deltaTime); 
					m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.7522727f,0.82f,0.7590909f),0.5f*Time.deltaTime); // 
					//}
							}
//						if(Star_particles_OBJ != null){
//							if(!Star_particles_OBJ.activeInHierarchy){
//								Star_particles_OBJ.SetActive(true);
//							}
//						}
							//if(Current_Time < 8){
							//if((AutoSunPosition && Rot_Sun_X < 0  ) | (!AutoSunPosition && Current_Time <(8+Shift_dawn))){//v3.0.3
							if((AutoSunPosition && Rot_Sun_X < 0  ) | (!AutoSunPosition && Current_Time <(8+Shift_dawn))){//v3.0.3
							if(Star_particles_OBJ_P!=null){
								if(!Star_particles_OBJ.activeInHierarchy){
									Star_particles_OBJ.SetActive(true);
									//Debug.Log ("1 done");
								}else{
									//Debug.Log ("1 not done");
								}
								if(!Star_particles_OBJ_P.gameObject.activeInHierarchy){
									Star_particles_OBJ_P.gameObject.SetActive(true);
									//Debug.Log ("2 done");
								}else{
									//Debug.Log ("2 not done");
								}
								ParticleSystem.MainModule MainMod = Star_particles_OBJ_P.main; //v3.4.9
								if(MainMod.maxParticles < 1000){
										MainMod.maxParticles +=5;
								}
							}else{
								//Debug.Log ("3 where");
								if(!Star_particles_OBJ.activeInHierarchy){
									Star_particles_OBJ.SetActive(true);
									//Debug.Log ("1a done");
								}
								Star_particles_OBJ_P = Star_particles_OBJ.GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
							}
							}else{
								if(Star_particles_OBJ_P!=null){
									if(Star_particles_OBJ_P.main.maxParticles>5){
										//Star_particles_OBJ_P.maxParticles -=(int)(5 * DawnAppearSpeed * 0.5f);//v3.1
										ParticleSystem.MainModule MainMod = Star_particles_OBJ_P.main; //v3.4.9
										if(Random.Range(1,27)==2){
											MainMod.maxParticles -=Mathf.Max(1,(int)(5 * DawnAppearSpeed * 0.5f));//v3.1
										}
										MainMod.startColor = Color.Lerp(MainMod.startColor.color,new Color(MainMod.startColor.color.r,MainMod.startColor.color.g,MainMod.startColor.color.b,0),Time.deltaTime*StarsPFadeSpeed* DawnAppearSpeed*SPEED);
									}else{
										if(Star_particles_OBJ_P.gameObject.activeInHierarchy){
											Star_particles_OBJ_P.gameObject.SetActive(false);
										}
										if(Star_particles_OBJ.activeInHierarchy){
											Star_particles_OBJ.SetActive(false);
										}
									}
								}
							}



			
			}else{ //18 evening up to 1 at night

							//if(Current_Time > 23 | Current_Time <=1){
							//if( (AutoSunPosition && Rot_Sun_X < 0  ) | (!AutoSunPosition &&  Current_Time > (23+Shift_dawn) | (Current_Time >= 0 & Current_Time <=(1+Shift_dawn))) ){ //v3.0.3
							if( (AutoSunPosition && Rot_Sun_X < 0  ) | (!AutoSunPosition &&  Current_Time > (NightTimeMax+Shift_dawn) | (Current_Time >= 0 & Current_Time <=(1+Shift_dawn))) ){ //v3.0.4
								if(Star_particles_OBJ_P!=null){
								if(!Star_particles_OBJ.activeInHierarchy){
									Star_particles_OBJ.SetActive(true);
								}
								if(!Star_particles_OBJ_P.gameObject.activeInHierarchy){
									Star_particles_OBJ_P.gameObject.SetActive(true);
								}

								ParticleSystem.MainModule MainMod = Star_particles_OBJ_P.main; //v3.4.9
								if(MainMod.maxParticles < 1000){
										MainMod.maxParticles +=5;
									//v3.1
										MainMod.startColor = Color.Lerp(MainMod.startColor.color,new Color(MainMod.startColor.color.r,MainMod.startColor.color.g,MainMod.startColor.color.b,StarsPMaxGlow),Time.deltaTime*0.6f*SPEED);
								}
								}else{
									if(!Star_particles_OBJ.activeInHierarchy){
										Star_particles_OBJ.SetActive(true);
										//Debug.Log ("1a done");
									}
									Star_particles_OBJ_P = Star_particles_OBJ.GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
								}
							}else{
								if(Star_particles_OBJ_P != null){
										if(Star_particles_OBJ.activeInHierarchy){
											Star_particles_OBJ.SetActive(false);
										}
									if(Star_particles_OBJ_P.gameObject.activeInHierarchy){
											Star_particles_OBJ_P.gameObject.SetActive(false);
									}
								}
							}

						if(Season == 1){//Spring
							Seasonal_factor1_add = 0f;
							Seasonal_factor2_add = -14.9f;// 36.9 to 22 //Esun
							Seasonal_factor3_add = -0.0062f;// 0.0106 to 0.0044
							Seasonal_factor4_add = -0.0259f;// 0.027 to 0.0011
							Seasonal_factor5_add = 0.02f;// -0.98  to -0.96
							Seasonal_factor6_add = 0.9f;// 2.1 to 3 //Fsamples
							Seasonal_factor7_add = -0.1f;// 0.6 to 0.5
							m_Coloration = Mathf.Lerp(m_Coloration, 0.12f + Seasonal_factor1_add,0.5f*Time.deltaTime);
						}

					float UP_rate = 0.5f;
									

					//assign settings 
							if(!Unity5){//v1.7
					if(Preset==0){//initial setting
							m_fExposure = Mathf.Lerp(m_fExposure, 0.8f + Seasonal_factor1_add,0.5f*Time.deltaTime);
							m_ESun = Mathf.Lerp(m_ESun, 36.9f + Seasonal_factor2_add,0.5f*Time.deltaTime); 			// 
							m_Kr = Mathf.Lerp(m_Kr, 0.0106587f + Seasonal_factor3_add,0.5f*Time.deltaTime); 			// 
							m_Km = Mathf.Lerp(m_Km, 0.09617826f + Seasonal_factor4_add,0.5f*Time.deltaTime);  			//
							m_g = Mathf.Lerp(m_g, -0.9802108f + Seasonal_factor5_add,0.5f*Time.deltaTime); 
						Sun_ring_factor=0;
							m_fSamples=Mathf.Lerp(m_fSamples, 2.1f + Seasonal_factor6_add,0.5f*Time.deltaTime); 
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth, 0.61f + Seasonal_factor7_add,0.5f*Time.deltaTime); 
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.7977272f,0.5472727f,0.5204545f),0.5f*Time.deltaTime); // 
					}
					else if(Preset==1){//wide scattering
							m_fExposure = Mathf.Lerp(m_fExposure,0.8213768f + Seasonal_factor1_add,0.5f*Time.deltaTime);
							m_ESun = Mathf.Lerp(m_ESun, 2*11.10942f + Seasonal_factor2_add,0.5f*Time.deltaTime); 			// 
							m_Kr = Mathf.Lerp(m_Kr, 0.01077681f + Seasonal_factor3_add,0.5f*Time.deltaTime); 			// 
							m_Km = Mathf.Lerp(m_Km, 0.02707681f + Seasonal_factor4_add,0.5f*Time.deltaTime);  			//
							m_g = Mathf.Lerp(m_g, -0.9572327f + Seasonal_factor5_add,0.5f*Time.deltaTime); 
						Sun_ring_factor=0;
							m_fSamples=Mathf.Lerp(m_fSamples, 2.695652f + Seasonal_factor6_add,0.5f*Time.deltaTime); 
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth, 0.4036232f + Seasonal_factor7_add,0.5f*Time.deltaTime); 
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(1,0.5472732f,0.5204539f),0.5f*Time.deltaTime); //
					}else{
							m_fExposure = Mathf.Lerp(m_fExposure,0.8213768f + Seasonal_factor1_add,0.5f*Time.deltaTime);
							m_ESun = Mathf.Lerp(m_ESun, 2*11.10942f + Seasonal_factor2_add,0.5f*Time.deltaTime); 			// 
							m_Kr = Mathf.Lerp(m_Kr, 0.01077681f + Seasonal_factor3_add,0.5f*Time.deltaTime); 			// 
							m_Km = Mathf.Lerp(m_Km, 0.02707681f + Seasonal_factor4_add,0.5f*Time.deltaTime);  			//
							m_g = Mathf.Lerp(m_g, -0.9572327f + Seasonal_factor5_add,0.5f*Time.deltaTime); 
						Sun_ring_factor=0;
							m_fSamples=Mathf.Lerp(m_fSamples, 2.695652f + Seasonal_factor6_add,0.5f*Time.deltaTime); 
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth, 0.4036232f + Seasonal_factor7_add,0.5f*Time.deltaTime); 
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(1,0.5472732f,0.5204539f),0.5f*Time.deltaTime); //
					}
							}

					//GLOBAL TINT
							if( (AutoSunPosition && Rot_Sun_X < 0  ) | (!AutoSunPosition && Current_Time > (22+Shift_dawn) | (Current_Time >= 0 & Current_Time <=(1+Shift_dawn)))){//v2.0.1

								if(Application.isPlaying){
									MAIN.color =  Color.Lerp (MAIN.color, Night_Sun_Color , 1.5f*UP_rate*Time.deltaTime*SPEED); //v2.0.1
								}else{
									if(MAIN.color !=  Night_Sun_Color){
										MAIN.color = Night_Sun_Color;
									}
								}

	//					m_TintColor =  Color.Lerp (m_TintColor, new Color(0.0f,0,0,0) , 2.35f*UP_rate*Time.deltaTime); 
						
							if(Application.isPlaying){
								RenderSettings.ambientLight = Color.Lerp (RenderSettings.ambientLight, Night_Ambient_Color , 2f*UP_rate*Time.deltaTime);//v1.2.1
							}else{
								if(RenderSettings.ambientLight != Night_Ambient_Color){
									RenderSettings.ambientLight =Night_Ambient_Color;//v1.5
								}
							}
					}else{


								if(Application.isPlaying){
									MAIN.color =  Color.Lerp (MAIN.color, Dusk_Sun_Color , 0.5f*UP_rate*Time.deltaTime*SPEED); 
								}else{
									if(MAIN.color !=  Dusk_Sun_Color){
										MAIN.color = Dusk_Sun_Color;
									}
								}

								if(Application.isPlaying){
								RenderSettings.ambientLight = Color.Lerp (RenderSettings.ambientLight, Dusk_Ambient_Color , 0.5f*UP_rate*Time.deltaTime);//v1.2.1
							}else{
								if(RenderSettings.ambientLight != Dusk_Ambient_Color){
									RenderSettings.ambientLight =Dusk_Ambient_Color;//v1.5
								}
							}
							//GLOBAL TINT - v1.6.5
	//						m_TintColor =  Color.Lerp (m_TintColor, new Color(0f,0,0,0) , 0.55f*UP_rate*Time.deltaTime);
					}

			}
				}

			if(USE_SKYBOX){

						if(Current_Time < 18 & Current_Time > 9 ){
							if(Current_Time > 16){

							}else if(Current_Time > 12){
								//SKYBOX
							}else{

							}

							//SKYBOX
							mSunSize=Mathf.Lerp(mSunSize,10,0.5f*Time.deltaTime);			
							mSunTint=Vector4.Lerp(mSunTint,new Vector4(255/Color_devider,148/Color_devider,27/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyExponent=Mathf.Lerp(mSkyExponent,1.72f,0.5f*Time.deltaTime);					
							mSkyTopColor=Vector4.Lerp(mSkyTopColor,new Vector4(100/Color_devider,5/Color_devider,129/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyMidColor=Vector4.Lerp(mSkyMidColor,new Vector4(125/Color_devider,128/Color_devider,219/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyEquatorColor=Vector4.Lerp(mSkyEquatorColor,new Vector4(89/Color_devider,126/Color_devider,124/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mGroundColor=Vector4.Lerp(mGroundColor,new Vector4(94/Color_devider,65/Color_devider,53/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							//SKYBOX

						}else if(Current_Time <=9 & Current_Time > 0){
							//SKYBOX
							mSunSize=Mathf.Lerp(mSunSize,5,0.5f*Time.deltaTime);			
							mSunTint=Vector4.Lerp(mSunTint,new Vector4(255/Color_devider,250/Color_devider,244/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyExponent=Mathf.Lerp(mSkyExponent,0.72f,0.5f*Time.deltaTime);					
							mSkyTopColor=Vector4.Lerp(mSkyTopColor,new Vector4(0/Color_devider,5/Color_devider,0/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyMidColor=Vector4.Lerp(mSkyMidColor,new Vector4(0/Color_devider,0/Color_devider,0/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyEquatorColor=Vector4.Lerp(mSkyEquatorColor,new Vector4(0/Color_devider,0/Color_devider,0/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mGroundColor=Vector4.Lerp(mGroundColor,new Vector4(0/Color_devider,0/Color_devider,0/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
						}else{//18 to 1 at night
							//SKYBOX
							mSunSize=Mathf.Lerp(mSunSize,10,0.5f*Time.deltaTime);			
							mSunTint=Vector4.Lerp(mSunTint,new Vector4(255/Color_devider,148/Color_devider,27/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyExponent=Mathf.Lerp(mSkyExponent,1.72f,0.5f*Time.deltaTime);					
							mSkyTopColor=Vector4.Lerp(mSkyTopColor,new Vector4(100/Color_devider,5/Color_devider,129/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyMidColor=Vector4.Lerp(mSkyMidColor,new Vector4(125/Color_devider,128/Color_devider,219/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mSkyEquatorColor=Vector4.Lerp(mSkyEquatorColor,new Vector4(89/Color_devider,126/Color_devider,124/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
							mGroundColor=Vector4.Lerp(mGroundColor,new Vector4(94/Color_devider,65/Color_devider,53/Color_devider,255/Color_devider),0.5f*Time.deltaTime);
						}
				}


                    ///// WEATHER RALATED FIXES

                    //STORM LERATED
                    if (Weather == Weather_types.HeavyStormDark)
                    {
                        m_fExposure = Mathf.Lerp(m_fExposure, 0, 1.88f * Time.deltaTime);
                        m_ESun = Mathf.Lerp(m_ESun, 1, 1.88f * Time.deltaTime);
                        m_g = Mathf.Lerp(m_g, -0.5f, 0.28f * Time.deltaTime);
                        m_TintColor = Color.Lerp(m_TintColor, Color.black, 1.88f * Time.deltaTime);

                        if (Application.isPlaying)
                        {
                            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, new Color(0.07f, 0.07f, 0.07f, 0.1f), 0.5f * Time.deltaTime);
                        }
                        else
                        {
                            if (RenderSettings.ambientLight != new Color(0.07f, 0.07f, 0.07f, 0.1f))
                            {
                                RenderSettings.ambientLight = new Color(0.07f, 0.07f, 0.07f, 0.1f);//v1.5
                            }
                        }
                        if (Application.isPlaying)
                        {
                            MAIN.intensity = Mathf.Lerp(MAIN.intensity, 0.02f, 0.5f * Time.deltaTime);
                        }
                        else
                        {
                            if (MAIN.intensity != 0.02f)
                            {
                                MAIN.intensity = 0.02f;
                            }
                        }
                        if (star_dome_Material)
                        {
                            star_dome_Material.color = Color.black;
                        }
						if(Star_particles_OBJ_P!=null){
							ParticleSystem.MainModule MainMod = Star_particles_OBJ_P.main; //v3.4.9
							if(MainMod.maxParticles>5){
								MainMod.maxParticles -=10;
							}else{
								//Star_particles_OBJ.SetActive(false);
							}
						}
						if(MoonObj!=null){
							if(MoonObj.activeInHierarchy){
								MoonObj.SetActive(false);
							}
						}
					}else{
										//v2.0.1 - //v3.0.3
										//bool is_DayLight  = (AutoSunPosition && Rot_Sun_X > 0 ) | (!AutoSunPosition && Current_Time >= ( 9.0f + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn)); //v3.3
										if(is_DayLight){
										//if(Current_Time > 9 & Current_Time <23){

												//v3.3 - regulate in weather
												float Max_sun_intensity1 = Max_sun_intensity;
												if(currentWeatherName == Volume_Weather_types.HeavyStorm | currentWeatherName == Volume_Weather_types.HeavyStormDark || (AutoMoonLighting && onEclipse)){
													Max_sun_intensity1 = Max_sun_intensity / 3;
												}
												if(currentWeatherName == Volume_Weather_types.Rain | currentWeatherName == Volume_Weather_types.SnowStorm){
													Max_sun_intensity1 = Max_sun_intensity / 1.5f;
												}

												if(Application.isPlaying){
													MAIN.intensity = Mathf.Lerp(MAIN.intensity,Max_sun_intensity1,To_max_intensity_speed*Time.deltaTime);
												}else{
													if(MAIN.intensity != Max_sun_intensity1){
														MAIN.intensity =Max_sun_intensity1;
													}
												}
										}else{
												if(Application.isPlaying){
													MAIN.intensity = Mathf.Lerp(MAIN.intensity,Min_sun_intensity,To_min_intensity_speed*Time.deltaTime);
												}else{
													if(MAIN.intensity != Min_sun_intensity){
														MAIN.intensity =Min_sun_intensity;
													}
												}
										}
//						if(Star_particles_OBJ_P!=null){
//							if(Star_particles_OBJ_P.maxParticles < 1000){
//								Star_particles_OBJ_P.maxParticles +=20;
//							}
//						}
						if(MoonObj!=null){
							if(!MoonObj.activeInHierarchy){
								MoonObj.SetActive(true);
							}
						}
					}

					//v1.5
					//if(skyboxMat != null)
					if(1==1) //v1.7
					{

						float Speed1 = SPEED;
						if(Speed1 <=0){
							Speed1=0.1f;

						}
						float speed = (2.2f*Time.deltaTime);

						if(Unity5){

							Run_presets();

						}else{
							if(Current_Time < 22 & Current_Time > 9 ){
								if(Preset ==0){
									if(Current_Time < 17){
										m_Coloration = Mathf.Lerp(m_Coloration,0.14f,speed);
										m_fExposure =Mathf.Lerp(m_fExposure,7,speed);
										m_ESun = Mathf.Lerp(m_ESun,35.38f,speed); 
										m_TintColor = Color.Lerp(m_TintColor,new Color(200f/255f,202f/255f,201f/255f,255f/255f),speed);
									}
									else if(Current_Time < 21){
										m_Coloration = Mathf.Lerp(m_Coloration,0.11f,speed);
										m_fExposure =Mathf.Lerp(m_fExposure,6,speed);
										m_ESun = Mathf.Lerp(m_ESun,30.38f,speed); 
										m_TintColor = Color.Lerp(m_TintColor,new Color(121f/255f,112f/255f,111f/255f,255f/255f),speed);
										
									}else{
										m_Coloration = Mathf.Lerp(m_Coloration,0.1f,speed/5);
										m_fExposure =Mathf.Lerp(m_fExposure,4,speed/5);
										m_ESun = Mathf.Lerp(m_ESun,26.38f,speed/5); 
										m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),speed/5);
									}

									m_Kr = Mathf.Lerp(m_Kr,0.0036f,speed);
									m_Km = 0.0044f;
									m_g = Mathf.Lerp(m_g,-0.94f,speed);
									Sun_ring_factor=0;
									m_fSamples= Mathf.Lerp(m_fSamples,1.93f,speed);
									m_fRayleighScaleDepth = 0.52f;
									m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.4286f,0.5f,0.80f),speed*8);
									//Debug.Log ("111");
								}
								if(Preset ==3){
									if(Current_Time < 17){
										m_Coloration = Mathf.Lerp(m_Coloration,0.014f,speed);
										m_fExposure =Mathf.Lerp(m_fExposure,7,speed);
										m_ESun = Mathf.Lerp(m_ESun,35.38f,speed); 
										m_TintColor = Color.Lerp(m_TintColor,new Color(200f/255f,202f/255f,201f/255f,255f/255f),speed);
									}
									else if(Current_Time < 21){
										m_Coloration = Mathf.Lerp(m_Coloration,0.011f,speed);
										m_fExposure =Mathf.Lerp(m_fExposure,6,speed);
										m_ESun = Mathf.Lerp(m_ESun,30.38f,speed); 
										m_TintColor = Color.Lerp(m_TintColor,new Color(121f/255f,112f/255f,111f/255f,255f/255f),speed);
										
									}else{
										m_Coloration = Mathf.Lerp(m_Coloration,0.01f,speed/5);
										m_fExposure =Mathf.Lerp(m_fExposure,4,speed/5);
										m_ESun = Mathf.Lerp(m_ESun,26.38f,speed/5); 
										m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),speed/5);
									}
									
									m_Kr = Mathf.Lerp(m_Kr,0.0036f,speed);
									m_Km = 0.012f;
									m_g = Mathf.Lerp(m_g,-0.94f,speed);
									Sun_ring_factor=0;
									m_fSamples= Mathf.Lerp(m_fSamples,1.93f,speed);
									m_fRayleighScaleDepth = 0.52f;
									m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.43f,0.5f,0.30f),speed*8);
								}

						}else if(Current_Time <=9 & Current_Time > 1){
								//if(Preset ==0){
								m_TintColor = Color.Lerp(m_TintColor,Color.black,speed);
								m_Coloration = Mathf.Lerp(m_Coloration,0.03f,speed);
								m_fExposure =Mathf.Lerp(m_fExposure,0.8f,speed);
								m_ESun = Mathf.Lerp(m_ESun,5.03f,speed);
								m_Kr = Mathf.Lerp(m_Kr,0.06f,speed);
								m_Km = 0.0044f;
								m_g = Mathf.Lerp(m_g,-0.99f,speed);
								Sun_ring_factor=0;
								m_fSamples = Mathf.Lerp(m_fSamples,2.53f,speed);
								m_fRayleighScaleDepth = 0.49f;
								m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.44f,0.48f,0.41f),speed);
								//}
						}else{//18 to 1 at night
								if(Preset ==0){
									m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),speed);
									m_Coloration = Mathf.Lerp(m_Coloration,0.1f,speed);
									m_fExposure =Mathf.Lerp(m_fExposure,2,speed);
									m_ESun = Mathf.Lerp(m_ESun,22.38f,speed); 
									m_Kr = Mathf.Lerp(m_Kr,0.0036f,speed);
									m_Km = 0.0044f;
									m_g = Mathf.Lerp(m_g,-0.94f,speed);
									Sun_ring_factor=0;
									m_fSamples= Mathf.Lerp(m_fSamples,1.93f,speed);
									m_fRayleighScaleDepth = 0.52f;
									m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.4286f,0.5f,0.80f),speed);
								}
								if(Preset ==3){

									m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),speed);
									m_Coloration = Mathf.Lerp(m_Coloration,0.01f,speed);
									m_fExposure =Mathf.Lerp(m_fExposure,2,speed);
									m_ESun = Mathf.Lerp(m_ESun,22.38f,speed); 
									m_Kr = Mathf.Lerp(m_Kr,0.0036f,speed);
									m_Km = 0.012f;
									m_g = Mathf.Lerp(m_g,-0.94f,speed);
									Sun_ring_factor=0;
									m_fSamples= Mathf.Lerp(m_fSamples,1.93f,speed);
									m_fRayleighScaleDepth = 0.52f;
									m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.43f,0.5f,0.30f),speed);

								}
							}
							m_fOuterRadius = 10250;
							//scale_dif = 0.06f;
							if(RenderSettings.skybox != skyboxMat){
								RenderSettings.skybox = skyboxMat;
							}
						}
					}

		}//END AUTO CYCLE DAY TIME

			if(!USE_SKYCUBE){	
				//v1.5
				if(skyMat != null){
						Assign_Material_Props(skyMat,Cam_tranform.position);//transform);
						Assign_Material_PropsS(skyMat,Cam_tranform.position);//transform);
				}
				if(skyboxMat != null && Cam_tranform != null)
                    {

						//v3.3c
						Vector3 inputT = Cam_tranform.position;
						if (!Application.isPlaying && init_scene) {
							if (Camera.current == null) {
								cut_off_main_cam = false;
								inputT = Cam_tranform.position;
								//if (Application.isEditor && prev_scene_cam_pos != null) {
								if (Application.isEditor) {
									inputT = prev_scene_cam_pos;
								}
							} else {
								inputT = Camera.current.transform.position;
							}
							prev_scene_cam_pos = Cam_tranform.position;
						}

						if (Time.fixedTime - last_mat_update > Update_mat_every | (!Application.isPlaying && !cut_off_main_cam)) {
							last_mat_update = Time.fixedTime;
							Assign_Material_Props (skyboxMat, inputT);
							init_scene = true;
						}
						Assign_Material_PropsS(skyboxMat, inputT);

						//Assign_Material_Props(skyboxMat,Cam_tranform.position);//transform);
				}
			}
			//////////////////////// END SET SKY MATERIAL /////////////////

			
		}			
		//END TIME

			//v1.5
			if(skyboxMat != null){
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}
			}

		//SKYBOX
		if(USE_SKYCUBE & Application.isPlaying){

		//END SKYBOX
				//v3.3b
				if(Time.fixedTime - lastReflectTime > ReflectEvery){

			if(SkyCam ==null){
				SkyCam = new Camera();
			}
			if(SkyCamOBJ ==null){				
				SkyCamOBJ = new GameObject("SkyMasterCam", typeof(Camera));				
			}

			SkyCam = SkyCamOBJ.GetComponent(typeof(Camera)) as Camera;
			SkyCam_transform = SkyCamOBJ.transform;

				//v3.3b
				if (SkyDomeSystem != null) {
					//v1.5
					if (skyMat != null) {
							Assign_Material_Props (skyMat, SkyCam_transform.position);
							Assign_Material_PropsS (skyMat, SkyCam_transform.position);
					}
					if (skyboxMat != null) {
							Assign_Material_Props (skyboxMat, SkyCam_transform.position);
							Assign_Material_PropsS (skyboxMat, SkyCam_transform.position);
					}
				} else {
					if (skyMat != null) {
							Assign_Material_Props (skyMat, Cam_tranform.position);
							Assign_Material_PropsS (skyMat, Cam_tranform.position);
					}
					if (skyboxMat != null) {
							Assign_Material_Props (skyboxMat, Cam_tranform.position);
							Assign_Material_PropsS (skyboxMat, Cam_tranform.position);
					}
				}



					lastReflectTime = Time.fixedTime;

					SkyCam.farClipPlane = ReflectCamfarClip;// 6750;
					SkyCam.nearClipPlane = 0.3f;
					SkyCam.aspect = 1;

					//v3.3b
					if (ReflectSkybox) {
						SkyCam.clearFlags = CameraClearFlags.Skybox;
					} else {
						SkyCam.clearFlags = CameraClearFlags.Depth;
					}
					SkyCam.backgroundColor = Color.black;

					SkyCam.enabled=false;

					SkyCam.cullingMask = SkyboxLayer;

					Vector3 Displace = Vector3.zero;
					SkyCam_transform.position = Test_Cubemap.transform.position+Displace;

					SkyCam_transform.rotation = Quaternion.identity;

					//v3.4
					bool PrevFogSetting = RenderSettings.fog; 
					RenderSettings.fog = false;

					//v3.3b
					if(CubeTexture == null){
						CubeTexture = new Cubemap(256,TextureFormat.ARGB32,false);
						//Shader CUBE_Shader = Shader.Find("RenderFX/Skybox Cubed");  
						CUBE_Mat.SetTexture("_Tex",CubeTexture);
					}

					CubeTexture.wrapMode = TextureWrapMode.Clamp;
					SkyCam.RenderToCubemap(CubeTexture);
					Material TEST_IT = Test_Cubemap.GetComponent<Renderer>().sharedMaterial;					
					TEST_IT.SetTexture("_Cube", CubeTexture);

					//v3.3b
					for(int i=0;i<AssignCubeMapMats.Count;i++){
						Material TEST_IT1 = AssignCubeMapMats[i];
						if (TEST_IT1.HasProperty ("_Cube")) {
							TEST_IT1.SetTexture ("_Cube", CubeTexture);
						}
					}

					DestroyImmediate( SkyCamOBJ );

					RenderSettings.fog = PrevFogSetting;//v3.4

					//DestroyImmediate( CubeTexture );
				}else{
					if(skyMat != null){
						Assign_Material_Props(skyMat,Cam_tranform.position);//transform);
						Assign_Material_PropsS(skyMat,Cam_tranform.position);//transform);
					}
					if(skyboxMat != null){
						//v3.3c
						if (Time.fixedTime - last_mat_update > Update_mat_every) {
							last_mat_update = Time.fixedTime;
							Assign_Material_Props(skyboxMat,Cam_tranform.position);
						}
						Assign_Material_PropsS(skyboxMat,Cam_tranform.position);
					}
				}

		}

	////////////////////////// END CLOUDS /////////////////////
			/// 

			//v1.6.5
			if(Camera.current != null & Application.isEditor){
				//cam_pos = Camera.current.transform.position;
			}

			//v3.3b
			if (updateSkyAmbient) {			
				if (Time.fixedTime - lastAmbientUpdateTime > AmbientUpdateEvery) {

					if (currentWeatherName == Volume_Weather_types.HeavyStorm || currentWeatherName == Volume_Weather_types.HeavyStormDark) {
						RenderSettings.ambientIntensity = AmbientIntensity/2;
					} else {
						DynamicGI.UpdateEnvironment ();
						RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
						RenderSettings.ambientIntensity = AmbientIntensity;
					}
					lastAmbientUpdateTime = Time.fixedTime;
				}			
			}

			//v3.4 - sun system Y follow hero
			if(SunFollowHero && Hero != null){
				SunSystem.transform.position = new Vector3(SunSystem.transform.position.x,Hero.position.y,SunSystem.transform.position.z);
			}

	}//END UPDATE

		//v3.3c
		public void OnDrawGizmos(){
			if (!Application.isPlaying && init_presets && init_scene) {
				cut_off_main_cam = false;
				if (Camera.current != null && Camera.main != null && Camera.current == Camera.main && Camera.current.transform.position != prev_scene_cam_pos) { //v4.9.3				
					Assign_Material_Props (skyboxMat, Camera.current.transform.position);
					Assign_Material_PropsS (skyboxMat, Camera.current.transform.position);
					cut_off_main_cam = true;
					prev_scene_cam_pos = Camera.current.transform.position;
				} 
			}
		}

		//v2.0.1
		void Run_presets(){

			float FinalSPEED = Time.deltaTime * SPEED;
			if(SPEED < 1){
				//FinalSPEED = Time.deltaTime; //v3.1
			}

			//v2.0.1
			if(!Application.isPlaying  | !init_presets){
				FinalSPEED = 4000;
			}


			//v3.4
			if (!Application.isPlaying && Rot_Sun_X == Previous_Rot_X && !UseGradients && AutoSunPosition) {
				return;
			}

			//v3.0 - presets 0 and 7-11 if(){
//			bool is_DayLight  = (AutoSunPosition && Rot_Sun_X > 0 ) | (!AutoSunPosition && Current_Time >= ( 9.0f + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn));
//			//bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 ) | (!AutoSunPosition && Current_Time >  (22.1f + Shift_dawn));
//			bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 ) | (!AutoSunPosition && Current_Time >  (NightTimeMax-0.3f + Shift_dawn));
//			bool is_after_17  = (AutoSunPosition && Rot_Sun_X > 65) | (!AutoSunPosition && Current_Time >  (17.1f + Shift_dawn));
//			bool is_before_10 = (AutoSunPosition && Rot_Sun_X < 10) | (!AutoSunPosition && Current_Time <  (10.0f + Shift_dawn));
//			bool is_before_11 = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time <  (11.0f + Shift_dawn));
//			bool is_before_16 = (AutoSunPosition && Rot_Sun_X < 60) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn));
//			bool is_before_85 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time <  ( 8.5f + Shift_dawn));
//			bool is_after_23  = (AutoSunPosition && Rot_Sun_X <  3) | (!AutoSunPosition && Current_Time >  (23.0f + Shift_dawn));
//			bool is_after_224 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time >  (NightTimeMax + Shift_dawn));
//			bool is_duskNight = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time >  (19.0f + Shift_dawn) & Current_Time < (23 + Shift_dawn));
//			bool is_dayToDusk = (AutoSunPosition && Rot_Sun_X > 15 && Rot_Sun_X < 45) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn) & Current_Time > (11f + Shift_dawn));

			//v3.3
			bool is_DayLight  = (AutoSunPosition && Rot_Sun_X > 0 ) | (!AutoSunPosition && Current_Time >= ( 9.0f + Shift_dawn) & Current_Time <= (NightTimeMax + Shift_dawn));
			//bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 ) | (!AutoSunPosition && Current_Time >  (22.1f + Shift_dawn));
			bool is_after_22  = (AutoSunPosition && Rot_Sun_X < 5 && Previous_Rot_X > Rot_Sun_X) | (!AutoSunPosition && Current_Time >  (NightTimeMax-0.3f + Shift_dawn));
			//bool is_after_17  = (AutoSunPosition && Rot_Sun_X > 65 && Previous_Rot_X < Rot_Sun_X) | (!AutoSunPosition && Current_Time >  (17.1f + Shift_dawn));
			bool is_after_17  = (AutoSunPosition && Rot_Sun_X < 45 && Previous_Rot_X > Rot_Sun_X) | (!AutoSunPosition && Current_Time >  (17.1f + Shift_dawn));
			bool is_before_10 = (AutoSunPosition && Rot_Sun_X < 10) | (!AutoSunPosition && Current_Time <  (10.0f + Shift_dawn));
			bool is_before_11 = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time <  (11.0f + Shift_dawn));
			bool is_before_16 = (AutoSunPosition && Rot_Sun_X < 60 && Previous_Rot_X < Rot_Sun_X) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn));
			bool is_before_85 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time <  ( 8.5f + Shift_dawn));
			bool is_after_23  = (AutoSunPosition && Rot_Sun_X <  3) | (!AutoSunPosition && Current_Time >  (23.0f + Shift_dawn));
			bool is_after_224 = (AutoSunPosition && Rot_Sun_X <  5) | (!AutoSunPosition && Current_Time >  (NightTimeMax + Shift_dawn));
			bool is_duskNight = (AutoSunPosition && Rot_Sun_X < 15) | (!AutoSunPosition && Current_Time >  (19.0f + Shift_dawn) & Current_Time < (23 + Shift_dawn));
			//bool is_dayToDusk = (AutoSunPosition && Rot_Sun_X > 15 && Rot_Sun_X < 45) | (!AutoSunPosition && Current_Time <  (16.1f + Shift_dawn) & Current_Time > (11f + Shift_dawn)); //v3.3d


			if(Preset == 0){
				//m_fExposure =0.92f;
				//		m_fExposure =1.6f;
				//m_fExposure =2.8f; //v1.6.5 
				//m_ESun = 7.56f;
				//v1.7
				
				//if(Current_Time >= (9 + Shift_dawn) & Current_Time <= (22.4f + Shift_dawn)  ){
				if(is_DayLight){
					
					m_fExposure =1.6f;
					
					//if(Current_Time > (22.1f + Shift_dawn) ){
					if(is_after_22 ){
						
						//m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , Time.deltaTime);
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
						if(!Application.isPlaying | !init_presets){
							m_ESun = 0f;
							//m_Kr =0.006510659f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
							scale_dif = 0.06f;
						}
					}
					else
					//if(Current_Time > (17.1f + Shift_dawn) ){
					if(is_after_17 ){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						
						m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.008510659f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 1.66f;
							m_Kr =0.008510659f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
							scale_dif = 0.06f;
						}
					}else{
						//if(Current_Time < (10+ Shift_dawn) ){//red dawn start
						if(is_before_10){
							//Debug.Log (10 + Shift_dawn);
								FinalSPEED = FinalSPEED * DawnAppearSpeed; //v3.1

							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.004510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.32f),FinalSPEED);


//							if(Shift_dawn  == -1 & 1==0){
//								if(Current_Time < 8.25f){
//									scale_dif = Mathf.Lerp(scale_dif,0.000567819f,FinalSPEED*2);
//								}else
//								if(Current_Time < 8.50f){
//									scale_dif = Mathf.Lerp(scale_dif,0.001567819f,FinalSPEED*0.2f);
//								}
//								else
//								if(Current_Time < 8.750f){
//									scale_dif = Mathf.Lerp(scale_dif,0.005567819f,FinalSPEED*0.2f);
//								}
//								else{
//									scale_dif = Mathf.Lerp(scale_dif,0.015567819f,FinalSPEED*0.1f);
//								}
//							}else{
								scale_dif = Mathf.Lerp(scale_dif,0.09f,FinalSPEED);
							//}

							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.004510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.32f);
								scale_dif = 0.09f;
							}
						}else 
						//if(Current_Time < (11 + Shift_dawn))//dawn pinkish
						if(is_before_11)
						{
							//Debug.Log (11 + Shift_dawn);

							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.45f,0.41f,0.39f),FinalSPEED*0.1f);
							//scale_dif = Mathf.Lerp(scale_dif,0.16f,0.3f*FinalSPEED);

//							if(Shift_dawn  == -1 & 1==0){
//								scale_dif = Mathf.Lerp(scale_dif,0.05f,FinalSPEED*0.1f);
//							}else{
								scale_dif = Mathf.Lerp(scale_dif,0.16f,FinalSPEED*0.1f);
							//}

							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.005510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.45f,0.41f,0.39f);
								scale_dif = 0.16f;
							}
						}
						else if(is_before_16){//if(Current_Time < (16.1f + Shift_dawn) ){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.003510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.003510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.07f;
							}
						}
						else{//return from pinkish
							m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
							
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.008f,FinalSPEED*0.1f);//m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.0318f,FinalSPEED*0.1f);//m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,Time.deltaTime*SPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.16f;
							}
						}
					}
					
					//m_Kr = 0.0045106587f;
					//m_fRayleighScaleDepth = 0.04161f;
					
					//if(!Application.isPlaying){ m_ESun = 1.66f; }
					
					m_g = -0.9990211f;//m_g = -0.9502108f;
					//Sun_ring_factor=0;
					m_fSamples=0.02f;//m_fSamples=1.37f;
					
				}else{
					
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);
					
					//if(Current_Time < (8.5f +Shift_dawn) ){
					if(is_before_85){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
						
						float SpeedA = 4;

						if(!Application.isPlaying | !init_presets){
							SpeedA = 4000;
						}

						m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
						m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
						m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
						scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
						m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
						m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
						
					}else{
						//if(Current_Time > (23 +Shift_dawn) ){
						if(is_after_23 ){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED); //v2.0.1
							if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
							
							float SpeedA = 1;

							if(!Application.isPlaying | !init_presets){
								SpeedA = 4000;
							}

							m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
							m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
							m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
							m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
							m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
							
						}else{
							//if(Current_Time > (22.4f +Shift_dawn)){
							if(is_after_224){
								m_ESun = 0;//Mathf.Lerp(m_ESun,0,2.1f*FinalSPEED);//fade out moon, so we can move shader to sun direction smoothly
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.3f*FinalSPEED);//v2.0.1 - fast fade out to avoid point light on moon = 0.2 to 0.3
							}
							if(!Application.isPlaying | !init_presets){ m_ESun = 0f; }
						}
					}
					
					//		m_Kr = 0.008510659f ;
					//m_fRayleighScaleDepth = 0.03f;
					//		m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED);
					//		m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED);
					//		scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED);
					
					if(!Application.isPlaying  | !init_presets){
						//m_ESun = 1.66f;
						//m_Kr =0.006510659f;
						m_fRayleighScaleDepth =0.03f;
						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
						scale_dif = 0.06f;
					}
				}
				
				//								if((Current_Time > 22.4 & Current_Time <=25) | (Current_Time >= 0 & Current_Time < 6)){
				//									//m_ESun = Moon_glow;//Mathf.Lerp(m_ESun,Moon_glow,81*FinalSPEED);
				//
				//								}else{
				//
				//								}
				//m_Kr = 0.0045106587f;
				m_Km = 0.0004f;//m_Km = 0.00069617826f;
				//			m_g = -0.9990211f;//m_g = -0.9502108f;
				//Sun_ring_factor=0;
				//			m_fSamples=0.02f;//m_fSamples=1.37f;
				//			m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
				//m_fRayleighScaleDepth = 0.04161f;
				//			scale_dif = 0.06f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}
				
				m_Coloration = 0.1f + SkyColorationOffset;
				Sun_ring_factor =0;

				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
			}
			
			////////////// UNITY5 PRESET 1 //////////////
			
			if(Preset ==1){
				m_fExposure =1.6f;

				//v2.0.1
				if(!Application.isPlaying  | !init_presets){
					FinalSPEED = 4000;
				}

				//v1.7								
				if(Current_Time >= (9+Shift_dawn) & Current_Time <=(22.4f+Shift_dawn)){									
					if(Current_Time > (22.1f+Shift_dawn)){									
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
					}
					else
					if(Current_Time > (17.1f+Shift_dawn)){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.008510659f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
					}else{
						if(Current_Time < (10+Shift_dawn) ){//red dawn start
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.004510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.32f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.09f,FinalSPEED);
						}else if(Current_Time < (11+Shift_dawn))//dawn pinkish
						{
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.45f,0.41f,0.39f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.16f,0.3f*FinalSPEED);
						}
						else if(Current_Time < (16.1f+Shift_dawn)){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.003510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,FinalSPEED*0.1f);
						}
						else{//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.16f,FinalSPEED*0.1f);
						}
					}
				}else{									
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);									
					if(Current_Time < (8.5f+Shift_dawn)){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
					}else{
						if(Current_Time > (23+Shift_dawn)){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED);
						}else{
							if(Current_Time > (22.4f+Shift_dawn)){
								m_ESun = 0;//Mathf.Lerp(m_ESun,0,2.1f*FinalSPEED);//fade out moon, so we can move shader to sun direction smoothly
							}else{
								m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.2f*FinalSPEED);
							}
						}
					}									
					m_Kr = 0.009510659f ;//m_Kr = 0.008510659f ;
					m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED);
					m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED);
					scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED);
				}
				m_Km = 0.0004f;//m_Km = 0.00069617826f;
				m_g = -0.9990211f;//m_g = -0.9502108f;
				m_fSamples=0.02f;//m_fSamples=1.37f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}
				
				m_Coloration = 0 + SkyColorationOffset;
				Sun_ring_factor =0;

				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
				
			}
			/// END PRESET 1
			
			////////////// UNITY5 PRESET 2 //////////////
			
			if(Preset ==2){
				m_fExposure =1.6f;

				//v2.0.1
				if(!Application.isPlaying  | !init_presets){
					FinalSPEED = 4000;
				}

				//v1.7								
				if(Current_Time >= (9+Shift_dawn) & Current_Time <=(22.4f+Shift_dawn)){									
					if(Current_Time > (22.1f+Shift_dawn)){									
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
					}
					else
					if(Current_Time > (17.1f+Shift_dawn)){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.008510659f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
					}else{
						if(Current_Time < (10+Shift_dawn) ){//red dawn start
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.004510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.32f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.09f,FinalSPEED);
						}else if(Current_Time < (11+Shift_dawn))//dawn pinkish
						{
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.45f,0.41f,0.39f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.16f,0.3f*FinalSPEED);
						}
						else if(Current_Time < (16.1f+Shift_dawn)){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.003510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,FinalSPEED*0.1f);
						}
						else{//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.16f,FinalSPEED*0.1f);
						}
					}
					Sun_ring_factor = 0.1f;
				}else{									
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);									
					if(Current_Time < (8.5f+Shift_dawn)){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
					}else{
						if(Current_Time > (23+Shift_dawn)){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.05f,FinalSPEED);
						}else{
							if(Current_Time > (22.4f+Shift_dawn)){
								m_ESun = 0;//Mathf.Lerp(m_ESun,0,2.1f*FinalSPEED);//fade out moon, so we can move shader to sun direction smoothly
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.2f*FinalSPEED);
							}
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED);
						}
						
					}									
					m_Kr = 0.009510659f ;//m_Kr = 0.008510659f ;
					//	m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED);
					m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED);
					scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED);
					Sun_ring_factor = 0.1f;
				}
				m_Km = 0.0004f;//m_Km = 0.00069617826f;
				m_g = -0.9990211f;//m_g = -0.9502108f;
				m_fSamples=0.02f;//m_fSamples=1.37f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}
				
				m_Coloration = 0.15f + SkyColorationOffset;
				//Sun_ring_factor = 0.1f;
				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
				
			}
			/// END PRESET 2 
			
			//////////// UNITY PRESET 3
			
			if(Preset == 3){
				
				if(Current_Time >= (9+Shift_dawn) & Current_Time <=(22.4f+Shift_dawn)){									
					m_fExposure =1.6f;									
					if(Current_Time > (22.1f+Shift_dawn)){										
						//m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , Time.deltaTime);
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_Km =Mathf.Lerp(m_Km,0.09f,0.5f*FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 0f;
							//m_Kr =0.006510659f;
							m_Km = 0.09f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
							scale_dif = 0.06f;
						}
					}
					else
					if(Current_Time > (17.1f+Shift_dawn)){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						
						m_ESun = Mathf.Lerp(m_ESun,3.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.008510659f,0.5f*FinalSPEED*0.1f);
						m_Km =Mathf.Lerp(m_Km,0.09f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.034f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.65f,0.64f,0.39f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.065f,FinalSPEED*0.1f);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 1.66f;
							m_Kr =0.008510659f;
							m_Km = 0.09f;
							m_fRayleighScaleDepth =0.034f;
							m_fWaveLength = new Vector3(0.65f,0.64f,0.39f);
							scale_dif = 0.065f;
						}
					}else{
						if(Current_Time < (10+Shift_dawn) ){//red dawn start
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.004510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.32f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.09f,FinalSPEED);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.004510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.32f);
								scale_dif = 0.09f;
							}
						}else if(Current_Time < (11+Shift_dawn))//dawn pinkish
						{
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.45f,0.41f,0.39f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.16f,0.3f*FinalSPEED);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.005510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.45f,0.41f,0.39f);
								scale_dif = 0.16f;
							}
						}
						else if(Current_Time < (16.1f+Shift_dawn)){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.003510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.003510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.07f;
							}
						}
						else{//return from pinkish
							m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
							
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.008f,FinalSPEED*0.1f);//m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.0318f,FinalSPEED*0.1f);//m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,Time.deltaTime*SPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.16f;
							}
						}
					}									
					
					m_g = -0.9990211f;//m_g = -0.9502108f;
					m_fSamples=0.02f;//m_fSamples=1.37f;
					
				}else{
					
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);
					
					if(Current_Time < (8.5f+Shift_dawn)){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
						
						float SpeedA = 4;
						m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
						m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
						m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
						scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
						m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
						m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
						
					}else{
						if(Current_Time > (23+Shift_dawn)){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED);
							if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
							
							float SpeedA = 1;
							m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
							m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
							m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
							m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
							m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
							
						}else{
							if(Current_Time > (22.4f+Shift_dawn)){
								m_ESun = 0;//Mathf.Lerp(m_ESun,0,2.1f*FinalSPEED);//fade out moon, so we can move shader to sun direction smoothly
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.2f*FinalSPEED);
							}
							if(!Application.isPlaying | !init_presets){ m_ESun = 0f; }
						}
					}									
					
					if(!Application.isPlaying  | !init_presets){
						m_fRayleighScaleDepth =0.03f;
						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
						scale_dif = 0.06f;
					}
				}								
				
				//	m_Km = 0.0004f;							
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}
				
				m_Coloration = 0.1f + SkyColorationOffset;
				Sun_ring_factor =0;

				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
			}
			
			//////////// END PRESET 3
			
			if(Preset ==4 & 1==1){
				m_fExposure =0.92f;
				//m_fExposure =2.8f; //v1.6.5
				//m_ESun = 7.56f;
				//v1.7
				if((Current_Time > (22.4f+Shift_dawn) & Current_Time <=25) | (Current_Time >= 0 & Current_Time < (9+Shift_dawn))){//6
					//m_ESun = Moon_glow;//Mathf.Lerp(m_ESun,Moon_glow,81*Time.deltaTime);
					if(Current_Time < (5+Shift_dawn)){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*Time.deltaTime);
						m_g = Mathf.Lerp(m_g,-0.999502108f,4*FinalSPEED);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = Moon_glow;
							m_g = -0.999502108f;
						}
					}else
					if(Current_Time < (9+Shift_dawn)){
						m_ESun = Mathf.Lerp(m_ESun,7.56f,4*Time.deltaTime);
						m_g = Mathf.Lerp(m_g,-0.999502108f,4*FinalSPEED);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 7.56f;
							m_g = -0.999502108f;
						}
					}else{
						m_ESun = Mathf.Lerp(m_ESun,7.56f,1*Time.deltaTime);
						m_g = Mathf.Lerp(m_g,-0.9502108f,4*FinalSPEED);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 7.56f;
							m_g = -0.9502108f;
						}
					}
				}else{
					if(Current_Time > (16.1f+Shift_dawn)){//DUSK
						m_ESun = Mathf.Lerp(m_ESun,7.56f,5*Time.deltaTime);
						m_g = Mathf.Lerp(m_g,-0.9502108f,4*FinalSPEED);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 7.56f;
							m_g = -0.9502108f;
						}
					}else
					if(Current_Time > (22.1+Shift_dawn)){//TO NIGHT
						m_ESun = Mathf.Lerp(m_ESun,0,5*Time.deltaTime);
						m_g = Mathf.Lerp(m_g,-0.999502108f,4*FinalSPEED);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 0;
							m_g = -0.999502108f;
						}
					}else{ //DAY
						m_ESun = Mathf.Lerp(m_ESun,70.56f,2*Time.deltaTime);
						m_g = Mathf.Lerp(m_g,-0.9502108f,4*FinalSPEED);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 70.56f;
							m_g = -0.9502108f;
						}
					}
				}
				m_Kr = 0.0045106587f;
				m_Km = 0.00069617826f;
				//		m_g = -0.9502108f;
				Sun_ring_factor=0;
				m_fSamples=1.37f;
				m_fRayleighScaleDepth = 0.04161f;
				m_fWaveLength = new Vector3(0.42f,0.35f,0.31f);
				scale_dif = 0.06f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}

				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
			}
			
			////// preset 5 - skydome
			
			if(Preset == 5){
				//m_fExposure =0.92f;
				//		m_fExposure =1.6f;
				//m_fExposure =2.8f; //v1.6.5 
				//m_ESun = 7.56f;
				//v1.7
				
				if(Current_Time >= (9+Shift_dawn) & Current_Time <=(22.4f+Shift_dawn)){
					
					m_fExposure =1.6f;
					
					if(Current_Time > (22.1f+Shift_dawn)){
						
						//m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , Time.deltaTime);
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,2.03f,FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 0f;
							//m_Kr =0.006510659f;
							m_fRayleighScaleDepth =2.03f;
							m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
							scale_dif = 0.06f;
						}
					}
					else
					if(Current_Time > (17.1f+Shift_dawn)){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						
						m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.008510659f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,2.03f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 1.66f;
							m_Kr =0.008510659f;
							m_fRayleighScaleDepth =2.03f;
							m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
							scale_dif = 0.06f;
						}
					}else{
						if(Current_Time < (10+Shift_dawn) ){//red dawn start
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.004510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,2.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.32f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.09f,FinalSPEED);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.004510659f;
								m_fRayleighScaleDepth =2.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.32f);
								scale_dif = 0.09f;
							}
						}else if(Current_Time < (11+Shift_dawn))//dawn pinkish
						{
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,2.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.45f,0.41f,0.39f),FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.16f,0.3f*FinalSPEED);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.005510659f;
								m_fRayleighScaleDepth =2.06f;
								m_fWaveLength = new Vector3(0.45f,0.41f,0.39f);
								scale_dif = 0.16f;
							}
						}
						else if(Current_Time < (16.1f+Shift_dawn)){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.003510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,2.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.003510659f;
								m_fRayleighScaleDepth =2.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.07f;
							}
						}
						else{//return from pinkish
							m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
							
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.008f,FinalSPEED*0.1f);//m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,2.0318f,FinalSPEED*0.1f);//m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,Time.deltaTime*SPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =2.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.16f;
							}
						}
					}
					
					//m_Kr = 0.0045106587f;
					//m_fRayleighScaleDepth = 0.04161f;
					
					//if(!Application.isPlaying){ m_ESun = 1.66f; }
					
					m_g = -0.9990211f;//m_g = -0.9502108f;
					//Sun_ring_factor=0;
					m_fSamples=3.02f;//m_fSamples=1.37f;
					
				}else{
					
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);
					
					if(Current_Time < (8.5f+Shift_dawn)){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
						
						float SpeedA = 4;
						m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
						m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
						m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
						scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
						m_fSamples = Mathf.Lerp(m_fSamples,3.02f,SpeedA*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,2.025f,SpeedA*FinalSPEED);
						m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
						
					}else{
						if(Current_Time > (23+Shift_dawn)){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED);
							if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
							
							float SpeedA = 1;
							m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
							m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
							m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
							m_fSamples = Mathf.Lerp(m_fSamples,3.02f,SpeedA*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,2.025f,SpeedA*FinalSPEED);
							m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
							
						}else{
							if(Current_Time > (22.4f+Shift_dawn)){
								m_ESun = 0;//Mathf.Lerp(m_ESun,0,2.1f*FinalSPEED);//fade out moon, so we can move shader to sun direction smoothly
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.2f*FinalSPEED);
							}
							if(!Application.isPlaying | !init_presets){ m_ESun = 0f; }
						}
					}
					
					//		m_Kr = 0.008510659f ;
					//m_fRayleighScaleDepth = 0.03f;
					//		m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED);
					//		m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED);
					//		scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED);
					
					if(!Application.isPlaying  | !init_presets){
						//m_ESun = 1.66f;
						//m_Kr =0.006510659f;
						m_fRayleighScaleDepth =2.03f;
						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
						scale_dif = 0.06f;
					}
				}
				
				//								if((Current_Time > 22.4 & Current_Time <=25) | (Current_Time >= 0 & Current_Time < 6)){
				//									//m_ESun = Moon_glow;//Mathf.Lerp(m_ESun,Moon_glow,81*FinalSPEED);
				//
				//								}else{
				//
				//								}
				//m_Kr = 0.0045106587f;
				m_Km = 0.0004f;//m_Km = 0.00069617826f;
				//			m_g = -0.9990211f;//m_g = -0.9502108f;
				//Sun_ring_factor=0;
				//			m_fSamples=0.02f;//m_fSamples=1.37f;
				//			m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
				//m_fRayleighScaleDepth = 0.04161f;
				//			scale_dif = 0.06f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}
				
				m_Coloration = 0.1f + SkyColorationOffset;
				Sun_ring_factor =0;

				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
			}
			
			///////// end preset 5
			
			if(Preset ==6 & 1==1){
				m_fExposure =3.35f;
				//m_fExposure =2.8f; //v1.6.5
				//m_ESun = 2.46f;
				//v1.7
				if((Current_Time > (22.4f+Shift_dawn) & Current_Time <=25) | (Current_Time >= 0 & Current_Time < (10+Shift_dawn))){
					//m_ESun =Moon_glow;
					m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*Time.deltaTime);
				}else{
					if(Current_Time > (22.1f+Shift_dawn)){
						m_ESun = Mathf.Lerp(m_ESun,0,5*Time.deltaTime);
					}else{
						m_ESun = Mathf.Lerp(m_ESun,2.46f,2*Time.deltaTime);
					}
				}
				m_Kr = 0.0055106587f;
				m_Km = 0.00069617826f;
				m_g = -0.960f;
				Sun_ring_factor=0;
				m_fSamples=0.98f;
				m_fRayleighScaleDepth = 0.08f;
				m_fWaveLength = new Vector3(0.42f,0.35f,0.31f);
				scale_dif = 0.06f;
				m_fOuterRadius = 10250;
				Glob_scale = 2.36f;
				m_Coloration = 0 + SkyColorationOffset;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}

				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
			}

			//Preset 7 - DUAL SUNS - v2.1
			if(Preset == 7){ // copy from Preset == 0
								
				//if(Current_Time >= (9 + Shift_dawn) & Current_Time <= (22.4f + Shift_dawn)  ){
				if(is_DayLight ){
					m_fExposure =1.6f;
					
					//if(Current_Time > (22.1f + Shift_dawn) ){
					if(is_after_22 ){
						
						//m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , Time.deltaTime);
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
						if(!Application.isPlaying | !init_presets){
							m_ESun = 0f;
							//m_Kr =0.006510659f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
							scale_dif = 0.06f;
						}
					}
					else
					//if(Current_Time > (17.1f + Shift_dawn) ){
					if(is_after_17 ){
						m_TintColor = Color.Lerp(m_TintColor,new Color(10f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);//v2.1
						
						m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.001510659f,0.5f*FinalSPEED*0.1f);//v2.1
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.44f,0.30f),FinalSPEED*0.1f);//v2.1
						scale_dif = Mathf.Lerp(scale_dif,0.08f,FinalSPEED*0.1f);//v2.1
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 1.66f;
							m_Kr =0.001510659f;//v2.1
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.42f,0.44f,0.30f);//v2.1
							scale_dif = 0.08f;//v2.1
						}
					}else{
						//if(Current_Time < (10+ Shift_dawn) ){//red dawn start							
						if(is_before_10){

								FinalSPEED = FinalSPEED * DawnAppearSpeed; //v3.1

							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.004510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.32f),FinalSPEED);													

							scale_dif = Mathf.Lerp(scale_dif,0.09f,FinalSPEED);
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.004510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.32f);
								scale_dif = 0.09f;
							}
						}else if(is_before_11)//if(Current_Time < (11 + Shift_dawn))//dawn pinkish
						{
							
							m_ESun = Mathf.Lerp(m_ESun,1.66f,2*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.45f,0.41f,0.39f),FinalSPEED*0.1f);

							scale_dif = Mathf.Lerp(scale_dif,0.16f,FinalSPEED*0.1f);						
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.005510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.45f,0.41f,0.39f);
								scale_dif = 0.16f;
							}
						}
						else if(is_before_16){//if(Current_Time < (16.1f + Shift_dawn) ){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.003510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.003510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.07f;
							}
						}
						else{//return from pinkish
							m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
							
							m_ESun = Mathf.Lerp(m_ESun,1.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.008f,FinalSPEED*0.1f);//m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.0318f,FinalSPEED*0.1f);//m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,Time.deltaTime*SPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.42f,0.34f,0.30f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.06f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 1.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
								scale_dif = 0.16f;
							}
						}
					}					
					//m_Kr = 0.0045106587f;
					//m_fRayleighScaleDepth = 0.04161f;					
					//if(!Application.isPlaying){ m_ESun = 1.66f; }					
					m_g = -0.9990211f;//m_g = -0.9502108f;
					//Sun_ring_factor=0;
					m_fSamples=0.02f;//m_fSamples=1.37f;

				}else{
					
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);
					
					//if(Current_Time < (8.5f +Shift_dawn) ){
					if(is_before_85){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
						
						float SpeedA = 4;
						
						if(!Application.isPlaying | !init_presets){
							SpeedA = 4000;
						}
						
						m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
						m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
						m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
						scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
						m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
						m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
						
					}else{
						//if(Current_Time > (23 +Shift_dawn) ){
						if(is_after_23 ){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED); //v2.0.1
							if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }
							
							float SpeedA = 1;
							
							if(!Application.isPlaying | !init_presets){
								SpeedA = 4000;
							}
							
							m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
							m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
							m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
							m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
							m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);
							
						}else{
							//if(Current_Time > (22.4f +Shift_dawn)){
							if(is_after_224){
								m_ESun = 0;//Mathf.Lerp(m_ESun,0,2.1f*FinalSPEED);//fade out moon, so we can move shader to sun direction smoothly
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.3f*FinalSPEED);//v2.0.1 - fast fade out to avoid point light on moon = 0.2 to 0.3
							}
							if(!Application.isPlaying | !init_presets){ m_ESun = 0f; }
						}
					}

					if(!Application.isPlaying  | !init_presets){
						//m_ESun = 1.66f;
						//m_Kr =0.006510659f;
						m_fRayleighScaleDepth =0.03f;
						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
						scale_dif = 0.06f;
					}
				}				

				//m_Kr = 0.0045106587f;
				m_Km = 0.0004f;//m_Km = 0.00069617826f;
				//			m_g = -0.9990211f;//m_g = -0.9502108f;
				//Sun_ring_factor=0;
				//			m_fSamples=0.02f;//m_fSamples=1.37f;
				//			m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
				//m_fRayleighScaleDepth = 0.04161f;
				//			scale_dif = 0.06f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}
				
				m_Coloration = 0.1f + SkyColorationOffset;
				Sun_ring_factor =0;

				if(skyboxMat.HasProperty("SunColor")){
					skyboxMat.SetColor("SunColor",  Color.white);
					skyboxMat.SetFloat("SunBlend",  66);
					skyboxMat.SetFloat("White_cutoff",  1.2f+White_cutoffOffset);
				}
			}

			//////// PRESET 8 - DAY TIME v3.0
			if(Preset == 8){

				//DEFINE DAY TIME
				//if(Current_Time >= (9 + Shift_dawn) & Current_Time <= (22.4f + Shift_dawn)  ){
				if(is_DayLight ){
					m_fExposure =0.3f;

					//MOVE TO NIGHT
					//if(Current_Time > (22.1f + Shift_dawn) ){
					if(is_after_22 ){
						
						//m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , Time.deltaTime);
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
						if(!Application.isPlaying | !init_presets){
							m_ESun = 0f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
							scale_dif = 0.0001f;
						}
					}
					else
					//if(Current_Time > (17.1f + Shift_dawn) ){
					if(is_after_17 ){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						
						m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.006510659f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 0.66f;
							m_Kr =0.008510659f;
							m_fRayleighScaleDepth =0.06f;
							m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
							scale_dif = 0.0001f;
						}
					}else{
						//if(Current_Time < (10+ Shift_dawn) ){//red dawn start
						if(is_before_10){

								FinalSPEED = FinalSPEED * DawnAppearSpeed; //v3.1

							m_ESun = Mathf.Lerp(m_ESun,0.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED);							

							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED);
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.004510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}else if(is_before_11)//if(Current_Time < (11 + Shift_dawn))//dawn pinkish
						{
							m_ESun = Mathf.Lerp(m_ESun,0.66f,2*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);

							scale_dif = Mathf.Lerp(scale_dif,0.0002f,FinalSPEED*0.1f);
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0002f;
							}
						}
						else if(is_before_16){//if(Current_Time < (16.1f + Shift_dawn) ){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.003510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}
						else{//return from pinkish
							m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
							
							m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.008f,FinalSPEED*0.1f);//m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.0618f,FinalSPEED*0.1f);//m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,Time.deltaTime*SPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}
					}
					m_g = -0.9990211f;
					m_fSamples=0.02f;


					//DEFINE NIGHT TIME - 22 to 9 in morning
				}else{					
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);

					//STILL NIGHT
					//if(Current_Time < (8.5f +Shift_dawn) ){
					if(is_before_85){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }						
						float SpeedA = 4;						
						if(!Application.isPlaying | !init_presets){
							SpeedA = 4000;
						}						
						m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
						m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
						m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
						scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
						m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
						m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);	

						//LERP TO NIGHT AND MORNING
					}else{
						//if(Current_Time > (23 +Shift_dawn) ){
						if(is_after_23 ){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED); //v2.0.1
							if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }							
							float SpeedA = 1;							
							if(!Application.isPlaying | !init_presets){
								SpeedA = 4000;
							}							
							m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
							m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
							m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
							m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
							m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);							
						}else{
							//if(Current_Time > (22.4f +Shift_dawn)){
							if(is_after_224){
								m_ESun = 0;
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.3f*FinalSPEED);//v2.0.1 - fast fade out to avoid point light on moon = 0.2 to 0.3
							}
							if(!Application.isPlaying | !init_presets){ m_ESun = 0f; }
						}
					}
					if(!Application.isPlaying  | !init_presets){
						m_fRayleighScaleDepth =0.03f;
						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
						scale_dif = 0.06f;
					}
				}
				m_Km = 0.0004f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}				
				m_Coloration = 0.1f + SkyColorationOffset;
				Sun_ring_factor =0;				
				if(skyboxMat.HasProperty("SunColor")){
					float SpeedA = 0.05f;							
					if(!Application.isPlaying | !init_presets){
						SpeedA = 4000;
					}
					//if(Current_Time > (19 +Shift_dawn)  & Current_Time < (23 +Shift_dawn)){
					if(is_duskNight){
						skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.red,0.55f*SpeedA*FinalSPEED));
						skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),21,7*SpeedA*FinalSPEED));
						skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),0.99f+White_cutoffOffset,7*SpeedA*FinalSPEED));
					}else{
						skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.white,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),66,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),1.2f+White_cutoffOffset,SpeedA*FinalSPEED));
//						skyboxMat.SetColor("SunColor",  Color.white);
//						skyboxMat.SetFloat("SunBlend",  66);
//						skyboxMat.SetFloat("White_cutoff",  1.2f);
					}
				}
			}
			//////// END PRESET 8
			/// 

			//////// PRESET 9 - DAY TIME v3.0 - Normal sun
			if(Preset == 9){
				
				//DEFINE DAY TIME

				m_Km = 0.0004f;
				//m_Coloration = 0.1f + SkyColorationOffset;

				//Debug.Log ("rot0=" + Rot_Sun_X + "Rot="+Previous_Rot_X);

				//if(Current_Time >= (9 + Shift_dawn) & Current_Time <= (22.4f + Shift_dawn)  ){
				if(is_DayLight ){
					
					m_fExposure =0.3f;
					m_fSamples=0.02f;
					m_g = -0.9990211f;

					//MOVE TO NIGHT
					//if(Current_Time > (22.1f + Shift_dawn) ){
					if(is_after_22){// && Previous_Rot_X > Rot_Sun_X){//v3.3
						
						//m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , Time.deltaTime);
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
						if(!Application.isPlaying | !init_presets){
							m_ESun = 0f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
							scale_dif = 0.0001f;
						}

						//Debug.Log ("rot1=" + Rot_Sun_X);
					}
					else
					//if(Current_Time > (17.1f + Shift_dawn) ){
					if(is_after_17 ){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						
						m_ESun = Mathf.Lerp(m_ESun,5f,1*FinalSPEED*0.1f);//6
						m_Kr =Mathf.Lerp(m_Kr,0.007510659f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.44f,0.37f,0.33f),FinalSPEED*0.1f);//44,37,33 //87
						scale_dif = Mathf.Lerp(scale_dif,0.07f,FinalSPEED*0.1f);

						//m_Coloration = Mathf.Lerp(m_Coloration,0.15f,FinalSPEED*FinalSPEED);	
						//m_fSamples= Mathf.Lerp(m_fSamples,0.01f,FinalSPEED*FinalSPEED);
						//m_g = Mathf.Lerp(m_g,-0.89f,FinalSPEED*FinalSPEED);
						//m_Km = Mathf.Lerp(m_Km,0.9f,FinalSPEED*FinalSPEED);

							//Debug.Log ("rot=" + Rot_Sun_X);

						if(!Application.isPlaying  | !init_presets){
							m_ESun = 5f;
							m_Kr =0.007510659f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.44f,0.37f,0.33f);
							scale_dif = 0.07f;

							//m_Coloration = 0.15f;	
							//m_fSamples=0.01f;
							//m_g = -0.89f;
							//m_Km = 0.9f;
						}

						//Debug.Log ("rot=" + Rot_Sun_X);

					}else{
						//if(Current_Time < (10+ Shift_dawn) ){//red dawn start

						//Debug.Log ("rot=" + Rot_Sun_X);

						if(is_before_10){
							
								FinalSPEED = FinalSPEED * DawnAppearSpeed; //v3.1

							m_ESun = Mathf.Lerp(m_ESun,0.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED);							
							
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED);
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.005510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}else if(is_before_11)//if(Current_Time < (11 + Shift_dawn))//dawn pinkish
						{
							m_ESun = Mathf.Lerp(m_ESun,0.66f,2*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							
							scale_dif = Mathf.Lerp(scale_dif,0.0002f,FinalSPEED*0.1f);
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0002f;
							}
						}
						else if(is_before_16){//if(Current_Time < (16.1f + Shift_dawn) ){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.003510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}
						else{//return from pinkish
							m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
							
							m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.008f,FinalSPEED*0.1f);//m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.0618f,FinalSPEED*0.1f);//m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,Time.deltaTime*SPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}
					}


					
					
					//DEFINE NIGHT TIME - 22 to 9 in morning
				}else{					
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);
					
					//STILL NIGHT
					//if(Current_Time < (8.6f +Shift_dawn) ){
					if(is_before_85){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }						
						float SpeedA = 4;						
						if(!Application.isPlaying | !init_presets){
							SpeedA = 4000;
						}						
						m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
						m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
						m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
						scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
						m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
						m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);	
						
						//LERP TO NIGHT AND MORNING
					}else{
						//if(Current_Time > (23 +Shift_dawn) ){
						if(is_after_23 ){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED); //v2.0.1
							if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }							
							float SpeedA = 1;							
							if(!Application.isPlaying | !init_presets){
								SpeedA = 4000;
							}							
							m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
							m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
							m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
							m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
							m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);							
						}else{
							//if(Current_Time > (22.4f +Shift_dawn)){
							if(is_after_224){
								m_ESun = 0;
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.3f*FinalSPEED);//v2.0.1 - fast fade out to avoid point light on moon = 0.2 to 0.3
							}
							if(!Application.isPlaying | !init_presets){ m_ESun = 0f; }
						}
					}
					if(!Application.isPlaying  | !init_presets){
						m_fRayleighScaleDepth =0.03f;
						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
						scale_dif = 0.06f;
					}
				}

				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}				
			//	m_Coloration = 0.1f;

				m_Coloration = 0.1f + SkyColorationOffset;

				Sun_ring_factor =0;				
				if(skyboxMat.HasProperty("SunColor")){
					float SpeedA = 0.05f;							
					if(!Application.isPlaying | !init_presets){
						SpeedA = 4000;
					}
					//if(Current_Time > (19 +Shift_dawn)  & Current_Time < (23 +Shift_dawn)){
					if(is_duskNight){
						//skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.red,0.55f*SpeedA*FinalSPEED));
						//skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),21,7*SpeedA*FinalSPEED));
						//skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),0.99f,7*SpeedA*FinalSPEED));
						skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.white,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),66,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),1.2f+White_cutoffOffset,SpeedA*FinalSPEED));
					}else{
						skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.white,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),66,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),1.2f+White_cutoffOffset,SpeedA*FinalSPEED));
						//						skyboxMat.SetColor("SunColor",  Color.white);
						//						skyboxMat.SetFloat("SunBlend",  66);
						//						skyboxMat.SetFloat("White_cutoff",  1.2f);
					}
				}
			}
			//////// END PRESET 9

			/////// PRESET 10

			//////// PRESET 11 - DAY TIME v3.0 FINAL
			if(Preset == 11){
				
				//DEFINE DAY TIME
				if( is_DayLight
					//TOD calcs - v3.0
//					( !AutoSunPosition && Current_Time >= (9 + Shift_dawn) & Current_Time <= (22.4f + Shift_dawn) ) 
//					|
//					(  AutoSunPosition && Rot_Sun_X > 0)
				   ){
					
					m_fExposure =0.41f;
					
					//MOVE TO NIGHT
//					if( (AutoSunPosition && Rot_Sun_X < 5  ) | (!AutoSunPosition && Current_Time > (22.1f + Shift_dawn) )){
					if(is_after_22){
						
						//m_TintColor =  Color.Lerp (m_TintColor, new Color(0.386f,0,0,0) , Time.deltaTime);
						m_TintColor = Color.Lerp(m_TintColor,new Color(41f/255f,12f/255f,21f/255f,255f/255f),FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.03f,FinalSPEED*0.1f);
						m_ESun = Mathf.Lerp(m_ESun,0,1*FinalSPEED*0.2f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
						if(!Application.isPlaying | !init_presets){
							m_ESun = 0f;
							m_fRayleighScaleDepth =0.03f;
							m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
							scale_dif = 0.0001f;
						}
					}
					else
					//if( (AutoSunPosition && Rot_Sun_X < 65  ) | (!AutoSunPosition && Current_Time > (17.1f + Shift_dawn) )){
					if(is_after_17){
						m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
						
						m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
						m_Kr =Mathf.Lerp(m_Kr,0.006510659f,0.5f*FinalSPEED*0.1f);
						m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
						scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
						if(!Application.isPlaying  | !init_presets){
							m_ESun = 0.66f;
							m_Kr =0.008510659f;
							m_fRayleighScaleDepth =0.06f;
							m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
							scale_dif = 0.0001f;
						}
					}else
					{
						//if( (AutoSunPosition && Rot_Sun_X < 10  ) | (!AutoSunPosition && Current_Time < (10+ Shift_dawn) )){//red dawn start
						if(is_before_10){
							
								FinalSPEED = FinalSPEED * DawnAppearSpeed; //v3.1

							m_ESun = Mathf.Lerp(m_ESun,0.66f,2*FinalSPEED);
							m_Kr =Mathf.Lerp(m_Kr,0.005510659f,FinalSPEED);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED);							
							
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED);
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.004510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}else if(is_before_11)//if( (AutoSunPosition && Rot_Sun_X < 15  ) | (!AutoSunPosition && Current_Time < (11 + Shift_dawn)))//dawn pinkish
						{
							m_ESun = Mathf.Lerp(m_ESun,0.66f,2*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							
							scale_dif = Mathf.Lerp(scale_dif,0.0002f,FinalSPEED*0.1f);
							
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0002f;
							}
						}
						else if(is_before_16){//if( (AutoSunPosition && Rot_Sun_X < 45  ) | (!AutoSunPosition && Current_Time < (16.1f + Shift_dawn) )){//return from pinkish
							m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,FinalSPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}
						else{//return from pinkish
							m_TintColor = Color.Lerp(m_TintColor,new Color(50f/255f,5f/255f,15f/255f,255f/255f),3*FinalSPEED);
							
							m_ESun = Mathf.Lerp(m_ESun,0.66f,1*FinalSPEED*0.1f);
							m_Kr =Mathf.Lerp(m_Kr,0.008f,FinalSPEED*0.1f);//m_Kr =Mathf.Lerp(m_Kr,0.006510659f,FinalSPEED*0.1f);
							m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.0618f,FinalSPEED*0.1f);//m_fRayleighScaleDepth =Mathf.Lerp(m_fRayleighScaleDepth,0.06f,Time.deltaTime*SPEED*0.1f);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.48f,0.32f,0.28f),FinalSPEED*0.1f);
							scale_dif = Mathf.Lerp(scale_dif,0.0001f,FinalSPEED*0.1f);
							if(!Application.isPlaying  | !init_presets){
								m_ESun = 0.66f;
								m_Kr =0.006510659f;
								m_fRayleighScaleDepth =0.06f;
								m_fWaveLength = new Vector3(0.48f,0.32f,0.28f);
								scale_dif = 0.0001f;
							}
						}
					}
					m_g = -0.9990211f;
					m_fSamples=0.02f;
					
					
					//DEFINE NIGHT TIME - 22 to 9 in morning
				}else{					
					m_TintColor =  Color.Lerp (m_TintColor, new Color(0,0,0,0) , FinalSPEED);
					
					//STILL NIGHT
					//if( (AutoSunPosition && Rot_Sun_X < 5  ) | (!AutoSunPosition && Current_Time < (8.5f +Shift_dawn) )){
					if(is_before_85){
						m_ESun = Mathf.Lerp(m_ESun,Moon_glow,4*FinalSPEED);
						if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }						
						float SpeedA = 4;						
						if(!Application.isPlaying | !init_presets){
							SpeedA = 4000;
						}						
						m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
						m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
						m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
						m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
						scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
						m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
						m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
						m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);	
						
						//LERP TO NIGHT AND MORNING
					}else{
						//if( (AutoSunPosition && Rot_Sun_X < 3  ) | (!AutoSunPosition && Current_Time > (23 +Shift_dawn) )){
						if(is_after_23){
							m_ESun = Mathf.Lerp(m_ESun,Moon_glow,0.1f*FinalSPEED); //v2.0.1
							if(!Application.isPlaying | !init_presets){ m_ESun = Moon_glow; }							
							float SpeedA = 1;							
							if(!Application.isPlaying | !init_presets){
								SpeedA = 4000;
							}							
							m_fExposure = Mathf.Lerp(m_fExposure,0.72f,SpeedA*FinalSPEED);
							m_fWaveLength = Vector3.Lerp(m_fWaveLength,new Vector3(0.312f,0.31f,0.31f),SpeedA*FinalSPEED);
							m_Kr = Mathf.Lerp(m_Kr,0.01851066f,SpeedA*FinalSPEED);
							m_g = Mathf.Lerp(m_g,-0.9950211f,SpeedA*FinalSPEED);
							scale_dif = Mathf.Lerp(scale_dif,0.07f,SpeedA*FinalSPEED);
							m_fSamples = Mathf.Lerp(m_fSamples,0.02f,SpeedA*FinalSPEED);
							m_fRayleighScaleDepth = Mathf.Lerp(m_fRayleighScaleDepth,0.025f,SpeedA*FinalSPEED);
							m_Coloration = Mathf.Lerp(m_Coloration,0.05f + SkyColorationOffset,SpeedA*FinalSPEED);							
						}else{
							//if( (AutoSunPosition && Rot_Sun_X < 5  ) | (!AutoSunPosition && Current_Time > (22.4f +Shift_dawn))){
							if(is_after_224){
								m_ESun = 0;
							}else{
								m_ESun = Mathf.Lerp(m_ESun,0,0.3f*FinalSPEED);//v2.0.1 - fast fade out to avoid point light on moon = 0.2 to 0.3
							}
							if(!Application.isPlaying | !init_presets){ m_ESun = 0f; }
						}
					}
					if(!Application.isPlaying  | !init_presets){
						m_fRayleighScaleDepth =0.03f;
						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
						scale_dif = 0.06f;
					}
				}
				m_Km = 0.0004f;
				m_fOuterRadius = 10250;
				if(RenderSettings.skybox != skyboxMat){
					RenderSettings.skybox = skyboxMat;
				}				
				m_Coloration = 0.1f + SkyColorationOffset;
				Sun_ring_factor =0;				
				if(skyboxMat.HasProperty("SunColor")){
					float SpeedA = 0.05f;							
					if(!Application.isPlaying | !init_presets){
						SpeedA = 4000;
					}
					//if( (AutoSunPosition && Rot_Sun_X < 15  ) | (!AutoSunPosition && Current_Time > (19 +Shift_dawn)  & Current_Time < (23 +Shift_dawn))){
					if(is_duskNight){
//						skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.red,0.55f*SpeedA*FinalSPEED));
//						skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),21,7*SpeedA*FinalSPEED));
//						skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),0.99f,7*SpeedA*FinalSPEED));
						skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.white,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),66,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),1.2f+White_cutoffOffset,SpeedA*FinalSPEED));
					}else{
						skyboxMat.SetColor("SunColor",  Color.Lerp(skyboxMat.GetColor("SunColor"),Color.white,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("SunBlend",  Mathf.Lerp(skyboxMat.GetFloat("SunBlend"),66,SpeedA*FinalSPEED));
						skyboxMat.SetFloat("White_cutoff",  Mathf.Lerp(skyboxMat.GetFloat("White_cutoff"),1.2f+White_cutoffOffset,SpeedA*FinalSPEED));
						//						skyboxMat.SetColor("SunColor",  Color.white);
						//						skyboxMat.SetFloat("SunBlend",  66);
						//						skyboxMat.SetFloat("White_cutoff",  1.2f);
					}
				}
			}
			//////// END PRESET 11
			/// 

			//v3.3e
			//if (UseGradients | VolCloudGradients) {

			float time_point = Current_Time / 24;

			//lat-lon case
			if(AutoSunPosition){
				float Sun_angle_360 = Rot_Sun_X;
				if (Rot_Sun_X - Previous_Rot_X < 0) {
					Sun_angle_360 = (MaxSunElevation*2) - Rot_Sun_X;
				}
				float Dawn_sun_degrees = 0;
				float Dusk_sun_degrees = (MaxSunElevation*2);//180;//(SunElevation*2);
				float Dawn_time = 7;
				float Dusk_time = 22;
				//float TEMP = 1 * ((Dusk_time - Dawn_time) / (Dusk_sun_degrees - Dawn_sun_degrees));
				//Debug.Log ("TEMP="+TEMP);
				time_point = (Sun_angle_360 * (Dusk_time - Dawn_time) / (Dusk_sun_degrees - Dawn_sun_degrees)) + Dawn_time; //()/(Dusk_time-Dawn_time);
				time_point = time_point / 24;

				if (time_point > 1) {
					time_point = 1;
				}
				if (time_point < 0) {
					time_point = 0;
				}
				//Debug.Log (Sun_angle_360 + " Time:" + time_point);
				if (!Application.isPlaying && Rot_Sun_X == Previous_Rot_X) {
					time_point = calcColorTime;
				}
			}

			calcColorTime = time_point;//Export for terrain fog use

			if (UseGradients || VolCloudGradients) { //v3.4.3

				if (VolCloudGradients) {
					Color TMP2 = VolCloudLitGrad.Evaluate (time_point);
					Color TMP3 = VolCloudShadGrad.Evaluate (time_point);
					Color TMP4 = VolCloudFogGrad.Evaluate (time_point);
					//lerp color
					if (Time.fixedTime > 1 && Application.isPlaying && !instaChangeSkyColor) { //v4.9.5
						//gradSkyColor = Color.Lerp (gradSkyColor,TMP1,SPEED*Time.deltaTime+0.1f);
						gradCloudLitColor = Color.Lerp (gradCloudLitColor, TMP2, SPEED * Time.deltaTime + 0.1f);
						gradCloudShadeColor = Color.Lerp (gradCloudShadeColor, TMP3, SPEED * Time.deltaTime + 0.1f);
						gradCloudFogColor = Color.Lerp (gradCloudFogColor, TMP4, SPEED * Time.deltaTime + 0.1f);
					} else {
						//gradSkyColor = TMP1;//export currenly used color
						gradCloudLitColor = TMP2;
						gradCloudShadeColor = TMP3;
						gradCloudFogColor = TMP4;
					}
				}



				if (UseGradients && SkyColorGrad != null) {

					m_TintColor = SkyTintGrad.Evaluate(time_point);
					Color TMP1 = SkyColorGrad.Evaluate(time_point);

				

					//Curve for sun instensity
					if (FexposureC != null) {
						m_fExposure = FexposureC.Evaluate (time_point) + 0.4f;
					}
					if (FscaleDiffC != null) {
						scale_dif = FscaleDiffC.Evaluate (time_point);
					}

					if (FSunGC != null) {
						m_g = -0.1f * FSunGC.Evaluate (time_point) - 0.9f;
					}
					if (FSunringC != null) {
						Sun_ring_factor = FSunringC.Evaluate (time_point);
					}

					//lerp color
					if(Time.fixedTime > 1 && Application.isPlaying && !instaChangeSkyColor) //v4.9.5
                    {
						gradSkyColor = Color.Lerp (gradSkyColor,TMP1,SPEED*Time.deltaTime+0.1f);

						//gradCloudLitColor = Color.Lerp (gradCloudLitColor,TMP2,SPEED*Time.deltaTime+0.1f);
						//gradCloudShadeColor = Color.Lerp (gradCloudShadeColor,TMP3,SPEED*Time.deltaTime+0.1f);
					}else{
						gradSkyColor = TMP1;//export currenly used color
						//gradCloudLitColor = TMP2;
						//gradCloudShadeColor = TMP3;
					}
					TMP1 = gradSkyColor;//assign the lerped

					CloudDomeL1Mat.SetVector ("_Color", TMP1*TMP1);
                    //float FogColorPow = 1.7f;FogWaterPow
                    if (gradAffectsFog) //v4.1d
                    {
                        if (currentWeatherName == Volume_Weather_types.HeavyStorm || currentWeatherName == Volume_Weather_types.HeavyStormDark)
                        {
                            RenderSettings.fogColor = new Color(Mathf.Pow(TMP1.r, FogColorPow), Mathf.Pow(TMP1.g, FogColorPow), Mathf.Pow(TMP1.b, FogColorPow)) * 0.55f;
                        }
                        else
                        {
                            RenderSettings.fogColor = new Color(Mathf.Pow(TMP1.r, FogColorPow), Mathf.Pow(TMP1.g, FogColorPow), Mathf.Pow(TMP1.b, FogColorPow));
                        }
                    }

//					if (water != null) {
//						WaterHandlerSM WaterHandler1 = water.GetComponent<WaterHandlerSM> ();
//						Color ReflColor = new Color (Mathf.Pow (TMP1.r, FogWaterPow), Mathf.Pow (TMP1.g, FogWaterPow), Mathf.Pow (TMP1.b, FogWaterPow));
//
//						if (!Application.isPlaying) {
//							WaterHandler1.oceanMat.SetColor ("_ReflectionColor", ReflColor);
//						}
//
//						WaterHandler1.OverrideReflectColor = true;
//						WaterHandler1.ReflectColor = ReflColor;
//
//						Color OceanBase = WaterHandler1.oceanMat.GetColor ("_BaseColor");
//						//Color Basefinal = OceanBase * TMP1 * 0.5f + 0.5f * OceanBase;
//						//WaterHandler.SetColor ("_BaseColor",new Color(Basefinal.r,Basefinal.g,Basefinal.b,OceanBase.a));
//						WaterHandler1.oceanMat.SetColor ("_BaseColor",new Color(TMP1.r * 0.5f,TMP1.g * 0.5f,TMP1.b * 0.5f,OceanBase.a));
//					}

					if (TMP1.r > 0.118f) {
						TMP1.r = 1.118f - TMP1.r;
					}
					if (TMP1.g > 0.118f) {
						TMP1.g = 1.118f - TMP1.g;
					}
					if (TMP1.b > 0.118f) {
						TMP1.b = 1.118f - TMP1.b;
					}

					if (currentWeatherName == Volume_Weather_types.HeavyStorm || currentWeatherName == Volume_Weather_types.HeavyStormDark) {						
						if (!Application.isPlaying) {
							RenderSettings.ambientIntensity = AmbientIntensity/2;
						}
					} else {
						m_fWaveLength = new Vector3 (TMP1.r, TMP1.g, TMP1.b);

						if (!Application.isPlaying) {
							//DynamicGI.UpdateEnvironment (); //v3.4.5
							RenderSettings.ambientIntensity = AmbientIntensity;
						}
					}
				}

				//Add cloud curves eval
				VcloudSunIntensity = IntensitySun.Evaluate (calcColorTime);
				VcloudLightDiff = IntensityDiff.Evaluate (calcColorTime);
				VcloudFog = IntensityFog.Evaluate (calcColorTime);
			}
			//v3.3e
//			if (UseGradients) {
//				if (SkyColorGrad != null) {
					//blend colors
//					m_TintColor = SkyTintGrad.Evaluate(Current_Time/24);
//					Color TMP1 = SkyColorGrad.Evaluate(Current_Time/24);
//
//					CloudDomeL1Mat.SetVector ("_Color", TMP1*TMP1);
//					//float FogColorPow = 1.7f;FogWaterPow
//					RenderSettings.fogColor = new Color(Mathf.Pow(TMP1.r,FogColorPow),Mathf.Pow(TMP1.g,FogColorPow),Mathf.Pow(TMP1.b,FogColorPow));
//
//					if (!Application.isPlaying) {
//						DynamicGI.UpdateEnvironment ();
//						RenderSettings.ambientIntensity = AmbientIntensity;
//					}
//					if (water != null) {
//						//Material WaterHandlerMat = water.GetComponent<WaterHandlerSM> ().oceanMat;
//						//WaterHandlerMat.SetColor ("_ReflectionColor", new Color(Mathf.Pow(TMP1.r,FogWaterPow),Mathf.Pow(TMP1.g,FogWaterPow),Mathf.Pow(TMP1.b,FogWaterPow)));
//						//Color OceanBase = WaterHandlerMat.GetColor ("_BaseColor");
//						//Color Basefinal = OceanBase * TMP1 * 0.5f + 0.5f * OceanBase;
//						//WaterHandlerMat.SetColor ("_BaseColor", new Color(Mathf.Clamp01(Basefinal.r),Mathf.Clamp01(Basefinal.g),Mathf.Clamp01(Basefinal.b)));
//					}
//
//					if (TMP1.r > 0.118f) {
//						TMP1.r = 1.118f - TMP1.r;
//					}
//					if (TMP1.g > 0.118f) {
//						TMP1.g = 1.118f - TMP1.g;
//					}
//					if (TMP1.b > 0.118f) {
//						TMP1.b = 1.118f - TMP1.b;
//					}
//					m_fWaveLength = new Vector3(TMP1.r,TMP1.g,TMP1.b);

//				}
//			}


			//editor moon handle
			//if(Current_Time >= (9+Shift_dawn) & Current_Time <=(22.4f+Shift_dawn)){
			if(is_DayLight){
				//if(!Application.isPlaying  | !init_presets){ //v3.3d
					//if(Current_Time < (16.1f+Shift_dawn) & Current_Time > (11f+Shift_dawn)){//return from pinkish

					//v3.3d
//					if(is_dayToDusk){
//						m_ESun = 1.66f;
//						m_Kr =0.003510659f;
//						m_fRayleighScaleDepth =0.06f;
//						m_fWaveLength = new Vector3(0.42f,0.34f,0.30f);
//						scale_dif = 0.07f;
//					}
				//}
			}else{
				if(!Application.isPlaying | !init_presets){
					m_TintColor = new Color(0,0,0,0);
					m_ESun = 0;
				}
			}

		}

	GameObject SkyCamOBJ;
	public GameObject Test_Cubemap; // reflective sphere to test
	private Camera SkyCam;
	Transform SkyCam_transform;
	Cubemap CubeTexture;
	public Material CUBE_Mat;

	public LayerMask SkyboxLayer;
		public bool ReflectSkybox = true;//v3.3b
		float lastReflectTime=0;
		public float ReflectEvery = 0.4f;
		public List<Material> AssignCubeMapMats = new List<Material>();
		public float ReflectCamfarClip  = 6700;

		public bool updateSkyAmbient = false;
		float lastAmbientUpdateTime;
		public float AmbientUpdateEvery = 5;
		public float AmbientIntensity = 1.3f;



		//v3.3c
		float last_mat_update;
		public float Update_mat_every = 0.1f;//v3.4.5a
		bool cut_off_main_cam = false;
		Vector3 prev_scene_cam_pos;
		bool init_scene=false;

		//v3.4.5a
		public bool LimitSunUpdateRate = false;
		float last_sun_update;
		public float Update_sun_every = 0.01f;

		//v3.4.3
		public void ApplyType0Scatter(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0;//0.37f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.13f;//-0.11f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset =0.005f; //-0.03f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -12f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 3f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 1005;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.ClearDayHorizon = 0.1f;
			SUNMASTERAH.VolShaderCloudsH.ClearDayCoverage = -0.22f;
			SUNMASTERAH.VolShaderCloudsH.CloudDensity = 0.0001f;

			Vector3 Eulers11 =SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles;
			SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (2,Eulers11.y,Eulers11.z);
		}


		public void ApplyType0Default(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.37f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.05f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.02f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = 0f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 3f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 1005;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.ClearDayHorizon = 0.28f;
			SUNMASTERAH.VolShaderCloudsH.ClearDayCoverage = -0.22f;
			SUNMASTERAH.VolShaderCloudsH.CloudDensity = 0.0001f;

			Vector3 Eulers11 =SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles;
			SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (6,Eulers11.y,Eulers11.z);
		}
		public void ApplyType0Default1(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.37f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.05f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.02f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -9f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 3f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 1005;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.ClearDayHorizon = 0.01f;
			SUNMASTERAH.VolShaderCloudsH.ClearDayCoverage = -0.22f;
			SUNMASTERAH.VolShaderCloudsH.CloudDensity = 0.00012f;

			Vector3 Eulers11 =SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles;
			SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (0,Eulers11.y,Eulers11.z);
		}
		public void ApplyType0Default2(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.7f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.04f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.02f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -3f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 2f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 992;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.ClearDayHorizon = 0.05f;
			SUNMASTERAH.VolShaderCloudsH.ClearDayCoverage = -0.19f;
			SUNMASTERAH.VolShaderCloudsH.CloudDensity = 0.0002f;

			Vector3 Eulers11 =SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles;
			SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (6,Eulers11.y,Eulers11.z);
		}
		public void ApplyType0Default3(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.3f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.22f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.15f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -9f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 3f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 1020;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.ClearDayHorizon = 0.01f;
			SUNMASTERAH.VolShaderCloudsH.ClearDayCoverage = -0.2f;
			SUNMASTERAH.VolShaderCloudsH.CloudDensity = 0.00015f;

			Vector3 Eulers11 =SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles;
			SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (0,Eulers11.y,Eulers11.z);
		}
		public void ApplyType0Default4(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.9f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.4f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.05f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -9.5f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 2.8f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 1030;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.ClearDayHorizon = 0.01f;
			SUNMASTERAH.VolShaderCloudsH.ClearDayCoverage = -0.22f;
			SUNMASTERAH.VolShaderCloudsH.CloudDensity = 0.00018f;

			Vector3 Eulers11 =SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles;
			SUNMASTERAH.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (0,Eulers11.y,Eulers11.z);
		}

		//DOME
		public void ApplyType1Default(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.6f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.0f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.0f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -0.0f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 1.4f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 1000;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.UpdateScatterShader = true;
			SUNMASTERAH.VolShaderCloudsH.fog_depth = 0.39f;
			SUNMASTERAH.VolShaderCloudsH.reileigh = 7.48f;
			SUNMASTERAH.VolShaderCloudsH.mieCoefficient = 0.01f;
			SUNMASTERAH.VolShaderCloudsH.mieDirectionalG = 0.41f;
			SUNMASTERAH.VolShaderCloudsH.ExposureBias = 0.04f;
			SUNMASTERAH.VolShaderCloudsH.K = new Vector3 (1, 0.5f, 0.5f);//new Vector3 (111, 1, 1);
		}
		public void ApplyType1Default1(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.6f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.0f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.0f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -0.0f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 1.4f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 1000;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;

			SUNMASTERAH.VolShaderCloudsH.UpdateScatterShader = false;
			SUNMASTERAH.VolShaderCloudsH.fog_depth = 0.39f;
			SUNMASTERAH.VolShaderCloudsH.reileigh = 7.48f;
			SUNMASTERAH.VolShaderCloudsH.mieCoefficient = 0.01f;
			SUNMASTERAH.VolShaderCloudsH.mieDirectionalG = 0.6f;
			SUNMASTERAH.VolShaderCloudsH.ExposureBias = 0.02f;
			SUNMASTERAH.VolShaderCloudsH.K = new Vector3 (111, 1f, 1f);//new Vector3 (111, 1, 1);
		}
		public void ApplyType1Default2(SkyMasterManager SUNMASTERAH){
			SUNMASTERAH.VolCloudTransp = 0.7f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.05f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = 0.04f;
			SUNMASTERAH.VolShaderCloudsH.IntensityFogOffset = -0.0f;

			SUNMASTERAH.VolShaderCloudsH.MultiQuadScale.y = 2.8f;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.x = 700;
			SUNMASTERAH.VolShaderCloudsH.MultiQuadHeights.y = 1;
			SUNMASTERAH.VolShaderCloudsH.ClearDayHorizon = 0.01f;

			SUNMASTERAH.VolShaderCloudsH.UpdateScatterShader = false;
			SUNMASTERAH.VolShaderCloudsH.fog_depth = 0.39f;
			SUNMASTERAH.VolShaderCloudsH.reileigh = 7.48f;
			SUNMASTERAH.VolShaderCloudsH.mieCoefficient = 0.01f;
			SUNMASTERAH.VolShaderCloudsH.mieDirectionalG = 0.6f;
			SUNMASTERAH.VolShaderCloudsH.ExposureBias = 0.02f;
			SUNMASTERAH.VolShaderCloudsH.K = new Vector3 (111, 1f, 1f);//new Vector3 (111, 1, 1);
		}
		public void ApplyCloudCurvesPresetA(){

			//v3.4.3
			IntensityDiff = new AnimationCurve (new Keyframe (0,0.4f), 
				new Keyframe (0.374f,0.292f), new Keyframe (0.602f,0.255f), new Keyframe (0.757f,0.278f), new Keyframe (0.798f,0.271f),
				//new Keyframe (0.869f,0.204f), new Keyframe (0.916f,0.232f), new Keyframe (0.944f,0.280f),
				new Keyframe (0.869f,0.204f), new Keyframe (0.9f,0.5f), new Keyframe (0.944f,0.280f),
				new Keyframe (1,0.4f));

			IntensityFog = new AnimationCurve (new Keyframe (0,5f), 
				new Keyframe (0.75f,10f), new Keyframe (0.88f,11f), new Keyframe (0.89f,10.58f), 
				new Keyframe (1,5f));

			IntensitySun = new AnimationCurve (new Keyframe (0,0f), 
				//new Keyframe (0.185f,0.127f), new Keyframe (0.668f,0.274f), new Keyframe (0.803f,0.365f),new Keyframe (0.827f,1.1f), new Keyframe (0.871f,0.666f),
				new Keyframe (0.185f,0.127f), new Keyframe (0.668f,0.274f), new Keyframe (0.803f,0.365f),new Keyframe (0.827f,0.65f), new Keyframe (0.9f,-0.1f),
				new Keyframe (1,0.0f));

		}
		public void ApplyCloudCurvesPresetB(){

			//v3.4.3
			IntensityDiff = new AnimationCurve (new Keyframe (0,0.4f), 
				new Keyframe (0.374f,0.293f), new Keyframe (0.6f,0.2766f), new Keyframe (0.757f,0.278f), new Keyframe (0.798f,0.2713f),
				//new Keyframe (0.869f,0.204f), new Keyframe (0.916f,0.232f), new Keyframe (0.944f,0.280f),
				new Keyframe (0.8495f,0.2752f), new Keyframe (0.887f,0.249f), new Keyframe (0.944f,0.280f),
				new Keyframe (1,0.4f));

			IntensityFog = new AnimationCurve (new Keyframe (0,5f), 
				new Keyframe (0.75f,10f), new Keyframe (0.883f,10.91f), 
				new Keyframe (1,5f));

			IntensitySun = new AnimationCurve (new Keyframe (0,0f), 
				//new Keyframe (0.185f,0.127f), new Keyframe (0.668f,0.274f), new Keyframe (0.803f,0.365f),new Keyframe (0.827f,1.1f), new Keyframe (0.871f,0.666f),
				new Keyframe (0.186f,0.148f), new Keyframe (0.71f,0.13f), new Keyframe (0.84f,0.30f),new Keyframe (0.9f,0.13f), 
				new Keyframe (1,0.0f));

		}
        public void setMoonShader(Transform CamPos, float dimMoon)
        {
            float Eclipse_factor = 0;
            if (AutoMoonLighting)
            {
                Vector3 Camera_to_moon = (MoonObj.transform.position - CamPos.position).normalized;
                Vector3 Moon_to_sun = (SunObj.transform.position - MoonObj.transform.position).normalized;
                Vector3 Normal = -CamPos.up;
                Vector3 NormalY = CamPos.right;
                float Angle_signed = Mathf.Atan2(Vector3.Dot(Normal, Vector3.Cross(Camera_to_moon, Moon_to_sun)), Vector3.Dot(Camera_to_moon, Moon_to_sun)) * Mathf.Rad2Deg;
                float Angle_signedY = Mathf.Atan2(Vector3.Dot(NormalY, Vector3.Cross(Camera_to_moon, Moon_to_sun)), Vector3.Dot(Camera_to_moon, Moon_to_sun)) * Mathf.Rad2Deg;
                //Debug.Log ("ANGLE=" + Vector3.Angle (Camera_to_moon, Moon_to_sun));
                //Debug.Log ("ANGLES=" + Angle_signed);
                //Debug.Log ("ANGLESY=" + Angle_signedY);

                Vector3 Camera_to_sun = (SunObj.transform.position - CamPos.position).normalized;
                float AngleA = Vector3.Angle(Camera_to_moon, Camera_to_sun);
                float AngleSign = 10;
                if (AngleA < 90)
                {
                    AngleSign = -1;
                }

                if (AngleA < 1.5f)
                { //0.83f) {
                    Eclipse_factor = -2000;
                    onEclipse = true;
                }
                else
                {
                    onEclipse = false;
                }
                //Debug.Log (AngleA);
                float DistBased = Eclipse_factor - 2200 + AngleSign * 20 * AngleA;

                MoonPhasesMat.SetVector("_SunDir", new Vector3(Angle_signed * 200, Angle_signedY * 350, DistBased));
                Color MoonCol = MoonPhasesMat.GetColor("_Color");
                MoonCol = new Color(MoonCol.r * dimMoon, MoonCol.g * dimMoon, MoonCol.b * dimMoon, 1);
                MoonPhasesMat.SetColor("_Color",  MoonColor);
            }
            else
            {
                float month1 = Current_Month - ((((int)((Current_Month + 0) / 12))) * 12);
                
                float day1 = Current_Day - ((((int)((Current_Day + 0) / days_per_month))) * days_per_month);

                float day2 = -30 + (day1) * 2;
                
                MoonPhasesMat.SetVector("_SunDir", new Vector3(day2 * 350 * 2 + 500 - ((1-dimMoon) * 90000), month1 * 350 * 2 + 500 -((1 - dimMoon) * 20000), day2 * 800 + ((1 - dimMoon) * 90000)));

                //v3.3 - set moon color
                Color MoonCol = MoonPhasesMat.GetColor("_Color");
                MoonCol = 0.01f*new Color(MoonCol.r * dimMoon * dimMoon * dimMoon, MoonCol.g * dimMoon * dimMoon * dimMoon, MoonCol.b * dimMoon * dimMoon * dimMoon, 1);
                MoonPhasesMat.SetColor("_Color", MoonColor);
            }

        }
    }

}