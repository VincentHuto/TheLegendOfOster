using UnityEngine;
using System.Collections;
using Artngame.GIPROXY;

namespace Artngame.SKYMASTER {
	public class SKYMASTER_API_5_0_URP : MonoBehaviour {

        //v5.0.2 - temperature based
        public bool tempBased = false;

        //v5.0
        public bool volumeClouds1 = true;
        public bool volumeClouds2 = false; //fly through clouds

        public connectSuntoVolumeCloudsURP volumeClouds2SCRIPT;

        public connectSuntoFullVolumeCloudsURP fullVolumeClouds2SCRIPT; // CloudScript volumeClouds2SCRIPT; //URP v0.1
        public WeatherScript WeatherPatternSCRIPT;

        public connectSuntoFullVolumeCloudsURP volumeClouds2SCRIPT_REFL; // CloudScript volumeClouds2SCRIPT_REFL; // Cloud script for reflection camera, fly through clouds  //URP v0.1
        //public WeatherScript WeatherPatternSCRIPT_REFL; // Weather pattern script for reflection camera, fly through clouds

#pragma warning disable 414

        public GameObject Volcano;
		public GameObject Caustics;

		public float min_height = -16f;
		bool cloudPresets = false;
		public bool EnablecloudPresets = false;
		public Texture2D cloudsAPic0;
		public Texture2D cloudsAPic1;
		public Texture2D cloudsAPic2;
		public Texture2D cloudsAPic3;
		public Texture2D cloudsAPic4;

		public Texture2D cloudsBPic0;
		public Texture2D cloudsBPic1;
		public Texture2D cloudsBPic2;

		public GameObject Terra1;
		public Material DropletsMat;

		public GameObject Abomb;
		float last_bomb_time=-8;
		public GameObject Rain2;
		public GameObject Bats;
		public GameObject Leaves;
		public GameObject Floor;
		public GameObject Floor_stripes;
		public GameObject Floor_collider;
		bool Special_effects=false;
		//bool Rain1_on;
		bool Rain2_on;
		bool Leaves_on;
		bool Bats_on;
		//bool Freeze_on;
//		bool Colorcorrect_on;
//		bool Contrast_on;
//		bool AntiAliasng_on; 


