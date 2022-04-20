using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artngame.GIPROXY;

namespace Artngame.SKYMASTER {

	[ExecuteInEditMode]
public class SkyMaster : MonoBehaviour {

		//v3.4.5
		public GameObject TMPCloudOBJ;

		//v3.4
		public bool AddSmTerrainMat = true;//avoid adding Sky Master snow enabled terrain material

		//v3.3e
		public bool UseTabs = true;//enable tab based presentation
		public int currentTab = 0;

		//v3.4.5
		public AnimationCurve IntensityDiff = new AnimationCurve();
		public AnimationCurve IntensityFog = new AnimationCurve();
		public AnimationCurve IntensitySun = new AnimationCurve();

		public AnimationCurve FexposureC   = new AnimationCurve();
		public AnimationCurve FscaleDiffC = new AnimationCurve();
		public AnimationCurve FSunGC = new AnimationCurve();
		public AnimationCurve FSunringC = new AnimationCurve();

		public AnimationCurve heightOffsetFogCurve = new AnimationCurve();
		[SerializeField]
		public AnimationCurve luminanceVFogCurve = new AnimationCurve();
		[SerializeField]
		public AnimationCurve lumFactorFogCurve = new AnimationCurve();
		[SerializeField]
		public AnimationCurve scatterFacFogCurve = new AnimationCurve();
		[SerializeField]
		public AnimationCurve turbidityFogCurve = new AnimationCurve();
		[SerializeField]
		public AnimationCurve turbFacFogCurve = new AnimationCurve();
		[SerializeField]
		public AnimationCurve horizonFogCurve = new AnimationCurve();
		[SerializeField]
		public AnimationCurve contrastFogCurve = new AnimationCurve();



		[SerializeField]
		public Gradient SkyColorGrad;
		[SerializeField]
		public Gradient SkyTintGrad;

		[SerializeField]
		public Gradient VolCloudLitGrad;
		[SerializeField]
		public Gradient VolCloudShadGrad;
		[SerializeField]
		public Gradient VolCloudFogGrad;

	//Global Sky master control script
		[SerializeField]
	public SkyMasterManager SkyManager;
	public SeasonalTerrainSKYMASTER TerrainManager;
	public WaterHandlerSM WaterManager;
	public LightCollisionsPDM GIProxyManager;
		public WaterHeightSM WaterHeightHandle;
	//public GameObject SkyDomeSystem;
	//public GameObject SunSystem;//instantiated system
	//public Transform MapCenter;

		public bool LargeWaterPlane = false;//use large plain instead of tiles
		public float LargeWaterPlaneScale = 1;

		//VOLUME CLOUD Performance controls
		public bool OverridePeformance = false;//override default performance settings of the prefab
		public bool DecoupleWind = true;
		public float UpdateInteraval = 0.2f;
		public int SpreadToFrames = 6;

		//Volume cloud renew
		public bool RenewClouds = false;
		public bool OverrideRenewSettings = false;//override default fade parameters
		public float FadeInSpeed = 0.15f;
		public float FadeOutSpeed = 0.3f;
		public float MaxFadeTime = 1.5f;
		public float Boundary = 90000;
		float lastFadeInSpeed;
		float lastFadeOutSpeed;
		float lastMaxFadeTime;
		float lastBoundary;

	//SCALING
	public float TerrainDepthSize = 0;
		public bool PreviewDepthTexture = false;
	//public float WorldScale=1;

	//TERRAIN SETUP
	public List<Terrain> UnityTerrains = new List<Terrain>();
	public List<GameObject> MeshTerrains = new List<GameObject>();//insert all here for shader setup, first ones will go to skymastermanager for seasonal-fog-shafts control


	public bool DontScaleParticleTranf = false;
	public bool DontScaleParticleProps = false;

		public bool SKY_folder = true; //v3.4.3
		public bool water_folder1 = true;
		public bool foliage_folder1 = true;
		public bool weather_folder1 = true;
		public bool cloud_folder1 = false;
		public bool cloud_folder2 = false;
        public bool cloud_folder3 = true;//v4.8
        public bool cloud_folder4 = false;
        public bool camera_folder1 = true;
		public bool terrain_folder1 = true;
		public bool scaler_folder1 = true;
		public bool scaler_folder11 = true;

	// Use this for initialization
	void Start () {
			if (this.gameObject.name == "GameObject") {
				this.gameObject.name = "SKY MASTER MANAGER";
			}
	}

        //v4.8
        public Material MeshTerrainSnowMat;
        public Material UnityTerrainSnowMat;

