using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {

[ExecuteInEditMode]
public class CloudHandlerSM_SRP : MonoBehaviour {

        //HDRP v0.1        
        [HideInInspector] public Transform Hero;
        public float WeatherSeverity = 1;
        public WindZone windZone;
        public float coverageSpeed = 1;
        public Transform Sun; public Transform Moon;
        [HideInInspector] public bool useGradients = false;
        public float Current_Time = 12;
        [HideInInspector] public bool AutoSunPosition = false; //define position either by sun transform (true) or time of day defined (false)
        public float Shift_dawn = 0;
        public float NightTimeMax = 23;
        public float fogPower = 0;
        public float fogPowerExp = 1;

        //v3.4.3
        [HideInInspector] public bool followPlayer = false;
        [HideInInspector] public bool rotateWithCamera = false;
        [HideInInspector] public bool rotateMultiQuadC = false;
		//public Vector3 ShadowPlaneScale = new Vector3(500,600,900);
		public float IntensityDiffOffset = 0;
		public float IntensitySunOffset = 0;
		public float IntensityFogOffset = 0;

		[HideInInspector] public bool EnableInsideClouds = false;

        [HideInInspector] public bool FogFromSkyGrad = false;

		[HideInInspector] public bool LerpRot = false;

        [HideInInspector]
        public AnimationCurve IntensityDiff = new AnimationCurve (new Keyframe (0,0.4f), 
			new Keyframe (0.374f, 0.292f), new Keyframe (0.6f,0.2766f), new Keyframe (0.757f,0.278f), new Keyframe (0.798f,0.271f),
			new Keyframe (0.849f,0.275f), new Keyframe (0.887f,0.248f), new Keyframe (0.944f,0.280f),
			new Keyframe (1,0.4f));

        [HideInInspector]
        public AnimationCurve IntensityFog = new AnimationCurve (new Keyframe (0,5f), 
			new Keyframe (0.75f,10f), new Keyframe (0.88f,11f), new Keyframe (0.89f,10.58f), 
			new Keyframe (1,5f));

        [HideInInspector]
        public AnimationCurve IntensitySun = new AnimationCurve (new Keyframe (0,0.078f), 
			new Keyframe (0.1864f,0.148f), new Keyframe (0.71f,0.129f), new Keyframe (0.839f,0.303f), new Keyframe (0.90f,0.13f),
			new Keyframe (1,0.078f));
        //public bool CurvesFromSkyMaster = false;//use Sky master inspector to grab the above curve values, these are global for all volume clouds and exported when SM is in gradient mode

		public bool DomeClouds = true;
		public Vector3 DomeScale;
		public Vector3 DomeHeights;

		public bool MultiQuadClouds = false;
		public Vector3 MultiQuadScale;
		public Vector3 MultiQuadHeights;

		public bool MultiQuadAClouds = false;
		public Vector3 MultiQuadAScale;
		public Vector3 MultiQuadAHeights;
		public bool MultiQuadBClouds = false;
		public Vector3 MultiQuadBScale;
		public Vector3 MultiQuadBHeights;
		public bool MultiQuadCClouds = false;//v3.4.3
		public Vector3 MultiQuadCScale;
		public Vector3 MultiQuadCHeights;

		public bool OneQuadClouds = false;
		public Vector3 OneQuadScale;
		//Heights of clouds - x=transform scale, y = shader height cutoff, z = 
		public Vector3 OneQuadHeights;

		//v3.4.2 - shadow planes not rotate
		public Transform FlatBedSPlane;
		public Transform SideSPlane;
		public Transform SideASPlane;
		public Transform SideBSPlane;
		public Transform SideCSPlane;
		public Transform RotCloudsSPlane;

		public bool WeatherDensity = false; //refine density based on weather

		//v3.4 - Cloud properties
		//public float HeightSwitch = 250;
		//public bool AboveClouds = false;
		public enum CloudPreset {Custom, ClearDay, Storm};//, Storm, Mobile};

		public bool UseUnityFog = false;

		public float CloudEvolution = 0.07f;

		public float ClearDayCoverage = -0.12f;
		//public float ClearDayTransp = 0.51f;
		//public float ClearDayIntensity = 0.1f;
		public float ClearDayHorizon = 0.1f;

		public float StormCoverage = 0;
		public float StormHorizon = 0.01f;
		public Color StormSunColor = new Color(0.5f,0.5f,0.5f,0.5f);

		public float Coverage = -0.12f;
		public float Transp = 0.51f;
		public float Intensity = 0.1f;
		public float Horizon = 0.1f;

		public CloudPreset cloudType = CloudPreset.ClearDay;
        //public Vector3 cloudDomeScale = Vector3.one;

        //public SkyMasterManager SkyManager;

        [HideInInspector] public bool autoRainbow = false;
        [HideInInspector] public float rainbowApperSpeed = 1;
        // bool was_rain = false;
        float was_rain_end_time;
        [HideInInspector] public float rainbowTime = 60;
        //public float rainbowIntensity = 0;
        [HideInInspector] public float rainboxMaxIntensity = 0.7f;
        [HideInInspector] public float rainboxMinIntensity = 0.0f;
		public Material rainbowMat;

		public Material cloudFlatMat;
		public Material cloudSidesMat;
		public Material cloudSidesAMat;
		public Material cloudSidesBMat;
		public Material cloudSidesCMat;//v3.4.3
		public Material cloudRotMat;

		public Transform FlatBedClouds;
		public Transform SideClouds;
		public Transform SideAClouds;
		public Transform SideBClouds;
		public Transform SideCClouds;//v3.4.3
		public Transform RotClouds;

		public GameObject LightningPrefab; //Prefab to instantiate for lighting, use only 1-2 prefabs and move them around
		public bool EnableLightning = false;

		//Color DayCloudColor = new Color(91f/255f,139f/255f,129f/255f,209f/255f);
		public Color DayCloudColor = new Color(230f/255f,230f/255f,230f/255f,20f/255f);//new Color(88f/255f,40f/255f,40f/255f,76f/255f);
		public Color DayCloudShadowCol = new Color(61f/255f,61f/255f,81f/255f,70f/255f);// new Color(61f/255f,61f/255f,81f/255f,61f/255f);
		public Color DayCloudTintCol = new Color(11f/255f,11f/255f,11f/255f,255f/255f);//new Color(156f/255f,142f/255f,142f/255f,255f/255f);
		public Color DayCloudFogCol = new Color(196f/255f,219f/255f,234f/255f,255f/255f);
		public float DayFogFactor = 1;
		public float DayIntensity = 0.1f;

		public Color DawnCloudColor = new Color(231f/255f,190f/255f,240f/255f,110f/255f);//new Color(31f/255f,29f/255f,29f/255f,37f/255f);
		public Color DawnCloudShadowCol =new Color(70f/255f,3f/255f,3f/255f,104f/255f);// new Color(201f/255f,150f/255f,140f/255f,63f/255f);
		public Color DawnCloudTintCol = new Color(11f/255f,10f/255f,10f/255f,255f/255f);//new Color(201f/255f,150f/255f,120f/255f,255f/255f);
		public Color DawnCloudFogCol = new Color(246f/255f,19f/255f,14f/255f,225f/255f);
		public float DawnFogFactor = 1.94f;
		public float DawnIntensity = -3.79f;

	//	Color DuskCloudColor = new Color(191f/255f,99f/255f,119f/255f,209f/255f);
		public Color DuskCloudColor = new Color(31f/255f,29f/255f,29f/255f,37f/255f);	
		public Color DuskCloudShadowCol = new Color(161f/255f,82f/255f,39f/255f,63f/255f);
		public Color DuskCloudTintCol = new Color(11f/255f,10f/255f,10f/255f,255f/255f);//new Color(201f/255f,150f/255f,120f/255f,255f/255f);
		public Color DuskCloudFogCol = new Color(246f/255f,19f/255f,14f/255f,225f/255f);
		public float DuskFogFactor = 1.94f;
		public float DuskIntensity = -1.25f;

		public Color NightCloudColor = new Color(220f/255f,225f/255f,210f/255f,7f/255f);//v3.4.3

		int prev_preset;

        [HideInInspector] public bool HasScatterShader = false;
        //[HideInInspector] public bool UpdateScatterShader = false;

		//scatter params
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

		public float WindStrength=1;
		public float WindParallaxFactor = 1.2f;

		//public bool AutoDensity = true;
		public float CloudDensity = 0.0001f;

		void Start () {

		if (Application.isPlaying) {


				if (rainbowMat != null) {
					Color RainbowC = rainbowMat.GetColor ("_Color");
					rainbowMat.SetColor ("_Color",new Color(RainbowC.r,RainbowC.g,RainbowC.b,0));
				}
			
			
			//v3.4 - if not defined, get material for water
			if (cloudFlatMat == null) {
				cloudFlatMat = FlatBedClouds.gameObject.GetComponentsInChildren<MeshRenderer> (true) [0].material;
			}
			if (cloudSidesMat == null) {
				cloudSidesMat = SideClouds.gameObject.GetComponentsInChildren<MeshRenderer> (true) [0].material;
			}
			if (cloudSidesAMat == null) {
				cloudSidesAMat = SideAClouds.gameObject.GetComponentsInChildren<MeshRenderer> (true) [0].material;
			}
				if (cloudSidesBMat == null) {
					cloudSidesBMat = SideBClouds.gameObject.GetComponentsInChildren<MeshRenderer> (true) [0].material;
				}
				if (cloudSidesCMat == null) {//v3.4.3
					cloudSidesCMat = SideCClouds.gameObject.GetComponentsInChildren<MeshRenderer> (true) [0].material;
				}
			if (cloudRotMat == null) {
					cloudRotMat = RotClouds.gameObject.GetComponentsInChildren<MeshRenderer> (true) [0].material;
			}	

			//v3.0
		

			if (WeatherDensity){// && Application.isPlaying) { //v3.4.3

                    if (cloudType == CloudPreset.ClearDay)
                    {
                        Coverage = ClearDayCoverage;
                        Horizon = ClearDayHorizon;
                        EnableLightning = false;
                    }
                    if (cloudType == CloudPreset.Storm)
                    {
                        Coverage = StormCoverage;
                        Horizon = StormHorizon;
                        EnableLightning = true;
                    }

                }
		}

			if (FlatBedClouds != null && cloudFlatMat != null) {
				DomeScale = FlatBedClouds.localScale;
				//DomeHeights = new Vector3 (FlatBedClouds.position.y,cloudFlatMat.GetFloat("_CutHeight"), 0);
			}
			if (SideClouds != null && cloudSidesMat != null) {
				MultiQuadScale = SideClouds.localScale;
				//MultiQuadHeights = new Vector3 (SideClouds.position.y,cloudSidesMat.GetFloat("_CutHeight"), 0);
			}
			if (SideAClouds != null && cloudSidesAMat != null) {
				MultiQuadAScale = SideAClouds.localScale;
				//MultiQuadAHeights = new Vector3 (SideAClouds.position.y,cloudSidesAMat.GetFloat("_CutHeight"), 0);
			}
			if (SideBClouds != null && cloudSidesBMat != null) {
				MultiQuadBScale = SideBClouds.localScale;
				//MultiQuadBHeights = new Vector3 (SideBClouds.position.y,cloudSidesBMat.GetFloat("_CutHeight"), 0);
			}
			if (SideCClouds != null && cloudSidesCMat != null) { //v3.4.3
				MultiQuadCScale = SideCClouds.localScale;
				//MultiQuadCHeights = new Vector3 (SideCClouds.position.y,cloudSidesCMat.GetFloat("_CutHeight"), 0);
			}
			if (RotClouds != null && cloudRotMat != null) {
				OneQuadScale = RotClouds.localScale;
				//OneQuadHeights = new Vector3 (RotClouds.position.y,cloudRotMat.GetFloat("_CutHeight"), 0);
			}

	}

			

	

			
	
		void UpdateCloudParams (bool Init, Material oceanMat) {

			


			//IF UNDERWATER
			//float Sm_speed = SkyManager.SPEED;// + minShiftSpeed;
			float Time_delta = Time.deltaTime;

			if (Init) {
				//Sm_speed=10000;
				Time_delta=10000;
			}
            
            if(Sun != null) {
                oceanMat.SetVector("sunPosition", -Sun.forward.normalized);
            }


			//APPLY WIND
			if (windZone != null) {
				Vector4 _Velocity1 = oceanMat.GetVector ("_Velocity1");
				Vector4 _Velocity2 = oceanMat.GetVector ("_Velocity2"); 

				float WindStr = windZone.windMain;

				Vector4 WindDir = new Vector4 (windZone.transform.forward.x, windZone.transform.forward.z, 0, -0.07f * 0.1f) * (-WindStr) * WindStrength * (1);

				oceanMat.SetVector ("_Velocity1", Vector4.Lerp (_Velocity1, WindDir, 0.0001f * Time_delta * 21));
				//		oceanMat.SetVector("_Velocity2",Vector4.Lerp(_Velocity2, WindDir*WindParallaxFactor, 0.001f * Time_delta * 11) );

				oceanMat.SetVector ("_Velocity2", new Vector4 (Mathf.Lerp (_Velocity2.x, WindDir.x * WindParallaxFactor*(1), 0.0001f * Time_delta * 11*WindParallaxFactor*(1)), 
					Mathf.Lerp (_Velocity2.y, WindDir.y * WindParallaxFactor*(1), 0.0001f * Time_delta * 11*WindParallaxFactor), 0.0f, CloudEvolution));
				//oceanMat.SetVector("_Velocity2", new Vector4() );
			}

			if (HasScatterShader) {
							

				//v3.4.5
				if(!Application.isPlaying){
					//Speed_factor = 1000 * Speed_factor;
					Time_delta=10000;
				}

				//GRAB PARAMETERS
				Color _SunColor = oceanMat.GetColor ("_SunColor");
				Color _ShadowColor = oceanMat.GetColor ("_ShadowColor");
	//			float _ColorDiff = oceanMat.GetFloat("_ColorDiff");

				//					_CloudMap ("_CloudMap", 2D) = "white" {}
				//					_CloudMap1 ("_CloudMap1", 2D) = "white" {}

				float _Density = oceanMat.GetFloat("_Density");
				//if (AutoDensity) {

				//}
				oceanMat.SetFloat ("_Density",Mathf.Lerp(_Density, CloudDensity, 0.27f * Time_delta +0.05f));//v3.4.3

				float _Coverage = oceanMat.GetFloat("_Coverage");
				float _Transparency = oceanMat.GetFloat("_Transparency");				

			//	Vector4 _LightingControl = oceanMat.GetVector ("_LightingControl");   
				float _HorizonFactor  = oceanMat.GetFloat("_HorizonFactor");

				Color _Color  = oceanMat.GetColor ("_Color");
				Color _FogColor = oceanMat.GetColor ("_FogColor");
		
				if (UseUnityFog) {
					oceanMat.SetFloat ("_FogUnity", 1);
				} else {
					oceanMat.SetFloat ("_FogUnity", 0);
				}

				float Speed_factor = 0.3f * (coverageSpeed +0.1f );//0.27

				//v3.4.5
				if(!Application.isPlaying){
					Speed_factor = 1000 * Speed_factor;
					//Time_delta=10000;
				}

                float EvalSunIntensity = 0;
                float EvalLightDiff = 0;
                float EvalFog = 0;
                if (useGradients)
                {
                    EvalSunIntensity = IntensitySun.Evaluate(Current_Time) + IntensitySunOffset;
                    EvalLightDiff = IntensityDiff.Evaluate(Current_Time) + IntensityDiffOffset + 0.30f;//v3.4.3
                    EvalFog = IntensityFog.Evaluate(Current_Time) + IntensityFogOffset;                    
                }
                else
                {
                    EvalSunIntensity = IntensitySunOffset;
                    //oceanMat.SetFloat("_ColorDiff", IntensityDiffOffset - 0.50f);
                    EvalLightDiff =  IntensityDiffOffset + 0.30f;//v3.4.3
                    EvalFog = IntensityFogOffset;
                }
                
                float Rot_Sun_X = Sun.transform.eulerAngles.x;

                bool is_DayLight = (AutoSunPosition && Rot_Sun_X > 0) || (!AutoSunPosition && Current_Time > (9.0f + Shift_dawn) && Current_Time <= (NightTimeMax + Shift_dawn));

                bool is_after_17 = (AutoSunPosition && Rot_Sun_X > 65) || (!AutoSunPosition && Current_Time > (17.0f + Shift_dawn));

                bool is_after_222 = (AutoSunPosition && Rot_Sun_X > 85) || (!AutoSunPosition && Current_Time > (22.0f + Shift_dawn));

                bool is_before_10 = (AutoSunPosition && Rot_Sun_X < 10) || (!AutoSunPosition && Current_Time < (10.0f + Shift_dawn));

                //	bool is_after_22  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 5 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (21.0f + SkyManager.Shift_dawn));

                //	bool is_after_21  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 75) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (20.7f + SkyManager.Shift_dawn));
                
                //bool is_DayLight  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 0 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time > ( 9.0f + SkyManager.Shift_dawn) & SkyManager.Current_Time <= (21.9f + SkyManager.Shift_dawn));
                //bool is_after_17  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 65) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (17.1f + SkyManager.Shift_dawn));
                
                if (is_DayLight)
                {
                    oceanMat.SetVector("sunPosition", -Sun.transform.forward.normalized);
                    //oceanMat.SetVector ("_SunColor", Color.Lerp (oceanMat.GetVector ("_SunColor"), DayCloudColor, 0.5f * Time.deltaTime));


                    if (is_after_17)
                    { //is_after_21
                      //oceanMat.SetVector ("_TintColor", Color.Lerp (oceanMat.GetVector ("_TintColor"), Color.Lerp (DayCloudColor, DuskCloudColor, 0.5f), 0.5f * Time.deltaTime));
                        if (!is_after_222)
                        {
                            if (cloudType == CloudPreset.Custom)
                            {

                            }
                            else if (cloudType == CloudPreset.ClearDay)
                            {
                                oceanMat.SetColor("_SunColor", Color.Lerp(_SunColor, DuskCloudColor + new Color(0,0,0, EvalSunIntensity), 3.0f * Speed_factor * Time_delta)); //v3.4.3 - added dusk colors

                                oceanMat.SetColor("_ShadowColor", Color.Lerp(_ShadowColor, DuskCloudShadowCol, Speed_factor * Time_delta));

                                //			oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DuskIntensity, Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat("_Coverage", Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat("_Transparency", Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat ("_HorizonFactor", Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor("_Color", Color.Lerp(_Color, DuskCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor("_FogColor", Color.Lerp(_FogColor, DuskCloudFogCol, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DuskFogFactor, Speed_factor * Time_delta));
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            ////////////////////////////////////////// NIGHT CLOUDS
                            if (cloudType == CloudPreset.Custom)
                            {

                            }
                            else if (cloudType == CloudPreset.ClearDay)
                            {
                                oceanMat.SetColor("_SunColor", Color.Lerp(_SunColor, DayCloudColor * 0.3f + new Color(0, 0, 0, EvalSunIntensity), 0.2f * Speed_factor * Time_delta));

                                oceanMat.SetColor("_ShadowColor", Color.Lerp(_ShadowColor, 4 * DayCloudShadowCol, Speed_factor * Time_delta));

                                //			oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DayIntensity, 0.2f * Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat("_Coverage", Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat("_Transparency", Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat ("_HorizonFactor", Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor("_Color", Color.Lerp(_Color, DayCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor("_FogColor", Color.Lerp(_FogColor, DayCloudFogCol, 4 * Speed_factor * Time_delta));
                                //		oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DayFogFactor * 2, 0.02f * Speed_factor * Time_delta));

                            }
                            else
                            {

                            }
                        }

                    }
                    else
                    {
                        if (is_before_10)
                        {
                            if (cloudType == CloudPreset.Custom)
                            {

                            }
                            else if (cloudType == CloudPreset.ClearDay)
                            {
                                oceanMat.SetColor("_SunColor", Color.Lerp(_SunColor, DawnCloudColor + new Color(0, 0, 0, EvalSunIntensity), 1.0f * Speed_factor * Time_delta)); //

                                oceanMat.SetColor("_ShadowColor", Color.Lerp(_ShadowColor, DawnCloudShadowCol, Speed_factor * Time_delta));

                                //			oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DawnIntensity, Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat("_Coverage", Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat("_Transparency", Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat ("_HorizonFactor", Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor("_Color", Color.Lerp(_Color, DawnCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor("_FogColor", Color.Lerp(_FogColor, DawnCloudFogCol, Speed_factor * Time_delta));
                                //		oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DawnFogFactor, Speed_factor * Time_delta));
                            }
                            else
                            {

                            }
                            //oceanMat.SetVector ("_TintColor", Color.Lerp (oceanMat.GetVector ("_TintColor"), Color.Lerp (DayCloudColor, DawnCloudColor, 0.5f), 0.5f * Time.deltaTime * 0.2f * SkyManager.DawnAppearSpeed)); 
                        }
                        else
                        {
                            if (cloudType == CloudPreset.Custom)
                            {

                            }
                            else if (cloudType == CloudPreset.ClearDay)
                            {
                                oceanMat.SetColor("_SunColor", Color.Lerp(_SunColor, DayCloudColor + new Color(0, 0, 0, EvalSunIntensity), 0.3f * Speed_factor * Time_delta));

                                oceanMat.SetColor("_ShadowColor", Color.Lerp(_ShadowColor, DayCloudShadowCol, Speed_factor * Time_delta));

                                //		oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DayIntensity, Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat("_Coverage", Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat("_Transparency", Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //		oceanMat.SetFloat ("_HorizonFactor", Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor("_Color", Color.Lerp(_Color, DayCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor("_FogColor", Color.Lerp(_FogColor, DayCloudFogCol, Speed_factor * Time_delta));
                                //		oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DayFogFactor, Speed_factor * Time_delta));

                            }
                            else
                            {

                            }
                            //oceanMat.SetVector ("_TintColor", Color.Lerp (oceanMat.GetVector ("_TintColor"), Color.Lerp (DayCloudColor, DawnCloudColor, 0.5f), 0.5f * Time.deltaTime)); 
                        }

                    }
                }
                else
                {
                    if (Moon != null)
                    {
                        oceanMat.SetVector("sunPosition", -Moon.transform.forward.normalized);
                    }
                    else
                    {
                        oceanMat.SetVector("sunPosition", -Sun.transform.forward.normalized);
                    }
                    //oceanMat.SetVector ("_SunColor", Color.Lerp (oceanMat.GetVector ("_SunColor"), NightCloudColor, 0.5f * Time.deltaTime));//		

                    ////////////////////////////////////////// NIGHT CLOUDS
                    if (cloudType == CloudPreset.Custom)
                    {

                    }
                    else if (cloudType == CloudPreset.ClearDay)
                    {
                        //oceanMat.SetColor ("_SunColor", Color.Lerp (_SunColor, DayCloudColor * 0.3f, 0.8f * Speed_factor * Time_delta));
                        oceanMat.SetColor("_SunColor", Color.Lerp(_SunColor, NightCloudColor + new Color(0, 0, 0, EvalSunIntensity), 0.8f * Speed_factor * Time_delta));//v3.4.3

                        oceanMat.SetColor("_ShadowColor", Color.Lerp(_ShadowColor, 1 * DayCloudShadowCol, Speed_factor * Time_delta));

                        //	oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DayIntensity, 0.2f * Speed_factor * Time_delta));//hardcoded _ColorDiff
                        oceanMat.SetFloat("_Coverage", Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                        oceanMat.SetFloat("_Transparency", Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                        //	oceanMat.SetFloat ("_HorizonFactor", Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                        oceanMat.SetColor("_Color", Color.Lerp(_Color, DayCloudTintCol, Speed_factor * Time_delta));
                        oceanMat.SetColor("_FogColor", Color.Lerp(_FogColor, DayCloudFogCol, 4 * Speed_factor * Time_delta));
                        //	oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DayFogFactor * 2, 0.22f * Speed_factor * Time_delta));

                    }
                    else
                    {

                    }


                }

                //STORM
                if(WeatherDensity && cloudType == CloudPreset.Storm)
                {
                    // StormSunColor
                    oceanMat.SetColor("_SunColor", Color.Lerp(_SunColor, StormSunColor, 2.8f * Speed_factor * Time_delta));//v3.4.3
                }


                oceanMat.SetFloat("_ColorDiff", EvalLightDiff);
                oceanMat.SetFloat("_HorizonFactor", Mathf.Lerp(_HorizonFactor, Horizon, Speed_factor * Time_delta + 0.05f));//v3.4.3
                oceanMat.SetFloat("_FogFactor", EvalFog);


                oceanMat.SetVector ("betaR", totalRayleigh (lambda) * reileigh);
				oceanMat.SetVector ("betaM", totalMie (lambda, K, fog_depth) * mieCoefficient);
				oceanMat.SetFloat ("fog_depth", fog_depth);
				oceanMat.SetFloat ("mieCoefficient", mieCoefficient);
				oceanMat.SetFloat ("mieDirectionalG", mieDirectionalG);    
				oceanMat.SetFloat ("ExposureBias", ExposureBias);
                ///////////////////////////////////////////////////
                /// 

                oceanMat.SetFloat("fogPower", fogPower);
                oceanMat.SetFloat("fogPowerExp", fogPowerExp);


                float modifier = 0.002f; //v3.4.8
				if (!Application.isPlaying) {
					modifier = 0.1f; //8 modifiers
				}

                //HDRP
                if (lightningScript1 != null && LightningOne.gameObject.activeInHierarchy)
                {
                    //pass light to shader
                    oceanMat.SetVector("pointLightPos", lightningScript1.startLight.transform.position);
                    oceanMat.SetFloat("pointLightPower", lightningScript1.startLight.intensity);
                    oceanMat.SetVector("pointLightColor", lightningScript1.startLight.color);
                }
                if (lightningScript2 != null && LightningTwo.gameObject.activeInHierarchy)
                {
                    //pass light to shader
                    oceanMat.SetVector("pointLightPos", lightningScript2.startLight.transform.position);
                    oceanMat.SetFloat("pointLightPower", lightningScript2.startLight.intensity);
                    oceanMat.SetVector("pointLightColor", lightningScript2.startLight.color);
                }
                if(lightningScript1 != null && !LightningOne.gameObject.activeInHierarchy && lightningScript2 != null && !LightningTwo.gameObject.activeInHierarchy)
                {
                    oceanMat.SetFloat("pointLightPower", 0);                    
                }

                if (WeatherDensity)
                {// && Application.isPlaying) { //v3.4.3

                    if (cloudType == CloudPreset.ClearDay)
                    {
                        Coverage = Mathf.Lerp(Coverage, ClearDayCoverage, 0.8f * Speed_factor * Time_delta + modifier);
                        Horizon = Mathf.Lerp(Horizon, ClearDayHorizon, Time_delta + 0.05f);//v3.4.3 - v3.4.5
                        EnableLightning = false;
                    }
                    if (cloudType == CloudPreset.Storm)
                    {
                        Coverage = Mathf.Lerp(Coverage, StormCoverage, 0.8f * Speed_factor * Time_delta + modifier);
                        Horizon = Mathf.Lerp(Horizon, StormHorizon, Time_delta + 0.05f);//v3.4.3 - v3.4.5
                        EnableLightning = true;
                    }

                }

			}
	}
        

	// Update is called once per frame
	void Update () {

			//v3.4
			if (MultiQuadClouds) {
				if (!SideClouds.gameObject.activeInHierarchy) {
					SideClouds.gameObject.SetActive (true);
				}
				if (SideClouds != null && cloudSidesMat != null) {
					SideClouds.localScale = MultiQuadScale;
					SideClouds.position = new Vector3(SideClouds.position.x,MultiQuadHeights.x,SideClouds.position.z);
					cloudSidesMat.SetFloat ("_CutHeight", MultiQuadHeights.y);
				}
			} else {
				if (SideClouds != null && SideClouds.gameObject.activeInHierarchy) {
					SideClouds.gameObject.SetActive (false);
				}
			}

			if (MultiQuadAClouds) {
				if (!SideAClouds.gameObject.activeInHierarchy) {
					SideAClouds.gameObject.SetActive (true);
				}
				if (SideAClouds != null && cloudSidesAMat != null) {
					SideAClouds.localScale = MultiQuadAScale;
					SideAClouds.position = new Vector3(SideAClouds.position.x,MultiQuadAHeights.x,SideAClouds.position.z);
					cloudSidesAMat.SetFloat ("_CutHeight", MultiQuadAHeights.y);
				}
			} else {
				if (SideAClouds != null && SideAClouds.gameObject.activeInHierarchy) {
					SideAClouds.gameObject.SetActive (false);
				}
			}

			if (MultiQuadBClouds) {
				if (!SideBClouds.gameObject.activeInHierarchy) {
					SideBClouds.gameObject.SetActive (true);
				}
				if (SideBClouds != null && cloudSidesBMat != null) {
					SideBClouds.localScale = MultiQuadBScale;
					SideBClouds.position = new Vector3(SideBClouds.position.x,MultiQuadBHeights.x,SideBClouds.position.z);
					cloudSidesBMat.SetFloat ("_CutHeight", MultiQuadBHeights.y);
				}
			} else {
				if (SideBClouds != null && SideBClouds.gameObject.activeInHierarchy) {
					SideBClouds.gameObject.SetActive (false);
				}
			}

			//v3.4.3
			if (MultiQuadCClouds) {
				if (!SideCClouds.gameObject.activeInHierarchy) {
					SideCClouds.gameObject.SetActive (true);
				}
				if (SideCClouds != null && cloudSidesCMat != null) {
					SideCClouds.localScale = MultiQuadCScale;
					SideCClouds.position = new Vector3(SideCClouds.position.x,MultiQuadCHeights.x,SideCClouds.position.z);
					cloudSidesCMat.SetFloat ("_CutHeight", MultiQuadCHeights.y);
				}
			} else {
				if (SideCClouds != null && SideCClouds.gameObject.activeInHierarchy) {
					SideCClouds.gameObject.SetActive (false);
				}
			}

			if (OneQuadClouds) {
				if (!RotClouds.gameObject.activeInHierarchy) {
					RotClouds.gameObject.SetActive (true);
				}
				if (RotClouds != null && cloudRotMat != null) {
					RotClouds.localScale = OneQuadScale;
					RotClouds.position =  new Vector3(RotClouds.position.x,OneQuadHeights.x,RotClouds.position.z);
					cloudRotMat.SetFloat("_CutHeight",OneQuadHeights.y);
				}
			} else {
				if (RotClouds != null && RotClouds.gameObject.activeInHierarchy) {
					RotClouds.gameObject.SetActive (false);
				}
			}

			if (DomeClouds) {
				if (!FlatBedClouds.gameObject.activeInHierarchy) {
					FlatBedClouds.gameObject.SetActive (true);
				}
				if (FlatBedClouds != null && cloudFlatMat != null) {
					FlatBedClouds.localScale = DomeScale;
					FlatBedClouds.position =  new Vector3(FlatBedClouds.position.x,DomeHeights.x,FlatBedClouds.position.z);
					cloudFlatMat.SetFloat ("_CutHeight",DomeHeights.y);
				}
			} else {
				if (FlatBedClouds != null && FlatBedClouds.gameObject.activeInHierarchy) {
					FlatBedClouds.gameObject.SetActive (false);
				}
			}

            //v3.0
            if (Sun != null)
            {
                if (cloudFlatMat != null && DomeClouds)
                {
                    UpdateCloudParams(false, cloudFlatMat);
                }
                if (cloudSidesMat != null && SideClouds)
                {
                    UpdateCloudParams(false, cloudSidesMat);
                }
                if (cloudSidesAMat != null && SideAClouds)
                {
                    UpdateCloudParams(false, cloudSidesAMat);
                }
                if (cloudSidesBMat != null && SideBClouds)
                {
                    UpdateCloudParams(false, cloudSidesBMat);
                }
                if (cloudSidesCMat != null && SideCClouds)
                {   //v3.4.3
                    UpdateCloudParams(false, cloudSidesCMat);
                }
                if (cloudRotMat != null && OneQuadClouds)
                {
                    UpdateCloudParams(false, cloudRotMat);
                }
            }
			//RAINBOW

			if (rainbowMat != null) {	

				Color RainbowC = rainbowMat.GetColor ("_Color");
				if (!Application.isPlaying) {
					//v3.4.5
					float IntensityR = rainboxMaxIntensity;
					if (autoRainbow) {
						
							IntensityR = rainboxMinIntensity;
						
					} 
					rainbowMat.SetColor ("_Color", new Color(RainbowC.r,RainbowC.g,RainbowC.b,IntensityR));
				} else {					

					float IntensityR = rainboxMaxIntensity;			

					rainbowMat.SetColor ("_Color", Color.Lerp (RainbowC, new Color(RainbowC.r,RainbowC.g,RainbowC.b,IntensityR), Time.deltaTime*0.001f*rainbowApperSpeed + 0.002f));
				}
			}



		if (Application.isPlaying) {

				if (EnableLightning) {
					if (LightningOne == null) {
						LightningOne = Instantiate (LightningPrefab).transform;
						LightningOne.gameObject.SetActive (false);
                        lightningScript1 = LightningOne.gameObject.GetComponentInChildren<ChainLightning_SKYMASTER>(true);//HDRP
                    }
					if (LightningTwo == null) {
						LightningTwo = Instantiate (LightningPrefab).transform;
						LightningTwo.gameObject.SetActive (false);
                        lightningScript2 = LightningOne.gameObject.GetComponentInChildren<ChainLightning_SKYMASTER>(true);//HDRP
                    }

                    

					//move around
					if (LightningBox != null) {
						if (Time.fixedTime - last_lightning_time > lightning_every - Random.Range (-lightning_rate_offset, lightning_rate_offset)) {

							Vector2 MinMaXLRangeX = LightningBox.position.x * Vector2.one + (LightningBox.localScale.x / 2) * new Vector2 (-1, 1);
							Vector2 MinMaXLRangeY = LightningBox.position.y * Vector2.one + (LightningBox.localScale.y / 2) * new Vector2 (-1, 1);
							Vector2 MinMaXLRangeZ = LightningBox.position.z * Vector2.one + (LightningBox.localScale.z / 2) * new Vector2 (-1, 1);

							LightningOne.position = new Vector3 (Random.Range (MinMaXLRangeX.x, MinMaXLRangeX.y), Random.Range (MinMaXLRangeY.x, MinMaXLRangeY.y), Random.Range (MinMaXLRangeZ.x, MinMaXLRangeZ.y));
							if (Random.Range (0, WeatherSeverity + 1) == 1) { 
								//do nothing
							}else{
								LightningOne.gameObject.SetActive (true);
							}

							LightningTwo.position = new Vector3 (Random.Range (MinMaXLRangeX.x, MinMaXLRangeX.y), Random.Range (MinMaXLRangeY.x, MinMaXLRangeY.y), Random.Range (MinMaXLRangeZ.x, MinMaXLRangeZ.y));
							if (Random.Range (0, WeatherSeverity + 1) == 1) { 
								//do nothing
							} else {
								LightningTwo.gameObject.SetActive (true);
							}

							last_lightning_time = Time.fixedTime;
						}else {
							if (Time.fixedTime - last_lightning_time > max_lightning_time) {
								if (LightningOne != null) {	
									if (LightningOne.gameObject.activeInHierarchy) {
										LightningOne.gameObject.SetActive (false);
									}
								}
								if (LightningTwo != null) {	
									if (LightningTwo.gameObject.activeInHierarchy) {
										LightningTwo.gameObject.SetActive (false);
									}
								}
							}
						}
					}
				} else {
					if (LightningOne != null) {	
						if (LightningOne.gameObject.activeInHierarchy) {
							LightningOne.gameObject.SetActive (false);
						}
					}
					if (LightningTwo != null) {	
						if (LightningTwo.gameObject.activeInHierarchy) {
							LightningTwo.gameObject.SetActive (false);
						}
					}
				}

				if (rotateWithCamera && Camera.main != null) {//v3.4.3
					if (RotClouds != null && RotClouds.gameObject.activeInHierarchy) {
						
						if (!EnableInsideClouds) {

							//v3.4.3
							//Vector3 prevScale = Vector3.one;
							if (RotCloudsSPlane != null && Application.isPlaying) {
								RotCloudsSPlane.parent = null;
								if (!RotCloudsSPlane.gameObject.activeInHierarchy) {
									RotCloudsSPlane.gameObject.SetActive (true);
								}
								//prevScale = RotCloudsSPlane.localScale;
							}

							if (LerpRot) {
								RotClouds.eulerAngles = new Vector3 (RotClouds.eulerAngles.x, Mathf.Lerp (RotClouds.eulerAngles.y, Camera.main.transform.eulerAngles.y, Time.deltaTime * 20), RotClouds.eulerAngles.z);
							} else {
								RotClouds.eulerAngles = new Vector3 (RotClouds.eulerAngles.x, Camera.main.transform.eulerAngles.y, RotClouds.eulerAngles.z);

							}
						} else {
							float Desired_rot = 0;
							Transform Cam_transf = Camera.main.transform;
							float Ydiff = Cam_transf.position.y - 100;
							if (Ydiff > 0) {
								Desired_rot = (Ydiff / 10) - Cam_transf.eulerAngles.x;
								Debug.Log (Desired_rot);
							}

							RotClouds.eulerAngles = new Vector3 (Desired_rot, Cam_transf.eulerAngles.y, RotClouds.eulerAngles.z);
						}
					} else {
						if (RotCloudsSPlane != null && RotCloudsSPlane.gameObject.activeInHierarchy) {
							RotCloudsSPlane.gameObject.SetActive (false);
						}
					}
					if (SideClouds != null && SideClouds.gameObject.activeInHierarchy) {
						if (!EnableInsideClouds) {

							//v3.4.3
							//Vector3 prevScale = Vector3.one;
							if (SideSPlane != null && Application.isPlaying) {
								SideSPlane.parent = null;
								if (!SideSPlane.gameObject.activeInHierarchy) {
									SideSPlane.gameObject.SetActive (true);
								}
								//prevScale = SideSPlane.localScale;
							}

							if (LerpRot) {
								SideClouds.eulerAngles = new Vector3 (SideClouds.eulerAngles.x, Mathf.Lerp (SideClouds.eulerAngles.y, Camera.main.transform.eulerAngles.y, Time.deltaTime * 20), SideClouds.eulerAngles.z);
							} else {
								SideClouds.eulerAngles = new Vector3 (SideClouds.eulerAngles.x, Camera.main.transform.eulerAngles.y, SideClouds.eulerAngles.z);
							}


						} 
					} else {
						//v3.4.3
						if (SideSPlane != null && SideSPlane.gameObject.activeInHierarchy) {
							SideSPlane.gameObject.SetActive (false);
						}
					}

					//v3.4.3
					if (rotateMultiQuadC && SideCClouds != null && SideCClouds.gameObject.activeInHierarchy) {
						if (!EnableInsideClouds) {

							//v3.4.3
							//Vector3 prevScale = Vector3.one;
							if (SideCSPlane != null && Application.isPlaying) {
								SideCSPlane.parent = null;
								if (!SideCSPlane.gameObject.activeInHierarchy) {
									SideCSPlane.gameObject.SetActive (true);
								}
								//prevScale = SideSPlane.localScale;
							}

							if (LerpRot) {
								SideCClouds.eulerAngles = new Vector3 (SideCClouds.eulerAngles.x, Mathf.Lerp (SideCClouds.eulerAngles.y, Camera.main.transform.eulerAngles.y, Time.deltaTime * 20), SideCClouds.eulerAngles.z);
							} else {
								SideCClouds.eulerAngles = new Vector3 (SideCClouds.eulerAngles.x, Camera.main.transform.eulerAngles.y, SideCClouds.eulerAngles.z);
							}
                            
						} 
					} else {
						//v3.4.3
						if (SideCSPlane != null && SideCSPlane.gameObject.activeInHierarchy) {
							SideCSPlane.gameObject.SetActive (false);
						}
					}
				}


				if (followPlayer && Hero != null) {//v3.4.3
					SideClouds.position = new Vector3(Hero.position.x,SideClouds.position.y,Hero.position.z);
					SideAClouds.position = new Vector3(Hero.position.x,SideAClouds.position.y,Hero.position.z);
					SideBClouds.position = new Vector3(Hero.position.x,SideBClouds.position.y,Hero.position.z);
					SideCClouds.position = new Vector3(Hero.position.x,SideCClouds.position.y,Hero.position.z);
					FlatBedClouds.position = new Vector3(Hero.position.x,FlatBedClouds.position.y,Hero.position.z);
					RotClouds.position = new Vector3(Hero.position.x,RotClouds.position.y,Hero.position.z);

					//v3.4.3
					if (SideSPlane != null) {
						SideSPlane.position = new Vector3(Hero.position.x,SideSPlane.position.y,Hero.position.z);
					}
					if (SideCSPlane != null) {
						SideCSPlane.position = new Vector3(Hero.position.x,SideCSPlane.position.y,Hero.position.z);
					}
					if (RotCloudsSPlane != null) {
						RotCloudsSPlane.position = new Vector3(Hero.position.x,RotCloudsSPlane.position.y,Hero.position.z);
					}
				}

		}
	}
		public Transform LightningBox; 

		float last_lightning_time = 0;
		public float lightning_every = 15;
		public float max_lightning_time = 2;
		public float lightning_rate_offset = 5;
		Transform LightningOne;
		Transform LightningTwo;

        public ChainLightning_SKYMASTER lightningScript1;
        public ChainLightning_SKYMASTER lightningScript2;

        //UPDATE SCATTER
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
