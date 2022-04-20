using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.SKYMASTER {
public class VolumeClouds_SM : MonoBehaviour {

		//v3.4.6
		ParticleSystem ShurikenParticle;

		//v3.5 - Shuriken particles
		bool useShuriken = true;//v3.4.6
		ParticleSystemRenderer PrenderS;	

		//v3.3a
		public float renewHeightPercent = 99.2f;
		public float renewFadeOutSpeed = 1f;
		public float renewFadeInSpeed = 1f;

		//v3.2 - box cloud definition
		public bool boxClouds = false;//
		public GameObject BoxesHolder;

		//v3.1 - free fall
		public bool renewAboveHeight = false;
		public float renewHeight = 500;

		//v3.0 - clouds
		public bool shadows_created = false;

		//v3.0 - wind
		public bool DecoupledWind = false;
		public bool DecoupledColor = false;
		public bool DifferentialMotion = false;
		public float MaxDiffSpeed = 0.02f;//max multiplier of wind motion in differential motion option
		public float MaxDiffOffset = 0;// above 0 will move all clouds 

		//v3.0 - extra controls
		public float LightShaderModifier = 0;
		public float minLightShaderModifier = 0;
		public float GlowShaderModifier = 0;
		public float IntensityShaderModifier = 0;
		public float ModifierApplicationSpeed = 0.1f;
		public float ModifierApplMinSpeed = 0.1f;

		//v3.0-alt clouds
		//[HideInInspector]
		//public bool Use_quads = false;//instantiate quads for clouds //v3.4.6
		[HideInInspector]
		public GameObject Quad;//quad object
		[HideInInspector]
		public List<Transform> Quads;
		public Light SunLight;
		public Light MoonLight;

		//v3.0
		public bool Accumulate = false;//create accumulation formations
		public float AccumPercent = 0.04f;//percent of the divider number that will accumulate (e.g. 0.1 in 100 centers means center will gather in groups of 10)
		public float AccumFactor = 1f;

		public bool StabilizeRoll = false;//stop cloud billboard rotation on camera roll
		public bool StableRollMethod2 = false;//stop cloud billboard rotation on camera roll
		public bool StableRollAllAxis = false;//stabilize also in Y axis rolls

		//ParticleRenderer Prender;		//v3.4.6
		Vector3 prev_cam_for;
		bool sortedInit = false;
		public float angle1=35;
		public float angle2=18;
		public float NearCloudDist = 50;
		public float CloudBedToThisDist = 0;		
		//bool has_sorted=false;

		//v2.4
		public bool Divide_lightning = false;//devide lighting per N cloud centers
		public int LightningDivider = 5;
		public bool AutoScale= false;//automatically chose best scale paramters for lighting, per time of day

	//v2.3 - Destroy after fadeout
		public bool DestroyOnfadeOut = false;
		public bool FadeOutOnBoundary = false;
		public bool ScatterShaderUpdate = false;
		public Material ScatterMat;
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

	//v2.2 - Vertical formation
	public bool VerticalForm = false;
	public float YScaleDiff = 0.6f;
	public Vector3 MaxCloudDist = new Vector3(10,20,0);
	public float Yspread = 2; //spread partciles more to y axis than initial particle assigned positions
		public Color HeightTint;

	//v2.2 - Smooth fade in-out
	public bool SmoothIn = false;//start smooth appearence of clouds
	public bool SmoothOut = false;//start smooth vanishing of clouds
		public  float SmoothInSpeed = 0;
		public  float SmoothoutSpeed = 0;
		public float SmoothInRate = 0.15f;
		public float SmoothOutRate = 0.15f;
		public float max_fade_speed = 2.5f;

		public float max_smooth_out_time = 1f;//v3.0
		float current_smooth_out_time;

		public float fade_in_time = 2.5f;
		public float current_fadein_time = 0;

	//v2.2 - LOD (move particles up by a big ammount if camera is looking directly at cloud bed height and particle is far away, to reduce overdraw)
		public bool EnableLOD = false;
		public float LOD_send_height = -10000; //where to send LODed particles, so they are not rendered
		public int[] ParticlesLODed;//keep a flag on LODed particles, 1 == loded
		public float LodMaxYDiff = 240;
		public float LodMaxHDiff = 1700;
		public float LODFadeInSpeed = 5;
		public float LODFadeOutSpeed = 5;
		public float LODMinDist = 80;//distance where close to camera particles will become transparent and smaller

		public bool CloudWaves = false;//wave fading
		public float WaveFreq = 0.22f;

	//v1.7
	public bool Moon_light=false; // apply moon light at night
	public Color Moon_light_color = Color.gray;
	public Color Moon_dark_color = Color.gray*0.9f;
	public bool Override_init_color = false;
	public Color Override_color= Color.gray;

	public bool Day_cycle = false; // grab time from SkyMasterManager and assign colors defined below
	public SkyMasterManager SkyManager;
	public Color Day_base_col = Color.grey;
	public Color Day_sun_col = Color.white; // this is used if "grab_sun" parameter is not active
	public Color Dawn_base_col = new Color(155f/255f,115f/255f,130f/255f,255f/255f);//250,165,140
	public Color Dawn_sun_col = new Color(245f/255f,195f/255f,205f/255f,255f/255f);//255,236,193
		public Color Dusk_base_col = new Color(66f/255f,0f/255f,9f/255f,255f/255f);//new Color(130f/255f,0f/255f,35f/255f,255f/255f);//244,51,51
	public Color Dusk_sun_col = new Color(233f/255f,74f/255f,117f/255f,255f/255f);//245,105,130
	public Color Night_base_col = Color.grey;
	public Color Night_moon_col = Color.white;

	//public bool Add_rain = false;
	public bool Add_shadows = false;
	List<Transform> Shadow_planes = new List<Transform>();

	//v1.6
	int alternate = 1;
	public bool Use2DCheck = false;
	public int max_divider = 1;
	float Cloud_update_current;
	public float Cloud_spread_delay = 1;//delay spread to frames optimization one time, to gather intital color

	//v1.2.6
	public int method = 3;//motion methods //v1.6 changed to 3, use this one
	public bool Turbulent = false;
	public bool Flatten_below = false;//flatten cloud underside
	
	//SUN SHAFTS
	public List<Vector3> ShaftCenters;
	public List<Transform> SunShafts;
	public bool Beam_Occlusion = false;//volume based beam occlusion, use alone or together with ray casting colliders
	//public Material SunBeamMat;
	public bool Smooth_mat_trans=false;
	
	public float Appear_Beam_speed=2;
	public float Disappear_Beam_speed=2;
	
	public List<Vector3> ShaftScale;
	public float Beam_length=300;
	public bool Scale_beams=false;
	public bool Scale_on_collision=false;
	public float Offset_col_factor = 1; //use this to offset collided beam upwards
	
	public bool Diminish_beams = false;//on demand scale down and make beams dissapear, for night time
	public bool Override_sun = false;//use a different sun color, for night time
	public float Cut_height=0;
	public bool Restore_on_bound=false;
	public bool Disable_on_bound=false;
	public float Bound = 9000;
	public bool destroy_on_end=false;
	public bool clone_on_end=false;//re-start clouds
	public bool cloned=false;//v3.0
	
	public List<Vector3> Centers; //initialize center in start and update them to get right lighting
	public List<Vector3> Centers_Init; 
	public float speed =0.5f;
	public float multiplier = 5f;
	public Vector3 wind = new Vector3(1,1,1);
	
	//For Future Updates
	public bool Get_wind_direction;
	public GameObject Wind_holder;
	public GameObject Rain_holder;
	public GameObject Lightning_holder;
	public GameObject Shadow_holder;
	
	public float max_bed_corner=1000;
	public float min_bed_corner=0;
	public float cloud_bed_heigh = 500;
	
	public Color MainColor;
	public Color SunColor;
	
	public int divider = 10;//number of centers to use, divide particles by this nunmber for each cloud particle number
	public float Sun_exposure = 2; //Further enchance sun light color
	
	public Transform sun_transf;
	Transform keep_sun_transf;
	public Transform moon_transf;//v1.7 - add moon lighting
	Transform  Cam_transf;
	Transform  This_transf;
	Color MainColor_init;
	float start_beam_transp=0.27f;
	
	//v3.4.5a - convert clouds to Shuriken
	public void convertToShuriken(){	}

	//v3.0
	public void ScaleClouds (float WorldScale,float VCloudCoverFac,float VCloudSizeFac, float VCloudCSizeFac) {
			//Scaling
			//v3.4.6
//			ParticleEmitter Cloud_Particles = this.gameObject.GetComponent<ParticleEmitter>();	
//			if(Cloud_Particles!=null){
//				Cloud_Particles.minSize = Cloud_Particles.minSize*(WorldScale/20)*VCloudSizeFac;
//				Cloud_Particles.maxSize = Cloud_Particles.maxSize*(WorldScale/20)*VCloudSizeFac;
//			}

			//v3.4.6
			ParticleSystem Cloud_Particles = this.gameObject.GetComponent<ParticleSystem>();
			if(Cloud_Particles != null){
				ParticleSystem.MainModule MainMod = Cloud_Particles.main; //v3.4.9
				//Cloud_Particles.startSize = Cloud_Particles.startSize*(WorldScale/20)*VCloudSizeFac;
				MainMod.startSizeMultiplier = MainMod.startSizeMultiplier*(WorldScale/20)*VCloudSizeFac;  //v3.4.9

				//Cloud_Particles.maxSize = Cloud_Particles.maxSize*(WorldScale/20)*VCloudSizeFac;
			}

			if (VCloudCoverFac > 0 && (int)(divider * VCloudCoverFac) > 10) {
				divider = (int)(divider * VCloudCoverFac);
			}
			
			min_bed_corner = min_bed_corner*(WorldScale/20);
			max_bed_corner = max_bed_corner*(WorldScale/20);
			cloud_bed_heigh = cloud_bed_heigh*(WorldScale/20);
			
			cloud_scale = cloud_scale*(WorldScale/20);
			global_cloud_scale = global_cloud_scale*(WorldScale/20);
			
			YScaleDiff = YScaleDiff*(WorldScale/20);
			Yspread = Yspread*(WorldScale/20);
			LODMinDist = LODMinDist*(WorldScale/20);
			LodMaxHDiff = LodMaxHDiff*(WorldScale/20);
			LodMaxYDiff = LodMaxYDiff*(WorldScale/20);
			
			cloud_min_scale = cloud_min_scale*(WorldScale/20);
			cloud_max_scale = cloud_max_scale *(WorldScale/20) * VCloudCSizeFac;
	}


	// Use this for initialization
	void Start () {

			//Debug.Log (init_centers);

			//v3.0
			if (SkyManager != null) {
				SunLight = SkyManager.SUN_LIGHT.GetComponent<Light> ();
				MoonLight = SkyManager.MOON_LIGHT.GetComponent<Light> ();
			}

			if(SkyManager != null && ScatterMat != null ){
					
					//v3.0 - new automatic TOD
					bool is_DayLight  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 0 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time > ( 9.0f + SkyManager.Shift_dawn) & SkyManager.Current_Time <= (21.9f + SkyManager.Shift_dawn));

					//if(SkyManager.Current_Time > (9+ SkyManager.Shift_dawn) & SkyManager.Current_Time <=(21.9f+ SkyManager.Shift_dawn)){
					if(is_DayLight ){
						ScatterMat.SetVector ("sunPosition", -SkyManager.SunObj.transform.forward.normalized);
						ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),SunLight.color,0.5f*Time.deltaTime));
					}else{
						ScatterMat.SetVector ("sunPosition", -SkyManager.MoonObj.transform.forward.normalized);
						ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),MoonLight.color,0.5f*Time.deltaTime));
					}

			}


			//v3.5
			if (this.GetComponent<ParticleSystem> () != null) {
				ScatterMat = this.GetComponent<ParticleSystemRenderer>().material; 
				PrenderS = this.gameObject.GetComponent<ParticleSystemRenderer> ();
				ShurikenParticle = this.GetComponent<ParticleSystem> ();
			}
			if (PrenderS != null) {
				PrenderS.sortMode = ParticleSystemSortMode.Distance;// = ParticleRenderMode.SortedBillboard;
				//PrenderS.renderMode = ParticleSystemRenderMode.Billboard;
			}


			//v3.0 //v3.4.6
//			if (this.GetComponent<ParticleRenderer> () != null) {
//				ScatterMat = this.GetComponent<ParticleRenderer>().material; 
//				Prender = this.gameObject.GetComponent<ParticleRenderer> ();
//			}
//			if (Prender != null) {
//				Prender.particleRenderMode = ParticleRenderMode.SortedBillboard;
//			}
			prev_cam_for = Camera.main.transform.forward;

			//v2.2
			//check even cloud centers
			if (divider % 2 == 1) {
				divider += 1;//make even
			}

		This_transf = this.transform;
		Cam_transf = Camera.main.transform;
		Prev_cam_rot = Cam_transf.eulerAngles;
		//add first
		Centers = new List<Vector3>();

		if (cloned) {
			cloned = false;
			Centers = Centers_Init;
			Debug.Log("ppp");
		}

		Centers_Init = new List<Vector3>();
		Centers.Add(This_transf.position);
		Centers_Init.Add(This_transf.position);
		
		//variant cloud sizes
		Scale_per_cloud = new List<float>();
		
		if(Variant_cloud_size){
			Scale_per_cloud.Add(Random.Range(cloud_min_scale,cloud_max_scale));
		}else{
			Scale_per_cloud.Add(1);
		}
		
			//v3.2
			Collider[] Boxes = null;
			if (boxClouds && BoxesHolder!=null && BoxesHolder.transform.childCount > 0) {
				Boxes = BoxesHolder.GetComponentsInChildren<Collider> ();
			}

		int accum_count = 0;
		for(int i=0;i<divider-1;i++){

			//v3.0
			if(Accumulate){
					if(accum_count==0){
						Centers.Add(new Vector3(Random.Range(min_bed_corner,max_bed_corner)+This_transf.position.x,Random.Range(This_transf.position.y,This_transf.position.y+cloud_bed_heigh),Random.Range(min_bed_corner,max_bed_corner)+This_transf.position.z));
						accum_count++;
					}else{
						Centers.Add(new Vector3(Random.Range(cloud_min_scale*cloud_scale*AccumFactor,cloud_max_scale*cloud_scale*AccumFactor)+Centers[i-1].x,
						                        Random.Range(This_transf.position.y,Centers[i-1].y+cloud_bed_heigh),
						                        Random.Range(cloud_min_scale*cloud_scale*AccumFactor,cloud_max_scale*cloud_scale*AccumFactor)+Centers[i-1].z));
//						Centers.Add(new Vector3(Random.Range(min_bed_corner/52,max_bed_corner/52)+Centers[i-1].x,
//						                        Random.Range(This_transf.position.y,Centers[i-1].y+cloud_bed_heigh),
//						                        Random.Range(min_bed_corner/52,max_bed_corner/52)+Centers[i-1].z));
						accum_count++;
						if(accum_count > (divider*AccumPercent)){
							accum_count = 0;
						}
					}
			}else{
					//v3.2
					if (boxClouds && BoxesHolder!=null && BoxesHolder.transform.childCount > 0) {

						if (Boxes.Length-1 < i) {
							Centers.Add (new Vector3 (Random.Range (min_bed_corner, max_bed_corner) + This_transf.position.x, Random.Range (This_transf.position.y, This_transf.position.y + cloud_bed_heigh), Random.Range (min_bed_corner, max_bed_corner) + This_transf.position.z));
						} else {
							Centers.Add (Boxes [i].transform.position);
						}
					} else {
						Centers.Add (new Vector3 (Random.Range (min_bed_corner, max_bed_corner) + This_transf.position.x, Random.Range (This_transf.position.y, This_transf.position.y + cloud_bed_heigh), Random.Range (min_bed_corner, max_bed_corner) + This_transf.position.z));
					}
			}
			Centers_Init.Add(Centers[i]);

			if(Variant_cloud_size){
				Scale_per_cloud.Add(Random.Range(cloud_min_scale,cloud_max_scale));
			}else{
				Scale_per_cloud.Add(1);
			}
		}

			//v3.2
			if (boxClouds && BoxesHolder!=null && BoxesHolder.transform.childCount > 0) {
				BoxesHolder.SetActive (false);
			}

			//v3.0-alt clouds //v3.4.6
//			if(Use_quads){
//				int psize1 = (int)GetComponent<ParticleEmitter> ().minEmission;
//				GameObject quads = new GameObject();
//				quads.AddComponent<ControlCombineChildrenSKYMASTER>().MakeActive=true;
//				//quads.GetComponent<ControlCombineChildrenSKYMASTER>().MakeActive=true;
//				quads.GetComponent<ControlCombineChildrenSKYMASTER>().Auto_Disable=true;
//				quads.transform.parent = this.transform;
//				for (int i=0; i<Centers.Count; i++) {
//					for (int j=0; j<psize1; j++) {
//						GameObject Quady = (GameObject)Instantiate(Quad,Vector3.zero,Quaternion.AngleAxis(Random.Range(1,360),new Vector3(Random.Range(-360,360),Random.Range(-360,360),Random.Range(-360,360))));
//						Quady.transform.LookAt(Camera.main.transform, Vector3.up);
//						Quady.transform.parent = quads.transform;
//						Quads.Add(Quady.transform);
//					}
//				}
//			}

		//Centers_Init = Centers;
		Prev_time = Time.fixedTime;

			if (sun_transf != null) {
				Sun_Light = sun_transf.gameObject.GetComponent<Light> ();
			} else {
				Debug.Log("Please add the sun object to Sun_Trasf script parameter");//v3.0
			}

		if(Grab_sun_color){
			SunColor = Sun_Light.color;
		}

		prev_intensity = Sun_Light.intensity;
		MainColor_init = MainColor;
		
		Shafts_renderes = new List<Renderer>();
		ShaftScale = new List<Vector3>();
		
		Shaft_update_current = Time.fixedTime;
		//v1.6
		Cloud_update_current = Time.fixedTime;
		
			Vector3 fwd = Cam_transf.forward;
		fwd.x = 0.0f;
		fwd.y = 0.0f;
		Quaternion Rot = Quaternion.identity;
		if(fwd != Vector3.zero){
			Rot = Quaternion.LookRotation(fwd);
			prev_Rot=Rot;
		}else{
			Rot = Quaternion.LookRotation(fwd+0.0001f*new Vector3(1,1,1));
			prev_Rot=Rot;
		}

		//v1.7
		keep_sun_transf = sun_transf;

			//v2.2
			//check even cloud centers
