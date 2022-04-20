using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Artngame.SKYMASTER{

	[ExecuteInEditMode]
public class SeasonalTerrainSKYMASTER : MonoBehaviour 
{

        //v3.4
        [Tooltip("For use with curve based system")]
        public float VFogDistance = 21;//200;//if DistanceFogOn //v4.8
        public bool DistanceFogOn = false;//true; //v4.8
		public bool HeightFogOn = true;
		public bool SkyFogOn = false;
        public float fogDensity = 0.3f;// 0.9f;
        public float fogGradientDistance = 1;//3500;//offset gradient extend to horizon

		//v3.3e
		public bool UseFogCurves = false;//use curves to override TOD definition for certain volume fog parameters
		[SerializeField]
		public AnimationCurve heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.95f,1),new Keyframe(1,1));
		[SerializeField]
        public AnimationCurve luminanceVFogCurve = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(0.75f, 0.5f), new Keyframe(0.85f, 0.5f), new Keyframe(0.9f, 0.5f), new Keyframe(1, 0.5f));
        //public AnimationCurve luminanceVFogCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(0.75f,0),new Keyframe(0.85f,0),new Keyframe(0.9f,0),new Keyframe(1,0));//-0.5 to 8
        [SerializeField]
		public AnimationCurve lumFactorFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//-1 to 15
		[SerializeField]
		public AnimationCurve scatterFacFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//-55 to 55
		[SerializeField]
		public AnimationCurve turbidityFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//0 to 100
		[SerializeField]
		public AnimationCurve turbFacFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//0 to 1500
		[SerializeField]
		public AnimationCurve horizonFogCurve= new AnimationCurve(new Keyframe(0,1),new Keyframe(1,1));//-55 to 55
		[SerializeField]
		public AnimationCurve contrastFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//1 to 200

		//v3.4.3
		public void setVFogCurvesPresetA(){
			heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.95f,1),new Keyframe(1,1));
			luminanceVFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//-0.5 to 8
			lumFactorFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//-1 to 15
			scatterFacFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//-55 to 55
			turbidityFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//0 to 100
			turbFacFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//0 to 1500
			horizonFogCurve= new AnimationCurve(new Keyframe(0,1),new Keyframe(1,1));//-55 to 55
			contrastFogCurve = new AnimationCurve(new Keyframe(0,1),new Keyframe(0.75f,1),new Keyframe(0.85f,1),new Keyframe(0.9f,1),new Keyframe(1,1));//1 to 200
			AddFogHeightOffset = 0;//100;
			fogDensity = 150;//30;
			DistanceFogOn = false;
			VFogDistance = 0;
		}
		public void setVFogCurvesPresetC(){
			//heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,55),new Keyframe(0.75f,45),new Keyframe(0.85f,180),new Keyframe(0.95f,54),new Keyframe(1,55));
			heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,55),new Keyframe(0.75f,45),new Keyframe(0.85f,60),new Keyframe(0.90f,130),new Keyframe(0.95f,54),new Keyframe(1,55));
			luminanceVFogCurve = new AnimationCurve(new Keyframe(0,2),new Keyframe(0.75f,2),new Keyframe(0.85f,9),new Keyframe(0.9f,2),new Keyframe(1,2));//-0.5 to 8
			lumFactorFogCurve = new AnimationCurve(new Keyframe(0,0.2f),new Keyframe(0.75f,0.2f),new Keyframe(0.85f,-0.1f),new Keyframe(0.9f,0.2f),new Keyframe(1,0.2f));//-1 to 15
			scatterFacFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,20),new Keyframe(0.85f,50f),new Keyframe(0.9f,20),new Keyframe(1,12));//-55 to 55
			turbidityFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,1),new Keyframe(0.85f,108f),new Keyframe(0.9f,5),new Keyframe(1,12));//0 to 100
			turbFacFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,12f),new Keyframe(0.85f,58f),new Keyframe(0.9f,4),new Keyframe(1,2));//0 to 1500
			horizonFogCurve= new AnimationCurve(new Keyframe(0,1),new Keyframe(1,1));//-55 to 55
			contrastFogCurve = new AnimationCurve(new Keyframe(0,6),new Keyframe(0.75f,1.5f),new Keyframe(0.85f,1.45f),new Keyframe(0.9f,1.5f),new Keyframe(1,6));//1 to 200
			//contrastFogCurve = new AnimationCurve(new Keyframe(0,6),new Keyframe(0.75f,1.5f),new Keyframe(0.85f,0.5f),new Keyframe(0.9f,1.5f),new Keyframe(1,6));//1 to 200
			AddFogHeightOffset = 0;
			fogDensity = 200;
			DistanceFogOn = false;
			VFogDistance = 0;
		}
		public void setVFogCurvesPresetD(){
			//heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,55),new Keyframe(0.75f,45),new Keyframe(0.85f,180),new Keyframe(0.95f,54),new Keyframe(1,55));
			heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,55),new Keyframe(0.75f,45),new Keyframe(0.85f,60),new Keyframe(0.90f,130),new Keyframe(0.95f,54),new Keyframe(1,55));
			luminanceVFogCurve = new AnimationCurve(new Keyframe(0,2),new Keyframe(0.75f,-0.6f),new Keyframe(0.85f,9),new Keyframe(0.9f,2),new Keyframe(1,2));//-0.5 to 8
			lumFactorFogCurve = new AnimationCurve(new Keyframe(0,0.2f),new Keyframe(0.75f,0.2f),new Keyframe(0.85f,-0.1f),new Keyframe(0.9f,0.2f),new Keyframe(1,0.2f));//-1 to 15
			scatterFacFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,20),new Keyframe(0.85f,50f),new Keyframe(0.9f,20),new Keyframe(1,12));//-55 to 55
			turbidityFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,1),new Keyframe(0.85f,108f),new Keyframe(0.9f,3),new Keyframe(1,12));//0 to 100
			turbFacFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,12f),new Keyframe(0.85f,58f),new Keyframe(0.9f,3),new Keyframe(1,2));//0 to 1500
			horizonFogCurve= new AnimationCurve(new Keyframe(0,1),new Keyframe(1,1));//-55 to 55
			contrastFogCurve = new AnimationCurve(new Keyframe(0,6),new Keyframe(0.75f,5.5f),new Keyframe(0.85f,3.45f),new Keyframe(0.9f,2.5f),new Keyframe(1,6));//1 to 200
			//contrastFogCurve = new AnimationCurve(new Keyframe(0,6),new Keyframe(0.75f,1.5f),new Keyframe(0.85f,0.5f),new Keyframe(0.9f,1.5f),new Keyframe(1,6));//1 to 200
			AddFogHeightOffset = 40;
			fogDensity = 150;
			DistanceFogOn = false;
			VFogDistance = 0;
		}
		public void setVFogCurvesPresetB(){
			//heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,55),new Keyframe(0.75f,45),new Keyframe(0.85f,180),new Keyframe(0.95f,54),new Keyframe(1,55));
			heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,55),new Keyframe(0.75f,45),new Keyframe(0.85f,60),new Keyframe(0.90f,130),new Keyframe(0.95f,54),new Keyframe(1,55));
			luminanceVFogCurve = new AnimationCurve(new Keyframe(0,2),new Keyframe(0.75f,2),new Keyframe(0.85f,9),new Keyframe(0.9f,2),new Keyframe(1,2));//-0.5 to 8
			lumFactorFogCurve = new AnimationCurve(new Keyframe(0,0.2f),new Keyframe(0.75f,0.2f),new Keyframe(0.85f,-0.25f),new Keyframe(0.9f,0.2f),new Keyframe(1,0.2f));//-1 to 15
			scatterFacFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,20),new Keyframe(0.85f,50f),new Keyframe(0.9f,20),new Keyframe(1,12));//-55 to 55
			turbidityFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,20),new Keyframe(0.85f,108f),new Keyframe(0.9f,20),new Keyframe(1,12));//0 to 100
			turbFacFogCurve = new AnimationCurve(new Keyframe(0,12),new Keyframe(0.75f,12f),new Keyframe(0.85f,58f),new Keyframe(0.9f,12),new Keyframe(1,2));//0 to 1500
			horizonFogCurve= new AnimationCurve(new Keyframe(0,1),new Keyframe(1,1));//-55 to 55
			contrastFogCurve = new AnimationCurve(new Keyframe(0,6),new Keyframe(0.75f,1.5f),new Keyframe(0.85f,1.45f),new Keyframe(0.9f,1.5f),new Keyframe(1,6));//1 to 200
			//contrastFogCurve = new AnimationCurve(new Keyframe(0,6),new Keyframe(0.75f,1.5f),new Keyframe(0.85f,0.5f),new Keyframe(0.9f,1.5f),new Keyframe(1,6));//1 to 200
			AddFogHeightOffset = -100;
			fogDensity = 200;
			DistanceFogOn = false;
			VFogDistance = 0;
		}

        //v4.8
        public void setVFogCurvesPresetE()
        {
            //heightOffsetFogCurve = new AnimationCurve(new Keyframe(0,55),new Keyframe(0.75f,45),new Keyframe(0.85f,180),new Keyframe(0.95f,54),new Keyframe(1,55));
            heightOffsetFogCurve = new AnimationCurve(new Keyframe(0, 55), new Keyframe(0.75f, 45), new Keyframe(0.85f, 60), new Keyframe(0.90f, 130), new Keyframe(0.95f, 54), new Keyframe(1, 55));
            luminanceVFogCurve = new AnimationCurve(new Keyframe(0, 2.3f), new Keyframe(0.75f, 2.3f), new Keyframe(0.85f, 2.3f), new Keyframe(0.9f, 2.3f), new Keyframe(1, 2.3f));//-0.5 to 8
            lumFactorFogCurve = new AnimationCurve(new Keyframe(0, 0.2f), new Keyframe(0.75f, 0.2f), new Keyframe(0.85f, 0.2f), new Keyframe(0.9f, 0.2f), new Keyframe(1, 0.2f));//-1 to 15
            scatterFacFogCurve = new AnimationCurve(new Keyframe(0, 2150), new Keyframe(0.75f, 2150), new Keyframe(0.85f, 2150), new Keyframe(0.9f, 2150), new Keyframe(1, 2150));//-55 to 55
            turbidityFogCurve = new AnimationCurve(new Keyframe(0, 12), new Keyframe(0.75f, 10), new Keyframe(0.85f, 10), new Keyframe(0.9f, 10), new Keyframe(1, 12));//0 to 100
            turbFacFogCurve = new AnimationCurve(new Keyframe(0, 1300), new Keyframe(0.75f, 1300), new Keyframe(0.85f, 1300), new Keyframe(0.9f, 1300), new Keyframe(1, 1300));//0 to 1500
            horizonFogCurve = new AnimationCurve(new Keyframe(0, 50), new Keyframe(1, 50));//-55 to 55
            contrastFogCurve = new AnimationCurve(new Keyframe(0, 1.4f), new Keyframe(0.75f, 1.5f), new Keyframe(0.85f, 1.45f), new Keyframe(0.9f, 1.5f), new Keyframe(1, 1.4f));//1 to 200
                                                                                                                                                                           //contrastFogCurve = new AnimationCurve(new Keyframe(0,6),new Keyframe(0.75f,1.5f),new Keyframe(0.85f,0.5f),new Keyframe(0.9f,1.5f),new Keyframe(1,6));//1 to 200
            AddFogHeightOffset = 0;
            fogDensity = 25;
            DistanceFogOn = false;
            VFogDistance = 0;
            ExposureBias = 1.5f;
            mieCoefficient = 0.9f;
            mieDirectionalG = 0.92f;
            reileigh = 1000;
        }

        //v3.3 - volume fog application speed
        public float VolumeFogSpeed = 1;

		//v3.0.3
		public bool tagBasedTreeGrab = false;//get terrain trees by their "SkyMasterTree" tag, no to be enable if tag not defined

		//transparent fog
		public bool UseTranspVFog = false;//use volume fog that affects clouds, it does not support looking at objects through clouds

		//v3.0a
		public float AddFogHeightOffset= 900;//0;//extra offset factor in the fog height //v4.8
		public float AddFogDensityOffset = 0;//extra density over the preset one
		//public float AddShaftsIntensity=0;//extra shaft intensity overwater
		public float AddShaftsIntensityUnder = 0;//extra shaft intensity underwater
		public float AddShaftsSizeUnder = 0;//extra shaft length underwater
        public float ShaftBlurRadiusOffset = 0;//900;//0; //v4.8

		//v3.0
		public bool Lerp_gradient = false;
		public bool FogHeightByTerrain = false;//ovveride preset fog height based on terrain height
		public float FogheightOffset = 0;
		public float FogdensityOffset = 0;
		//float FogUnityOffset = 0;

		//v2.2 - trees Unity terrain
		public List<GameObject> TreePefabs = new List<GameObject>();
		public Color currentTerrainTreeCol = Color.white;
		public bool AlwaysUpdateBillboards = false;//add small rotation to camera to trigger refresh
		//public Color prevTerrainTreeCol = Color.white;
		Transform Cam_transf;



	//v2.1
		//public List<GlobalFogSkyMaster> GradientHolders = new List<GlobalFogSkyMaster>();//if != null copy gradient from component GlobalFogSkyMaster
		public bool StereoMode = false; // cardboard setup left/right camera
		//GlobalFogSkyMaster SkyFogL;
		//SunShaftsSkyMaster SunShaftsL;
		//GlobalFogSkyMaster SkyFogR;
		//SunShaftsSkyMaster SunShaftsR;//check if null and fill if SteroMode is on
		public GameObject LeftCam;
		public GameObject RightCam;

		//GlobalTranspFogSkyMaster SkyFogLT;
		//GlobalTranspFogSkyMaster SkyFogRT;

	//v1.7
	//GlobalTranspFogSkyMaster SkyFogTransp;
	//GlobalFogSkyMaster SkyFog;
	//SunShaftsSkyMaster SunShafts;
	public bool ImageEffectFog=false;
		public bool Use_both_fogs = false;

	public bool ImageEffectShafts=false;
	//public bool ImageEffectSkyUpate=false; // get image effect volume fog parameters from SkymasterManager parameters.
	public int FogPreset = 0;
		public bool UpdateLeafMat = false;
		public List<Material> LeafMats;
		public Color Rays_day_color = new Color(236f/255f,49f/255f,20f/255f,255f/255f);
		public Color Rays_night_color = new Color(236f/255f,236f/255f,236f/255f,255f/255f);
		public float Shafts_intensity = 1.45f;
		public float Moon_Shafts_intensity = 0.45f;//v3.0
		public bool Mesh_moon = false; //If mesh used, smooth out shafts
		public bool Glow_moon = false; //Enable additive mode in shafts image effect
		public bool Glow_sun = false; // Additive shafts

	GameObject[] SkyMaster_TREE_objects;
	public Color TreeA_color = Color.white;
	public Color Terrain_tint = Color.white;
	public Color Grass_tint = Color.white;
	public bool Enable_trasition = false;	
		public TerrainData tData;//v3.3b
	public Material TerrainMat;

	Color Starting_grass_tint;
	Color Starting_terrain_tint;

	public float Trans_speed_tree=0.4f;
	public float Trans_speed_terr=1f;
	public float Trans_speed_grass=0.4f;
	public float Trans_speed_sky = 0.4f;

		//v1.2.5
		public bool Mesh_Terrain = false;
		public bool Foggy_Terrain = false;//Use with terrain fog shader - mesh terrain
		public bool Fog_Sky_Update = false;

		public Vector3 SUN_POS;
		public SkyMasterManager SkyManager;
		public float fog_depth = 0.29f;// 1.5f;
		public float reileigh = 1.3f;//2.0f;
		public float mieCoefficient = 1;//0.1f;
		public float mieDirectionalG = 0.1f;
		public float ExposureBias = 0.11f;//0.15f; 
		const float n = 1.0003f; 
		const float N = 2.545E25f;  
		const float pn = 0.035f;  
		public Vector3 lambda =  new Vector3(680E-9f, 550E-9f, 450E-9f);//new Vector3(680E-9f, 550E-9f, 450E-9f);
		public Vector3 K = new Vector3(0.9f, 0.5f, 0.5f);//new Vector3(0.686f, 0.678f, 0.666f);