		void ApplyType0Scatter(){
			SUNMASTERAH.VolCloudTransp = 0;//0.37f;
			SUNMASTERAH.VolShaderCloudsH.IntensitySunOffset = -0.11f;
			SUNMASTERAH.VolShaderCloudsH.IntensityDiffOffset = -0.03f;
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


		void ApplyType0Default(){
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
		void ApplyType0Default1(){
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
		void ApplyType0Default2(){
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
		void ApplyType0Default3(){
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
		void ApplyType0Default4(){
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
		void ApplyType1Default(){
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
		void ApplyType1Default1(){
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
		void ApplyType1Default2(){
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
		void ApplyType1Default3(){

		}


		void Disable_all(){
			if(Leaves.activeInHierarchy){
				Leaves.SetActive(false);
			}
//			if(Freezer.activeInHierarchy){
//				Freezer.SetActive(false);
//			}
//			if(Freeze_POOL.activeInHierarchy){
//				Freeze_POOL.SetActive(false);
//			}
			if(Bats.activeInHierarchy){
				Bats.SetActive(false);
			}
//			if(Rain1.activeInHierarchy){
//				Rain1.SetActive(false);
//			}
			if(Rain2.activeInHierarchy){
				Rain2.SetActive(false);
			}
//			if(Typhoon.activeInHierarchy){
//				Typhoon.SetActive(false);
//			}
//			if(ChainLightning.activeInHierarchy){
//				ChainLightning.SetActive(false);
//			}
		}


		public Transform PointLights;
		public Transform PointLightsCin;
		public Transform Camera1;
		public Transform Camera2;
		public Flare FlareTexture;

		//v3.4
		//public CloudHandlerSM CloudHandler;
        public CloudHandlerSM_SRP CloudHandler;

        public Transform TinyPlanet; 
		public Transform Pillars; 

		void Start () {

			if(SKYMASTER_OBJ!=null){
				SUNMASTER = SKYMASTER_OBJ.GetComponent(typeof(SkyMasterManager)) as SkyMasterManager;
			}
			SPEED = SUNMASTER.SPEED;
			SUNMASTER.Seasonal_change_auto = false;
			TOD = SUNMASTER.Current_Time;

			WaterHeightHandle = SUNMASTER.water.gameObject.GetComponent<WaterHeightSM>();
			WaterHandler = SUNMASTER.water.gameObject.GetComponent<WaterHandlerSM>();

			Dome_rot = SUNMASTER.Rot_Sun_Y;


			SUNMASTERAH = SUNMASTERA.GetComponent<SkyMasterManager> ();
			SUNMASTERBH = SUNMASTERB.GetComponent<SkyMasterManager> ();

//			if (SUNMASTER.currentWeather.VolumeCloud != null && SUNMASTER.currentWeather.VolumeCloud.activeInHierarchy) {
//				SUNMASTER.currentWeather.VolumeCloud.SetActive (false);
//			}

			if (CloudHandler.MultiQuadCClouds) {
				SUNMASTERAH.ApplyType0Scatter (SUNMASTERAH);
			}

		}

		public float Sun_time_start = 14.43f;	//at this time, rotation of sunsystem must be 62 (14, -1.525879e-05, -1.525879e-05 WORKS !!)
		public GameObject SKYMASTER_OBJ;
		SkyMasterManager SUNMASTER;


		public GameObject SUNMASTERA;
		public GameObject SUNMASTERB;
		public SkyMasterManager SUNMASTERAH;
		public SkyMasterManager SUNMASTERBH;

		public bool HUD_ON=true;
		bool set_sun_start=false;

		float Dome_rot = 0;
		float Camera_up;
		float TOD;
		float SPEED;
		WaterHeightSM WaterHeightHandle;
		WaterHandlerSM WaterHandler;
		int windowsON = 0;
		public Transform windowsSpot;
		public Transform underwaterSpot;
		public Transform AtollViewSpot;
		public Transform oceanSpot;
		public Transform boatSpot;
		public Transform boatSpot2;

		public Transform smokeSPOT;

		public GameObject farOceanplane;

		bool offsetsON = false;
		bool colorsON = false;

		public GameObject ParticleClouds;

		public GameObject ParticleCloudsA;
		public GameObject ParticleCloudsB;

		public GameObject skyA;
		public GameObject skyB;

		int VolCloud = 0;
		bool made_once = false;
		bool made_onceB = false;

		int activeSky = 0;
		int prevSky = 0;

		bool CinemaLook = false;

		bool Freezeme = false;

		//GlobalFogSkyMaster Fog;

		int limitRef=0;

        //5.0.2 - temprature based
        float humidity = 45;
        float temperature = 25;
        public float highTemperature = 10;
        public float HighHumidity = 55;
        bool enableCirrus = false;
        bool enableStratus = false; bool enable2DStratus = false;
        bool enableCumulus = false;
        bool changedWeather = false;
        float changedWeatherTime = 0;
        public float weatherChangeDelay = 14;

        void OnGUI() {

            //			if (VolCloud != 5 && SUNMASTER.currentWeather.VolumeCloud != null && SUNMASTER.currentWeather.VolumeCloud.activeInHierarchy) {
            //				SUNMASTER.currentWeather.VolumeCloud.SetActive (false);
            //			}

            float BOX_WIDTH = 100; float BOX_HEIGHT = 30;

            //5.0.2
            //if (tempBased)
            //{
            //    //DOME CONTROL		
            //    GUI.TextArea(new Rect(2 * (BOX_WIDTH + 10), 1 * BOX_HEIGHT, BOX_WIDTH + 0, 20), "SkyDome rot");
            //    Dome_rot = GUI.HorizontalSlider(new Rect(2 * (BOX_WIDTH + 10), 1 * BOX_HEIGHT + 25, BOX_WIDTH + 0, 30), Dome_rot, 0, 360);
            //    SUNMASTER.Rot_Sun_Y = Dome_rot;

            //    GUI.TextArea(new Rect(0 * BOX_WIDTH + 10, 6, BOX_WIDTH * 2, 22), "Weather Severity:" + SUNMASTER.WeatherSeverity);

            //    GUI.TextArea(new Rect(0 * BOX_WIDTH +10, BOX_HEIGHT , BOX_WIDTH * 1, 22), "Humidity:" + humidity);
            //    humidity = GUI.VerticalSlider(new Rect(0 * BOX_WIDTH +10, BOX_HEIGHT +30, BOX_WIDTH * 1, 132), humidity, 100, 0);

            //    GUI.TextArea(new Rect(1 * BOX_WIDTH + 10, BOX_HEIGHT, BOX_WIDTH * 1, 22), "Temperature:" + temperature);
            //    temperature = GUI.VerticalSlider(new Rect(1 * BOX_WIDTH + 10, BOX_HEIGHT + 30, BOX_WIDTH * 1, 132), temperature, 50, -20);

            //    GUI.TextArea(new Rect(2 * (BOX_WIDTH + 10), 3 * BOX_HEIGHT, BOX_WIDTH * 2, 20), "Time of Day");
            //    TOD = GUI.HorizontalSlider(new Rect(2 * (BOX_WIDTH + 10), 3 * BOX_HEIGHT + 25, BOX_WIDTH * 2, 20), SUNMASTER.Current_Time, 0f, 24);
            //    SUNMASTER.Current_Time = TOD;

            //    if (GUI.Button(new Rect(2, 8 * BOX_HEIGHT, BOX_WIDTH - 2, 22), "Status"))
            //    {
            //        if (enableStratus){  enableStratus = false;}
            //        else {enableStratus = true; enableCirrus = false; enableCumulus = false; }
            //    }
            //    //if (GUI.Button(new Rect(BOX_WIDTH, 8 * BOX_HEIGHT, BOX_WIDTH - 2, 22), "2D or 3D"))
            //    //{
            //    //    if (enable2DStratus) { enable2DStratus = false; }
            //   //     else { enable2DStratus = true; }
            //    //}
            //    if (GUI.Button(new Rect(2, 9 * BOX_HEIGHT, BOX_WIDTH - 2, 22), "Cumulus"))
            //    {
            //        if (enableCumulus) { enableCumulus = false; }
            //        else { enableCumulus = true; enableStratus = false; enableCirrus = false; }
            //    }
            //    if (GUI.Button(new Rect(2, 10 * BOX_HEIGHT, BOX_WIDTH - 2, 22), "Cirrus"))
            //    {
            //        if (enableCirrus) { enableCirrus = false; }
            //        else { enableCirrus = true; enableCumulus = false; enableStratus = false; }
            //    }

            //    //RESET weather change time
            //    if (changedWeather && Time.fixedTime - changedWeatherTime > weatherChangeDelay)
            //    {
            //        changedWeather = false;
            //    }

            //    //LOW
            //    if (enableStratus)
            //    {
            //        //Two options, 3D noise with low altitude and high density or 2D noise clouds
            //        if (enable2DStratus)
            //        {
            //            //DISABLE image effect clouds
            //            volumeClouds1 = false; //Disable image effect volume clouds
            //            SUNMASTER.volumeClouds.enabled = false;
            //            SUNMASTER.volumeClouds.reflectClouds.enabled = false;

            //            volumeClouds2 = false; //Disable image effect volume clouds, with fly through
            //            volumeClouds2SCRIPT.enabled = false;
            //            //Disable weather pattern maker for the clouds with fly through
            //            WeatherPatternSCRIPT.gameObject.SetActive(false);
            //            volumeClouds2SCRIPT_REFL.enabled = false;


            //            //ENABLE shader based clouds
            //            SUNMASTER.VolShaderClouds.gameObject.SetActive(true);
                        
            //            if (prevSky == 1)
            //            {
            //                //prevSky = 0;
            //            }
            //            else
            //            {
            //               // VolCloud++;
            //            }

            //            if (VolCloud > 4)
            //            { //was 5, remove particle clouds for now
            //                VolCloud = 0;
            //            }
            //            if (VolCloud == 0)
            //            {
            //                CloudHandler.DomeClouds = false;
            //                CloudHandler.MultiQuadClouds = true;
            //                CloudHandler.OneQuadClouds = false;
            //                CloudHandler.MultiQuadAClouds = false;
            //                CloudHandler.MultiQuadBClouds = false;
            //                CloudHandler.MultiQuadCClouds = false;
            //                SUNMASTERAH.ApplyType0Default(SUNMASTERAH);
            //            }
            //            if (VolCloud == 1)
            //            {
            //                CloudHandler.DomeClouds = true;
            //                CloudHandler.MultiQuadClouds = false;
            //                CloudHandler.OneQuadClouds = false;
            //                CloudHandler.MultiQuadAClouds = false;
            //                CloudHandler.MultiQuadBClouds = false;
            //                CloudHandler.MultiQuadCClouds = false;
            //                SUNMASTERAH.ApplyType1Default(SUNMASTERAH);
            //            }
            //            if (VolCloud == 2)
            //            {
            //                CloudHandler.DomeClouds = false;
            //                CloudHandler.MultiQuadClouds = false;
            //                CloudHandler.OneQuadClouds = true;
            //                CloudHandler.MultiQuadAClouds = false;
            //                CloudHandler.MultiQuadBClouds = false;
            //                CloudHandler.MultiQuadCClouds = false;
            //                SUNMASTERAH.ApplyType1Default(SUNMASTERAH);
            //            }
            //            if (VolCloud == 3)
            //            {
            //                CloudHandler.DomeClouds = false;
            //                CloudHandler.MultiQuadClouds = false;
            //                CloudHandler.OneQuadClouds = false;
            //                CloudHandler.MultiQuadAClouds = true;
            //                CloudHandler.MultiQuadBClouds = false;
            //                CloudHandler.MultiQuadCClouds = false;
            //                SUNMASTERAH.ApplyType1Default(SUNMASTERAH);
            //            }
            //            if (VolCloud == 4)
            //            {
            //                CloudHandler.DomeClouds = false;
            //                CloudHandler.MultiQuadClouds = false;
            //                CloudHandler.OneQuadClouds = false;
            //                CloudHandler.MultiQuadAClouds = false;
            //                CloudHandler.MultiQuadBClouds = true;
            //                CloudHandler.MultiQuadCClouds = false;
            //                SUNMASTERAH.ApplyType1Default(SUNMASTERAH);
            //            }
            //        }
            //        else
            //        {
            //            //ENABLE 3D CLOUDS
            //            volumeClouds1 = true; //enable image effect volume clouds
            //            SUNMASTER.volumeClouds.enabled = true;
            //            SUNMASTER.volumeClouds.reflectClouds.enabled = true;

            //            SUNMASTER.volumeClouds._Altitude0 = 3300;
            //            SUNMASTER.volumeClouds._Altitude1 = 4500;
            //            SUNMASTER.volumeClouds.contrast = 11;
            //            SUNMASTER.volumeClouds._InteractTextureAtr.x = 0.9f;
            //            SUNMASTER.volumeClouds._InteractTextureAtr.y = 0.999f;
            //            SUNMASTER.volumeClouds._NoiseAmp1 = 6.2f -0.8f + humidity * 0.01f;

            //            volumeClouds2 = false; //Disable image effect volume clouds, with fly through
            //            volumeClouds2SCRIPT.enabled = false;
            //            //Disable weather pattern maker for the clouds with fly through
            //            WeatherPatternSCRIPT.gameObject.SetActive(false);
            //            volumeClouds2SCRIPT_REFL.enabled = false;

            //            //close shader based clouds
            //            CloudHandler.DomeClouds = false;
            //            CloudHandler.MultiQuadClouds = false;
            //            CloudHandler.OneQuadClouds = false;
            //            CloudHandler.MultiQuadAClouds = false;
            //            CloudHandler.MultiQuadBClouds = false;
            //            CloudHandler.MultiQuadCClouds = false;
            //            SUNMASTER.VolCloudGradients = false;
            //            SUNMASTER.VolShaderClouds.gameObject.SetActive(false);
            //        }

            //        if (humidity > HighHumidity && temperature > highTemperature) //RAIN
            //        {
            //            if (!changedWeather && SUNMASTER.currentWeatherName != SkyMasterManager.Volume_Weather_types.Rain) { //change only if not same
            //                SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Rain;
            //                changedWeather = true;
            //                changedWeatherTime = Time.fixedTime;
            //                Debug.Log("Changing weather to Rain");
            //            }

            //            SUNMASTER.WeatherSeverity = humidity - HighHumidity; //the more humidity, more rain
            //        }
            //        if (temperature < highTemperature) //SNOW
            //        {
            //            if (!changedWeather && SUNMASTER.currentWeatherName != SkyMasterManager.Volume_Weather_types.SnowStorm)
            //            {
            //                SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.SnowStorm;
            //                changedWeather = true;
            //                changedWeatherTime = Time.fixedTime;
            //                Debug.Log("Changing weather to SnowStorm");
            //            }
            //            SUNMASTER.WeatherSeverity = highTemperature - temperature; //the less temprarture, more snow
            //        }
            //        if (humidity < HighHumidity && temperature > highTemperature) //RAIN
            //        {
            //            if (!changedWeather && SUNMASTER.currentWeatherName != SkyMasterManager.Volume_Weather_types.Cloudy)
            //            {
            //                SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Cloudy;
            //                changedWeather = true;
            //                changedWeatherTime = Time.fixedTime;
            //                Debug.Log("Changing weather to Cloudy");
            //            }
            //            SUNMASTER.WeatherSeverity = 0; //the more humidity, more rain
            //        }
            //    }
            //    //MIDDLE
            //    if (enableCumulus)
            //    {
            //        volumeClouds1 = false; //Disable image effect volume clouds
            //        SUNMASTER.volumeClouds.enabled = false;
            //        SUNMASTER.volumeClouds.reflectClouds.enabled = false;

            //        //ENABLE VOLUME CLOUDS WITH FLY THROUGH
            //        volumeClouds2 = true;  //enable image effect volume clouds, with fly through
            //        volumeClouds2SCRIPT.enabled = true;
            //        //enable weather pattern maker for the clouds with fly through
            //        volumeClouds2SCRIPT_REFL.enabled = true;
            //        WeatherPatternSCRIPT.gameObject.SetActive(true);


            //        //Disable shader based clouds
            //        CloudHandler.DomeClouds = false;
            //        CloudHandler.MultiQuadClouds = false;
            //        CloudHandler.OneQuadClouds = false;
            //        CloudHandler.MultiQuadAClouds = false;
            //        CloudHandler.MultiQuadBClouds = false;
            //        CloudHandler.MultiQuadCClouds = false;
            //        SUNMASTER.VolCloudGradients = false;
            //        SUNMASTER.VolShaderClouds.gameObject.SetActive(false);

            //        //lower cumulus, incsrease cirrus
            //        volumeClouds2SCRIPT.scale = 0.14f;
            //        volumeClouds2SCRIPT.coverage = 0.55f + humidity*0.008f;// 1.1f;
            //        volumeClouds2SCRIPT.coverageHigh = 0;

            //        if (temperature < highTemperature) //HEAVY or LIGHT RAIN based on severity
            //        {
            //            if (!changedWeather && SUNMASTER.currentWeatherName != SkyMasterManager.Volume_Weather_types.Rain)
            //            { //change only if not same
            //                SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Rain;
            //                changedWeather = true;
            //                changedWeatherTime = Time.fixedTime;
            //                Debug.Log("Changing weather to Heavy or Light Rain");
            //            }

            //            SUNMASTER.WeatherSeverity = humidity - HighHumidity; //the more humidity, more rain
            //        }
            //        else
            //        {
            //            if (!changedWeather && SUNMASTER.currentWeatherName != SkyMasterManager.Volume_Weather_types.Cloudy)
            //            {
            //                SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Cloudy;
            //                changedWeather = true;
            //                changedWeatherTime = Time.fixedTime;
            //                Debug.Log("Changing weather to Cloudy");
            //            }
            //            SUNMASTER.WeatherSeverity = 0; //the more humidity, more rain
            //        }
            //    }
            //    //HIGH
            //    if (enableCirrus)
            //    {
            //         volumeClouds1 = false; //Disable image effect volume clouds
            //        SUNMASTER.volumeClouds.enabled = false;
            //        SUNMASTER.volumeClouds.reflectClouds.enabled = false;

            //        //ENABLE VOLUME CLOUDS WITH FLY THROUGH
            //        volumeClouds2 = true;  //enable image effect volume clouds, with fly through
            //        volumeClouds2SCRIPT.enabled = true;
            //        //enable weather pattern maker for the clouds with fly through
            //        volumeClouds2SCRIPT_REFL.enabled = true;
            //        WeatherPatternSCRIPT.gameObject.SetActive(true);
                    

            //        //Disable shader based clouds
            //        CloudHandler.DomeClouds = false;
            //        CloudHandler.MultiQuadClouds = false;
            //        CloudHandler.OneQuadClouds = false;
            //        CloudHandler.MultiQuadAClouds = false;
            //        CloudHandler.MultiQuadBClouds = false;
            //        CloudHandler.MultiQuadCClouds = false;
            //        SUNMASTER.VolCloudGradients = false;
            //        SUNMASTER.VolShaderClouds.gameObject.SetActive(false);

            //        //lower cumulus, incsrease cirrus
            //        volumeClouds2SCRIPT.coverage = 0;
            //        volumeClouds2SCRIPT.coverageHigh = 3 * (humidity/100);
            //        volumeClouds2SCRIPT.highCloudsScale = 0.008f;

            //        //NO RAIN OR SNOW
            //        if (!changedWeather && SUNMASTER.currentWeatherName != SkyMasterManager.Volume_Weather_types.Cloudy)
            //        {
            //            SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Cloudy;
            //            changedWeather = true;
            //            changedWeatherTime = Time.fixedTime;
            //            Debug.Log("Changing weather to Cloudy");
            //        }
            //        SUNMASTER.WeatherSeverity = 0; //the more humidity, more rain
            //    }
            //    return;
            //}


			

			string HUD_gui = "Disable HUD";
			if(!HUD_ON){
				HUD_gui = "Enable HUD";
			}
			if (GUI.Button(new Rect(2, 0*BOX_HEIGHT, BOX_WIDTH-2, 22), HUD_gui)){				
				if(HUD_ON){
					HUD_ON = false;
				}else{
					HUD_ON = true;
				}				
			}

			float XOffset = 5;

			if(HUD_ON){	

				//WEATHER SEVERITY - //v5.0
				//GUI.TextArea( new Rect (5 * BOX_WIDTH + 20, BOX_HEIGHT + 60 + 30 +30+0-2, BOX_WIDTH, 21),"Weather level");
                //SUNMASTER.WeatherSeverity = GUI.HorizontalSlider(new Rect (5 * BOX_WIDTH + 20, BOX_HEIGHT + 60 + 30 +0+60-5-3, BOX_WIDTH, 30),SUNMASTER.WeatherSeverity,1,20);
                //if (GUI.Button(new Rect(3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + 31 + 31 + 31, BOX_WIDTH * 2, 32), "Lightning Storm"))
                GUI.TextArea(new Rect(3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + 31 + 31 + 31 + 31 + 31 + 11, BOX_WIDTH * 2, 22), "Weather intensity:" + SUNMASTER.WeatherSeverity);
                SUNMASTER.WeatherSeverity = GUI.HorizontalSlider(new Rect(3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + 31 + 31 + 31 + 31 + 31 + 31 + 11, BOX_WIDTH * 4, 32), SUNMASTER.WeatherSeverity, 1, 20);


                //v3.4.3
                GUI.TextArea( new Rect (6 * BOX_WIDTH + 20+10, BOX_HEIGHT + 60 + 30 +30+0-2, BOX_WIDTH+10, 21),"Sky brightness");
				//SUNMASTER.m_Coloration = GUI.HorizontalSlider(new Rect (6 * BOX_WIDTH + 20+10, BOX_HEIGHT + 60 + 30 +0+60-5-3, BOX_WIDTH+10, 30),SUNMASTER.m_Coloration,-0.4f,0.5f);
				SUNMASTER.SkyColorationOffset = GUI.HorizontalSlider(new Rect (6 * BOX_WIDTH + 20+10, BOX_HEIGHT + 60 + 30 +0+60-5-3, BOX_WIDTH+10, 30),SUNMASTER.SkyColorationOffset,-0.4f,0.5f);
				GUI.TextArea( new Rect (7 * BOX_WIDTH + 35+10, BOX_HEIGHT + 60 + 30 +30+0-2, BOX_WIDTH+10, 21),"Cloud brightness");
				CloudHandler.IntensityDiffOffset = GUI.HorizontalSlider(new Rect (7 * BOX_WIDTH + 35+10, BOX_HEIGHT + 60 + 30 +0+60-5-3, BOX_WIDTH+10, 30), CloudHandler.IntensityDiffOffset,-0.35f,0.35f);
				GUI.TextArea( new Rect (8 * BOX_WIDTH + 35+25, BOX_HEIGHT + 60 + 30 +30+0-2, BOX_WIDTH+15, 21),"Horizon brightness");
				SUNMASTER.FogColorPow = GUI.HorizontalSlider(new Rect (8 * BOX_WIDTH + 35+25, BOX_HEIGHT + 60 + 30 +0+60-5-3, BOX_WIDTH+15, 30),SUNMASTER.FogColorPow,-5,5);


				GUI.TextArea( new Rect (5 * BOX_WIDTH + 20, BOX_HEIGHT + 60 + 30 +0+0-12, BOX_WIDTH, 21),"Cloud Horizon");
                CloudHandler.ClearDayHorizon = GUI.HorizontalSlider(new Rect (5 * BOX_WIDTH + 20, BOX_HEIGHT + 60 + 0 +0+60-5-13, BOX_WIDTH, 30), CloudHandler.ClearDayHorizon,0,0.4f);

				//FREEZE
				string FreezeOnOff = "on";
				if (Freezeme) {
					FreezeOnOff = "off";
				}
				if (GUI.Button (new Rect (4 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 +0, BOX_WIDTH, 30), "Freeze "+FreezeOnOff)) {
					if (Freezeme) {
						Freezeme = false;
						//SUNMASTER.wate
						SUNMASTER.MaxWater = 0.4f;
						SUNMASTER.MaxRefract = 0;
					} else {
						Freezeme = true;
						SUNMASTER.MaxWater = 0.4f;
						SUNMASTER.MaxRefract = 5;
					}
				}
				if (Freezeme) {
					SUNMASTER.FreezeInwards = true;
					SUNMASTER.ScreenFreezeFX = true;
					SUNMASTER.FreezeSpeed = 2;
					GUI.TextArea( new Rect (4 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 +30+0-2, BOX_WIDTH, 21),"Freeze Amount");
					SUNMASTER.MaxRefract = GUI.HorizontalSlider(new Rect (4 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 +0+60-5-3, BOX_WIDTH, 30),SUNMASTER.MaxRefract,0,210f);
				} else {
					//SUNMASTER.FreezeInwards = false;
					SUNMASTER.ScreenFreezeFX = false;
					SUNMASTER.FreezeSpeed = 2;
					SUNMASTER.MaxRefract = 0;

				}
				//DropletsMat.set

				//ABOMB

				if (Time.fixedTime - last_bomb_time > 8) {
					if (GUI.Button (new Rect (3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30, BOX_WIDTH, 30), "Cast ABomb")) {
						GameObject NEWBOMB =  (GameObject)Instantiate (Abomb,  Camera.main.transform.position + Camera.main.transform.forward * 100, Quaternion.identity);
						NEWBOMB.SetActive (true);
						last_bomb_time = Time.fixedTime;
					}
				}

				//VOLCANO - Volcano
				if (GUI.Button (new Rect (3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30+31, BOX_WIDTH, 22), "Toggle Volcano")) {
					if (!Volcano.activeInHierarchy) {
						Volcano.SetActive (true);
					} else {
						Volcano.SetActive (false);
					}
				}

				//SPECIAL FX

				if (activeSky == 1) {
					string toggle_effects = "On";
					if (Special_effects) {
						toggle_effects = "Off";
					}

					//v1.7 - special effects
					if (GUI.Button (new Rect (3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + 30, BOX_WIDTH, 30), "Special FX " + toggle_effects)) {
						if (Special_effects) {
							Special_effects = false;

						} else {
							Special_effects = true;
							//Sky_effects = false;
						}
					}
					if (Special_effects) { // special effects GUI

//					//Anti Aliasing
//					string AntiAliasng_toggle = "On";
//					if(AntiAliasng_on){
//						AntiAliasng_toggle = "Off";
//					}
//					if (GUI.Button(new Rect(25+3*BOX_WIDTH+10, BOX_HEIGHT+60+30 +30, BOX_WIDTH, 20), "AntiAlising "+AntiAliasng_toggle)){
//						if(AntiAliasng_on){
//							AntiAliasng_on = false;
//							AntiAlising.enabled = false;
//						}else{
//							AntiAliasng_on = true;
//							AntiAlising.enabled = true;
//						}
//					}
//
//					//Color correct
//					string ColorCorrect_toggle = "On";
//					if(Colorcorrect_on){
//						ColorCorrect_toggle = "Off";
//					}
//					if (GUI.Button(new Rect(25+3*BOX_WIDTH+10, BOX_HEIGHT+60+30 +20+30, BOX_WIDTH, 20), "Color FX "+ColorCorrect_toggle)){
//						if(Colorcorrect_on){
//							Colorcorrect_on = false;
//							Colorizer.enabled = false;
//						}else{
//							Colorcorrect_on = true;
//							Colorizer.enabled = true;
//						}
//					}
//
//					//Contrast
//					string Contrast_toggle = "On";
//					if(Contrast_on){
//						Contrast_toggle = "Off";
//					}
//					if (GUI.Button(new Rect(25+3*BOX_WIDTH+10, BOX_HEIGHT+60+30 +(2*20)+30, BOX_WIDTH, 20), "Contrast "+Contrast_toggle)){
//						if(Contrast_on){
//							Contrast_on = false;
//							ContrastFilter.enabled = false;
//						}else{
//							Contrast_on = true;
//							ContrastFilter.enabled = true;
//						}
//					}

						//RAIN 1
//					string toggle_rain1 = "On";
//					if(Rain1_on){
//						toggle_rain1 = "Off";
//					}
//					if (GUI.Button(new Rect(25+3*BOX_WIDTH+10, BOX_HEIGHT+60+30 +(3*20)+30, BOX_WIDTH, 20), "Rain "+toggle_rain1)){
//						if(Rain1_on){
//							Rain1_on = false;
//							if(Rain1.activeInHierarchy){
//								Rain1.SetActive(false);
//							}
//						}else{
//							Rain1_on = true;
//							Disable_all();
//							if(!Rain1.activeInHierarchy){
//								Rain1.SetActive(true);
//							}
//
//						}
//					}

						//RAIN 2
						string toggle_rain2 = "On";
						if (Rain2_on) {
							toggle_rain2 = "Off";
						}
						if (GUI.Button (new Rect (25 + 3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + (2 * 20) + 30, BOX_WIDTH, 20), "Heavy Rain " + toggle_rain2)) {
							if (Rain2_on) {
								Rain2_on = false;
								if (Rain2.activeInHierarchy) {
									Rain2.SetActive (false);
								}
							} else {
								Rain2_on = true;
								Disable_all ();
								if (!Rain2.activeInHierarchy) {
									Rain2.SetActive (true);
								}

							}
						}
						//Leaves
						string toggle_leaves = "On";
						if (Leaves_on) {
							toggle_leaves = "Off";
						}
						if (GUI.Button (new Rect (25 + 3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + (3 * 20) + 30, BOX_WIDTH, 20), "Leaves " + toggle_leaves)) {
							if (Leaves_on) {
								Leaves_on = false;
								if (Leaves.activeInHierarchy) {
									Leaves.SetActive (false);
								}
							} else {
								Leaves_on = true;
								Disable_all ();
								if (!Leaves.activeInHierarchy) {
									Leaves.SetActive (true);
								}

							}
						}
						//Bats
						string toggle_bats = "On";
						if (Bats_on) {
							toggle_bats = "Off";
						}
						if (GUI.Button (new Rect (25 + 3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + (4 * 20) + 30, BOX_WIDTH, 20), "Bats " + toggle_bats)) {
							if (Bats_on) {
								Bats_on = false;
								if (Bats.activeInHierarchy) {
									Bats.SetActive (false);
								}
							} else {
								if (Floor.activeInHierarchy) {
									Floor.SetActive (false);
								}
								if (Floor_stripes.activeInHierarchy) {
									Floor_stripes.SetActive (false);
								}
								if (Floor_collider.activeInHierarchy) {
									Floor_collider.SetActive (false);
								}
								Bats_on = true;
								Disable_all ();
								if (!Bats.activeInHierarchy) {
									Bats.SetActive (true);

								}

							}
						}
						//Freeze
//					string toggle_ice_decals = "On";
//					if(Freeze_on){
//						toggle_ice_decals = "Off";
//					}
//					if (GUI.Button(new Rect(25+3*BOX_WIDTH+10, BOX_HEIGHT+60+30 +(7*20)+30, BOX_WIDTH, 20), "Ice decals "+toggle_ice_decals)){
//						if(Freeze_on){
//							Freeze_on = false;
//							if(Freezer.activeInHierarchy){
//								Freezer.SetActive(false);
//							}
//							if(Freeze_POOL.activeInHierarchy){
//								Freeze_POOL.SetActive(false);
//							}
//						}else{
//							Freeze_on = true;
//							Disable_all();
//							if(!Freezer.activeInHierarchy){
//								Freezer.SetActive(true);
//							}
//							if(!Freeze_POOL.activeInHierarchy){
//								Freeze_POOL.SetActive(true);
//							}
//						}
//					}

						//TYPHOON
//					string toggle_typhoon = "On";
//					if(Typhoon_on){
//						toggle_typhoon = "Off";
//					}
//					if (GUI.Button(new Rect(25+3*BOX_WIDTH+10, BOX_HEIGHT+60+30 +(8*20)+30, BOX_WIDTH, 20), "Tornados "+toggle_typhoon)){
//						if(Typhoon_on){
//							Typhoon_on = false;
//							if(Typhoon.activeInHierarchy){
//								Typhoon.SetActive(false);
//							}
//						}else{
//							Typhoon_on = true;
//							Disable_all();
//							if(!Typhoon.activeInHierarchy){
//								Typhoon.SetActive(true);
//							}
//
//						}
//					}

						//Chain Lightning
//					string toggle_chain = "On";
//					if(Chain_on){
//						toggle_chain = "Off";
//					}
//					if (GUI.Button(new Rect(25+3*BOX_WIDTH+10, BOX_HEIGHT+60+30 +(9*20)+30, BOX_WIDTH, 20), "Lightning "+toggle_chain)){
//						if(Chain_on){
//							Chain_on = false;
//							if(ChainLightning.activeInHierarchy){
//								ChainLightning.SetActive(false);
//							}
//						}else{
//							Chain_on = true;
//							Disable_all();
//							if(!ChainLightning.activeInHierarchy){
//								ChainLightning.SetActive(true);
//							}
//
//						}
//					}


					}// END Special EFFECTS handle GUI
					//v1.7 - special effects
					if (Special_effects) {
//					if(current_ground == 2 | Typhoon_on){ //if ocean or typhoon, remove floor
//						//Floor
//						if(Floor.activeInHierarchy){
//							Floor.SetActive(false);
//						}
//						if(Floor_stripes.activeInHierarchy){
//							Floor_stripes.SetActive(false);
//						}
//						if(!Floor_collider.activeInHierarchy){
//							Floor_collider.SetActive(true);
//						}
//					}else{
						if (!Bats.activeInHierarchy) {
							if (!Floor.activeInHierarchy) {
								Floor.SetActive (true);
							}
							if (!Floor_stripes.activeInHierarchy) {
								Floor_stripes.SetActive (true);
							}
							if (!Floor_collider.activeInHierarchy) {
								Floor_collider.SetActive (true);
							}
						}
						//}
					} else {
						if (Floor.activeInHierarchy) {
							Floor.SetActive (false);
						}
						if (Floor_stripes.activeInHierarchy) {
							Floor_stripes.SetActive (false);
						}
						if (Leaves.activeInHierarchy) {
							Leaves.SetActive (false);
						}
//					if(Freezer.activeInHierarchy){
//						Freezer.SetActive(false);
//					}
//					if(Freeze_POOL.activeInHierarchy){
//						Freeze_POOL.SetActive(false);
//					}
						if (Bats.activeInHierarchy) {
							Bats.SetActive (false);
						}
//					if(Rain1.activeInHierarchy){
//						Rain1.SetActive(false);
//					}
						if (Rain2.activeInHierarchy) {
							Rain2.SetActive (false);
						}
						if (Floor_collider.activeInHierarchy) {
							Floor_collider.SetActive (false);
						}
					}
				}


				//shadow control
				GUI.TextArea( new Rect (2, 13 * BOX_HEIGHT+4, BOX_WIDTH - 2 +0 , 22),"Shadow Dist.");
				QualitySettings.shadowDistance = GUI.HorizontalSlider(new Rect (2, 14 * BOX_HEIGHT, BOX_WIDTH - 2 +0 , 22),QualitySettings.shadowDistance,250,3500f);

				//Unity fog
				string fogOnOff = "on";
				if(!SUNMASTERAH.Use_fog){
					fogOnOff = "off";
				}
				if (GUI.Button (new Rect (2, 14 * BOX_HEIGHT+15, BOX_WIDTH - 2 + 2, 22), "Unity fog "+fogOnOff)) {		
					if (SUNMASTERAH.Use_fog) {
						SUNMASTERAH.Use_fog = false;
					} else {
						SUNMASTERAH.Use_fog = true;
					}
				}

				//casutics intensity
				if (CinemaLook) {
					GUI.TextArea (new Rect (2, 15 * BOX_HEIGHT + 10, BOX_WIDTH - 2 + 0, 22), "Caustic Bright.");
					WaterHandler.CausticIntensity = GUI.HorizontalSlider (new Rect (2, 16 * BOX_HEIGHT + 10, BOX_WIDTH - 2 + 0, 22), WaterHandler.CausticIntensity, 0, 30f);
				}

				//REFLECTION CONTROL
				string Refonoff = "Full Reflect";
				if (limitRef == 0) {					
					Refonoff = "Full Reflect";
				}
				if (limitRef == 1) {
					Refonoff = "Optimized Reflect";
				}
				if (limitRef == 2) {
					Refonoff = "No Reflections";
				}
				GUI.TextArea(new Rect (2, 12 * BOX_HEIGHT, BOX_WIDTH - 2 +2 , 22),Refonoff);
				if (GUI.Button (new Rect (2, 11 * BOX_HEIGHT, BOX_WIDTH - 2 +2 , 22), "Reflect Quality")) {					
					if (limitRef == 0) {	
						//limitRef = false;
						SUNMASTERA.GetComponent<SkyMasterManager> ().water.gameObject.GetComponent<PlanarReflectionSM> ().reflectionMask = 1 << LayerMask.NameToLayer("TerrainSM"); //half
						SUNMASTERB.GetComponent<SkyMasterManager> ().water.gameObject.GetComponent<PlanarReflectionSM> ().reflectionMask = 1 << LayerMask.NameToLayer("TerrainSM"); //half

						SUNMASTERA.GetComponent<SkyMasterManager> ().SkyboxLayer = (1 << LayerMask.NameToLayer("TerrainSM")) | (1 << LayerMask.NameToLayer("Water"));
						SUNMASTERB.GetComponent<SkyMasterManager> ().SkyboxLayer = (1 << LayerMask.NameToLayer("TerrainSM")) | (1 << LayerMask.NameToLayer("Water"));
					} else if (limitRef == 1) {
						//limitRef = true;
						SUNMASTERA.GetComponent<SkyMasterManager> ().water.gameObject.GetComponent<PlanarReflectionSM> ().reflectionMask = 0;
						SUNMASTERB.GetComponent<SkyMasterManager> ().water.gameObject.GetComponent<PlanarReflectionSM> ().reflectionMask = 0;

						SUNMASTERA.GetComponent<SkyMasterManager> ().SkyboxLayer = 0;
						SUNMASTERB.GetComponent<SkyMasterManager> ().SkyboxLayer = 0;
					}else if (limitRef == 2) {
						//limitRef = true;
						SUNMASTERA.GetComponent<SkyMasterManager> ().water.gameObject.GetComponent<PlanarReflectionSM> ().reflectionMask = -1;//full
						SUNMASTERB.GetComponent<SkyMasterManager> ().water.gameObject.GetComponent<PlanarReflectionSM> ().reflectionMask = -1;

						SUNMASTERA.GetComponent<SkyMasterManager> ().SkyboxLayer = -1;
						SUNMASTERB.GetComponent<SkyMasterManager> ().SkyboxLayer = -1;
					}
					limitRef++;
					if (limitRef > 2) {
						limitRef = 0;
					}

				}




			string onoff = "off";
			if (CinemaLook) {
				onoff = "on";
			}
			if (GUI.Button (new Rect (2, 10 * BOX_HEIGHT, BOX_WIDTH - 2, 22), "Cinematics "+onoff)) {	
				if (CinemaLook) {
					//UnderWaterImageEffect[] UW = Camera1.GetComponentsInChildren<UnderWaterImageEffect> ();
					//UW [UW.Length - 1].enabled = false;
					//UnderWaterImageEffect[] UW1 = Camera2.GetComponentsInChildren<UnderWaterImageEffect> ();
					//UW1 [UW1.Length - 1].enabled = false;

					if (!PointLights.gameObject.activeInHierarchy) {
						PointLights.gameObject.SetActive (true);
					}
					if (PointLightsCin.gameObject.activeInHierarchy) {
						PointLightsCin.gameObject.SetActive (false);
					}
					//SUNMASTER.SUN_LIGHT.GetComponent<Light> ().flare = null;
					SUNMASTERA.GetComponent<SkyMasterManager> ().SUN_LIGHT.GetComponent<Light>().flare = null;
					SUNMASTERB.GetComponent<SkyMasterManager> ().SUN_LIGHT.GetComponent<Light>().flare = null;
					CinemaLook = false;

						Caustics.SetActive (false);

				}else{
					//UnderWaterImageEffect[] UW = Camera1.GetComponentsInChildren<UnderWaterImageEffect> ();
					//UW [UW.Length - 1].enabled = true;
					//UnderWaterImageEffect[] UW1 = Camera2.GetComponentsInChildren<UnderWaterImageEffect> ();
					//UW1 [UW1.Length - 1].enabled = true;

					if (PointLights.gameObject.activeInHierarchy) {
						PointLights.gameObject.SetActive (false);
					}
					if (!PointLightsCin.gameObject.activeInHierarchy) {
						PointLightsCin.gameObject.SetActive (true);
					}
					//SUNMASTER.SUN_LIGHT.GetComponent<Light> ().flare = FlareTexture;
					SUNMASTERA.GetComponent<SkyMasterManager> ().SUN_LIGHT.GetComponent<Light>().flare = FlareTexture;
					SUNMASTERB.GetComponent<SkyMasterManager> ().SUN_LIGHT.GetComponent<Light>().flare = FlareTexture;
					CinemaLook = true;

						Caustics.SetActive (true);//v3.4.3
				}

			}


			if (GUI.Button (new Rect (2, 5 * BOX_HEIGHT, BOX_WIDTH - 2, 22), "Toggle sky")) {	
				if (activeSky == 0) {
					
					if(Camera.main != null){
						Camera.main.transform.parent = AtollViewSpot;
						Camera.main.transform.localPosition = new Vector3(0,0,0);
						Camera.main.transform.parent = skyA.transform;
						Camera.main.transform.up = Vector3.up;
						//						WaterHeightHandle.controlBoat = true;
						//						WaterHeightHandle.LerpMotion = true;
						//						windowsON = 3;
						//						WaterHandler.DisableUnderwater = false;
						//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Atoll;	
						//						WaterHandler.DisableUnderwater = true;
					}

					skyB.SetActive (true);
					skyA.SetActive (false);
					//ParticleClouds = ParticleCloudsB;
					SUNMASTER = SUNMASTERB.GetComponent<SkyMasterManager> ();
					activeSky = 1;

					SUNMASTER.water.GetComponent<GerstnerDisplaceSM> ().enabled = false;
					SUNMASTER.water.GetComponent<GerstnerDisplaceSM> ().enabled = true;

					//SUNMASTER.VolLightingH.SkyManager = SUNMASTER;

				} else {

					if (Camera.main != null) {
						Camera.main.transform.parent = AtollViewSpot;
						Camera.main.transform.localPosition = new Vector3 (0, 0, 0);
						Camera.main.transform.parent = skyB.transform;
						Camera.main.transform.up = Vector3.up;
					}

					skyA.SetActive (true);
					skyB.SetActive (false);
					//ParticleClouds = ParticleCloudsA;
					SUNMASTER = SUNMASTERA.GetComponent<SkyMasterManager> ();
					activeSky = 0;	

					SUNMASTER.water.GetComponent<GerstnerDisplaceSM> ().enabled = false;
					SUNMASTER.water.GetComponent<GerstnerDisplaceSM> ().enabled = true;

					//SUNMASTER.VolLightingH.SkyManager = SUNMASTER;

				}

				prevSky = 1;
			}


				//RAYCAST
				//if (activeSky == 1) {
				//	if (Fog == null) {
				//		Fog = Camera.main.GetComponent<GlobalFogSkyMaster> ();
				//	}
				//	if (Fog != null) {
				//		RaycastHit hit = new RaycastHit ();
				//		Vector3 Direct = Camera.main.transform.position - SUNMASTER.SunObj.transform.position;
				//		if(Physics.Raycast(SUNMASTER.SunObj.transform.position,Direct,out hit,Direct.magnitude)){
				//			Fog.mieDirectionalG = 0.833f;
				//			//Debug.Log (hit.collider.gameObject.name);
				//			Fog.contrast = 2.5f;
				//		}else{
				//			Fog.mieDirectionalG = 0.913f;
				//			Fog.contrast = 1.51f;
				//		}
				//	}
				//}

				GUI.TextArea( new Rect(2, 1*BOX_HEIGHT, 100-2, 20),"Sun Speed");
				SPEED = GUI.HorizontalSlider(new Rect(2, 1*BOX_HEIGHT+25, 100-2, 30),SPEED,0.01f,70f);
				SUNMASTER.SPEED = SPEED;

				GUI.TextArea( new Rect(2, 1*BOX_HEIGHT+50, 100-2, 20),"Sun Intensity");
				SUNMASTER.Max_sun_intensity = GUI.HorizontalSlider(new Rect(2, 1*BOX_HEIGHT+25+50, 100-2, 15),SUNMASTER.Max_sun_intensity,0.5f,2.50f);



				GUI.TextArea( new Rect(2, 4*BOX_HEIGHT+50+10, 100-2, 20),"Sun Beams");
	//			SUNMASTER.VolLightingH.heightRayleighIntensity = GUI.HorizontalSlider(new Rect(2, 4*BOX_HEIGHT+25+50+10, 100-2, 15),SUNMASTER.VolLightingH.heightRayleighIntensity,0.0f,0.7f);

				GUI.TextArea( new Rect(2, 5*BOX_HEIGHT+70+10, 100-2, 20),"Ambient");
				SUNMASTER.AmbientIntensity = GUI.HorizontalSlider(new Rect(2, 5*BOX_HEIGHT+25+70+10, 100-2, 15),SUNMASTER.AmbientIntensity,0.0f,1.8f);


				//CAMERA 
				if (windowsON == 0 | windowsON == 3) {
					GUI.TextArea (new Rect (6 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 25, BOX_WIDTH + 0, 20), "Camera height");
					//float min_height = 6f;
					Camera_up = GUI.HorizontalSlider (new Rect (6 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 25 + 25, BOX_WIDTH + 315, 30), Camera.main.transform.position.y, min_height, 1760);			
					Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera_up, Camera.main.transform.position.z);
				}

				//DOME CONTROL		
				GUI.TextArea( new Rect(6*(BOX_WIDTH+XOffset), 3*BOX_HEIGHT+10, BOX_WIDTH+0, 20),"Cloud coverage");

                //CONTROL CLOUD COVERAGE
                if (!volumeClouds1 && !volumeClouds2)
                { //v5.0 - if old clouds chosen, control the coverage per type
                    if (CloudHandler.DomeClouds)
                    {
                        CloudHandler.Coverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), CloudHandler.Coverage, -8.6f, 1.4f);
                    }
                    if (CloudHandler.MultiQuadClouds)
                    {
                        CloudHandler.Coverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), CloudHandler.Coverage, -0.55f, -0.05f);
                    }
                    if (CloudHandler.MultiQuadAClouds)
                    {
                        CloudHandler.Coverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), CloudHandler.Coverage, -0.55f, -0.05f);
                    }
                    if (CloudHandler.OneQuadClouds)
                    {
                        CloudHandler.Coverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), CloudHandler.Coverage, -1.25f, 1.05f);
                    }
                    if (CloudHandler.MultiQuadBClouds)
                    {
                        CloudHandler.Coverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), CloudHandler.Coverage, -0.55f, -0.05f);
                    }
                    if (CloudHandler.MultiQuadCClouds)
                    { //v3.4.3
                        CloudHandler.Coverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), CloudHandler.Coverage, -15.55f, 5f);
                    }
                }
                else {
                    //v5.0 - if new clouds chosen, control the coverage
                    //SUNMASTER.VolShaderCloudsH.ClearDayCoverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), SUNMASTER.VolShaderCloudsH.ClearDayCoverage, -8.6f, 1.4f);

                    if(volumeClouds1 && volumeClouds2SCRIPT != null) // && SUNMASTER.volumeClouds != null){                    
                    {
                        //Grab the voluem clouds reference from the sky master manager script
                        //SUNMASTER.volumeClouds._NoiseAmp1 = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), SUNMASTER.volumeClouds._NoiseAmp1, 1.55f, 15f);
                        volumeClouds2SCRIPT._NoiseAmp1 = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), volumeClouds2SCRIPT._NoiseAmp1, 1.55f, 15f);

                    }
                    if (volumeClouds2 && fullVolumeClouds2SCRIPT != null)
                    {
                        //Grab the voluem clouds reference from the sky master manager script
                        fullVolumeClouds2SCRIPT.coverage = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), fullVolumeClouds2SCRIPT.coverage, 0f, 2.6f);
                        //volumeClouds2SCRIPT_REFL.coverage = volumeClouds2SCRIPT.coverage;
                    }
                    if (volumeClouds1 && fullVolumeClouds2SCRIPT != null)
                    {
                        //Grab the voluem clouds reference from the sky master manager script
                        fullVolumeClouds2SCRIPT._NoiseAmp1 = GUI.HorizontalSlider(new Rect(6 * (BOX_WIDTH + XOffset), 3 * BOX_HEIGHT + 25 + 10, BOX_WIDTH * 4 + 15, 30), fullVolumeClouds2SCRIPT._NoiseAmp1, 1.55f, 15f);
                    }

                }

                if (Camera_up > 200) {
					if (farOceanplane.activeInHierarchy) {
						farOceanplane.SetActive (false);
					}
				} else {
					if (!farOceanplane.activeInHierarchy) {
						farOceanplane.SetActive (true);
					}
				}


				//DOME CONTROL		
				GUI.TextArea( new Rect(2*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH+0, 20),"SkyDome rot");			
				Dome_rot = GUI.HorizontalSlider(new Rect(2*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH+0, 30),Dome_rot,0,360);
				SUNMASTER.Rot_Sun_Y = Dome_rot;

				//DOME CONTROL		
				GUI.TextArea( new Rect(3*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH+0, 20),"Wind direction");			
				float aurlerY = GUI.HorizontalSlider(new Rect(3*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH+0, 30),SUNMASTER.windZone.transform.eulerAngles.y,0,360);
				Vector3 angles = SUNMASTER.windZone.gameObject.transform.eulerAngles;
				SUNMASTER.windZone.gameObject.transform.eulerAngles = new Vector3(angles.x,aurlerY,angles.z);

				//DOME CONTROL		
				GUI.TextArea( new Rect(4*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH+0, 20),"Wind intensity");			
				SUNMASTER.windZone.windMain  = GUI.HorizontalSlider(new Rect(4*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH+0, 30),SUNMASTER.windZone.windMain ,0,24);		

				//time of day
				if (activeSky == 0) {
					GUI.TextArea (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT, BOX_WIDTH, 20), "Time of Day");
					TOD = GUI.HorizontalSlider (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 25, BOX_WIDTH, 20), SUNMASTER.Current_Time, 0f, 24);
					SUNMASTER.Current_Time = TOD;
				}
				if (activeSky == 1) {
					GUI.TextArea (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT, BOX_WIDTH, 20), "Time of Day");
					TOD = GUI.HorizontalSlider (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 25, BOX_WIDTH, 20), SUNMASTER.Current_Time, 9.4f, 12);
					SUNMASTER.Current_Time = TOD;
				}

				//v3.4
				if (activeSky == 0) {
					if (GUI.Button (new Rect (2 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50, BOX_WIDTH * 2, 20), "Toggle Refect Probe")) {
						if (SUNMASTER.USE_SKYCUBE) {
							SUNMASTER.USE_SKYCUBE = false;
						} else {
							SUNMASTER.USE_SKYCUBE = true;
						}
					}
				}
				if (GUI.Button (new Rect (4 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50, BOX_WIDTH*2, 20), "Toggle Tiny Planet")) {
					if (!TinyPlanet.gameObject.activeInHierarchy) {
						TinyPlanet.gameObject.SetActive (true);
					} else {
						TinyPlanet.gameObject.SetActive (false);
					}
				}


				//v3.4.3
				if (VolCloud == 0) {
					if (GUI.Button (new Rect (2 * (BOX_WIDTH + XOffset), 2 * BOX_HEIGHT + 20+21, BOX_WIDTH * 2, 20), "Toggle Light Scatter")) {
						//toggle scatter clouds
						if (CloudHandler.MultiQuadClouds) {
							CloudHandler.DomeClouds = false;
							CloudHandler.MultiQuadClouds = false;
							CloudHandler.OneQuadClouds = false;
							CloudHandler.MultiQuadAClouds = false;
							CloudHandler.MultiQuadBClouds = false;
							CloudHandler.MultiQuadCClouds = true;

							SUNMASTERAH.ApplyType0Scatter (SUNMASTERAH);

						} else {
							CloudHandler.DomeClouds = false;
							CloudHandler.MultiQuadClouds = true;
							CloudHandler.OneQuadClouds = false;
							CloudHandler.MultiQuadAClouds = false;
							CloudHandler.MultiQuadBClouds = false;
							CloudHandler.MultiQuadCClouds = false;

							SUNMASTERAH.ApplyType0Default (SUNMASTERAH);
						}
					}
				}


				if (EnablecloudPresets) {
					if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20, BOX_WIDTH, 20), "Presets")) {

						if (!cloudPresets) {
							cloudPresets = true;
						} else {
							cloudPresets = false;
						}

					}
					if (cloudPresets) {
						if (VolCloud == 0 && CloudHandler.MultiQuadClouds) {
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20, BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsAPic0)) { 
								SUNMASTERAH.ApplyType0Default (SUNMASTERAH);
							}
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20 + 1 * (40 * 2 - 5), BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsAPic1)) { 
								SUNMASTERAH.ApplyType0Default1 (SUNMASTERAH);
							}
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20 + 2 * (40 * 2 - 5), BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsAPic2)) { 
								SUNMASTERAH.ApplyType0Default2 (SUNMASTERAH);
							}
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20 + 3 * (40 * 2 - 5), BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsAPic3)) { 
								SUNMASTERAH.ApplyType0Default3 (SUNMASTERAH);
							}
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20 + 4 * (40 * 2 - 5), BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsAPic4)) { 
								SUNMASTERAH.ApplyType0Default4 (SUNMASTERAH);
							}
						}
						if (VolCloud == 1) {
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20, BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsBPic0)) { 
								SUNMASTERAH.ApplyType1Default (SUNMASTERAH);
							}
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20 + 1 * (40 * 2 - 5), BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsBPic1)) { 
								SUNMASTERAH.ApplyType1Default1 (SUNMASTERAH);
							}
							if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 + 20 + 20 + 2 * (40 * 2 - 5), BOX_WIDTH * 2 + 5, 40 * 2 - 5), cloudsBPic2)) { 
								SUNMASTERAH.ApplyType1Default2 (SUNMASTERAH);
							}
						}
					}
				}

                // v5.0 VOLUMETRIC CLOUDS
                // THREE MAIN CLOUD TYPES
                // Image effect based VOLUME CLOUDS with fly through - use cloudScript script on camera, plus separate weather pattern maker script (WeatherSystem.cs)
                // Image effect based VOLUME CLOUDS without fly through - use fullVolumeCloudsSkyMaster script on camera, plus same script in Water reflection camera optionally (Water4Advanced(Clone)ReflectionMain Camera VOLUME CLOUDS)
                // Shader based VOLUME CLOUDS - Uses CloudHandlerSM control script on scene gameobjet and parented clouds layers using quads with the clouds shader
                //if (GUI.Button(new Rect(1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50, BOX_WIDTH, 20), "Toggle Clouds") || prevSky == 1)
                if (GUI.Button(new Rect(3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + 31 + 31, BOX_WIDTH*2, 32), "Toggle Volume Clouds"))
                {
                    volumeClouds1 = true; //enable image effect volume clouds
                    volumeClouds2 = false; //Disable image effect volume clouds, with fly through


                    if (fullVolumeClouds2SCRIPT != null) //if both clouds available
                    {
                        //SUNMASTER.volumeClouds.enabled = true;
                        //SUNMASTER.volumeClouds.reflectClouds.enabled = true;
                        //volumeClouds2SCRIPT.enabled = false;
                        fullVolumeClouds2SCRIPT.cloudChoice = 0;
                    }

                    if (fullVolumeClouds2SCRIPT != null) //if both clouds available
                    {
                        fullVolumeClouds2SCRIPT.enableFog = true;
                    }
                    if (volumeClouds2SCRIPT != null) //if both clouds available
                    {
                        volumeClouds2SCRIPT.enableFog = true;
                    }

                    //Disable weather pattern maker for the clouds with fly through
                    //WeatherPatternSCRIPT.gameObject.SetActive(false);
                    //volumeClouds2SCRIPT_REFL.enabled = false;

                    //close shader based clouds
                    CloudHandler.DomeClouds = false;
                    CloudHandler.MultiQuadClouds = false;
                    CloudHandler.OneQuadClouds = false;
                    CloudHandler.MultiQuadAClouds = false;
                    CloudHandler.MultiQuadBClouds = false;
                    CloudHandler.MultiQuadCClouds = false;
                    SUNMASTER.VolCloudGradients = false;
                    CloudHandler.gameObject.SetActive(false);
                }
                if (fullVolumeClouds2SCRIPT != null && GUI.Button(new Rect(3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + 31 + 31 + 31, BOX_WIDTH * 2, 32), "Toggle Fly Through Clouds"))
                {
                    volumeClouds1 = false; //Disable image effect volume clouds
                    //SUNMASTER.volumeClouds.enabled = false;
                    //SUNMASTER.volumeClouds.reflectClouds.enabled = false;

                    //ENABLE VOLUME CLOUDS WITH FLY THROUGH
                    volumeClouds2 = true;  //enable image effect volume clouds, with fly through
                                           // volumeClouds2SCRIPT.enabled = true;
                                           //enable weather pattern maker for the clouds with fly through
                                           // volumeClouds2SCRIPT_REFL.enabled = true;
                                           //WeatherPatternSCRIPT.gameObject.SetActive(true);

                    Camera.main.clearFlags = CameraClearFlags.Skybox;

                    //URP
                    fullVolumeClouds2SCRIPT.cloudChoice = 1;
                    fullVolumeClouds2SCRIPT.enableFog = true;

                    //Disable shader based clouds
                    CloudHandler.DomeClouds = false;
                    CloudHandler.MultiQuadClouds = false;
                    CloudHandler.OneQuadClouds = false;
                    CloudHandler.MultiQuadAClouds = false;
                    CloudHandler.MultiQuadBClouds = false;
                    CloudHandler.MultiQuadCClouds = false;
                    SUNMASTER.VolCloudGradients = false;
                    CloudHandler.gameObject.SetActive(false);
                }

                //v3.4
                if (GUI.Button (new Rect (1 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 50 , BOX_WIDTH, 20), "Toggle Clouds") || prevSky == 1) {

                    //DISABLE image effect clouds
                    volumeClouds1 = false; //Disable image effect volume clouds
                    volumeClouds2 = false; //Disable image effect volume clouds, with fly through

                    Camera.main.clearFlags = CameraClearFlags.Skybox;

                    //SUNMASTER.volumeClouds.enabled = false;
                    // SUNMASTER.volumeClouds.reflectClouds.enabled = false;
                    if (fullVolumeClouds2SCRIPT != null) //if both clouds available
                    {                        
                        fullVolumeClouds2SCRIPT.enableFog = false;
                    }
                    if (volumeClouds2SCRIPT != null) //if both clouds available
                    {
                        volumeClouds2SCRIPT.enableFog = false;
                    }

                    // volumeClouds2SCRIPT.enabled = false;
                    //Disable weather pattern maker for the clouds with fly through
                    //WeatherPatternSCRIPT.gameObject.SetActive(false);
                    //volumeClouds2SCRIPT_REFL.enabled = false;


                    //ENABLE shader based clouds
                    CloudHandler.gameObject.SetActive(true);
                                       


                    if (prevSky == 1) {
						prevSky = 0;
					} else {
						VolCloud++;
					}


					if (VolCloud > 4) { //was 5, remove particle clouds for now
						VolCloud = 0;
					}
					if (VolCloud == 0) {
						CloudHandler.DomeClouds = false;
						CloudHandler.MultiQuadClouds = true;
						CloudHandler.OneQuadClouds = false;
						CloudHandler.MultiQuadAClouds = false;
						CloudHandler.MultiQuadBClouds = false;
						CloudHandler.MultiQuadCClouds = false;
						SUNMASTERAH.ApplyType0Default (SUNMASTERAH);
					}
					if (VolCloud == 1) {
						CloudHandler.DomeClouds = true;
						CloudHandler.MultiQuadClouds = false;
						CloudHandler.OneQuadClouds = false;
						CloudHandler.MultiQuadAClouds = false;
						CloudHandler.MultiQuadBClouds = false;
						CloudHandler.MultiQuadCClouds = false;
						SUNMASTERAH.ApplyType1Default (SUNMASTERAH);
					}
					if (VolCloud == 2) {
						CloudHandler.DomeClouds = false;
						CloudHandler.MultiQuadClouds = false;
						CloudHandler.OneQuadClouds = true;
						CloudHandler.MultiQuadAClouds = false;
						CloudHandler.MultiQuadBClouds = false;
						CloudHandler.MultiQuadCClouds = false;
						SUNMASTERAH.ApplyType1Default (SUNMASTERAH);
					}
					if (VolCloud == 3) {
						CloudHandler.DomeClouds = false;
						CloudHandler.MultiQuadClouds = false;
						CloudHandler.OneQuadClouds = false;
						CloudHandler.MultiQuadAClouds = true;
						CloudHandler.MultiQuadBClouds = false;
						CloudHandler.MultiQuadCClouds = false;
						SUNMASTERAH.ApplyType1Default (SUNMASTERAH);
					}
					if (VolCloud == 4) {
						CloudHandler.DomeClouds = false;
						CloudHandler.MultiQuadClouds = false;
						CloudHandler.OneQuadClouds = false;
						CloudHandler.MultiQuadAClouds = false;
						CloudHandler.MultiQuadBClouds = true;
						CloudHandler.MultiQuadCClouds = false;
						SUNMASTERAH.ApplyType1Default (SUNMASTERAH);
					}
					if (VolCloud == 5) {
						CloudHandler.DomeClouds = false;
						CloudHandler.MultiQuadClouds = false;
						CloudHandler.OneQuadClouds = false;
						CloudHandler.MultiQuadAClouds = false;
						CloudHandler.MultiQuadBClouds = false;
						CloudHandler.MultiQuadCClouds = false;

//						if (SUNMASTER.DayClearVolumeClouds == null && SUNMASTER.currentWeather.VolumeCloud == null && !made_once) {
//							SUNMASTER.DayClearVolumeClouds = ParticleClouds;
//							SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Rain;
//							made_once = true;
//						}

						SUNMASTER.VolCloudGradients = false;

						if (activeSky == 0) {
							if (!made_once) {
								ParticleCloudsA.SetActive (true);
								//ParticleCloudsA.GetComponent<ParticleRenderer> ().enabled = true;
								//ParticleCloudsA.GetComponent<EllipsoidParticleEmitter> ().enabled = true;
								ParticleSystem PS = ParticleCloudsA.GetComponent<ParticleSystem> (); //v3.4.6
								if(PS != null){
									var emitModule = PS.emission;
									emitModule.enabled = true;
								}
								ParticleCloudsA.GetComponent<VolumeClouds_SM> ().enabled = true;

								//ParticleCloudsB.GetComponent<ParticleRenderer> ().enabled = false;
								//ParticleCloudsB.GetComponent<EllipsoidParticleEmitter> ().enabled = false;
								ParticleSystem PS1 = ParticleCloudsB.GetComponent<ParticleSystem> ();//v3.4.6
								if(PS1 != null){
									var emitModule1 = PS1.emission;
									emitModule1.enabled = false;
								}
								ParticleCloudsB.GetComponent<VolumeClouds_SM> ().enabled = false;
								made_once = true;
							} else {
								//ParticleCloudsA.GetComponent<ParticleRenderer> ().enabled = true;
								//ParticleCloudsA.GetComponent<EllipsoidParticleEmitter> ().enabled = true;
								ParticleSystem PS = ParticleCloudsA.GetComponent<ParticleSystem> (); //v3.4.6
								if(PS != null){
									var emitModule = PS.emission;
									emitModule.enabled = true;
								}
								ParticleCloudsA.GetComponent<VolumeClouds_SM> ().enabled = true;

								//ParticleCloudsB.GetComponent<ParticleRenderer> ().enabled = false;
								//ParticleCloudsB.GetComponent<EllipsoidParticleEmitter> ().enabled = false;
								ParticleSystem PS1 = ParticleCloudsB.GetComponent<ParticleSystem> (); //v3.4.6
								if(PS1 != null){
									var emitModule1 = PS1.emission;
									emitModule1.enabled = false;
								}
								ParticleCloudsB.GetComponent<VolumeClouds_SM> ().enabled = false;
							}

//						if (skyB && !ParticleClouds.activeInHierarchy) {
//							ParticleClouds.SetActive (true);
//						}

							ParticleCloudsA.GetComponent<VolumeClouds_SM> ().LightShaderModifier = 0.3f;
							ParticleCloudsA.GetComponent<VolumeClouds_SM> ().minLightShaderModifier = 0.3f;
							ParticleCloudsA.GetComponent<VolumeClouds_SM> ().GlowShaderModifier = 0.1f;
							ParticleCloudsA.GetComponent<VolumeClouds_SM> ().IntensityShaderModifier = 0.35f;
						}

						if (activeSky == 1) {
							if (!made_onceB) {
								ParticleCloudsB.SetActive (true);
								//ParticleCloudsB.GetComponent<ParticleRenderer> ().enabled = true;
								//ParticleCloudsB.GetComponent<EllipsoidParticleEmitter> ().enabled = true;
								ParticleSystem PS = ParticleCloudsB.GetComponent<ParticleSystem> (); //v3.4.6
								if(PS != null){
									var emitModule = PS.emission;
									emitModule.enabled = true;
								}
								ParticleCloudsB.GetComponent<VolumeClouds_SM> ().enabled = true;

								//ParticleCloudsA.GetComponent<ParticleRenderer> ().enabled = false;
								//ParticleCloudsA.GetComponent<EllipsoidParticleEmitter> ().enabled = false;
								ParticleSystem PS1 = ParticleCloudsA.GetComponent<ParticleSystem> (); //v3.4.6
								if(PS1 != null){
									var emitModule1 = PS1.emission;
									emitModule1.enabled = false;
								}
								ParticleCloudsA.GetComponent<VolumeClouds_SM> ().enabled = false;
								made_onceB = true;
							} else {
								//ParticleCloudsB.GetComponent<ParticleRenderer> ().enabled = true;
								//ParticleCloudsB.GetComponent<EllipsoidParticleEmitter> ().enabled = true;
								ParticleSystem PS = ParticleCloudsB.GetComponent<ParticleSystem> (); //v3.4.6
								if(PS != null){
									var emitModule = PS.emission;
									emitModule.enabled = true;
								}
								ParticleCloudsB.GetComponent<VolumeClouds_SM> ().enabled = true;

								//ParticleCloudsA.GetComponent<ParticleRenderer> ().enabled = false;
								//ParticleCloudsA.GetComponent<EllipsoidParticleEmitter> ().enabled = false;
								ParticleSystem PS1 = ParticleCloudsA.GetComponent<ParticleSystem> ();//v3.4.6
								if(PS1 != null){
									var emitModule1 = PS1.emission;
									emitModule1.enabled = false;
								}
								ParticleCloudsA.GetComponent<VolumeClouds_SM> ().enabled = false;
							}

							ParticleCloudsB.GetComponent<VolumeClouds_SM> ().LightShaderModifier = 0.3f;
							ParticleCloudsB.GetComponent<VolumeClouds_SM> ().minLightShaderModifier = 0.3f;
							ParticleCloudsB.GetComponent<VolumeClouds_SM> ().GlowShaderModifier = 0.1f;
							ParticleCloudsB.GetComponent<VolumeClouds_SM> ().IntensityShaderModifier = 0.35f;
						}


//						if (SUNMASTER.currentWeather != null && SUNMASTER.currentWeather.VolumeCloud != null ) {												
//							//SUNMASTER.currentWeather.VolumeCloud.SetActive (true);
//							SUNMASTER.currentWeather.VolumeCloud.GetComponent<ParticleRenderer> ().enabled = true;
//							SUNMASTER.currentWeather.VolumeCloud.GetComponent<EllipsoidParticleEmitter> ().enabled = true;
//							SUNMASTER.currentWeather.VolumeScript.enabled = true;
//						}

					} else {

						if (VolCloud != 5) {

							//if (skyA) {
							ParticleCloudsA.GetComponent<VolumeClouds_SM> ().enabled = false;
							//ParticleCloudsA.GetComponent<ParticleRenderer> ().enabled = false; //v3.4.6
							//ParticleCloudsA.GetComponent<EllipsoidParticleEmitter> ().enabled = false; //v3.4.6
							ParticleSystem PS = ParticleCloudsA.GetComponent<ParticleSystem> ();//v3.4.6
							if(PS != null){
								ParticleSystem.EmissionModule emitModule = PS.emission;
								emitModule.enabled = false;
							}
							//}
							//if (skyB) {
							ParticleCloudsB.GetComponent<VolumeClouds_SM> ().enabled = false;
							//ParticleCloudsB.GetComponent<ParticleRenderer> ().enabled = false; //v3.4.6
							//ParticleCloudsB.GetComponent<EllipsoidParticleEmitter> ().enabled = false; //v3.4.6
							ParticleSystem PS1 = ParticleCloudsB.GetComponent<ParticleSystem> (); //v3.4.6
							if(PS1 != null){
								ParticleSystem.EmissionModule emitModule1 = PS1.emission;
								emitModule1.enabled = false;
							}
							//}

							SUNMASTER.VolCloudGradients = true;
						}

//						if (VolCloud != 5) {
//							if (SUNMASTER.currentWeather != null && SUNMASTER.currentWeather.VolumeCloud != null ) {												
//								//SUNMASTER.currentWeather.VolumeCloud.SetActive (false);
//
//								SUNMASTER.currentWeather.VolumeScript.enabled = false;
//								SUNMASTER.currentWeather.VolumeCloud.GetComponent<ParticleRenderer> ().enabled = false;
//								SUNMASTER.currentWeather.VolumeCloud.GetComponent<EllipsoidParticleEmitter> ().enabled = false;
//								SUNMASTER.VolCloudGradients = true;
//							}
//							if (SUNMASTER.DayClearVolumeClouds != null) {
//								SUNMASTER.DayClearVolumeClouds = null;
//							}
//						}
					}

				}