//			if (divider % 2 == 1) {
//				divider -= 1;//make even
//			}


			//v3.5
			int psize = 0;
			if (useShuriken) {
				ParticleSystem.MainModule MainMod = GetComponent<ParticleSystem> ().main; //v3.4.9
				psize = (int)MainMod.maxParticles; //v3.4.9

				int SizeP = ((int)(psize / divider)) * (divider);
				MainMod.maxParticles = SizeP; //+ (divider)*1;
				MainMod.maxParticles = SizeP; //+ (divider)*1;
				//}
		//		GetComponent<ParticleSystem> ().ClearParticles ();
				GetComponent<ParticleSystem> ().Emit (SizeP);
				ParticleSystem.Particle[] particles = new ParticleSystem.Particle[SizeP];
				GetComponent<ParticleSystem> ().GetParticles (particles);	//		Particle[] particles = GetComponent<ParticleEmitter> ().particles;

				//v1.7 - init clouds on start
				if (GetComponent<ParticleSystem> () != null) {
//					if (!Sorted) {
//						//LEGACY
//						//Particle[] particles = GetComponent<ParticleEmitter>().particles;
//
//						//find particle number for each center
//						int particles_per_cloud = (int)(particles.Length / divider);
//
//						if (!init_centers) {
//							int count_me = 0;
//
//							//v2.2
//							if (VerticalForm) {
//								for (int j = 0; j < divider / 2; j++) {
//									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
//										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];
//
//										particles [i].color = Global_tint + MainColor;
//
//										int start_C = (i + (divider / 2) * particles_per_cloud);
//										//int end_C = ((i+(divider/2))*particles_per_cloud);
//
//										particles [start_C].position = (particles [start_C].position + new Vector3 (0, Yspread, 0) - This_transf.position) * Scale_per_cloud [j] * YScaleDiff + Centers [j] + MaxCloudDist;
//
//										particles [start_C].color = Color.Lerp (Global_tint + MainColor, HeightTint, HeightTint.a);
//
//										count_me = count_me + 1;
//
//										//v2.2
//										if (SmoothIn) {
//											//zero visibility at start
//											particles [i].color = new Color (particles [i].color.r, particles [i].color.g, particles [i].color.b, 0);
//											particles [start_C].color = new Color (particles [start_C].color.r, particles [start_C].color.g, particles [start_C].color.b, 0);
//										}
//									}
//								}
//							} else {
//								for (int j = 0; j < divider; j++) {
//									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
//										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];
//
//										particles [i].color = Global_tint + MainColor;
//
//										count_me = count_me + 1;
//
//										//v2.2
//										if (SmoothIn) {
//											//zero visibility at start
//											particles [i].color = new Color (particles [i].color.r, particles [i].color.g, particles [i].color.b, 0);
//										}
//									}
//								}
//							}
//							if (count_me > 0) {
//								init_centers = true;
//								Init = true; 
//								GetComponent<ParticleEmitter> ().particles = particles;
//							}
//						}
//					} else 
						if (Sorted) {							///////////////////// SORTED CASE - go per particle, check closest center
						//LEGACY
						//Particle[] particles = GetComponent<ParticleEmitter>().particles;

						//find particle number for each center
						int particles_per_cloud = (int)(particles.Length / divider);

						if (!init_centers) {
							int count_me = 0;

							//v2.2
							if (VerticalForm) {
								int count_shadows = 0;
								for (int j = 0; j < divider / 2; j++) {
									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];

										particles [i].startColor = Global_tint + MainColor;

										int start_C = (i + (divider / 2) * particles_per_cloud);
										//int end_C = ((i+(divider/2))*particles_per_cloud);

										particles [start_C].position = (particles [start_C].position + new Vector3 (0, Yspread, 0) - This_transf.position) * Scale_per_cloud [j] * YScaleDiff + Centers [j] + MaxCloudDist;

										particles [start_C].startColor = Color.Lerp (Global_tint + MainColor, HeightTint, HeightTint.a);

										count_me = count_me + 1;

										//v2.2
										if (SmoothIn) {
											//zero visibility at start
											particles [i].startColor = new Color (particles [i].startColor.r, particles [i].startColor.g, particles [i].startColor.b, 0);
											particles [start_C].startColor = new Color (particles [start_C].startColor.r, particles [start_C].startColor.g, particles [start_C].startColor.b, 0);
										}



										//v3.5
										//particles [i].lifetime = particles [i].startLifetime;
										//particles [i].


										//v3.0-alt clouds
//										if (Use_quads) {
//											//for (int j=0; j < divider/2;j++){
//											Quads [i].position = particles [i].position;
//											Quads [start_C].position = particles [start_C].position;
//											//}
//										}
									}
									//v1.7 - add shadow planes
									if (Add_shadows & !shadows_created) {
										//v3.0
										if (!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)) {
											GameObject ShadowPlane = (GameObject)Instantiate (Shadow_holder, Centers [j], Quaternion.identity);
											ShadowPlane.transform.parent = this.gameObject.transform;
											//ShadowPlane.activeInHierarchy = true;
											ShadowPlane.SetActive (true);
											Shadow_planes.Add (ShadowPlane.transform);
											count_shadows = 0;
										} else {
											count_shadows++;
										}
									}
								}
							} else {

								int count_shadows = 0;
								for (int j = 0; j < divider; j++) {
									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];

										particles [i].startColor = Global_tint + MainColor;

										count_me = count_me + 1;

										//v2.2
										if (SmoothIn) {
											//zero visibility at start
											particles [i].startColor = new Color (particles [i].startColor.r, particles [i].startColor.g, particles [i].startColor.b, 0);
										}
									}
									//v1.7 - add shadow planes
									if (Add_shadows & !shadows_created) {
										//v3.0
										if (!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)) {
											GameObject ShadowPlane = (GameObject)Instantiate (Shadow_holder, Centers [j], Quaternion.identity);
											ShadowPlane.transform.parent = this.gameObject.transform;
											//ShadowPlane.activeInHierarchy = true;
											ShadowPlane.SetActive (true);
											Shadow_planes.Add (ShadowPlane.transform);
											count_shadows = 0;
										} else {
											count_shadows++;
										}
									}
								}
							}
							if (count_me > 0) {
								init_centers = true;
								Init = true; 
								GetComponent<ParticleSystem> ().SetParticles (particles, particles.Length);  //GetComponent<ParticleEmitter> ().particles = particles;
							}
						}			

					}//END if SORTED LEGACY
				}//END CHECK

				//v2.2 - LOD
				ParticlesLODed = new int[particles.Length];
			}
			//v3.4.6
