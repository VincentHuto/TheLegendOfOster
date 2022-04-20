using UnityEditor;
using UnityEditor.Macros;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artngame.GIPROXY;
using UnityEditorInternal;
using UnityEditor.SceneManagement; //v4.1e
using UnityEngine.SceneManagement; //v4.1e

namespace Artngame.SKYMASTER {

[CustomEditor(typeof(SkyMaster))] 	
public class SkyMasterInspector : Editor {

        //v4.8.7

		//v3.4.5a
		//public Object TMPCloudOBJ;

		//v3.4.5
		SerializedProperty IntensityDiff;
		SerializedProperty IntensityFog;
		SerializedProperty IntensitySun;
		SerializedProperty heightOffsetFogCurve;
		SerializedProperty luminanceVFogCurve;
		SerializedProperty lumFactorFogCurve;
		SerializedProperty scatterFacFogCurve;
		SerializedProperty turbidityFogCurve;
		SerializedProperty turbFacFogCurve;
		SerializedProperty horizonFogCurve;
		SerializedProperty contrastFogCurve;

		SerializedProperty FexposureC;
		SerializedProperty FscaleDiffC;
		SerializedProperty FSunGC;
		SerializedProperty FSunringC;

		//v3.4
		//VOL. CLOUDS
		public Object VolCloudsPREFAB;
		public Object VolLightingPREFAB;

		//v3.3e
		SerializedProperty SkyColorGrad;
		SerializedProperty SkyTintGrad;
		SerializedProperty VolCloudLitGrad;
		SerializedProperty VolCloudShadGrad;
		SerializedProperty VolCloudFogGrad;

		//DEPTH CAMERA for TERRAIN
		public Object DepthCameraPREFAB;
		//WaterHeightSM WaterHeightHandle;
		//public bool PreviewDepthTexture ;

		//FOLIAGE
		public Object snowMESH;

		//SPECIAL FX PRESETS
		public Object ABOMB_Prefab;
		public Object ASTEROIDS_Prefab;
		public Object FREEZE_EFFECT_Prefab;
		public Object AURORA_Prefab;
		public Object CHAIN_LIGHTNING_Prefab;
		public Object SAND_STORM_Prefab;

		public Object VOLCANO_Prefab;//v3.4.3

		public Texture2D SpecialFXOptA;
		public Texture2D SpecialFXOptB;
		public Texture2D SpecialFXOptC;
		public Texture2D SpecialFXOptD;
		public Texture2D SpecialFXOptE;
		public Texture2D SpecialFXOptF;
		public Texture2D SpecialFXOptG;//v3.4.3
		public Texture2D InfiniGRASS_ICON;

		//WATER PRESETS
		public Texture2D WaterOptA;
		public Texture2D WaterOptB;
		public Texture2D WaterOptC;
		public Texture2D WaterOptD;
		public Texture2D WaterOptE;
		public Texture2D WaterOptF;
		public Texture2D WaterOptG;
		public Texture2D WaterOptH;
		public Texture2D WaterOptI;
		public Texture2D WaterOptJ;

		public Texture2D UnderWaterOptA;
		public Texture2D UnderWaterOptB;

		//SKY OPTIONS
		public Texture2D SkyOptA;
		public Texture2D SkyOptB;
		public Texture2D SkyOptC;

		//MOON - WATER DROPS
		public Material MoonPhasesMat;
		public Material ScreenRainDropsMat;
		public Object RainDropsPlane;
		public Material StarsMat;//v3.3 - stars and galaxy

		//WATER
		public Object WaterPREFAB;
		public Object WaterSamplerPREFAB;
		public Object WaterTilePREFAB;
		public Object WaterTileLargePREFAB;
		public Vector2 Water_tiles = new Vector2(4,6);//how many tiles in x-y
		public Vector3 WaterScale = new Vector3(3,1,1);//initial scale of waves
		public float tileSize = 50;//real world tile size, 1,1,1 scale
		public Material WaterMat;

		public Object CausticsPREFAB;

		public Object SunSystemPREFAB;
//		public GameObject SunSystem;//instantiated system
		public Object MapCenter;
		public Object MiddleTerrain;

		//SKY DOME OPTION
		public Object SkyDomePREFAB;
		//public GameObject SkyDomeSystem;
		public float GlobalScale = 10000;//scale of globe, used for the dome system
		public float SunSystemScale = 11f;

		public bool Exclude_children = false;

		//public bool DontScaleParticleTranf = false;
		//public bool DontScaleParticleProps = false;

		public float ParticlePlaybackScaleFactor = 1f;		
		public float ParticleScaleFactor = 3f;
		public float ParticleDelay = 0f;

		//public bool SKY_folder;
		public Material skyMat;
		public Material skyboxMat;
		public Material skyMatDUAL_SUN;

//		//Terrain
		public Object SampleTerrain;
//		public Transform Mesh_terrain; 
		public Object mesh_terrain;
		public Object UnityTerrain;

		public Material UnityTerrainSnowMat;
		public Material MeshTerrainSnowMat;
		public Material SpeedTreeSnowMat;
		public Object UnityTreePrefab;//use this prefab to grab the Unity tree creator material and update it

		public GameObject FogPresetGradient1_5;
		public GameObject FogPresetGradient6;
		public GameObject FogPresetGradient7;
		public GameObject FogPresetGradient8;
		public GameObject FogPresetGradient9;
		public GameObject FogPresetGradient10;
		public GameObject FogPresetGradient11;
		public GameObject FogPresetGradient12;
		public GameObject FogPresetGradient13;
		public GameObject FogPresetGradient14;
		public GameObject FogPresetGradient15;//v3.3

		SerializedProperty	UnityTerrains;
		SerializedProperty	MeshTerrains;

		//v3.4.3
		public Texture2D VSCLOUD_SETA_ICON;
		public Texture2D VSCLOUD_SETB_ICON;
		public Texture2D VSCLOUD_SETC_ICON;
		public Texture2D VSCLOUD_SETD_ICON;
		public Texture2D VSCLOUD_SETE_ICON;
		public Texture2D VSCLOUDC_SETA_ICON;
		public Texture2D VSCLOUDC_SETB_ICON;
		public Texture2D VSCLOUDC_SETC_ICON;
		public Texture2D VSCLOUDD_SETA_ICON;
		public Texture2D VSCLOUDD_SETB_ICON;
		public Texture2D VSCLOUDD_SETC_ICON;

		public Texture2D VCLOUD_SETA_ICON;
		//VOLUME CLOUDS SETB - sheet clouds
		public Object HeavyStormVOLUME_CLOUD;
		public Object DustyStormVOLUME_CLOUD;
		public Object DayClearVOLUME_CLOUD;
		public Object SnowStormVOLUME_CLOUD;
		public Object SnowVOLUME_CLOUD;
		public Object RainStormVOLUME_CLOUD;
		public Object RainVOLUME_CLOUD;
		public Object PinkVOLUME_CLOUD;
		public Object LightningVOLUME_CLOUD;


		public Texture2D VCLOUD_SETB_ICON;
		//VOLUME CLOUDS SETA - single dense, best for camera roll
		public Object HeavyStormVOLUME_CLOUD2;
		public Object DustyStormVOLUME_CLOUD2;
		public Object DayClearVOLUME_CLOUD2;
		public Object SnowStormVOLUME_CLOUD2;
		public Object SnowVOLUME_CLOUD2;
		public Object RainStormVOLUME_CLOUD2;
		public Object RainVOLUME_CLOUD2;
		public Object PinkVOLUME_CLOUD2;
		public Object LightningVOLUME_CLOUD2;

		public Texture2D VCLOUD_SETC_ICON;
		//VOLUME CLOUDS SETC - toon clouds
		public Object HeavyStormVOLUME_CLOUD3;
		public Object DustyStormVOLUME_CLOUD3;
		public Object DayClearVOLUME_CLOUD3;
		public Object SnowStormVOLUME_CLOUD3;
		public Object SnowVOLUME_CLOUD3;
		public Object RainStormVOLUME_CLOUD3;
		public Object RainVOLUME_CLOUD3;
		public Object PinkVOLUME_CLOUD3;
		public Object LightningVOLUME_CLOUD3;

		public Texture2D VCLOUD_SETD_ICON;
		//VOLUME CLOUDS SETD - mobile clouds
		public Object HeavyStormVOLUME_CLOUD4;
		public Object DustyStormVOLUME_CLOUD4;
		public Object DayClearVOLUME_CLOUD4;
		public Object SnowStormVOLUME_CLOUD4;
		public Object SnowVOLUME_CLOUD4;
		public Object RainStormVOLUME_CLOUD4;
		public Object RainVOLUME_CLOUD4;
		public Object PinkVOLUME_CLOUD4;
		public Object LightningVOLUME_CLOUD4;

		//Particle Clouds v3.0
		public GameObject VolumeRain_Heavy;
		public GameObject VolumeRain_Mild;
		public GameObject RefractRain_Heavy;
		public GameObject RefractRain_Mild;

		//Particle Clouds
		public GameObject Rain_Heavy;
		public GameObject Rain_Mild;

		public Material cloud_dome_downMaterial;
		public Material star_dome_Material;

		public Material cloud_upMaterial;
		public Material cloud_downMaterial;
		public Material flat_cloud_upMaterial;
		public Material flat_cloud_downMaterial;
		public Material real_cloud_upMaterial;
		public Material real_cloud_downMaterial;
		public Material Surround_Clouds_Mat;
		
		public GameObject Sun_Ray_CloudPREFAB;
		public GameObject Cloud_DomePREFAB;
		public GameObject Star_particlesPREFAB;
		public GameObject Star_domePREFAB;

		public GameObject Cloud_Domev22PREFAB1;
		public GameObject Cloud_Domev22PREFAB2;

		public GameObject Upper_Dynamic_CloudPREFAB;
		public GameObject Lower_Dynamic_CloudPREFAB;
		public GameObject Upper_Cloud_BedPREFAB;
		public GameObject Lower_Cloud_BedPREFAB;
		public GameObject Upper_Cloud_RealPREFAB;
		public GameObject Lower_Cloud_RealPREFAB;
		public GameObject Upper_Static_CloudPREFAB;
		public GameObject Lower_Static_CloudPREFAB;
		public GameObject Surround_CloudsPREFAB;
		public GameObject Surround_Clouds_HeavyPREFAB;



		//Special FX
//		public GameObject SnowStorm_OBJ;
//		public GameObject[] FallingLeaves_OBJ;
//		public GameObject Butterfly_OBJ;
//		public GameObject[] Tornado_OBJs;
//		public GameObject[] Butterfly3D_OBJ;
//		public GameObject Ice_Spread_OBJ;
//		public GameObject Ice_System_OBJ;
//		public GameObject Lightning_System_OBJ;
//		public GameObject Lightning_OBJ;//single lightning to instantiate 
//		public GameObject Star_particles_OBJ;
//		public GameObject[] Volcano_OBJ;
//		public GameObject VolumeFog_OBJ;
		public GameObject SnowStormPREFAB;
		public GameObject FallingLeavesPREFAB;
		public GameObject ButterflyPREFAB;
		public GameObject TornadoPREFAB;
		public GameObject Butterfly3DPREFAB;
		public GameObject Ice_SpreadPREFAB;
		public GameObject Ice_SystemPREFAB;
		public GameObject Lightning_SystemPREFAB;
		public GameObject LightningPREFAB;//single lightning to instantiate 
		public GameObject VolcanoPREFAB;
		public GameObject VolumeFogPREFAB;

//		public bool water_folder1;
//		public bool foliage_folder1;
//		public bool weather_folder1;
//		public bool cloud_folder1;
//		public bool cloud_folder2;
//		public bool camera_folder1;
//		public bool terrain_folder1;
//		public bool scaler_folder1;
//		public bool scaler_folder11;
		public bool Include_inactive = true;
		public Object MainParticle;

		//WEATHER FOLDER
		WeatherEventSM.Volume_Weather_event_types WeatherEvent_Weather_type;
		WeatherEventSM.Volume_Weather_event_types FollowUpWeatherEvent_Weather_type;//v3.4
	//	WeatherEventSM.Volume_Weather_event_types FollowUpWeatherEvent_Weather_type;
		public float WeatherEvent_Chance;
		public float WeatherEvent_StartHour;
		public int WeatherEvent_StartDay;
		public int WeatherEvent_StartMonth;
		public float WeatherEvent_EndHour;
		public int WeatherEvent_EndDay;
		public int WeatherEvent_EndMonth;
		public float WeatherEvent_VolCloudHeight;
		public float WeatherEvent_VolCloudsHorScale;
		public bool WeatherEvent_Loop;//v3.4


		//ICONS
		public Texture2D MainIcon1;
		public Texture2D MainIcon2;
		public Texture2D MainIcon3;
		public Texture2D MainIcon4;
		public Texture2D MainIcon5;
		public Texture2D MainIcon6;
		public Texture2D MainIcon7;
		public Texture2D MainIcon8;

		//Global Sky master control script
		private SkyMaster script;
		void Awake()
		{
			script = (SkyMaster)target;
			script.gameObject.transform.position = Vector3.zero;
		}

		public GameObject FindInChildren (GameObject gameObject, string name){		
			foreach(Transform transf in gameObject.GetComponentsInChildren<Transform>()){
				if(transf.name == name){
					Debug.Log(transf.name);
					return transf.gameObject;
				}
			}
			return null;		
		}

        void setupTerrainScript()
        {
            //v4.8
            script.UnityTerrainSnowMat = UnityTerrainSnowMat;
            script.MeshTerrainSnowMat = MeshTerrainSnowMat;
            script.TerrainManager.TerrainMat = UnityTerrainSnowMat;
            script.SkyManager.SnowMat = MeshTerrainSnowMat;
            script.SkyManager.SnowMatTerrain = UnityTerrainSnowMat;	

            //RUN TO INIT TERRAIN SCRIPT
            if (script.TerrainManager != null && script.WaterManager != null && script.WaterManager.TerrainManager == null)
            {
                script.WaterManager.TerrainManager = script.TerrainManager;
            }
            //if (script.TerrainManager != null && script.TerrainManager.GradientHolders.Count == 0)
            //{
            //    script.TerrainManager.TreePefabs.Add(UnityTreePrefab as GameObject);
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient6.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient7.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient8.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient9.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient10.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient11.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient12.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient13.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient14.GetComponent<GlobalFogSkyMaster>());
            //    script.TerrainManager.GradientHolders.Add(FogPresetGradient15.GetComponent<GlobalFogSkyMaster>());//v3.3
            //}
        }

		public void OnEnable(){
			
			UnityTerrains = serializedObject.FindProperty ("UnityTerrains");
			MeshTerrains = serializedObject.FindProperty ("MeshTerrains");

			//v3.3e
			SkyColorGrad = serializedObject.FindProperty ("SkyColorGrad");
			SkyTintGrad = serializedObject.FindProperty ("SkyTintGrad");
			VolCloudLitGrad = serializedObject.FindProperty ("VolCloudLitGrad");
			VolCloudShadGrad = serializedObject.FindProperty ("VolCloudShadGrad");
			VolCloudFogGrad = serializedObject.FindProperty ("VolCloudFogGrad");

			//v3.4.5
			IntensityDiff= serializedObject.FindProperty ("IntensityDiff");
			IntensityFog= serializedObject.FindProperty ("IntensityFog");
			IntensitySun= serializedObject.FindProperty ("IntensitySun");
			heightOffsetFogCurve= serializedObject.FindProperty ("heightOffsetFogCurve");
			luminanceVFogCurve= serializedObject.FindProperty ("luminanceVFogCurve");
			lumFactorFogCurve = serializedObject.FindProperty ("lumFactorFogCurve");
			scatterFacFogCurve = serializedObject.FindProperty ("scatterFacFogCurve");
			turbidityFogCurve = serializedObject.FindProperty ("turbidityFogCurve");
			turbFacFogCurve = serializedObject.FindProperty ("turbFacFogCurve");
			horizonFogCurve = serializedObject.FindProperty ("horizonFogCurve");
			contrastFogCurve = serializedObject.FindProperty ("contrastFogCurve");

			FexposureC  = serializedObject.FindProperty ("FexposureC");
			FscaleDiffC  = serializedObject.FindProperty ("FscaleDiffC");
			FSunGC  = serializedObject.FindProperty ("FSunGC");
			FSunringC  = serializedObject.FindProperty ("FSunringC");
		}


		public override void  OnInspectorGUI() {

//		}
//
//		public void  OnGUI () {

			serializedObject.Update ();

			if (script != null && script.SkyManager != null) {
				Undo.RecordObject (script.SkyManager, "Sky Variabe Change");
				if(script.TerrainManager != null){
					Undo.RecordObject (script.TerrainManager, "Terrain Variabe Change");
				}
				if(script.GIProxyManager != null){
					Undo.RecordObject (script.GIProxyManager, "GI Proxy Variabe Change");
				}
				if(script.WaterManager != null){
					Undo.RecordObject (script.WaterManager, "Water Variabe Change");
				}

				//v3.4.5
				if(script.SkyManager.VolShaderCloudsH != null){
					Undo.RecordObject (script.SkyManager.VolShaderCloudsH, "Cloud Variabe Change");
					Undo.RecordObject (script.SkyManager.VolShaderCloudsH.FlatBedClouds, "Cloud Rotation Change");
					Undo.RecordObject (script.SkyManager.VolShaderCloudsH.RotClouds, "Cloud Rotation Change");
					Undo.RecordObject (script.SkyManager.VolShaderCloudsH.SideClouds, "Cloud Rotation Change");
					Undo.RecordObject (script.SkyManager.VolShaderCloudsH.SideAClouds, "Cloud Rotation Change");
					Undo.RecordObject (script.SkyManager.VolShaderCloudsH.SideBClouds, "Cloud Rotation Change");
					Undo.RecordObject (script.SkyManager.VolShaderCloudsH.SideCClouds, "Cloud Rotation Change");
				}
				if(script.SkyManager.windZone != null){
					Undo.RecordObject (script.SkyManager.windZone, "Wind Variabe Change");
					Undo.RecordObject (script.SkyManager.windZone.gameObject.transform, "Wind Direction Change");
				}
				//if(script.SkyManager.VolLightingH != null){
				//	Undo.RecordObject (script.SkyManager.VolLightingH, "Configurator Change");
				//}
			}
			//Undo.

			//CHOOSE TAB BASED
			script.UseTabs = EditorGUILayout.Toggle ("Use tabs", script.UseTabs, GUILayout.MaxWidth (180.0f));

			//TABS
			if (script.UseTabs) {
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Sky")) {
					script.currentTab = 0;
				}
				if (GUILayout.Button ("Clouds")) {
					script.currentTab = 1;
				}
				if (GUILayout.Button ("Terrain")) {
					script.currentTab = 2;
				}
				if (GUILayout.Button ("Camera FX")) {
					script.currentTab = 3;
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Weather")) {
					script.currentTab = 4;
				}
				if (GUILayout.Button ("Foliage")) {
					script.currentTab = 5;
				}
				if (GUILayout.Button ("Water")) {
					script.currentTab = 6;
				}
				if (GUILayout.Button ("Special FX")) {
					script.currentTab = 7;
				}
				EditorGUILayout.EndHorizontal ();
			}

			//v3.4
			float sliderWidth = 295.0f;