    // Update is called once per frame
        public void Update () {
            //if (OverridePeformance | RenewClouds) {

            //v4.8 - search for manager
            SeasonalTerrainSKYMASTER terrmanag = GetComponent<SeasonalTerrainSKYMASTER>();

            if(TerrainManager == null && terrmanag != null)
            {
                TerrainManager = terrmanag;
            }

            if (UnityTerrainSnowMat != null && MeshTerrainSnowMat != null)
            {
                TerrainManager.TerrainMat = UnityTerrainSnowMat;
                SkyManager.SnowMat = MeshTerrainSnowMat;
                SkyManager.SnowMatTerrain = UnityTerrainSnowMat;
            }

            //v4.8 - add terrain manager directly, not during terrain setup and plug it in skymaster object
            if (TerrainManager == null && SkyManager != null) {
                //script.SkyManager.Unity_terrain.gameObject.AddComponent<SeasonalTerrainSKYMASTER>();
                //script.TerrainManager = script.SkyManager.Unity_terrain.gameObject.GetComponent<SeasonalTerrainSKYMASTER>();
                //script.TerrainManager.TerrainMat = UnityTerrainSnowMat;
                SkyManager.gameObject.AddComponent<SeasonalTerrainSKYMASTER>();
                TerrainManager = SkyManager.gameObject.GetComponent<SeasonalTerrainSKYMASTER>();
                //TerrainManager.TerrainMat = UnityTerrainSnowMat;
                //UnityTerrainSnowMat = UnityTerrainSnowMat;
                //MeshTerrainSnowMat = MeshTerrainSnowMat;               

                TerrainManager.Mesh_moon = true;
                TerrainManager.Glow_moon = true;
                TerrainManager.Enable_trasition = true;
                TerrainManager.Fog_Sky_Update = true;
                TerrainManager.Foggy_Terrain = true;
                TerrainManager.Use_both_fogs = true;
                //script.TerrainManager.ImageEffectFog = true;
                //script.TerrainManager.ImageEffectShafts = true;
                TerrainManager.SkyManager = SkyManager;
            }

			if (Application.isPlaying && SkyManager != null) {

				bool Clouds_exist = false;
				if (SkyManager.currentWeather != null && SkyManager.currentWeather.VolumeScript != null) {
					Clouds_exist = true;
				}
				if (OverridePeformance) {
					if (Clouds_exist) {
						SkyManager.currentWeather.VolumeScript.DecoupledWind = DecoupleWind;
						SkyManager.currentWeather.VolumeScript.max_divider = SpreadToFrames;
						SkyManager.currentWeather.VolumeScript.UpdateInterval = UpdateInteraval;
					}
				}
				if (RenewClouds) {
					if (Clouds_exist) {
						SkyManager.currentWeather.VolumeScript.DestroyOnfadeOut = true;
						SkyManager.currentWeather.VolumeScript.Restore_on_bound = true;
						SkyManager.currentWeather.VolumeScript.Disable_on_bound = true;
						SkyManager.currentWeather.VolumeScript.SmoothInRate = FadeInSpeed;
						SkyManager.currentWeather.VolumeScript.SmoothOutRate = FadeOutSpeed;
						SkyManager.currentWeather.VolumeScript.max_fade_speed = MaxFadeTime;
						SkyManager.currentWeather.VolumeScript.Bound = Boundary;
					}
				} else {
					if (Clouds_exist) {

						//disable renew
						SkyManager.currentWeather.VolumeScript.DestroyOnfadeOut = false;
						SkyManager.currentWeather.VolumeScript.Restore_on_bound = false;
						SkyManager.currentWeather.VolumeScript.Disable_on_bound = false;

//					//reload previous values
//					SkyManager.currentWeather.VolumeScript.SmoothInRate = lastFadeInSpeed;
//					SkyManager.currentWeather.VolumeScript.SmoothOutRate = lastFadeOutSpeed;
//					SkyManager.currentWeather.VolumeScript.max_fade_speed = lastMaxFadeTime;
//					SkyManager.currentWeather.VolumeScript.Bound = lastBoundary;
//
//					//save parameters for restore
//					lastFadeInSpeed = SkyManager.currentWeather.VolumeScript.SmoothInRate;
//					lastFadeOutSpeed = SkyManager.currentWeather.VolumeScript.SmoothOutRate;
//					lastMaxFadeTime = SkyManager.currentWeather.VolumeScript.max_fade_speed;
//					lastBoundary = SkyManager.currentWeather.VolumeScript.Bound;
					}
				}
			
				//}
			}
	}



}
}