//			 else {
//				psize = (int)GetComponent<ParticleEmitter> ().minEmission;
//			
//				//Debug.Log (psize);
//
//				//if ((psize/divider) - (psize/divider) > 0) {
//				int SizeP = ((int)(psize / divider)) * (divider);
//				GetComponent<ParticleEmitter> ().minEmission = SizeP; //+ (divider)*1;
//				GetComponent<ParticleEmitter> ().maxEmission = SizeP; //+ (divider)*1;
//				//}
//				GetComponent<ParticleEmitter> ().ClearParticles ();
//				GetComponent<ParticleEmitter> ().Emit ();
//				Particle[] particles = GetComponent<ParticleEmitter> ().particles;
//
//
//				//v1.7 - init clouds on start
//				if (GetComponent<ParticleEmitter> () != null) {
//					if (!Sorted) {
//						//LEGACY
//						//Particle[] particles = GetComponent<ParticleEmitter>().particles;
//				
//						//find particle number for each center
//						int particles_per_cloud = (int)(particles.Length / divider);
//				
//						if (!init_centers) {
//							int count_me = 0;
//
//							//v2.2
//							if (VerticalForm) {
//								for (int j = 0; j < divider / 2; j++) {
//									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
//										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];
//									
//										particles [i].color = Global_tint + MainColor;
//
//										int start_C = (i + (divider / 2) * particles_per_cloud);
//										//int end_C = ((i+(divider/2))*particles_per_cloud);
//
//										particles [start_C].position = (particles [start_C].position + new Vector3 (0, Yspread, 0) - This_transf.position) * Scale_per_cloud [j] * YScaleDiff + Centers [j] + MaxCloudDist;
//									
//										particles [start_C].color = Color.Lerp (Global_tint + MainColor, HeightTint, HeightTint.a);
//									
//										count_me = count_me + 1;
//
//										//v2.2
//										if (SmoothIn) {
//											//zero visibility at start
//											particles [i].color = new Color (particles [i].color.r, particles [i].color.g, particles [i].color.b, 0);
//											particles [start_C].color = new Color (particles [start_C].color.r, particles [start_C].color.g, particles [start_C].color.b, 0);
//										}
//									}
//								}
//							} else {
//								for (int j = 0; j < divider; j++) {
//									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
//										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];
//
//										particles [i].color = Global_tint + MainColor;
//
//										count_me = count_me + 1;
//
//										//v2.2
//										if (SmoothIn) {
//											//zero visibility at start
//											particles [i].color = new Color (particles [i].color.r, particles [i].color.g, particles [i].color.b, 0);
//										}
//									}
//								}
//							}
//							if (count_me > 0) {
//								init_centers = true;
//								Init = true; 
//								GetComponent<ParticleEmitter> ().particles = particles;
//							}
//						}
//					} else if (Sorted) {							///////////////////// SORTED CASE - go per particle, check closest center
//						//LEGACY
//						//Particle[] particles = GetComponent<ParticleEmitter>().particles;
//				
//						//find particle number for each center
//						int particles_per_cloud = (int)(particles.Length / divider);
//				
//						if (!init_centers) {
//							int count_me = 0;
//
//							//v2.2
//							if (VerticalForm) {
//								int count_shadows = 0;
//								for (int j = 0; j < divider / 2; j++) {
//									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
//										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];
//									
//										particles [i].color = Global_tint + MainColor;
//									
//										int start_C = (i + (divider / 2) * particles_per_cloud);
//										//int end_C = ((i+(divider/2))*particles_per_cloud);
//									
//										particles [start_C].position = (particles [start_C].position + new Vector3 (0, Yspread, 0) - This_transf.position) * Scale_per_cloud [j] * YScaleDiff + Centers [j] + MaxCloudDist;
//									
//										particles [start_C].color = Color.Lerp (Global_tint + MainColor, HeightTint, HeightTint.a);
//									
//										count_me = count_me + 1;
//
//										//v2.2
//										if (SmoothIn) {
//											//zero visibility at start
//											particles [i].color = new Color (particles [i].color.r, particles [i].color.g, particles [i].color.b, 0);
//											particles [start_C].color = new Color (particles [start_C].color.r, particles [start_C].color.g, particles [start_C].color.b, 0);
//										}
//
//										//v3.0-alt clouds
//										if (Use_quads) {
//											//for (int j=0; j < divider/2;j++){
//											Quads [i].position = particles [i].position;
//											Quads [start_C].position = particles [start_C].position;
//											//}
//										}
//									}
//									//v1.7 - add shadow planes
//									if (Add_shadows & !shadows_created) {
//										//v3.0
//										if (!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)) {
//											GameObject ShadowPlane = (GameObject)Instantiate (Shadow_holder, Centers [j], Quaternion.identity);
//											ShadowPlane.transform.parent = this.gameObject.transform;
//											//ShadowPlane.activeInHierarchy = true;
//											ShadowPlane.SetActive (true);
//											Shadow_planes.Add (ShadowPlane.transform);
//											count_shadows = 0;
//										} else {
//											count_shadows++;
//										}
//									}
//								}
//							} else {
//
//								int count_shadows = 0;
//								for (int j = 0; j < divider; j++) {
//									for (int i = (j * particles_per_cloud); i < ((j + 1) * particles_per_cloud); i++) {
//										particles [i].position = (particles [i].position - This_transf.position) * Scale_per_cloud [j] + Centers [j];
//
//										particles [i].color = Global_tint + MainColor;
//
//										count_me = count_me + 1;
//
//										//v2.2
//										if (SmoothIn) {
//											//zero visibility at start
//											particles [i].color = new Color (particles [i].color.r, particles [i].color.g, particles [i].color.b, 0);
//										}
//									}
//									//v1.7 - add shadow planes
//									if (Add_shadows & !shadows_created) {
//										//v3.0
//										if (!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)) {
//											GameObject ShadowPlane = (GameObject)Instantiate (Shadow_holder, Centers [j], Quaternion.identity);
//											ShadowPlane.transform.parent = this.gameObject.transform;
//											//ShadowPlane.activeInHierarchy = true;
//											ShadowPlane.SetActive (true);
//											Shadow_planes.Add (ShadowPlane.transform);
//											count_shadows = 0;
//										} else {
//											count_shadows++;
//										}
//									}
//								}
//							}
//							if (count_me > 0) {
//								init_centers = true;
//								Init = true; 
//								GetComponent<ParticleEmitter> ().particles = particles;
//							}
//						}			
//
//					}//END if SORTED LEGACY
//				}//END CHECK
//
//				//v2.2 - LOD
//				ParticlesLODed = new int[particles.Length];
//
//			}//END LEGACY CHECK



			//v3.0
			if(ScatterMat != null && ScatterMat.HasProperty("_Glow")){
				IntensityShaderModifier = ScatterMat.GetFloat("_Intensity");
			}
	}
	
	
	
	float Prev_time;
	public float UpdateInterval = 1f;
	bool init_centers = false;
	Vector3 Prev_cam_rot;
	public float color_speed = 0.5f;
	public float cloud_scale = 50;
	public float global_cloud_scale = 1000;
	public Color Global_tint= new Color(0,0,0,0);
	
	public bool Variant_cloud_size = false;
	List<float> Scale_per_cloud; // variant scale per cloud holder
	public float cloud_min_scale=0.8f;
	public float cloud_max_scale=2f;
	
	public bool Sorted=false;
	bool Init = false;
	int Init_color = 0; //DONT lerp the first color !!!
	public float Delay_lerp = 10000;
	public bool Sun_dist_on=false;
	Light Sun_Light;
	public bool Grab_sun_color;
	public bool Grab_sun_intensity;
	public bool Grab_sun_angle;
	public float Day_light_Intensity = 3;
	public float Sun_angle_influence = 0.01f;
	
	public bool Look_at_camera=false; // make beams look to camera position, with an offset
	public Vector3 Beams_camera_target_offset = new Vector3(0,0,0);
	
	float prev_intensity;
	bool Got_shaft_items=false;//grab shafts after combined into one
	List<Renderer> Shafts_renderes;
	
	float Shaft_update_current;
	public float Occlusion_sort_delay = 1;
	Quaternion prev_Rot;
	public bool Debug_mode=false;

		//v2.2 - sound
		public bool Use_audio;
		public AudioClip RainSound;//fade clip in/out
		public AudioSource CloudAudioSource;
		public float AudiofadeSpeed = 0.025f;
		public float MaxSoundVol = 0.1f;
	
	void Update () {

			//v3.0
			if (StabilizeRoll) {
				
				//if camera rotates > 180, do sort again
				if (Vector3.Angle (prev_cam_for, Cam_transf.forward) > 11
				    & (Cam_transf.eulerAngles.z < 5 | Cam_transf.eulerAngles.z > 355)) {
					
					prev_cam_for = Cam_transf.forward;
					sortedInit = false;
					//Debug.Log("resort");
				}
				//v3.4.6
//				if (Prender != null) {
//					float angle = Vector3.Angle (Cam_transf.forward, new Vector3 (Cam_transf.forward.x, 0, Cam_transf.forward.z));
//					
//					float angleR = Vector3.Angle (Cam_transf.right, new Vector3 (Cam_transf.right.x, 0, Cam_transf.right.z));
//					//	Debug.Log (Vector3.Angle (MainCam.forward, new Vector3 (MainCam.forward.x, 0, MainCam.forward.z)));
//					
//					if (!sortedInit) {
//						sortedInit = true;//sort at first
//						if (Prender.particleRenderMode != ParticleRenderMode.SortedBillboard) {
//							Prender.particleRenderMode = ParticleRenderMode.SortedBillboard;
//						}
//					} else {
//						
//						if (Mathf.Abs (Cam_transf.position.y - (This_transf.position.y + CloudBedToThisDist)) < NearCloudDist | (angle > angle1 & angleR < angle2)) {
//							if (Prender.particleRenderMode != ParticleRenderMode.SortedBillboard) {
//								Prender.particleRenderMode = ParticleRenderMode.SortedBillboard;
//							}
//							//Debug.Log ("change");
//						} else {
//							if (Prender.particleRenderMode != ParticleRenderMode.VerticalBillboard) {
//								Prender.particleRenderMode = ParticleRenderMode.VerticalBillboard;
//							}
//						}
//						
//					}
//				}


				//v3.5
				if (PrenderS != null) {
					float angle = Vector3.Angle (Cam_transf.forward, new Vector3 (Cam_transf.forward.x, 0, Cam_transf.forward.z));

					float angleR = Vector3.Angle (Cam_transf.right, new Vector3 (Cam_transf.right.x, 0, Cam_transf.right.z));
					//	Debug.Log (Vector3.Angle (MainCam.forward, new Vector3 (MainCam.forward.x, 0, MainCam.forward.z)));

					if (!sortedInit) {
						sortedInit = true;//sort at first
						if (PrenderS.sortMode != ParticleSystemSortMode.Distance ) {
							PrenderS.sortMode = ParticleSystemSortMode.Distance;
							//PrenderS.renderMode = ParticleSystemRenderMode.Billboard;
						}
					} else {

						if (Mathf.Abs (Cam_transf.position.y - (This_transf.position.y + CloudBedToThisDist)) < NearCloudDist | (angle > angle1 & angleR < angle2)) {
							if (PrenderS.sortMode != ParticleSystemSortMode.Distance) {
								PrenderS.sortMode = ParticleSystemSortMode.Distance;
								//PrenderS.renderMode = ParticleSystemRenderMode.Billboard;
							}
							//Debug.Log ("change");
						} else {
							if (PrenderS.renderMode != ParticleSystemRenderMode.VerticalBillboard) {
								//PrenderS.renderMode = ParticleSystemRenderMode.VerticalBillboard;
							}
						}

					}
				}
				
			}
		
			//v2.2 - SOUND
			if (Use_audio) {
				if (CloudAudioSource == null) {
					CloudAudioSource = GetComponent<AudioSource> ();
				}
				if(CloudAudioSource != null){
					if(CloudAudioSource.clip == null & RainSound != null){
						CloudAudioSource.clip = RainSound;
					}
					if (SmoothIn) {
						if(CloudAudioSource.volume < MaxSoundVol){
							CloudAudioSource.volume += AudiofadeSpeed * Time.deltaTime;
						}
					}
					if (SmoothOut) {
						if(CloudAudioSource.volume >0){
							CloudAudioSource.volume -= AudiofadeSpeed * Time.deltaTime;
						}
					}
				}
			}

		//v2.2
		if (SmoothInSpeed < max_fade_speed & SmoothIn) {
			SmoothInSpeed += SmoothInRate*Time.deltaTime;
			SmoothOut = false;
			SmoothoutSpeed =0;
		}
		if (SmoothOut & SmoothoutSpeed < max_fade_speed) {
			//if(SmoothIn & SmoothInSpeed > 0){
				SmoothIn = false;
				SmoothInSpeed = 0;
			//}
			SmoothoutSpeed += SmoothOutRate*Time.deltaTime;
		}
			if (SmoothOut & (SmoothoutSpeed >= max_fade_speed )){
			//if (SmoothOut & (SmoothoutSpeed >= max_fade_speed || Time.fixedTime - current_smooth_out_time > max_smooth_out_time)) {
				if(DestroyOnfadeOut & cloned){
					Destroy(this.transform.gameObject);
				}
			}

		//v1.7
		if(Override_init_color){
			//if(Override_color != MainColor){
			if(Override_color != MainColor_init){
				MainColor_init = Override_color;
			}
		}

			if(Day_cycle && SkyManager != null){

				//v3.0 - new automatic TOD
				//bool is_DayLight  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 0 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time > ( 9.0f + SkyManager.Shift_dawn) & SkyManager.Current_Time <= (22.4f + SkyManager.Shift_dawn));
				//bool is_after_22  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 5 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (22.1f + SkyManager.Shift_dawn));

				bool is_DayLight  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 0 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time > ( 9.0f + SkyManager.Shift_dawn) & SkyManager.Current_Time <= (SkyManager.NightTimeMax + SkyManager.Shift_dawn));
				bool is_after_22  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 5 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (SkyManager.NightTimeMax - 0.3f + SkyManager.Shift_dawn));

				bool is_after_17  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 65) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (17.1f + SkyManager.Shift_dawn));
				bool is_before_10 = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 10) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time <  (10.0f + SkyManager.Shift_dawn));
				bool is_before_11 = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 15) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time <  (11.0f + SkyManager.Shift_dawn));
				bool is_before_16 = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 60) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time <  (16.1f + SkyManager.Shift_dawn));
				bool is_before_85 = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X <  5) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time <  ( 8.5f + SkyManager.Shift_dawn));
				bool is_after_23  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X <  3) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (23.0f + SkyManager.Shift_dawn));
				//bool is_after_224 = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X <  5) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (22.4f + SkyManager.Shift_dawn));
				bool is_after_224 = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X <  5) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (SkyManager.NightTimeMax + SkyManager.Shift_dawn));

				float Trans_speed = Time.deltaTime * color_speed;
				MainColor = MainColor_init;
				//if(SkyManager.Current_Time >= (9 + SkyManager.Shift_dawn)  & SkyManager.Current_Time <=(22.4f + SkyManager.Shift_dawn)){ //dawn to dusk
				if(is_DayLight ){
					//if(SkyManager.Current_Time > (22.1f + SkyManager.Shift_dawn)){
					if(is_after_22 ){
						MainColor_init = Color.Lerp(MainColor_init,Dusk_base_col,Trans_speed);
						if(!Grab_sun_color){
							SunColor = Color.Lerp(SunColor,Dusk_sun_col,Trans_speed);
						}
					}else
					//if(SkyManager.Current_Time > (17.1f + SkyManager.Shift_dawn)){
					if(is_after_17 ){
						MainColor_init = Color.Lerp(MainColor_init,Dusk_base_col,Trans_speed);
						if(!Grab_sun_color){
							SunColor = Color.Lerp(SunColor,Dusk_sun_col,Trans_speed);
						}
					}else{
						//if(SkyManager.Current_Time < (10+ SkyManager.Shift_dawn) ){
						if(is_before_10){
							//MainColor_init = Color.Lerp(MainColor_init,Dawn_base_col,Trans_speed);
							MainColor_init = Color.Lerp(MainColor_init,Dawn_base_col,Trans_speed * SkyManager.DawnAppearSpeed); //v3.1
							if(!Grab_sun_color){
								SunColor = Color.Lerp(SunColor,Dawn_sun_col,Trans_speed * SkyManager.DawnAppearSpeed);//v3.1
							}
						}else //if(SkyManager.Current_Time < (11 + SkyManager.Shift_dawn)){
						if(is_before_11){
							MainColor_init = Color.Lerp(MainColor_init,Day_base_col,Trans_speed);
							if(!Grab_sun_color){
								SunColor = Color.Lerp(SunColor,Day_sun_col,Trans_speed);
							}
						}else //if(SkyManager.Current_Time < (16.1f + SkyManager.Shift_dawn)){
						if(is_before_16){
							MainColor_init = Color.Lerp(MainColor_init,Day_base_col,Trans_speed);
							if(!Grab_sun_color){
								SunColor = Color.Lerp(SunColor,Day_sun_col,Trans_speed);
							}
						}else{
							MainColor_init = Color.Lerp(MainColor_init,Dusk_base_col,Trans_speed);
							if(!Grab_sun_color){
								SunColor = Color.Lerp(SunColor,Dusk_sun_col,Trans_speed);
							}
						}
					}
				}else{//to and from night transitions
					//if(SkyManager.Current_Time < (8.5f +SkyManager.Shift_dawn)){
					if(is_before_85){
						MainColor_init = Color.Lerp(MainColor_init,Night_base_col,Trans_speed);
						if(!Grab_sun_color){
							SunColor = Color.Lerp(SunColor,Night_moon_col,Trans_speed);
						}
					}else{
						//if(SkyManager.Current_Time > (23 +SkyManager.Shift_dawn)){
						if(is_after_23 ){
							MainColor_init = Color.Lerp(MainColor_init,Night_base_col,Trans_speed);
							if(!Grab_sun_color){
								SunColor = Color.Lerp(SunColor,Night_moon_col,Trans_speed);
							}
						}else{
							//if(SkyManager.Current_Time > (22.4f +SkyManager.Shift_dawn)){
							if(is_after_224){
								MainColor_init = Color.Lerp(MainColor_init,Night_base_col,Trans_speed);
								if(!Grab_sun_color){
									SunColor = Color.Lerp(SunColor,Night_moon_col,Trans_speed);
								}
							}else{
								MainColor_init = Color.Lerp(MainColor_init,Night_base_col,Trans_speed);
								if(!Grab_sun_color){
									SunColor = Color.Lerp(SunColor,Night_moon_col,Trans_speed);
								}
							}
						}
					}
				}

				//v3.0
				if(Override_sun){//if below cutoff, start night mode !!!!
					//if(sun_transf.position.y < Cut_height){
					
					if(sun_transf.position.y < Cut_height){ //v1.7
						//MainColor = Color.Lerp (MainColor,Moon_dark_color,2.5f*Time.deltaTime);
						
						if(Moon_light){
							if(sun_transf.position.y < Cut_height-10){ 
								//SunColor = Color.Lerp (SunColor,Moon_light_color,2.5f*Time.deltaTime);
								if(keep_sun_transf != moon_transf){
									keep_sun_transf = moon_transf;
								}
							}else{
								//SunColor = Color.Lerp (SunColor,Moon_dark_color,2.5f*Time.deltaTime);
							}
							
							
						}else{
							//SunColor = Color.Lerp (SunColor,Color.black,2.5f*Time.deltaTime);
						}
					}else{
						//MainColor = Color.Lerp (MainColor,MainColor_init,2.5f*Time.deltaTime);
						//SunColor = Color.Lerp (SunColor,Sun_Light.color,2.5f*Time.deltaTime);
						
						//						if(sun_transf != keep_sun_transf){
						//							sun_transf = keep_sun_transf;
						//						}
						if(keep_sun_transf != sun_transf){
							keep_sun_transf = sun_transf;
						}
					}
				}else{
					//SunColor = Sun_Light.color;
				}

				//v3.3e
				if(SkyManager.VolCloudGradients){
					SunColor = SkyManager.gradCloudLitColor;
					SunColor.a = SkyManager.VolCloudTransp;
					MainColor = SkyManager.gradCloudShadeColor;

					//AutoScale = true; //v3.5

					minLightShaderModifier = SkyManager.VcloudLightDiff-0.5f;
					IntensityShaderModifier = SkyManager.VcloudSunIntensity*8;
					GlowShaderModifier = SkyManager.VcloudFog;
					Global_tint = SkyManager.gradCloudFogColor;
					//gradCloudFogColor, VcloudSunIntensity, VcloudLightDiff, VcloudFog
				}

			}

			//v3.0-scatter params
			if (SkyManager != null) {

				//intensity control
				if(ScatterMat != null && ScatterMat.HasProperty("_Glow")){
					ScatterMat.SetFloat("_Intensity",IntensityShaderModifier);
				}


				//v3.0 auto scale for best lighting
				if(AutoScale){

					//v3.0
					bool is_dayToDusk = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 15 && SkyManager.Rot_Sun_X < 45) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time <  (17.9f + SkyManager.Shift_dawn) & SkyManager.Current_Time > (11f + SkyManager.Shift_dawn));

					//HIGH ANGLES
					//if(SkyManager.Current_Time > (11+ SkyManager.Shift_dawn) & SkyManager.Current_Time <=(17.9f+ SkyManager.Shift_dawn)){
					if(is_dayToDusk){
						if(ScatterMat != null && ScatterMat.HasProperty("_Glow")){

							if(SkyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStorm){
								ScatterMat.SetFloat("_MinLight",Mathf.Lerp(ScatterMat.GetFloat("_MinLight"), -0.55f + minLightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
								ScatterMat.SetFloat("_LightIntensity",Mathf.Lerp(ScatterMat.GetFloat("_LightIntensity"), 4f + LightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
								ScatterMat.SetFloat("_Glow",Mathf.Lerp(ScatterMat.GetFloat("_Glow"), 5.2f + GlowShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
							}else{
								ScatterMat.SetFloat("_MinLight",Mathf.Lerp(ScatterMat.GetFloat("_MinLight"), -0.55f + minLightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed * 0.1f + ModifierApplMinSpeed * 0.02f));
								ScatterMat.SetFloat("_LightIntensity",Mathf.Lerp(ScatterMat.GetFloat("_LightIntensity"), 1f + LightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed * 0.1f + ModifierApplMinSpeed * 0.02f));
								ScatterMat.SetFloat("_Glow",Mathf.Lerp(ScatterMat.GetFloat("_Glow"), 3.5f + GlowShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed * 0.1f + ModifierApplMinSpeed * 0.02f));
							}
						}
						if (!useShuriken) {//v3.4 //v3.4.6
//							cloud_scale = Mathf.Lerp (cloud_scale, 40, Time.deltaTime * SkyManager.SPEED);//160
//							global_cloud_scale = Mathf.Lerp (global_cloud_scale, 1000, Time.deltaTime * SkyManager.SPEED);
						} else {
							cloud_scale = Mathf.Lerp (cloud_scale, 0.4f, Time.deltaTime * SkyManager.SPEED);//160 //v3.4
							global_cloud_scale = Mathf.Lerp (global_cloud_scale, 1.5f, Time.deltaTime * SkyManager.SPEED);
						}
					}else{
						//LOW ANGLES
						if(ScatterMat != null && ScatterMat.HasProperty("_Glow")){

							if(SkyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStorm){
								ScatterMat.SetFloat("_MinLight",Mathf.Lerp(ScatterMat.GetFloat("_MinLight"), -0.55f + minLightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
								ScatterMat.SetFloat("_LightIntensity",Mathf.Lerp(ScatterMat.GetFloat("_LightIntensity"), 3.2f + LightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
								ScatterMat.SetFloat("_Glow",Mathf.Lerp(ScatterMat.GetFloat("_Glow"), 6.6f + GlowShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
							}else{
								ScatterMat.SetFloat("_MinLight",Mathf.Lerp(ScatterMat.GetFloat("_MinLight"), -0.8f + minLightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
								ScatterMat.SetFloat("_LightIntensity",Mathf.Lerp(ScatterMat.GetFloat("_LightIntensity"), 0.2f + LightShaderModifier,Time.deltaTime*SkyManager.SPEED*ModifierApplicationSpeed + ModifierApplMinSpeed));
								if (!useShuriken) {//v3.4.6
									//ScatterMat.SetFloat ("_Glow", Mathf.Lerp (ScatterMat.GetFloat ("_Glow"), 2.0f + GlowShaderModifier, Time.deltaTime * SkyManager.SPEED * ModifierApplicationSpeed + ModifierApplMinSpeed));
								} else {
									ScatterMat.SetFloat ("_Glow", Mathf.Lerp (ScatterMat.GetFloat ("_Glow"), 3.0f + GlowShaderModifier, Time.deltaTime * SkyManager.SPEED * ModifierApplicationSpeed + ModifierApplMinSpeed));
								}
							}
						}
						if (!useShuriken) {//v3.4.6
							//cloud_scale = Mathf.Lerp (cloud_scale, 20, Time.deltaTime * SkyManager.SPEED);//80
							//global_cloud_scale = Mathf.Lerp (global_cloud_scale, 2000, Time.deltaTime * SkyManager.SPEED);
						} else {
							cloud_scale = Mathf.Lerp (cloud_scale, 0.2f, Time.deltaTime * SkyManager.SPEED);//80 //v3.4
							global_cloud_scale = Mathf.Lerp (global_cloud_scale, 1, Time.deltaTime * SkyManager.SPEED);
						}
					}
				}



				if(ScatterMat != null & !SmoothOut){

					//v3.0 - new automatic TOD
					bool is_DayLight  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 0 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time > ( 9.0f + SkyManager.Shift_dawn) & SkyManager.Current_Time <= (21.9f + SkyManager.Shift_dawn));
					bool is_after_17  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 65) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (17.1f + SkyManager.Shift_dawn));
					bool is_before_10 = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 10) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time <  (10.0f + SkyManager.Shift_dawn));

					if (ScatterShaderUpdate) {	

						reileigh = SkyManager.m_fRayleighScaleDepth;
						reileigh = SkyManager.m_Kr;
						mieCoefficient = SkyManager.m_Km;
						//fog_depth = 1.5f;
						//if(SkyManager.Current_Time > (9+ SkyManager.Shift_dawn) & SkyManager.Current_Time <=(21.9f+ SkyManager.Shift_dawn)
						if(is_DayLight
						   | SkyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStorm){
							ScatterMat.SetVector ("sunPosition", -SkyManager.SunObj.transform.forward.normalized);
							ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),SunColor,0.5f*Time.deltaTime));
							//ScatterMat.SetVector ("_TintColor", Color.Lerp(ScatterMat.GetVector ("_TintColor"),Color.Lerp(SunLight.color,Color.white,0.5f)/(0.5f*(SkyManager.Current_Time-9f+0.01f)),0.5f*Time.deltaTime));
							//if(SkyManager.Current_Time > 17){
							if(is_after_17){
								ScatterMat.SetVector ("_TintColor", Color.Lerp(ScatterMat.GetVector ("_TintColor"),Color.Lerp(SunColor,Dusk_sun_col,0.5f),0.5f*Time.deltaTime));
							}else{
								if (is_before_10) {//v3.1
									//ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Color.white, 0.5f), 0.5f * Time.deltaTime * 0.2f * SkyManager.DawnAppearSpeed ));
									ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Dawn_sun_col, 0.5f), 0.5f * Time.deltaTime * 0.2f * SkyManager.DawnAppearSpeed )); //v3.3
								}else{
									//ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Color.white, 0.5f), 0.5f * Time.deltaTime));
									ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Day_sun_col, 0.5f), 0.5f * Time.deltaTime)); //v3.3
								}
							}
						}else{
							ScatterMat.SetVector ("sunPosition", -SkyManager.MoonObj.transform.forward.normalized);
							if(!Moon_light){
								ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),MoonLight.color,0.5f*Time.deltaTime));
								ScatterMat.SetVector ("_TintColor", Color.Lerp(ScatterMat.GetVector ("_TintColor"),MoonLight.color,0.5f*Time.deltaTime));
							}else{
								ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),Moon_light_color,0.5f*Time.deltaTime));
								ScatterMat.SetVector ("_TintColor", Color.Lerp(ScatterMat.GetVector ("_TintColor"),Moon_light_color,0.5f*Time.deltaTime));
							}
						}
						ScatterMat.SetVector ("betaR", totalRayleigh (lambda) * reileigh);
						ScatterMat.SetVector ("betaM", totalMie (lambda, K, fog_depth) * mieCoefficient);
						ScatterMat.SetFloat ("fog_depth", fog_depth);
						ScatterMat.SetFloat ("mieCoefficient", mieCoefficient);
						ScatterMat.SetFloat ("mieDirectionalG", mieDirectionalG);    
						ScatterMat.SetFloat ("ExposureBias", ExposureBias); 

					} else {

						if(ScatterMat.HasProperty("_SunColor")){
							//if(SkyManager.Current_Time > (9+ SkyManager.Shift_dawn) & SkyManager.Current_Time <=(21.9f+ SkyManager.Shift_dawn)
							if(is_DayLight
							   | SkyManager.currentWeatherName == SkyMasterManager.Volume_Weather_types.HeavyStorm){
								ScatterMat.SetVector ("sunPosition", -SkyManager.SunObj.transform.forward.normalized);
								ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),SunColor,0.5f*Time.deltaTime));
								//ScatterMat.SetVector ("_TintColor", Color.Lerp(ScatterMat.GetVector ("_TintColor"),Color.Lerp(SunLight.color,Color.white,0.5f)/(0.5f*(SkyManager.Current_Time-9f+0.01f)),0.5f*Time.deltaTime));
								//if(SkyManager.Current_Time > 17){
								if (is_after_17) {
									ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Dusk_sun_col, 0.5f), 0.5f * Time.deltaTime));
								} else {
									if (is_before_10) {//v3.1
										//ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Color.white, 0.5f), 0.5f * Time.deltaTime * 0.2f * SkyManager.DawnAppearSpeed));
										ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Dawn_sun_col, 0.5f), 0.5f * Time.deltaTime * 0.2f * SkyManager.DawnAppearSpeed )); //v3.3
									}else{
										//ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Color.white, 0.5f), 0.5f * Time.deltaTime));
										ScatterMat.SetVector ("_TintColor", Color.Lerp (ScatterMat.GetVector ("_TintColor"), Color.Lerp (SunColor, Day_sun_col, 0.5f), 0.5f * Time.deltaTime)); //v3.3
									}
								}
							}else{
								ScatterMat.SetVector ("sunPosition", -SkyManager.MoonObj.transform.forward.normalized);
								if(!Moon_light){
									ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),MoonLight.color,0.5f*Time.deltaTime));
									ScatterMat.SetVector ("_TintColor", Color.Lerp(ScatterMat.GetVector ("_TintColor"),MoonLight.color,0.5f*Time.deltaTime));
								}else{
									ScatterMat.SetVector ("_SunColor", Color.Lerp(ScatterMat.GetVector ("_SunColor"),Moon_light_color,0.5f*Time.deltaTime));
									ScatterMat.SetVector ("_TintColor", Color.Lerp(ScatterMat.GetVector ("_TintColor"),Moon_light_color,0.5f*Time.deltaTime));
								}
							}
							//TerrainMat.SetVector("sunPosition", SUN_POS);
							ScatterMat.SetVector ("betaR", totalRayleigh (lambda) * reileigh);
							ScatterMat.SetVector ("betaM", totalMie (lambda, K, fog_depth) * mieCoefficient);
							ScatterMat.SetFloat ("fog_depth", fog_depth);
							ScatterMat.SetFloat ("mieCoefficient", mieCoefficient);
							ScatterMat.SetFloat ("mieDirectionalG", mieDirectionalG);    
							ScatterMat.SetFloat ("ExposureBias", ExposureBias); 
						}
					}
				}
			}

		//v1.2.6
		//if camera moves, sort again and disable sorting
		//		ParticleRenderer Prender = this.GetComponent(typeof(ParticleRenderer)) as ParticleRenderer;
		//		//Debug.Log (Prender.particleRenderMode);
		//		if(Prender.particleRenderMode == ParticleRenderMode.SortedBillboard){
		//			Prender.particleRenderMode =ParticleRenderMode.Billboard;
		//		}else{
		//			Prender.particleRenderMode = ParticleRenderMode.SortedBillboard;
		//		}
		
		//SHAFTS COMPONENTS
		if(SunShafts != null){
			if(SunShafts.Count > 0  & !Got_shaft_items ){
				for(int i=0;i<SunShafts.Count;i++){
					Component[] Renderers = SunShafts[i].GetComponentsInChildren(typeof(Renderer));
					for(int j=0;j<Renderers.Length;j++){
						Shafts_renderes.Add(Renderers[j].GetComponent<Renderer>());
					}
					ShaftScale.Add(SunShafts[i].localScale);
				}
				Got_shaft_items = true;
			}
		}
		
		//SUN SHAFTS
		//check if closest obscures, if not move to next and break
		//sort vectors
		List<Vector3> Sorted_centers = Centers;
		List<Vector3> Sorted_centers_SUN = Centers;
		List<float> Sorted_scales = Scale_per_cloud;
		List<float> Sorted_scales_SUN = Scale_per_cloud;
		if(Beam_Occlusion & ( Time.fixedTime - Shaft_update_current > Occlusion_sort_delay )){
			Shaft_update_current = Time.fixedTime;
			if(init_centers){
				if(Scale_beams & 1==1){
					for ( int j  = 0; j < Sorted_centers_SUN.Count - 1; j ++ )
					{
						float sqrMag1 = ( Sorted_centers_SUN[j + 0] - sun_transf.position ).sqrMagnitude;
						float sqrMag2 = ( Sorted_centers_SUN[j + 1] - sun_transf.position ).sqrMagnitude;
						if ( sqrMag2 < sqrMag1 )
						{
							Vector3 temp1  = Sorted_centers_SUN[j];
							Sorted_centers_SUN[j] = Sorted_centers_SUN[j + 1];
							Sorted_centers_SUN[j + 1] = temp1;
							
							float Sorted_scales_SUN_temp = Sorted_scales_SUN[j];
							Sorted_scales_SUN[j] = Sorted_scales_SUN[j + 1];
							Sorted_scales_SUN[j + 1] = Sorted_scales_SUN_temp;
							
							j = 0;
						}
					}
				}else{
					for ( int j  = 0; j < Sorted_centers.Count - 1; j ++ )
					{
							float sqrMag1 = ( Sorted_centers[j + 0] - Cam_transf.position ).sqrMagnitude;
							float sqrMag2 = ( Sorted_centers[j + 1] - Cam_transf.position ).sqrMagnitude;
						if ( sqrMag2 < sqrMag1 )
						{
							Vector3 temp2  = Sorted_centers[j];
							Sorted_centers[j] = Sorted_centers[j + 1];
							Sorted_centers[j + 1] = temp2;
							
							float Sorted_scales_temp = Sorted_scales[j];
							Sorted_scales[j] = Sorted_scales[j + 1];
							Sorted_scales[j + 1] = Sorted_scales_temp;
							
							j = 0;
						}
					}
				}			
			}
		}
		
		for (int i=0;i<SunShafts.Count;i++){
			
				Vector3 fwd = Cam_transf.forward;
			fwd.x = 0.0f;
			fwd.y = 0.0f;
			Quaternion Rot = Quaternion.identity;
			if(fwd != Vector3.zero){
				Rot = Quaternion.LookRotation(fwd);
				prev_Rot=Rot;
			}else{
				Rot = prev_Rot;
			}
			
				float Angle_rot = Vector3.Dot(Cam_transf.forward, SunShafts[i].right)*2f*180/Mathf.PI;
				Angle_rot  = Vector3.Angle(SunShafts[i].position - Cam_transf.position, SunShafts[i].right)*1f;
				float Angle_rot2 = Vector3.Angle(-SunShafts[i].up, Cam_transf.forward);
			
			if(Angle_rot==90){
				Angle_rot=89.9f;
			}
			Angle_rot = Angle_rot - Angle_rot2;
			
			Quaternion ROt_Y = Quaternion.identity;
			
			//Random.seed=i; //v3.4.6
			
			ROt_Y = sun_transf.rotation*Quaternion.Euler(Random.Range(-9,9),Random.Range(-9,9),Random.Range(-9,9));
			
			if(Look_at_camera){
					ROt_Y = Quaternion.LookRotation(-Cam_transf.forward)*Quaternion.Euler(Random.Range(-9,9),Random.Range(-9,9),Random.Range(-9,9));
			}
			
				Quaternion targetRotation = Quaternion.LookRotation ((SunShafts[i].position - Cam_transf.position).normalized, Vector3.forward);
			targetRotation.x = 0;//Set to zero because we only care about z axis
			targetRotation.y = 0;
			SunShafts[i].rotation =  ROt_Y*targetRotation;
			
			if(i>5){
				SunShafts[i].rotation =  ROt_Y;
			}
			
			if(Beam_Occlusion){
				if(init_centers){ //check that centers have been assigned and clouds are moved to the centers
					
					bool Diminishing=false;
					if(Diminish_beams){//if below cutoff, start night mode !!!!
						if(sun_transf.position.y < Cut_height){
							//SunColor = Color.Lerp (SunColor,Color.black,0.5f*Time.deltaTime);
							for(int k=3*(i);k<3*(i+1);k++){
								Color StartCol = Shafts_renderes[k].material.GetVector("_TintColor");
								Shafts_renderes[k].material.SetVector("_TintColor", Color.Lerp(StartCol,new Color(StartCol.r,StartCol.g,StartCol.b,0),Disappear_Beam_speed*2.5f*Time.deltaTime));
							}
							Diminishing = true;
						}
					}
					
					if(!Diminishing){
						//check in order, based on the sun
						if(Scale_on_collision){
							if(Got_shaft_items){
								
								
								float Collision_dist=9999990;
								
								RaycastHit hit = new RaycastHit();
								if (Physics.Raycast(SunShafts[i].position,SunShafts[i].forward, out hit)){
									Collision_dist = hit.distance;
									
								}
								
								if(Beam_length > Collision_dist){
									float New_scale = Offset_col_factor*(Collision_dist * ShaftScale[i].z)/Beam_length;
									if(SunShafts[i].localScale.z > (New_scale+0.1f) ){
										SunShafts[i].localScale = new Vector3(SunShafts[i].localScale.x,SunShafts[i].localScale.y,New_scale);
									}
									
								}else{
									if(SunShafts[i].localScale.z != ShaftScale[i].z){
										SunShafts[i].localScale = new Vector3(SunShafts[i].localScale.x,SunShafts[i].localScale.y,ShaftScale[i].z);
									}
								}
							}
						}//else
						if(Scale_beams & 1==1){
							if(Got_shaft_items){
								for (int j=0; j < divider;j++){					
									Vector3 Sun_center = Sorted_centers_SUN[j] - sun_transf.position;
									Vector3 Beam = SunShafts[i].forward;
									float Angle_sun_center_beam = Vector3.Angle(Sun_center,Beam);
									if(Angle_sun_center_beam > 20){
										float New_scale = (Sun_center.magnitude * ShaftScale[i].z)/Beam_length;
										
										if(SunShafts[i].localScale.z > (New_scale+0.1f) ){
											SunShafts[i].localScale = new Vector3(SunShafts[i].localScale.x,SunShafts[i].localScale.y,New_scale);
										}
										break;
									}else{
										if(SunShafts[i].localScale.z != ShaftScale[i].z){
											SunShafts[i].localScale = new Vector3(SunShafts[i].localScale.x,SunShafts[i].localScale.y,ShaftScale[i].z);
										}
									}
								}
							}
						}else{
							
							//check in order, based on camera
							for (int j=0; j < divider;j++){
								
								if(Got_shaft_items){
									
									if(Debug_mode){
										Debug.DrawLine(This_transf.position,Sorted_centers[j],new Color(1/(0.1f*(j+1)),0,0,1));
									}
									
										Vector3 Corner1 = Sorted_centers[j] + Quaternion.LookRotation(Cam_transf.forward)*new Vector3(cloud_scale*Sorted_scales[j],cloud_scale*Sorted_scales[j]/2,0);
										Vector3 Corner1_opp = Sorted_centers[j] - Quaternion.LookRotation(Cam_transf.forward)*new Vector3(cloud_scale*Sorted_scales[j],cloud_scale*Sorted_scales[j]/2,0);
									
									Vector3 flat1 = Camera.main.WorldToScreenPoint(Corner1);//
									Vector3 flat1_opp = Camera.main.WorldToScreenPoint(Corner1_opp);//
									Vector3 flat_beam =  Camera.main.WorldToScreenPoint(SunShafts[i].position);
									
									bool flat1_is_left = true; // for X, Y is assumed as always flat1 !!! 
									if(flat1.x > flat1_opp.x){
										flat1_is_left=false;
									}
									if( (((flat1_is_left & (flat_beam.x > flat1.x & flat_beam.x < flat1_opp.x & flat_beam.y < flat1.y & flat_beam.y > flat1_opp.y))
									      | (!flat1_is_left & (flat_beam.x < flat1.x & flat_beam.x > flat1_opp.x & flat_beam.y < flat1.y & flat_beam.y > flat1_opp.y))
									      ) & (flat1.z > 0 & flat1_opp.z > 0)) 
										   | ((Cam_transf.position - Centers[j]).magnitude < cloud_scale*Scale_per_cloud[j])
									   )
									{
										if(Smooth_mat_trans){
											for(int k=3*(i);k<3*(i+1);k++){
												Color StartCol = Shafts_renderes[k].material.GetVector("_TintColor");
												Shafts_renderes[k].material.SetVector("_TintColor", Color.Lerp(StartCol,new Color(StartCol.r,StartCol.g,StartCol.b,0),Disappear_Beam_speed*2.5f*Time.deltaTime));
											}
											
										}else{
											if(SunShafts[i].gameObject.activeInHierarchy){
												SunShafts[i].gameObject.SetActive(false);
											}
										}
										
										if(Debug_mode){
											Debug.DrawLine(This_transf.position,Sorted_centers[j]);
											Debug.DrawLine(This_transf.position,Corner1,Color.blue);
											Debug.DrawLine(This_transf.position,Corner1_opp,Color.blue);
										}
										
										break;
									}else{
										if(Smooth_mat_trans){
											
											for(int k=3*(i);k<3*(i+1);k++){
												Color StartCol = Shafts_renderes[k].material.GetVector("_TintColor");
												Shafts_renderes[k].material.SetVector("_TintColor", Color.Lerp(StartCol,new Color(StartCol.r,StartCol.g,StartCol.b,start_beam_transp),Appear_Beam_speed*0.5f*Time.deltaTime));
											}
											
										}else{
											if(!SunShafts[i].gameObject.activeInHierarchy){
												SunShafts[i].gameObject.SetActive(true);
											}
										}
									}
								}
							}
						}
					}
					
				}
			}
			
		}
		
			#region SHURIKEN


			//v3.5 - init clouds on start
			if (ShurikenParticle != null) {

				//v3.5
				//ParticleSystem.Particle[] 
				//particles = new ParticleSystem.Particle[ShurikenParticle.maxParticles];//v3.5
				if(particles == null){
					particles = new ParticleSystem.Particle[ShurikenParticle.main.maxParticles]; //v3.4.9
				}

				//if(particles == null){
				//ShurikenParticle.GetParticles (particles);	//		Particle[] particles = GetComponent<ParticleEmitter> ().particles;
				//}

				if (DecoupledWind) {
					//particles = GetComponent<ParticleEmitter>().particles;
					ShurikenParticle.GetParticles (particles);
				}

				if(Time.fixedTime - Prev_time  > UpdateInterval | !Init){
					Prev_time = Time.fixedTime;

					if (!DecoupledWind) {
						//particles = GetComponent<ParticleEmitter>().particles;
						ShurikenParticle.GetParticles (particles);
					}


					//v3.0
					if(Grab_sun_color){

						if(Override_sun){//if below cutoff, start night mode !!!!
							//if(sun_transf.position.y < Cut_height){

							if(sun_transf.position.y < Cut_height){ //v1.7
								MainColor = Color.Lerp (MainColor,Moon_dark_color,2.5f*Time.deltaTime);

								if(Moon_light){
									if(sun_transf.position.y < Cut_height-10){ 
										SunColor = Color.Lerp (SunColor,Moon_light_color,2.5f*Time.deltaTime);
										if(keep_sun_transf != moon_transf){
											keep_sun_transf = moon_transf;
										}
									}else{
										SunColor = Color.Lerp (SunColor,Moon_dark_color,2.5f*Time.deltaTime);
									}


								}else{
									SunColor = Color.Lerp (SunColor,Color.black,2.5f*Time.deltaTime);
								}
							}else{
								MainColor = Color.Lerp (MainColor,MainColor_init,2.5f*Time.deltaTime);
								SunColor = Color.Lerp (SunColor,Sun_Light.color,2.5f*Time.deltaTime);

								//						if(sun_transf != keep_sun_transf){
								//							sun_transf = keep_sun_transf;
								//						}
								if(keep_sun_transf != sun_transf){
									keep_sun_transf = sun_transf;
								}
							}
						}else{
							SunColor = Sun_Light.color;
						}

						if(prev_intensity != Sun_Light.intensity){
							//MainColor = Sun_Light.intensity*(MainColor_init/3);

							//v1.6
							MainColor = Color.Lerp (MainColor, Sun_Light.intensity*(MainColor_init/3),2.5f*Time.deltaTime);

							prev_intensity = Sun_Light.intensity;
						}
						//ASSUME 3 intensity is daylight
					}

					if(!Sorted){
						//LEGACY
						//		Particle[] particles = GetComponent<ParticleEmitter>().particles;

						//find particle number for each center
						int particles_per_cloud = (int)(particles.Length/divider);

						if(!init_centers){
							int count_me = 0;
							//v2.2
							if(VerticalForm){
								for (int j=0; j < divider/2;j++){
									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
										particles[i].position = (particles[i].position  - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;

										particles[i].startColor = Global_tint + MainColor;

										int start_C = (i+(divider/2)*particles_per_cloud);
										//int end_C = ((i+(divider/2))*particles_per_cloud);

										particles[start_C].position = (particles[start_C].position + new Vector3(0,Yspread,0) - This_transf.position)*Scale_per_cloud[j]*YScaleDiff + Centers[j]+MaxCloudDist ;

										particles[start_C].startColor = Color.Lerp(Global_tint + MainColor , HeightTint,HeightTint.a);

										//v2.2
										if(SmoothIn){
											particles[i].startColor = Color.Lerp(particles[i].startColor,Global_tint + MainColor,Time.deltaTime*SmoothInSpeed);
											particles[start_C].startColor = Color.Lerp(particles[i].startColor,Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a),Time.deltaTime*SmoothInSpeed);
											//Debug.Log(HeightTint.a);
										}else{
											particles[i].startColor = Global_tint + MainColor;
											particles[start_C].startColor = Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a);
										}

										count_me = count_me+1;
									}
								}
							}else{
								for (int j=0; j < divider;j++){
									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
										particles[i].position = (particles[i].position - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;
										count_me = count_me+1;
									}
								}
							}
							if(	count_me > 0){
								init_centers=true; Init = true; 
								//GetComponent<ParticleEmitter>().particles = particles;
								ShurikenParticle.SetParticles (particles,particles.Length); //v3.5
							}
						}else{

							//for each center !!!!!!!!
							for (int j=0; j < divider;j++){
								float speed2=speed;
								speed2 = multiplier*speed*(j+1);

								//move centers
								if(!DecoupledWind){
									Centers[j] = Centers[j]+ wind*speed2*Time.deltaTime;
								}
								//	Debug.DrawLine(Centers[0],Centers[j]);

								float Dist_sun_center = (Centers[j] - keep_sun_transf.position).magnitude;

								//for specific center particles !!!
								for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {

									//find closest center and apply lighting based on that
									Vector3 Closest_center = Centers[j];
									int Closest_Center_ID=j;
									for (int k=0; k<Centers.Count; k++) {
										if(Vector3.Distance(Centers[k],particles[i].position) < Vector3.Distance(Closest_center,particles[i].position)){
											Closest_center = Centers[k];
											Closest_Center_ID=k;
										}
									}
									Dist_sun_center = (Closest_center - keep_sun_transf.position).magnitude;

									particles[i].position = particles[i].position +wind*(multiplier*speed*(Closest_Center_ID+1))*Time.deltaTime; 

									float Dist_sun_part = (particles[i].position - keep_sun_transf.position).magnitude;
									float Diff = (Dist_sun_part - (Dist_sun_center))  / 1.380f; // move to each center's center

									if(Prev_cam_rot == Cam_transf.eulerAngles){
										particles[i].startColor = Color.Lerp (SunColor,MainColor,Diff);

									}else{
										Prev_cam_rot = Cam_transf.eulerAngles;
									}
								}
							}		

							if (!DecoupledWind) {
								//GetComponent<ParticleEmitter>().particles = particles;
								ShurikenParticle.SetParticles (particles,particles.Length); //v3.5
							}
						}
					}else if(Sorted){							///////////////////// SORTED CASE - go per particle, check closest center
						//LEGACY
						//			Particle[] particles = GetComponent<ParticleEmitter>().particles;

						//find particle number for each center
						int particles_per_cloud = (int)(particles.Length/divider);

						if(!init_centers){
							int count_me = 0;

							//v2.2
							if(VerticalForm){
								for (int j=0; j < divider/2;j++){
									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
										particles[i].position = (particles[i].position - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;

										//particles[i].color = Global_tint + MainColor; //v2.2

										int start_C = (i+(divider/2)*particles_per_cloud);
										//int end_C = ((i+(divider/2))*particles_per_cloud);

										particles[start_C].position = (particles[start_C].position + new Vector3(0,Yspread,0) - This_transf.position)*Scale_per_cloud[j]*YScaleDiff + Centers[j]+MaxCloudDist;

										//v2.2
										if(SmoothIn){
											particles[i].startColor = Color.Lerp(particles[i].startColor,Global_tint + MainColor,Time.deltaTime*SmoothInSpeed);
											particles[start_C].startColor = Color.Lerp(particles[i].startColor,Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a),Time.deltaTime*SmoothInSpeed);
											//Debug.Log(HeightTint.a);
										}else{
											particles[i].startColor = Global_tint + MainColor;
											particles[start_C].startColor = Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a);
										}

										particles[i].remainingLifetime = 100000; //v3.4.5a
										particles[i].startLifetime = 100000;
										particles[start_C].remainingLifetime = 100000; //v3.4.5a
										particles[start_C].startLifetime = 100000;

										count_me = count_me+1;
									}
								}
							}else{
								for (int j=0; j < divider;j++){
									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
										particles[i].position = (particles[i].position - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;

										particles[i].remainingLifetime = 100000; //v3.4.5a
										particles[i].startLifetime = 100000;

										count_me = count_me+1;
									}
								}
							}
							if(	count_me > 0){
								init_centers=true; Init = true; 
								//GetComponent<ParticleEmitter>().particles = particles;
								ShurikenParticle.SetParticles (particles,particles.Length); //v3.5
							}
						}

						if(init_centers){

							//v2.2
							bool hero_close_to_center = false;
							//float Dist_hero_center = 0;

							//move each center !!!!!!!!
							int count_outers = 0;
							int count_shadows = 0;
							int count_all_shadows = 0;
							for (int j=0; j < divider;j++){
								float speed2=speed;
								speed2 = multiplier*speed*(j+1);

								//move centers
								if(!DecoupledWind){
									Centers[j] = Centers[j]+ wind*speed2*Time.deltaTime;
								}
								//		Debug.DrawLine(Centers[0],Centers[j]);

								//v2.2 - LOD
								if(EnableLOD){
									float Dist_hero_center1 = Vector3.Distance(Cam_transf.position,Centers[j]+new Vector3(0,120,0));
									if(Dist_hero_center1 < LodMaxYDiff){
										hero_close_to_center = true;
										//Dist_hero_center = Dist_hero_center1;
									}
								}

								//v1.7 - add shadow planes
								if(!DecoupledWind){
									if(Add_shadows & !shadows_created){
										//v3.0
										if(!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)){
											if(count_all_shadows < Shadow_planes.Count){
												Shadow_planes[count_all_shadows].position = Centers[j];
											}
											count_shadows=0;
											count_all_shadows++;
										}else{
											count_shadows++;
										}
									}
								}

								if(Restore_on_bound){

									if(Vector3.Distance(Centers[j],This_transf.position) > Bound){
										count_outers = count_outers+1;
									}
									if(Disable_on_bound){

										//v2.3 - fade out on bound
										//if(FadeOutOnBoundary && count_outers >= Centers.Count - 10){
										//if(FadeOutOnBoundary && count_outers >= (Centers.Count - (0.01f*Centers.Count)) ){
										if(!SmoothOut && FadeOutOnBoundary && count_outers >= Centers.Count - 4){
											//FadeOutOnBoundary//fade
											SmoothIn = false;
											SmoothOut = true;
											//		current_smooth_out_time = Time.fixedTime;
											SmoothoutSpeed = 0;
											SmoothInSpeed = 0;
										}

										if(count_outers >= Centers.Count){
											if(clone_on_end & !cloned){
												//Instantiate(this.transform.gameObject);

												shadows_created = true;//flag that shadows have been instantiated, for next instance

												//v3.0
												GameObject InstanceC = Instantiate(this.transform.gameObject);
												InstanceC.GetComponent<VolumeClouds_SM>().SmoothIn = true;
												InstanceC.GetComponent<VolumeClouds_SM>().SmoothOut = false;
												InstanceC.GetComponent<VolumeClouds_SM>().current_fadein_time = Time.fixedTime;

												SkyManager.currentWeather.VolumeScript = InstanceC.GetComponent<VolumeClouds_SM>();

												//scale clouds
												//InstanceC.GetComponent<VolumeClouds_SM>().ScaleClouds(SkyManager.WorldScale,SkyManager.VCloudCoverFac,SkyManager.VCloudSizeFac,SkyManager.VCloudCSizeFac);

												cloned = true;
											}
											if(!DestroyOnfadeOut){
												This_transf.gameObject.SetActive(false);
												if(destroy_on_end){
													Destroy(this.transform.gameObject);
												}
											}
										}
									}
								}
							}

							//for specific center particles !!!
							//						if(alternate == 2){
							//							alternate =1;
							//						}else if (alternate == 1){
							//							alternate =2;
							//						}
							int frames_divider = max_divider;
							if(Time.fixedTime - Cloud_update_current < Cloud_spread_delay){
								alternate = 2;
								frames_divider =1;
								//Cloud_update_current = Time.fixedTime;
							}else if(Time.fixedTime - Cloud_update_current == Cloud_spread_delay){
								alternate = 2;
								frames_divider =1;
							}
							else{
								if(alternate > max_divider+0){
									alternate = 2;
								}else{
									alternate++;
								}
								//Debug.Log ("DIV="+frames_divider);
								//Debug.Log ("ALT="+alternate);
							}

							for (int i=alternate-2; i<particles.Length; i=i+frames_divider) {

								particles[i].remainingLifetime = 100000; //v3.4.5a
								particles[i].startLifetime = 100000;

								if(!DecoupledColor){

									//find closest center and apply lighting based on that	
									int Closest_Center_ID=0;
									Vector3 Closest_center = Centers[0];							
									for (int k=0; k<Centers.Count; k++) {
										//
										if(Use2DCheck){
											if(Vector2.Distance(new Vector2(Centers[k].x,Centers[k].z),new Vector2(particles[i].position.x,particles[i].position.z)) 
												< Vector3.Distance(new Vector2(Closest_center.x,Closest_center.z),new Vector2(particles[i].position.x,particles[i].position.z))){

												Closest_center = Centers[k];
												Closest_Center_ID = k;
											}
										}else{
											if(Vector3.Distance(Centers[k],particles[i].position) < Vector3.Distance(Closest_center,particles[i].position)){
												Closest_center = Centers[k];
												Closest_Center_ID = k;
											}
										}
									}
									float Dist_sun_center = (Closest_center - keep_sun_transf.position).magnitude;
									///		Debug.DrawLine(particles[i].position,Closest_center);		

									//v1.2.6
									//int method =1;

									//v.1.7 - sheet handle
									//Random.seed = i;
			//						particles[i].energy = 2;//Random.Range(1,4);

									if(Flatten_below){
										float Dist_part_center = (Closest_center.y - particles[i].position.y);
										if(Dist_part_center > 0){
											particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y +(500)*Time.deltaTime,particles[i].position.z); 
										}
									}

									float dist = (keep_sun_transf.position - particles[i].position).magnitude/10;

									if(Turbulent){

										if(method ==1){//use for static camera, no extra sorting required, will break if camera rotates

											//particles[i].size = 3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f);
											particles[i].startSize = Mathf.Abs(3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f));
											if(particles[i].startSize < 330){
												particles[i].startSize =330;
											}else if(particles[i].startSize > 600){
												particles[i].startSize =600;
											}
										}
										else if(method ==2){

											if(particles[i].startSize < 300 | 1==1){

												particles[i].startSize = particles[i].startSize + 4f*200*Mathf.Cos (Time.fixedTime+dist*Time.fixedTime*0.0005f)*Time.deltaTime;
												//particles[i].size = particles[i].size + 4f*200*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f)*Time.deltaTime;
											}else{
												//	particles[i].size = particles[i].size - 3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f);
											}
											if(particles[i].startSize < 300){
												particles[i].startSize =300;
											}else if(particles[i].startSize > 500){
												particles[i].startSize =500;
											}
										}
										else if(method == 3){

											float COS_IN = Time.fixedTime+dist*Time.fixedTime*0.0005f;

											COS_IN = Time.fixedTime+dist*0.115f;

											if(particles[i].startSize < 500 & particles[i].startSize > 200){

												particles[i].startSize = particles[i].startSize + 4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
												//particles[i].size = particles[i].size + 4f*200*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f)*Time.deltaTime;
											}else{
												//	particles[i].size = particles[i].size - 3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f);
											}
											if(particles[i].startSize < 200){
												//particles[i].size =100;//keep growing, but slower, until cos turns it around
												if(Mathf.Cos (COS_IN) <0){
													particles[i].startSize = particles[i].startSize + 0.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
												}else{
													particles[i].startSize = particles[i].startSize + 4.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
												}
											}else if(particles[i].startSize > 500){
												//particles[i].size =500;
												//keep growing, but slower, until cos turns it around
												if(Mathf.Cos (COS_IN) > 0){
													particles[i].startSize = particles[i].startSize + 0.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
												}else{
													particles[i].startSize = particles[i].startSize + 4.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
												}

											}
										}

										particles[i].position = particles[i].position +wind*(multiplier*speed*(dist+1))*Time.deltaTime;
									}else{
										if(!DecoupledWind){
											particles[i].position = particles[i].position +wind*(multiplier*speed*(Closest_Center_ID+1))*Time.deltaTime; 
										}
									}

									//v2.2 - LOD
									if(EnableLOD & Prev_cam_rot == Cam_transf.eulerAngles){
										//float LodMaxYDiff = 140;
										//float LodMaxHDiff = 700;
										float DistCP = Vector2.Distance(new Vector2(Cam_transf.position.x,Cam_transf.position.z),new Vector2(particles[i].position.x,particles[i].position.z));
										//if(Mathf.Abs(Cam_transf.position.y - particles[i].position.y) < LodMaxYDiff 
										if(//Mathf.Abs(Cam_transf.position.y - Closest_center.y) < LodMaxYDiff &
											//Vector2.Distance(new Vector2(Cam_transf.position.x,Cam_transf.position.z),new Vector2(particles[i].position.x,particles[i].position.z)) > (LodMaxHDiff * (0.7f+(Dist_hero_center/LodMaxYDiff)))
											(DistCP > LodMaxHDiff | DistCP < LODMinDist)
											//Vector3.Distance(Cam_transf.position,particles[i].position) < LODMinDist)
											& hero_close_to_center){
											if(ParticlesLODed[i] == 0){
												//particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y+LOD_send_height, particles[i].position.z);
												//ParticlesLODed[i] = 1; 
												ParticlesLODed[i] = (int)particles[i].startSize;
												//particles[i].size = 0;
												//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed);
											}
											if(ParticlesLODed[i] > 0){
												particles[i].startSize = Mathf.Lerp(particles[i].startSize, 0 ,Time.deltaTime*LODFadeOutSpeed);
												particles[i].startColor = Color.Lerp(particles[i].startColor,new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,0),Time.deltaTime*LODFadeOutSpeed*2);//become transp. faster
											}
										}else{
											//if(ParticlesLODed[i] == 1){
											if(ParticlesLODed[i] > 0){
												//particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y-LOD_send_height, particles[i].position.z);

												if(particles[i].startSize < ParticlesLODed[i]){
													particles[i].startSize = Mathf.Lerp(particles[i].startSize, ParticlesLODed[i] ,Time.deltaTime*LODFadeInSpeed*2);//become normal size before appear
													particles[i].startColor = Color.Lerp(particles[i].startColor,new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,1),Time.deltaTime*LODFadeInSpeed);
												}else{
													ParticlesLODed[i] = 0; 
												}

												//particles[i].size = 410;										
											}
											//if(ParticlesLODed[i] == 0){
											//		particles[i].size = Mathf.Lerp(particles[i].size, 0 ,Time.deltaTime*SmoothoutSpeed);
											//}
										}
									}


									//

									float Dist_sun_part = (particles[i].position - keep_sun_transf.position).magnitude;
									float Diff = ((Dist_sun_part - (Dist_sun_center))+((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]))  /((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]); //normalize based on cloud size

									float Diff2 = 0;
									if(Sun_dist_on){
										float Dist_sun_GLOBALcenter = (This_transf.position - keep_sun_transf.position).magnitude;
										Diff2 = ((Dist_sun_part - (Dist_sun_GLOBALcenter))+global_cloud_scale)  /global_cloud_scale;
										Diff = Mathf.Lerp(Diff,Diff2,0.5f);
									}

									Color SunColor2 = SunColor;
									if(Grab_sun_angle){
										float Angle_factor = Vector3.Angle((particles[i].position - keep_sun_transf.position),(This_transf.position - keep_sun_transf.position));
										SunColor2.a = SunColor.a/(Mathf.Abs (Angle_factor)*Sun_angle_influence);
									}

									//					if(!DecoupledColor){
									if(Prev_cam_rot == Cam_transf.eulerAngles){
										if(Init_color < Delay_lerp){
											//v2.2
											if(SmoothIn){
												particles[i].startColor = Color.Lerp(particles[i].startColor,Global_tint + Color.Lerp (SunColor2,MainColor,Diff),Time.deltaTime*SmoothInSpeed);
											}else{
												particles[i].startColor = Global_tint + Color.Lerp (SunColor2,MainColor,Diff);
											}
											Init_color++;
										}else{
											//v1.6 - move tint inside
											if(SmoothIn){//v2.2
												if(!DecoupledColor){
													if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
														particles[i].startColor = Color.Lerp(particles[i].startColor,Color.Lerp (particles[i].startColor,  Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
													}else{
														if(!DecoupledWind){
															particles[i].startColor = Color.Lerp(particles[i].startColor,Color.Lerp (particles[i].startColor, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
														}
													}
												}
											}else if(SmoothOut){//v2.2
												if(!DecoupledColor){
													if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
														//particles[i].color =  Color.Lerp(Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
														//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
													}else{
														//particles[i].color = Color.Lerp(Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
														//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
													}

													if(ScatterMat != null && ScatterMat.HasProperty("_TintColor")){ //v3.2
														Color tintC = ScatterMat.GetVector ("_TintColor");
														ScatterMat.SetVector ("_TintColor", Color.Lerp(tintC,new Color(tintC.r,tintC.g,tintC.b,0),0.01f*Time.deltaTime*SmoothoutSpeed));
													}

													particles[i].startColor = Color.Lerp(particles[i].startColor,new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,0),Time.deltaTime*SmoothoutSpeed*0.5f);
													//Debug.Log(SmoothoutSpeed);
												}
											}
											else{
												if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
													particles[i].startColor = Color.Lerp (particles[i].startColor, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime*0.005f);
												}else{
													particles[i].startColor = Color.Lerp (particles[i].startColor, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime*0.005f);
												}
											}
										}
									}else{
										Prev_cam_rot = Cam_transf.eulerAngles;
									}
								}

								//v2.2 - Cloud Wave
								if(CloudWaves){
									//if(SmoothIn & particles[i].color.a == 1){
									//	SmoothIn = false;
									//}
									if(ParticlesLODed[i] == 0 & !SmoothIn){
										//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,Mathf.Abs(Mathf.Cos(WaveFreq*Time.fixedTime+particles[i].position.x))),Time.deltaTime*LODFadeInSpeed);
										//particles[i].color =new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,Mathf.Abs(Mathf.Cos(WaveFreq*Time.fixedTime+0.1f*Mathf.Sin (Vector3.Distance(particles[i].position,This_transf.position)))));
										particles[i].startColor =new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,Mathf.Abs(Mathf.Cos(WaveFreq*Time.fixedTime+0.005f* (Vector3.Distance(particles[i].position,This_transf.position)))));
									}
								}

							}// END PARTICLE CYCLE							

							///////////////////////// DECOUPLE WIND



							if (!DecoupledWind) {
								//GetComponent<ParticleEmitter>().particles = particles;
								ShurikenParticle.SetParticles (particles,particles.Length); //v3.5
							}
						}
					}//END if SORTED LEGACY


				}//END UPDATE INTERVAL CHECK

				if (DecoupledWind) {

					int count_shadows = 0;
					int count_all_shadows = 0;

					for (int j=0; j < divider; j++) {
						//float speed2 = speed;
						//speed2 = multiplier * speed * (j + 1);

						//move centers
						//if (!DecoupledWind) {
						//Centers [j] = Centers [j] + wind * speed2 * Time.deltaTime;

						if(!DifferentialMotion){
							Centers [j] = Centers [j] + wind * (multiplier * speed*5) * Time.deltaTime; 
						}else{
							//Centers [j] = Centers [j] + wind * (multiplier * speed*5) * Time.deltaTime * (MaxDiffSpeed*((j+1)/divider)); 
							Centers [j] = Centers [j] + wind * (multiplier * speed*5) * Time.deltaTime * (MaxDiffSpeed*((j+MaxDiffOffset)/divider));
							//Debug.Log("in1");
						}
						//}

						if(Add_shadows){
							//v3.0
							if(!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)){
								if(count_all_shadows < Shadow_planes.Count){
									Shadow_planes[count_all_shadows].position = Centers[j];
								}
								count_shadows=0;
								count_all_shadows++;
							}else{
								count_shadows++;
							}
						}

					}
					//if (GetComponent<ParticleEmitter> () != null) 
					{

						if (Sorted) 
						{

							//				Particle[] particles = GetComponent<ParticleEmitter> ().particles;




							//find particle number for each center
							//int particles_per_cloud = (int)(particles.Length / divider);

							if (init_centers) 
							{
								//v3.1
								float currentRot = Cam_transf.eulerAngles.z;
								float diffRot = currentRot - Prev_cam_rot.z;

								float currentRotY = Cam_transf.eulerAngles.y;
								float diffRotY = currentRotY - Prev_cam_rot.y;

								if (Mathf.Abs(diffRotY) > 180) {
									diffRotY = 360-Mathf.Abs(diffRotY);
								}

								float Angle_check = 0;
								float Dot_check = 0;
								//v3.1
								if(StableRollMethod2) {
									Angle_check = Mathf.Abs(Vector3.Angle (Cam_transf.forward, Mathf.Sign(Cam_transf.forward.y)*Vector3.up));
									Dot_check = -Mathf.Sign(Cam_transf.forward.y)*Vector3.Dot(Cam_transf.forward, Mathf.Sign (Cam_transf.forward.y) * Vector3.up);

									//Debug.Log("diffRotY="+diffRotY + "Dot_check="+Dot_check + " SUM="+(diffRotY*Dot_check).ToString() + " Angle="+Angle_check);
								}

								//Debug.Log(particles.Length);
								for (int i=0; i<particles.Length; i=i+1) {

									particles[i].remainingLifetime = 100000; //v3.4.5a
									particles[i].startLifetime = 100000;

									if(!DifferentialMotion){
										particles [i].position = particles [i].position + wind * (multiplier * speed*5) * Time.deltaTime; 
									}else{
										particles [i].position = particles [i].position + wind * (multiplier * speed*5) * Time.deltaTime* (MaxDiffSpeed*((i+MaxDiffOffset)/particles.Length)); 
									}

									//v3.1
									if(renewAboveHeight){
										if(particles[i].position.y > renewHeight){
											//if(Random.Range(1,1000) == 3){
											particles[i].position = new Vector3(particles[i].position.x, This_transf.position.y, particles[i].position.z);
											//}
										}
										if (particles [i].position.y > (renewHeightPercent / 100)*renewHeight) {//just before go away, fade out //v3.3b
											particles[i].startColor = Color.Lerp(particles[i].startColor,new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,0),Time.deltaTime*renewFadeOutSpeed*5.5f);//v3.3b
										}else{
											//if (particles [i].position.y < (15 / 100) * renewHeight) {
											particles[i].startColor = Color.Lerp(particles[i].startColor,new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,(Global_tint + Color.Lerp (SunColor,MainColor,0.8f)).a),Time.deltaTime*renewFadeInSpeed*10.5f);
										}

										if (particles [i].position.y < This_transf.position.y+0.1*This_transf.position.y) {
											particles[i].startColor = new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,0);//v3.3b
										}//else
										//if (particles [i].position.y < This_transf.position.y + 0.3 * This_transf.position.y) {

										//}
									}


									if(DecoupledColor){

										int Closest_Center_ID=0;
										Vector3 Closest_center = Centers[0];



										if(DecoupledCounter == i || CenterIDs.Count < particles.Length){// || Prev_cam_rot != Cam_transf.eulerAngles){
											for (int k=0; k<Centers.Count; k++) {
												//
												if(Use2DCheck){
													if(Vector2.Distance(new Vector2(Centers[k].x,Centers[k].z),new Vector2(particles[i].position.x,particles[i].position.z)) 
														< Vector3.Distance(new Vector2(Closest_center.x,Closest_center.z),new Vector2(particles[i].position.x,particles[i].position.z))){

														Closest_center = Centers[k];
														Closest_Center_ID = k;
													}
												}else{
													if(Vector3.Distance(Centers[k],particles[i].position) < Vector3.Distance(Closest_center,particles[i].position)){
														Closest_center = Centers[k];
														Closest_Center_ID = k;
													}
												}
												//break;

											}

											//Debug.Log("particles len ="+particles.Length +" centers ID"+CenterIDs.Count);

										}else{
											Closest_Center_ID = CenterIDs[i];
											Closest_center = Centers[Closest_Center_ID];
										}

										if(CenterIDs.Count < particles.Length){
											CenterIDs.Add(Closest_Center_ID);
										}

										float Dist_sun_center = (Closest_center - keep_sun_transf.position).magnitude;

					//					particles[i].energy = 2;//Random.Range(1,4);

										if(Flatten_below){
											float Dist_part_center = (Closest_center.y - particles[i].position.y);
											if(Dist_part_center > 0){
												particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y +(500)*Time.deltaTime,particles[i].position.z); 
											}
										}

										float Dist_sun_part = (particles[i].position - keep_sun_transf.position).magnitude;
										float Diff = ((Dist_sun_part - (Dist_sun_center))+((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]))  /((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]); //normalize based on cloud size

										float Diff2 = 0;
										if(Sun_dist_on){
											float Dist_sun_GLOBALcenter = (This_transf.position - keep_sun_transf.position).magnitude;
											Diff2 = ((Dist_sun_part - (Dist_sun_GLOBALcenter))+global_cloud_scale)  /global_cloud_scale;
											Diff = Mathf.Lerp(Diff,Diff2,0.5f);
										}

										Color SunColor2 = SunColor;
										if(Grab_sun_angle){
											float Angle_factor = Vector3.Angle((particles[i].position - keep_sun_transf.position),(This_transf.position - keep_sun_transf.position));
											SunColor2.a = SunColor.a/(Mathf.Abs (Angle_factor)*Sun_angle_influence);
										}





										//	if(DecoupledColor){
										if(Prev_cam_rot == Cam_transf.eulerAngles ){
											if(Init_color < Delay_lerp){
												//v2.2
												if(SmoothIn){
													particles[i].startColor = Color.Lerp(particles[i].startColor,Global_tint + Color.Lerp (SunColor2,MainColor,Diff),Time.deltaTime*SmoothInSpeed);
												}else{
													particles[i].startColor = Global_tint + Color.Lerp (SunColor2,MainColor,Diff);
												}
												Init_color++;
											}else{
												//v1.6 - move tint inside
												if(SmoothIn){//v2.2
													//if(!DecoupledWind){
													if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
														particles[i].startColor = Color.Lerp(particles[i].startColor,Color.Lerp (particles[i].startColor,  Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
													}else{
														particles[i].startColor = Color.Lerp(particles[i].startColor,Color.Lerp (particles[i].startColor, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
													}

													if(Time.fixedTime - current_fadein_time > fade_in_time){
														SmoothIn = false;
													}
													//}
												}else if(SmoothOut){//v2.2
													//if(!DecoupledWind){
													if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
														//particles[i].color =  Color.Lerp(Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
														//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
													}else{
														//particles[i].color = Color.Lerp(Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
														//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
													}

													if(ScatterMat != null){
														Color tintC = ScatterMat.GetVector ("_TintColor");
														ScatterMat.SetVector ("_TintColor", Color.Lerp(tintC,new Color(tintC.r,tintC.g,tintC.b,0),0.2f*Time.deltaTime*SmoothoutSpeed));
													}

													particles[i].startColor = Color.Lerp(particles[i].startColor,new Color(particles[i].startColor.r,particles[i].startColor.g,particles[i].startColor.b,0),Time.deltaTime*SmoothoutSpeed*100);
													//Debug.Log(SmoothoutSpeed);
													//}
												}
												else{
													if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
														particles[i].startColor = Color.Lerp (particles[i].startColor, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime);
													}else{
														particles[i].startColor = Color.Lerp (particles[i].startColor, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime);
													}
												}
											}
										}else{
											//Prev_cam_rot = Cam_transf.eulerAngles;
										}
									}else{
										if(SmoothIn){//v2.2																			
											//particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor,MainColor,0.8f),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);

											//particles[i].color = new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,Mathf.Lerp(particles[i].color.a,(Global_tint + Color.Lerp (SunColor,MainColor,0.8f)).a,Time.deltaTime*SmoothInSpeed));
											particles[i].startColor = Color.Lerp(particles[i].startColor,Color.Lerp (particles[i].startColor, Global_tint + Color.Lerp (SunColor,MainColor,0.8f),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed*0.5f);

											if(Time.fixedTime - current_fadein_time > fade_in_time){
												SmoothIn = false;
											}else{
												//current_fadein_time = Time.deltaTime;
											}																			
										}
									}

									//								if(1==0){
									//								if(SmoothIn){//v2.2
									//									//if(!DecoupledWind){
									//									//if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
									//									//	particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color,  Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
									//									//}else{
									//									particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor,MainColor,0.8f),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
									//									
									//									if(Time.fixedTime - current_fadein_time > fade_in_time){
									//										SmoothIn = false;
									//									}else{
									//										//current_fadein_time = Time.deltaTime;
									//									} 
									//									
									//									//}
									//									//}
									//								}else if(SmoothOut){//v2.2
									//									//if(!DecoupledWind){
									//									//if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
									//									//particles[i].color =  Color.Lerp(Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
									//									//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
									//									//}else{
									//									//particles[i].color = Color.Lerp(Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
									//									//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
									//									//}
									//									
									//									if(ScatterMat != null){
									//										Color tintC = ScatterMat.GetVector ("_TintColor");
									//										ScatterMat.SetVector ("_TintColor", Color.Lerp(tintC,new Color(tintC.r,tintC.g,tintC.b,0),0.2f*Time.deltaTime*SmoothoutSpeed));
									//									}
									//									
									//									particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*100);
									//									//Debug.Log(SmoothoutSpeed);
									//									//}
									//								}
									//								}

									//v3.0.2
									if(StableRollMethod2){

										if(!StableRollAllAxis){
											float currentRot1 = Cam_transf.eulerAngles.z;
											float diffRot1 = currentRot1 - Prev_cam_rot.z;

											float currentRotY1 = Cam_transf.eulerAngles.y;
											if(diffRot1 == 0 && currentRotY1!=Prev_cam_rot.y && Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){

												float diffRotY1 = currentRotY1 - Prev_cam_rot.y;
												particles[i].rotation = particles[i].rotation+diffRotY1;
												//Debug.Log("aa");
											}else{
												if(Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 45){
													//particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
												}else{
													particles[i].rotation = particles[i].rotation+diffRot1;
												}
											}
										}else{

											//v3.4.9 - disabled old code below
//											if(diffRot == 0 && currentRotY!=Prev_cam_rot.y && Angle_check < 65){//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){									
//
//												//particles[i].rotation = particles[i].rotation-diffRotY*Dot_check;
//												particles[i].rotation = particles[i].rotation+diffRotY*Dot_check * 1; //v3.5
//												//Debug.Log("aa");
//											}else{
//												if(Angle_check < 45){
//													//particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
//													//particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
//												}else{
//													//particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
//												}
//												particles[i].rotation = particles[i].rotation-diffRot+diffRotY*Dot_check  * 1; //v3.5 
//											}

											//v3.4.9
											//float currentRot1 = Cam_transf.eulerAngles.z;
											//float diffRot1 = currentRot1 - Prev_cam_rot.z;

											//float currentRotY1 = Cam_transf.eulerAngles.y;
											if(diffRot == 0 && currentRotY!=Prev_cam_rot.y && Angle_check< 65){//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){

												//float diffRotY1 = currentRotY1 - Prev_cam_rot.y;
												particles[i].rotation = particles[i].rotation-diffRotY*Dot_check;
												//Debug.Log("aa");
											}else{
												if(Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 45){
													//particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
												}else{
													//particles[i].rotation = particles[i].rotation+diffRot1-diffRotY*Dot_check ;
												}
												particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check ;
											}
											//end v3.4.9

										}
									}

								}//END PARTILE LOOP

								if(Prev_cam_rot != Cam_transf.eulerAngles ){
									Prev_cam_rot = Cam_transf.eulerAngles;
								}

								DecoupledCounter ++;
								if(DecoupledCounter == particles.Length){
									DecoupledCounter = 0;
								}

								//GetComponent<ParticleEmitter>().particles = particles;
							}

						}
						//GetComponent<ParticleEmitter>().particles = particles;
						ShurikenParticle.SetParticles (particles,particles.Length); //v3.5
					}
				}
			}
			#endregion

			#region LEGACY