void Start () {

	//v1.2.5
	if(!Mesh_Terrain){
		if (Terrain.activeTerrain != null) {
			tData = Terrain.activeTerrain.terrainData;
			Starting_grass_tint = tData.wavingGrassTint;
		}
	}

			if (TerrainMat != null) {//v3.3
				Starting_terrain_tint = TerrainMat.color;
			}

            //v2.2
            //currentTerrainTreeCol = Color.red;	//v3.3e
            if (Camera.main != null) {
                Cam_transf = Camera.main.transform;
            }

	//v3.0
	//RunPresets(SkyFog,SunShafts,10,true);
	//if (SkyFogTransp != null) {
	//	RunPresetsT(SkyFogTransp, SunShafts, 10, true);
	//}
}
void OnApplicationQuit() {

			if(!Mesh_Terrain){
				if(tData != null){
	tData.wavingGrassTint = Starting_grass_tint;
				}
			}

			if (TerrainMat != null) {//v3.3
				TerrainMat.color = Starting_terrain_tint;
			}
}

		void Update(){

			if (Application.isPlaying) {
				//v3.4.8
				if (Cam_transf == null) {
					Cam_transf = Camera.main.transform;
				}
			}

			if (AlwaysUpdateBillboards) {
				if(Application.isPlaying){
					if(toggle_rot==1){
						//Cam_transf.Rotate(Vector3.up,SmallRotFactor);
						//toggle_rot = 0;
						Cam_transf.Rotate(Vector3.up,SmallRotFactor);
					}
					if(currentTerrainTreeCol != TreeA_color){ //start rotating the camera little by little so is not visible, until effect ends. Use slow rate to make sure rotation is enough
						if(toggle_rot == 0){
							//Cam_transf.Rotate(Vector3.up,SmallRotFactor);
							toggle_rot = 1;
						}
					}else{
						toggle_rot = 0;
					}
				}
			}
		}
		int toggle_rot = 0;
		public float SmallRotFactor = 0.0001f;

		//v3.4.8
		Material[] mats;