			//TAB0
			if ((script.UseTabs && script.currentTab == 0) | !script.UseTabs) {

				////////////////////////////////////////////////////////////
				GUI.backgroundColor = Color.blue * 0.2f;
				//GUI.color = Color.gray*0.15f;
				EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));
				GUI.backgroundColor = Color.white;
				//X_offset_left = 200;
				//Y_offset_top = 100;
			
				//GUILayout.Box("",GUILayout.Height(3),GUILayout.Width(410));	
				GUILayout.Label (MainIcon1, GUILayout.MaxWidth (410.0f));
			
				EditorGUILayout.LabelField ("Sky Options", EditorStyles.boldLabel);


				EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
				GUILayout.Space (10);
				script.SKY_folder = EditorGUILayout.Foldout (script.SKY_folder, "Sky options");
				//GUILayout.Space (50);
				EditorGUILayout.EndHorizontal ();
			
				if (script.SKY_folder) {

//				GUI.backgroundColor = Color.gray*0.9f;
//				GUI.contentColor = Color.gray*0.9f;
//				GUI.color = Color.white;
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));
					EditorGUILayout.BeginHorizontal ();
				
					//CHECK IF SKY MANAGER EXISTS and grab that instead, also search its components all exist etc
					if (script.SkyManager == null & script.gameObject.GetComponent<SkyMasterManager> () != null) {
						script.SkyManager = script.gameObject.GetComponent<SkyMasterManager> ();
						if (script.SkyManager.Mesh_terrain != null) {
							if (script.TerrainManager == null) {
								script.TerrainManager = script.SkyManager.Mesh_terrain.GetComponentsInChildren<SeasonalTerrainSKYMASTER> (true) [0];
							}
							//if still null, check for terrain
							if (script.TerrainManager == null) {
								script.TerrainManager = Terrain.activeTerrain.gameObject.GetComponentsInChildren<SeasonalTerrainSKYMASTER> (true) [0];
							}
						}
					}
					EditorGUILayout.EndHorizontal ();


					//EditorGUILayout.Separator();
					//			GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(410));	
					//EditorGUILayout.HelpBox("Define map center",MessageType.None);
					EditorGUILayout.HelpBox ("Default sun system", MessageType.None);
				
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Sun system", EditorStyles.boldLabel, GUILayout.MaxWidth (95.0f));
					SunSystemPREFAB = EditorGUILayout.ObjectField (SunSystemPREFAB, typeof(GameObject), true, GUILayout.MaxWidth (150.0f));
					EditorGUILayout.EndHorizontal ();
				
					//EditorGUILayout.Separator();
					GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
					//EditorGUILayout.HelpBox("Default sun system",MessageType.None);
					EditorGUILayout.HelpBox ("Define map center", MessageType.None);
				
					//SunSystem = EditorGUILayout.ObjectField(SunSystem,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Map center", EditorStyles.boldLabel, GUILayout.MaxWidth (95.0f));
					if (script.SkyManager != null) {
						script.SkyManager.MapCenter = EditorGUILayout.ObjectField (script.SkyManager.MapCenter, typeof(Transform), true, GUILayout.MaxWidth (150.0f)) as Transform;
					} else {
						MapCenter = EditorGUILayout.ObjectField (MapCenter, typeof(GameObject), true, GUILayout.MaxWidth (150.0f)) as GameObject;
					}
					EditorGUILayout.EndHorizontal ();


					//EditorGUILayout.EndVertical();
					//EditorGUILayout.BeginVertical();

					//EditorGUILayout.BeginHorizontal();



					if (GUILayout.Button ("Add Sky")) {
						// --------------- ADD SKY --------------- 
						if (script.SkyManager == null & SunSystemPREFAB != null) {

							// --------------- ADD SKY SCRIPT --------------- 
							script.gameObject.AddComponent<SkyMasterManager> ();
							script.SkyManager = script.gameObject.GetComponent<SkyMasterManager> ();
							script.SkyManager.PlanetScale = GlobalScale;//define for skybox, get from dome in skydome version
							script.SkyManager.DefinePlanetScale = true;
							script.SkyManager.Unity5 = true;

							// --------------- DEFINE MAP CENTER --------------- 
							if (script.SkyManager.MapCenter != null) {
								//(MapCenter as GameObject).transform.parent = script.transform;
							} else {
								if (MapCenter == null) {//if not defined in configurator inspector
									script.SkyManager.MapCenter = (new GameObject ()).transform;
									script.SkyManager.MapCenter.gameObject.name = "Map Center";
									script.SkyManager.MapCenter.parent = script.transform;
									MapCenter = script.SkyManager.MapCenter.gameObject;
								} else {
									script.SkyManager.MapCenter = (MapCenter as GameObject).transform;
									script.SkyManager.MapCenter.gameObject.name = "Map Center";
									script.SkyManager.MapCenter.parent = script.transform;
								}

								//IF UNITY TERRAIN - PUT IN MIDDLE

								//IF MESH TERRAIN - CENTER
							}
							//			GameObject MapCenterOBJ = MapCenter as GameObject;

							//v3.2
							script.transform.position = script.SkyManager.MapCenter.position;
							if (Camera.main != null) {
                                //Camera.main.transform.position = Camera.main.transform.position + script.SkyManager.MapCenter.position;
                                Camera.main.farClipPlane = 30000; //v4.8.4
                            }

							// --------------- INSTANTIATE SUN - MOON SYSTEM --------------- 
							script.SkyManager.SunSystem = (GameObject)Instantiate (SunSystemPREFAB);
							script.SkyManager.SunSystem.transform.position = script.SkyManager.MapCenter.transform.position;

                            script.SkyManager.Current_Time = 12.0f;// 20.5f; //v4.8

							script.SkyManager.SunSystem.transform.eulerAngles = new Vector3 (28.14116f, 170, 180);
							script.SkyManager.SunSystem.name = "Sun System";
							script.SkyManager.SunSystem.transform.localScale = SunSystemScale * Vector3.one;
							script.SkyManager.SunSystem.transform.parent = script.transform;
							//		script.SkyManager.SunSystem = SunSystem;
							script.SkyManager.MoonPhasesMat = MoonPhasesMat;
							script.SkyManager.MoonPhases = true;

							// --------------- FIND SUN SYSTEM CENTER AND ALIGN TO MAP CENTER --------------- 
							GameObject SunSystemCenter = FindInChildren (script.SkyManager.SunSystem, "Sun Target");
							Vector3 Distance = SunSystemCenter.transform.position - script.SkyManager.MapCenter.transform.position;
							script.SkyManager.SunSystem.transform.position -= Distance;
							script.SkyManager.SunTarget = SunSystemCenter;

							// --------------- ASSIGN SUN - MOON LIGHTS TO SKY MANAGER --------------- 
							script.SkyManager.SunObj = FindInChildren (script.SkyManager.SunSystem, "SunBody");
							script.SkyManager.MoonObj = FindInChildren (script.SkyManager.SunSystem, "MoonBody");
							script.SkyManager.SUN_LIGHT = script.SkyManager.SunObj.transform.parent.gameObject;
							script.SkyManager.SUPPORT_LIGHT = FindInChildren (script.SkyManager.SUN_LIGHT, "MoonLight");
							script.SkyManager.MOON_LIGHT = FindInChildren (FindInChildren (script.SkyManager.SunSystem, "MoonBody"), "MoonLight");

							//ADD CLOUDS
							script.SkyManager.Upper_Dynamic_Cloud = (GameObject)Instantiate (Upper_Dynamic_CloudPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Upper_Dynamic_Cloud.transform.parent = script.transform;
							script.SkyManager.Lower_Dynamic_Cloud = (GameObject)Instantiate (Lower_Dynamic_CloudPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Lower_Dynamic_Cloud.transform.parent = script.transform;
							script.SkyManager.Upper_Cloud_Bed = (GameObject)Instantiate (Upper_Cloud_BedPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Upper_Cloud_Bed.transform.parent = script.transform;
							script.SkyManager.Lower_Cloud_Bed = (GameObject)Instantiate (Lower_Cloud_BedPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Lower_Cloud_Bed.transform.parent = script.transform;
							script.SkyManager.Upper_Cloud_Real = (GameObject)Instantiate (Upper_Cloud_RealPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Upper_Cloud_Real.transform.parent = script.transform;
							script.SkyManager.Lower_Cloud_Real = (GameObject)Instantiate (Lower_Cloud_RealPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Lower_Cloud_Real.transform.parent = script.transform;
							script.SkyManager.Upper_Static_Cloud = (GameObject)Instantiate (Upper_Static_CloudPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Upper_Static_Cloud.transform.parent = script.transform;
							script.SkyManager.Lower_Static_Cloud = (GameObject)Instantiate (Lower_Static_CloudPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Lower_Static_Cloud.transform.parent = script.transform;
							script.SkyManager.Surround_Clouds = (GameObject)Instantiate (Surround_CloudsPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Surround_Clouds.transform.parent = script.transform;
							script.SkyManager.Surround_Clouds_Heavy = (GameObject)Instantiate (Surround_Clouds_HeavyPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Surround_Clouds_Heavy.transform.parent = script.transform;

							script.SkyManager.cloud_upMaterial = script.SkyManager.Upper_Dynamic_Cloud.GetComponentsInChildren<ParticleSystemRenderer> (true) [0].sharedMaterial;
							script.SkyManager.cloud_downMaterial = script.SkyManager.Upper_Dynamic_Cloud.GetComponentsInChildren<ParticleSystemRenderer> (true) [0].sharedMaterial;
							script.SkyManager.flat_cloud_upMaterial = script.SkyManager.Upper_Cloud_Bed.GetComponentsInChildren<ParticleSystemRenderer> (true) [0].sharedMaterial;
							script.SkyManager.flat_cloud_downMaterial = script.SkyManager.Lower_Cloud_Bed.GetComponentsInChildren<ParticleSystemRenderer> (true) [0].sharedMaterial;
							script.SkyManager.Surround_Clouds_Mat = script.SkyManager.Surround_Clouds.GetComponentsInChildren<ParticleSystemRenderer> (true) [0].sharedMaterial;
							script.SkyManager.real_cloud_upMaterial = script.SkyManager.Upper_Cloud_Real.GetComponentsInChildren<ParticleSystemRenderer> (true) [0].sharedMaterial;
							script.SkyManager.real_cloud_downMaterial = script.SkyManager.Lower_Cloud_Real.GetComponentsInChildren<ParticleSystemRenderer> (true) [0].sharedMaterial;

							script.SkyManager.Sun_Ray_Cloud = (GameObject)Instantiate (Sun_Ray_CloudPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Sun_Ray_Cloud.transform.parent = script.transform;
							script.SkyManager.Cloud_Dome = (GameObject)Instantiate (Cloud_DomePREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Cloud_Dome.transform.parent = script.transform;
							script.SkyManager.Star_particles_OBJ = (GameObject)Instantiate (Star_particlesPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Star_particles_OBJ.transform.parent = script.transform;
							script.SkyManager.Star_particles_OBJ.transform.position = script.SkyManager.MapCenter.position;

							script.SkyManager.StarDome = (GameObject)Instantiate (Star_domePREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.StarDome.transform.parent = script.transform;
							script.SkyManager.CloudDomeL1 = (GameObject)Instantiate (Cloud_Domev22PREFAB1, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.CloudDomeL1.transform.parent = script.transform;
							script.SkyManager.CloudDomeL2 = (GameObject)Instantiate (Cloud_Domev22PREFAB2, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.CloudDomeL2.transform.parent = script.transform;

							script.SkyManager.cloud_dome_downMaterial = script.SkyManager.Cloud_Dome.GetComponentsInChildren<Renderer> (true) [0].sharedMaterial;
							script.SkyManager.star_dome_Material = script.SkyManager.StarDome.GetComponentsInChildren<Renderer> (true) [0].sharedMaterial;
							//script.SkyManager.star = script.SkyManager.Lower_Cloud_Real.GetComponentsInChildren<Renderer>(true)[0].sharedMaterial;

							script.SkyManager.CloudDomeL1Mat = script.SkyManager.CloudDomeL1.GetComponentsInChildren<Renderer> (true) [0].sharedMaterial;
							script.SkyManager.CloudDomeL2Mat = script.SkyManager.CloudDomeL2.GetComponentsInChildren<Renderer> (true) [0].sharedMaterial;

							//DUAL SUN HANDLE

							//STARS SHADER
							script.SkyManager.StarsMaterial = StarsMat;

							//FOGS
							script.SkyManager.VFogsPerVWeather.Add (0);//sunny--
							script.SkyManager.VFogsPerVWeather.Add (13);//foggy
							script.SkyManager.VFogsPerVWeather.Add (14);//heavy fog
							script.SkyManager.VFogsPerVWeather.Add (0);//tornado
							script.SkyManager.VFogsPerVWeather.Add (7);//snow storm--

							script.SkyManager.VFogsPerVWeather.Add (7);//freeze storm
							script.SkyManager.VFogsPerVWeather.Add (0);//flat clouds
							script.SkyManager.VFogsPerVWeather.Add (0);//lightning storm
							script.SkyManager.VFogsPerVWeather.Add (7);//heavy storm--
							script.SkyManager.VFogsPerVWeather.Add (7);//heavy storm dark--

							script.SkyManager.VFogsPerVWeather.Add (12);//cloudy--
							script.SkyManager.VFogsPerVWeather.Add (0);//rolling fog
							script.SkyManager.VFogsPerVWeather.Add (0);//volcano erupt
							script.SkyManager.VFogsPerVWeather.Add (14);//rain

							//LOCALIZE EFFECTS
							script.SkyManager.Snow_local = true;
							script.SkyManager.Mild_rain_local = true;
							script.SkyManager.Heavy_rain_local = true;
							script.SkyManager.Fog_local = true;
							script.SkyManager.Butterflies_local = true;

							//ADD RAIN
							script.SkyManager.SnowStorm_OBJ	= (GameObject)Instantiate (SnowStormPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.SnowStorm_OBJ.transform.parent = script.transform;
							script.SkyManager.Butterfly_OBJ	= (GameObject)Instantiate (ButterflyPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Butterfly_OBJ.transform.parent = script.transform;

							//Parent ice spread to snow storm system and assign pool particle to collision manager
							//v.3.0.3	script.SkyManager.Ice_System_OBJ=(GameObject)Instantiate(Ice_SystemPREFAB); script.SkyManager.Ice_System_OBJ.transform.parent = script.transform;
							//v.3.0.3	script.SkyManager.Ice_Spread_OBJ=(GameObject)Instantiate(Ice_SpreadPREFAB); script.SkyManager.Ice_Spread_OBJ.transform.parent = script.SkyManager.SnowStorm_OBJ.transform;
							//v.3.0.3	script.SkyManager.Ice_Spread_OBJ.GetComponentsInChildren<ParticleCollisionsSKYMASTER>(true)[0].ParticlePOOL = script.SkyManager.Ice_System_OBJ.GetComponentsInChildren<ParticlePropagationSKYMASTER>(true)[0].gameObject;


							script.SkyManager.Lightning_OBJ	= (GameObject)Instantiate (LightningPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Lightning_OBJ.transform.parent = script.transform;
								
							script.SkyManager.Lightning_System_OBJ	= (GameObject)Instantiate (Lightning_SystemPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Lightning_System_OBJ.transform.parent = script.transform;
							script.SkyManager.VolumeFog_OBJ = (GameObject)Instantiate (VolumeFogPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.VolumeFog_OBJ.transform.parent = script.transform;
							script.SkyManager.Rain_Heavy = (GameObject)Instantiate (Rain_Heavy, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Rain_Heavy.transform.parent = script.transform;
							script.SkyManager.Rain_Mild = (GameObject)Instantiate (Rain_Mild, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Rain_Mild.transform.parent = script.transform;
							script.SkyManager.VolumeRain_Heavy	= (GameObject)Instantiate (VolumeRain_Heavy, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.VolumeRain_Heavy.transform.parent = script.transform;
							script.SkyManager.VolumeRain_Mild	= (GameObject)Instantiate (VolumeRain_Mild, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.VolumeRain_Mild.transform.parent = script.transform;
								
							//init arrays
							script.SkyManager.FallingLeaves_OBJ = new GameObject[1];
							script.SkyManager.Tornado_OBJs = new GameObject[1];
							//v.3.0.3 script.SkyManager.Butterfly3D_OBJ = new GameObject[1];
							script.SkyManager.Volcano_OBJ = new GameObject[1];

							script.SkyManager.FallingLeaves_OBJ [0]	=	(GameObject)Instantiate (FallingLeavesPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.FallingLeaves_OBJ [0].transform.parent = script.transform;					
							script.SkyManager.Tornado_OBJs [0] =	(GameObject)Instantiate (TornadoPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Tornado_OBJs [0].transform.parent = script.transform;				
							//v.3.0.3			script.SkyManager.Butterfly3D_OBJ[0]	=(GameObject)Instantiate(Butterfly3DPREFAB); script.SkyManager.Butterfly3D_OBJ[0].transform.parent = script.transform;						
							script.SkyManager.Volcano_OBJ [0]	= (GameObject)Instantiate (VolcanoPREFAB, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.Volcano_OBJ [0].transform.parent = script.transform;

							script.SkyManager.RefractRain_Mild = (GameObject)Instantiate (RefractRain_Mild, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.RefractRain_Mild.transform.parent = script.transform;
							script.SkyManager.RefractRain_Heavy = (GameObject)Instantiate (RefractRain_Heavy, script.SkyManager.MapCenter.position, Quaternion.identity);
							script.SkyManager.RefractRain_Heavy.transform.parent = script.transform;

							//SETUP GI PROXY for sun
							//SET PLAYER
							if (!script.SkyManager.Tag_based_player) {
								script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Tag_based_player = false;
								if (script.SkyManager.Hero != null) {
									script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().HERO = script.SkyManager.Hero;
								} else {
									Debug.Log ("Note: Hero has not been defined. The Main Camera will be used as the player. For a specific player usage, define a hero in 'hero' parameters in Sky Master Manager and LightColliionsPDM scripts.");
									Debug.Log ("The 'tag based player' option may also be used to define player by tag (default tag is 'Player')");
								}
							} else {
								script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Tag_based_player = true;
							}

							//DEFINE START PRESET
							script.SkyManager.Preset = 9;//v3.0 day time - no red sun
							script.SkyManager.Auto_Cycle_Sky = true;
							script.SkyManager.SPEED = 1;
							script.SkyManager.currentWeatherName = SkyMasterManager.Volume_Weather_types.Cloudy;

							//ASSIGN SKY MATERIALS
							if (script.SkyManager.SunObj2 != null) {
								script.SkyManager.skyboxMat = skyMatDUAL_SUN;
							} else {
								script.SkyManager.skyboxMat = skyboxMat;
							}
							RenderSettings.skybox = script.SkyManager.skyboxMat;
							//Debug.Log(script.SkyManager.skyMat.name);
							//Debug.Log(RenderSettings.skybox.name);
							//script.SkyManager.skyMat = skyMat;		//v3.3c

						} else {
							if (script.SkyManager != null) {
								Debug.Log ("Sky Manager exists");
							}
							if (SunSystemPREFAB != null) {
								Debug.Log ("Please add a sun system");
							}
						}

                        //v4.8
                        script.SkyManager.SkyColorationOffset = -0.1f;
                        script.SkyManager.MoonColor = new Color(150.0f/255.0f, 150.0f / 255.0f, 150.0f / 255.0f, 1);
                        script.SkyManager.MoonAmbientColor = new Color(100.0f / 255.0f, 100.0f / 255.0f, 100.0f / 255.0f, 1);
                        script.SkyManager.MinMaxEclMoonTransp = new Vector3(0.1f,0.9f,0.1f);
                        script.SkyManager.MoonSunLight = 0.008f;
                        script.SkyManager.MoonCoverage = 0.52f;
                        script.SkyManager.MoonSize = 0.2f;

                        //SETUP CAMERA
                        Camera.main.farClipPlane = 30000;
						//Camera.main.transform.Translate(0,10,0); //v3.3d
						Camera.main.allowHDR = true;
						Camera.main.renderingPath = RenderingPath.DeferredShading;

                        //v4.8
                        script.Update();
                        setupTerrainScript();
                    }


					//EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical ();
					EditorGUILayout.Space ();
					//EditorGUILayout.EndVertical();
					//EditorGUILayout.BeginVertical();


					//EditorGUILayout.BeginHorizontal();

					if (script.SkyManager != null) {

						EditorGUILayout.BeginVertical ("box", GUILayout.Width (180.0f));
						//MOON v3.3
						//	GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(415));
						EditorGUILayout.HelpBox ("Enable Moon for Latitude/Longitude system)", MessageType.None);
						if (GUILayout.Button ("Setup Lat/Lon Moon")) {

							//create separate holder for moon and parent to sky master
							GameObject MoonSystem = new GameObject ();
							MoonSystem.name = "MoonSystem";
							MoonSystem.transform.parent = script.transform;
							MoonSystem.transform.position = script.SkyManager.SunSystem.transform.position;
							MoonSystem.transform.eulerAngles = script.SkyManager.SunSystem.transform.eulerAngles;

							//add it to skymanager
							script.SkyManager.MoonCenterObj = MoonSystem.transform;
							script.SkyManager.StarsMaterial = StarsMat;

							//reparent moon
							script.SkyManager.MoonObj.transform.parent = MoonSystem.transform;
							script.SkyManager.MoonObj.transform.localEulerAngles = Vector3.zero;

							//enable lat/lon moon params
							script.SkyManager.LatLonMoonPos = true;
							script.SkyManager.AutoMoonLighting = true;
							script.SkyManager.AutoMoonFade = true;
							script.SkyManager.AutoSunPosition = true;
							script.SkyManager.StarsFollowMoon = true;

							//add look at
							script.SkyManager.MOON_LIGHT.AddComponent<SmoothLookAtSKYMASTER> ().target = MoonSystem.transform;
						}


						//GI PROXY
						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));
						EditorGUILayout.HelpBox ("Setup GI Proxy (real time GI approximation)", MessageType.None);
						if (GUILayout.Button ("Setup GI Proxy on Sun")) {
							GameObject GIProxyPool = new GameObject ();
							GIProxyPool.name = "GI Proxy Sun Bounce Lights";
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().LightPool = GIProxyPool;
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().enabled = true;
							//add cut height based on terrain or map center
							float cut_height = 0;
							if (script.SkyManager.MapCenter != null) {
								cut_height = script.SkyManager.MapCenter.position.y;
							}
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Cut_height = cut_height;
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().extend_X = 2 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().extend_Y = 6 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().PointLight_Radius = 70 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().PointLight_Radius_2ond = 70 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Hero_offset = new Vector3 (2 * (script.SkyManager.WorldScale / 20), 0, 40 * (script.SkyManager.WorldScale / 20));
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Bounce_Range = 64 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().min_dist_to_last_light = 5 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().min_dist_to_source = 1000 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().min_density_dist = 9 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Follow_dist = 5 * (script.SkyManager.WorldScale / 20);

							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().min_dist_to_camera = 65 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().max_hit_light_dist = 6 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().placement_offset = 6 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Cut_off_height = 9 * (script.SkyManager.WorldScale / 20);
						}




						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));
						//DUAL SUN
						EditorGUILayout.HelpBox ("Define 2ond sun", MessageType.None);
						//EditorGUILayout.LabelField("Define 2ond sun",EditorStyles.boldLabel);
						script.SkyManager.SunObj2 = EditorGUILayout.ObjectField (script.SkyManager.SunObj2, typeof(GameObject), true, GUILayout.MaxWidth (180.0f)) as GameObject;
						//script.SkyManager.DualSunsFactors

				
				
						if (GUILayout.Button ("Add Second Sun")) {
							// --------------- ASSIGN PROPER MATERIAL --------------- 
							if (script.SkyManager.SunObj2 != null) {
								script.SkyManager.skyboxMat = skyMatDUAL_SUN;
							} else {
								Debug.Log ("Please enter a Gameobject to define the 2ond sun position first");
							}
							// --------------- ADD SUN OBJECT --------------- 

							// --------------- REGISTER OBJECT IN SKY MANAGER --------------- 
						}

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));

						//v.3.4.1
						EditorGUILayout.HelpBox ("Use a spherical Sky Dome instead of the default Unity 5 skybox shader. Use only for special FX and cases. The recommended sky shading for Unity 5 is the default one applied to the skybox by pressing the" +
							"'Add Sky' button.", MessageType.None);
						
						if (GUILayout.Button ("Add Sky Dome")) {
							//Use a dome instead of skybox, if required
							script.SkyManager.SkyDomeSystem = ((GameObject)Instantiate (SkyDomePREFAB)).transform;
							GameObject SkyDomeSystemCenter = FindInChildren (script.SkyManager.SkyDomeSystem.gameObject, "Sun Target");
							Vector3 Distance = SkyDomeSystemCenter.transform.position - script.SkyManager.MapCenter.position;// - (MapCenter as GameObject).transform.position;
							script.SkyManager.SkyDomeSystem.transform.position -= Distance;
							//DEFINE PROPER PRESET
							script.SkyManager.Preset = 5;
							script.SkyManager.skyMat = skyMat; //v3.3c
						}

						//v.3.4.1
						EditorGUILayout.HelpBox ("Sun light colors (Dawn-Day-Dusk-Night)", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Dawn_Sun_Color = EditorGUILayout.ColorField (script.SkyManager.Dawn_Sun_Color, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Day_Sun_Color = EditorGUILayout.ColorField (script.SkyManager.Day_Sun_Color, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Dusk_Sun_Color = EditorGUILayout.ColorField (script.SkyManager.Dusk_Sun_Color, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Night_Sun_Color = EditorGUILayout.ColorField (script.SkyManager.Night_Sun_Color, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();

						//PLAYER
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));

						//	GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(410));
						EditorGUILayout.HelpBox ("Define player", MessageType.None);
						script.SkyManager.Tag_based_player = EditorGUILayout.Toggle ("Define player by tag", script.SkyManager.Tag_based_player, GUILayout.MaxWidth (180.0f));

						if (!script.SkyManager.Tag_based_player) {

							//EditorGUILayout.LabelField("Define player object",EditorStyles.boldLabel);
							script.SkyManager.Hero = EditorGUILayout.ObjectField (script.SkyManager.Hero, typeof(Transform), true, GUILayout.MaxWidth (180.0f)) as Transform;
						}
						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));
						if (GUILayout.Button ("Define player")) {
							//Scale sun system after initial addition
							//SET PLAYER
							if (!script.SkyManager.Tag_based_player) {
								script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Tag_based_player = false;
								if (script.SkyManager.Hero != null) {
									script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().HERO = script.SkyManager.Hero;
								} else {
									Debug.Log ("Note: Hero has not been defined. The Main Camera will be used as the player. For a specific player usage, define a hero in 'hero' parameters in Sky Master Manager and LightColliionsPDM scripts.");
									Debug.Log ("The 'tag based player' option may also be used to define player by tag (default tag is 'Player')");
								}
							} else {
								script.SkyManager.SUN_LIGHT.GetComponent<LightCollisionsPDM> ().Tag_based_player = true;
								//v3.3d
								script.SkyManager.Hero = null; //nullify to grab new player based on latest options
							}
						}

						//v3.4
						EditorGUILayout.HelpBox ("Enable Sun follow player to decouple the sun-sky rendering from the player height", MessageType.None);
						if (script.SkyManager.Hero != null) {
							script.SkyManager.SunFollowHero = EditorGUILayout.Toggle ("Sun system follows player", script.SkyManager.SunFollowHero, GUILayout.MaxWidth (180.0f));
						}

						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();


						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));
						//GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(410));



						//ADD WINDZONE

						GUILayout.Label ("Windzone");
					
						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Add windzone"), GUILayout.Width (120))) {
							if (script.SkyManager.windZone == null) {
								GameObject WindZone = new GameObject ();
								//script.SkyManager.windZone.gameObject = WindZone;
								WindZone.AddComponent<WindZone> ();
								script.SkyManager.windZone = WindZone.GetComponent<WindZone> ();
								WindZone.name = "Sky Master windzone";
							} else {
								Debug.Log ("Windzone exists");
							}
						}
						//Handle external zone
//					if(script.SkyManager.windZone==null){
//						if(script.SkyManager.windZone.gameObject!=null){
//							script.SkyManager.windZone = script.SkyManager.windZone.gameObject.GetComponent<WindZone>();
//						}
//					}
					
						//Wind_Zone = EditorGUILayout.ObjectField (Wind_Zone, typeof(Transform), true, GUILayout.MaxWidth (180.0f)) as Transform;
						//script.windzone = Wind_Zone;
						script.SkyManager.windZone = EditorGUILayout.ObjectField (script.SkyManager.windZone, typeof(WindZone), true, GUILayout.MaxWidth (180.0f)) as WindZone;



						EditorGUILayout.EndHorizontal ();	

						if (script.SkyManager.windZone != null) {
							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	
							EditorGUILayout.HelpBox ("Wind intensity", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.windZone.windMain = EditorGUILayout.Slider (script.SkyManager.windZone.windMain, 0, 24, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	
							EditorGUILayout.HelpBox ("Wind direction", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							float aurlerY = EditorGUILayout.Slider (script.SkyManager.windZone.transform.eulerAngles.y, 0, 360, GUILayout.MaxWidth (sliderWidth));
							Vector3 angles = script.SkyManager.windZone.gameObject.transform.eulerAngles;
							script.SkyManager.windZone.gameObject.transform.eulerAngles = new Vector3 (angles.x, aurlerY, angles.z);
							EditorGUILayout.EndHorizontal ();

							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	
							EditorGUILayout.HelpBox ("Wind Turbulence", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.windZone.windTurbulence = EditorGUILayout.Slider (script.SkyManager.windZone.windTurbulence, -50, 50, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
						}


						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();

					}


				
					//EditorGUILayout.EndHorizontal();
				
					EditorGUIUtility.wideMode = false;


//				GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(410));	
//				//EditorGUILayout.HelpBox("Define map center",MessageType.None);
//				EditorGUILayout.HelpBox("Scale sun-moon system",MessageType.None);
//				//script.SkyManager.SunSystem.transform.localScale = EditorGUILayout.FloatField(script.SkyManager.SunSystem.transform.localScale.x,GUILayout.MaxWidth(95.0f))*Vector3.one;
//				EditorGUILayout.BeginHorizontal();
//				script.SkyManager.SunSystem.transform.localScale = EditorGUILayout.Slider(script.SkyManager.SunSystem.transform.localScale.x,1,20,GUILayout.MaxWidth(195.0f))*Vector3.one;
//				EditorGUILayout.EndHorizontal();

					if (script.SkyManager != null) {

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));

						//		GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(410));	
						EditorGUILayout.HelpBox ("Season", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Season = (int)EditorGUILayout.Slider (script.SkyManager.Season, 0, 4, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	

						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Decrease year"), GUILayout.Width (150))) {
							if (script.SkyManager.Current_Day > 365) {
								script.SkyManager.Current_Day -= 365;
							}
						}
						int current_year = (int)(script.SkyManager.Current_Day / 365);
						EditorGUILayout.HelpBox ("Year:" + current_year.ToString (), MessageType.None);
						if (GUILayout.Button (new GUIContent ("Increase year"), GUILayout.Width (150))) {
							script.SkyManager.Current_Day += 365;
						}
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.HelpBox ("Day", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						float day = EditorGUILayout.Slider (script.SkyManager.Current_Day % 365, 1, 364, GUILayout.MaxWidth (sliderWidth));
						current_year = (int)(script.SkyManager.Current_Day / 365); //recalc current year in case increase year button was pressed
						script.SkyManager.Current_Day = current_year * 365 + day;
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.HelpBox ("Time Zone (-12 to +12)", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Time_zone = EditorGUILayout.Slider (script.SkyManager.Time_zone, -12, 12, GUILayout.MaxWidth (sliderWidth));

						float signA = Mathf.Sign (script.SkyManager.Time_zone);
						float diffA = script.SkyManager.Time_zone - (int)script.SkyManager.Time_zone; 

						if (diffA != 0) {
							if (Mathf.Abs (diffA) > 0.75f) {
								script.SkyManager.Time_zone = (int)script.SkyManager.Time_zone + signA; //script.SkyManager.Time_zone + (1 - signA*diffA);
							} else if (Mathf.Abs (diffA) > 0.25f) {
								script.SkyManager.Time_zone = (int)script.SkyManager.Time_zone + (signA / 2); //script.SkyManager.Time_zone + (0.5f - signA*diffA);
							} else {
								script.SkyManager.Time_zone = (int)script.SkyManager.Time_zone;//script.SkyManager.Time_zone - signA*diffA;
							}
						}
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.HelpBox ("Time of Day", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Current_Time = EditorGUILayout.Slider (script.SkyManager.Current_Time, 0, 24, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.HelpBox ("Shift dawn", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Shift_dawn = EditorGUILayout.Slider (script.SkyManager.Shift_dawn, -1.5f, 0, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.AutoSunPosition = EditorGUILayout.Toggle ("LatLonTOD", script.SkyManager.AutoSunPosition, GUILayout.MaxWidth (380.0f));
						EditorGUILayout.EndHorizontal ();

						if (script.SkyManager.AutoSunPosition) {
							EditorGUILayout.HelpBox ("Latitude", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.Latitude = EditorGUILayout.Slider (script.SkyManager.Latitude, -89.99f, 89.99f, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("Longitude", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.Longitude = EditorGUILayout.Slider (script.SkyManager.Longitude, -180, 180, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
						}
						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));

						EditorGUILayout.HelpBox ("Sun speed - Use to adjust day time length", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.SPEED = EditorGUILayout.Slider (script.SkyManager.SPEED, 0.0f, 16, GUILayout.MaxWidth (sliderWidth));//v3.3 - extend to 16 max -v3.4 zero low end instead of 0.1f
						EditorGUILayout.EndHorizontal ();

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	
						EditorGUILayout.HelpBox ("Horizontal Sun Position", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Rot_Sun_Y = EditorGUILayout.Slider (script.SkyManager.Rot_Sun_Y, -360, 360, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
						EditorGUILayout.HelpBox ("Midday sun intensity", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Max_sun_intensity = EditorGUILayout.Slider (script.SkyManager.Max_sun_intensity, 0.3f, 5, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	
						EditorGUILayout.HelpBox ("Moon light intensity", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.max_moon_intensity = EditorGUILayout.Slider (script.SkyManager.max_moon_intensity, 0.1f, 3, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						//v3.3e - editor boxes
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));

						EditorGUILayout.HelpBox ("Sky shader update per defined seconds", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Update_mat_every = EditorGUILayout.Slider (script.SkyManager.Update_mat_every, 0, 0.5f, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.HelpBox ("Ambient Light from Sky", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.updateSkyAmbient = EditorGUILayout.Toggle ("Update Ambient from Sky", script.SkyManager.updateSkyAmbient, GUILayout.MaxWidth (380.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.HelpBox ("Ambient Intensity", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.AmbientIntensity = EditorGUILayout.Slider (script.SkyManager.AmbientIntensity, 0, 15f, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.HelpBox ("Update Ambient light Every specified seconds", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.AmbientUpdateEvery = EditorGUILayout.Slider (script.SkyManager.AmbientUpdateEvery, 0, 10f, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						//v3.0.2
						//		GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(410));	
						EditorGUILayout.HelpBox ("Sky Coloration intensity", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.SkyColorationOffset = EditorGUILayout.Slider (script.SkyManager.SkyColorationOffset, -0.35f, 0.5f, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						//v3.3e - gradient based sky coloration
						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	
						EditorGUILayout.HelpBox ("Gradient based Time of Day Sky, Cloud & Water coloration", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.UseGradients = EditorGUILayout.Toggle ("Gradient based coloration", script.SkyManager.UseGradients, GUILayout.MaxWidth (380.0f));
						EditorGUILayout.EndHorizontal ();

						if (script.SkyManager.UseGradients) {

							//v3.4.3
							if (script.SkyManager.SkyColorGrad == null || GUILayout.Button (new GUIContent ("Add Sample gradients A"), GUILayout.Width (150))) {
								//EditorGUILayout.
								//script.SkyManager.SkyColorGrad

								GradientColorKey[] Keys = new GradientColorKey[8]; 
								Keys [0] = new GradientColorKey (new Color (77f, 77f, 77f) / 255f, 0.001f);
								Keys [1] = new GradientColorKey (new Color (72f, 73f, 73f) / 255f, 0.257f);
								Keys [2] = new GradientColorKey (new Color (110f, 124f, 124f) / 255f, 0.389f);
								Keys [3] = new GradientColorKey (new Color (211f, 203f, 219f) / 255f, 0.41f);

								Keys [4] = new GradientColorKey (new Color (200f, 210f, 230f) / 255f, 0.63f);
								Keys [5] = new GradientColorKey (new Color (164f, 144f, 135f) / 255f, 0.814f);
								Keys [6] = new GradientColorKey (new Color (135f, 112f, 131f) / 255f, 0.967f);
								Keys [7] = new GradientColorKey (new Color (56f, 48f, 54f) / 255f, 1.000f);
								if (script.SkyManager.SkyColorGrad != null) {
									script.SkyManager.SkyColorGrad.SetKeys (Keys, script.SkyManager.SkyColorGrad.alphaKeys);
								} else {
									script.SkyManager.SkyColorGrad = new Gradient ();
									//script.SkyColorGrad.
									//Debug.Log("sss");
									script.SkyManager.SkyColorGrad.SetKeys (Keys, script.SkyManager.SkyColorGrad.alphaKeys);
								}
								Keys [0] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.001f);
								Keys [1] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.257f);
								Keys [2] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.389f);
								Keys [3] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.41f);

								Keys [4] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.63f);
								Keys [5] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.814f);
								Keys [6] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.967f);
								Keys [7] = new GradientColorKey (new Color (1, 1, 1) / 255f, 1.000f);
								if (script.SkyManager.SkyTintGrad != null) {
									script.SkyManager.SkyTintGrad.SetKeys (Keys, script.SkyManager.SkyTintGrad.alphaKeys);
								} else {
									script.SkyManager.SkyTintGrad = new Gradient ();
									script.SkyManager.SkyTintGrad.SetKeys (Keys, script.SkyManager.SkyTintGrad.alphaKeys);
								}

								EditorUtility.SetDirty (target);
								//Repaint ();

							}

							//show library to choose initial gradient
							if (script.SkyManager.SkyColorGrad == null || GUILayout.Button (new GUIContent ("Add Sample gradients B"), GUILayout.Width (150))) {
								//EditorGUILayout.
								//script.SkyManager.SkyColorGrad

								GradientColorKey[] Keys = new GradientColorKey[8]; 
								Keys [0] = new GradientColorKey (new Color (77f, 77f, 77f) / 255f, 0.001f);
								Keys [1] = new GradientColorKey (new Color (72f, 73f, 73f) / 255f, 0.257f);
								Keys [2] = new GradientColorKey (new Color (110f, 124f, 124f) / 255f, 0.389f);
								Keys [3] = new GradientColorKey (new Color (211f, 203f, 219f) / 255f, 0.41f);

								Keys [4] = new GradientColorKey (new Color (200f, 210f, 230f) / 255f, 0.63f);
								Keys [5] = new GradientColorKey (new Color (204f, 174f, 135f) / 255f, 0.814f);
								Keys [6] = new GradientColorKey (new Color (135f, 112f, 131f) / 255f, 0.967f);
								Keys [7] = new GradientColorKey (new Color (56f, 48f, 54f) / 255f, 1.000f);
								if (script.SkyManager.SkyColorGrad != null) {
									script.SkyManager.SkyColorGrad.SetKeys (Keys, script.SkyManager.SkyColorGrad.alphaKeys);
								} else {
									script.SkyManager.SkyColorGrad = new Gradient ();
									//script.SkyColorGrad.
									//Debug.Log("sss");
									script.SkyManager.SkyColorGrad.SetKeys (Keys, script.SkyManager.SkyColorGrad.alphaKeys);
								}
								Keys [0] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.001f);
								Keys [1] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.257f);
								Keys [2] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.389f);
								Keys [3] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.41f);

								Keys [4] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.63f);
								Keys [5] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.814f);
								Keys [6] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.967f);
								Keys [7] = new GradientColorKey (new Color (1, 1, 1) / 255f, 1.000f);
								if (script.SkyManager.SkyTintGrad != null) {
									script.SkyManager.SkyTintGrad.SetKeys (Keys, script.SkyManager.SkyTintGrad.alphaKeys);
								} else {
									script.SkyManager.SkyTintGrad = new Gradient ();
									script.SkyManager.SkyTintGrad.SetKeys (Keys, script.SkyManager.SkyTintGrad.alphaKeys);
								}

								EditorUtility.SetDirty (target);
								//Repaint ();

							}

                            //v4.8
                            //show library to choose initial gradient
                            if (script.SkyManager.SkyColorGrad == null || GUILayout.Button(new GUIContent("Add Sample gradients C"), GUILayout.Width(150)))
                            {
                                //EditorGUILayout.
                                //script.SkyManager.SkyColorGrad

                                GradientColorKey[] Keys = new GradientColorKey[8];
                                Keys[0] = new GradientColorKey(new Color(77f, 77f, 77f) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(new Color(72f, 73f, 73f) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(new Color(110f, 124f, 124f) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(new Color(203f, 219f, 219f) / 255f, 0.41f);

                                Keys[4] = new GradientColorKey(new Color(200f, 210f, 230f) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(new Color(204f, 146f, 36f) / 255f, 0.814f);
                                //Keys[6] = new GradientColorKey(new Color(135f, 112f, 131f) / 255f, 0.967f);//v4.8.4
                                Keys[6] = new GradientColorKey(new Color(75.0f, 60.0f, 50.0f) / 255f, 0.93f);//v4.8.4
                                Keys[7] = new GradientColorKey(new Color(56f, 48f, 54f) / 255f, 1.000f);
                                if (script.SkyManager.SkyColorGrad != null)
                                {
                                    script.SkyManager.SkyColorGrad.SetKeys(Keys, script.SkyManager.SkyColorGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.SkyColorGrad = new Gradient();
                                    //script.SkyColorGrad.
                                    //Debug.Log("sss");
                                    script.SkyManager.SkyColorGrad.SetKeys(Keys, script.SkyManager.SkyColorGrad.alphaKeys);
                                }
                                Keys[0] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.41f);

                                Keys[4] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.814f);
                                //Keys[6] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.967f);
                                Keys[6] = new GradientColorKey(new Color(1, 1, 1) / 255f, 0.93f);
                                Keys[7] = new GradientColorKey(new Color(1, 1, 1) / 255f, 1.000f);
                                if (script.SkyManager.SkyTintGrad != null)
                                {
                                    script.SkyManager.SkyTintGrad.SetKeys(Keys, script.SkyManager.SkyTintGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.SkyTintGrad = new Gradient();
                                    script.SkyManager.SkyTintGrad.SetKeys(Keys, script.SkyManager.SkyTintGrad.alphaKeys);
                                }

                                EditorUtility.SetDirty(target);
                                //Repaint ();

                            }

                            script.SkyColorGrad = script.SkyManager.SkyColorGrad;
							script.SkyTintGrad = script.SkyManager.SkyTintGrad;
							EditorGUILayout.PropertyField (SkyColorGrad);
							EditorGUILayout.PropertyField (SkyTintGrad);
							script.SkyManager.SkyColorGrad = script.SkyColorGrad;
							script.SkyManager.SkyTintGrad = script.SkyTintGrad;

							EditorGUILayout.HelpBox ("Sky Brightness curve", MessageType.None);
							if (script.SkyManager.FexposureC != null) {
								//script.SkyManager.FexposureC = EditorGUILayout.CurveField (script.SkyManager.FexposureC, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.FexposureC = script.SkyManager.FexposureC;
								EditorGUILayout.PropertyField(FexposureC, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.SkyManager.FexposureC = script.FexposureC;
							} else {
								Keyframe[] CurveKeys = new Keyframe[4];
								CurveKeys [0] = new Keyframe (0, 0.3f);
								CurveKeys [1] = new Keyframe (0.3f, 0.0f);
								CurveKeys [2] = new Keyframe (0.7f, 0.0f);
								CurveKeys [3] = new Keyframe (1, 0.3f);
								script.SkyManager.FexposureC = new AnimationCurve (CurveKeys);
							}

							EditorGUILayout.HelpBox ("Sun light spread curve", MessageType.None);
							if (script.SkyManager.FscaleDiffC != null) {
								//script.SkyManager.FscaleDiffC = EditorGUILayout.CurveField (script.SkyManager.FscaleDiffC, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.FscaleDiffC = script.SkyManager.FscaleDiffC;
								EditorGUILayout.PropertyField(FscaleDiffC, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.SkyManager.FscaleDiffC = script.FscaleDiffC;
							} else {
								Keyframe[] CurveKeys = new Keyframe[4];
								CurveKeys [0] = new Keyframe (0, 0.001f);
								CurveKeys [1] = new Keyframe (0.3f, 0.001f);
								CurveKeys [2] = new Keyframe (0.7f, 0.001f);
								CurveKeys [3] = new Keyframe (1, 0.001f);
								script.SkyManager.FscaleDiffC = new AnimationCurve (CurveKeys);
							}


							EditorGUILayout.HelpBox ("Sun & moon halo size", MessageType.None);
							if (script.SkyManager.FSunGC != null) {
								//script.SkyManager.FSunGC = EditorGUILayout.CurveField (script.SkyManager.FSunGC, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.FSunGC = script.SkyManager.FSunGC;
								EditorGUILayout.PropertyField(FSunGC, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.SkyManager.FSunGC = script.FSunGC;
							} else {
								Keyframe[] CurveKeys = new Keyframe[4];
								CurveKeys [0] = new Keyframe (0, 0.95f);
								CurveKeys [1] = new Keyframe (0.3f, 0.99f);
								CurveKeys [2] = new Keyframe (0.7f, 0.99f);
								CurveKeys [3] = new Keyframe (1, 0.95f);
								script.SkyManager.FSunGC = new AnimationCurve (CurveKeys);
							}
							EditorGUILayout.HelpBox ("Sun ring factor", MessageType.None);
							if (script.SkyManager.FSunringC != null) {
								//script.SkyManager.FSunringC = EditorGUILayout.CurveField (script.SkyManager.FSunringC, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.FSunringC = script.SkyManager.FSunringC;
								EditorGUILayout.PropertyField(FSunringC, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.SkyManager.FSunringC = script.FSunringC;
							} else {
								Keyframe[] CurveKeys = new Keyframe[4];
								CurveKeys [0] = new Keyframe (0, 0.0f);
								CurveKeys [1] = new Keyframe (0.3f, 0.0f);
								CurveKeys [2] = new Keyframe (0.7f, 0.0f);
								CurveKeys [3] = new Keyframe (1, 0.0f);
								script.SkyManager.FSunringC = new AnimationCurve (CurveKeys);
							}


							//EditorGUILayout.HelpBox ("Use Sky Gradient on Water", MessageType.None);
							if (script.WaterManager != null) {
								script.WaterManager.UseSkyGradient = EditorGUILayout.Toggle ("Gradient on Water", script.WaterManager.UseSkyGradient, GUILayout.MaxWidth (380.0f));
							}
						} else {
							if (script.WaterManager != null) {
								script.WaterManager.UseSkyGradient = false;
							}
						}

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	
						EditorGUILayout.HelpBox ("Moon halo intensity", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Moon_glow = EditorGUILayout.Slider (script.SkyManager.Moon_glow, 0.1f, 20, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						//v3.3e
						EditorGUILayout.HelpBox ("Moon size", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.MoonSize = EditorGUILayout.Slider (script.SkyManager.MoonSize, 0.05f, 1, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));

						//		GUILayout.Box("",GUILayout.Height(2),GUILayout.Width(410));	
						//EditorGUILayout.HelpBox("Define map center",MessageType.None);
						EditorGUILayout.HelpBox ("World scale", MessageType.None);
						//script.SkyManager.SunSystem.transform.localScale = EditorGUILayout.FloatField(script.SkyManager.SunSystem.transform.localScale.x,GUILayout.MaxWidth(95.0f))*Vector3.one;
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.WorldScale = EditorGUILayout.Slider (script.SkyManager.WorldScale, 1, 50, GUILayout.MaxWidth (sliderWidth));

						script.SkyManager.SunSystem.transform.localScale = (script.SkyManager.WorldScale / 2) * Vector3.one;
						//v3.4.1
						if (script.SkyManager.LatLonMoonPos) {
							script.SkyManager.MoonCenterObj.transform.localScale = (script.SkyManager.WorldScale / 20) * Vector3.one;
						}

						EditorGUILayout.EndHorizontal ();

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (415));	

						EditorGUILayout.BeginHorizontal ();
						script.DontScaleParticleProps = EditorGUILayout.Toggle ("No particle size scaling", script.DontScaleParticleProps, GUILayout.MaxWidth (380.0f));
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						script.DontScaleParticleTranf = EditorGUILayout.Toggle ("No particles bounding box scaling", script.DontScaleParticleTranf, GUILayout.MaxWidth (380.0f));
						EditorGUILayout.EndHorizontal ();

						//SCALE shader based cloud dome
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();

						//SCALE PARTICLE CLOUDS
						if (script.SkyManager.WorldScale != script.SkyManager.prevWorldScale) {

							//scale volume cloud positioning
							script.SkyManager.VolCloudsHorScale = 1000 * (script.SkyManager.WorldScale / 20);
							script.SkyManager.VolCloudHeight = 650 * (script.SkyManager.WorldScale / 20);

							//scale stars
							float prev_scale = script.SkyManager.prevWorldScale;
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Star_particles_OBJ, true);

							//scale moon in lat-lon mode v3.3
							if (script.SkyManager.LatLonMoonPos) {
								script.SkyManager.MoonObj.transform.localPosition = new Vector3 (script.SkyManager.MoonObj.transform.localPosition.x, script.SkyManager.MoonObj.transform.localPosition.y, 25800 * (script.SkyManager.WorldScale / 20));
							}

							//SCALE shader based cloud dome
							script.SkyManager.CloudDomeL1.transform.localScale = new Vector3 (35404, 33044, 35405) * (script.SkyManager.WorldScale / 20);
							script.SkyManager.CloudDomeL2.transform.localScale = new Vector3 (35404, 21190, 35405) * (script.SkyManager.WorldScale / 20);

							//scale clouds
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Sun_Ray_Cloud, true);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Star_particles_OBJ, false);

							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Upper_Dynamic_Cloud, true);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Lower_Dynamic_Cloud, true);

							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Upper_Cloud_Bed, true);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Lower_Cloud_Bed, true);

							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Lower_Cloud_Real, true);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Upper_Cloud_Real, true);

							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Upper_Static_Cloud, true);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Lower_Static_Cloud, true);

							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Surround_Clouds, true);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Surround_Clouds_Heavy, true);

							//WEATHER PARTICLES
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.SnowStorm_OBJ, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Butterfly_OBJ, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Ice_Spread_OBJ, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Ice_System_OBJ, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Lightning_OBJ, false);

							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Lightning_System_OBJ, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.VolumeFog_OBJ, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Rain_Heavy, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Rain_Mild, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.VolumeRain_Heavy, false);

							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.VolumeRain_Mild, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.RefractRain_Heavy, false);
							ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.RefractRain_Mild, false);

							if (script.SkyManager.FallingLeaves_OBJ != null) { //v3.3
								for (int i = 0; i < script.SkyManager.FallingLeaves_OBJ.Length; i++) {
									ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.FallingLeaves_OBJ [i], false);
								}
							}
							if (script.SkyManager.Tornado_OBJs != null) { //v3.3
								for (int i = 0; i < script.SkyManager.Tornado_OBJs.Length; i++) {
									ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Tornado_OBJs [i], false);
								}
							}

							if (script.SkyManager.Butterfly3D_OBJ != null) { //v3.3
								for (int i = 0; i < script.SkyManager.Butterfly3D_OBJ.Length; i++) {
									ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Butterfly3D_OBJ [i], false);
								}
							}
							if (script.SkyManager.Volcano_OBJ != null) { //v3.3
								for (int i = 0; i < script.SkyManager.Volcano_OBJ.Length; i++) {
									ScaleMe ((script.SkyManager.WorldScale / prev_scale), script.SkyManager.Volcano_OBJ [i], false);
								}
							}

							//SKY DOME
							if (script.SkyManager.SkyDomeSystem != null) {
								script.SkyManager.SkyDomeSystem.localScale = new Vector3 (11936.62f, 11936.62f, 11936.62f) * (script.SkyManager.WorldScale / 20);
							}

							//RAIN DROPS PLANE
							if (script.SkyManager.RainDropsPlane != null) {
								script.SkyManager.RainDropsPlane.transform.localScale = new Vector3 (0.6486322f, 0.6486322f, 0.6486322f) * (script.SkyManager.WorldScale / 20);
							}

							//WATER
							if (script.SkyManager.water != null) {
								script.SkyManager.water.localScale = new Vector3 (3f, 1f, 1f) * (script.SkyManager.WorldScale / 20);
							}

							script.SkyManager.prevWorldScale = script.SkyManager.WorldScale;
						}

						//SCALE WATER
				


						//REFINE SKY
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.HelpBox ("Setup Sky & Fog(Realistic) - Add & Setup terrain first", MessageType.None);
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (SkyOptA, GUILayout.MaxWidth (380.0f), GUILayout.MaxHeight (120.0f))) {
							script.SkyManager.Preset = 11;
							if (script.TerrainManager != null) {
								script.TerrainManager.FogPreset = 11;
							} else {
								Debug.Log ("Please add a terrain and set it up with SkyMaster in order to set the volumetric fog preset in SeasonalTerrainSKYMASTER script");
							}
						}
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.HelpBox ("Setup Sky & Fog (Fantasy) - Add & Setup terrain first", MessageType.None);
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (SkyOptB, GUILayout.MaxWidth (380.0f), GUILayout.MaxHeight (120.0f))) {
							script.SkyManager.Preset = 0;
							if (script.TerrainManager != null) {
								script.TerrainManager.FogPreset = 0;
							} else {
								Debug.Log ("Please add a terrain and set it up with SkyMaster in order to set the volumetric fog preset in SeasonalTerrainSKYMASTER script");
							}
						}
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.HelpBox ("Setup Sky & Fog (Mild) - Add & Setup terrain first", MessageType.None);
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (SkyOptC, GUILayout.MaxWidth (380.0f), GUILayout.MaxHeight (120.0f))) {
							script.SkyManager.Preset = 9;
							if (script.TerrainManager != null) {
								script.TerrainManager.FogPreset = 0;
							} else {
								Debug.Log ("Please add a terrain and set it up with SkyMaster in order to set the volumetric fog preset in SeasonalTerrainSKYMASTER script");
							}
						}
						EditorGUILayout.EndHorizontal ();
					}
//				EditorGUILayout.BeginHorizontal();
//				
//				ParticleScaleFactor = EditorGUILayout.FloatField(ParticleScaleFactor,GUILayout.MaxWidth(95.0f));
//				ParticlePlaybackScaleFactor = EditorGUILayout.FloatField(ParticlePlaybackScaleFactor,GUILayout.MaxWidth(95.0f));
//				EditorGUILayout.EndHorizontal();
				
					//	MainParticle =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
				
					//	Exclude_children = EditorGUILayout.Toggle("Exclude children", Exclude_children,GUILayout.MaxWidth(180.0f));
					//	Include_inactive = EditorGUILayout.Toggle("Include inactive", Include_inactive,GUILayout.MaxWidth(180.0f));


				
				}
				EditorGUILayout.EndVertical ();
			
				////////////////////////////////////////////////////////////

			}//END TAB0


			//v3.4.3

			//TAB1
			if (script.UseTabs && script.currentTab == 1) {				
				if (script.SkyManager == null) {
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (410.0f));
					EditorGUILayout.HelpBox ("Please add Sky to enable Cloud options", MessageType.None);
					EditorGUILayout.EndVertical ();
				}
			}
			//TAB2
			if (script.UseTabs && script.currentTab == 2) {				
				if (script.SkyManager == null) {
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (410.0f));
					EditorGUILayout.HelpBox ("Please add Sky to enable Terrain setup options", MessageType.None);
					EditorGUILayout.EndVertical ();
				}
			}
			//TAB3
			if (script.UseTabs && script.currentTab == 3) {				
				if (script.SkyManager == null) {
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (410.0f));
					EditorGUILayout.HelpBox ("Please add Sky to enable Camera Image effects and Volume Fog options", MessageType.None);
					EditorGUILayout.EndVertical ();
				}
			}
			//TAB4
			if (script.UseTabs && script.currentTab == 4) {				
				if (script.SkyManager == null) {
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (410.0f));
					EditorGUILayout.HelpBox ("Please add Sky to enable Weather options", MessageType.None);
					EditorGUILayout.EndVertical ();
				}
			}
			//TAB5
			if (script.UseTabs && script.currentTab == 5) {				
				if (script.SkyManager == null) {
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (410.0f));
					EditorGUILayout.HelpBox ("Please add Sky to enable Foliage options", MessageType.None);
					EditorGUILayout.EndVertical ();
				}
			}
			//TAB6
			if (script.UseTabs && script.currentTab == 6) {				
				if (script.SkyManager == null) {
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (410.0f));
					EditorGUILayout.HelpBox ("Please add Sky to enable Water options", MessageType.None);
					EditorGUILayout.EndVertical ();
				}
			}
			//TAB7
//			if (script.UseTabs && script.currentTab == 7) {
//				//v3.4.3
//				if (script.SkyManager == null) {
//					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (410.0f)); //v3.3e
//					//GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));
//					//Debug.Log ("Add Sky to enable Weather options");
//					EditorGUILayout.HelpBox ("Please add Sky to enable Weather options", MessageType.None);
//					EditorGUILayout.EndVertical ();
//				}
//			}


			if (script.SkyManager != null) {



                /////////////////////////////////////////////////////////////////////////////// VOLUMETRIC CLOUDS /////////////////////////////////
                //		EditorGUILayout.BeginVertical (GUILayout.MaxWidth (180.0f));			
                //X_offset_left = 200;
                //Y_offset_top = 100;			

                //TAB1
                if ((script.UseTabs && script.currentTab == 1) | !script.UseTabs)
                {


                    GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));

                    //v3.3e
                    GUI.backgroundColor = Color.blue * 0.2f;
                    EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));
                    GUI.backgroundColor = Color.white;


                    GUILayout.Label(MainIcon2, GUILayout.MaxWidth(410.0f));

                    EditorGUILayout.LabelField("Volumetric & Shader based Clouds", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                    GUILayout.Space(10);
                    script.cloud_folder2 = EditorGUILayout.Foldout(script.cloud_folder2, "Shader based Cloud dome (Simple clouds, good for mobile)");
                    EditorGUILayout.EndHorizontal();

                    if (script.cloud_folder2)
                    {

                        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));
                        //control L1 dome
                        EditorGUILayout.HelpBox("Cloud shift - density control", MessageType.None);
                        EditorGUILayout.BeginHorizontal();
                        script.SkyManager.L1CloudDensOffset = EditorGUILayout.Slider(script.SkyManager.L1CloudDensOffset, 0, 10, GUILayout.MaxWidth(sliderWidth));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.HelpBox("Cloud lower layer size", MessageType.None);
                        EditorGUILayout.BeginHorizontal();
                        script.SkyManager.L1CloudSize = EditorGUILayout.Slider(script.SkyManager.L1CloudSize, 0.1f, 10, GUILayout.MaxWidth(sliderWidth));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.HelpBox("Cloud coverage offset", MessageType.None);
                        EditorGUILayout.BeginHorizontal();
                        script.SkyManager.L1CloudCoverOffset = EditorGUILayout.Slider(script.SkyManager.L1CloudCoverOffset, 0, 0.2f, GUILayout.MaxWidth(sliderWidth));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.HelpBox("Cloud speed offset (over windzone effect)", MessageType.None);
                        EditorGUILayout.BeginHorizontal();
                        script.SkyManager.L12SpeedOffset = EditorGUILayout.Slider(script.SkyManager.L12SpeedOffset, -5f, 5f, GUILayout.MaxWidth(sliderWidth));
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(415));

                        EditorGUILayout.HelpBox("Cloud ambience", MessageType.None);
                        EditorGUILayout.BeginHorizontal();
                        script.SkyManager.L1Ambience = EditorGUILayout.Slider(script.SkyManager.L1Ambience, 0.1f, 1.5f, GUILayout.MaxWidth(sliderWidth));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                    }

                    //v4.8 - FUL LVOLUME CLOUDS 1

                    GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                    GUILayout.Space(10);
                    script.cloud_folder3 = EditorGUILayout.Foldout(script.cloud_folder3, "Volumetric Clouds v4.8 (Cutting edge, Image effect based clouds)");
                    EditorGUILayout.EndHorizontal();

                    if (script.cloud_folder3)
                    {
                        //
                        {
                            EditorGUILayout.HelpBox("Setup Clouds", MessageType.None);

                            //if (script.SkyManager.volumeClouds == null && GUILayout.Button("Create Volumeric Clouds v4.8"))
                            //{
                            //    //v4.8.4
                            //    Camera.main.farClipPlane = 30000;

                            //    FullVolumeCloudsSkyMaster cloudsScript = Camera.main.gameObject.GetComponent<FullVolumeCloudsSkyMaster>();
                            //    if (cloudsScript != null)
                            //    {
                            //        cloudsScript.Sun = script.SkyManager.SunObj.transform; //volumeFog.Sun;
                            //        cloudsScript.SkyManager = script.SkyManager; //volumeFog.SkyManager;

                            //        //cloudsScript.useTOD = true;
                            //        //cloudsScript.unity2018 = true;
                            //        //cloudsScript.useWeather = true;
                            //        //cloudsScript._ExposureUnder = 1;
                            //        //cloudsScript._GroundColor = new Vector3(1, 1, 1) * 0.8f;
                            //        //cloudsScript._SunSize = 20;// 35;
                            //        //cloudsScript._BackShade = 0.1f;
                            //        //cloudsScript.updateShadows = true;
                            //        //cloudsScript.shadowsUpdate();
                            //        //cloudsScript.updateReflectionCamera = true;

                            //        //cloudsScript._HorizonYAdjust = 1500;
                            //        //cloudsScript._NoiseFreq1 = 3.9f;
                            //        //cloudsScript._NoiseFreq2 = 39f;
                            //        //cloudsScript._NoiseAmp1 = 5.32f;
                            //        //cloudsScript._NoiseAmp2 = 4.04f;
                            //        //cloudsScript._NoiseBias = -3.03f;
                            //        //cloudsScript._Altitude0 = 2300;
                            //        //cloudsScript._Altitude1 = 4500;
                            //        //cloudsScript._Scatter = 0.02f;
                            //        //cloudsScript._Extinct = 0.007f;
                            //        //cloudsScript._ExposureUnder = 1;
                            //        //cloudsScript._GroundColor = new Vector3(1.82f,1.8f,1.8f);
                            //        //cloudsScript._SunSize = 16;
                            //        //cloudsScript._BackShade = 0.08f;
                            //        //cloudsScript._UndersideCurveFactor = 0.79f;

                            //        cloudsScript.initVariablesScatter();

                            //        //cloudsScript.initVariablesA();
                            //        script.SkyManager.volumeClouds = cloudsScript; //script.CloudsScript = cloudsScript;
                            //        //MoveComponentToBottom(Camera.main.gameObject);
                            //    }
                            //    else
                            //    {
                            //        //GlobalFogSkyMaster volumeFog = Camera.main.gameObject.GetComponent<GlobalFogSkyMaster>();
                            //        cloudsScript = Camera.main.gameObject.AddComponent<FullVolumeCloudsSkyMaster>();
                            //        cloudsScript.Sun = script.SkyManager.SunObj.transform; //volumeFog.Sun;
                            //        cloudsScript.SkyManager = script.SkyManager; //volumeFog.SkyManager;

                            //        cloudsScript.initVariablesA();

                            //        cloudsScript.initVariablesScatter();

                            //        //cloudsScript.useTOD = true;
                            //        //cloudsScript.unity2018 = true;
                            //        //cloudsScript.useWeather = true;
                            //        //cloudsScript._ExposureUnder = 1;
                            //        //cloudsScript._GroundColor = new Vector3(1, 1, 1) * 0.8f;
                            //        //cloudsScript._SunSize = 20;// 35;
                            //        //cloudsScript._BackShade = 0.1f;
                            //        //cloudsScript.updateShadows = true;
                            //        //cloudsScript.shadowsUpdate();
                            //        //cloudsScript.updateReflectionCamera = true;

                            //        //cloudsScript._HorizonYAdjust = 1500;
                            //        //cloudsScript._NoiseFreq1 = 3.9f;
                            //        //cloudsScript._NoiseFreq2 = 39f;
                            //        //cloudsScript._NoiseAmp1 = 5.32f;
                            //        //cloudsScript._NoiseAmp2 = 4.04f;
                            //        //cloudsScript._NoiseBias = -3.03f;
                            //        //cloudsScript._Altitude0 = 2300;
                            //        //cloudsScript._Altitude1 = 4500;
                            //        //cloudsScript._Scatter = 0.02f;
                            //        //cloudsScript._Extinct = 0.007f;
                            //        //cloudsScript._ExposureUnder = 1;
                            //        //cloudsScript._GroundColor = new Vector3(1.82f, 1.8f, 1.8f);
                            //        //cloudsScript._SunSize = 16;
                            //        //cloudsScript._BackShade = 0.08f;
                            //        //cloudsScript._UndersideCurveFactor = 0.79f;

                            //        script.SkyManager.volumeClouds = cloudsScript; //script.CloudsScript = cloudsScript;

                            //        MoveComponentToBottom(Camera.main.gameObject);
                            //    }

                            //    //v4.8.4
                            //    EditorUtility.SetDirty(target);
                            //    EditorUtility.SetDirty(script.SkyManager);
                            //}

                            //v4.8.2
                            //if (script.SkyManager.volumeClouds != null && GUILayout.Button("Add Temporal Anti Alising in Camera"))
                            //{
                            //    //Use Temporal AA to lower cloud raycast steps
                            //    if(Camera.main != null && Camera.main.gameObject.GetComponent<TemporalReprojection>() == null)
                            //    {
                            //        Camera.main.gameObject.AddComponent<TemporalReprojection>();
                            //    }
                            //}

                            //if (script.SkyManager.volumeClouds != null && GUILayout.Button("Create Reflections on Sky Master Water"))
                            //{
                            //    FullVolumeCloudsSkyMaster CloudsScript = script.SkyManager.volumeClouds;
                            //    if (CloudsScript != null)
                            //    {
                            //        if (script.SkyManager.water != null)
                            //        { //v4.8.5
                            //            if (CloudsScript.reflectClouds == null) //v4.8.5
                            //            {
                            //                //check if volume script already on reflect camera
                            //                if (script.SkyManager.water.GetComponent<PlanarReflectionSM>().m_ReflectionCameraOut != null
                            //                    && script.SkyManager.water.GetComponent<PlanarReflectionSM>().m_ReflectionCameraOut.GetComponent<FullVolumeCloudsSkyMaster>() != null)
                            //                {
                            //                    //clouds exist already, handle this here
                            //                    FullVolumeCloudsSkyMaster Rclouds = script.SkyManager.water.GetComponent<PlanarReflectionSM>().m_ReflectionCameraOut.GetComponent<FullVolumeCloudsSkyMaster>();
                            //                    CloudsScript.updateReflectionCamera = true;
                            //                    CloudsScript.reflectClouds = Rclouds;
                            //                    Debug.Log("Cloud script found on reflection camera, adding auto update based on main clouds system");

                            //                    CloudsScript.reflectClouds.startDistance = 10000000000; //v4.8.5
                            //                    CloudsScript.reflectClouds.Sun = CloudsScript.Sun;//v4.8.5
                            //                }
                            //                else
                            //                {
                            //                    if (script.SkyManager.water.GetComponent<PlanarReflectionSM>().m_ReflectionCameraOut != null) //v4.8.5
                            //                    {
                            //                        CloudsScript.updateReflectionCamera = true;
                            //                        CloudsScript.updateReflections();

                            //                        //script.CloudsScript.updateReflectionCamera
                            //                        CloudsScript.reflectClouds._HorizonYAdjust = -500;
                            //                        CloudsScript.reflectClouds._FarDist = CloudsScript._FarDist / 2;

                            //                        //v4.8
                            //                        CloudsScript.reflectClouds.isForReflections = true;
                            //                        //remove back layer from refections
                            //                        int layerToCheck = LayerMask.NameToLayer("Background");
                            //                        //backgroundCam.cullingMask = 1 << layerToCheck;
                            //                        script.SkyManager.water.GetComponent<PlanarReflectionSM>().m_ReflectionCameraOut.cullingMask &= ~(1 << layerToCheck);
                            //                        script.SkyManager.water.GetComponent<PlanarReflectionSM>().reflectionMask &= ~(1 << layerToCheck);

                            //                        Debug.Log("Created clouds");

                            //                        if (!Application.isPlaying)
                            //                        {
                            //                            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); //EditorApplication.MarkSceneDirty(); //v4.1e
                            //                        }

                            //                        CloudsScript.reflectClouds.startDistance = 10000000000; //v4.8.5
                            //                        CloudsScript.reflectClouds.Sun = CloudsScript.Sun;//v4.8.5
                            //                    }
                            //                    else
                            //                    {
                            //                        Debug.Log("No reflection camera in scene, please enable the water and Planer reflection script components.");
                            //                    }
                            //                }
                            //            }
                            //            else
                            //            {
                            //                Debug.Log("Reflection cloud script already setup on reflect camera");

                            //                //v4.8
                            //                CloudsScript.updateReflectionCamera = true;
                            //                CloudsScript.updateReflections();
                            //                CloudsScript.reflectClouds.isForReflections = true;
                            //                //remove back layer from refections
                            //                int layerToCheck = LayerMask.NameToLayer("Background");
                            //                //backgroundCam.cullingMask = 1 << layerToCheck;
                            //                script.SkyManager.water.GetComponent<PlanarReflectionSM>().m_ReflectionCameraOut.cullingMask &= ~(1 << layerToCheck);
                            //                script.SkyManager.water.GetComponent<PlanarReflectionSM>().reflectionMask &= ~(1 << layerToCheck);

                            //                CloudsScript.reflectClouds.startDistance = 10000000000; //v4.8.5
                            //                CloudsScript.reflectClouds.Sun = CloudsScript.Sun;//v4.8.5
                            //            }

                            //            //CloudsScript.reflectClouds.startDistance = 10000000000; //v4.8
                            //            //CloudsScript.reflectClouds.Sun = CloudsScript.Sun;//v4.8
                            //        }
                            //        else
                            //        {
                            //            Debug.Log("No Water in scene, please add water component first in Water section");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("No Clouds");
                            //    }
                            //}

                            //if (script.SkyManager.volumeClouds != null && GUILayout.Button("Setup Shadows"))
                            //{
                            //    FullVolumeCloudsSkyMaster CloudsScript = script.SkyManager.volumeClouds;
                            //    if (CloudsScript != null && CloudsScript.shadowDome == null)
                            //    {
                            //        CloudsScript.setupShadows = true;
                            //        CloudsScript.createShadowDome();
                            //        CloudsScript.shadowsUpdate();
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("Shadows already setup");
                            //    }
                            //}

                            //if (script.SkyManager.volumeClouds != null && GUILayout.Button("Setup Back Layer"))
                            //{
                            //    FullVolumeCloudsSkyMaster CloudsScript = script.SkyManager.volumeClouds;

                            //    //v4.8.5
                            //    var layerToCheck = LayerMask.NameToLayer("Background");
                            //    if (layerToCheck > -1)
                            //    {
                            //        if (CloudsScript != null && CloudsScript.backgroundCam == null)
                            //        {
                            //            CloudsScript.setupDepth = true;
                            //            CloudsScript.createDepthSetup();
                            //            CloudsScript.setupDepth = true;
                            //            CloudsScript.blendBackground = true;
                            //        }
                            //        else
                            //        {
                            //            Debug.Log("Depth camera already setup");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("Please add the Background layer to proceed with the setup");
                            //    }
                            //}

                            //if (script.SkyManager.volumeClouds != null && GUILayout.Button("Setup Lightning"))
                            //{
                            //    FullVolumeCloudsSkyMaster CloudsScript = script.SkyManager.volumeClouds;
                            //    if (CloudsScript != null && CloudsScript.LightningBox == null)
                            //    {
                            //        CloudsScript.setupLightning = true;
                            //        CloudsScript.createLightningBox();
                            //        //script.CloudsScript.shadowsUpdate ();
                            //        CloudsScript.EnableLightning = true;
                            //        CloudsScript.lightning_every = 5;
                            //        CloudsScript.max_lightning_time = 9;
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("Lightning components already setup");
                            //    }
                            //}

                            //if (script.SkyManager.volumeClouds != null && GUILayout.Button("Setup Local Light"))
                            //{
                            //    FullVolumeCloudsSkyMaster CloudsScript = script.SkyManager.volumeClouds;
                            //    if (CloudsScript != null && CloudsScript.EnableLightning && CloudsScript.localLight == null)
                            //    {
                            //        CloudsScript.useLocalLightLightn = true;
                            //        //create local light
                            //        GameObject localLight = new GameObject();
                            //        localLight.name = "Clouds Local Light";
                            //        Light actuallight = localLight.AddComponent<Light>();
                            //        actuallight.type = LightType.Point;
                            //        actuallight.range = 2000;
                            //        localLight.transform.position = Camera.main.transform.forward * 1000 + new Vector3(0, 2000, 0);

                            //        CloudsScript.localLight = actuallight; //v4.8
                            //    }
                            //    else
                            //    {
                            //        Debug.Log("Local light already setup");
                            //    }
                            //}
                            //v4.8
                            //CONTROL CLOUD SETTINGS
                            //if (script.SkyManager.volumeClouds != null)
                            //{
                            //    //FullVolumeCloudsSkyMaster CloudS = script.SkyManager.volumeClouds;

                            //    //performance
                            //    EditorGUILayout.HelpBox("Raycast light steps", MessageType.None);
                            //    CloudS._SampleCountL = (int)EditorGUILayout.Slider(CloudS._SampleCountL, 1, 20, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Raycast noise steps", MessageType.None);
                            //    CloudS._SampleCount1 = (int)EditorGUILayout.Slider(CloudS._SampleCount1, 1, 220, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Scale water reflection noise steps (steps:" + (CloudS._SampleCount1 / CloudS.downscaleReflectNoiseSteps).ToString("F1") + ")", MessageType.None);
                            //    CloudS.downscaleReflectNoiseSteps = EditorGUILayout.Slider(CloudS.downscaleReflectNoiseSteps, 0.1f, 5, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Split calculation in frames", MessageType.None);
                            //    CloudS.splitPerFrames = (int)EditorGUILayout.Slider(CloudS.splitPerFrames, 0, 8, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Downscale resolution factor", MessageType.None);
                            //    float beforeDownscaleFactor = CloudS.downScaleFactor;
                            //    CloudS.downScaleFactor = (int)EditorGUILayout.Slider(CloudS.downScaleFactor, 1, 4, GUILayout.MaxWidth(sliderWidth));
                            //    if (beforeDownscaleFactor != CloudS.downScaleFactor)
                            //    {
                            //        CloudS._needsReset = true;
                            //        Debug.Log("Downscale factor for clouds set to " + CloudS.downScaleFactor);
                            //    }

                            //    //NIGHT TIME - v4.8.6
                            //    EditorGUILayout.HelpBox("Change cloud color, lighting & density for night time", MessageType.None);
                            //    CloudS.adjustNightLigting = EditorGUILayout.Toggle("", CloudS.adjustNightLigting, GUILayout.MaxWidth(480.0f));
                            //    //v4.8.6
                            //    if (CloudS.adjustNightLigting)
                            //    {
                            //        EditorGUILayout.HelpBox("Shift dawn for Night to Day density change (+1.3 recommended)", MessageType.None);                                    
                            //        CloudS.shift_dawn = EditorGUILayout.Slider(CloudS.shift_dawn, -3, 3, GUILayout.MaxWidth(sliderWidth));
                            //        EditorGUILayout.HelpBox("Shift dusk for Day to Night density change (-1 recommended)", MessageType.None);
                            //        CloudS.shift_dusk = EditorGUILayout.Slider(CloudS.shift_dusk, -3, 3, GUILayout.MaxWidth(sliderWidth));
                            //    }


                            //    //WIND
                            //    EditorGUILayout.HelpBox("Change clouds based on weather", MessageType.None);
                            //    CloudS.useWeather = EditorGUILayout.Toggle("", CloudS.useWeather, GUILayout.MaxWidth(480.0f));
                            //    EditorGUILayout.HelpBox("Change wind based on weather", MessageType.None);
                            //    CloudS.windByWeather = EditorGUILayout.Toggle("", CloudS.windByWeather, GUILayout.MinWidth(380.0f), GUILayout.MaxWidth(480.0f), GUILayout.ExpandWidth(true));

                            //    GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(415));

                            //    EditorGUILayout.HelpBox("Wind Intensity", MessageType.None);
                            //    //EditorGUILayout.BeginHorizontal();

                            //    if (CloudS.useWeather && CloudS.windByWeather)
                            //    {
                            //        EditorGUILayout.HelpBox("Intensity for Sunny, Cloudy & Storm weather", MessageType.None);
                            //        EditorGUILayout.BeginVertical();
                            //        CloudS.windSunny = EditorGUILayout.Slider(CloudS.windSunny, 0, 2, GUILayout.MaxWidth(sliderWidth));
                            //        EditorGUILayout.EndVertical();
                            //        EditorGUILayout.BeginVertical();
                            //        CloudS.windCloudy = EditorGUILayout.Slider(CloudS.windCloudy, 0, 2, GUILayout.MaxWidth(sliderWidth));
                            //        EditorGUILayout.EndVertical();
                            //        CloudS.windStorm = EditorGUILayout.Slider(CloudS.windStorm, 0, 2, GUILayout.MaxWidth(sliderWidth));
                            //    }
                            //    else
                            //    {

                            //        if (CloudS.useWeather && script.SkyManager.windZone != null) //if use weather assign speeds based on wind forward and control only multiplier
                            //        {
                            //            CloudS.windMultiply = EditorGUILayout.Slider(CloudS.windMultiply, 0, 2, GUILayout.MaxWidth(sliderWidth));
                            //        }
                            //        else
                            //        {
                            //            CloudS._Scroll1.x = EditorGUILayout.Slider(CloudS._Scroll1.x, -2, 2, GUILayout.MaxWidth(sliderWidth));
                            //            CloudS._Scroll1.z = EditorGUILayout.Slider(CloudS._Scroll1.z, -2, 2, GUILayout.MaxWidth(sliderWidth));
                            //        }
                            //    }
                            //    //EditorGUILayout.EndHorizontal();
                            //    EditorGUILayout.HelpBox("Wind Direction", MessageType.None);
                            //    EditorGUILayout.BeginHorizontal();
                            //    //rotate wind zone
                            //    if (script.SkyManager.windZone != null)
                            //    {
                            //        float rotation = script.SkyManager.windZone.transform.eulerAngles.y;
                            //        rotation = EditorGUILayout.Slider(rotation, 0, 360, GUILayout.MaxWidth(sliderWidth));
                            //        script.SkyManager.windZone.transform.eulerAngles = new Vector3(0, rotation, 0);
                            //    }
                            //    EditorGUILayout.EndHorizontal();

                            //    //LIGHTNING
                            //    EditorGUILayout.HelpBox("Tint cloud top color based on TOD", MessageType.None);
                            //    CloudS.useTOD = EditorGUILayout.Toggle(CloudS.useTOD, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Tint cloud base color based on TOD", MessageType.None);
                            //    CloudS.tintUnder = EditorGUILayout.Toggle(CloudS.tintUnder, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Tint cloud base color", MessageType.None);
                            //    float normalizer = 8;
                            //    Color colVal = new Color(CloudS.UnderColorMultiplier.x, CloudS.UnderColorMultiplier.y, CloudS.UnderColorMultiplier.z) / normalizer;
                            //    colVal = EditorGUILayout.ColorField(colVal, GUILayout.MaxWidth(sliderWidth));
                            //    CloudS.UnderColorMultiplier = new Vector3(colVal.r, colVal.g, colVal.b) * normalizer;

                            //    //v4.8.6
                            //    if (CloudS.adjustNightLigting)
                            //    {
                            //        EditorGUILayout.HelpBox("Tint cloud base color (Night)", MessageType.None);
                            //        //float normalizer = 8;
                            //        Color colValN = new Color(CloudS.groundColorNight.x, CloudS.groundColorNight.y, CloudS.groundColorNight.z) / normalizer;
                            //        colValN = EditorGUILayout.ColorField(colValN, GUILayout.MaxWidth(sliderWidth));
                            //        CloudS.groundColorNight = new Vector3(colValN.r, colValN.g, colValN.b) * normalizer;
                            //    }

                            //    EditorGUILayout.HelpBox("Tint cloud top color", MessageType.None);
                            //    Color colVal1 = new Color(CloudS.colorMultiplier.x, CloudS.colorMultiplier.y, CloudS.colorMultiplier.z) / normalizer;
                            //    colVal1 = EditorGUILayout.ColorField(colVal1, GUILayout.MaxWidth(sliderWidth));
                            //    CloudS.colorMultiplier = new Vector3(colVal1.r, colVal1.g, colVal1.b) * normalizer;

                            //    //OPTIMIZATIONS

                            //    //DENSITY
                            //    EditorGUILayout.HelpBox("Cloud density - Sunny", MessageType.None);
                            //    CloudS.sunnyDensity = EditorGUILayout.Slider(CloudS.sunnyDensity, 0, 1.5f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Cloud density - Cloudy", MessageType.None);
                            //    CloudS.cloudyDensity = EditorGUILayout.Slider(CloudS.cloudyDensity, 1, 4, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Cloud density - Storm", MessageType.None);
                            //    CloudS.stormyDensity = EditorGUILayout.Slider(CloudS.stormyDensity, 3, 6, GUILayout.MaxWidth(sliderWidth));
                            //    //

                            //    //FOG
                            //    EditorGUILayout.HelpBox("Horizon Fog density", MessageType.None);
                            //    CloudS.heightDensity = EditorGUILayout.Slider(CloudS.heightDensity, 0.001f, 0.01f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Horizon Fog Height", MessageType.None);
                            //    CloudS.height = EditorGUILayout.Slider(CloudS.height, -100, 1000f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Horizon Adjust", MessageType.None);
                            //    CloudS._HorizonYAdjust = EditorGUILayout.Slider(CloudS._HorizonYAdjust, -8000, 8000f, GUILayout.MaxWidth(sliderWidth));

                            //    //ALTITUDES
                            //    EditorGUILayout.HelpBox("Cloud bottom Height", MessageType.None);
                            //    CloudS._Altitude0 = EditorGUILayout.Slider(CloudS._Altitude0, 0, 5500f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Cloud top Height", MessageType.None);
                            //    CloudS._Altitude1 = EditorGUILayout.Slider(CloudS._Altitude1, 100, 15000f, GUILayout.MaxWidth(sliderWidth));

                            //    //Cloud distances
                            //    EditorGUILayout.HelpBox("Cloud distance day", MessageType.None);
                            //    CloudS._FarDistDay = EditorGUILayout.Slider(CloudS._FarDistDay, 0, 5500000f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Cloud distance night", MessageType.None);
                            //    CloudS._FarDistNight = EditorGUILayout.Slider(CloudS._FarDistNight, 0, 5500000f, GUILayout.MaxWidth(sliderWidth));
                            //    if (CloudS._FarDist != CloudS._FarDistDay) {
                            //        CloudS._FarDist = CloudS._FarDistDay;
                            //    }

                            //    // SCATTER
                            //    EditorGUILayout.HelpBox("Sun exposure", MessageType.None);
                            //    CloudS._SunSize = EditorGUILayout.Slider(CloudS._SunSize, -50, 50, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Atmosphere scatter", MessageType.None);
                            //    CloudS._Scatter = EditorGUILayout.Slider(CloudS._Scatter, 0.002f, 1.8f, GUILayout.MaxWidth(sliderWidth));

                            //    //v4.8.6
                            //    if (CloudS.adjustNightLigting)
                            //    {
                            //        EditorGUILayout.HelpBox("Atmosphere scatter (Night)", MessageType.None);
                            //        CloudS.scatterNight = EditorGUILayout.Slider(CloudS.scatterNight, 0.002f, 6.0f, GUILayout.MaxWidth(sliderWidth));
                            //    }
                            //    //v4.8.6
                            //    EditorGUILayout.HelpBox("Cloud Underside Curvature", MessageType.None);
                            //    CloudS._UndersideCurveFactor = EditorGUILayout.Slider(CloudS._UndersideCurveFactor, -0.1f, 8.0f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Cloud Back shade", MessageType.None);
                            //    CloudS._BackShade = EditorGUILayout.Slider(CloudS._BackShade, 0.002f, 2.0f, GUILayout.MaxWidth(sliderWidth));
                            //    if (CloudS.adjustNightLigting)
                            //    {
                            //        EditorGUILayout.HelpBox("Cloud Back shade (Night)", MessageType.None);
                            //        CloudS.backShadeNight = EditorGUILayout.Slider(CloudS.backShadeNight, 0.002f, 2.0f, GUILayout.MaxWidth(sliderWidth));
                            //    }

                            //    EditorGUILayout.HelpBox("Atmosphere extiction", MessageType.None);
                            //    CloudS._Extinct = EditorGUILayout.Slider(CloudS._Extinct, 0.001f, 0.05f, GUILayout.MaxWidth(sliderWidth));
                            //    //v4.8.6
                            //    if (CloudS.adjustNightLigting)
                            //    {
                            //        EditorGUILayout.HelpBox("Atmosphere extiction (Night)", MessageType.None);
                            //        CloudS.extinctionNight = EditorGUILayout.Slider(CloudS.extinctionNight, 0.001f, 0.08f, GUILayout.MaxWidth(sliderWidth));
                            //    }

                            //    EditorGUILayout.HelpBox("Atmosphere turbidity", MessageType.None);
                            //    CloudS.turbidity = EditorGUILayout.Slider(CloudS.turbidity, 1f, 650f, GUILayout.MaxWidth(sliderWidth));
                            //    //v4.8.6
                            //    if (CloudS.adjustNightLigting)
                            //    {
                            //        EditorGUILayout.HelpBox("Atmosphere turbidity (Night)", MessageType.None);
                            //        CloudS.turbidityNight = EditorGUILayout.Slider(CloudS.turbidityNight, 1f, 650f, GUILayout.MaxWidth(sliderWidth));
                            //    }

                            //    EditorGUILayout.HelpBox("Luminance", MessageType.None);
                            //    CloudS.luminance = EditorGUILayout.Slider(CloudS.luminance, 0.1f, 4f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Luminance factor", MessageType.None);
                            //    CloudS.lumFac = EditorGUILayout.Slider(CloudS.lumFac, 0, 4f, GUILayout.MaxWidth(sliderWidth));

                            //    EditorGUILayout.HelpBox("Sun spread", MessageType.None);
                            //    CloudS.mieDirectionalG = EditorGUILayout.Slider(CloudS.mieDirectionalG, 0.1f, 1f, GUILayout.MaxWidth(sliderWidth));
                            //    EditorGUILayout.HelpBox("Sun bias", MessageType.None);
                            //    CloudS.bias = EditorGUILayout.Slider(CloudS.bias, 0.0f, 8f, GUILayout.MaxWidth(sliderWidth));

                            //    //END
                            //}

                            //if (GUILayout.Button("Move Volume Fog to Top"))
                            //{
                            //    MoveComponentToTop(Camera.main.gameObject);
                            //}
                            //if (GUILayout.Button("Move Volume Fog to Bottom"))
                            //{
                            //    MoveComponentToBottom(Camera.main.gameObject);
                            //}
                            EditorGUIUtility.wideMode = false;
                        }//END FULL OVLUME CLOUDS
                        //EditorGUILayout.EndVertical();
                        ////////////////////////////////////////////////////////////
                    }

                    //END FULL VOLUME CLOUDS 1


                    GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                    GUILayout.Space(10);
                    script.cloud_folder1 = EditorGUILayout.Foldout(script.cloud_folder1, "Volumetric Clouds (Shader based volume clouds, using spherical dome)");
                    EditorGUILayout.EndHorizontal();

                    if (script.cloud_folder1)
                    {

                        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));//v3.3e

                        EditorGUILayout.HelpBox("Volume cloud lit and shadowed areas gradient based coloration", MessageType.None);
                        GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));

                        EditorGUILayout.BeginHorizontal();
                        script.SkyManager.VolCloudGradients = EditorGUILayout.Toggle("Gradient Coloration", script.SkyManager.VolCloudGradients, GUILayout.MaxWidth(380.0f));
                        EditorGUILayout.EndHorizontal();

                        if (script.SkyManager.VolCloudGradients)
                        {

                            //v3.4.3
                            EditorGUILayout.BeginHorizontal();
                            //show library to choose initial gradient
                            if (script.SkyManager.VolCloudShadGrad == null || script.SkyManager.VolCloudFogGrad == null || script.SkyManager.VolCloudLitGrad == null || GUILayout.Button(new GUIContent("Add Sample cloud gradients A"), GUILayout.Width(185)))
                            {

                                GradientColorKey[] Keys = new GradientColorKey[8];

                                Keys[0] = new GradientColorKey(new Color(77f, 77f, 77f) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(new Color(72f, 73f, 73f) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(new Color(110f, 124f, 124f) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(new Color(230f, 189f, 238f) / 255f, 0.41f);//

                                Keys[4] = new GradientColorKey(new Color(200f, 210f, 230f) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(new Color(213f, 157f, 91f) / 255f, 0.814f);// //
                                Keys[6] = new GradientColorKey(new Color(135f, 112f, 131f) / 255f, 0.967f);
                                Keys[7] = new GradientColorKey(new Color(56f, 48f, 54f) / 255f, 1.000f);

                                if (script.SkyManager.VolCloudLitGrad != null)
                                {
                                    script.SkyManager.VolCloudLitGrad.SetKeys(Keys, script.SkyManager.VolCloudLitGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.VolCloudLitGrad = new Gradient();
                                    script.SkyManager.VolCloudLitGrad.SetKeys(Keys, script.SkyManager.VolCloudLitGrad.alphaKeys);
                                }

                                Keys[0] = new GradientColorKey(new Color(77f, 77f, 77f) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(new Color(72f, 73f, 73f) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(new Color(110f, 124f, 124f) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(new Color(211f, 203f, 219f) / 255f, 0.41f);

                                Keys[4] = new GradientColorKey(new Color(200f, 210f, 230f) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(new Color(230f, 90f, 105f) / 255f, 0.78f);//
                                Keys[6] = new GradientColorKey(new Color(180f, 70f, 70f) / 255f, 0.876f);// //
                                Keys[7] = new GradientColorKey(new Color(56f, 48f, 54f) / 255f, 1.000f);

                                if (script.SkyManager.VolCloudFogGrad != null)
                                {
                                    script.SkyManager.VolCloudFogGrad.SetKeys(Keys, script.SkyManager.VolCloudFogGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.VolCloudFogGrad = new Gradient();
                                    script.SkyManager.VolCloudFogGrad.SetKeys(Keys, script.SkyManager.VolCloudFogGrad.alphaKeys);
                                }

                                Keys[0] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.41f);

                                Keys[4] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(1 * new Color(60f, 10f, 10f) / 255f, 0.814f);
                                Keys[6] = new GradientColorKey(1 * new Color(50f, 10f, 10f) / 255f, 0.967f);
                                Keys[7] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 1.000f);

                                if (script.SkyManager.VolCloudShadGrad != null)
                                {
                                    script.SkyManager.VolCloudShadGrad.SetKeys(Keys, script.SkyManager.VolCloudShadGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.VolCloudShadGrad = new Gradient();
                                    script.SkyManager.VolCloudShadGrad.SetKeys(Keys, script.SkyManager.VolCloudShadGrad.alphaKeys);
                                }
                            }

                            //show library to choose initial gradient
                            if (script.SkyManager.VolCloudShadGrad == null || script.SkyManager.VolCloudFogGrad == null || script.SkyManager.VolCloudLitGrad == null || GUILayout.Button(new GUIContent("Add Sample cloud gradients B"), GUILayout.Width(185)))
                            {

                                GradientColorKey[] Keys = new GradientColorKey[8];
                                //								Keys [0] = new GradientColorKey (new Color (77f, 77f, 77f) / 255f, 0.001f);
                                //								Keys [1] = new GradientColorKey (new Color (72f, 73f, 73f) / 255f, 0.257f);
                                //								Keys [2] = new GradientColorKey (new Color (110f, 124f, 124f) / 255f, 0.389f);
                                //								Keys [3] = new GradientColorKey (new Color (211f, 203f, 219f) / 255f, 0.41f);
                                //
                                //								Keys [4] = new GradientColorKey (new Color (200f, 210f, 230f) / 255f, 0.63f);
                                //								Keys [5] = new GradientColorKey (new Color (204f, 174f, 135f) / 255f, 0.814f);
                                //								Keys [6] = new GradientColorKey (new Color (135f, 112f, 131f) / 255f, 0.967f);
                                //								Keys [7] = new GradientColorKey (new Color (56f, 48f, 54f) / 255f, 1.000f);

                                Keys[0] = new GradientColorKey(new Color(77f, 77f, 77f) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(new Color(72f, 73f, 73f) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(new Color(110f, 124f, 124f) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(new Color(230f, 189f, 238f) / 255f, 0.41f);//

                                Keys[4] = new GradientColorKey(new Color(200f, 210f, 230f) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(new Color(253f, 104f, 9f) / 255f, 0.814f);//
                                Keys[6] = new GradientColorKey(new Color(135f, 112f, 131f) / 255f, 0.967f);
                                Keys[7] = new GradientColorKey(new Color(56f, 48f, 54f) / 255f, 1.000f);

                                if (script.SkyManager.VolCloudLitGrad != null)
                                {
                                    script.SkyManager.VolCloudLitGrad.SetKeys(Keys, script.SkyManager.VolCloudLitGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.VolCloudLitGrad = new Gradient();
                                    script.SkyManager.VolCloudLitGrad.SetKeys(Keys, script.SkyManager.VolCloudLitGrad.alphaKeys);
                                }

                                //								Keys [0] = new GradientColorKey (new Color (77f, 77f, 77f) / 255f, 0.001f);
                                //								Keys [1] = new GradientColorKey (new Color (72f, 73f, 73f) / 255f, 0.257f);
                                //								Keys [2] = new GradientColorKey (new Color (110f, 124f, 124f) / 255f, 0.389f);
                                //								Keys [3] = new GradientColorKey (new Color (211f, 203f, 219f) / 255f, 0.41f);
                                //
                                //								Keys [4] = new GradientColorKey (new Color (200f, 210f, 230f) / 255f, 0.63f);
                                //								Keys [5] = new GradientColorKey (new Color (204f, 174f, 135f) / 255f, 0.814f);
                                //								Keys [6] = new GradientColorKey (new Color (135f, 112f, 131f) / 255f, 0.967f);
                                //								Keys [7] = new GradientColorKey (new Color (56f, 48f, 54f) / 255f, 1.000f);

                                Keys[0] = new GradientColorKey(new Color(77f, 77f, 77f) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(new Color(72f, 73f, 73f) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(new Color(110f, 124f, 124f) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(new Color(211f, 203f, 219f) / 255f, 0.41f);

                                Keys[4] = new GradientColorKey(new Color(200f, 210f, 230f) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(new Color(230f, 90f, 105f) / 255f, 0.78f);//
                                Keys[6] = new GradientColorKey(new Color(248f, 16f, 31f) / 255f, 0.876f);//
                                Keys[7] = new GradientColorKey(new Color(56f, 48f, 54f) / 255f, 1.000f);

                                if (script.SkyManager.VolCloudFogGrad != null)
                                {
                                    script.SkyManager.VolCloudFogGrad.SetKeys(Keys, script.SkyManager.VolCloudFogGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.VolCloudFogGrad = new Gradient();
                                    script.SkyManager.VolCloudFogGrad.SetKeys(Keys, script.SkyManager.VolCloudFogGrad.alphaKeys);
                                }

                                //								Keys [0] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.001f);
                                //								Keys [1] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.257f);
                                //								Keys [2] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.389f);
                                //								Keys [3] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.41f);
                                //
                                //								Keys [4] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.63f);
                                //								Keys [5] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.814f);
                                //								Keys [6] = new GradientColorKey (new Color (1, 1, 1) / 255f, 0.967f);
                                //								Keys [7] = new GradientColorKey (new Color (1, 1, 1) / 255f, 1.000f);

                                Keys[0] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.001f);
                                Keys[1] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.257f);
                                Keys[2] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.389f);
                                Keys[3] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.41f);

                                Keys[4] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.63f);
                                Keys[5] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.814f);
                                Keys[6] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 0.967f);
                                Keys[7] = new GradientColorKey(50 * new Color(1, 1, 1) / 255f, 1.000f);

                                if (script.SkyManager.VolCloudShadGrad != null)
                                {
                                    script.SkyManager.VolCloudShadGrad.SetKeys(Keys, script.SkyManager.VolCloudShadGrad.alphaKeys);
                                }
                                else
                                {
                                    script.SkyManager.VolCloudShadGrad = new Gradient();
                                    script.SkyManager.VolCloudShadGrad.SetKeys(Keys, script.SkyManager.VolCloudShadGrad.alphaKeys);
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            script.VolCloudLitGrad = script.SkyManager.VolCloudLitGrad;
                            script.VolCloudShadGrad = script.SkyManager.VolCloudShadGrad;
                            script.VolCloudFogGrad = script.SkyManager.VolCloudFogGrad;
                            EditorGUILayout.PropertyField(VolCloudLitGrad);
                            EditorGUILayout.PropertyField(VolCloudShadGrad);
                            EditorGUILayout.PropertyField(VolCloudFogGrad);
                            script.SkyManager.VolCloudLitGrad = script.VolCloudLitGrad;
                            script.SkyManager.VolCloudShadGrad = script.VolCloudShadGrad;
                            script.SkyManager.VolCloudFogGrad = script.VolCloudFogGrad;

                            if (script.SkyManager.VolShaderCloudsH != null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.FogFromSkyGrad = EditorGUILayout.Toggle("Sky gradient for fog color", script.SkyManager.VolShaderCloudsH.FogFromSkyGrad, GUILayout.MaxWidth(380.0f));
                                EditorGUILayout.EndHorizontal();
                            }

                        }

                        if (script.SkyManager.VolShaderClouds != null || script.SkyManager.VolCloudGradients)
                        {

                            EditorGUILayout.HelpBox("Cloud transparency", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolCloudTransp = EditorGUILayout.Slider(script.SkyManager.VolCloudTransp, 0, 1f, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            //v3.4.3
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button(new GUIContent("Add Sample cloud Curves A"), GUILayout.Width(185)))
                            {
                                script.SkyManager.ApplyCloudCurvesPresetA();
                            }
                            if (GUILayout.Button(new GUIContent("Add Sample cloud Curves B"), GUILayout.Width(185)))
                            {
                                script.SkyManager.ApplyCloudCurvesPresetB();
                            }
                            EditorGUILayout.EndHorizontal();

                            //v3.4
                            EditorGUILayout.HelpBox("Cloud sun intensity", MessageType.None);
                            if (script.SkyManager.IntensitySun != null)
                            {
                                //script.SkyManager.IntensitySun = EditorGUILayout.CurveField (script.SkyManager.IntensitySun, GUILayout.MaxWidth (415.0f));
                                //v3.4.5
                                script.IntensitySun = script.SkyManager.IntensitySun;
                                EditorGUILayout.PropertyField(IntensitySun, GUIContent.none, GUILayout.MaxWidth(415.0f));
                                script.SkyManager.IntensitySun = script.IntensitySun;
                            }
                            else
                            {

                            }

                            EditorGUILayout.HelpBox("Cloud Sun-shadow Difference", MessageType.None);
                            if (script.SkyManager.IntensityDiff != null)
                            {
                                //script.SkyManager.IntensityDiff = EditorGUILayout.CurveField (script.SkyManager.IntensityDiff, GUILayout.MaxWidth (415.0f));
                                //v3.4.5
                                script.IntensityDiff = script.SkyManager.IntensityDiff;
                                EditorGUILayout.PropertyField(IntensityDiff, GUIContent.none, GUILayout.MaxWidth(415.0f));
                                script.SkyManager.IntensityDiff = script.IntensityDiff;
                            }
                            else
                            {

                            }

                            EditorGUILayout.HelpBox("Cloud distant fog", MessageType.None);
                            if (script.SkyManager.IntensityFog != null)
                            {
                                //script.SkyManager.IntensityFog = EditorGUILayout.CurveField (script.SkyManager.IntensityFog, GUILayout.MaxWidth (415.0f));
                                //v3.4.5
                                script.IntensityFog = script.SkyManager.IntensityFog;
                                EditorGUILayout.PropertyField(IntensityFog, GUIContent.none, GUILayout.MaxWidth(415.0f));
                                script.SkyManager.IntensityFog = script.IntensityFog;
                            }
                            else
                            {

                            }
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();

                        //SHADER CLOUDS
                        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));//v3.3e
                        EditorGUILayout.HelpBox("Shader based Volumetric Clouds", MessageType.None);
                        //v3.4
                        if (script.SkyManager.VolShaderClouds == null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button(new GUIContent("Add Shader based volume clouds"), GUILayout.Width(260)))
                            {
                                Vector3 MapcenterPos = script.SkyManager.MapCenter.position;
                                GameObject VolClouds = (GameObject)Instantiate(VolCloudsPREFAB, MapcenterPos + new Vector3(0, 3, 0), Quaternion.identity);
                                script.SkyManager.VolShaderClouds = VolClouds.transform;
                                script.SkyManager.VolShaderCloudsH = VolClouds.GetComponent<CloudHandlerSM>();
                                script.SkyManager.VolShaderCloudsH.WeatherDensity = true;
                                script.SkyManager.VolShaderCloudsH.CurvesFromSkyMaster = true;
                                script.SkyManager.VolShaderCloudsH.SkyManager = script.SkyManager;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            if (script.SkyManager.VolShaderCloudsH != null)
                            {

                            }
                            else
                            {
                                script.SkyManager.VolShaderCloudsH = script.SkyManager.VolShaderClouds.gameObject.GetComponent<CloudHandlerSM>();
                                script.SkyManager.VolShaderCloudsH.WeatherDensity = true;
                                script.SkyManager.VolShaderCloudsH.CurvesFromSkyMaster = true;
                                script.SkyManager.VolShaderCloudsH.SkyManager = script.SkyManager;
                            }
                        }

                        if (script.SkyManager.VolShaderCloudsH != null)
                        {
                            script.SkyManager.VolShaderCloudsH.DomeClouds = EditorGUILayout.Toggle("Dome clouds toggle", script.SkyManager.VolShaderCloudsH.DomeClouds, GUILayout.MaxWidth(380.0f));
                            script.SkyManager.VolShaderCloudsH.MultiQuadClouds = EditorGUILayout.Toggle("Multi quad clouds toggle", script.SkyManager.VolShaderCloudsH.MultiQuadClouds, GUILayout.MaxWidth(380.0f));
                            script.SkyManager.VolShaderCloudsH.MultiQuadAClouds = EditorGUILayout.Toggle("Multi quad clouds A toggle", script.SkyManager.VolShaderCloudsH.MultiQuadAClouds, GUILayout.MaxWidth(380.0f));
                            script.SkyManager.VolShaderCloudsH.MultiQuadBClouds = EditorGUILayout.Toggle("Multi quad clouds B toggle", script.SkyManager.VolShaderCloudsH.MultiQuadBClouds, GUILayout.MaxWidth(380.0f));
                            script.SkyManager.VolShaderCloudsH.MultiQuadCClouds = EditorGUILayout.Toggle("Multi quad clouds C toggle", script.SkyManager.VolShaderCloudsH.MultiQuadCClouds, GUILayout.MaxWidth(380.0f));//v3.4.3
                            script.SkyManager.VolShaderCloudsH.OneQuadClouds = EditorGUILayout.Toggle("One quad clouds toggle", script.SkyManager.VolShaderCloudsH.OneQuadClouds, GUILayout.MaxWidth(380.0f));

                            GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(415));

                            //v3.4.3
                            EditorGUILayout.HelpBox("Offset Sun Intensity", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolShaderCloudsH.IntensitySunOffset = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.IntensitySunOffset, -2, 2, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.HelpBox("Offset Shadow Difference", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.IntensityDiffOffset, -2, 2, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.HelpBox("Offset Fog Density", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolShaderCloudsH.IntensityFogOffset = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.IntensityFogOffset, -12, 12, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(415));

                            if (script.SkyManager.VolShaderCloudsH.DomeClouds)
                            {
                                EditorGUILayout.HelpBox("Cloud dome height", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.DomeHeights.x = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.DomeHeights.x, -1500, 1500, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Cloud dome scale", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.DomeScale.y = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.DomeScale.y, -15, 15, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                            }
                            if (script.SkyManager.VolShaderCloudsH.MultiQuadClouds)
                            {
                                EditorGUILayout.HelpBox("Multi quad clouds height", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadHeights.x = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadHeights.x, -1500, 1500, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Multi quad clouds horizon offset", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadHeights.y = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadHeights.y, -1500, 1500, GUILayout.MaxWidth(sliderWidth));//v3.4.3
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Multi quad clouds scale", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadScale.y = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadScale.y, -15, 15, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Multi quad clouds rotation", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                float Xrot = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles.x, 0, 15, GUILayout.MaxWidth(sliderWidth));//v3.4.3
                                Vector3 Eulers = script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles;
                                script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3(Xrot, Eulers.y, Eulers.z);
                                EditorGUILayout.EndHorizontal();
                            }

                            if (script.SkyManager.VolShaderCloudsH.MultiQuadAClouds)
                            {
                                //multi quad clouds A
                                EditorGUILayout.HelpBox("Multi quad clouds A height", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadAHeights.x = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadAHeights.x, -1500, 1500, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Multi quad clouds A scale", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadAScale.y = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadAScale.y, -15, 15, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                            }

                            if (script.SkyManager.VolShaderCloudsH.MultiQuadBClouds)
                            {
                                //multi quad clouds B
                                EditorGUILayout.HelpBox("Multi quad clouds B height", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadBHeights.x = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadBHeights.x, -1500, 1500, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Multi quad clouds B scale", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadBScale.y = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadBScale.y, -15, 15, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                            }

                            //v3.4.3
                            if (script.SkyManager.VolShaderCloudsH.MultiQuadCClouds)
                            {
                                //multi quad clouds C
                                EditorGUILayout.HelpBox("Multi quad clouds C height", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadCHeights.x = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadCHeights.x, -1500, 1500, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Multi quad clouds C scale", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.MultiQuadCScale.y = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.MultiQuadCScale.y, -15, 15, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Multi quad clouds C rotation", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                float Xrot1 = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.SideCClouds.eulerAngles.x, 0, 15, GUILayout.MaxWidth(sliderWidth));//v3.4.3
                                Vector3 Eulers1 = script.SkyManager.VolShaderCloudsH.SideCClouds.eulerAngles;
                                script.SkyManager.VolShaderCloudsH.SideCClouds.eulerAngles = new Vector3(Xrot1, Eulers1.y, Eulers1.z);
                                EditorGUILayout.EndHorizontal();
                            }



                            if (script.SkyManager.VolShaderCloudsH.OneQuadClouds)
                            {
                                EditorGUILayout.HelpBox("Single quad clouds height", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.OneQuadHeights.x = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.OneQuadHeights.x, -1500, 1500, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Single quad clouds scale", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.VolShaderCloudsH.OneQuadScale.y = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.OneQuadScale.y, -15, 15, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Singe quad clouds rotation", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                float Xrot1 = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.RotClouds.eulerAngles.x, 0, 8, GUILayout.MaxWidth(sliderWidth));
                                Vector3 Eulers1 = script.SkyManager.VolShaderCloudsH.RotClouds.eulerAngles;
                                script.SkyManager.VolShaderCloudsH.RotClouds.eulerAngles = new Vector3(Xrot1, Eulers1.y, Eulers1.z);
                                EditorGUILayout.EndHorizontal();
                            }

                            EditorGUILayout.HelpBox("Cloud horizon", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolShaderCloudsH.ClearDayHorizon = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.ClearDayHorizon, 0f, 1.5f, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.HelpBox("Cloud density", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolShaderCloudsH.CloudDensity = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.CloudDensity, 0.00004f, 0.0006f, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.HelpBox("Cloud coverage clear day", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolShaderCloudsH.ClearDayCoverage = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.ClearDayCoverage, -15, 5, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.HelpBox("Cloud coverage storm", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolShaderCloudsH.StormCoverage = EditorGUILayout.Slider(script.SkyManager.VolShaderCloudsH.StormCoverage, -15, 5, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();


                        //v3.4.3
                        if (script.SkyManager.VolShaderCloudsH != null)
                        {
                            if (script.SkyManager.VolShaderCloudsH.MultiQuadClouds)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Multi Quad - Realistic Low altitude)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUD_SETA_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.37f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = -0.05f;//-0.1f;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = -0.02f;//0.03f;//0.06f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = 0f;
                                    ////									script.SkyManager.VolShaderCloudsH.DomeScale.y = 3f;//1.2f;//1.5f;
                                    ////									script.SkyManager.VolShaderCloudsH.DomeHeights.x = 1005;//987;
                                    ////									script.SkyManager.VolShaderCloudsH.DomeHeights.y = 1;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadScale.y = 3f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.x = 1005;//987;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.y = 1;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayHorizon = 0.28f;
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayCoverage = -0.22f;
                                    //									script.SkyManager.VolShaderCloudsH.CloudDensity = 0.0001f;
                                    //
                                    //									Vector3 Eulers11 = script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles;
                                    //									script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (6,Eulers11.y,Eulers11.z);

                                    script.SkyManager.ApplyType0Default(script.SkyManager);

                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Multi Quad - Realistic Bright)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUD_SETB_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.37f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = -0.05f;//-0.1f;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = -0.02f;//0.03f;//0.06f;	
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = -9f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadScale.y = 3f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.x = 1005;//987;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.y = 1;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayHorizon = 0.01f;
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayCoverage = -0.22f;
                                    //									script.SkyManager.VolShaderCloudsH.CloudDensity = 0.00012f;
                                    //									//script.SkyManager.VolShaderCloudsH.rot
                                    //
                                    //									Vector3 Eulers11 = script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles;
                                    //									script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (0,Eulers11.y,Eulers11.z);

                                    script.SkyManager.ApplyType0Default1(script.SkyManager);

                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Multi Quad - Realistic High altitude)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUD_SETC_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.7f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = -0.04f;//-0.1f;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = -0.02f;//0.03f;//0.06f;	
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = -3f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadScale.y = 2f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.x = 992;//987;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.y = 1;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayHorizon = 0.05f;
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayCoverage = -0.19f;
                                    //									script.SkyManager.VolShaderCloudsH.CloudDensity = 0.0002f;
                                    //									//script.SkyManager.VolShaderCloudsH.rot
                                    //
                                    //									Vector3 Eulers11 = script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles;
                                    //									script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (6,Eulers11.y,Eulers11.z);

                                    script.SkyManager.ApplyType0Default2(script.SkyManager);

                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Multi Quad - Realistic Bumpy)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUD_SETD_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.3f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = -0.22f;//-0.1f;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = -0.15f;//0.03f;//0.06f;	
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = -9f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadScale.y = 3f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.x = 1020;//987;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.y = 1;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayHorizon = 0.01f;
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayCoverage = -0.2f;
                                    //									script.SkyManager.VolShaderCloudsH.CloudDensity = 0.00015f;
                                    //									//script.SkyManager.VolShaderCloudsH.rot
                                    //
                                    //									Vector3 Eulers11 = script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles;
                                    //									script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (0,Eulers11.y,Eulers11.z);

                                    script.SkyManager.ApplyType0Default3(script.SkyManager);

                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Multi Quad - Dramatic Sunset)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUD_SETE_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.9f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = -0.4f;//-0.1f;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = -0.05f;//0.03f;//0.06f;	
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = -9.5f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadScale.y = 2.8f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.x = 1030;//987;
                                    //									script.SkyManager.VolShaderCloudsH.MultiQuadHeights.y = 1;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayHorizon = 0.01f;
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayCoverage = -0.22f;
                                    //									script.SkyManager.VolShaderCloudsH.CloudDensity = 0.00018f;
                                    //									//script.SkyManager.VolShaderCloudsH.rot
                                    //
                                    //									Vector3 Eulers11 = script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles;
                                    //									script.SkyManager.VolShaderCloudsH.SideClouds.eulerAngles = new Vector3 (0,Eulers11.y,Eulers11.z);

                                    script.SkyManager.ApplyType0Default4(script.SkyManager);

                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            //							if (script.SkyManager.VolShaderCloudsH.MultiQuadCClouds && 1==0) {
                            //								EditorGUILayout.BeginHorizontal ();
                            //								EditorGUILayout.HelpBox ("Setup Shader Volume clouds (Multi Quad C - Realistic with Scatter)", MessageType.None);
                            //								EditorGUILayout.EndHorizontal ();
                            //								EditorGUILayout.BeginHorizontal ();
                            //								if (GUILayout.Button (VSCLOUDC_SETA_ICON, GUILayout.MaxWidth (380.0f), GUILayout.MaxHeight (130.0f))) {
                            //
                            //
                            //								}
                            //								EditorGUILayout.EndHorizontal ();
                            //								EditorGUILayout.BeginHorizontal ();
                            //								EditorGUILayout.HelpBox ("Setup Shader Volume clouds (Multi Quad C - Realistic with Scatter)", MessageType.None);
                            //								EditorGUILayout.EndHorizontal ();
                            //								EditorGUILayout.BeginHorizontal ();
                            //								if (GUILayout.Button (VSCLOUDC_SETB_ICON, GUILayout.MaxWidth (380.0f), GUILayout.MaxHeight (130.0f))) {
                            //
                            //
                            //								}
                            //								EditorGUILayout.EndHorizontal ();
                            //								EditorGUILayout.BeginHorizontal ();
                            //								EditorGUILayout.HelpBox ("Setup Shader Volume clouds (Multi Quad C - Realistic with Scatter)", MessageType.None);
                            //								EditorGUILayout.EndHorizontal ();
                            //								EditorGUILayout.BeginHorizontal ();
                            //								if (GUILayout.Button (VSCLOUDC_SETC_ICON, GUILayout.MaxWidth (380.0f), GUILayout.MaxHeight (130.0f))) {
                            //
                            //
                            //								}
                            //								EditorGUILayout.EndHorizontal ();
                            //							}
                            if (script.SkyManager.VolShaderCloudsH.DomeClouds)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Dome thin with scatter)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUDD_SETA_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.6f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = 0;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = 0;//0.06f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = 0f;
                                    //									script.SkyManager.VolShaderCloudsH.DomeScale.y = 1.4f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.DomeHeights.x = 1000;//987;
                                    //									script.SkyManager.VolShaderCloudsH.DomeHeights.y = 1;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.UpdateScatterShader = true;
                                    //									script.SkyManager.VolShaderCloudsH.fog_depth = 0.39f;
                                    //									script.SkyManager.VolShaderCloudsH.reileigh = 7.48f;
                                    //									script.SkyManager.VolShaderCloudsH.mieCoefficient = 0.01f;
                                    //									script.SkyManager.VolShaderCloudsH.mieDirectionalG = 0.41f;
                                    //									script.SkyManager.VolShaderCloudsH.ExposureBias = 0.04f;
                                    //									script.SkyManager.VolShaderCloudsH.K = new Vector3 (1, 0.5f, 0.5f);//new Vector3 (111, 1, 1);

                                    script.SkyManager.ApplyType1Default(script.SkyManager);
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Dome thin with focused scatter)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUDD_SETB_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.6f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = 0;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = 0;//0.06f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = 0f;
                                    //									script.SkyManager.VolShaderCloudsH.DomeScale.y = 1.4f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.DomeHeights.x = 1000;//987;
                                    //									script.SkyManager.VolShaderCloudsH.DomeHeights.y = 1;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.UpdateScatterShader = false;
                                    //									script.SkyManager.VolShaderCloudsH.fog_depth = 0.39f;
                                    //									script.SkyManager.VolShaderCloudsH.reileigh = 7.48f;
                                    //									script.SkyManager.VolShaderCloudsH.mieCoefficient = 0.01f;
                                    //									script.SkyManager.VolShaderCloudsH.mieDirectionalG = 0.6f;//0.41f;
                                    //									script.SkyManager.VolShaderCloudsH.ExposureBias = 0.02f;//0.04f;
                                    //									script.SkyManager.VolShaderCloudsH.K = new Vector3 (111, 1f, 1f);//new Vector3 (111, 1, 1);

                                    script.SkyManager.ApplyType1Default1(script.SkyManager);

                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.HelpBox("Setup Shader Volume clouds (Dome thick)", MessageType.None);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button(VSCLOUDD_SETC_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                                {

                                    //									script.SkyManager.VolCloudTransp = 0.7f;//0.5f;//0.858f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensitySunOffset = -0.05f;//-0.1f;//-0.16f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityDiffOffset = 0.04f;//0.03f;//0.06f;
                                    //									script.SkyManager.VolShaderCloudsH.IntensityFogOffset = 0f;
                                    //									script.SkyManager.VolShaderCloudsH.DomeScale.y = 2.8f;//1.2f;//1.5f;
                                    //									script.SkyManager.VolShaderCloudsH.DomeHeights.x = 700;//987;
                                    //									script.SkyManager.VolShaderCloudsH.DomeHeights.y = 1;
                                    //									script.SkyManager.VolShaderCloudsH.ClearDayHorizon = 0.01f;
                                    //
                                    //									script.SkyManager.VolShaderCloudsH.UpdateScatterShader = false;
                                    //									script.SkyManager.VolShaderCloudsH.fog_depth = 0.39f;
                                    //									script.SkyManager.VolShaderCloudsH.reileigh = 7.48f;
                                    //									script.SkyManager.VolShaderCloudsH.mieCoefficient = 0.01f;
                                    //									script.SkyManager.VolShaderCloudsH.mieDirectionalG = 0.6f;//0.41f;
                                    //									script.SkyManager.VolShaderCloudsH.ExposureBias = 0.02f;//0.04f;
                                    //									script.SkyManager.VolShaderCloudsH.K = new Vector3 (111, 1f, 1f);//new Vector3 (111, 1, 1);

                                    script.SkyManager.ApplyType1Default2(script.SkyManager);

                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.Space();
                    }

                    //EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));//v3.3e
                    EditorGUILayout.BeginVertical();//v3.3e

                    //v4.8.2
                    GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));

                        EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
                        GUILayout.Space(10);
                        script.cloud_folder4 = EditorGUILayout.Foldout(script.cloud_folder4, "Particle Volume Clouds (Use for exact cloud shaping, uses particles)");
                        EditorGUILayout.EndHorizontal();

                        if (script.cloud_folder4)
                        {


                            EditorGUILayout.HelpBox("Particle based Volumetric Clouds", MessageType.None);


                            //v3.4.5a - convert clouds to Shuriken
                            EditorGUILayout.HelpBox("Gameobject with 'VolumeCloud_SM' script to convert to Shuriken (from Legacy).", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.TMPCloudOBJ = EditorGUILayout.ObjectField(script.TMPCloudOBJ, typeof(GameObject), true, GUILayout.MaxWidth(sliderWidth)) as GameObject;
                            if (GUILayout.Button(new GUIContent("Convert to Shuriken"), GUILayout.Width(160)))
                            {
                                if (script.TMPCloudOBJ != null)
                                {
                                    //script.SkyManager.VCloudCenter = TMPCloudCenter.transform;
                                    if (script.TMPCloudOBJ.GetComponent<VolumeClouds_SM>() != null)
                                    {
                                        script.TMPCloudOBJ.GetComponent<VolumeClouds_SM>().convertToShuriken();
                                        Debug.Log("Volume particle cloud converted to Shuriken");
                                    }
                                    else
                                    {
                                        Debug.Log("Volume particle cloud controller not found");
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();


                            //v3.4.1
                            EditorGUILayout.HelpBox("Cloud bed start point. If not defined Map Center is used.", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            GameObject TMPCloudCenter = EditorGUILayout.ObjectField(script.SkyManager.VCloudCenter, typeof(GameObject), true, GUILayout.MaxWidth(sliderWidth)) as GameObject;
                            if (TMPCloudCenter != null)
                            {
                                script.SkyManager.VCloudCenter = TMPCloudCenter.transform;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.HelpBox("Cloud bed opposing wind offset from center.", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.WindBasedOffsetFactor = EditorGUILayout.Slider(script.SkyManager.WindBasedOffsetFactor, -20, 20, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();
                            //VCloudXZOffset
                            //WindBasedOffsetFactor
                            //VCloudCenter


                            EditorGUILayout.HelpBox("Parameters activated with the next volumetric cloud bed creation", MessageType.None);
                            GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));

                            //v3.4.1
                            //if (script.SkyManager.VCloudCustomSize) {
                            EditorGUILayout.HelpBox("Custom Cloud bed width", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VCloudCustomSize = EditorGUILayout.Toggle(script.SkyManager.VCloudCustomSize, GUILayout.MaxWidth(sliderWidth));//v3.4.1 extended from 2000 to 12K
                            EditorGUILayout.EndHorizontal();
                            //}
                            EditorGUILayout.HelpBox("Cloud bed width", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolCloudsHorScale = EditorGUILayout.Slider(script.SkyManager.VolCloudsHorScale, -12000, 12000, GUILayout.MaxWidth(sliderWidth));//v3.4.1 extended from 2000 to 12K
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.HelpBox("Cloud bed height", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VolCloudHeight = EditorGUILayout.Slider(script.SkyManager.VolCloudHeight, 10, 2000, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.HelpBox("Cloud particles size", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VCloudSizeFac = EditorGUILayout.Slider(script.SkyManager.VCloudSizeFac, 0.5f, 10, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.HelpBox("Cloud size", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VCloudCSizeFac = EditorGUILayout.Slider(script.SkyManager.VCloudCSizeFac, 1f, 50, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.HelpBox("Cloud centers multiplier", MessageType.None);
                            EditorGUILayout.BeginHorizontal();
                            script.SkyManager.VCloudCoverFac = EditorGUILayout.Slider(script.SkyManager.VCloudCoverFac, 0.1f, 10, GUILayout.MaxWidth(sliderWidth));
                            EditorGUILayout.EndHorizontal();


                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();


                            //	GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));
                            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));//v3.3e

                            EditorGUILayout.BeginHorizontal();
                            script.OverridePeformance = EditorGUILayout.Toggle("Override Performance", script.OverridePeformance, GUILayout.MaxWidth(380.0f));
                            EditorGUILayout.EndHorizontal();
                            if (script.OverridePeformance)
                            {

                                EditorGUILayout.HelpBox("Override default performance settings of current volume cloud bed. Use Smooth cloud motion for decoupling the cloud motion from the performance" +
                                " settings.", MessageType.None);

                                EditorGUILayout.BeginHorizontal();
                                script.DecoupleWind = EditorGUILayout.Toggle("Smooth cloud motion", script.DecoupleWind, GUILayout.MaxWidth(380.0f));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Update interval", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.UpdateInteraval = EditorGUILayout.Slider(script.UpdateInteraval, 0.01f, 0.5f, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(415));

                                EditorGUILayout.HelpBox("Spread calcs to frames", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SpreadToFrames = EditorGUILayout.IntSlider(script.SpreadToFrames, 0, 15, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();
                            }

                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();

                            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));
                            //		GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));

                            EditorGUILayout.BeginHorizontal();
                            script.RenewClouds = EditorGUILayout.Toggle("Renew clouds", script.RenewClouds, GUILayout.MaxWidth(380.0f));
                            EditorGUILayout.EndHorizontal();
                            if (script.RenewClouds)
                            {

                                EditorGUILayout.HelpBox("Renew the volumetric cloud bed with an instance of the current clouds with reset in " +
                                "position.", MessageType.None);

                                GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(415));

                                EditorGUILayout.BeginHorizontal();
                                script.OverrideRenewSettings = EditorGUILayout.Toggle("Override Fade-Boundary", script.OverrideRenewSettings, GUILayout.MaxWidth(380.0f));
                                EditorGUILayout.EndHorizontal();
                                if (script.OverrideRenewSettings)
                                {

                                    EditorGUILayout.HelpBox("Boundary where clouds are renewed", MessageType.None);
                                    EditorGUILayout.BeginHorizontal();
                                    script.Boundary = EditorGUILayout.Slider(script.Boundary, 500f, 150000f, GUILayout.MaxWidth(sliderWidth));
                                    EditorGUILayout.EndHorizontal();



                                    EditorGUILayout.HelpBox("Fade in speed", MessageType.None);
                                    EditorGUILayout.BeginHorizontal();
                                    script.FadeInSpeed = EditorGUILayout.Slider(script.FadeInSpeed, 0.1f, 0.9f, GUILayout.MaxWidth(sliderWidth));
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.HelpBox("Fade out speed", MessageType.None);
                                    EditorGUILayout.BeginHorizontal();
                                    script.FadeOutSpeed = EditorGUILayout.Slider(script.FadeOutSpeed, 0.1f, 0.9f, GUILayout.MaxWidth(sliderWidth));
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.HelpBox("Fade out time", MessageType.None);
                                    EditorGUILayout.BeginHorizontal();
                                    script.MaxFadeTime = EditorGUILayout.Slider(script.MaxFadeTime, 0.5f, 2.5f, GUILayout.MaxWidth(sliderWidth));
                                    EditorGUILayout.EndHorizontal();
                                }

                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();

                            GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(410));

                            //MODIFIERS - real time
                            if (script.SkyManager.currentWeather != null && script.SkyManager.currentWeather.VolumeScript != null)
                            {

                                ////
                                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(180.0f));

                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.DifferentialMotion = EditorGUILayout.Toggle("Differential motion", script.SkyManager.currentWeather.VolumeScript.DifferentialMotion, GUILayout.MaxWidth(380.0f));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Differential speed", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.MaxDiffSpeed = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.MaxDiffSpeed, -20, 20, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Differential factor", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.MaxDiffOffset = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.MaxDiffOffset, -2000, 2000, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Cloud speed multiplier", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.speed = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.speed, 0, 50, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                ////

                                GUILayout.Box("", GUILayout.Height(3), GUILayout.Width(415));

                                EditorGUILayout.HelpBox("Glow modifier", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.GlowShaderModifier = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.GlowShaderModifier, -20, 20, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Min light modifier", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.minLightShaderModifier = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.minLightShaderModifier, -20, 20, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Light intensity modifier", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.LightShaderModifier = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.LightShaderModifier, -20, 20, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Sun intensity modifier", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.IntensityShaderModifier = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.IntensityShaderModifier, -2, 2, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Modifiers speed", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.ModifierApplMinSpeed = EditorGUILayout.Slider(script.SkyManager.currentWeather.VolumeScript.ModifierApplMinSpeed, 0, 20, GUILayout.MaxWidth(sliderWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.HelpBox("Cloud dusk color", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.Dusk_sun_col = EditorGUILayout.ColorField(script.SkyManager.currentWeather.VolumeScript.Dusk_sun_col, GUILayout.MaxWidth(195.0f));
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.HelpBox("Cloud base dusk color", MessageType.None);
                                EditorGUILayout.BeginHorizontal();
                                script.SkyManager.currentWeather.VolumeScript.Dusk_base_col = EditorGUILayout.ColorField(script.SkyManager.currentWeather.VolumeScript.Dusk_base_col, GUILayout.MaxWidth(195.0f));
                                EditorGUILayout.EndHorizontal();



                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space();
                            }



                            EditorGUILayout.BeginHorizontal();
                            //EditorGUILayout.LabelField("Setup Volumetric clouds (Realistic)",EditorStyles.boldLabel);	
                            EditorGUILayout.HelpBox("Setup Volumetric clouds (Realistic)", MessageType.None);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            //if(GUILayout.Button(new GUIContent("Setup Volumetric clouds (Realistic)"),GUILayout.Width(220))){		//SET A	
                            if (GUILayout.Button(VCLOUD_SETA_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                            {

                                script.SkyManager.HeavyStormVolumeClouds = HeavyStormVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.DayClearVolumeClouds = DayClearVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.SnowVolumeClouds = SnowVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.RainStormVolumeClouds = RainStormVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.SnowStormVolumeClouds = SnowStormVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.RainVolumeClouds = RainVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.PinkVolumeClouds = PinkVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.DustyStormVolumeClouds = DustyStormVOLUME_CLOUD2 as GameObject;
                                script.SkyManager.LightningVolumeClouds = LightningVOLUME_CLOUD2 as GameObject;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("Setup Volumetric clouds (Fantasy)", MessageType.None);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            //if(GUILayout.Button(new GUIContent("Setup Volumetric clouds (Fantasy)"),GUILayout.Width(220))){			//SET B
                            if (GUILayout.Button(VCLOUD_SETB_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                            {
                                script.SkyManager.HeavyStormVolumeClouds = HeavyStormVOLUME_CLOUD as GameObject;
                                script.SkyManager.DayClearVolumeClouds = DayClearVOLUME_CLOUD as GameObject;
                                script.SkyManager.SnowVolumeClouds = SnowVOLUME_CLOUD as GameObject;
                                script.SkyManager.RainStormVolumeClouds = RainStormVOLUME_CLOUD as GameObject;
                                script.SkyManager.SnowStormVolumeClouds = SnowStormVOLUME_CLOUD as GameObject;
                                script.SkyManager.RainVolumeClouds = RainVOLUME_CLOUD as GameObject;
                                script.SkyManager.PinkVolumeClouds = PinkVOLUME_CLOUD as GameObject;
                                script.SkyManager.DustyStormVolumeClouds = DustyStormVOLUME_CLOUD as GameObject;
                                script.SkyManager.LightningVolumeClouds = LightningVOLUME_CLOUD as GameObject;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("Setup Volumetric clouds (Toon)", MessageType.None);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            //if(GUILayout.Button(new GUIContent("Setup Volumetric clouds (Fantasy)"),GUILayout.Width(220))){			//SET B
                            if (GUILayout.Button(VCLOUD_SETC_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                            {
                                script.SkyManager.HeavyStormVolumeClouds = HeavyStormVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.DayClearVolumeClouds = DayClearVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.SnowVolumeClouds = SnowVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.RainStormVolumeClouds = RainStormVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.SnowStormVolumeClouds = SnowStormVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.RainVolumeClouds = RainVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.PinkVolumeClouds = PinkVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.DustyStormVolumeClouds = DustyStormVOLUME_CLOUD3 as GameObject;
                                script.SkyManager.LightningVolumeClouds = LightningVOLUME_CLOUD3 as GameObject;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("Setup Volumetric clouds (No scatter)", MessageType.None);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            //if(GUILayout.Button(new GUIContent("Setup Volumetric clouds (Fantasy)"),GUILayout.Width(220))){			//SET B
                            if (GUILayout.Button(VCLOUD_SETD_ICON, GUILayout.MaxWidth(380.0f), GUILayout.MaxHeight(130.0f)))
                            {
                                script.SkyManager.HeavyStormVolumeClouds = HeavyStormVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.DayClearVolumeClouds = DayClearVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.SnowVolumeClouds = SnowVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.RainStormVolumeClouds = RainStormVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.SnowStormVolumeClouds = SnowStormVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.RainVolumeClouds = RainVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.PinkVolumeClouds = PinkVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.DustyStormVolumeClouds = DustyStormVOLUME_CLOUD4 as GameObject;
                                script.SkyManager.LightningVolumeClouds = LightningVOLUME_CLOUD4 as GameObject;
                            }
                        }//end particle clouds section

                        //UnityTerrain =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
                        EditorGUILayout.EndHorizontal();


                        //EditorGUILayout.BeginHorizontal();
                        //if(GUILayout.Button(new GUIContent("Scale Volumetric clouds"),GUILayout.Width(220))){			
                        //					if(mesh_terrain != null){
                        //						
                        //					}else{
                        //						Debug.Log ("Add a mesh terrain first");
                        //					}
                        //					
                        //}
                        //				//mesh_terrain =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
                        //				
                        //				
                        //EditorGUILayout.EndHorizontal();	

                        EditorGUIUtility.wideMode = false;

                        EditorGUILayout.BeginHorizontal();

                        //ParticleScaleFactor = EditorGUILayout.FloatField(ParticleScaleFactor,GUILayout.MaxWidth(95.0f));
                        //ParticlePlaybackScaleFactor = EditorGUILayout.FloatField(ParticlePlaybackScaleFactor,GUILayout.MaxWidth(95.0f));
                        EditorGUILayout.EndHorizontal();




                    //}v4.8.2
                    EditorGUILayout.EndVertical();
                    ///////////////////////////////////////////////////////////////////////////

                    


                }//END TAB1






				/////////////////////////////////////////////////////////////////////////////// TERRAIN ///////////////////////////////////////
		//		EditorGUILayout.BeginVertical (GUILayout.MaxWidth (180.0f));			
				//X_offset_left = 200;
				//Y_offset_top = 100;			
				//GUILayout.Box("",GUILayout.Height(3),GUILayout.Width(410));

				//TAB2
				if ((script.UseTabs && script.currentTab == 2) | !script.UseTabs) {
					

					GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	

					GUI.backgroundColor = Color.blue * 0.2f;
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f));//v3.3e
					GUI.backgroundColor = Color.white;

					GUILayout.Label (MainIcon3, GUILayout.MaxWidth (410.0f));

					EditorGUILayout.LabelField ("Terrain", EditorStyles.boldLabel);			
					EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
					GUILayout.Space (10);
					script.terrain_folder1 = EditorGUILayout.Foldout (script.terrain_folder1, "Terrain");
					EditorGUILayout.EndHorizontal ();
			
					if (script.terrain_folder1) {

						//v3.3d
						EditorGUILayout.HelpBox ("Insert the terrains in the relevant list, for mesh or Unity terrains by dragging from the Hierachy panel (expand the below list with the arrow & put a number first to grow the list in Size - e.g. '1' for a single terrain) and press the relevant setup button. The terrain & fog handler script will be attached " +
						"to the first terrain in the list, and all terrains will receieve the snow enabled material (the material can then be changed if needed to any other)", MessageType.None);

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
						EditorGUILayout.BeginHorizontal (GUILayout.Width (400));
						GUILayout.Space (15);
						EditorGUILayout.PropertyField (UnityTerrains, true);
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal (GUILayout.Width (400));
						GUILayout.Space (15);
						EditorGUILayout.PropertyField (MeshTerrains, true);
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.EndVertical ();
						//EditorGUILayout.Space ();

						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Add sample Unity terrain"), GUILayout.Width (160))) {		
				
							GameObject SampleTerrainOBJ = (GameObject)Instantiate (SampleTerrain, script.SkyManager.MapCenter.position + new Vector3 (-100, 0, 0), Quaternion.identity);
							UnityTerrain = SampleTerrainOBJ;//assign to terrain
							script.SkyManager.Unity_terrain = SampleTerrainOBJ.transform;

							script.UnityTerrains.Add (SampleTerrainOBJ.GetComponent<Terrain> ());//add to Unitty terrains list
						}
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Setup Unity terrain(s)"), GUILayout.Width (160))) {

							if (UnityTerrain != null | script.SkyManager.Unity_terrain != null | script.UnityTerrains.Count > 0) {//if terrain defined in editor or in Sky manager, set it up
								if (script.TerrainManager == null) {

									//v3.0.1
//							if(script.SkyManager.Unity_terrain == null){
//								script.SkyManager.Unity_terrain = (UnityTerrain as GameObject).transform;
//							}

									//Add material to terrain
									//script.SkyManager.Unity_terrain.gameObject.GetComponent<Terrain>().materialType = Terrain.MaterialType.Custom;
									//script.SkyManager.Unity_terrain.gameObject.GetComponent<Terrain>().materialTemplate = UnityTerrainSnowMat;

									//v3.4
									if (script.AddSmTerrainMat) {
										for (int i = 0; i < script.UnityTerrains.Count; i++) {
											//script.UnityTerrains [i].materialType = Terrain.MaterialType.Custom;
											script.UnityTerrains [i].materialTemplate = UnityTerrainSnowMat;
										}
									}

									script.SkyManager.Unity_terrain = script.UnityTerrains [0].transform;

                                    //script.TerrainManager.TreePefabs.Add(UnityTreePrefab as GameObject);
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient6.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient7.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient8.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient9.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient10.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient11.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient12.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient13.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient14.GetComponent<GlobalFogSkyMaster>());
                                    //script.TerrainManager.GradientHolders.Add(FogPresetGradient15.GetComponent<GlobalFogSkyMaster>());//v3.3

                                    //Add terrain handler and configure (trees are configured in foliage section, volume fog in camera FX section)
                                    //	script.SkyManager.Unity_terrain.gameObject.AddComponent<SeasonalTerrainSKYMASTER> (); //v4.8
                                    //	script.TerrainManager = script.SkyManager.Unity_terrain.gameObject.GetComponent<SeasonalTerrainSKYMASTER> (); //v4.8


                                    //v4.8
                                    //assing materials to script
                                    //script.UnityTerrainSnowMat = UnityTerrainSnowMat;
                                    ////script.MeshTerrainSnowMat = MeshTerrainSnowMat;
                                    //script.TerrainManager.TerrainMat = UnityTerrainSnowMat; 

									//script.SkyManager.SnowMat = MeshTerrainSnowMat;
									//script.SkyManager.SnowMatTerrain = UnityTerrainSnowMat;						
									

									//script.TerrainManager.Mesh_moon = true;
									//script.TerrainManager.Glow_moon = true;
									//script.TerrainManager.Enable_trasition = true;
									//script.TerrainManager.Fog_Sky_Update = true;
									//script.TerrainManager.Foggy_Terrain = true;
									//script.TerrainManager.Use_both_fogs = true;
									////script.TerrainManager.ImageEffectFog = true;
									////script.TerrainManager.ImageEffectShafts = true;
									//script.TerrainManager.SkyManager = script.SkyManager;

									//v3.3b
									////if (script.TerrainManager != null && script.WaterManager != null && script.WaterManager.TerrainManager == null) {
									//	script.WaterManager.TerrainManager = script.TerrainManager;
									//}
								}
							} else {
								Debug.Log ("Add Unity terrain first");
							}

						}

                        

                        //UnityTerrain =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
                        EditorGUILayout.EndHorizontal ();


						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Setup mesh terrain"), GUILayout.Width (160))) {			
							if (mesh_terrain != null | script.MeshTerrains.Count > 0) {
								if (script.TerrainManager == null) {

									//Add material to mesh terrain
									//(mesh_terrain as GameObject).GetComponent<Renderer>().material = MeshTerrainSnowMat; //v3.0.1

									//v3.4
									if (script.AddSmTerrainMat) {
										for (int i = 0; i < script.MeshTerrains.Count; i++) {
											script.MeshTerrains [i].GetComponent<Renderer> ().material = MeshTerrainSnowMat;
										}
									}

									script.SkyManager.Mesh_terrain = script.MeshTerrains [0].transform;

									//(mesh_terrain as GameObject).AddComponent<SeasonalTerrainSKYMASTER>();
									script.SkyManager.Mesh_terrain.gameObject.AddComponent<SeasonalTerrainSKYMASTER> ();
									script.TerrainManager = script.SkyManager.Mesh_terrain.gameObject.GetComponent<SeasonalTerrainSKYMASTER> ();
									script.TerrainManager.TerrainMat = MeshTerrainSnowMat; 

									script.SkyManager.SnowMat = MeshTerrainSnowMat;
									script.SkyManager.SnowMatTerrain = UnityTerrainSnowMat;

									script.TerrainManager.TreePefabs.Add (UnityTreePrefab as GameObject);
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient1_5.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient6.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient7.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient8.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient9.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient10.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient11.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient12.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient13.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient14.GetComponent<GlobalFogSkyMaster> ());
									//script.TerrainManager.GradientHolders.Add (FogPresetGradient15.GetComponent<GlobalFogSkyMaster> ());//v3.3
							
									script.TerrainManager.Mesh_moon = true;
									script.TerrainManager.Glow_moon = true;
									script.TerrainManager.Enable_trasition = true;
									script.TerrainManager.Fog_Sky_Update = true;
									script.TerrainManager.Foggy_Terrain = true;
									script.TerrainManager.Use_both_fogs = true;
									//script.TerrainManager.ImageEffectFog = true;
									//script.TerrainManager.ImageEffectShafts = true;
									script.TerrainManager.SkyManager = script.SkyManager;
									script.TerrainManager.Mesh_Terrain = true;
									//script.SkyManager.Mesh_terrain = (mesh_terrain as GameObject).transform;//v3.0.1

									//
									//v3.3b
									if (script.TerrainManager != null && script.WaterManager != null) {
										script.WaterManager.TerrainManager = script.TerrainManager;
									}
								}
							} else {
								Debug.Log ("Add a mesh terrain first and make sure a Unity terrain has not been setup already");
							}
					
						}
						//mesh_terrain =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
				
			
						EditorGUILayout.EndHorizontal ();				
						EditorGUIUtility.wideMode = false;
				
						EditorGUILayout.BeginHorizontal ();
				
						//v3.4
						if (script != null) {
							script.AddSmTerrainMat = EditorGUILayout.Toggle ("Add snow material",script.AddSmTerrainMat, GUILayout.MaxWidth (380.0f));
						}

						//ParticleScaleFactor = EditorGUILayout.FloatField(ParticleScaleFactor,GUILayout.MaxWidth(95.0f));
						//ParticlePlaybackScaleFactor = EditorGUILayout.FloatField(ParticlePlaybackScaleFactor,GUILayout.MaxWidth(95.0f));
						EditorGUILayout.EndHorizontal ();
				
						//v3.4
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
						if(script.SkyManager.SnowMatTerrain != null){

							//v3.4.1
							EditorGUILayout.BeginHorizontal();
							if (GUILayout.Button (new GUIContent ("Terrain Preset A (wet ground)"), GUILayout.Width (205))) {
								if (script.SkyManager.SnowMatTerrain.HasProperty ("_Shininess")) {
									script.SkyManager.SnowMatTerrain.SetFloat ("_Shininess", 0.85f);
									script.SkyManager.SnowMatTerrain.SetColor ("_SpecColor", Color.white);
								}
							}
							if (GUILayout.Button (new GUIContent ("Terrain Preset B (dry ground)"), GUILayout.Width (205))) {
								if (script.SkyManager.SnowMatTerrain.HasProperty ("_Shininess")) {
									script.SkyManager.SnowMatTerrain.SetFloat ("_Shininess", 0.0f);
									script.SkyManager.SnowMatTerrain.SetColor ("_SpecColor", Color.black);
								}
							}
							EditorGUILayout.EndHorizontal ();

							if (script.SkyManager.SnowMatTerrain.HasProperty("_Shininess")) {

								if (script.TerrainManager != null) {
									EditorGUILayout.HelpBox ("Unity terrain tint color", MessageType.None);
									EditorGUILayout.BeginHorizontal ();
									script.TerrainManager.Terrain_tint = EditorGUILayout.ColorField (script.TerrainManager.Terrain_tint, GUILayout.MaxWidth (195.0f));
									EditorGUILayout.EndHorizontal ();
								}

								if (script.SkyManager.SnowMatTerrain != null) {
									if (script.SkyManager.SnowMatTerrain.HasProperty ("_SpecColor")) {
										EditorGUILayout.HelpBox ("Terrain specular color", MessageType.None);
										Color ShininessCol = EditorGUILayout.ColorField (script.SkyManager.SnowMatTerrain.GetColor ("_SpecColor"), GUILayout.MaxWidth (195.0f));
										script.SkyManager.SnowMatTerrain.SetColor ("_SpecColor", ShininessCol);
									}
									if (script.SkyManager.SnowMatTerrain.HasProperty ("_Shininess")) {
										EditorGUILayout.HelpBox ("Unity terrain shininess", MessageType.None);
										float Shininess = EditorGUILayout.Slider (script.SkyManager.SnowMatTerrain.GetFloat ("_Shininess"), 0, 1, GUILayout.MaxWidth (sliderWidth));
										script.SkyManager.SnowMatTerrain.SetFloat ("_Shininess", Shininess);
									}
									if (script.SkyManager.SnowMatTerrain.HasProperty ("_LightIntensity")) {
										EditorGUILayout.HelpBox ("Unity terrain Snow Light Factor", MessageType.None);
										float LightIntensity = EditorGUILayout.Slider (script.SkyManager.SnowMatTerrain.GetFloat ("_LightIntensity"), 0, 1, GUILayout.MaxWidth (sliderWidth));
										script.SkyManager.SnowMatTerrain.SetFloat ("_LightIntensity", LightIntensity);
									}
									if (script.SkyManager.SnowMatTerrain.HasProperty ("_SnowBlend")) {
										EditorGUILayout.HelpBox ("Unity terrain Snow Blend Factor", MessageType.None);
										float SnowBlend = EditorGUILayout.Slider (script.SkyManager.SnowMatTerrain.GetFloat ("_SnowBlend"), 0, 1.5f, GUILayout.MaxWidth (sliderWidth));
										script.SkyManager.SnowMatTerrain.SetFloat ("_SnowBlend", SnowBlend);
									}
								}
							}
						}
						EditorGUILayout.EndVertical ();

				
					}
					EditorGUILayout.EndVertical ();
					///////////////////////////////////////////////////////////////////////////


				}//END TAB2









				/////////////////////////////////////////////////////////////////////////////// CAMERA FX - VOLUMETRIC FOG - SUN SHAFTS /////////////////
		//		EditorGUILayout.BeginVertical (GUILayout.MaxWidth (180.0f));			
				//X_offset_left = 200;
				//Y_offset_top = 100;			
				//GUILayout.Box("",GUILayout.Height(3),GUILayout.Width(410));
			
				//TAB3
				if ((script.UseTabs && script.currentTab == 3) | !script.UseTabs) {


					GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));


					GUI.backgroundColor = Color.blue * 0.2f;
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f)); //v3.3e
					GUI.backgroundColor = Color.white;

			
					GUILayout.Label (MainIcon4, GUILayout.MaxWidth (410.0f));
			
					EditorGUILayout.LabelField ("Camera FX", EditorStyles.boldLabel);			
					EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
					GUILayout.Space (10);
					script.camera_folder1 = EditorGUILayout.Foldout (script.camera_folder1, "Volume fog, Sun shafts");
					EditorGUILayout.EndHorizontal ();
			
					if (script.camera_folder1) {	

						//v3.4
						//SHADER CLOUDS
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f));//v3.3e
						EditorGUILayout.HelpBox ("Volumetric lighting, fog and sun shafts", MessageType.None);
						//if (script.SkyManager.VolLightingH == null) {
						//	EditorGUILayout.BeginHorizontal ();
						//	if (GUILayout.Button (new GUIContent ("Add Volumetric lighting"), GUILayout.Width (260))) {
						//		Vector3 MapcenterPos = script.SkyManager.MapCenter.position;
						//		GameObject VolLight = (GameObject)Instantiate (VolLightingPREFAB, MapcenterPos + new Vector3 (0, 3, 0), Quaternion.identity); 
						//		//script.SkyManager.VolShaderClouds = VolClouds.transform;
						//		script.SkyManager.VolLightingH = VolLight.GetComponent<AtmosphericScatteringSkyMaster> ();
						//		script.SkyManager.VolLightingDefH = Camera.main.gameObject.AddComponent<AtmosphericScatteringDeferredSkyMaster> ();
						//		script.SkyManager.VolLightingH.SkyManager = script.SkyManager;
						//		//script.SkyManager.VolShaderCloudsH.SkyManager = script.SkyManager;
						//	}
						//	EditorGUILayout.EndHorizontal ();
						//} else {
						//	if(script.SkyManager.VolLightingH != null){

						//		//VOLUME LIGHTNING PARAMETERS
						//		script.SkyManager.VolLightingH.FogSky = EditorGUILayout.Toggle ("Fog Sky",script.SkyManager.VolLightingH.FogSky, GUILayout.MaxWidth (380.0f));
						//		script.SkyManager.VolLightingH.useOcclusion = EditorGUILayout.Toggle ("Shadow fog",script.SkyManager.VolLightingH.useOcclusion, GUILayout.MaxWidth (380.0f));
						//		script.SkyManager.VolLightingH.occlusionFullSky = EditorGUILayout.Toggle ("Shadow Fog in Sky",script.SkyManager.VolLightingH.occlusionFullSky, GUILayout.MaxWidth (380.0f));

						//		EditorGUILayout.HelpBox ("Lower Horizon Fog Amount", MessageType.None);
						//		script.SkyManager.VolLightingH.FogHorizonLower = EditorGUILayout.Slider(script.SkyManager.VolLightingH.FogHorizonLower,-3,10, GUILayout.Width(sliderWidth));
						//		EditorGUILayout.HelpBox ("Fog distance from Camera", MessageType.None);
						//		script.SkyManager.VolLightingH.heightDistance = EditorGUILayout.Slider(script.SkyManager.VolLightingH.heightDistance,0,1000, GUILayout.Width(sliderWidth));

						//		script.SkyManager.VolLightingH.heightRayleighColor = EditorGUILayout.ColorField(script.SkyManager.VolLightingH.heightRayleighColor,GUILayout.Width(180));

						//		EditorGUILayout.HelpBox ("Volumetric Sun shafts intensity", MessageType.None);
						//		script.SkyManager.VolLightingH.heightRayleighIntensity = EditorGUILayout.Slider(script.SkyManager.VolLightingH.heightRayleighIntensity,0,10, GUILayout.Width(sliderWidth));

						//		EditorGUILayout.HelpBox ("Volumetric Lighting-Fog Height", MessageType.None);
						//		script.SkyManager.VolLightingH.heightSeaLevel = EditorGUILayout.Slider(script.SkyManager.VolLightingH.heightSeaLevel,-1000,20000, GUILayout.Width(sliderWidth));

						//		//v3.4.3
						//		EditorGUILayout.HelpBox ("Volume light max distance", MessageType.None);
						//		script.SkyManager.VolLightingH.backLightDepth = EditorGUILayout.Slider(script.SkyManager.VolLightingH.backLightDepth,-10000,10000, GUILayout.Width(sliderWidth));
						//		EditorGUILayout.HelpBox ("Volume light back intensity", MessageType.None);
						//		script.SkyManager.VolLightingH.backLightIntensity = EditorGUILayout.Slider(script.SkyManager.VolLightingH.backLightIntensity,0,5, GUILayout.Width(sliderWidth));

						//	}
						//}
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();



						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
						if (script.TerrainManager != null) {

							EditorGUILayout.BeginHorizontal ();
							//if (GUILayout.Button (new GUIContent ("Add volumetric fog"), GUILayout.Width (150))) {			
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<GlobalFogSkyMaster> () == null) {
							//		Camera.main.gameObject.AddComponent<GlobalFogSkyMaster> ();
							//		Camera.main.gameObject.GetComponent<GlobalFogSkyMaster> ().SkyManager = script.SkyManager;
							//		Camera.main.gameObject.GetComponent<GlobalFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//		//Debug.Log("CAMERA FOUND");
							//		script.TerrainManager.Lerp_gradient = true;
							//		script.TerrainManager.ImageEffectFog = true;
							//		script.TerrainManager.FogHeightByTerrain = true;
							//	} else {
							//		if (Camera.main == null) {
							//			Debug.Log ("Add a main camera first");
							//		}
							//		if (Camera.main.gameObject.GetComponent<GlobalFogSkyMaster> () != null) {
							//			//setup existing
							//			Camera.main.gameObject.GetComponent<GlobalFogSkyMaster> ().SkyManager = script.SkyManager;
							//			Camera.main.gameObject.GetComponent<GlobalFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//			script.TerrainManager.Lerp_gradient = true;
							//			script.TerrainManager.ImageEffectFog = true;
							//			script.TerrainManager.FogHeightByTerrain = true;
							//		}
							//	}

       //                         //v4.8
       //                         script.TerrainManager.setVFogCurvesPresetE();

       //                         //LEFT-RIGHT VR CAMERAS
       //                         if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<GlobalFogSkyMaster> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<GlobalFogSkyMaster> ();
							//			script.TerrainManager.LeftCam.GetComponent<GlobalFogSkyMaster> ().SkyManager = script.SkyManager;
							//			script.TerrainManager.LeftCam.GetComponent<GlobalFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//			script.TerrainManager.Lerp_gradient = true;
							//			script.TerrainManager.ImageEffectFog = true;
							//			script.TerrainManager.FogHeightByTerrain = true;
							//		} else {
							//			if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<GlobalFogSkyMaster> () != null) {
							//				//setup existing
							//				script.TerrainManager.LeftCam.GetComponent<GlobalFogSkyMaster> ().SkyManager = script.SkyManager;
							//				script.TerrainManager.LeftCam.GetComponent<GlobalFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//				script.TerrainManager.Lerp_gradient = true;
							//				script.TerrainManager.ImageEffectFog = true;
							//				script.TerrainManager.FogHeightByTerrain = true;
							//			}
							//		}
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<GlobalFogSkyMaster> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<GlobalFogSkyMaster> ();
							//			script.TerrainManager.RightCam.GetComponent<GlobalFogSkyMaster> ().SkyManager = script.SkyManager;
							//			script.TerrainManager.RightCam.GetComponent<GlobalFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//			script.TerrainManager.Lerp_gradient = true;
							//			script.TerrainManager.ImageEffectFog = true;
							//			script.TerrainManager.FogHeightByTerrain = true;
							//		} else {
							//			if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<GlobalFogSkyMaster> () != null) {
							//				//setup existing
							//				script.TerrainManager.RightCam.GetComponent<GlobalFogSkyMaster> ().SkyManager = script.SkyManager;
							//				script.TerrainManager.RightCam.GetComponent<GlobalFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//				script.TerrainManager.Lerp_gradient = true;
							//				script.TerrainManager.ImageEffectFog = true;
							//				script.TerrainManager.FogHeightByTerrain = true;
							//			}
							//		}	
							//	}
							//}
							//if (GUILayout.Button (new GUIContent ("Add transparent v.fog"), GUILayout.Width (150))) {			
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<GlobalTranspFogSkyMaster> () == null) {
							//		Camera.main.gameObject.AddComponent<GlobalTranspFogSkyMaster> ();
							//		Camera.main.gameObject.GetComponent<GlobalTranspFogSkyMaster> ().SkyManager = script.SkyManager;
							//		Camera.main.gameObject.GetComponent<GlobalTranspFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//		//Debug.Log("CAMERA FOUND");
							//		script.TerrainManager.Lerp_gradient = true;
							//		script.TerrainManager.ImageEffectFog = true;
							//		script.TerrainManager.FogHeightByTerrain = true;
							//	} else {
							//		if (Camera.main == null) {
							//			Debug.Log ("Add a main camera first");
							//		}
							//		if (Camera.main.gameObject.GetComponent<GlobalTranspFogSkyMaster> () != null) {
							//			//setup existing
							//			Camera.main.gameObject.GetComponent<GlobalTranspFogSkyMaster> ().SkyManager = script.SkyManager;
							//			Camera.main.gameObject.GetComponent<GlobalTranspFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//			script.TerrainManager.Lerp_gradient = true;
							//			script.TerrainManager.ImageEffectFog = true;
							//			script.TerrainManager.FogHeightByTerrain = true;
							//		}
							//	}

							//	//LEFT-RIGHT VR CAMERAS
							//	if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<GlobalTranspFogSkyMaster> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<GlobalTranspFogSkyMaster> ();
							//			script.TerrainManager.LeftCam.GetComponent<GlobalTranspFogSkyMaster> ().SkyManager = script.SkyManager;
							//			script.TerrainManager.LeftCam.GetComponent<GlobalTranspFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//			script.TerrainManager.Lerp_gradient = true;
							//			script.TerrainManager.ImageEffectFog = true;
							//			script.TerrainManager.FogHeightByTerrain = true;
							//		} else {
							//			if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<GlobalTranspFogSkyMaster> () != null) {
							//				//setup existing
							//				script.TerrainManager.LeftCam.GetComponent<GlobalTranspFogSkyMaster> ().SkyManager = script.SkyManager;
							//				script.TerrainManager.LeftCam.GetComponent<GlobalTranspFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//				script.TerrainManager.Lerp_gradient = true;
							//				script.TerrainManager.ImageEffectFog = true;
							//				script.TerrainManager.FogHeightByTerrain = true;
							//			}
							//		}
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<GlobalTranspFogSkyMaster> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<GlobalTranspFogSkyMaster> ();
							//			script.TerrainManager.RightCam.GetComponent<GlobalTranspFogSkyMaster> ().SkyManager = script.SkyManager;
							//			script.TerrainManager.RightCam.GetComponent<GlobalTranspFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//			script.TerrainManager.Lerp_gradient = true;
							//			script.TerrainManager.ImageEffectFog = true;
							//			script.TerrainManager.FogHeightByTerrain = true;
							//		} else {
							//			if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<GlobalTranspFogSkyMaster> () != null) {
							//				//setup existing
							//				script.TerrainManager.RightCam.GetComponent<GlobalTranspFogSkyMaster> ().SkyManager = script.SkyManager;
							//				script.TerrainManager.RightCam.GetComponent<GlobalTranspFogSkyMaster> ().Sun = script.SkyManager.SUN_LIGHT.transform;
							//				script.TerrainManager.Lerp_gradient = true;
							//				script.TerrainManager.ImageEffectFog = true;
							//				script.TerrainManager.FogHeightByTerrain = true;
							//			}
							//		}	
							//	}
							//}

							EditorGUILayout.EndVertical ();
							EditorGUILayout.BeginVertical (GUILayout.MaxWidth (180.0f));

							//EditorGUILayout.BeginHorizontal ();

							//if (GUILayout.Button (new GUIContent ("Add sun shafts"), GUILayout.Width (150))) {			
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<SunShaftsSkyMaster> () == null) {
							//		Camera.main.gameObject.AddComponent<SunShaftsSkyMaster> ();
							//		Camera.main.gameObject.GetComponent<SunShaftsSkyMaster> ().sunTransform = script.SkyManager.SunObj.transform;
							//		//Debug.Log("CAMERA FOUND");
							//		script.TerrainManager.ImageEffectShafts = true;
							//	} else {
							//		if (Camera.main == null) {
							//			Debug.Log ("Add a main camera first");
							//		}
							//		if (Camera.main.gameObject.GetComponent<SunShaftsSkyMaster> () != null) {
							//			Camera.main.gameObject.GetComponent<SunShaftsSkyMaster> ().sunTransform = script.SkyManager.SunObj.transform;
							//			script.TerrainManager.ImageEffectShafts = true;
							//		}
							//	}		
							//	//LEFT-RIGHT VR CAMERAS
							//	if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<SunShaftsSkyMaster> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<SunShaftsSkyMaster> ();
							//			script.TerrainManager.LeftCam.GetComponent<SunShaftsSkyMaster> ().sunTransform = script.SkyManager.SunObj.transform;
							//			script.TerrainManager.ImageEffectShafts = true;
							//		} else {
							//			if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<SunShaftsSkyMaster> () != null) {
							//				script.TerrainManager.LeftCam.GetComponent<SunShaftsSkyMaster> ().sunTransform = script.SkyManager.SunObj.transform;
							//				script.TerrainManager.ImageEffectShafts = true;
							//			}
							//		}	
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<SunShaftsSkyMaster> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<SunShaftsSkyMaster> ();
							//			script.TerrainManager.RightCam.GetComponent<SunShaftsSkyMaster> ().sunTransform = script.SkyManager.SunObj.transform;
							//			script.TerrainManager.ImageEffectShafts = true;
							//		} else {
							//			if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<SunShaftsSkyMaster> () != null) {
							//				script.TerrainManager.RightCam.GetComponent<SunShaftsSkyMaster> ().sunTransform = script.SkyManager.SunObj.transform;
							//				script.TerrainManager.ImageEffectShafts = true;
							//			}
							//		}	
							//	}
							//}

							//if (GUILayout.Button (new GUIContent ("Add Color correction"), GUILayout.Width (150))) {			
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<ColorCorrectionCurvesSkyMaster> () == null) {
							//		Camera.main.gameObject.AddComponent<ColorCorrectionCurvesSkyMaster> ();
							//	} else {
							//		Debug.Log ("Add a main camera first");
							//	}

							//	//LEFT-RIGHT VR CAMERAS
							//	if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<ColorCorrectionCurvesSkyMaster> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<ColorCorrectionCurvesSkyMaster> ();
							//		}
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<ColorCorrectionCurvesSkyMaster> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<ColorCorrectionCurvesSkyMaster> ();
							//		}
							//	}
							//}

							//EditorGUILayout.EndHorizontal ();

							//if (GUILayout.Button (new GUIContent ("Add bloom"), GUILayout.Width (150))) {			
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<BloomSkyMaster> () == null) {
							//		Camera.main.gameObject.AddComponent<BloomSkyMaster> ();
							//	} else {
							//		Debug.Log ("Add a main camera first");
							//	}

							//	//LEFT-RIGHT VR CAMERAS
							//	if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<BloomSkyMaster> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<BloomSkyMaster> ();
							//		}
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<BloomSkyMaster> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<BloomSkyMaster> ();
							//		}
							//	}
							//}
							//if (GUILayout.Button (new GUIContent ("Add Underwater blur"), GUILayout.Width (150))) {			
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<UnderWaterImageEffect> () == null) {
							//		Camera.main.gameObject.AddComponent<UnderWaterImageEffect> ();
							//	} else {
							//		Debug.Log ("Add a main camera first");
							//	}

							//	//LEFT-RIGHT VR CAMERAS
							//	if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<UnderWaterImageEffect> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<UnderWaterImageEffect> ();
							//		}
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<UnderWaterImageEffect> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<UnderWaterImageEffect> ();
							//		}
							//	}
							//}

							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (new GUIContent ("Add Water Drops"), GUILayout.Width (150))) {	
								script.SkyManager.RainDropsPlane = (Instantiate (RainDropsPlane) as GameObject).transform;
								script.SkyManager.RainDropsPlane.parent = Camera.main.transform;
								script.SkyManager.RainDropsPlane.localPosition = Vector3.zero;
								script.SkyManager.ScreenRainDrops = true;
								script.SkyManager.ScreenRainDropsMat = ScreenRainDropsMat;

								//LEFT-RIGHT VR CAMERAS
								if (script.TerrainManager != null) {
									if (script.TerrainManager.LeftCam != null) {
										GameObject DropPlainL = Instantiate (RainDropsPlane) as GameObject;
										DropPlainL.transform.parent = script.TerrainManager.LeftCam.transform;
										DropPlainL.transform.localPosition = Vector3.zero;
									}
									if (script.TerrainManager.RightCam != null) {
										GameObject DropPlainL = Instantiate (RainDropsPlane) as GameObject;
										DropPlainL.transform.parent = script.TerrainManager.RightCam.transform;
										DropPlainL.transform.localPosition = Vector3.zero;
									}
								}
                                EditorUtility.SetDirty(script.SkyManager);//v4.9.2
                            }
							EditorGUILayout.EndHorizontal ();

							//EditorGUILayout.BeginHorizontal ();
							//if (GUILayout.Button (new GUIContent ("Add Aberrarion"), GUILayout.Width (150))) {	
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<VignetteAndChromaticAberrationSM> () == null) {
							//		Camera.main.gameObject.AddComponent<VignetteAndChromaticAberrationSM> ();
							//	} else {
							//		Debug.Log ("Add a main camera first");
							//	}	
							//	//LEFT-RIGHT VR CAMERAS
							//	if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<VignetteAndChromaticAberrationSM> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<VignetteAndChromaticAberrationSM> ();
							//		}
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<VignetteAndChromaticAberrationSM> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<VignetteAndChromaticAberrationSM> ();
							//		}
							//	}
							//}
							//EditorGUILayout.EndHorizontal ();

							//EditorGUILayout.BeginHorizontal ();
							//if (GUILayout.Button (new GUIContent ("Add Tone Mapping"), GUILayout.Width (150))) {	
							//	if (Camera.main != null && Camera.main.gameObject.GetComponent<TonemappingSM> () == null) {
							//		Camera.main.gameObject.AddComponent<TonemappingSM> ();
							//		Camera.main.gameObject.GetComponent<TonemappingSM> ().exposureAdjustment = 2.2f;
							//	} else {
							//		Debug.Log ("Add a main camera first");
							//	}		
							//	//LEFT-RIGHT VR CAMERAS
							//	if (script.TerrainManager != null) {
							//		if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<TonemappingSM> () == null) {
							//			script.TerrainManager.LeftCam.AddComponent<TonemappingSM> ();
							//		}
							//		if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<TonemappingSM> () == null) {
							//			script.TerrainManager.RightCam.AddComponent<TonemappingSM> ();
							//		}
							//	}
							//}
							//EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (new GUIContent ("Add Mouse Look"), GUILayout.Width (150))) {	
								if (Camera.main != null && Camera.main.gameObject.GetComponent<MouseLookSKYMASTER> () == null) {
									Camera.main.gameObject.AddComponent<MouseLookSKYMASTER> ();
								} else {
									Debug.Log ("Add a main camera first");
								}
								//LEFT-RIGHT VR CAMERAS
								if (script.TerrainManager != null) {
									if (script.TerrainManager.LeftCam != null && script.TerrainManager.LeftCam.GetComponent<MouseLookSKYMASTER> () == null) {
										script.TerrainManager.LeftCam.AddComponent<MouseLookSKYMASTER> ();
									}
									if (script.TerrainManager.RightCam != null && script.TerrainManager.RightCam.GetComponent<MouseLookSKYMASTER> () == null) {
										script.TerrainManager.RightCam.AddComponent<MouseLookSKYMASTER> ();
									}
								}
							}
							EditorGUILayout.EndHorizontal ();


//				if(GUILayout.Button(new GUIContent("Add Depth of Field"),GUILayout.Width(120))){			
//					if(Camera.main != null && Camera.main.gameObject.GetComponent<>() == null){
//						Camera.main.gameObject.AddComponent<SunShaftsSkyMaster>();
//					}else{
//						Debug.Log ("Add a main camera first");
//					}					
//				}
				
				
							EditorGUILayout.EndHorizontal ();				
							EditorGUIUtility.wideMode = false;
				
							EditorGUILayout.BeginHorizontal ();
				
							//ParticleScaleFactor = EditorGUILayout.FloatField(ParticleScaleFactor,GUILayout.MaxWidth(95.0f));
							//ParticlePlaybackScaleFactor = EditorGUILayout.FloatField(ParticlePlaybackScaleFactor,GUILayout.MaxWidth(95.0f));
							EditorGUILayout.EndHorizontal ();
				
				
				
						} else {

							EditorGUILayout.HelpBox ("Please configure a terrain first, before adding fog, sun shafts and other filters to the Main Camera and VR Cameras", MessageType.Warning);

							//Debug.Log("Please configure a terrain first, before adding fog, sun shafts and other filters to the Main Camera and VR Cameras");
						}
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();



						//v3.3e
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
						EditorGUILayout.HelpBox ("Volumetric gradient fog", MessageType.None);



						EditorGUILayout.HelpBox ("Volume fog parameter apply speed", MessageType.None);
						if (script.TerrainManager != null) {
							script.TerrainManager.VolumeFogSpeed = EditorGUILayout.Slider (script.TerrainManager.VolumeFogSpeed, 1, 1000, GUILayout.MaxWidth (sliderWidth));

							//enabled curves
							EditorGUILayout.HelpBox ("Enable volume fog control curves", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							//script.TerrainManager.UseFogCurves = EditorGUILayout.Toggle ("Use volume fog control curves", script.TerrainManager.UseFogCurves, GUILayout.MaxWidth (380.0f));
							script.TerrainManager.UseFogCurves = EditorGUILayout.Toggle (script.TerrainManager.UseFogCurves, GUILayout.MaxWidth (380.0f));
							EditorGUILayout.EndHorizontal ();
						}
						if (script.TerrainManager != null && script.TerrainManager.UseFogCurves) {

							//v3.4.3
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (new GUIContent ("Sample fog Curves A - Default"), GUILayout.Width (205))) {
								script.TerrainManager.setVFogCurvesPresetA ();
							}
							if (GUILayout.Button (new GUIContent ("Sample fog Curves B - Vivid dusk"), GUILayout.Width (205))) {
								script.TerrainManager.setVFogCurvesPresetB ();
							}
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (new GUIContent ("Sample fog Curves C - Misty"), GUILayout.Width (205))) {
								script.TerrainManager.setVFogCurvesPresetC ();
							}
							if (GUILayout.Button (new GUIContent ("Sample fog Curves D - Rainy"), GUILayout.Width (205))) {
								script.TerrainManager.setVFogCurvesPresetD ();
							}
							EditorGUILayout.EndHorizontal ();

							//v3.4 - fog toggles
							script.TerrainManager.DistanceFogOn = EditorGUILayout.Toggle ("Distance Fog",script.TerrainManager.DistanceFogOn, GUILayout.MaxWidth (380.0f));
							script.TerrainManager.HeightFogOn = EditorGUILayout.Toggle ("Height Fog",script.TerrainManager.HeightFogOn, GUILayout.MaxWidth (380.0f));
							script.TerrainManager.SkyFogOn = EditorGUILayout.Toggle ("Sky Fog",script.TerrainManager.SkyFogOn, GUILayout.MaxWidth (380.0f));

							EditorGUILayout.HelpBox ("Volume fog distance from camera", MessageType.None);
							script.TerrainManager.VFogDistance = EditorGUILayout.Slider (script.TerrainManager.VFogDistance, 0, 5000, GUILayout.MaxWidth (sliderWidth));

							EditorGUILayout.HelpBox ("Volume fog density", MessageType.None);
							script.TerrainManager.fogDensity = EditorGUILayout.Slider (script.TerrainManager.fogDensity, 0, 500, GUILayout.MaxWidth (sliderWidth)); //v3.4.3

							EditorGUILayout.HelpBox ("Volume fog gradient extend to horizon", MessageType.None);
							script.TerrainManager.fogGradientDistance = EditorGUILayout.Slider (script.TerrainManager.fogGradientDistance, 0, 35000, GUILayout.MaxWidth (sliderWidth));

							EditorGUILayout.HelpBox ("Volume fog height curve offset", MessageType.None);
							script.TerrainManager.AddFogHeightOffset = EditorGUILayout.Slider (script.TerrainManager.AddFogHeightOffset, -10000, 10000, GUILayout.MaxWidth (sliderWidth));//v3.4.3

							EditorGUILayout.HelpBox ("Volume fog height offset", MessageType.None);
							if (script.TerrainManager.heightOffsetFogCurve != null) {
								//script.TerrainManager.heightOffsetFogCurve = EditorGUILayout.CurveField (script.TerrainManager.heightOffsetFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.heightOffsetFogCurve = script.TerrainManager.heightOffsetFogCurve;
								EditorGUILayout.PropertyField(heightOffsetFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.heightOffsetFogCurve = script.heightOffsetFogCurve;
							} else {
								//Keyframe[] CurveKeys = new Keyframe[4];
								//CurveKeys [0] = new Keyframe (0, 0.001f);
								//CurveKeys [1] = new Keyframe (0.3f, 0.001f);
								//CurveKeys [2] = new Keyframe (0.7f, 0.001f);
								//CurveKeys [3] = new Keyframe (1, 0.001f);
								//script.TerrainManager.heightOffsetFogCurve = new AnimationCurve (CurveKeys);
							}
							EditorGUILayout.HelpBox ("Volume fog luminance offset", MessageType.None);
							if (script.TerrainManager.luminanceVFogCurve != null) {
								//script.TerrainManager.luminanceVFogCurve = EditorGUILayout.CurveField (script.TerrainManager.luminanceVFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.luminanceVFogCurve = script.TerrainManager.luminanceVFogCurve;
								EditorGUILayout.PropertyField(luminanceVFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.luminanceVFogCurve = script.luminanceVFogCurve;
							} else {
								//Init
							}
							EditorGUILayout.HelpBox ("Volume fog luminance factor offset", MessageType.None);
							if (script.TerrainManager.lumFactorFogCurve != null) {
								//script.TerrainManager.lumFactorFogCurve = EditorGUILayout.CurveField (script.TerrainManager.lumFactorFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.lumFactorFogCurve = script.TerrainManager.lumFactorFogCurve;
								EditorGUILayout.PropertyField(lumFactorFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.lumFactorFogCurve = script.lumFactorFogCurve;
							} else {
								//Init
							}
							EditorGUILayout.HelpBox ("Volume fog scatter factor offset", MessageType.None);
							if (script.TerrainManager.scatterFacFogCurve != null) {
								//script.TerrainManager.scatterFacFogCurve = EditorGUILayout.CurveField (script.TerrainManager.scatterFacFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.scatterFacFogCurve = script.TerrainManager.scatterFacFogCurve;
								EditorGUILayout.PropertyField(scatterFacFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.scatterFacFogCurve = script.scatterFacFogCurve;
							} else {
								//Init
							}
							EditorGUILayout.HelpBox ("Volume fog turbidity offset", MessageType.None);
							if (script.TerrainManager.turbidityFogCurve != null) {
								//script.TerrainManager.turbidityFogCurve = EditorGUILayout.CurveField (script.TerrainManager.turbidityFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.turbidityFogCurve = script.TerrainManager.turbidityFogCurve;
								EditorGUILayout.PropertyField(turbidityFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.turbidityFogCurve = script.turbidityFogCurve;
							} else {
								//Init
							}
							EditorGUILayout.HelpBox ("Volume fog turbidity factor offset", MessageType.None);
							if (script.TerrainManager.turbFacFogCurve != null) {
								//script.TerrainManager.turbFacFogCurve = EditorGUILayout.CurveField (script.TerrainManager.turbFacFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.turbFacFogCurve = script.TerrainManager.turbFacFogCurve;
								EditorGUILayout.PropertyField(turbFacFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.turbFacFogCurve = script.turbFacFogCurve;
							} else {
								//Init
							}
							EditorGUILayout.HelpBox ("Volume fog horizon spread offset", MessageType.None);
							if (script.TerrainManager.horizonFogCurve != null) {
								//script.TerrainManager.horizonFogCurve = EditorGUILayout.CurveField (script.TerrainManager.horizonFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.horizonFogCurve = script.TerrainManager.horizonFogCurve;
								EditorGUILayout.PropertyField(horizonFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.horizonFogCurve = script.horizonFogCurve;
							} else {
								//Init
							}
							EditorGUILayout.HelpBox ("Volume fog contrast offset", MessageType.None);
							if (script.TerrainManager.contrastFogCurve != null) {
								//script.TerrainManager.contrastFogCurve = EditorGUILayout.CurveField (script.TerrainManager.contrastFogCurve, GUILayout.MaxWidth (415.0f));
								//v3.4.5
								script.contrastFogCurve = script.TerrainManager.contrastFogCurve;
								EditorGUILayout.PropertyField(contrastFogCurve, GUIContent.none,GUILayout.MaxWidth (415.0f));
								script.TerrainManager.contrastFogCurve = script.contrastFogCurve;
							} else {
								//Init
							}
						}
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();



						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
						//TONE MAP CONTROL
						//if (Camera.main != null) {
						//	TonemappingSM ToneMapper = Camera.main.GetComponent<TonemappingSM> () as TonemappingSM;
						//	if (ToneMapper != null) {
						//		EditorGUILayout.HelpBox ("Tone mapper brightness", MessageType.None);
						//		EditorGUILayout.BeginHorizontal ();
						//		ToneMapper.exposureAdjustment = EditorGUILayout.Slider (ToneMapper.exposureAdjustment, 0.5f, 5, GUILayout.MaxWidth (sliderWidth));
						//		EditorGUILayout.EndHorizontal ();
						//	}
						//}

						if (script.TerrainManager != null) {
							EditorGUILayout.HelpBox ("Sun Shaft size", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.TerrainManager.Shafts_intensity = EditorGUILayout.Slider (script.TerrainManager.Shafts_intensity, 0, 100, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("Moon Shaft size", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.TerrainManager.Moon_Shafts_intensity = EditorGUILayout.Slider (script.TerrainManager.Moon_Shafts_intensity, 0, 100, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("Sun Shaft length", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.TerrainManager.ShaftBlurRadiusOffset = EditorGUILayout.Slider (script.TerrainManager.ShaftBlurRadiusOffset, 0, 100, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	
						}
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
						//splashes control
						if (script.SkyManager.RainDropsPlane != null) {
							EditorGUILayout.HelpBox ("Water splash amount", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.MaxWater = EditorGUILayout.Slider (script.SkyManager.MaxWater, 0, 15, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Water refraction", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.MaxRefract = EditorGUILayout.Slider (script.SkyManager.MaxRefract, 0, 15, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Water Freeze speed", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.FreezeSpeed = EditorGUILayout.Slider (script.SkyManager.FreezeSpeed, 0, 15, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();


							EditorGUILayout.HelpBox ("Enable water freeze", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.ScreenFreezeFX = EditorGUILayout.Toggle (script.SkyManager.ScreenFreezeFX, GUILayout.MaxWidth (195.0f));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Freeze inwards", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.FreezeInwards = EditorGUILayout.Toggle (script.SkyManager.FreezeInwards, GUILayout.MaxWidth (195.0f));
							EditorGUILayout.EndHorizontal ();
							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
						}
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

						EditorGUILayout.HelpBox ("Orthographic camera", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Ortho_cam = EditorGUILayout.Toggle (script.SkyManager.Ortho_cam, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();
						if (script.SkyManager.Ortho_cam) {
							EditorGUILayout.HelpBox ("Orthographic correction", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.SkyManager.Ortho_factor = EditorGUILayout.Slider (script.SkyManager.Ortho_factor, -2, 2, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
						}

						//DUAL CAMERA SETUP
						EditorGUILayout.HelpBox ("VR Cameras - Left/Right - Define to add effects - Terrain Script is required", MessageType.None);
						if (script.TerrainManager != null) {
							script.TerrainManager.LeftCam = EditorGUILayout.ObjectField (script.TerrainManager.LeftCam, typeof(GameObject), true, GUILayout.MaxWidth (180.0f)) as GameObject;
							script.TerrainManager.RightCam = EditorGUILayout.ObjectField (script.TerrainManager.RightCam, typeof(GameObject), true, GUILayout.MaxWidth (180.0f)) as GameObject;
						}
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();



					}
					EditorGUILayout.EndVertical ();
					///////////////////////////////////////////////////////////////////////////

				}//END TAB3





				/////////////////////////////////////////////////////////////////////////////// WEATHER - EVENTS
	//			EditorGUILayout.BeginVertical (GUILayout.MaxWidth (180.0f));			
				//X_offset_left = 200;
				//Y_offset_top = 100;			
				//GUILayout.Box("",GUILayout.Height(3),GUILayout.Width(410));
			
				//TAB4
				if ((script.UseTabs && script.currentTab == 4) | !script.UseTabs) {


					GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	
		
					GUI.backgroundColor = Color.blue * 0.2f;
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f)); //v3.3e
					GUI.backgroundColor = Color.white;


					GUILayout.Label (MainIcon5, GUILayout.MaxWidth (410.0f));
			
					EditorGUILayout.LabelField ("Weather", EditorStyles.boldLabel);			
					EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
					GUILayout.Space (10);
					script.weather_folder1 = EditorGUILayout.Foldout (script.weather_folder1, "Weather events");
					EditorGUILayout.EndHorizontal ();
			
					if (script.weather_folder1) {

//						//v3.4.3
//						if (script.SkyManager == null) {
//							EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f)); //v3.3e
//							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));
//							Debug.Log ("Add Sky to enable Weather options");
//							EditorGUILayout.HelpBox ("Please add Sky to enable Weather option", MessageType.None);
//							EditorGUILayout.EndVertical ();
//						}

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f)); //v3.3e
						//choose weather
						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
						EditorGUILayout.HelpBox ("Current weather", MessageType.None);
						if (script.SkyManager != null) {
							script.SkyManager.currentWeatherName = (Artngame.SKYMASTER.SkyMasterManager.Volume_Weather_types)EditorGUILayout.EnumPopup (script.SkyManager.currentWeatherName, GUILayout.Width (120));
						}

						//v3.4
						EditorGUILayout.HelpBox ("Weather severity", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.WeatherSeverity = EditorGUILayout.Slider (script.SkyManager.WeatherSeverity, 1f, 20, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
						EditorGUILayout.HelpBox ("Snow coverage", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.MaxSnowCoverage = EditorGUILayout.Slider (script.SkyManager.MaxSnowCoverage, 1f, 8, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.HelpBox ("Snow coverage speed (terrain)", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.SnowTerrRateFactor = EditorGUILayout.Slider (script.SkyManager.SnowTerrRateFactor, 1f, 1000, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (180.0f)); //v3.3e


						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
						EditorGUILayout.HelpBox ("Enable Unity fog (Editor use)", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Use_fog = EditorGUILayout.Toggle (script.SkyManager.Use_fog, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();

                        //v4.8.7
                        GUILayout.Box("", GUILayout.Height(2), GUILayout.Width(410));
                        EditorGUILayout.HelpBox("Color fog by Sky Gradient", MessageType.None);
                        EditorGUILayout.BeginHorizontal();
                        script.SkyManager.gradAffectsFog = EditorGUILayout.Toggle(script.SkyManager.gradAffectsFog, GUILayout.MaxWidth(195.0f));
                        EditorGUILayout.EndHorizontal();

                        //v4.8.7 - fog mode
                        GUILayout.Box("", GUILayout.Height(2), GUILayout.Width(410));
                        EditorGUILayout.HelpBox("Unity Fog mode", MessageType.None);
                        if (script.SkyManager != null)
                        {
                            script.SkyManager.fogMode = (UnityEngine.FogMode)EditorGUILayout.EnumPopup(script.SkyManager.fogMode, GUILayout.Width(120));
                        }

                        //v3.3c
                        EditorGUILayout.HelpBox ("Fog color Day", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Spring_fog_day = EditorGUILayout.ColorField (script.SkyManager.Spring_fog_day, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.HelpBox ("Fog color Dusk", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Spring_fog_dusk = EditorGUILayout.ColorField (script.SkyManager.Spring_fog_dusk, GUILayout.MaxWidth (195.0f));
						EditorGUILayout.EndHorizontal ();

						if (script.TerrainManager != null) {
							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
							EditorGUILayout.HelpBox ("Use Transparent Volume Fog (Volume Cloud interaction)", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.TerrainManager.UseTranspVFog = EditorGUILayout.Toggle (script.TerrainManager.UseTranspVFog, GUILayout.MaxWidth (195.0f));
							EditorGUILayout.EndHorizontal ();
						}

						//SELECT UNITY FOG DENSITY
						GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
						EditorGUILayout.HelpBox ("Unity fog density", MessageType.None);
						EditorGUILayout.BeginHorizontal ();
						script.SkyManager.Fog_Density_Mult = EditorGUILayout.Slider (script.SkyManager.Fog_Density_Mult, 0.1f, 100, GUILayout.MaxWidth (sliderWidth));
						EditorGUILayout.EndHorizontal ();

						if (script.TerrainManager != null) {
							// OFFSET VOLUME FOG DENSITY and HEIGHT
							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
							EditorGUILayout.HelpBox ("Volume fog density", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.TerrainManager.AddFogDensityOffset = EditorGUILayout.Slider (script.TerrainManager.AddFogDensityOffset, -2, 10, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							if (!script.TerrainManager.UseFogCurves) {
								GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));	
								EditorGUILayout.HelpBox ("Volume fog height", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.TerrainManager.AddFogHeightOffset = EditorGUILayout.Slider (script.TerrainManager.AddFogHeightOffset, -200, 200, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
							}

							//v3.3e
							GUILayout.Box ("", GUILayout.Height (2), GUILayout.Width (410));
							if (script.SkyManager.UseGradients) {
								EditorGUILayout.HelpBox ("Sky colored fog power", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.SkyManager.FogColorPow = EditorGUILayout.Slider (script.SkyManager.FogColorPow, -25, 15, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
							}
						}

						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();

						//RANDOM WEATHER EVENTS ADDTION !!! Also decide whether there will be automatic seasonal changes
						//	GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	
						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Add event"), GUILayout.Width (120))) {			
//					if(UnityTerrain != null){
//						
//					}else{
//						Debug.Log ("Add Unity terrain first");
//					}
							WeatherEventSM WeatherEvent = new WeatherEventSM ();
							WeatherEvent.Chance = WeatherEvent_Chance;
							WeatherEvent.EventStartHour = WeatherEvent_StartHour;
							WeatherEvent.EventStartDay = WeatherEvent_StartDay;
							WeatherEvent.EventStartMonth = WeatherEvent_StartMonth;
							WeatherEvent.EventEndHour = WeatherEvent_EndHour;
							WeatherEvent.EventEndDay = WeatherEvent_EndDay;
							WeatherEvent.EventEndMonth = WeatherEvent_EndMonth;
							WeatherEvent.SkyManager = script.SkyManager;
							WeatherEvent.VolCloudHeight = WeatherEvent_VolCloudHeight;
							WeatherEvent.VolCloudsHorScale = WeatherEvent_VolCloudsHorScale;
							//WeatherEvent.VolumeCloudsPREFAB = WeatherEvent_VolumeCloudsPREFAB;
							WeatherEvent.Weather_type = WeatherEvent_Weather_type;//WeatherEventSM.Volume_Weather_event_types.Cloudy;
							WeatherEvent.loop = WeatherEvent_Loop;//v3.4

							WeatherEvent.FollowUpWeather = FollowUpWeatherEvent_Weather_type;//v3.4

							script.SkyManager.WeatherEvents.Add (WeatherEvent);
						}
				

						EditorGUILayout.EndHorizontal ();


						//v3.4
						EditorGUILayout.HelpBox ("Weather events have two modes, one is specific day definition and the other the cycle mode where the month and day in the month is specified and the event will repeat every year the specific date." +
							" In non loop mode the month is not used-required and day should be calculated from game start (e.g for January 1st in 2ond year is 365 plus 1 days and the event will trigger only once at that time).", MessageType.None);

						EditorGUILayout.BeginHorizontal ();
				
						//EditorGUILayout.LabelField("Chance:"+script.SkyManager.WeatherEvents[i].Chance,EditorStyles.boldLabel,GUILayout.MaxWidth(125.0f));
						//					EditorGUILayout.HelpBox("Chance",MessageType.None);
						//					EditorGUILayout.HelpBox("Time span",MessageType.None);
						//					EditorGUILayout.HelpBox("Day span",MessageType.None);
						//					EditorGUILayout.HelpBox("Month span",MessageType.None);
						//					EditorGUILayout.HelpBox("Weather type",MessageType.None);
						EditorGUILayout.TextField ("Chance", GUILayout.MaxWidth (55.0f));
						EditorGUILayout.TextField ("Time span", GUILayout.MaxWidth (75.0f));
						EditorGUILayout.TextField ("Day span", GUILayout.MaxWidth (75.0f));
						EditorGUILayout.TextField ("Month span", GUILayout.MaxWidth (75.0f));
						EditorGUILayout.TextField ("Weather", GUILayout.MaxWidth (75.0f));
						EditorGUILayout.TextField ("Loop", GUILayout.MaxWidth (35.0f));
						EditorGUILayout.TextField ("E", GUILayout.MaxWidth (15.0f));
						EditorGUILayout.EndHorizontal ();
						//EditorGUILayout.BeginVertical();
						for (int i = 0; i < script.SkyManager.WeatherEvents.Count; i++) {




							//REMOVE EVENT BUTTON
							EditorGUILayout.BeginHorizontal ();
						
							//EditorGUILayout.LabelField("Chance:"+script.SkyManager.WeatherEvents[i].Chance,EditorStyles.boldLabel,GUILayout.MaxWidth(125.0f));
							EditorGUILayout.TextField (script.SkyManager.WeatherEvents [i].Chance.ToString (), GUILayout.MaxWidth (55.0f));
							EditorGUILayout.TextField (script.SkyManager.WeatherEvents [i].EventStartHour + "-" + script.SkyManager.WeatherEvents [i].EventEndHour, GUILayout.MaxWidth (75.0f));
							EditorGUILayout.TextField (script.SkyManager.WeatherEvents [i].EventStartDay + "-" + script.SkyManager.WeatherEvents [i].EventEndDay, GUILayout.MaxWidth (75.0f));
							EditorGUILayout.TextField (script.SkyManager.WeatherEvents [i].EventStartMonth + "-" + script.SkyManager.WeatherEvents [i].EventEndMonth, GUILayout.MaxWidth (75.0f));
							EditorGUILayout.TextField (script.SkyManager.WeatherEvents [i].Weather_type.ToString (), GUILayout.MaxWidth (75.0f));
							EditorGUILayout.TextField (script.SkyManager.WeatherEvents [i].loop.ToString (), GUILayout.MaxWidth (35.0f));//v3.4
//					EditorGUILayout.HelpBox("Chance:"+script.SkyManager.WeatherEvents[i].Chance,MessageType.None);
//					EditorGUILayout.HelpBox("Time span:"+script.SkyManager.WeatherEvents[i].EventStartHour+"-"+script.SkyManager.WeatherEvents[i].EventEndHour,MessageType.None);
//					EditorGUILayout.HelpBox("Day span:"+script.SkyManager.WeatherEvents[i].EventStartDay+"-"+script.SkyManager.WeatherEvents[i].EventEndDay,MessageType.None);
//					EditorGUILayout.HelpBox("Month span:"+script.SkyManager.WeatherEvents[i].EventStartMonth+"-"+script.SkyManager.WeatherEvents[i].EventEndMonth,MessageType.None);
//					EditorGUILayout.HelpBox("Weather type:"+script.SkyManager.WeatherEvents[i].Weather_type,MessageType.None);
							if (GUILayout.Button (new GUIContent ("-"), GUILayout.Width (15))) {
								//if(GUILayout.Button(new GUIContent("Remove event"),GUILayout.Width(120))){
								script.SkyManager.WeatherEvents.RemoveAt (i);
							}
							EditorGUILayout.EndHorizontal ();
						}
						//EditorGUILayout.EndVertical();


						EditorGUIUtility.wideMode = false;
				
						EditorGUILayout.BeginHorizontal ();				
						//ParticleScaleFactor = EditorGUILayout.FloatField(ParticleScaleFactor,GUILayout.MaxWidth(95.0f));
						//ParticlePlaybackScaleFactor = EditorGUILayout.FloatField(ParticlePlaybackScaleFactor,GUILayout.MaxWidth(95.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Weather type:", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_Weather_type = (WeatherEventSM.Volume_Weather_event_types)EditorGUILayout.EnumPopup (WeatherEvent_Weather_type);
						EditorGUILayout.EndHorizontal ();

						//v3.4
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Weather Follow:", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						FollowUpWeatherEvent_Weather_type = (WeatherEventSM.Volume_Weather_event_types)EditorGUILayout.EnumPopup (FollowUpWeatherEvent_Weather_type);
						EditorGUILayout.EndHorizontal ();
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Loop weather:", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_Loop = EditorGUILayout.Toggle (WeatherEvent_Loop, GUILayout.MaxWidth (95.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Weather chance (%):", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_Chance = EditorGUILayout.FloatField (WeatherEvent_Chance, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Start hour (1-24):", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_StartHour = EditorGUILayout.FloatField (WeatherEvent_StartHour, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Start day (1-30 for loop):", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_StartDay = EditorGUILayout.IntField (WeatherEvent_StartDay, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Start month (1-12 for loop):", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_StartMonth = EditorGUILayout.IntField (WeatherEvent_StartMonth, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("End hour (1-24):", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_EndHour = EditorGUILayout.FloatField (WeatherEvent_EndHour, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("End day (1-30 for loop):", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_EndDay = EditorGUILayout.IntField (WeatherEvent_EndDay, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("End month (1-12 for loop):", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_EndMonth = EditorGUILayout.IntField (WeatherEvent_EndMonth, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Volume cloud height:", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_VolCloudHeight = EditorGUILayout.FloatField (WeatherEvent_VolCloudHeight, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Volume cloud span:", EditorStyles.boldLabel, GUILayout.MaxWidth (200.0f));
						WeatherEvent_VolCloudsHorScale = EditorGUILayout.FloatField (WeatherEvent_VolCloudsHorScale, GUILayout.MaxWidth (75.0f));
						EditorGUILayout.EndHorizontal ();




						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();

					}
					EditorGUILayout.EndVertical ();
					///////////////////////////////////////////////////////////////////////////


				}//END TAB4





				/////////////////////////////////////////////////////////////////////////////// FOLIAGE
			//	EditorGUILayout.BeginVertical (GUILayout.MaxWidth (180.0f));			
				//X_offset_left = 200;
				//Y_offset_top = 100;			
				//GUILayout.Box("",GUILayout.Height(3),GUILayout.Width(410));
			
				//TAB5
				if ((script.UseTabs && script.currentTab == 5) | !script.UseTabs) {


					GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	
			
					GUI.backgroundColor = Color.blue * 0.2f;
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
					GUI.backgroundColor = Color.white;

					GUILayout.Label (MainIcon6, GUILayout.MaxWidth (410.0f));
			
					EditorGUILayout.LabelField ("Foliage", EditorStyles.boldLabel);			
					EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
					GUILayout.Space (10);
					script.foliage_folder1 = EditorGUILayout.Foldout (script.foliage_folder1, "Foliage");
					EditorGUILayout.EndHorizontal ();
			
					if (script.foliage_folder1) {

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Setup snow on mesh"), GUILayout.Width (150))) {			
							//add material to mesh
							(snowMESH as GameObject).GetComponent<MeshRenderer> ().material = MeshTerrainSnowMat;
						}
						snowMESH = EditorGUILayout.ObjectField (snowMESH, typeof(GameObject), true, GUILayout.MaxWidth (150.0f)) as GameObject;
						//UnityTerrain =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
						EditorGUILayout.EndHorizontal ();
				
						EditorGUILayout.HelpBox ("For Unity terrain trees & grass, replacement shaders are used 'SkyMaster->Assets->Version2.2->TerrainShading->Resources' folder, that enable snow coverage on trees and billboards. " +
							"To restore these shaders" +
							" after an erase if they are not required, extract the 'Unity_terrain_foliage_SM3_overrides.unitypackage' file located in the V3.0 assets, foliage folder", MessageType.Warning);
					
						EditorGUILayout.HelpBox ("For SpeedTree trees, please extract the 'SpeedTree_SM3.unitypackage' file that can be downloaded from the Unity Forum thread.", MessageType.Warning);
						if (GUILayout.Button ("Download SpeedTree snow shaders", GUILayout.MaxWidth (405.0f), GUILayout.MaxHeight (210.0f))) {
							Application.OpenURL ("http://forum.unity3d.com/threads/discount-until-v3-sky-master-2-the-one-draw-call-3d-volume-cloud-fog-physically-based-rendering.280612/");
						}

						EditorGUILayout.HelpBox ("Sky Master gradual snow growth can be directly used with InfiniGRASS asset grass & foliage and is activated in snow conditions." +
						"The water module is compatible with the InfiniGRASS shaders. For more information on InfiniGRASS, press below:", MessageType.Warning);
						if (GUILayout.Button (InfiniGRASS_ICON, GUILayout.MaxWidth (405.0f), GUILayout.MaxHeight (210.0f))) {
							Application.OpenURL ("http://u3d.as/jiM");
						}


						EditorGUIUtility.wideMode = false;
				
						EditorGUILayout.BeginHorizontal ();
				
						//ParticleScaleFactor = EditorGUILayout.FloatField(ParticleScaleFactor,GUILayout.MaxWidth(95.0f));
						//ParticlePlaybackScaleFactor = EditorGUILayout.FloatField(ParticlePlaybackScaleFactor,GUILayout.MaxWidth(95.0f));
						EditorGUILayout.EndHorizontal ();
				
				
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();
				
					}
					EditorGUILayout.EndVertical ();
					///////////////////////////////////////////////////////////////////////////

				}//END TAB5







				/////////////////////////////////////////////////////////////////////////////// WATER
		//		EditorGUILayout.BeginVertical (GUILayout.MaxWidth (180.0f));			
				//X_offset_left = 200;
				//Y_offset_top = 100;			
				//GUILayout.Box("",GUILayout.Height(3),GUILayout.Width(410));
			
				//TAB6
				if ((script.UseTabs && script.currentTab == 6) | !script.UseTabs) {


					GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	
			
					GUI.backgroundColor = Color.blue * 0.2f;
					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
					GUI.backgroundColor = Color.white;

					GUILayout.Label (MainIcon7, GUILayout.MaxWidth (410.0f));
			
					EditorGUILayout.LabelField ("Water", EditorStyles.boldLabel);			
					EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
					GUILayout.Space (10);
					script.water_folder1 = EditorGUILayout.Foldout (script.water_folder1, "Water");
					EditorGUILayout.EndHorizontal ();
			
					if (script.water_folder1) {

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

						//v3.4.3
						EditorGUILayout.HelpBox ("Configuration of water planes. The default option adds a tile based arrangement, which can be further adjusted individually for cases like rivers and" +
							" multilayered water pools.", MessageType.None);
						EditorGUILayout.HelpBox ("The Radial planes option (Set Radial Large Plane checkbox before pressing 'Add water' button) adds 4 planes with the detail distributed radially " +
							"closer to the center where the planes meet and can be used together with the Camera Follow option for endless ocean case.", MessageType.None);

						//if(script.WaterManager != null){
						EditorGUILayout.BeginHorizontal ();
						script.LargeWaterPlane = EditorGUILayout.Toggle ("Radial large plane", script.LargeWaterPlane, GUILayout.MaxWidth (280.0f));
						EditorGUILayout.EndHorizontal ();
						//}

						if (!script.LargeWaterPlane) {
							EditorGUILayout.BeginHorizontal ();
							GUILayout.Label ("Tiles X", GUILayout.Width (50));
							Water_tiles.x = EditorGUILayout.FloatField (Water_tiles.x, GUILayout.Width (50));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							GUILayout.Label ("Tiles Y", GUILayout.Width (50));
							Water_tiles.y = EditorGUILayout.FloatField (Water_tiles.y, GUILayout.Width (50));
							EditorGUILayout.EndHorizontal ();
						} else {
							EditorGUILayout.BeginHorizontal ();
							GUILayout.Label ("Plane scale", GUILayout.Width (120));
							script.LargeWaterPlaneScale = EditorGUILayout.FloatField (script.LargeWaterPlaneScale, GUILayout.MaxWidth (95.0f));
							EditorGUILayout.EndHorizontal ();
						}



						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button (new GUIContent ("Add Water"), GUILayout.Width (120))) {			
							//add water in start point, add tiles from prefab and pass water size
							//control fog based on collider or position

							//if (Camera.main != null && Camera.main.gameObject.GetComponent<UnderWaterImageEffect> () == null) {
							//	Camera.main.gameObject.AddComponent<UnderWaterImageEffect> ();
							//} else {
							//	Debug.Log ("Add a main camera and then re-add water for Underwater blur image effect");
							//}

							//pass SeasonalTerrain script to WaterHandler to change fog if underwater
							Vector3 MapcenterPos = script.SkyManager.MapCenter.position;

							GameObject Water = null;

							if (script.SkyManager.water != null) {
								Water = script.SkyManager.water.gameObject;
							}

							float Height = MapcenterPos.y;
							if (script.TerrainManager != null) {
								Height = script.TerrainManager.gameObject.transform.position.y;
							} else if (Terrain.activeTerrain != null) {
								Height = Terrain.activeTerrain.gameObject.transform.position.y;
							} else if (script.SkyManager.Mesh_terrain != null) {
								Height = script.SkyManager.Mesh_terrain.position.y;
							}
					
							//float tiles_count = Water_tiles.x * Water_tiles.y;
							//Vector3 Tilepos = Vector3.zero;
							//for (int i=0;i<tiles_count;i++){
							Vector3 Start_pos = MapcenterPos - new Vector3 ((Water_tiles.x * tileSize) / 2, Height + 3, (Water_tiles.y * tileSize) / 2);

							if (Water == null) {
								Water = (GameObject)Instantiate (WaterPREFAB, MapcenterPos + new Vector3 (0, 3, 0), Quaternion.identity);
								Water.AddComponent<WaterHandlerSM> ().Water_start = MapcenterPos;
								Water.GetComponent<WaterHandlerSM> ().Water_size = Water_tiles;
								Water.GetComponent<WaterHandlerSM> ().TerrainManager = script.TerrainManager;
								Water.GetComponent<WaterHandlerSM> ().SkyManager = script.SkyManager;
								Water.GetComponent<WaterHandlerSM> ().oceanMat = WaterMat;
								Water.GetComponent<WaterHandlerSM> ().SpecularSource = Water.GetComponent<SpecularLightingSM> ();
								Water.GetComponent<WaterHandlerSM> ().WaterBase = Water.GetComponent<WaterBaseSM> ();

								Water.GetComponent<WaterHandlerSM> ().SeaAudio = Water.GetComponent<AudioSource> () as AudioSource;

								Water.GetComponent<SpecularLightingSM> ().specularLight = script.SkyManager.SunObj.transform;




								Water.transform.localScale = WaterScale;

								//add caustics
								GameObject WaterCaustics = (GameObject)Instantiate (CausticsPREFAB, MapcenterPos + new Vector3 (0, 3, 0), Quaternion.identity);
								//pass projector and material to water controller

								//add to skymanager
								script.SkyManager.water = (Water as GameObject).transform;
								script.WaterManager = Water.GetComponent<WaterHandlerSM> ();

								//pass caustic properties
								script.WaterManager.CausticsProjector = WaterCaustics.GetComponentsInChildren<Projector> () [0];
								script.WaterManager.CausticsMat = script.WaterManager.CausticsProjector.material;
							}

							//add central - WaterTileLargePREFAB
							if (script.LargeWaterPlane) {
						
								Vector3 Start_pos1 = Vector3.zero;
						
								GameObject WaterTile1 = (GameObject)Instantiate (WaterTileLargePREFAB);
								Vector3 StartScale = WaterTile1.transform.localScale;
								WaterTile1.transform.parent = Water.transform;
								WaterTile1.transform.position = new Vector3 (Start_pos.x, Height + 3, Start_pos.z);
								WaterTile1.transform.localScale = new Vector3 (StartScale.x * script.LargeWaterPlaneScale, StartScale.y, StartScale.z * script.LargeWaterPlaneScale);
								WaterTile1.transform.localPosition = Start_pos1;
						
								WaterTile1 = (GameObject)Instantiate (WaterTileLargePREFAB);
								WaterTile1.transform.parent = Water.transform;
								WaterTile1.transform.position = new Vector3 (Start_pos.x, Height + 3, Start_pos.z);
								WaterTile1.transform.localScale = new Vector3 (-StartScale.x * script.LargeWaterPlaneScale, StartScale.y, StartScale.z * script.LargeWaterPlaneScale);
								WaterTile1.transform.localPosition = Start_pos1 + new Vector3 (0.002f, 0, 0);
						
								WaterTile1 = (GameObject)Instantiate (WaterTileLargePREFAB);
								WaterTile1.transform.parent = Water.transform;
								WaterTile1.transform.position = new Vector3 (Start_pos.x, Height + 3, Start_pos.z);
								WaterTile1.transform.localScale = new Vector3 (StartScale.x * script.LargeWaterPlaneScale, StartScale.y, -StartScale.z * script.LargeWaterPlaneScale);
								WaterTile1.transform.localPosition = Start_pos1 + new Vector3 (0, 0, 0.002f);
						
								WaterTile1 = (GameObject)Instantiate (WaterTileLargePREFAB);
								WaterTile1.transform.parent = Water.transform;
								WaterTile1.transform.position = new Vector3 (Start_pos.x, Height + 3, Start_pos.z);
								WaterTile1.transform.localScale = new Vector3 (-StartScale.x * script.LargeWaterPlaneScale, StartScale.y, -StartScale.z * script.LargeWaterPlaneScale);
								WaterTile1.transform.localPosition = Start_pos1 + new Vector3 (0.002f, 0, 0.002f);
						
							} else {
								//add tiles
								for (int i = 0; i < Water_tiles.x; i++) {
									for (int j = 0; j < Water_tiles.y; j++) {
										GameObject WaterTile = (GameObject)Instantiate (WaterTilePREFAB);
										WaterTile.transform.parent = Water.transform;
										WaterTile.transform.position = new Vector3 (Start_pos.x + (i * tileSize) + tileSize / 2, Height + 3, Start_pos.z + (j * tileSize) + tileSize / 2);
									}
								}
							}


						}
						EditorGUILayout.EndHorizontal ();

						//v3.3e
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();


						if (script.SkyManager.water != null) {

							EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (new GUIContent ("Remove water colliders"), GUILayout.Width (200))) {
								script.WaterManager.DisableColliders ();
							}
							if (GUILayout.Button (new GUIContent ("Restore water colliders"), GUILayout.Width (200))) {
								script.WaterManager.EnableColliders ();
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (new GUIContent ("Add Height Sampler"), GUILayout.Width (150))) {
								Vector3 MapcenterPos = script.SkyManager.MapCenter.position;
								GameObject WaterSampler = (GameObject)Instantiate (WaterSamplerPREFAB, MapcenterPos + new Vector3 (0, 3, 20), Quaternion.identity); 
								script.SkyManager.water.gameObject.AddComponent<WaterHeightSM> ().SampleCube = WaterSampler.transform;
								script.SkyManager.water.gameObject.GetComponent<WaterHeightSM> ().Waterhandler = script.WaterManager;
								script.SkyManager.water.gameObject.GetComponent<WaterHeightSM> ().WaterMaterial = script.WaterManager.oceanMat;
								script.SkyManager.water.gameObject.GetComponent<WaterHeightSM> ().LerpMotion = true;

								//v3.2
								if (script.SkyManager.water.GetComponent<WaterHandlerSM> () != null) {
									script.SkyManager.water.GetComponent<WaterHandlerSM> ().heightController = script.SkyManager.water.GetComponent<WaterHeightSM> ();
								}
							}
							EditorGUILayout.EndHorizontal ();

							//parent camera
							//WaterHeightSM WaterHeightHandle = script.SkyManager.water.gameObject.GetComponent<WaterHeightSM>();
							if (script.WaterHeightHandle == null) {
								script.WaterHeightHandle = script.SkyManager.water.gameObject.GetComponent<WaterHeightSM> ();
							}

							if (script.WaterHeightHandle != null && script.WaterHeightHandle.SampleCube != null) {
								EditorGUILayout.BeginHorizontal ();
								if (GUILayout.Button (new GUIContent ("Board boat"), GUILayout.Width (120))) {
									if (Camera.main != null) {
										Camera.main.transform.parent = script.WaterHeightHandle.SampleCube;
										Camera.main.transform.localPosition = new Vector3 (-0.5f, 1.65f, -2.4f);
										script.WaterHeightHandle.controlBoat = true;
										script.WaterHeightHandle.LerpMotion = true;
									}
								}


								if (GUILayout.Button (new GUIContent ("Enable thrower"), GUILayout.Width (120))) {
									script.WaterHeightHandle.ThrowItem = script.WaterHeightHandle.SampleCube.Find ("Sphere").gameObject;
								}
								EditorGUILayout.EndHorizontal ();


							}

							//EditorGUILayout.EndHorizontal(); 
							if (script.WaterHeightHandle != null && script.WaterHeightHandle.SampleCube != null) {
								EditorGUILayout.BeginHorizontal ();
								script.WaterHeightHandle.followCamera = EditorGUILayout.Toggle ("Water follows player", script.WaterHeightHandle.followCamera, GUILayout.MaxWidth (280.0f));
								EditorGUILayout.EndHorizontal ();
							}


							//v3.3e
							EditorGUILayout.EndVertical ();
							EditorGUILayout.Space ();
							EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

							//SHORE LINE
							GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));
							EditorGUILayout.HelpBox ("Shore line configuration", MessageType.None);

							// Check "TerrainSM" layer exists 
							if (LayerMask.NameToLayer ("TerrainSM") == -1) {
								//Debug.Log(LayerMask.NameToLayer("TerrainSM"));
								EditorGUILayout.HelpBox ("Please add the layer 'TerrainSM' to the layer list & to all Terrains that will be depth tested for use in shore line effect", MessageType.None);
							} else {

								if (MiddleTerrain != null) {
									if ((MiddleTerrain as GameObject).layer == LayerMask.NameToLayer ("TerrainSM")) {

										EditorGUILayout.BeginHorizontal ();
										GUILayout.Label ("Terrain size", GUILayout.Width (120));
										script.TerrainDepthSize = EditorGUILayout.FloatField (script.TerrainDepthSize, GUILayout.MaxWidth (95.0f));
										EditorGUILayout.EndHorizontal ();

										EditorGUILayout.BeginHorizontal ();
										if (GUILayout.Button (new GUIContent ("Auto calculate middle terrain size (single terrain)"), GUILayout.Width (320))) {
											Terrain TempTerrain = (MiddleTerrain as GameObject).GetComponent<Terrain> ();
											if (TempTerrain != null) {
												script.TerrainDepthSize = Mathf.Max (TempTerrain.terrainData.size.x, TempTerrain.terrainData.size.z);
											}
										}

										EditorGUILayout.EndHorizontal ();
										EditorGUILayout.BeginHorizontal ();
										if (GUILayout.Button (new GUIContent ("Add Shore Line effect"), GUILayout.Width (220))) {
											if (script.TerrainDepthSize > 0 && script.WaterManager != null) {
												Debug.Log ("Adding terrain depth render camera & script");
												float HalfSize = script.TerrainDepthSize / 2;
												//GameObject DepthSampler = (GameObject)Instantiate(DepthCameraPREFAB, (MiddleTerrain as GameObject).transform.position + new Vector3(HalfSize,HalfSize/0.57735026918f,HalfSize),Quaternion.identity); 
												GameObject DepthSampler = (GameObject)Instantiate (DepthCameraPREFAB); 
												DepthSampler.transform.position = (MiddleTerrain as GameObject).transform.position + new Vector3 (HalfSize, HalfSize / 0.57735026918f, HalfSize);
												Debug.Log ("Adding depth camera to WaterHandler");
												script.WaterManager.TerrainDepthCamera = DepthSampler.GetComponent<Camera> ().transform;
												//DepthSampler.GetComponent<Camera>().cullingMask = LayerMask.NameToLayer("TerrainSM");
												DepthSampler.GetComponent<Camera> ().cullingMask = (1 << LayerMask.NameToLayer ("TerrainSM"));

												DepthSampler.GetComponent<Camera> ().farClipPlane = DepthSampler.GetComponent<Camera> ().transform.position.y * 2;

												script.WaterManager.DepthRenderController = DepthSampler.GetComponent<TerrainDepthSM> ();
												script.WaterManager.TerrainScale = script.TerrainDepthSize;
											} else {
												Debug.Log ("Please define the size of the terrain(s) to be depth tested");
											}
										}
										EditorGUILayout.EndHorizontal ();

									} else {
										EditorGUILayout.BeginHorizontal ();
										EditorGUILayout.HelpBox ("Please add the tag 'TerrainSM' to all Terrains that will be depth tested for use in shore line effect", MessageType.None);
										EditorGUILayout.EndHorizontal ();
										//EditorGUILayout.EndHorizontal();
										//GUILayout.Box("",GUILayout.Height(3),GUILayout.Width(410));

										EditorGUILayout.BeginHorizontal ();
										if (GUILayout.Button (new GUIContent ("Auto assign layer (single terrain)"), GUILayout.Width (320))) {
											(MiddleTerrain as GameObject).layer = LayerMask.NameToLayer ("TerrainSM");

											//v3.0.4a - fix sun light to include TerrainSM layer
											script.SkyManager.SUN_LIGHT.GetComponent<Light> ().cullingMask = ~(0);
										}
										EditorGUILayout.EndHorizontal ();
									}
								} else {
									EditorGUILayout.BeginHorizontal ();
									EditorGUILayout.HelpBox ("Please add the middle Terrain to apply the shore line to", MessageType.None);
									EditorGUILayout.EndHorizontal ();
								}

								EditorGUILayout.BeginHorizontal ();
								MiddleTerrain = EditorGUILayout.ObjectField (MiddleTerrain, typeof(GameObject), true, GUILayout.MaxWidth (150.0f)) as GameObject;
								EditorGUILayout.EndHorizontal ();
							}

							if (script.WaterManager != null && script.WaterManager.TerrainDepthCamera != null) {
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.followCamera = EditorGUILayout.Toggle ("Depth camera follows player", script.WaterManager.followCamera, GUILayout.MaxWidth (280.0f));
								EditorGUILayout.EndHorizontal ();
							}


							if (script.WaterManager != null && script.WaterManager.DepthRenderController != null) {
								//add depth render controls

								EditorGUILayout.HelpBox ("Shore waves fade", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.ShoreWavesFade = EditorGUILayout.Slider (script.WaterManager.ShoreWavesFade, 0, 20f, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Depth camera clipping distance", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.TerrainDepthCamera.GetComponent<Camera> ().farClipPlane = EditorGUILayout.Slider (script.WaterManager.TerrainDepthCamera.GetComponent<Camera> ().farClipPlane, 0, 12000f, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Depth camera Field of View", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.TerrainDepthCamera.GetComponent<Camera> ().fieldOfView = EditorGUILayout.Slider (script.WaterManager.TerrainDepthCamera.GetComponent<Camera> ().fieldOfView, 10, 170f, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();

								EditorGUILayout.HelpBox ("Depth cutoff", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.DepthRenderController.heightCutoff = EditorGUILayout.Slider (script.WaterManager.DepthRenderController.heightCutoff, 0, 1.1f, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Height factor", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.DepthRenderController.heightFactor = EditorGUILayout.Slider (script.WaterManager.DepthRenderController.heightFactor, 0, 15f, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Contrast factor", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.DepthRenderController.contrast = EditorGUILayout.Slider (script.WaterManager.DepthRenderController.contrast, 0.01f, 1.15f, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();

								//preview terrain depth texture render
								script.PreviewDepthTexture = EditorGUILayout.Toggle ("Preview depth texture", script.PreviewDepthTexture, GUILayout.MaxWidth (180.0f));
								if (script.PreviewDepthTexture) {
									GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));
									EditorGUILayout.BeginHorizontal ();
									GUILayout.Label (script.WaterManager.TerrainDepthCamera.transform.GetComponent<Camera> ().targetTexture, GUILayout.MaxWidth (410.0f), GUILayout.MaxHeight (410.0f));
									EditorGUILayout.EndHorizontal ();
									GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	
								}
							}

							//v3.3e
							EditorGUILayout.EndVertical ();
							EditorGUILayout.Space ();


							// add middle terrain and terrain size - make sure it has the layer assigned
							// instantiate (in terrain center and height = terrain_width/tan(30) for the 60 field of view 
							// adjust DepthCameraPREFAB with terrain layer
							// add camera in waterhandler
							//			GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));

							//v3.4 - Reflection controls
							EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

							if (script.SkyManager.water.gameObject.GetComponent<PlanarReflectionSM> () != null) {
								EditorGUILayout.HelpBox ("Water Reflections downsample", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								PlanarReflectionSM ReflectScript = script.SkyManager.water.gameObject.GetComponent<PlanarReflectionSM> ();
								if (!Application.isPlaying) {
									ReflectScript.Downscale = EditorGUILayout.Slider (ReflectScript.Downscale, 0.1f, 1, GUILayout.MaxWidth (sliderWidth));
								}
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Water Reflections clip plane", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								ReflectScript.clipPlaneOffset = EditorGUILayout.Slider(ReflectScript.clipPlaneOffset, 0.05f, 40, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();

								EditorGUILayout.HelpBox ("Reflect specific layers", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								//ReflectScript.reflectionMask = EditorGUILayout.MaskField(ReflectScript.reflectionMask, GUILayout.MaxWidth (195.0f));
							//	ReflectScript.reflectionMask.value = EditorGUILayout.LayerField(ReflectScript.reflectionMask.value, GUILayout.MaxWidth (195.0f));
							//	ReflectScript.reflectionMask = EditorGUILayout.EnumMaskPopup(new GUIContent("aaa"), ReflectScript.reflectionMask, GUILayout.MaxWidth (195.0f));
								//string[] A1 = new string[3];
								//A1[0] = "aa";
								//A1[1] = "aa1";
								//A1[2] = "aa2";
								var layers = InternalEditorUtility.layers;
								ReflectScript.reflectionMask = EditorGUILayout.MaskField(ReflectScript.reflectionMask.value,layers,GUILayout.MaxWidth (195.0f));
								EditorGUILayout.EndHorizontal ();
							}

							EditorGUILayout.EndVertical ();
							EditorGUILayout.Space ();

						}

						//UnityTerrain =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
						//EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

						if (script.WaterManager != null) {

							EditorGUILayout.HelpBox ("Waves direction", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.WaveDir1Offset.y = EditorGUILayout.Slider (script.WaterManager.WaveDir1Offset.y, -1f, 1, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Waves direction", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.WaveDir1Offset.z = EditorGUILayout.Slider (script.WaterManager.WaveDir1Offset.z, -0.5f, 0.5f, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Extra waves factor X", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.ExtraWavesFactor.x = EditorGUILayout.Slider (script.WaterManager.ExtraWavesFactor.x, -10f, 10, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Extra waves factor Y", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.ExtraWavesFactor.y = EditorGUILayout.Slider (script.WaterManager.ExtraWavesFactor.y, -10f, 10, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Extra waves direction", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.ExtraWavesDirFactor.y = EditorGUILayout.Slider (script.WaterManager.ExtraWavesDirFactor.y, -10f, 10, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							///////

							EditorGUILayout.HelpBox ("Foam", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.FoamOffset = EditorGUILayout.Slider (script.WaterManager.FoamOffset, -10f, 10, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Foam cutoff", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.FoamCutoff = EditorGUILayout.Slider (script.WaterManager.FoamCutoff, -10f, 10, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							////

							EditorGUILayout.HelpBox ("Shore blend offset", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.ShoreBlendOffset = EditorGUILayout.Slider (script.WaterManager.ShoreBlendOffset, -0.2f, 2, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("Depth Color Offset", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.DepthColorOffset = EditorGUILayout.Slider (script.WaterManager.DepthColorOffset, -140f, 12, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("BumpFocusOffset", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.BumpFocusOffset = EditorGUILayout.Slider (script.WaterManager.BumpFocusOffset, -5f, 5, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("FresnelOffset", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.FresnelOffset = EditorGUILayout.Slider (script.WaterManager.FresnelOffset, -2f, 3, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("FresnelBias", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.FresnelBias = EditorGUILayout.Slider (script.WaterManager.FresnelBias, -140f, 240, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("Water height offset", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.waterScaleOffset.y = EditorGUILayout.Slider (script.WaterManager.waterScaleOffset.y, 0.1f, 20, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							///

							//v3.3e
							EditorGUILayout.EndVertical ();
							EditorGUILayout.Space ();
							EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

							//v3.4.3 - fog color strength
							script.WaterManager.UseSkyGradient = EditorGUILayout.Toggle ("Gradient on Water", script.WaterManager.UseSkyGradient, GUILayout.MaxWidth (380.0f));

							EditorGUILayout.HelpBox ("Underwater Unity Fog color", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.Fog_Color = EditorGUILayout.ColorField (script.WaterManager.Fog_Color, GUILayout.MaxWidth (195.0f));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Underwater Water Fog color", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.underwaterFogColorShader = EditorGUILayout.ColorField (script.WaterManager.underwaterFogColorShader, GUILayout.MaxWidth (195.0f));
							EditorGUILayout.EndHorizontal ();

							if (!script.WaterManager.UseSkyGradient) {
								EditorGUILayout.HelpBox ("Water Fog color Day", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.fogColorShaderDay = EditorGUILayout.ColorField (script.WaterManager.fogColorShaderDay, GUILayout.MaxWidth (195.0f));
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Water Fog color Dusk", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.fogColorShaderDusk = EditorGUILayout.ColorField (script.WaterManager.fogColorShaderDusk, GUILayout.MaxWidth (195.0f));
								EditorGUILayout.EndHorizontal ();
							} else {
								EditorGUILayout.HelpBox ("Water Fog Color Brightness", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.autoColorfogBrightness = EditorGUILayout.Slider (script.WaterManager.autoColorfogBrightness, 0, 2, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
							}

							//GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	
							EditorGUILayout.EndVertical ();
							EditorGUILayout.Space ();
							EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e


							EditorGUILayout.HelpBox ("Underwater Unity Fog Density", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.underwaterFogDensity = EditorGUILayout.Slider (script.WaterManager.underwaterFogDensity, 0, 1, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Underwater Water Fog Density", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.underwaterfogDensity = EditorGUILayout.Slider (script.WaterManager.underwaterfogDensity, 0, 1, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Water Fog Density", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.fogDensity = EditorGUILayout.Slider (script.WaterManager.fogDensity, 0, 5, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Water Fog Bias", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.fogBias = EditorGUILayout.Slider (script.WaterManager.fogBias, 0, 200, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Water Fog Start Distance", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.fogStartDistance = EditorGUILayout.Slider (script.WaterManager.fogStartDistance, 0, 500, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.HelpBox ("Underwater Water Volume Density", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.volumeFogDensityUnderwater = EditorGUILayout.Slider (script.WaterManager.volumeFogDensityUnderwater, 0, 500, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();


							script.WaterManager.OverrideReflectColor = EditorGUILayout.Toggle ("Override reflection color", script.WaterManager.OverrideReflectColor, GUILayout.MaxWidth (180.0f));

							EditorGUILayout.HelpBox ("Reflection color", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.ReflectColor = EditorGUILayout.ColorField (script.WaterManager.ReflectColor, GUILayout.MaxWidth (195.0f));

                            script.WaterManager.UpdateWaterParams(false);//v4.8.6

                            EditorGUILayout.EndHorizontal ();

							//v3.3e
							if (script.WaterManager.UseSkyGradient) {
								EditorGUILayout.HelpBox ("Reflection Intensity", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.GradReflIntensity = EditorGUILayout.Slider (script.WaterManager.GradReflIntensity, 0, 1, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Sky Reflect Color Power", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.SkyManager.FogWaterPow = EditorGUILayout.Slider (script.SkyManager.FogWaterPow, -10, 20, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
								EditorGUILayout.HelpBox ("Water transparency", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.WaterManager.GradTransp = EditorGUILayout.Slider (script.WaterManager.GradTransp, 0, 1, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
							}

							///

							EditorGUILayout.HelpBox ("Caustic intensity", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.CausticIntensity = EditorGUILayout.Slider (script.WaterManager.CausticIntensity, 0, 200, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("Caustic size", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.CausticSize = EditorGUILayout.Slider (script.WaterManager.CausticSize, 0, 500, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.HelpBox ("Shaft size", MessageType.None);
							EditorGUILayout.BeginHorizontal ();
							script.WaterManager.SunShaftsInt = EditorGUILayout.Slider (script.WaterManager.SunShaftsInt, 0, 500, GUILayout.MaxWidth (sliderWidth));
							EditorGUILayout.EndHorizontal ();

							if (script.TerrainManager != null) {
								EditorGUILayout.HelpBox ("Shaft length", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.TerrainManager.AddShaftsSizeUnder = EditorGUILayout.Slider (script.TerrainManager.AddShaftsSizeUnder, 0, 50, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();

								EditorGUILayout.HelpBox ("Shafts intensity", MessageType.None);
								EditorGUILayout.BeginHorizontal ();
								script.TerrainManager.AddShaftsIntensityUnder = EditorGUILayout.Slider (script.TerrainManager.AddShaftsIntensityUnder, 0, 5, GUILayout.MaxWidth (sliderWidth));
								EditorGUILayout.EndHorizontal ();
							}
						}

						//v3.3e
						EditorGUILayout.EndVertical ();
						EditorGUILayout.Space ();


						if (script.WaterManager != null) {
							script.WaterManager.waterType = (Artngame.SKYMASTER.WaterHandlerSM.WaterPreset)EditorGUILayout.EnumPopup (script.WaterManager.waterType, GUILayout.Width (120));

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Caribbean Water", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptA, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.Caribbean;
							}
							EditorGUILayout.EndHorizontal ();
					
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Dark Ocean", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptB, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.DarkOcean;
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Emerald shores", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptC, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.Emerald;
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Focus Specular Ocean", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptD, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.FocusOcean;
							}
							EditorGUILayout.EndHorizontal ();
					
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Muddy Ocean", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptE, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.Muddy;
							}
							EditorGUILayout.EndHorizontal ();
					
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Ocean", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptF, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.Ocean;
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("River", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptG, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.River;
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Small Waves", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptH, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.SmallWaves;
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Lake", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptI, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.Lake;
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Atoll", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (WaterOptJ, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.waterType = WaterHandlerSM.WaterPreset.Atoll;
							}
							EditorGUILayout.EndHorizontal ();
						}
						if (script.WaterManager != null) {
							script.WaterManager.underWaterType = (Artngame.SKYMASTER.WaterHandlerSM.UnderWaterPreset)EditorGUILayout.EnumPopup (script.WaterManager.underWaterType, GUILayout.Width (120));

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Calm Underwater", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (UnderWaterOptA, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.underWaterType = WaterHandlerSM.UnderWaterPreset.Fancy;
							}
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.HelpBox ("Turbulent Underwater", MessageType.None);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							if (GUILayout.Button (UnderWaterOptB, GUILayout.MaxWidth (300.0f), GUILayout.MaxHeight (80.0f))) {
								script.WaterManager.underWaterType = WaterHandlerSM.UnderWaterPreset.Turbulent;
							}
							EditorGUILayout.EndHorizontal ();
						}

//				EditorGUILayout.BeginHorizontal();
//				if(GUILayout.Button(new GUIContent("Setup mesh terrain"),GUILayout.Width(120))){			
//					if(mesh_terrain != null){
//						
//					}else{
//						Debug.Log ("Add a mesh terrain first");
//					}					
//				}
//				mesh_terrain =  EditorGUILayout.ObjectField(null,typeof( GameObject ),true,GUILayout.MaxWidth(180.0f));
//				
//				
//				EditorGUILayout.EndHorizontal();				
						EditorGUIUtility.wideMode = false;
				
						EditorGUILayout.BeginHorizontal ();
				
						//ParticleScaleFactor = EditorGUILayout.FloatField(ParticleScaleFactor,GUILayout.MaxWidth(95.0f));
						//ParticlePlaybackScaleFactor = EditorGUILayout.FloatField(ParticlePlaybackScaleFactor,GUILayout.MaxWidth(95.0f));
						EditorGUILayout.EndHorizontal ();					
					}
					EditorGUILayout.EndVertical ();

				}//END TAB6
				///////////////////////////////////////////////////////////////////////////

			}//END CHECK SKYMANAGER EXISTS








			///////////////////////////////////////////////////////////////////////////////
	//		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(180.0f));			
			//X_offset_left = 200;
			//Y_offset_top = 100;		

			//TAB7
			if ((script.UseTabs && script.currentTab == 7) | !script.UseTabs) {

				GUILayout.Box ("", GUILayout.Height (3), GUILayout.Width (410));	

				GUI.backgroundColor = Color.blue * 0.2f;
				EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e
				GUI.backgroundColor = Color.white;

				GUILayout.Label (MainIcon8, GUILayout.MaxWidth (410.0f));
				EditorGUILayout.LabelField ("Particle tools & Special FX", EditorStyles.boldLabel);			
				EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
				GUILayout.Space (10);
				script.scaler_folder1 = EditorGUILayout.Foldout (script.scaler_folder1, "Scale size and speed");
				EditorGUILayout.EndHorizontal ();
			
				if (script.scaler_folder1) {

					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (new GUIContent ("Scale particles", "Scale particle systems"), GUILayout.Width (95))) {			
					
						if (Selection.activeGameObject != null) {
                            ScaleMe(ParticleScaleFactor, (GameObject)MainParticle, Exclude_children); //ScaleMe (ParticleScaleFactor, Selection.activeGameObject, Exclude_children); //v4.1d
                        } else {
							Debug.Log ("Please select a particle system to scale");
						}
					}
				
					if (GUILayout.Button (new GUIContent ("Scale speed", "Scale playback speed"), GUILayout.Width (95))) {
					
						if (Selection.activeGameObject != null) {
							ParticleSystem[] PSystems = Selection.activeGameObject.GetComponentsInChildren<ParticleSystem> (Include_inactive);
						
							Undo.RecordObjects (PSystems, "speed scale");
						
							if (PSystems != null) {
								if (PSystems.Length > 0) {
									for (int i = 0; i < PSystems.Length; i++) {
										ParticleSystem.MainModule MainMod = PSystems [i].main; //v3.4.9
										//PSystems [i].playbackSpeed = ParticlePlaybackScaleFactor;	
										MainMod.simulationSpeed = ParticlePlaybackScaleFactor; //v3.4.9
									}
								}
							}
						} else {
							Debug.Log ("Please select a particle system to scale");
						}
					}
					EditorGUILayout.EndHorizontal ();				
					EditorGUIUtility.wideMode = false;

					EditorGUILayout.BeginHorizontal ();
				
					ParticleScaleFactor = EditorGUILayout.FloatField (ParticleScaleFactor, GUILayout.MaxWidth (95.0f));
					ParticlePlaybackScaleFactor = EditorGUILayout.FloatField (ParticlePlaybackScaleFactor, GUILayout.MaxWidth (95.0f));
					EditorGUILayout.EndHorizontal ();

                    MainParticle = EditorGUILayout.ObjectField(MainParticle, typeof(GameObject), true, GUILayout.MaxWidth(180.0f)); //v4.1d
                    //MainParticle = EditorGUILayout.ObjectField (null, typeof(GameObject), true, GUILayout.MaxWidth (180.0f));

                    Exclude_children = EditorGUILayout.Toggle ("Exclude children", Exclude_children, GUILayout.MaxWidth (180.0f));
					Include_inactive = EditorGUILayout.Toggle ("Include inactive", Include_inactive, GUILayout.MaxWidth (180.0f));
					

					EditorGUILayout.EndVertical ();
					EditorGUILayout.Space ();
				}


				//SPECIAL FX
				EditorGUILayout.BeginHorizontal (GUILayout.Width (200));
				GUILayout.Space (10);
				script.scaler_folder11 = EditorGUILayout.Foldout (script.scaler_folder11, "Special FX");
				EditorGUILayout.EndHorizontal ();
			
				if (script.scaler_folder11) {

					EditorGUILayout.BeginVertical ("box", GUILayout.MaxWidth (415.0f)); //v3.3e

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("Atomic Bomb", MessageType.None);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (SpecialFXOptA, GUILayout.MaxWidth (260.0f), GUILayout.MaxHeight (80.0f))) {
						GameObject ABOMB = Instantiate (ABOMB_Prefab) as GameObject;
						ABOMB.name = "Atomic Bomb Effect";
						if (Camera.main != null) {
							ABOMB.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 50;
						}
						if (script.SkyManager != null) {
							for (int i = 0; i < ABOMB.GetComponentsInChildren<VolumeParticleShadePDM> (true).Length; i++) {
								ABOMB.GetComponentsInChildren<VolumeParticleShadePDM> (true) [i].Sun = script.SkyManager.SUN_LIGHT.GetComponent<Light> ();
							}
						}
					}
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("Asteroid field", MessageType.None);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (SpecialFXOptB, GUILayout.MaxWidth (260.0f), GUILayout.MaxHeight (80.0f))) {
						GameObject ABOMB1 = Instantiate (ASTEROIDS_Prefab) as GameObject;
						ABOMB1.name = "Asteroid field";
						if (Camera.main != null) {
							ABOMB1.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 50 + new Vector3 (-2000, 2500, 7200);
						}
					}
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("Freeze effect - Ice decals", MessageType.None);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (SpecialFXOptC, GUILayout.MaxWidth (260.0f), GUILayout.MaxHeight (80.0f))) {
						GameObject ABOMB2 = Instantiate (FREEZE_EFFECT_Prefab) as GameObject;
						ABOMB2.name = "Freeze & Ice decals";
						if (Camera.main != null) {
							ABOMB2.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0 + new Vector3 (5, 2, 10);
						}
					}
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("Aurora", MessageType.None);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (SpecialFXOptD, GUILayout.MaxWidth (260.0f), GUILayout.MaxHeight (80.0f))) {
						GameObject ABOMB3 = Instantiate (AURORA_Prefab) as GameObject;
						ABOMB3.name = "Aurora effect";
						if (Camera.main != null) {
							ABOMB3.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 50 + new Vector3 (-95, 70, 130);
						}
					}
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("Chain Lightning", MessageType.None);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (SpecialFXOptE, GUILayout.MaxWidth (260.0f), GUILayout.MaxHeight (80.0f))) {
						GameObject ABOMB4 = Instantiate (CHAIN_LIGHTNING_Prefab) as GameObject;
						ABOMB4.name = "Chain Lightning";
						if (Camera.main != null) {
							ABOMB4.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1 + new Vector3 (0, 0, 10);
						}
					}
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("Sand storm", MessageType.None);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (SpecialFXOptF, GUILayout.MaxWidth (260.0f), GUILayout.MaxHeight (80.0f))) {
						GameObject ABOMB5 = Instantiate (SAND_STORM_Prefab) as GameObject;
						ABOMB5.name = "Sand storm";
						if (Camera.main != null) {
							ABOMB5.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 550;
						}
					}
					EditorGUILayout.EndHorizontal ();

					//v3.4.3
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("Volcano", MessageType.None);
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button (SpecialFXOptG, GUILayout.MaxWidth (260.0f), GUILayout.MaxHeight (80.0f))) {
						GameObject ABOMB6 = Instantiate (VOLCANO_Prefab) as GameObject;
						ABOMB6.name = "Volcano";
						ABOMB6.GetComponent<VolumeParticleShadePDM> ().Sun = script.SkyManager.SUN_LIGHT.GetComponent<Light> ();
						if (Camera.main != null) {
							ABOMB6.transform.position = Camera.main.transform.position + new Vector3(Camera.main.transform.forward.x,Camera.main.transform.position.y/550,Camera.main.transform.forward.z) * 550;
						}
					}
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.EndVertical ();
					EditorGUILayout.Space ();
				}

				EditorGUILayout.EndVertical ();
				///////////////////////////////////////////////////////////////////////////

			}//END TAB7


			serializedObject.ApplyModifiedProperties ();


//			Handles.color = Color.red;
//			Event cur = Event.current;
//			
//			if (cur.type == EventType.MouseDown && cur.button == 1) {
//				Ray ray = HandleUtility.GUIPointToWorldRay(cur.mousePosition);
//				
//				RaycastHit hit = new RaycastHit();
//				if (Physics.Raycast(ray, out hit, Mathf.Infinity))					
//				{
//					//Handles.SphereCap(i,FIND_moved_pos,Quaternion.identity,script.Marker_size);
//				}
//			}

			//Repaint (); //v3.4.5
			//Undo.RecordObject (script,"undo");
			if(GUI.changed){//v3.4.5

				if (script.SkyManager.currentWeatherName == Artngame.SKYMASTER.SkyMasterManager.Volume_Weather_types.HeavyStorm || script.SkyManager.currentWeatherName == Artngame.SKYMASTER.SkyMasterManager.Volume_Weather_types.HeavyStormDark) {						
					if (!Application.isPlaying) {
						RenderSettings.ambientIntensity = script.SkyManager.AmbientIntensity/2;
					}
				} else {
					//m_fWaveLength = new Vector3 (TMP1.r, TMP1.g, TMP1.b);

					if (!Application.isPlaying) {
						DynamicGI.UpdateEnvironment (); //v3.4.5
						RenderSettings.ambientIntensity = script.SkyManager.AmbientIntensity;
					}
				}

				EditorUtility.SetDirty (script);//v3.4.5


			}
			//if (script.SkyManager != null) {
			//	EditorUtility.SetDirty (script.SkyManager);
			//}
		}


		private void SceneGUI(SceneView sceneview)
		{
			
		}
		


		void ScaleMe(float ParticleScaleFactor, GameObject ParticleHolder, bool Exclude_children){
			
			if(1==1)
			{
				//GameObject ParticleHolder = Selection.activeGameObject;
				//scale parent object
				
				if(Exclude_children){
					
					//v3.3
					if (ParticleHolder != null) {
					
						ParticleSystem ParticleParent = ParticleHolder.GetComponent (typeof(ParticleSystem)) as ParticleSystem;
					
						if (ParticleParent != null) {
							Object[] ToUndo = new Object[2];
							ToUndo [0] = ParticleParent as Object;
							ToUndo [1] = Selection.activeGameObject.transform as Object;
						
							Undo.RecordObjects (ToUndo, "scale");

							if (!script.DontScaleParticleTranf) {
								ParticleHolder.transform.localScale = ParticleHolder.transform.localScale * ParticleScaleFactor;
							}
						}
					
						if (ParticleParent != null && !script.DontScaleParticleProps) {
						
							ParticleSystem.MainModule MainMod = ParticleParent.main; //v3.4.9 
							ParticleSystem.MinMaxCurve startSize = MainMod.startSize;
							ParticleSystem.MinMaxCurve startSpeed = MainMod.startSpeed;
							//ParticleParent.startSize = ParticleParent.startSize * ParticleScaleFactor;						
							//ParticleParent.startSpeed = ParticleParent.startSpeed * ParticleScaleFactor;	
							startSize.constant = startSize.constant * ParticleScaleFactor;						
							startSpeed.constant = startSpeed.constant * ParticleScaleFactor;	
						
							SerializedObject SerializedParticle = new SerializedObject (ParticleParent);				
						
							if (SerializedParticle.FindProperty ("VelocityModule.enabled").boolValue) {
								SerializedParticle.FindProperty ("VelocityModule.x.scalar").floatValue *= ParticleScaleFactor;
								SerializedParticle.FindProperty ("VelocityModule.y.scalar").floatValue *= ParticleScaleFactor;
								SerializedParticle.FindProperty ("VelocityModule.z.scalar").floatValue *= ParticleScaleFactor;
							
								Scale_inner (SerializedParticle.FindProperty ("VelocityModule.x.minCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("VelocityModule.x.maxCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("VelocityModule.y.minCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("VelocityModule.y.maxCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("VelocityModule.z.minCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("VelocityModule.z.maxCurve").animationCurveValue);
							}
						
							if (SerializedParticle.FindProperty ("ForceModule.enabled").boolValue) {
								SerializedParticle.FindProperty ("ForceModule.x.scalar").floatValue *= ParticleScaleFactor;
								SerializedParticle.FindProperty ("ForceModule.y.scalar").floatValue *= ParticleScaleFactor;
								SerializedParticle.FindProperty ("ForceModule.z.scalar").floatValue *= ParticleScaleFactor;
							
								Scale_inner (SerializedParticle.FindProperty ("ForceModule.x.minCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("ForceModule.x.maxCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("ForceModule.y.minCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("ForceModule.y.maxCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("ForceModule.z.minCurve").animationCurveValue);
								Scale_inner (SerializedParticle.FindProperty ("ForceModule.z.maxCurve").animationCurveValue);
							}
						
							SerializedParticle.ApplyModifiedProperties ();
						}
					}
				}
				
				if(!Exclude_children){

					///v3.3
					if (ParticleHolder != null) {
						ParticleSystem[] ParticleParents = ParticleHolder.GetComponentsInChildren<ParticleSystem> (true);
					
						if (ParticleParents != null) {
							Object[] ParticleParentsOBJ = new Object[ParticleParents.Length + 1];
							for (int i = 0; i < ParticleParents.Length; i++) {
								ParticleParentsOBJ [i] = ParticleParents [i] as Object;
							}
							ParticleParentsOBJ [ParticleParentsOBJ.Length - 1] = Selection.activeGameObject.transform as Object;
						
							Undo.RecordObjects (ParticleParentsOBJ, "scale");

							if (!script.DontScaleParticleTranf) {
								ParticleHolder.transform.localScale = ParticleHolder.transform.localScale * ParticleScaleFactor;
							}
						}
					

						if(!script.DontScaleParticleProps){
							foreach(ParticleSystem ParticlesA in ParticleHolder.GetComponentsInChildren<ParticleSystem>(true))
							{

								ParticleSystem.MainModule MainMod = ParticlesA.main; //v3.4.9
								ParticleSystem.MinMaxCurve startSize = MainMod.startSize;
								ParticleSystem.MinMaxCurve startSpeed = MainMod.startSpeed;

								//ParticlesA.startSize = ParticlesA.startSize * ParticleScaleFactor;								
								//ParticlesA.startSpeed = ParticlesA.startSpeed * ParticleScaleFactor;	
								startSize.constant = startSize.constant * ParticleScaleFactor;								
								startSpeed.constant = startSpeed.constant * ParticleScaleFactor;	
								
								SerializedObject SerializedParticle = new SerializedObject(ParticlesA);
								
								if(SerializedParticle.FindProperty("VelocityModule.enabled").boolValue)
								{
									SerializedParticle.FindProperty("VelocityModule.x.scalar").floatValue *= ParticleScaleFactor;
									SerializedParticle.FindProperty("VelocityModule.y.scalar").floatValue *= ParticleScaleFactor;
									SerializedParticle.FindProperty("VelocityModule.z.scalar").floatValue *= ParticleScaleFactor;
									
									Scale_inner(SerializedParticle.FindProperty("VelocityModule.x.minCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("VelocityModule.x.maxCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("VelocityModule.y.minCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("VelocityModule.y.maxCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("VelocityModule.z.minCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("VelocityModule.z.maxCurve").animationCurveValue);
								}
								
								if(SerializedParticle.FindProperty("ForceModule.enabled").boolValue)
								{
									SerializedParticle.FindProperty("ForceModule.x.scalar").floatValue *= ParticleScaleFactor;
									SerializedParticle.FindProperty("ForceModule.y.scalar").floatValue *= ParticleScaleFactor;
									SerializedParticle.FindProperty("ForceModule.z.scalar").floatValue *= ParticleScaleFactor;
									
									Scale_inner(SerializedParticle.FindProperty("ForceModule.x.minCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("ForceModule.x.maxCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("ForceModule.y.minCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("ForceModule.y.maxCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("ForceModule.z.minCurve").animationCurveValue);
									Scale_inner(SerializedParticle.FindProperty("ForceModule.z.maxCurve").animationCurveValue);
								}
								
								SerializedParticle.ApplyModifiedProperties();
							}	
						}
					}//end check if holder is null
				}
			}
		}

		void Scale_inner(AnimationCurve AnimCurve){
			
			for(int i = 0; i < AnimCurve.keys.Length; i++)
			{
				AnimCurve.keys[i].value = AnimCurve.keys[i].value * ParticleScaleFactor;
			}
		}

        private void MoveComponentToTop(GameObject GameObj)//(MenuCommand menuCommand)
        {
            //Component c = (Component)menuCommand.context;
            //Component[] allComponents = GameObj.GetComponents<Component>();
            //int iOffset = 0;
            //for (int i = 0; i < allComponents.Length; i++)
            //{
            //    if (allComponents[i] is GlobalFogSkyMaster) //if(allComponents[i] == c)
            //    {
            //        iOffset = i - 4;
            //        for (int j = 0; j < iOffset - 1; j++)
            //        {
            //            UnityEditorInternal.ComponentUtility.MoveComponentUp(allComponents[i]);
            //        }
            //        break;
            //    }
            //}

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); //EditorApplication.MarkSceneDirty(); //v4.1e
        }
        //[MenuItem("CONTEXT/Component/Move To Bottom")]
        private void MoveComponentToBottom(GameObject GameObj)//(MenuCommand menuCommand)
        {
            //Component c = (Component)menuCommand.context;
            //Component[] allComponents = GameObj.GetComponents<Component>();
            //int iOffset = 0;
            //for (int i = 0; i < allComponents.Length; i++)
            //{
            //    if (allComponents[i] is GlobalFogSkyMaster) //if (allComponents[i] == c)
            //    {
            //        iOffset = i;
            //        for (; iOffset < allComponents.Length; iOffset++)
            //        {
            //            UnityEditorInternal.ComponentUtility.MoveComponentDown(allComponents[i]);
            //        }
            //        break;
            //    }
            //}

            //EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); //EditorApplication.MarkSceneDirty(); //v4.1e
        }

    }
}