//			if(GetComponent<ParticleEmitter>()!=null){
//				Particle[] particles = new Particle[(int)GetComponent<ParticleEmitter>().maxEmission];//v3.5
//				if (DecoupledWind) {
//					particles = GetComponent<ParticleEmitter>().particles;
//				}
//
//		if(Time.fixedTime - Prev_time  > UpdateInterval | !Init){
//			Prev_time = Time.fixedTime;
//			
//					if (!DecoupledWind) {
//						particles = GetComponent<ParticleEmitter>().particles;
//					}
//			
//
//					//v3.0
//					if(Grab_sun_color){
//						
//						if(Override_sun){//if below cutoff, start night mode !!!!
//							//if(sun_transf.position.y < Cut_height){
//							
//							if(sun_transf.position.y < Cut_height){ //v1.7
//								MainColor = Color.Lerp (MainColor,Moon_dark_color,2.5f*Time.deltaTime);
//								
//								if(Moon_light){
//									if(sun_transf.position.y < Cut_height-10){ 
//										SunColor = Color.Lerp (SunColor,Moon_light_color,2.5f*Time.deltaTime);
//										if(keep_sun_transf != moon_transf){
//											keep_sun_transf = moon_transf;
//										}
//									}else{
//										SunColor = Color.Lerp (SunColor,Moon_dark_color,2.5f*Time.deltaTime);
//									}
//									
//									
//								}else{
//									SunColor = Color.Lerp (SunColor,Color.black,2.5f*Time.deltaTime);
//								}
//							}else{
//								MainColor = Color.Lerp (MainColor,MainColor_init,2.5f*Time.deltaTime);
//								SunColor = Color.Lerp (SunColor,Sun_Light.color,2.5f*Time.deltaTime);
//								
//								//						if(sun_transf != keep_sun_transf){
//								//							sun_transf = keep_sun_transf;
//								//						}
//								if(keep_sun_transf != sun_transf){
//									keep_sun_transf = sun_transf;
//								}
//							}
//						}else{
//							SunColor = Sun_Light.color;
//						}
//						
//						if(prev_intensity != Sun_Light.intensity){
//							//MainColor = Sun_Light.intensity*(MainColor_init/3);
//							
//							//v1.6
//							MainColor = Color.Lerp (MainColor, Sun_Light.intensity*(MainColor_init/3),2.5f*Time.deltaTime);
//							
//							prev_intensity = Sun_Light.intensity;
//						}
//						//ASSUME 3 intensity is daylight
//					}
//			
//				if(!Sorted){
//					//LEGACY
//			//		Particle[] particles = GetComponent<ParticleEmitter>().particles;
//					
//					//find particle number for each center
//					int particles_per_cloud = (int)(particles.Length/divider);
//					
//					if(!init_centers){
//						int count_me = 0;
//							//v2.2
//							if(VerticalForm){
//								for (int j=0; j < divider/2;j++){
//									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
//										particles[i].position = (particles[i].position  - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;
//										
//										particles[i].color = Global_tint + MainColor;
//										
//										int start_C = (i+(divider/2)*particles_per_cloud);
//										//int end_C = ((i+(divider/2))*particles_per_cloud);
//										
//										particles[start_C].position = (particles[start_C].position + new Vector3(0,Yspread,0) - This_transf.position)*Scale_per_cloud[j]*YScaleDiff + Centers[j]+MaxCloudDist ;
//										
//										particles[start_C].color = Color.Lerp(Global_tint + MainColor , HeightTint,HeightTint.a);
//										
//										//v2.2
//										if(SmoothIn){
//											particles[i].color = Color.Lerp(particles[i].color,Global_tint + MainColor,Time.deltaTime*SmoothInSpeed);
//											particles[start_C].color = Color.Lerp(particles[i].color,Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a),Time.deltaTime*SmoothInSpeed);
//											//Debug.Log(HeightTint.a);
//										}else{
//											particles[i].color = Global_tint + MainColor;
//											particles[start_C].color = Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a);
//										}
//
//										count_me = count_me+1;
//									}
//								}
//							}else{
//								for (int j=0; j < divider;j++){
//									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
//										particles[i].position = (particles[i].position - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;
//										count_me = count_me+1;
//									}
//								}
//							}
//						if(	count_me > 0){
//							init_centers=true; Init = true; 
//							GetComponent<ParticleEmitter>().particles = particles;
//						}
//					}else{
//						
//						//for each center !!!!!!!!
//						for (int j=0; j < divider;j++){
//							float speed2=speed;
//							speed2 = multiplier*speed*(j+1);
//							
//							//move centers
//							if(!DecoupledWind){
//								Centers[j] = Centers[j]+ wind*speed2*Time.deltaTime;
//							}
//							//	Debug.DrawLine(Centers[0],Centers[j]);
//							
//							float Dist_sun_center = (Centers[j] - keep_sun_transf.position).magnitude;
//							
//							//for specific center particles !!!
//							for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
//								
//								//find closest center and apply lighting based on that
//								Vector3 Closest_center = Centers[j];
//								int Closest_Center_ID=j;
//								for (int k=0; k<Centers.Count; k++) {
//									if(Vector3.Distance(Centers[k],particles[i].position) < Vector3.Distance(Closest_center,particles[i].position)){
//										Closest_center = Centers[k];
//										Closest_Center_ID=k;
//									}
//								}
//								Dist_sun_center = (Closest_center - keep_sun_transf.position).magnitude;
//								
//								particles[i].position = particles[i].position +wind*(multiplier*speed*(Closest_Center_ID+1))*Time.deltaTime; 
//								
//								float Dist_sun_part = (particles[i].position - keep_sun_transf.position).magnitude;
//								float Diff = (Dist_sun_part - (Dist_sun_center))  / 1.380f; // move to each center's center
//								
//								if(Prev_cam_rot == Cam_transf.eulerAngles){
//									particles[i].color = Color.Lerp (SunColor,MainColor,Diff);
//									
//								}else{
//									Prev_cam_rot = Cam_transf.eulerAngles;
//								}
//							}
//						}		
//						
//							if (!DecoupledWind) {
//								GetComponent<ParticleEmitter>().particles = particles;
//							}
//					}
//				}else if(Sorted){							///////////////////// SORTED CASE - go per particle, check closest center
//					//LEGACY
//		//			Particle[] particles = GetComponent<ParticleEmitter>().particles;
//					
//					//find particle number for each center
//					int particles_per_cloud = (int)(particles.Length/divider);
//					
//					if(!init_centers){
//						int count_me = 0;
//
//							//v2.2
//							if(VerticalForm){
//								for (int j=0; j < divider/2;j++){
//									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
//										particles[i].position = (particles[i].position - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;
//										
//										//particles[i].color = Global_tint + MainColor; //v2.2
//										
//										int start_C = (i+(divider/2)*particles_per_cloud);
//										//int end_C = ((i+(divider/2))*particles_per_cloud);
//										
//										particles[start_C].position = (particles[start_C].position + new Vector3(0,Yspread,0) - This_transf.position)*Scale_per_cloud[j]*YScaleDiff + Centers[j]+MaxCloudDist;
//
//										//v2.2
//										if(SmoothIn){
//											particles[i].color = Color.Lerp(particles[i].color,Global_tint + MainColor,Time.deltaTime*SmoothInSpeed);
//											particles[start_C].color = Color.Lerp(particles[i].color,Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a),Time.deltaTime*SmoothInSpeed);
//											//Debug.Log(HeightTint.a);
//										}else{
//											particles[i].color = Global_tint + MainColor;
//											particles[start_C].color = Color.Lerp(Global_tint + MainColor, HeightTint,HeightTint.a);
//										}
//
//										count_me = count_me+1;
//									}
//								}
//							}else{
//								for (int j=0; j < divider;j++){
//									for (int i=(j*particles_per_cloud); i<((j+1)*particles_per_cloud); i++) {
//										particles[i].position = (particles[i].position - This_transf.position)*Scale_per_cloud[j] + Centers[j] ;
//										count_me = count_me+1;
//									}
//								}
//							}
//						if(	count_me > 0){
//							init_centers=true; Init = true; 
//							GetComponent<ParticleEmitter>().particles = particles;
//						}
//					}
//					
//					if(init_centers){
//
//							//v2.2
//							bool hero_close_to_center = false;
//							//float Dist_hero_center = 0;
//
//						//move each center !!!!!!!!
//						int count_outers = 0;
//							int count_shadows = 0;
//							int count_all_shadows = 0;
//						for (int j=0; j < divider;j++){
//							float speed2=speed;
//							speed2 = multiplier*speed*(j+1);
//							
//							//move centers
//							if(!DecoupledWind){
//								Centers[j] = Centers[j]+ wind*speed2*Time.deltaTime;
//							}
//							//		Debug.DrawLine(Centers[0],Centers[j]);
//
//								//v2.2 - LOD
//								if(EnableLOD){
//									float Dist_hero_center1 = Vector3.Distance(Cam_transf.position,Centers[j]+new Vector3(0,120,0));
//									if(Dist_hero_center1 < LodMaxYDiff){
//										hero_close_to_center = true;
//										//Dist_hero_center = Dist_hero_center1;
//									}
//								}
//
//							//v1.7 - add shadow planes
//							if(!DecoupledWind){
//								if(Add_shadows & !shadows_created){
//									//v3.0
//									if(!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)){
//										if(count_all_shadows < Shadow_planes.Count){
//											Shadow_planes[count_all_shadows].position = Centers[j];
//										}
//										count_shadows=0;
//										count_all_shadows++;
//									}else{
//										count_shadows++;
//									}
//								}
//							}
//							
//							if(Restore_on_bound){
//								
//								if(Vector3.Distance(Centers[j],This_transf.position) > Bound){
//									count_outers = count_outers+1;
//								}
//								if(Disable_on_bound){
//
//										//v2.3 - fade out on bound
//										//if(FadeOutOnBoundary && count_outers >= Centers.Count - 10){
//										//if(FadeOutOnBoundary && count_outers >= (Centers.Count - (0.01f*Centers.Count)) ){
//										if(!SmoothOut && FadeOutOnBoundary && count_outers >= Centers.Count - 4){
//											//FadeOutOnBoundary//fade
//											SmoothIn = false;
//											SmoothOut = true;
//									//		current_smooth_out_time = Time.fixedTime;
//											SmoothoutSpeed = 0;
//											SmoothInSpeed = 0;
//										}
//
//									if(count_outers >= Centers.Count){
//										if(clone_on_end & !cloned){
//											//Instantiate(this.transform.gameObject);
//
//												shadows_created = true;//flag that shadows have been instantiated, for next instance
//
//												//v3.0
//												GameObject InstanceC = Instantiate(this.transform.gameObject);
//												InstanceC.GetComponent<VolumeClouds_SM>().SmoothIn = true;
//												InstanceC.GetComponent<VolumeClouds_SM>().SmoothOut = false;
//												InstanceC.GetComponent<VolumeClouds_SM>().current_fadein_time = Time.fixedTime;
//
//												SkyManager.currentWeather.VolumeScript = InstanceC.GetComponent<VolumeClouds_SM>();
//
//												//scale clouds
//												//InstanceC.GetComponent<VolumeClouds_SM>().ScaleClouds(SkyManager.WorldScale,SkyManager.VCloudCoverFac,SkyManager.VCloudSizeFac,SkyManager.VCloudCSizeFac);
//
//												cloned = true;
//										}
//										if(!DestroyOnfadeOut){
//											This_transf.gameObject.SetActive(false);
//											if(destroy_on_end){
//												Destroy(this.transform.gameObject);
//											}
//										}
//									}
//								}
//							}
//						}
//						
//						//for specific center particles !!!
////						if(alternate == 2){
////							alternate =1;
////						}else if (alternate == 1){
////							alternate =2;
////						}
//						int frames_divider = max_divider;
//						if(Time.fixedTime - Cloud_update_current < Cloud_spread_delay){
//							alternate = 2;
//							frames_divider =1;
//							//Cloud_update_current = Time.fixedTime;
//						}else if(Time.fixedTime - Cloud_update_current == Cloud_spread_delay){
//							alternate = 2;
//							frames_divider =1;
//						}
//						else{
//							if(alternate > max_divider+0){
//								alternate = 2;
//							}else{
//								alternate++;
//							}
//							//Debug.Log ("DIV="+frames_divider);
//							//Debug.Log ("ALT="+alternate);
//						}
//
//						for (int i=alternate-2; i<particles.Length; i=i+frames_divider) {
//							
//
//								if(!DecoupledColor){
//
//							//find closest center and apply lighting based on that	
//							int Closest_Center_ID=0;
//							Vector3 Closest_center = Centers[0];							
//							for (int k=0; k<Centers.Count; k++) {
//								//
//								if(Use2DCheck){
//									if(Vector2.Distance(new Vector2(Centers[k].x,Centers[k].z),new Vector2(particles[i].position.x,particles[i].position.z)) 
//									   < Vector3.Distance(new Vector2(Closest_center.x,Closest_center.z),new Vector2(particles[i].position.x,particles[i].position.z))){
//
//										Closest_center = Centers[k];
//										Closest_Center_ID = k;
//									}
//								}else{
//									if(Vector3.Distance(Centers[k],particles[i].position) < Vector3.Distance(Closest_center,particles[i].position)){
//										Closest_center = Centers[k];
//										Closest_Center_ID = k;
//									}
//								}
//							}
//							float Dist_sun_center = (Closest_center - keep_sun_transf.position).magnitude;
//							///		Debug.DrawLine(particles[i].position,Closest_center);		
//							
//							//v1.2.6
//							//int method =1;
//
//							//v.1.7 - sheet handle
//							//Random.seed = i;
//							particles[i].energy = 2;//Random.Range(1,4);
//							
//							if(Flatten_below){
//								float Dist_part_center = (Closest_center.y - particles[i].position.y);
//								if(Dist_part_center > 0){
//									particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y +(500)*Time.deltaTime,particles[i].position.z); 
//								}
//							}
//							
//							float dist = (keep_sun_transf.position - particles[i].position).magnitude/10;
//							
//							if(Turbulent){
//								
//								if(method ==1){//use for static camera, no extra sorting required, will break if camera rotates
//									
//									//particles[i].size = 3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f);
//									particles[i].size = Mathf.Abs(3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f));
//									if(particles[i].size < 330){
//										particles[i].size =330;
//									}else if(particles[i].size > 600){
//										particles[i].size =600;
//									}
//								}
//								else if(method ==2){
//									
//									if(particles[i].size < 300 | 1==1){
//										
//										particles[i].size = particles[i].size + 4f*200*Mathf.Cos (Time.fixedTime+dist*Time.fixedTime*0.0005f)*Time.deltaTime;
//										//particles[i].size = particles[i].size + 4f*200*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f)*Time.deltaTime;
//									}else{
//										//	particles[i].size = particles[i].size - 3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f);
//									}
//									if(particles[i].size < 300){
//										particles[i].size =300;
//									}else if(particles[i].size > 500){
//										particles[i].size =500;
//									}
//								}
//								else if(method == 3){
//									
//									float COS_IN = Time.fixedTime+dist*Time.fixedTime*0.0005f;
//									
//									COS_IN = Time.fixedTime+dist*0.115f;
//									
//									if(particles[i].size < 500 & particles[i].size > 200){
//										
//										particles[i].size = particles[i].size + 4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
//										//particles[i].size = particles[i].size + 4f*200*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f)*Time.deltaTime;
//									}else{
//										//	particles[i].size = particles[i].size - 3f*250*Mathf.Cos (Time.fixedTime+i*Time.fixedTime*0.0005f);
//									}
//									if(particles[i].size < 200){
//										//particles[i].size =100;//keep growing, but slower, until cos turns it around
//										if(Mathf.Cos (COS_IN) <0){
//											particles[i].size = particles[i].size + 0.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
//										}else{
//											particles[i].size = particles[i].size + 4.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
//										}
//									}else if(particles[i].size > 500){
//										//particles[i].size =500;
//										//keep growing, but slower, until cos turns it around
//										if(Mathf.Cos (COS_IN) > 0){
//											particles[i].size = particles[i].size + 0.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
//										}else{
//											particles[i].size = particles[i].size + 4.4f*200*Mathf.Cos (COS_IN)*Time.deltaTime;
//										}
//										
//									}
//								}
//								
//								particles[i].position = particles[i].position +wind*(multiplier*speed*(dist+1))*Time.deltaTime;
//							}else{
//									if(!DecoupledWind){
//										particles[i].position = particles[i].position +wind*(multiplier*speed*(Closest_Center_ID+1))*Time.deltaTime; 
//									}
//							}
//							
//							//v2.2 - LOD
//							if(EnableLOD & Prev_cam_rot == Cam_transf.eulerAngles){
//								//float LodMaxYDiff = 140;
//								//float LodMaxHDiff = 700;
//									float DistCP = Vector2.Distance(new Vector2(Cam_transf.position.x,Cam_transf.position.z),new Vector2(particles[i].position.x,particles[i].position.z));
//								//if(Mathf.Abs(Cam_transf.position.y - particles[i].position.y) < LodMaxYDiff 
//								if(//Mathf.Abs(Cam_transf.position.y - Closest_center.y) < LodMaxYDiff &
//									//Vector2.Distance(new Vector2(Cam_transf.position.x,Cam_transf.position.z),new Vector2(particles[i].position.x,particles[i].position.z)) > (LodMaxHDiff * (0.7f+(Dist_hero_center/LodMaxYDiff)))
//									 (DistCP > LodMaxHDiff | DistCP < LODMinDist)
//									 //Vector3.Distance(Cam_transf.position,particles[i].position) < LODMinDist)
//									& hero_close_to_center){
//									if(ParticlesLODed[i] == 0){
//										//particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y+LOD_send_height, particles[i].position.z);
//										//ParticlesLODed[i] = 1; 
//										ParticlesLODed[i] = (int)particles[i].size;
//										//particles[i].size = 0;
//										//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed);
//									}
//									if(ParticlesLODed[i] > 0){
//										particles[i].size = Mathf.Lerp(particles[i].size, 0 ,Time.deltaTime*LODFadeOutSpeed);
//										particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*LODFadeOutSpeed*2);//become transp. faster
//									}
//								}else{
//									//if(ParticlesLODed[i] == 1){
//									if(ParticlesLODed[i] > 0){
//										//particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y-LOD_send_height, particles[i].position.z);
//
//											if(particles[i].size < ParticlesLODed[i]){
//												particles[i].size = Mathf.Lerp(particles[i].size, ParticlesLODed[i] ,Time.deltaTime*LODFadeInSpeed*2);//become normal size before appear
//												particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,1),Time.deltaTime*LODFadeInSpeed);
//											}else{
//												ParticlesLODed[i] = 0; 
//											}
//
//										//particles[i].size = 410;										
//									}
//									//if(ParticlesLODed[i] == 0){
//									//		particles[i].size = Mathf.Lerp(particles[i].size, 0 ,Time.deltaTime*SmoothoutSpeed);
//									//}
//								}
//							}
//
//
//							//
//							
//							float Dist_sun_part = (particles[i].position - keep_sun_transf.position).magnitude;
//							float Diff = ((Dist_sun_part - (Dist_sun_center))+((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]))  /((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]); //normalize based on cloud size
//							
//							float Diff2 = 0;
//							if(Sun_dist_on){
//								float Dist_sun_GLOBALcenter = (This_transf.position - keep_sun_transf.position).magnitude;
//								Diff2 = ((Dist_sun_part - (Dist_sun_GLOBALcenter))+global_cloud_scale)  /global_cloud_scale;
//								Diff = Mathf.Lerp(Diff,Diff2,0.5f);
//							}
//							
//							Color SunColor2 = SunColor;
//							if(Grab_sun_angle){
//								float Angle_factor = Vector3.Angle((particles[i].position - keep_sun_transf.position),(This_transf.position - keep_sun_transf.position));
//								SunColor2.a = SunColor.a/(Mathf.Abs (Angle_factor)*Sun_angle_influence);
//							}
//							
//			//					if(!DecoupledColor){
//							if(Prev_cam_rot == Cam_transf.eulerAngles){
//								if(Init_color < Delay_lerp){
//										//v2.2
//										if(SmoothIn){
//											particles[i].color = Color.Lerp(particles[i].color,Global_tint + Color.Lerp (SunColor2,MainColor,Diff),Time.deltaTime*SmoothInSpeed);
//										}else{
//											particles[i].color = Global_tint + Color.Lerp (SunColor2,MainColor,Diff);
//										}
//									Init_color++;
//								}else{
//										//v1.6 - move tint inside
//										if(SmoothIn){//v2.2
//											if(!DecoupledColor){
//												if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
//													particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color,  Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
//												}else{
//													if(!DecoupledWind){
//														particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
//													}
//												}
//											}
//										}else if(SmoothOut){//v2.2
//											if(!DecoupledColor){
//												if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
//													//particles[i].color =  Color.Lerp(Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
//													//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
//												}else{
//													//particles[i].color = Color.Lerp(Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
//													//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
//												}
//
//												if(ScatterMat != null && ScatterMat.HasProperty("_TintColor")){ //v3.2
//													Color tintC = ScatterMat.GetVector ("_TintColor");
//													ScatterMat.SetVector ("_TintColor", Color.Lerp(tintC,new Color(tintC.r,tintC.g,tintC.b,0),0.01f*Time.deltaTime*SmoothoutSpeed));
//												}
//
//												particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*0.5f);
//												//Debug.Log(SmoothoutSpeed);
//											}
//										}
//										else{
//											if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
//													particles[i].color = Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime*0.005f);
//											}else{
//												particles[i].color = Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime*0.005f);
//											}
//										}
//								}
//							}else{
//								Prev_cam_rot = Cam_transf.eulerAngles;
//							}
//								}
//
//								//v2.2 - Cloud Wave
//								if(CloudWaves){
//									//if(SmoothIn & particles[i].color.a == 1){
//									//	SmoothIn = false;
//									//}
//									if(ParticlesLODed[i] == 0 & !SmoothIn){
//										//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,Mathf.Abs(Mathf.Cos(WaveFreq*Time.fixedTime+particles[i].position.x))),Time.deltaTime*LODFadeInSpeed);
//										//particles[i].color =new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,Mathf.Abs(Mathf.Cos(WaveFreq*Time.fixedTime+0.1f*Mathf.Sin (Vector3.Distance(particles[i].position,This_transf.position)))));
//										particles[i].color =new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,Mathf.Abs(Mathf.Cos(WaveFreq*Time.fixedTime+0.005f* (Vector3.Distance(particles[i].position,This_transf.position)))));
//									}
//								}
//
//						}// END PARTICLE CYCLE							
//						
//							///////////////////////// DECOUPLE WIND
//
//
//
//							if (!DecoupledWind) {
//								GetComponent<ParticleEmitter>().particles = particles;
//							}
//					}
//				}//END if SORTED LEGACY
//
//			
//		}//END UPDATE INTERVAL CHECK
//
//			if (DecoupledWind) {
//
//					int count_shadows = 0;
//					int count_all_shadows = 0;
//
//				for (int j=0; j < divider; j++) {
//					//float speed2 = speed;
//					//speed2 = multiplier * speed * (j + 1);
//					
//					//move centers
//					//if (!DecoupledWind) {
//					//Centers [j] = Centers [j] + wind * speed2 * Time.deltaTime;
//
//						if(!DifferentialMotion){
//							Centers [j] = Centers [j] + wind * (multiplier * speed*5) * Time.deltaTime; 
//						}else{
//							//Centers [j] = Centers [j] + wind * (multiplier * speed*5) * Time.deltaTime * (MaxDiffSpeed*((j+1)/divider)); 
//							Centers [j] = Centers [j] + wind * (multiplier * speed*5) * Time.deltaTime * (MaxDiffSpeed*((j+MaxDiffOffset)/divider));
//							//Debug.Log("in1");
//						}
//					//}
//
//						if(Add_shadows){
//							//v3.0
//							if(!Divide_lightning || (Divide_lightning & count_shadows == LightningDivider)){
//								if(count_all_shadows < Shadow_planes.Count){
//									Shadow_planes[count_all_shadows].position = Centers[j];
//								}
//								count_shadows=0;
//								count_all_shadows++;
//							}else{
//								count_shadows++;
//							}
//						}
//
//				}
//				//if (GetComponent<ParticleEmitter> () != null) 
//				{
//					
//					if (Sorted) 
//					{
//						
//		//				Particle[] particles = GetComponent<ParticleEmitter> ().particles;
//						
//
//
//
//						//find particle number for each center
//						//int particles_per_cloud = (int)(particles.Length / divider);
//						
//						if (init_centers) 
//						{
//							//v3.1
//							float currentRot = Cam_transf.eulerAngles.z;
//							float diffRot = currentRot - Prev_cam_rot.z;
//
//							float currentRotY = Cam_transf.eulerAngles.y;
//							float diffRotY = currentRotY - Prev_cam_rot.y;
//
//								if (Mathf.Abs(diffRotY) > 180) {
//									diffRotY = 360-Mathf.Abs(diffRotY);
//								}
//
//							float Angle_check = 0;
//							float Dot_check = 0;
//							//v3.1
//							if(StableRollMethod2) {
//								Angle_check = Mathf.Abs(Vector3.Angle (Cam_transf.forward, Mathf.Sign(Cam_transf.forward.y)*Vector3.up));
//								Dot_check = -Mathf.Sign(Cam_transf.forward.y)*Vector3.Dot(Cam_transf.forward, Mathf.Sign (Cam_transf.forward.y) * Vector3.up);
//							}
//
//							//Debug.Log(particles.Length);
//							for (int i=0; i<particles.Length; i=i+1) {
//								
//								if(!DifferentialMotion){
//									particles [i].position = particles [i].position + wind * (multiplier * speed*5) * Time.deltaTime; 
//								}else{
//									particles [i].position = particles [i].position + wind * (multiplier * speed*5) * Time.deltaTime* (MaxDiffSpeed*((i+MaxDiffOffset)/particles.Length)); 
//								}
//
//									//v3.1
//									if(renewAboveHeight){
//										if(particles[i].position.y > renewHeight){
//											//if(Random.Range(1,1000) == 3){
//												particles[i].position = new Vector3(particles[i].position.x, This_transf.position.y, particles[i].position.z);
//											//}
//										}
//										if (particles [i].position.y > (renewHeightPercent / 100)*renewHeight) {//just before go away, fade out //v3.3b
//											particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*renewFadeOutSpeed*5.5f);//v3.3b
//										}else{
//										//if (particles [i].position.y < (15 / 100) * renewHeight) {
//											particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,(Global_tint + Color.Lerp (SunColor,MainColor,0.8f)).a),Time.deltaTime*renewFadeInSpeed*10.5f);
//										}
//
//										if (particles [i].position.y < This_transf.position.y+0.1*This_transf.position.y) {
//											particles[i].color = new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0);//v3.3b
//										}//else
//										//if (particles [i].position.y < This_transf.position.y + 0.3 * This_transf.position.y) {
//											
//										//}
//									}
//
//
//								if(DecoupledColor){
//
//								int Closest_Center_ID=0;
//								Vector3 Closest_center = Centers[0];
//
//
//
//								if(DecoupledCounter == i || CenterIDs.Count < particles.Length){// || Prev_cam_rot != Cam_transf.eulerAngles){
//									for (int k=0; k<Centers.Count; k++) {
//										//
//										if(Use2DCheck){
//											if(Vector2.Distance(new Vector2(Centers[k].x,Centers[k].z),new Vector2(particles[i].position.x,particles[i].position.z)) 
//											   < Vector3.Distance(new Vector2(Closest_center.x,Closest_center.z),new Vector2(particles[i].position.x,particles[i].position.z))){
//												
//												Closest_center = Centers[k];
//												Closest_Center_ID = k;
//											}
//										}else{
//											if(Vector3.Distance(Centers[k],particles[i].position) < Vector3.Distance(Closest_center,particles[i].position)){
//												Closest_center = Centers[k];
//												Closest_Center_ID = k;
//											}
//										}
//										//break;
//
//									}
//
//									//Debug.Log("particles len ="+particles.Length +" centers ID"+CenterIDs.Count);
//
//								}else{
//									Closest_Center_ID = CenterIDs[i];
//									Closest_center = Centers[Closest_Center_ID];
//								}
//
//								if(CenterIDs.Count < particles.Length){
//									CenterIDs.Add(Closest_Center_ID);
//								}
//
//								float Dist_sun_center = (Closest_center - keep_sun_transf.position).magnitude;
//
//									particles[i].energy = 2;//Random.Range(1,4);
//									
//									if(Flatten_below){
//										float Dist_part_center = (Closest_center.y - particles[i].position.y);
//										if(Dist_part_center > 0){
//											particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y +(500)*Time.deltaTime,particles[i].position.z); 
//										}
//									}
//
//								float Dist_sun_part = (particles[i].position - keep_sun_transf.position).magnitude;
//								float Diff = ((Dist_sun_part - (Dist_sun_center))+((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]))  /((cloud_scale/Sun_exposure)*Scale_per_cloud[Closest_Center_ID]); //normalize based on cloud size
//								
//								float Diff2 = 0;
//								if(Sun_dist_on){
//									float Dist_sun_GLOBALcenter = (This_transf.position - keep_sun_transf.position).magnitude;
//									Diff2 = ((Dist_sun_part - (Dist_sun_GLOBALcenter))+global_cloud_scale)  /global_cloud_scale;
//									Diff = Mathf.Lerp(Diff,Diff2,0.5f);
//								}
//								
//								Color SunColor2 = SunColor;
//								if(Grab_sun_angle){
//									float Angle_factor = Vector3.Angle((particles[i].position - keep_sun_transf.position),(This_transf.position - keep_sun_transf.position));
//									SunColor2.a = SunColor.a/(Mathf.Abs (Angle_factor)*Sun_angle_influence);
//								}
//
//
//
//
//
//							//	if(DecoupledColor){
//									if(Prev_cam_rot == Cam_transf.eulerAngles ){
//										if(Init_color < Delay_lerp){
//											//v2.2
//											if(SmoothIn){
//												particles[i].color = Color.Lerp(particles[i].color,Global_tint + Color.Lerp (SunColor2,MainColor,Diff),Time.deltaTime*SmoothInSpeed);
//											}else{
//												particles[i].color = Global_tint + Color.Lerp (SunColor2,MainColor,Diff);
//											}
//											Init_color++;
//										}else{
//											//v1.6 - move tint inside
//											if(SmoothIn){//v2.2
//												//if(!DecoupledWind){
//													if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
//														particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color,  Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
//													}else{
//														particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
//													}
//
//												if(Time.fixedTime - current_fadein_time > fade_in_time){
//													SmoothIn = false;
//												}
//												//}
//											}else if(SmoothOut){//v2.2
//												//if(!DecoupledWind){
//													if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
//														//particles[i].color =  Color.Lerp(Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
//														//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
//													}else{
//														//particles[i].color = Color.Lerp(Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
//														//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
//													}
//													
//													if(ScatterMat != null){
//														Color tintC = ScatterMat.GetVector ("_TintColor");
//														ScatterMat.SetVector ("_TintColor", Color.Lerp(tintC,new Color(tintC.r,tintC.g,tintC.b,0),0.2f*Time.deltaTime*SmoothoutSpeed));
//													}
//													
//													particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*100);
//													//Debug.Log(SmoothoutSpeed);
//												//}
//											}
//											else{
//												if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
//													particles[i].color = Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime);
//												}else{
//													particles[i].color = Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime);
//												}
//											}
//										}
//									}else{
//										//Prev_cam_rot = Cam_transf.eulerAngles;
//									}
//								}else{
//										if(SmoothIn){//v2.2																			
//											//particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor,MainColor,0.8f),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
//											
//												//particles[i].color = new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,Mathf.Lerp(particles[i].color.a,(Global_tint + Color.Lerp (SunColor,MainColor,0.8f)).a,Time.deltaTime*SmoothInSpeed));
//												particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor,MainColor,0.8f),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed*0.5f);
//
//											if(Time.fixedTime - current_fadein_time > fade_in_time){
//												SmoothIn = false;
//											}else{
//												//current_fadein_time = Time.deltaTime;
//											}																			
//										}
//									}
//
////								if(1==0){
////								if(SmoothIn){//v2.2
////									//if(!DecoupledWind){
////									//if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
////									//	particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color,  Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
////									//}else{
////									particles[i].color = Color.Lerp(particles[i].color,Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor,MainColor,0.8f),color_speed*Time.deltaTime),Time.deltaTime*SmoothInSpeed);
////									
////									if(Time.fixedTime - current_fadein_time > fade_in_time){
////										SmoothIn = false;
////									}else{
////										//current_fadein_time = Time.deltaTime;
////									} 
////									
////									//}
////									//}
////								}else if(SmoothOut){//v2.2
////									//if(!DecoupledWind){
////									//if(VerticalForm & (particles[i].position.y - Closest_center.y) > YScaleDiff){
////									//particles[i].color =  Color.Lerp(Color.Lerp (particles[i].color, Color.Lerp(Global_tint + Color.Lerp (SunColor2,MainColor,Diff),HeightTint,HeightTint.a),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
////									//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
////									//}else{
////									//particles[i].color = Color.Lerp(Color.Lerp (particles[i].color, Global_tint + Color.Lerp (SunColor2,MainColor,Diff),color_speed*Time.deltaTime),new Color(0,0,0,0),Time.deltaTime*SmoothoutSpeed);
////									//particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*1000);
////									//}
////									
////									if(ScatterMat != null){
////										Color tintC = ScatterMat.GetVector ("_TintColor");
////										ScatterMat.SetVector ("_TintColor", Color.Lerp(tintC,new Color(tintC.r,tintC.g,tintC.b,0),0.2f*Time.deltaTime*SmoothoutSpeed));
////									}
////									
////									particles[i].color = Color.Lerp(particles[i].color,new Color(particles[i].color.r,particles[i].color.g,particles[i].color.b,0),Time.deltaTime*SmoothoutSpeed*100);
////									//Debug.Log(SmoothoutSpeed);
////									//}
////								}
////								}
//
//								//v3.0.2
//								if(StableRollMethod2){
//
//										if(!StableRollAllAxis){
//											float currentRot1 = Cam_transf.eulerAngles.z;
//											float diffRot1 = currentRot1 - Prev_cam_rot.z;
//
//											float currentRotY1 = Cam_transf.eulerAngles.y;
//											if(diffRot1 == 0 && currentRotY1!=Prev_cam_rot.y && Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){
//
//												float diffRotY1 = currentRotY1 - Prev_cam_rot.y;
//												particles[i].rotation = particles[i].rotation+diffRotY1;
//												//Debug.Log("aa");
//											}else{
//												if(Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 45){
//													//particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
//												}else{
//													particles[i].rotation = particles[i].rotation+diffRot1;
//												}
//											}
//										}else{
//
//											if(diffRot == 0 && currentRotY!=Prev_cam_rot.y && Angle_check < 65){//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){									
//												
//												particles[i].rotation = particles[i].rotation-diffRotY*Dot_check;
//												//Debug.Log("aa");
//											}else{
//												if(Angle_check < 45){
//													//particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
//														particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
//												}else{
//														particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
//												}
//											}
//										}
//								}
//
//							}//END PARTILE LOOP
//
//							if(Prev_cam_rot != Cam_transf.eulerAngles ){
//								Prev_cam_rot = Cam_transf.eulerAngles;
//							}
//
//							DecoupledCounter ++;
//							if(DecoupledCounter == particles.Length){
//								DecoupledCounter = 0;
//							}
//
//							//GetComponent<ParticleEmitter>().particles = particles;
//						}
//						
//					}
//					GetComponent<ParticleEmitter>().particles = particles;
//				}
//			}
//			}
			#endregion

	}//END UPDATE

		ParticleSystem.Particle[] particles;

		public List<int> CenterIDs = new List<int>();
		public int DecoupledCounter = 0;

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