//				if (SUNMASTER.currentWeather != null && SUNMASTER.currentWeather.VolumeCloud != null) {
//					SUNMASTER.currentWeather.VolumeScript.LightShaderModifier = 0.3f;
//					SUNMASTER.currentWeather.VolumeScript.minLightShaderModifier = 0.3f;
//					SUNMASTER.currentWeather.VolumeScript.GlowShaderModifier = 0.1f;
//					SUNMASTER.currentWeather.VolumeScript.IntensityShaderModifier = 0.35f;
//				}

				if ( VolCloud == 5 && SUNMASTER.windZone != null) {
					VolumeClouds_SM VolCloudScript = ParticleCloudsA.GetComponent<VolumeClouds_SM> ();
					VolCloudScript.Wind_holder = SUNMASTER.windZone.gameObject;
					VolCloudScript.wind = SUNMASTER.windZone.transform.forward * SUNMASTER.windZone.windMain * SUNMASTER.AmplifyWind *0.1f;//get current wind

					VolumeClouds_SM VolCloudScriptB = ParticleCloudsB.GetComponent<VolumeClouds_SM> ();
					VolCloudScriptB.Wind_holder = SUNMASTER.windZone.gameObject;
					VolCloudScriptB.wind = SUNMASTER.windZone.transform.forward * SUNMASTER.windZone.windMain * SUNMASTER.AmplifyWind*0.1f;//get current wind
				}


				//WAVE HEIGHT
				GUI.TextArea( new Rect(5*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH, 20),"Wave height");
				WaterHandler.waterScaleOffset.y = GUI.HorizontalSlider(new Rect(5*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.waterScaleOffset.y,0f,3);

				//BOAT SPEED
				GUI.TextArea( new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50, BOX_WIDTH, 20),"Boat Speed");
				WaterHeightHandle.BoatSpeed = GUI.HorizontalSlider(new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50, BOX_WIDTH, 20),WaterHeightHandle.BoatSpeed,0.3f,5);


				//EXTRA CONTROLS
				if(1==1){
					if (GUI.Button(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50, BOX_WIDTH, 20), "Reset Water")){
						WaterHandler.FresnelOffset = 0;
						WaterHandler.FresnelBias = 0;
						WaterHandler.BumpFocusOffset = 0;
						WaterHandler.DepthColorOffset = 0;
						WaterHandler.ShoreBlendOffset = 0;

						//v3.4.3
						WaterHandler.volumeFogDensityUnderwater = 50;
						WaterHandler.autoColorfogBrightness = 0.77f;
						WaterHandler.fogBias = 2;
						WaterHandler.GradTransp = 0.7f;
						WaterHandler.GradReflIntensity = 0.7f;
					}
					if (GUI.Button(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+25, BOX_WIDTH, 20), "Water Controls")){
						if(offsetsON){
							offsetsON = false;
						}else{
							offsetsON = true;
						}
					}
					if(offsetsON){
						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+1*50 + 2, BOX_WIDTH*1, 20),"Fresnel Power");
						WaterHandler.FresnelOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+1*50, BOX_WIDTH*3, 20),WaterHandler.FresnelOffset,-0.5f,1.7905f);//

						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+2*50, BOX_WIDTH*1, 20),"Fresnel Bias");
						WaterHandler.FresnelBias = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+2*50, BOX_WIDTH*3, 20),WaterHandler.FresnelBias,-130f,240);

						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+3*50, BOX_WIDTH*1, 20),"Specular focus");
						WaterHandler.BumpFocusOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+3*50, BOX_WIDTH*3, 20),WaterHandler.BumpFocusOffset,-4,4);//

						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+4*50, BOX_WIDTH*1, 20),"Depth Offset");
						WaterHandler.ShoreBlendOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+4*50, BOX_WIDTH*3, 20),WaterHandler.ShoreBlendOffset,-0.155f,0.1f);//-0.031f

						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+5*50, BOX_WIDTH*1, 20),"Depth FX");
						WaterHandler.DepthColorOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+5*50, BOX_WIDTH*3, 20),WaterHandler.DepthColorOffset,-140f,100);

						//v3.4.3
						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+6*50, BOX_WIDTH*2, 20),"Underwater Fog Density");
						WaterHandler.volumeFogDensityUnderwater = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+6*50, BOX_WIDTH*3, 20),WaterHandler.volumeFogDensityUnderwater,0,500);
						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+7*50, BOX_WIDTH*1, 20),"Fog Brightness");
						WaterHandler.autoColorfogBrightness = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+7*50, BOX_WIDTH*3, 20),WaterHandler.autoColorfogBrightness,-0.5f,2);
						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+8*50, BOX_WIDTH*1, 20),"Fog depth");
						WaterHandler.fogBias = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+8*50, BOX_WIDTH*3, 20),WaterHandler.fogBias,0,4);
						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+9*50, BOX_WIDTH*2, 20),"Transparency");
						WaterHandler.GradTransp = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+9*50, BOX_WIDTH*3, 20),WaterHandler.GradTransp,0f,1);
						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+10*50, BOX_WIDTH*1, 20),"Reflect power");
						WaterHandler.GradReflIntensity = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+10*50, BOX_WIDTH*3, 20),WaterHandler.GradReflIntensity,0f,1);

						//volume fog
						GUI.TextArea( new Rect(6*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+9*50, BOX_WIDTH*2, 20),"Volume fog Density");
						SUNMASTERAH.Mesh_Terrain_controller.fogDensity = GUI.HorizontalSlider(new Rect(6*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+9*50, BOX_WIDTH*3, 20),SUNMASTERAH.Mesh_Terrain_controller.fogDensity,0f,500);
						GUI.TextArea( new Rect(6*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+10*50, BOX_WIDTH*2, 20),"Volume fog Height");
						SUNMASTERAH.Mesh_Terrain_controller.AddFogHeightOffset = GUI.HorizontalSlider(new Rect(6*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+10*50, BOX_WIDTH*3, 20),SUNMASTERAH.Mesh_Terrain_controller.AddFogHeightOffset,-150f,200);
					}
				}

				if (GUI.Button(new Rect(11*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Smoke & FX")){
					if(smokeSPOT !=null){
						if(smokeSPOT.gameObject.activeInHierarchy){
							smokeSPOT.gameObject.SetActive(false);
						}else{
							smokeSPOT.gameObject.SetActive(true);
							WaterHandler.waterScaleOffset.y =0;
							WaterHandler.waterType = WaterHandlerSM.WaterPreset.Atoll;
						}		
					}
				}


				//BACK TO BOAT		
				if(windowsON==0){
					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Enter Cave")){
						if(Camera.main != null){
							Camera.main.transform.parent = windowsSpot;
							Camera.main.transform.forward = windowsSpot.forward;
							Camera.main.transform.localPosition = new Vector3(-0.5f,1.65f,-2.4f);
							//WaterHeightHandle.controlBoat = true;
							//WaterHeightHandle.LerpMotion = true;
							windowsON = 1;
							WaterHandler.waterType = WaterHandlerSM.WaterPreset.River;	

							// move water and boat to boatSpot2
							WaterHeightHandle.BoatSpeed = 0.3f;
							WaterHeightHandle.SampleCube.position = new Vector3(boatSpot2.position.x,WaterHeightHandle.SampleCube.position.y,boatSpot2.position.z);
							WaterHeightHandle.start_pos = new Vector3(boatSpot2.position.x,WaterHeightHandle.SampleCube.position.y,boatSpot2.position.z);
							WaterHandler.transform.position = new Vector3(boatSpot2.position.x,WaterHandler.transform.position.y,boatSpot2.position.z);

							WaterHandler.DisableUnderwater = true;
						}
					}
				}else if(windowsON==1){

					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Underwater")){
						if(Camera.main != null){
							Camera.main.transform.parent = underwaterSpot;
							Camera.main.transform.localPosition = new Vector3(0,0,0);
							WaterHeightHandle.controlBoat = true;
							WaterHeightHandle.LerpMotion = true;
							windowsON = 2;
							WaterHandler.DisableUnderwater = false;

							WaterHandler.DisableUnderwater = false;
						}
					}

				}else if(windowsON==2){

					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Atoll View")){
						if(Camera.main != null){
							Camera.main.transform.parent = AtollViewSpot;
							Camera.main.transform.localPosition = new Vector3(0,0,0);
							Camera.main.transform.up = Vector3.up;
							WaterHeightHandle.controlBoat = true;
							WaterHeightHandle.LerpMotion = true;
							windowsON = 3;
							WaterHandler.DisableUnderwater = false;
							WaterHandler.waterType = WaterHandlerSM.WaterPreset.Atoll;	

							WaterHandler.DisableUnderwater = true;

							Pillars.gameObject.SetActive (true);
						}
					}

				}else{
					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Board boat")){
						if(Camera.main != null){
							Camera.main.transform.parent = WaterHeightHandle.SampleCube;
							Camera.main.transform.forward = WaterHeightHandle.SampleCube.forward;
							Camera.main.transform.localPosition = new Vector3(-0.5f,2.25f,0.34f); //new Vector3(-0.5f,2.88f,-3.0f);
							WaterHeightHandle.controlBoat = true;
							WaterHeightHandle.LerpMotion = true;
							windowsON = 0;

							Pillars.gameObject.SetActive (false);

							WaterHeightHandle.start_pos = oceanSpot.position;
							WaterHeightHandle.SampleCube.position = boatSpot.position;
							WaterHandler.DisableUnderwater = false;
							WaterHandler.waterType = WaterHandlerSM.WaterPreset.River;	

							WaterHeightHandle.BoatSpeed = 1.2f;
							WaterHandler.transform.position = new Vector3(oceanSpot.position.x,WaterHandler.transform.position.y,oceanSpot.position.z);

							WaterHandler.DisableUnderwater = true;
						}
					}
				}

				//VOLUME WEATHER
				if (activeSky == 0) {
					if ((SUNMASTER.currentWeather != null && SUNMASTER.currentWeather.currentState != WeatherSM.Volume_Weather_State.FadeIn) | SUNMASTER.currentWeather == null) {
						if (GUI.Button (new Rect (2 * (BOX_WIDTH + XOffset), 0, BOX_WIDTH, 22), "Cloudy")) {
							SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Cloudy;

                            //v5.0
                            //if (SUNMASTER.volumeClouds != null)
                            //{
                            //    SUNMASTER.volumeClouds.EnableLightning = false;
                            //}
                            lightningCameraVolumeCloudsSM_URP lightningURP = Camera.main.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                            if (lightningURP != null)
                            {
                                lightningURP.EnableLightning = false;
                            }
                        }	
						if (GUI.Button (new Rect (3 * (BOX_WIDTH + XOffset), 0, BOX_WIDTH, 22), "Snow")) {
							SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.SnowStorm;

                            //v5.0
                            //if (SUNMASTER.volumeClouds != null)
                            //{
                            //    SUNMASTER.volumeClouds.EnableLightning = false;
                            //}
                            lightningCameraVolumeCloudsSM_URP lightningURP = Camera.main.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                            if (lightningURP != null)
                            {
                                lightningURP.EnableLightning = false;
                            }
                        }
						if (GUI.Button (new Rect (4 * (BOX_WIDTH + XOffset), 0, BOX_WIDTH, 22), "Heavy Storm")) {
							SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.HeavyStorm;

                            //v5.0
                            //if (SUNMASTER.volumeClouds != null)
                            //{
                            //    SUNMASTER.volumeClouds.EnableLightning = true;
                            //}
                            lightningCameraVolumeCloudsSM_URP lightningURP = Camera.main.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                            if (lightningURP != null)
                            {
                                lightningURP.EnableLightning = true;
                            }
                        }
						if (GUI.Button (new Rect (5 * (BOX_WIDTH + XOffset), 0, BOX_WIDTH, 22), "Rain")) {
							SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Rain;

                            //v5.0
                            //if (SUNMASTER.volumeClouds != null)
                            //{
                            //    SUNMASTER.volumeClouds.EnableLightning = false;
                            //}

                            lightningCameraVolumeCloudsSM_URP lightningURP = Camera.main.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                            if (lightningURP != null)
                            {
                                lightningURP.EnableLightning = false;
                            }
                        }

                        //v5.0
                        if (GUI.Button(new Rect(3 * BOX_WIDTH + 10, BOX_HEIGHT + 60 + 30 + 31 + 31 + 31 + 31, BOX_WIDTH * 2, 32), "Lightning Storm"))
                        {
                            SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.LightningStorm;

                            //v5.0
                            //if (SUNMASTER.volumeClouds != null)
                            //{
                            //    SUNMASTER.volumeClouds.EnableLightning = true;
                            //}

                            lightningCameraVolumeCloudsSM_URP lightningURP = Camera.main.GetComponent<lightningCameraVolumeCloudsSM_URP>();
                            if (lightningURP != null)
                            {
                                lightningURP.EnableLightning = true;
                            }
                        }


                    }
				}

				//TERRAIN EXTRA
				if (SUNMASTER.currentWeatherName == SkyMasterManager.Volume_Weather_types.SnowStorm) {
					if (!Terra1.activeInHierarchy) {
						Terra1.SetActive (true);
					}
				} else {
					if (Terra1.activeInHierarchy) {
						Terra1.SetActive (false);
					}
				}

				//WATER TYPE

				if(windowsON==2){

					if (GUI.Button(new Rect(7*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Turbulent")){
						WaterHandler.underWaterType = WaterHandlerSM.UnderWaterPreset.Turbulent;			
					}
					if (GUI.Button(new Rect(6*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Calm")){
						WaterHandler.underWaterType = WaterHandlerSM.UnderWaterPreset.Calm;			
					}

				}else{
					if (GUI.Button(new Rect(6*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "Caribbean")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Caribbean;			
					}
					if (GUI.Button(new Rect(6*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Lake")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Lake;			
					}
					if (GUI.Button(new Rect(10*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Atoll")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Atoll;			
					}
					if (GUI.Button(new Rect(9*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Dark Ocean")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.DarkOcean;			
						//WaterHandler.waterScaleOffset.y = 0.4f;
						if(Camera.main != null){
							WaterHeightHandle.SampleCube.position = oceanSpot.position;
							WaterHeightHandle.start_pos = oceanSpot.position;
							WaterHandler.DisableUnderwater = true;
							Camera.main.transform.localPosition = new Vector3(-0.5f,2.25f,0.34f);//new Vector3(-0.5f,2.88f,-3.0f);
						}
					}
					if (GUI.Button(new Rect(8*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Focus Ocean")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.FocusOcean;			
					}
					if (GUI.Button(new Rect(7*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Muddy Water")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Muddy;			
					}
					if (GUI.Button(new Rect(7*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "River")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.River;			
					}
					if (GUI.Button(new Rect(8*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "Small Waves")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.SmallWaves;			
					}
					if (GUI.Button(new Rect(9*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "Ocean")){
						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Ocean;

						if(Camera.main != null){
							WaterHeightHandle.SampleCube.position = oceanSpot.position;
							WaterHeightHandle.start_pos = oceanSpot.position;
							WaterHandler.DisableUnderwater = true;
							Camera.main.transform.localPosition = new Vector3(-0.5f,2.25f,0.34f);//new Vector3(-0.5f,2.88f,-3.0f);
						}
					}

					//					if(WaterHandler.waterType == WaterHandlerSM.WaterPreset.DarkOcean){
					//						//WaterHandler.waterScaleOffset.y = -6.4f;
					//						//WaterHandler.BelowWater = false;
					//					}

				}

				//WAVE HEIGHT
				GUI.TextArea( new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH, 20),"Water detail");
				WaterHandler.bumpTilingXoffset = GUI.HorizontalSlider(new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.bumpTilingXoffset,0.02f,0.3f);
				WaterHandler.bumpTilingYoffset = WaterHandler.bumpTilingXoffset; 

				GUI.TextArea( new Rect(11*(BOX_WIDTH+XOffset), 2*BOX_HEIGHT+20, BOX_WIDTH, 20),"Refraction");
				WaterHandler.RefractOffset = GUI.HorizontalSlider(new Rect(11*(BOX_WIDTH+XOffset), 2*BOX_HEIGHT+25+20, BOX_WIDTH, 20),WaterHandler.RefractOffset,-0.2f,2f);

				GUI.TextArea( new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH, 20),"Extra Waves");
				WaterHandler.ExtraWavesFactor.x = GUI.HorizontalSlider(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.ExtraWavesFactor.x,0f,4f);
				WaterHandler.ExtraWavesFactor.y = GUI.HorizontalSlider(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+25, BOX_WIDTH, 20),WaterHandler.ExtraWavesFactor.y,0f,1f);
				WaterHandler.ExtraWavesFactor.z = GUI.HorizontalSlider(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+25+25, BOX_WIDTH, 20),WaterHandler.ExtraWavesFactor.z,0f,1f);




				//ATOLL COLORS
				if (GUI.Button(new Rect(0*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+20+25, BOX_WIDTH, 20), "Atoll colors")){
					if(colorsON){
						colorsON = false;
					}else{
						colorsON = true;
					}
				}
				if(colorsON){
					GUI.TextArea( new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT, BOX_WIDTH+32, 20),"Base Color (RGBA)");
					WaterHandler.AtollWaterColor.r = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.r,0f,4f);
					WaterHandler.AtollWaterColor.g = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.g,0f,1f);
					WaterHandler.AtollWaterColor.b = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.b,0f,1f);
					WaterHandler.AtollWaterColor.a = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.a,0f,1f);

					GUI.TextArea( new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT, BOX_WIDTH+32, 20),"Reflect Color (RGBA)");
					WaterHandler.AtollReflectColor.r = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.r,0f,4f);
					WaterHandler.AtollReflectColor.g = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.g,0f,1f);
					WaterHandler.AtollReflectColor.b = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.b,0f,1f);
					WaterHandler.AtollReflectColor.a = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.a,0f,1f);
				}



				if(SPEED < 1){
					//SPEED =0;
				}

				if(SUNMASTER.Current_Time!=Sun_time_start & !set_sun_start){				
					set_sun_start=true;
				}








			}
		}// END OnGUI	

		float prev_ambient = 0;

		void LateUpdate(){

			if (prev_ambient != SUNMASTER.AmbientIntensity) {
				DynamicGI.UpdateEnvironment ();
				//lastAmbientUpdateTime = Time.fixedTime;
				//RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
				RenderSettings.ambientIntensity = SUNMASTER.AmbientIntensity;
				prev_ambient = SUNMASTER.AmbientIntensity;
				//Debug.Log ("a");
			}

//			if (VolCloud != 5) {
//				if ( SUNMASTER.currentWeather !=null && SUNMASTER.currentWeather.VolumeCloud != null && SUNMASTER.currentWeather.VolumeCloud.activeInHierarchy) {
//					//if (Time.fixedTime > 1.1f) {
//						SUNMASTER.currentWeather.VolumeCloud.SetActive (false);
//					//}
//					SUNMASTER.VolCloudGradients = true;
//				}
//			} else {
//				SUNMASTER.VolCloudGradients = false;
//				if (SUNMASTER.currentWeather !=null && SUNMASTER.currentWeather.VolumeCloud != null && !SUNMASTER.currentWeather.VolumeCloud.activeInHierarchy) {
//					SUNMASTER.currentWeather.VolumeCloud.SetActive (true);
//				}
//			}
		}

	}
}