void LateUpdate () {

			//v3.3b
			if(!Mesh_Terrain){
				if (tData != null) {
					//tData.wavingGrassTint = Starting_grass_tint;
				} else {
					if (Terrain.activeTerrain != null) {
						tData = Terrain.activeTerrain.terrainData;
					}
				}
			}

			//if (SkyFogTransp == null && Camera.main != null) {
			//	//SkyFogTransp = Camera.main.GetComponent<GlobalTranspFogSkyMaster> ();
			//}

			//if (SkyFog == null && Camera.main != null) {
			//	//find in camera
			//	//SkyFog = Camera.main.GetComponent<GlobalFogSkyMaster> ();
			//	//if (SkyFog == null) {
			//	//	if (ImageEffectFog) {
			//	//		Debug.Log ("Please enter the GlobalFogSkyMaster script in the camera to enable the volumetric fog. Ignore this message if in open prefab mode.");
			//	//	}
			//	//} else {
			//	//	//Debug.Log ("GlobalFogSkyMaster script found in the camera, enable 'ImageEffectFog' option to use the volumetric fog");
			//	//}
			//} else {
   //             //v2.0.1
   //             if (SkyFog != null)
   //             {
   //                 if (SkyFog.Sun == null)
   //                 {
   //                     SkyFog.Sun = SkyManager.SunObj.transform;
   //                 }
   //                 if (SkyFog.SkyManager == null)
   //                 {
   //                     SkyFog.SkyManager = SkyManager;
   //                 }
   //             }
			//}

			//if (SunShafts == null && Camera.main != null) {
			//	//find in camera
			//	//SunShafts = Camera.main.GetComponent<SunShaftsSkyMaster> ();
			//	//if (SunShafts == null) {
			//	//	if (ImageEffectShafts) {
			//	//		Debug.Log ("Please enter the SunShaftsSkyMaster script in the camera to enable the Sun Shafts. Ignore this message if in open prefab mode.");
			//	//	}
			//	//}
			//} else {
			//	//v2.0.1 - auto assign sun if not present
			//	if(SunShafts!=null && SunShafts.sunTransform == null){
			//		SunShafts.sunTransform = SkyManager.SunObj.transform;
			//	}
			//}

			////v2.1
			//if(StereoMode){
			//	if(SkyFog != null & LeftCam != null & RightCam != null){
			//		//try to grab effect, if does not exist add/copy from main camera SkyFog
			//		//if(SkyFogL == null){
			//		//	SkyFogL = LeftCam.GetComponent(typeof(GlobalFogSkyMaster)) as GlobalFogSkyMaster;
			//		//	if(SkyFogL == null){//if not found, copy from main
			//		//		LeftCam.AddComponent<GlobalFogSkyMaster>();
			//		//	}
			//		//}
			//		//if(SkyFogR == null){
			//		//	SkyFogR = RightCam.GetComponent(typeof(GlobalFogSkyMaster)) as GlobalFogSkyMaster;
			//		//	if(SkyFogR == null){//if not found, copy from main
			//		//		RightCam.AddComponent<GlobalFogSkyMaster>();
			//		//	}
			//		//}

			//		//if(SkyFogLT == null){
			//		//	SkyFogLT = LeftCam.GetComponent(typeof(GlobalTranspFogSkyMaster)) as GlobalTranspFogSkyMaster;
			//		//	if(SkyFogLT == null){//if not found, copy from main
			//		//		LeftCam.AddComponent<GlobalTranspFogSkyMaster>();
			//		//	}
			//		//}
			//		//if(SkyFogRT == null){
			//		//	SkyFogRT = RightCam.GetComponent(typeof(GlobalTranspFogSkyMaster)) as GlobalTranspFogSkyMaster;
			//		//	if(SkyFogRT == null){//if not found, copy from main
			//		//		RightCam.AddComponent<GlobalTranspFogSkyMaster>();
			//		//	}
			//		//}

			//		//if(SunShaftsL == null){
			//		//	SunShaftsL = LeftCam.GetComponent(typeof(SunShaftsSkyMaster)) as SunShaftsSkyMaster;
			//		//	if(SunShaftsL == null){//if not found, copy from main
			//		//		LeftCam.AddComponent<SunShaftsSkyMaster>();
			//		//	}
			//		//}
			//		//if(SunShaftsR == null){
			//		//	SunShaftsR = RightCam.GetComponent(typeof(SunShaftsSkyMaster)) as SunShaftsSkyMaster;
			//		//	if(SunShaftsR == null){//if not found, copy from main
			//		//		RightCam.AddComponent<SunShaftsSkyMaster>();
			//		//	}
			//		//}

			//		if(SkyFogL != null & SkyFogR != null & SunShaftsL != null & SunShaftsR != null){
			//			float speed = 10;
			//			if(!Application.isPlaying){
			//				speed = 10000;
			//			}


			//			//if(SkyFogTransp != null && SkyFogLT != null & SkyFogRT != null & UseTranspVFog){
			//			//	RunPresetsT(SkyFogLT,SunShaftsL,speed,false);
			//			//	RunPresetsT(SkyFogRT,SunShaftsR,speed,false);
			//			//}else{
			//			//	RunPresets(SkyFogL,SunShaftsL,speed,false);
			//			//	RunPresets(SkyFogR,SunShaftsR,speed,false);
			//			//}

			//		}

			//	}else{
			//		Debug.Log ("Please enter the left/right cameras in the script parameters to use stereo mode");
			//	}
			//}else{
			//	float speed = 10;
			//	if(!Application.isPlaying){
			//		speed = 10000;
			//	}

				
			//}

			//if (ImageEffectFog) { //v3.3a
			//	if (UseTranspVFog) {
			//		if (SkyFog != null) {
			//			SkyFog.enabled = false;
			//		}
			//		if (SkyFogTransp != null) {
			//			SkyFogTransp.enabled = true;
			//		}
			//		if (SkyFogLT != null & SkyFogRT != null) {
			//			SkyFogLT.enabled = true;
			//			SkyFogRT.enabled = true;
			//		}
			//		if (SkyFogL != null & SkyFogR != null) {
			//			SkyFogL.enabled = false;
			//			SkyFogR.enabled = false;
			//		}
			//	} else {
			//		if (SkyFog != null) {
			//			SkyFog.enabled = true;
			//		}
			//		if (SkyFogTransp != null) {
			//			SkyFogTransp.enabled = false;
			//		}
			//		if (SkyFogLT != null & SkyFogRT != null) {
			//			SkyFogLT.enabled = false;
			//			SkyFogRT.enabled = false;
			//		}
			//		if (SkyFogL != null & SkyFogR != null) {
			//			SkyFogL.enabled = true;
			//			SkyFogR.enabled = true;
			//		}
			//	}
			//}

			//v2.1 - run presets moved to function

//////////////////////////////////////////////// CHANGE STANDALONE TREE COLOR and GRASS TINT ///////////////////////
if (Enable_trasition){
		/////////////////// TREE COLOR //////////////////////
				 
				//v2.2
				if(currentTerrainTreeCol != TreeA_color){
					currentTerrainTreeCol =  Color.Lerp (currentTerrainTreeCol,TreeA_color,Trans_speed_tree*Time.deltaTime);
					Shader.SetGlobalVector("_UnityTerrainTreeTintColorSM", currentTerrainTreeCol);
				}
				//v3.4.8
				if(TreePefabs != null){
					for (int i = 0;i<TreePefabs.Count;i++){
						if(TreePefabs[i] !=null){
							mats = TreePefabs [i].GetComponent<Renderer> ().sharedMaterials;
							for(int j=0;j<mats.Length;j++){
								if(mats[j].name.Contains("Leaf")
									|| mats[j].name.Contains("leaf")
									|| mats[j].name.Contains("Leaves")
									|| mats[j].name.Contains("leaves")
								){
									if(mats[j].HasProperty("_Color")){
										if(mats[j].color != TreeA_color){
											Color current = mats[j].color;
											mats[j].color = Color.Lerp (current,TreeA_color,Trans_speed_tree*Time.deltaTime);
										}
									}
								}
							}
						}
					}
				}
				 
		if(tagBasedTreeGrab && SkyMaster_TREE_objects == null){//v3.0.3
			SkyMaster_TREE_objects = GameObject.FindGameObjectsWithTag("SkyMasterTree");
		}
		//if(TreeA_color_prev != TreeA_color | Terrain_tint_prev!=Terrain_tint | Grass_tint_prev!=Grass_tint){
		if(SkyMaster_TREE_objects != null & Application.isPlaying){
			if(SkyMaster_TREE_objects.Length > 0){
				
						for (int i = 0;i<SkyMaster_TREE_objects.Length;i++){
							//v1.7 - handle Speed tree first
//							if(SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials.Length >4){
//								//Debug.l
//								if(SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[2].color != TreeA_color){
//									Color current = SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[2].color;
//									SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[2].color = Color.Lerp (current,TreeA_color,Trans_speed_tree*Time.deltaTime);
//								}
//								if(SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[4].color != TreeA_color){
//									Color current = SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[4].color;
//									SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[4].color = Color.Lerp (current,TreeA_color,Trans_speed_tree*Time.deltaTime);
//								}
//							}else{
//								if(SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[1].color != TreeA_color){
//									Color current = SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[1].color;
//									SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[1].color = Color.Lerp (current,TreeA_color,Trans_speed_tree*Time.deltaTime);
//								}
//							}
							//Search in materials for Leaf or Leaves
							for(int j=0;j<SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials.Length;j++){
								if(SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[j].name.Contains("Leaf")
								   | SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[j].name.Contains("leaf")
								   | SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[j].name.Contains("Leaves")
								   | SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[j].name.Contains("leaves")
								   ){
									if(SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[j].color != TreeA_color){
										Color current = SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[j].color;
										SkyMaster_TREE_objects[i].GetComponent<Renderer>().sharedMaterials[j].color = Color.Lerp (current,TreeA_color,Trans_speed_tree*Time.deltaTime);
									}
								}
							}
//							if(UpdateLeafMat){
//								for(int j=0;j<LeafMats.Count;j++){
//									if(LeafMats[j].color != TreeA_color){
//										Color current = LeafMats[j].color;
//										LeafMats[j].color = Color.Lerp (current,TreeA_color,Trans_speed_tree*Time.deltaTime);
//									}
//								}
//							}
						}

			}
		}

				//v1.7
				if(Application.isPlaying){
					if(UpdateLeafMat){
						for(int j=0;j<LeafMats.Count;j++){
							if(LeafMats[j].color != TreeA_color){
								Color current = LeafMats[j].color;
								LeafMats[j].color = Color.Lerp (current,TreeA_color,Trans_speed_tree*Time.deltaTime);
							}
						}
					}
				}

		//v1.2.5
		if(!Mesh_Terrain){
			if(tData != null && tData.wavingGrassTint != Grass_tint){ //v4.8
				Color current1 = tData.wavingGrassTint;		 
				tData.wavingGrassTint = Color.Lerp (current1,Grass_tint,Trans_speed_grass*Time.deltaTime);

			}
		}

		//if(TerrainMat != null && TerrainMat.color != Terrain_tint & Application.isPlaying){ //v3.4
		if(TerrainMat != null && TerrainMat.color != Terrain_tint){
					if (Application.isPlaying) {
						Color current2 = TerrainMat.color;
						TerrainMat.color = Color.Lerp (current2, Terrain_tint, Trans_speed_terr * Time.deltaTime);
					} else {
						//Color current2 = TerrainMat.color;
						TerrainMat.color = Terrain_tint;
					}
		}		
	}
	//////////////////////////////////////////////// CHANGE STANDALONE TREE COLOR and GRASS TINT ///////////////////////

	if(Foggy_Terrain){
		if(TerrainMat != null && SkyManager != null){

					//Debug.Log ("AA");
					if(Fog_Sky_Update){
						reileigh = SkyManager.m_fRayleighScaleDepth;
						reileigh = SkyManager.m_Kr;
						mieCoefficient = SkyManager.m_Km;
						//fog_depth = 1.5f;
						TerrainMat.SetVector("sunPosition", -SkyManager.SunObj.transform.forward.normalized);
						TerrainMat.SetVector("betaR", totalRayleigh(lambda) * reileigh);
						TerrainMat.SetVector("betaM", totalMie(lambda, K, fog_depth) * mieCoefficient);
						TerrainMat.SetFloat("fog_depth", fog_depth);
						TerrainMat.SetFloat("mieCoefficient", mieCoefficient);
						TerrainMat.SetFloat("mieDirectionalG", mieDirectionalG);    
						TerrainMat.SetFloat("ExposureBias", ExposureBias); 
					}else{
						TerrainMat.SetVector("sunPosition", -SkyManager.SunObj.transform.forward.normalized);
						//TerrainMat.SetVector("sunPosition", SUN_POS);
						TerrainMat.SetVector("betaR", totalRayleigh(lambda) * reileigh);
						TerrainMat.SetVector("betaM", totalMie(lambda, K, fog_depth) * mieCoefficient);
						TerrainMat.SetFloat("fog_depth", fog_depth);
						TerrainMat.SetFloat("mieCoefficient", mieCoefficient);
						TerrainMat.SetFloat("mieDirectionalG", mieDirectionalG);    
						TerrainMat.SetFloat("ExposureBias", ExposureBias); 
					}
		}
	}

   }// end LateUpdate

		static Vector3 totalRayleigh(Vector3 lambda)
		{
			Vector3 result = new Vector3((8.0f * Mathf.Pow(Mathf.PI, 3.0f) * Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pn)) / (3.0f * N * Mathf.Pow(lambda.x, 4.0f) * (6.0f - 7.0f * pn)),
			                             (8.0f * Mathf.Pow(Mathf.PI, 3.0f) * Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pn)) / (3.0f * N * Mathf.Pow(lambda.y, 4.0f) * (6.0f - 7.0f * pn)),
			                             (8.0f * Mathf.Pow(Mathf.PI, 3.0f) * Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pn)) / (3.0f * N * Mathf.Pow(lambda.z, 4.0f) * (6.0f - 7.0f * pn)));
			return result;
		}
		
		static Vector3 totalMie(Vector3 lambda, Vector3 K, float T)
		{
			float c = (0.2f * T) * 10E-17f;
			Vector3 result = new Vector3(
				0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / lambda.x, 2.0f) * K.x,
				0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / lambda.y, 2.0f) * K.y,
				0.434f * c * Mathf.PI * Mathf.Pow((2.0f * Mathf.PI) / lambda.z, 2.0f) * K.z
				);
			return result;
		}


 }
}
