using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Artngame.PDM;

namespace Artngame.SKYMASTER {

[ExecuteInEditMode()]
	public class ParticlePropagationSKYMASTER : MonoBehaviour {
	
		//overrides
		public List<GameObject> Systems_To_override;
		private List<ParticlePropagationSKYMASTER> systems_To_override;//use to cache scripts
		public float cut_off_dist=0.1f; //max distance to check other system for painted positions
		public bool reset_overrides=false;

		public enum PaintType {Painted, BrushPainted, SkinnedMesh,SimpleMesh,Propagated,Projected, Spline, ParticleCollision, ParticlePosition};
		[HideInInspector]
		public List<PaintType> PaintTypes; 

	void Start () {

			systems_To_override = new List<ParticlePropagationSKYMASTER>();
			
			if(Systems_To_override !=null){
				if(Systems_To_override.Count >0){
					for(int i=0;i< Systems_To_override.Count;i++){

						systems_To_override.Add(Systems_To_override[i].GetComponent(typeof(ParticlePropagationSKYMASTER)) as ParticlePropagationSKYMASTER);
						
					}
				}
			}

		Grab_time=Time.fixedTime;
		Grab_time_calcs=Time.fixedTime;
	
			if(p11==null){
				p11=this.gameObject.GetComponent<ParticleSystem>();
				
				if(p11!=null & gameobject_mode){
					//p11.main.maxParticles = particle_count; //v3.4.9
					ParticleSystem.MainModule MainModu = p11.main;
					MainModu.maxParticles = particle_count; //v3.4.9

					p11.Emit(particle_count);
				}
				
			}

			noise = new PerlinSKYMASTER ();				
						
			Current_Grow_time = Time.fixedTime;

			if(Particle_color==null){
				Particle_color = new List<Vector4>();
				
				for(int i=0;i<Registered_paint_positions.Count;i++){
					Particle_color.Add(new Vector4(0,0,0,999));
				}
			}else{
				if(Particle_color.Count != Registered_paint_positions.Count){
					Particle_color.Clear();
					for(int i=0;i<Registered_paint_positions.Count;i++){
						Particle_color.Add(new Vector4(0,0,0,999));
					}
				}
			}
		
		}

		//GAMEOBJECTS
		#region GAMEOBJECTS
		public bool gameobject_mode = false;

		public bool Scale_by_texture = false;
		public bool Color_by_texture = false;
		public bool Follow_scale = false;

		public bool Grow_trees;
		public float Grow_time=1f;
		private float Current_Grow_time;
		
		public bool Preview_mode=false;
		public int particle_count = 100;
		[HideInInspector]
		public List<Transform> Gameobj_instances;
		public GameObject Gameobj;
		public bool Gravity_Mode=false;
		public float grav_factor=0.1f;
		public float Y_offset=0f;
		public float X_offset_factor=0.005f;
		public float Z_offset_factor=0.007f;
		public bool let_loose = false;
		
		public GameObject Parent_OBJ;
		
		public bool Angled=false;
		public bool Asign_rot=false;
		public Vector3 Local_rot = Vector3.zero;
		private PerlinSKYMASTER  noise;
		public float Wind_speed=1f;
		
		public bool follow_particles=false;
		public bool Remove_colliders=false;

		public bool look_at_direction=false;

		//v1.6
		//Add look at normal
		public bool look_at_normal=false;
		private ControlCombineChildrenSKYMASTER Combiner;
		public bool enable_combine = false;

		//v1.2.2
		[HideInInspector]
		public List<Vector3> Updated_gameobject_positions;

		public float Release_Gravity = 0.05f;

		#endregion
		//END GAMEOBJECTS

	void Awake () {
		p11 = this.gameObject.GetComponent("ParticleSystem") as ParticleSystem;

			if(p11==null){
				Debug.Log ("Please attach the script to a particle system");
			}

		Flammable_objects = GameObject.FindGameObjectsWithTag("Flammable");

		Flamer_objects = GameObject.FindGameObjectsWithTag("Flamer");

	}

		public bool Use_stencil=false;
		public Texture2D Stencil;
		public Vector2 tex_scale= new Vector2(3,3);
		public float Coloration_ammount = 0.5f;
		public bool Real_time_painting=false;

		public bool Color_effects=false;
		public bool Lerp_color=false;
		public bool Keep_color=false;
		[HideInInspector]
		public List<Vector4> Particle_color;
		public Vector2 Stencil_divisions = new Vector2(8,8);

	 GameObject[] Flammable_objects;

	 GameObject[] Flamer_objects;

		public bool propagation = true;

	[HideInInspector]
	public int maxemitter_count;
	[HideInInspector]
	public int current_emitters_count;

	public float brush_size=1f;

	public bool Erase_mode=false;
	public float Marker_size=0.5f;

	public ParticleSystem p11;

	[HideInInspector]
	public List<Transform> Emitter_objects;
	[HideInInspector]
	public List<Vector3> Registered_paint_positions; 

		//v1.4
		public float stay_time=2f;
		[HideInInspector]
		public List<Vector3> Registered_paint_rotations; 
		[HideInInspector]
		public List<float> Registered_paint_times; 
		[HideInInspector]
		public List<float> Registered_paint_size; 
		public List<GameObject> Projectors;
		public float max_growth_size = 1f;
		public float min_propagation_dist=3f;
		public float max_propagation_dist=6f;
		List<Vector3> ray_dest_positions = new List<Vector3>(); 
		bool got_random_offsets=false;
		public bool go_random=false;
		Vector2[] rand_offsets;
		public float extend=1f;
		private bool randomize=false;
		public bool follow_normals=false;//v1.4
		public float density_dist_factor=3f;
		public int propagation_chance_factor = 1; // higher increases chance to skip
		public bool use_projection=false;
		public bool use_particle_collisions=false;

		[HideInInspector]
		public List<Vector3> Updated_Registered_paint_positions; 
		public bool By_layer=false;
		public List<string> Layers;

		public bool keep_alive=false;
		public bool Grow_ice_mesh=false; //use to trigger the Ice_Grow_PDM script on a mesh particle - need Flammable tag on script item
		public bool variant_size=false;
		public bool Vary_gameobj_size=false;

		public float Random_size_upper_bound=2;
		public float Random_size_lower_bound=0.9f;
		[HideInInspector]
		public float Random_size_factor;
		public float debug_rot=0;

		public bool is_ice=false;
		public bool is_fire=false;//use in collision script, add 1 if opposite target (fire source, ice target etc)
		public bool is_butterfly=false;//use in collision script, add 2 in overides in close by painted positions to colisions

		public bool enable_overides=false; // check in melt and let_loose code, if 1 melt locally, if 2 let_loose locally
		[HideInInspector]
		public List<int> LocalOverrides;

		public bool enable_melt;//check opposite systems, if ice near fire, make go out
		public bool enable_freeze=false; //use when fire goes out, inject FreezeBurnControlSKYMASTER script, increase counter if exists
		public bool enable_burn=false;

		public float max_burn_ammount=25; //inject to Propagation script
		public float max_freeze_ammount=25;
		public float Thaw_speed=0;
		public float Freeze_speed=0.15f;

		public bool enable_flyaway=false;
		public float melt_speed = 0.2f;
		public float fast_melt_speed = 0.4f;
		public bool enable_LocalWind=false;
		public bool is_grass=false;

		public float overide_GrassAngle=0;
		public float overide_WindSpeed=0;

	[HideInInspector]
	public List<Vector3> Registered_initial_positions;
	[HideInInspector]
	public List<Quaternion> Registered_initial_rotation;
	[HideInInspector]
	public List<Vector3> Registered_initial_scale;

	ParticleSystem.Particle[] ParticleList;

	private float Grab_time; //propgation last time
	public float Delay=1; //propagtion delay
	public bool Optimize=false; //optimize propagation
		public bool Use_Lerp =false;

		private float Grab_time_calcs; //particle position calcs last time
		public float Delay_calcs=1; //particle position calcs delay
		public bool Optimize_calcs=false; //particle position calcs optimize

	public bool relaxed = true;
	
	public bool draw_in_sequence;

		//v1.4
		public bool Velocity_toward_normal=false;
		public Vector3 Normal_Velocity=new Vector3(0,0,0);
		public float keep_in_position_factor =0.90f;
		public float keep_alive_factor =0.1f;

	void LateUpdate () {

		if(p11 == null){return;}


			//v1.6
			if(Parent_OBJ!=null){
				if(enable_combine){
					if(Combiner==null){
						Combiner = Parent_OBJ.GetComponent(typeof(ControlCombineChildrenSKYMASTER)) as ControlCombineChildrenSKYMASTER;
						//make sure auto disable is on
						if(Combiner!=null){
							Combiner.Auto_Disable=true;
						}else{
							Parent_OBJ.AddComponent(typeof(ControlCombineChildrenSKYMASTER));
							Combiner = Parent_OBJ.GetComponent(typeof(ControlCombineChildrenSKYMASTER)) as ControlCombineChildrenSKYMASTER;
							Combiner.Auto_Disable=true;
						}
					}
				}
			}

			//v1.4
			if(Gameobj_instances !=null ){
				if(Gameobj_instances.Count>0  & Updated_gameobject_positions !=null){
				
					for(int i=0;i<Gameobj_instances.Count;i++){

						//v1.6
//						if(Parent_OBJ!=null){
//							if(enable_combine){
//								//if(Updated_gameobject_positions[i] != Gameobj_instances[i].position){
//								if((Updated_gameobject_positions[i] - Gameobj_instances[i].position).magnitude>0.01f){
//						//			Combiner.active=true;
//								}
//							}
//						}

						//v1.4
						if(  (Updated_gameobject_positions[i] - Gameobj_instances[i].position).magnitude >0.1f){
							Updated_gameobject_positions[i] = Gameobj_instances[i].position;
						}else{
							Gameobj_instances[i].position = Updated_gameobject_positions[i];
						}
					}				
				}
			}

			//v1.4
			if(Registered_paint_times == null & Application.isPlaying){ //grab time for those that were already painted in editor mode

				for(int i = 0;i<Registered_paint_positions.Count;i++){

					Registered_paint_times.Add(Time.fixedTime);

				}

			}
			if(Projectors!=null & Application.isPlaying){

				for (int i=Projectors.Count-1;i>=0 ;i--){
					if(Projectors[i] == null)
					{
						Projectors.RemoveAt(i);
					}
				}

			}

			Random_size_factor=0;
			if(variant_size){

				Random_size_factor = Random.Range(-Random_size_lower_bound,Random_size_upper_bound);
			}

			//reset overrides
			if(reset_overrides){
				for(int i =0;i< LocalOverrides.Count;i++){

					LocalOverrides[i] =0;

				}
				reset_overrides=false;

			}

			//GAMEOBJECTS
			#region GAMEOBJECTS
			if(gameobject_mode){

				if(Preview_mode & !Application.isPlaying){
					
					if(Gameobj_instances!=null){
						
						if(Parent_OBJ.transform.childCount > Gameobj_instances.Count){
							
							for(int i=Parent_OBJ.transform.childCount-1;i>=0;i--){
								DestroyImmediate(Parent_OBJ.transform.GetChild(i).gameObject);
							}							
						}						
					}					
				}
				
				if(Gameobj_instances==null){
					Gameobj_instances = new List<Transform>();
				}				
				
				if(!Preview_mode & !Application.isPlaying){
					
					for(int i=Gameobj_instances.Count-1;i>=0;i--){
						
						DestroyImmediate(Gameobj_instances[i].gameObject);
					}					
				}
				
				if(Gameobj_instances!=null){
					for(int i=Gameobj_instances.Count-1;i>=0;i--){
												
						if(Gameobj_instances[i] ==null){
							
							Gameobj_instances.RemoveAt(i);
							Updated_gameobject_positions.RemoveAt(i);
						}
					}
				}
			}
			#endregion
			//END GAMEOBJECTS

			if(Time.fixedTime-Grab_time>Delay | !Optimize){
			
			Grab_time=Time.fixedTime;

		if(Registered_paint_positions!=null){
			for (int i=Registered_paint_positions.Count-1;i>=0 ;i--){
								
				if(enable_melt){

							for(int j=0;j<systems_To_override.Count;j++){

								if((systems_To_override[j].is_ice & is_fire)|(systems_To_override[j].is_fire & is_ice)){
									for (int k=systems_To_override[j].Registered_paint_positions.Count-1;k>=0 ;k--){

										if(Vector3.Distance(Registered_paint_positions[i],systems_To_override[j].Registered_paint_positions[k]) < cut_off_dist)
										{
											LocalOverrides[i] = 1;
										}
									}
								}
							}
				}

				if(Emitter_objects[i] == null ) //v1.4
				{
					Updated_Registered_paint_positions.RemoveAt(i);
					Particle_color.RemoveAt(i);

					Registered_paint_rotations.RemoveAt(i);
					Registered_paint_times.RemoveAt(i);
					Registered_paint_size.RemoveAt(i);
					Registered_paint_positions.RemoveAt(i);
					Registered_initial_positions.RemoveAt(i);
					Registered_initial_rotation.RemoveAt(i);
					Registered_initial_scale.RemoveAt(i);
					Emitter_objects.RemoveAt(i);
					
						LocalOverrides.RemoveAt(i);
						PaintTypes.RemoveAt(i);

						//GAMEOBJECTS
						#region GAMEOBJECTS
						if(gameobject_mode){
							if(i < Gameobj_instances.Count){ //v3.4.9c
								Destroy(Gameobj_instances[i].gameObject);
								Gameobj_instances.RemoveAt(i);
								Updated_gameobject_positions.RemoveAt(i);
							}
						}
						#endregion
						//END GAMEOBJECTS
					
				}
				else if( 1==1 & (Application.isPlaying & (!keep_alive | (enable_overides & (LocalOverrides[i] == 1 | LocalOverrides[i] == 3) ) ) & Emitter_objects[i].gameObject.tag != "Flamer" & ( (Time.fixedTime - Registered_paint_times[i]) > stay_time) ) ) //v1.4
				{
							//BURN-FREEZE
							if(enable_freeze & Emitter_objects[i].gameObject.tag =="Freezable"){
								
								//search for script in emitter, if not there inject it
								FreezeBurnControlSKYMASTER Burner = Emitter_objects[i].gameObject.GetComponent(typeof(FreezeBurnControlSKYMASTER)) as FreezeBurnControlSKYMASTER;
								
								if(Burner == null){
									Emitter_objects[i].gameObject.AddComponent(typeof(FreezeBurnControlSKYMASTER));
									Burner = Emitter_objects[i].gameObject.GetComponent(typeof(FreezeBurnControlSKYMASTER)) as FreezeBurnControlSKYMASTER;

									Burner.max_burn_ammount=max_burn_ammount; //inject to Propagation script
									Burner.max_freeze_ammount=max_freeze_ammount;
									Burner.Thaw_speed=Thaw_speed;
									Burner.Freeze_speed=Freeze_speed;
								}
								if(Burner.freeze_ammount < Burner.max_freeze_ammount){
									Burner.freeze_ammount += 1; 
								}
								
							}
							if(enable_burn & Emitter_objects[i].gameObject.tag =="Flammable"){
								
								//search for script in emitter, if not there inject it
								FreezeBurnControlSKYMASTER Burner = Emitter_objects[i].gameObject.GetComponent(typeof(FreezeBurnControlSKYMASTER)) as FreezeBurnControlSKYMASTER;
								
								if(Burner == null){

									Emitter_objects[i].gameObject.AddComponent(typeof(FreezeBurnControlSKYMASTER));
									Burner = Emitter_objects[i].gameObject.GetComponent(typeof(FreezeBurnControlSKYMASTER)) as FreezeBurnControlSKYMASTER;

									Burner.max_burn_ammount=max_burn_ammount; //inject to Propagation script
									Burner.max_freeze_ammount=max_freeze_ammount;
									Burner.Thaw_speed=Thaw_speed;
									Burner.Freeze_speed=Freeze_speed;
								}
								if(Burner.burn_ammount < Burner.max_burn_ammount){
									Burner.burn_ammount += 1; 
								}							
								
							}
							//END BURN-FREEZE

						Updated_Registered_paint_positions.RemoveAt(i);
							Particle_color.RemoveAt(i);

						Registered_paint_rotations.RemoveAt(i);
						Registered_paint_times.RemoveAt(i);
						Registered_paint_size.RemoveAt(i);
						Registered_paint_positions.RemoveAt(i);
						Registered_initial_positions.RemoveAt(i);
						Registered_initial_rotation.RemoveAt(i);
						Registered_initial_scale.RemoveAt(i);
						Emitter_objects.RemoveAt(i);

						LocalOverrides.RemoveAt(i);
						PaintTypes.RemoveAt(i);

						//GAMEOBJECTS
						#region GAMEOBJECTS
						if(gameobject_mode){
							Destroy(Gameobj_instances[i].gameObject);
							Gameobj_instances.RemoveAt(i);
							Updated_gameobject_positions.RemoveAt(i);
						}
						#endregion
						//END GAMEOBJECTS						

				}
			}
		}

		if(Application.isPlaying & propagation){

		Flamer_objects = GameObject.FindGameObjectsWithTag("Flamer");
		Flammable_objects = GameObject.FindGameObjectsWithTag("Flammable");

		for(int i=0;i<Flammable_objects.Length;i++){

					if(Emitter_objects!=null){
			for(int j=0;j<Emitter_objects.Count;j++){
				if( Emitter_objects[j] != null & Flammable_objects[i]!= null  ){
									if( Vector3.Distance(Emitter_objects[j].position,Flammable_objects[i].transform.position) <max_propagation_dist &
									   Vector3.Distance(Emitter_objects[j].position,Flammable_objects[i].transform.position) >min_propagation_dist ){ 

					Vector3 point = Emitter_objects[j].position; 

					point = Flammable_objects[i].transform.InverseTransformPoint(point);
					
					Vector3 nearestVertex = Vector3.zero;
					Vector3 nearestNormal = Vector3.zero;
					
					RaycastHit hit1 = new RaycastHit();
					if(Physics.Raycast(Emitter_objects[j].position, (Flammable_objects[i].transform.position - Emitter_objects[j].position ), out hit1,Mathf.Infinity))
					{
							if(hit1.collider.gameObject.tag == "Flammable"){

								nearestVertex = hit1.point;
								nearestNormal = hit1.normal;
							}
							else{
								nearestVertex = Vector3.zero;
								nearestNormal = Vector3.zero;
							}
					}

					Vector3 result = Flammable_objects[i].transform.TransformPoint(nearestVertex);
					result =nearestVertex;

					if(result != Vector3.zero){
											if(Emitter_objects.Count > (p11.main.maxParticles/2) ){//if(Emitter_objects.Count > (p11.emissionRate/2) ){ //SMv3.1
							//do nothing
						}else{

										int is_close_to_other_point_on_object=0; 

												for(int k=0;k<Registered_paint_positions.Count;k++){

													if(Emitter_objects[k].gameObject == Flammable_objects[i]){
														
														if(Vector3.Distance( Registered_paint_positions[k],result )<density_dist_factor){
															is_close_to_other_point_on_object=1;
														}
													}
												}

												//// lower chances to get a propagtion, if not a flamer
												if(Random.Range(1,propagation_chance_factor+1) != propagation_chance_factor){

													is_close_to_other_point_on_object =1;

												}

										if(is_close_to_other_point_on_object==0){ 

											Registered_paint_positions.Add(result);
													Updated_Registered_paint_positions.Add(result);
													Particle_color.Add(new Vector4(0,0,0,999));
													Registered_paint_rotations.Add(nearestNormal);
													Registered_paint_times.Add (Time.fixedTime);

													//v1.6
													if(!gameobject_mode){
														Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor); //v3.4.9 instead of startSiz e
													}

													Emitter_objects.Add(hit1.collider.gameObject.transform);
													Registered_initial_positions.Add(hit1.collider.gameObject.transform.position);
													Registered_initial_scale.Add (hit1.collider.gameObject.transform.localScale);

													Registered_initial_rotation.Add(hit1.collider.gameObject.transform.rotation);

													LocalOverrides.Add(0);
													PaintTypes.Add(PaintType.Propagated);

													//GAMEOBJECTS
													#region GAMEOBJECTS
													if(gameobject_mode & (Application.isPlaying) ){
														if(Gameobj_instances.Count < (particle_count)){
															
															GameObject TEMP = Instantiate(Gameobj,Registered_paint_positions[Registered_paint_positions.Count-1],Quaternion.identity)as GameObject;
															
															if(TEMP.GetComponent<Collider>()!=null){
																if(Remove_colliders ){
																	TEMP.GetComponent<Collider>().enabled=false;
																}else{TEMP.GetComponent<Collider>().enabled=true;}
															}
															
															Gameobj_instances.Add(TEMP.transform);
															TEMP.transform.position = Registered_paint_positions[Registered_paint_positions.Count-1];
															
															if(Angled){															
																TEMP.transform.localEulerAngles = Registered_paint_rotations[Registered_paint_positions.Count-1];
															}															
															
															TEMP.transform.parent = Parent_OBJ.transform;
															
															//v1.2.2
															Updated_gameobject_positions.Add (TEMP.transform.position);
															//v1.6
															Registered_paint_size.Add(Gameobj_instances[Gameobj_instances.Count-1].localScale.x);
														}
														else{//v1.6
															Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
														}
													}
													#endregion
													//END GAMEOBJECTS
										}
								}
							}

						}
					}
				}
			}//END EMITTER OBJECT CHECK 

			if(Flamer_objects!=null){
			for(int j=0;j<Flamer_objects.Length;j++){

								if( Vector3.Distance(Flamer_objects[j].transform.position,Flammable_objects[i].transform.position) <max_propagation_dist &
								   Vector3.Distance(Flamer_objects[j].transform.position,Flammable_objects[i].transform.position) >min_propagation_dist ){ 
						
						Vector3 point = Flamer_objects[j].transform.position; 
						
						point = Flammable_objects[i].transform.InverseTransformPoint(point);

						Vector3 nearestVertex = Vector3.zero;
									Vector3 nearestNormal = Vector3.zero;

						RaycastHit hit1 = new RaycastHit();
						if(Physics.Raycast(Flamer_objects[j].transform.position, (Flammable_objects[i].transform.position - Flamer_objects[j].transform.position ), out hit1,Mathf.Infinity))
						{
							if(hit1.collider.gameObject.tag == "Flammable"){
								
								nearestVertex = hit1.point;
											nearestNormal=hit1.normal;
							}else{nearestVertex = Vector3.zero;
											nearestNormal=Vector3.zero;}
						}

						Vector3 result = Flammable_objects[i].transform.TransformPoint(nearestVertex);
						result =nearestVertex;
						
							if(result != Vector3.zero){
										if(Emitter_objects.Count > (p11.main.maxParticles/2) ){//SMv3.1
									//do nothing
								}else{
									
									int is_close_to_other_point_on_object=0;

											for(int k=0;k<Registered_paint_positions.Count;k++){

												if(Emitter_objects[k].gameObject == Flammable_objects[i]){
													
													if(Vector3.Distance( Registered_paint_positions[k],result )<density_dist_factor){
														is_close_to_other_point_on_object=1;
													}
												}
											}

										if(is_close_to_other_point_on_object==0){

											Registered_paint_positions.Add(result);
												Updated_Registered_paint_positions.Add(result);
												Particle_color.Add(new Vector4(0,0,0,999));
												Registered_paint_rotations.Add(nearestNormal);
												Registered_paint_times.Add (Time.fixedTime);

												//v1.6
												if(!gameobject_mode){
													Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
												}

											Emitter_objects.Add(hit1.collider.gameObject.transform);
											Registered_initial_positions.Add(hit1.collider.gameObject.transform.position);
											Registered_initial_scale.Add (hit1.collider.gameObject.transform.localScale);

											Registered_initial_rotation.Add(hit1.collider.gameObject.transform.rotation);
											
												LocalOverrides.Add(0);
												PaintTypes.Add(PaintType.Propagated);

												//GAMEOBJECTS
												#region GAMEOBJECTS
												if(gameobject_mode & (Application.isPlaying) ){
													if(Gameobj_instances.Count < (particle_count)){
														
														GameObject TEMP = Instantiate(Gameobj,Registered_paint_positions[Registered_paint_positions.Count-1],Quaternion.identity)as GameObject;
														
														if(TEMP.GetComponent<Collider>()!=null){
															if(Remove_colliders ){
																TEMP.GetComponent<Collider>().enabled=false;
															}else{TEMP.GetComponent<Collider>().enabled=true;}
														}
														
														Gameobj_instances.Add(TEMP.transform);
														TEMP.transform.position = Registered_paint_positions[Registered_paint_positions.Count-1];
														
														if(Angled){															
															TEMP.transform.localEulerAngles = Registered_paint_rotations[Registered_paint_positions.Count-1];
														}														
														
														TEMP.transform.parent = Parent_OBJ.transform;
														
														//v1.2.2
														Updated_gameobject_positions.Add (TEMP.transform.position);
														//v1.6
														Registered_paint_size.Add(Gameobj_instances[Gameobj_instances.Count-1].localScale.x);
													}
													else{//v1.6
														Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
													}
												}
												#endregion
												//END GAMEOBJECTS
										}
									}
							}
						}
				
					}
		 		}//END FLAMER OBJECT CHECK


				// PROJECTORS
						if(use_projection){
				for(int j=0;j<Projectors.Count;j++){

							#region PROJECTION
							// PROJECTION

							//ORIGIN v1.4
							Vector3 Position_at_origin = Projectors[j].transform.position;
							Quaternion Rotation_at_origin =  Projectors[j].transform.rotation;
							
								int max_positions = (int)p11.main.maxParticles/2;//SMv3.1

								max_positions = p11.main.maxParticles;
								int particle_count = p11.main.maxParticles;
							
							ray_dest_positions.Clear();
							
							int GET_X = (int)Mathf.Sqrt(max_positions);
							
							if(!got_random_offsets){
								
								got_random_offsets=true;
								
								rand_offsets=new Vector2[GET_X*GET_X];
								
								int count_me=0;
								for (int m=0;m<GET_X;m++){
									for (int n=0;n<GET_X;n++){
										
										rand_offsets[count_me] = new Vector2(Random.Range(0,extend*(GET_X/2)*m)+(1.5f*count_me), Random.Range(0,extend*(GET_X/2)*n)+(1.1f*count_me)  );
										count_me=count_me+1;
									}
								}
								
							}
							int count_2=0;
							for (int m=0;m<GET_X;m++){
								for (int n=0;n<GET_X;n++){
									
									float X_AXIS = (extend*(GET_X/2)*m);
									float Z_AXIS = (extend*(GET_X/2)*n);
									if(randomize){
										X_AXIS = X_AXIS+Random.Range(0,extend*(GET_X/2)*m);
										Z_AXIS = Z_AXIS+Random.Range(0,extend*(GET_X/2)*n);
									}
									
									if(!go_random){
										ray_dest_positions.Add(Rotation_at_origin*new Vector3(Position_at_origin.x - X_AXIS, Position_at_origin.y-1000,Position_at_origin.z - Z_AXIS  ));
										ray_dest_positions.Add(Rotation_at_origin*new Vector3(Position_at_origin.x + X_AXIS, Position_at_origin.y-1000,Position_at_origin.z - Z_AXIS ));
									}
									
									if(go_random){
										
										ray_dest_positions.Add(Rotation_at_origin*new Vector3(Position_at_origin.x - rand_offsets[count_2].x, Position_at_origin.y-1000,Position_at_origin.z - rand_offsets[count_2].y  ));
										count_2=count_2+1;
									}
									
								}
							}				

							if(Registered_paint_positions!=null){
								if(Registered_paint_positions.Count > (particle_count/2) ){
									//do nothing
								}else{
									
									for (int k=0;k<ray_dest_positions.Count;k++)
									{

										Vector3 ORIGIN = Position_at_origin;//v1.3
										Vector3 DEST = ray_dest_positions[k];
										
										RaycastHit hit = new RaycastHit();
										if (Physics.Raycast(ORIGIN,DEST, out hit, Mathf.Infinity))
										{											
											if(Registered_paint_positions!=null){
												
												if(Registered_paint_positions.Count > (particle_count/2) ){
													//do nothing
												}else{													

													Registered_paint_positions.Add(hit.point);
														Updated_Registered_paint_positions.Add(hit.point);
														Particle_color.Add(new Vector4(0,0,0,999));
													Registered_paint_rotations.Add(hit.normal);
													Registered_paint_times.Add (Time.fixedTime);

														//v1.6
														if(!gameobject_mode){
															Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
														}
													
													Emitter_objects.Add(hit.collider.gameObject.transform);
													Registered_initial_positions.Add(hit.collider.gameObject.transform.position);
													Registered_initial_scale.Add (hit.collider.gameObject.transform.localScale);

													Registered_initial_rotation.Add(hit.collider.gameObject.transform.rotation);													
													
														LocalOverrides.Add(0);
														PaintTypes.Add(PaintType.Projected);													
												}
											}
										}
									}									
								}
							}
							#endregion

						}}//END if use projection

			} //end check for flammables
				
		} //end check if play mode

	}

		
		if(!Use_stencil & Application.isPlaying & Real_time_painting){
		if(Input.GetMouseButtonDown(1))
		{

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			
			if(hit.collider.gameObject.tag == "PPaint"){

				if(Emitter_objects!=null){
					
					if(!Erase_mode){
									if(Emitter_objects.Count > (p11.main.maxParticles/2)){//SMv3.1
							//do nothing
						}else{
							Emitter_objects.Add(hit.collider.gameObject.transform);
							Registered_paint_positions.Add(hit.point);
									Updated_Registered_paint_positions.Add(hit.point);
										Particle_color.Add(new Vector4(p11.main.startColor.color.r,p11.main.startColor.color.g,p11.main.startColor.color.b,p11.main.startColor.color.a));
									Registered_paint_rotations.Add(hit.normal);
									Registered_paint_times.Add (Time.fixedTime);

										//v1.6
										if(!gameobject_mode){
											Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
										}

							Registered_initial_positions.Add(hit.collider.gameObject.transform.position);
							Registered_initial_scale.Add (hit.collider.gameObject.transform.localScale);

							Registered_initial_rotation.Add(hit.collider.gameObject.transform.rotation);

										LocalOverrides.Add(0);
										PaintTypes.Add(PaintType.Painted);

										//GAMEOBJECTS
										#region GAMEOBJECTS
										if(gameobject_mode & (Application.isPlaying) ){
											if(Gameobj_instances.Count < (particle_count)){
												
												GameObject TEMP = Instantiate(Gameobj,Registered_paint_positions[Registered_paint_positions.Count-1],Quaternion.identity)as GameObject;
												
												if(TEMP.GetComponent<Collider>()!=null){
													if(Remove_colliders ){
														TEMP.GetComponent<Collider>().enabled=false;
													}else{TEMP.GetComponent<Collider>().enabled=true;}
												}
												
												Gameobj_instances.Add(TEMP.transform);
												TEMP.transform.position = Registered_paint_positions[Registered_paint_positions.Count-1];
												
												if(Angled){															
													TEMP.transform.localEulerAngles = Registered_paint_rotations[Registered_paint_positions.Count-1];
												}
												
													
												TEMP.transform.parent = Parent_OBJ.transform;
												
												//v1.2.2
												Updated_gameobject_positions.Add (TEMP.transform.position);
												//v1.6
												Registered_paint_size.Add(Gameobj_instances[Gameobj_instances.Count-1].localScale.x);
											}
											else{//v1.6
												Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
											}
										}
										#endregion
										//END GAMEOBJECTS
						}
					}else if(Erase_mode){						
						
						for (int i=0;i< Registered_paint_positions.Count;i++){
							
							if( Vector3.Distance(hit.point,Registered_paint_positions[i]) < (0.5f* brush_size))
							{
											//GAMEOBJECTS
											#region GAMEOBJECTS
											if(gameobject_mode & (Application.isPlaying) ){
												Destroy(Gameobj_instances[i].gameObject);
												Gameobj_instances.RemoveAt(i);
												Updated_gameobject_positions.RemoveAt(i);
											}
											#endregion
											//END GAMEOBJECTS

											LocalOverrides.RemoveAt(i);
											PaintTypes.RemoveAt(i);

								Emitter_objects.RemoveAt(i);

										Updated_Registered_paint_positions.RemoveAt(i);
											Particle_color.RemoveAt(i);

										Registered_paint_rotations.RemoveAt(i);
										Registered_paint_times.RemoveAt(i);
										Registered_paint_size.RemoveAt(i);

								Registered_paint_positions.RemoveAt(i);
								Registered_initial_positions.RemoveAt(i);
								Registered_initial_rotation.RemoveAt(i);
								Registered_initial_scale.RemoveAt(i);
								break;
							}							
						}						
					}					
				}
			}
		}
		}
		}

			if(Use_stencil & Application.isPlaying  & Real_time_painting){				

				if(Stencil_divisions.x <1){Stencil_divisions.x=1;}
				if(Stencil_divisions.y <1){Stencil_divisions.y=1;}

				int counter=0;
				
				for( int j=0; j<Stencil.width ;j=j+(int)Stencil_divisions.x){
					for( int k=0; k<Stencil.height ;k=k+(int)Stencil_divisions.y){						
						
						Color tex_col = Stencil.GetPixel(j,k);
						
						if(1==0 | tex_col.a > 0.9f){							
							
							Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition
							                                       - new Vector3(tex_scale.x*Stencil.width/2,tex_scale.y*Stencil.height/2,0) 
							                                       + new Vector3(tex_scale.x*j,tex_scale.y*k,0) 
							                                       );
							
							RaycastHit hit = new RaycastHit();
							if (Physics.Raycast(ray, out hit, Mathf.Infinity))								
							{
								if(hit.collider.gameObject.tag == "PPaint"){									
									
									if(Emitter_objects!=null){
										
										if(!Erase_mode){
											
											if(Emitter_objects.Count > (p11.main.maxParticles/2)){//SMv3.1
												//do nothing
											}else{												
												
												if( Input.GetMouseButtonDown(0)) 
												{													
													Emitter_objects.Add(hit.collider.gameObject.transform);
													Registered_paint_positions.Add(hit.point);
													Updated_Registered_paint_positions.Add(hit.point);
													Particle_color.Add(new Vector4(tex_col.r,tex_col.g,tex_col.b,tex_col.a));
													Registered_paint_rotations.Add(hit.normal);
													Registered_paint_times.Add (Time.fixedTime);

													//v1.6
													if(!gameobject_mode){
														Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
													}
													
													Registered_initial_positions.Add(hit.collider.gameObject.transform.position);
													Registered_initial_scale.Add (hit.collider.gameObject.transform.localScale);
													Registered_initial_rotation.Add(hit.collider.gameObject.transform.rotation);													
													
													LocalOverrides.Add(0);
													PaintTypes.Add(PaintType.BrushPainted);

													//GAMEOBJECTS
													#region GAMEOBJECTS
													if(gameobject_mode & (Application.isPlaying) ){
													if(Gameobj_instances.Count < (particle_count)){
																												
														GameObject TEMP = Instantiate(Gameobj,Registered_paint_positions[Registered_paint_positions.Count-1],Quaternion.identity)as GameObject;
														
														if(TEMP.GetComponent<Collider>()!=null){
															if(Remove_colliders ){
																TEMP.GetComponent<Collider>().enabled=false;
															}else{TEMP.GetComponent<Collider>().enabled=true;}
														}
														
														Gameobj_instances.Add(TEMP.transform);
														TEMP.transform.position = Registered_paint_positions[Registered_paint_positions.Count-1];
														
														if(Angled){															
															TEMP.transform.localEulerAngles = Registered_paint_rotations[Registered_paint_positions.Count-1];
														}
														
														if(Scale_by_texture){
															TEMP.transform.localScale = new Vector3(600*tex_col.b/255,600*tex_col.b/255,600*tex_col.b/255);
														}
														if(Color_by_texture){
															
															Renderer[] renderer = TEMP.GetComponentsInChildren< Renderer >();
															
															if(!Application.isPlaying){
																renderer[0].sharedMaterial.color = Color.Lerp(renderer[0].sharedMaterial.color, tex_col,0.5f);
																renderer[1].sharedMaterial.color = Color.Lerp(renderer[0].sharedMaterial.color, tex_col,0.5f);
															}else{
																renderer[0].material.color = Color.Lerp(renderer[0].material.color, tex_col,Coloration_ammount);
																renderer[1].material.color = Color.Lerp(renderer[0].material.color, tex_col,Coloration_ammount);
															}
														}														
														TEMP.transform.parent = Parent_OBJ.transform;

															//v1.2.2
															Updated_gameobject_positions.Add (TEMP.transform.position);
															//v1.6
															Registered_paint_size.Add(Gameobj_instances[Gameobj_instances.Count-1].localScale.x);
													}
													else{//v1.6
															Registered_paint_size.Add (p11.main.startSizeMultiplier+Random_size_factor);
													}
												  }
												#endregion
												//END GAMEOBJECTS
													
												}
											}
										}
										else if(Erase_mode){											
											
											for (int i=0;i< Updated_Registered_paint_positions.Count;i++){
												
												if( Vector3.Distance(hit.point,Updated_Registered_paint_positions[i]) < (0.5f* brush_size))
												{
													//GAMEOBJECTS
													#region GAMEOBJECTS
													if(gameobject_mode & (Application.isPlaying) ){
														Destroy(Gameobj_instances[i].gameObject);
														Gameobj_instances.RemoveAt(i);
														Updated_gameobject_positions.RemoveAt(i);
													}
													#endregion
													//END GAMEOBJECTS

													LocalOverrides.RemoveAt(i);
													PaintTypes.RemoveAt(i);

													Emitter_objects.RemoveAt(i);
													
													Updated_Registered_paint_positions.RemoveAt(i);
													Particle_color.RemoveAt(i);

													Registered_paint_rotations.RemoveAt(i);
													Registered_paint_times.RemoveAt(i);
													Registered_paint_size.RemoveAt(i);
													
													Registered_paint_positions.RemoveAt(i);
													Registered_initial_positions.RemoveAt(i);
													Registered_initial_rotation.RemoveAt(i);
													Registered_initial_scale.RemoveAt(i);

													break;
												}												
											}											
										}										
									}
								}
							}
							
							maxemitter_count = ((int)p11.main.maxParticles/2)+1;//SMv3.1
							if(Emitter_objects!=null){
								current_emitters_count = Emitter_objects.Count;
							}							
						}
						counter=counter+1;
					}					
				}				
			}

			//GAMEOBJECTS
			#region GAMEOBJECTS
			if(gameobject_mode & 1==1){

				//GAMEOBJECT MODE
				if(Preview_mode | Application.isPlaying){
					
					if(Registered_paint_positions!=null){
												
						for(int i=0;i<Registered_paint_positions.Count;i++){
																
								if(Gameobj_instances.Count < (Registered_paint_positions.Count)){

									GameObject TEMP = Instantiate(Gameobj,Registered_paint_positions[i],Quaternion.identity)as GameObject;
																		
									if(TEMP.GetComponent<Collider>()!=null){
										if(Remove_colliders ){
											TEMP.GetComponent<Collider>().enabled=false;
										}else{TEMP.GetComponent<Collider>().enabled=true;}
									}
									
									Gameobj_instances.Add(TEMP.transform);
									TEMP.transform.position = Registered_paint_positions[i];
									
									if(Angled){										
										TEMP.transform.localEulerAngles = Registered_paint_rotations[i];
									}
									
									TEMP.transform.parent = Parent_OBJ.transform;

									//v1.2.2
									Updated_gameobject_positions.Add (TEMP.transform.position);
								}
						}
					}
				}
			}
			#endregion
			//END GAMEOBJECTS

			if(1==1){ //IF FIRE

				ParticleList = new ParticleSystem.Particle[p11.particleCount];
				p11.GetParticles(ParticleList);
				
				int counter_regsitered = 0;

				for (int i=0; i < ParticleList.Length;i++)
				{
					if(Registered_paint_positions!=null & Registered_paint_positions.Count > 0){

						if(Emitter_objects[counter_regsitered] != null){

						if(Registered_paint_positions!=null & Registered_paint_positions.Count > 0 & draw_in_sequence){
							
							if(Emitter_objects[counter_regsitered].gameObject.activeInHierarchy )
							{
								
								if(((counter_regsitered+1)*(ParticleList.Length/Registered_paint_positions.Count)) > i){									
									
									Vector3 FIND_moved_toZERO = Registered_paint_positions[counter_regsitered] 
									- Registered_initial_positions[counter_regsitered];
									
									Vector3 FIXED_ROT = Emitter_objects[counter_regsitered].eulerAngles;
									
									Quaternion relative = Quaternion.Euler(FIXED_ROT)*Quaternion.Inverse(Registered_initial_rotation[counter_regsitered]) ;
									Vector3 FIND_rotated = relative*(FIND_moved_toZERO);  //+ Emitter_objects[counter_regsitered].gameObject.transform.eulerAngles ;
									
									Vector3 FIND_scaled = new Vector3(FIND_rotated.x*(Emitter_objects[counter_regsitered].localScale.x / Registered_initial_scale[counter_regsitered].x),
									                                  FIND_rotated.y*(Emitter_objects[counter_regsitered].localScale.y / Registered_initial_scale[counter_regsitered].y),
									                                  FIND_rotated.z*(Emitter_objects[counter_regsitered].localScale.z / Registered_initial_scale[counter_regsitered].z)  );
									
									Vector3 FIND_re_translated = FIND_scaled+Emitter_objects[counter_regsitered].position;
									Vector3 FIND_moved_pos = FIND_re_translated;
									
									Vector3 FIND_moved_normal_toZERO = Registered_paint_rotations[counter_regsitered];
									
									Vector3 FIND_rotated_normal = relative*(FIND_moved_normal_toZERO);

									float FIND_Y = ParticleList[i].position.y;
									
									Vector3 FIND_moved_pos1 = Updated_Registered_paint_positions[counter_regsitered];															
									FIND_Y = FIND_moved_pos1.y;									
									
									if(!relaxed){
										
										if(Emitter_objects[counter_regsitered].gameObject.activeInHierarchy){
											ParticleList[i].position  = new Vector3(FIND_moved_pos.x,FIND_Y,FIND_moved_pos.z) ; 
										}
										
										//v1.4										
										if(follow_normals){
											
											ParticleList[i].rotation = 90;
											
											float FIX_for_Z = 90;											
										
											Vector3 FIXED_NORMAL = FIND_rotated_normal;
											
											if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x >=0){
												FIX_for_Z = FIX_for_Z - Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); //- Mathf.Abs((vertices[i].z-normals[i].z));
											}
											if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x >=0){
												FIX_for_Z = FIX_for_Z + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); //- Mathf.Abs((vertices[i].z-normals[i].z));
											}
											
											if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x <0){
												FIX_for_Z = FIX_for_Z + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; //- Mathf.Abs((vertices[i].z-normals[i].z));
											}
											if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x <0){
												FIX_for_Z = FIX_for_Z - Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; //- Mathf.Abs((vertices[i].z-normals[i].z));
											}
											
											float DOT = Vector3.Dot(new Vector3(0.0000001f,0.0000001f,1),FIXED_NORMAL);
											float ACOS = 0f; 
											if(DOT >1 | DOT <-1){
											}else{
												ACOS= Mathf.Acos(DOT);
											}
											
											ParticleList[i].rotation = ACOS+FIX_for_Z;
											ParticleList[i].axisOfRotation =  Vector3.Cross( new Vector3(0.00000001f,0.00000001f,1), FIXED_NORMAL);
											
										}									
										
									}
									
									if(relaxed){
										if(Emitter_objects[counter_regsitered].gameObject.activeInHierarchy){
											if(ParticleList[i].remainingLifetime > keep_in_position_factor*ParticleList[i].startLifetime){
												ParticleList[i].position  = new Vector3(FIND_moved_pos.x,FIND_Y,FIND_moved_pos.z) ; 
											}
										}
									}
									
									Updated_Registered_paint_positions[counter_regsitered] = FIND_moved_pos;
									
								}else{  
									if(counter_regsitered < Registered_paint_positions.Count-1 ){
										counter_regsitered= counter_regsitered+1;
									}else{counter_regsitered=0;}
								}
							}							
						}
						
						if(Registered_paint_positions!=null & Registered_paint_positions.Count > 0 & !draw_in_sequence){ 						
		
							
							Vector3 FIND_moved_toZERO = Registered_paint_positions[counter_regsitered] 
							- Registered_initial_positions[counter_regsitered];
							
							Vector3 FIXED_ROT = Emitter_objects[counter_regsitered].eulerAngles;
							
							Quaternion relative = Quaternion.Euler(FIXED_ROT)*Quaternion.Inverse(Registered_initial_rotation[counter_regsitered]) ;
							Vector3 FIND_rotated = relative*(FIND_moved_toZERO);  //+ Emitter_objects[counter_regsitered].gameObject.transform.eulerAngles ;
							
							Vector3 FIND_scaled = new Vector3(FIND_rotated.x*(Emitter_objects[counter_regsitered].localScale.x / Registered_initial_scale[counter_regsitered].x),
							                                  FIND_rotated.y*(Emitter_objects[counter_regsitered].localScale.y / Registered_initial_scale[counter_regsitered].y),
							                                  FIND_rotated.z*(Emitter_objects[counter_regsitered].localScale.z / Registered_initial_scale[counter_regsitered].z)  );
							
							Vector3 FIND_re_translated = FIND_scaled+Emitter_objects[counter_regsitered].position;
							Vector3 FIND_moved_pos = FIND_re_translated;
							
							Vector3 FIND_moved_normal_toZERO = Registered_paint_rotations[counter_regsitered];
							
							Vector3 FIND_rotated_normal = relative*(FIND_moved_normal_toZERO);

							//GAMEOBJECTS
							#region GAMEOBJECTS
							if(gameobject_mode & (Application.isPlaying | Preview_mode) ){
								if(counter_regsitered < Gameobj_instances.Count & !follow_particles){									

									Gameobj_instances[counter_regsitered].position = FIND_moved_pos; 

									if(!look_at_direction){
										Gameobj_instances[counter_regsitered].rotation = relative*Quaternion.FromToRotation(Vector3.up,FIND_moved_normal_toZERO); 
									}else{

										//v1.4
										//if(!look_at_normal){
											Vector3 Motion_vec = FIND_moved_pos - Updated_Registered_paint_positions[counter_regsitered];

											Quaternion New_rot = Quaternion.identity;
											if(Motion_vec != Vector3.zero){
												New_rot = Quaternion.LookRotation(1*Motion_vec);
												Gameobj_instances[counter_regsitered].rotation = Quaternion.Slerp(Gameobj_instances[counter_regsitered].rotation,New_rot,Time.deltaTime*16);
											}
										//}else{
												//v1.6
//												Vector3 Motion_vec = FIND_rotated_normal;
//												
//												Quaternion New_rot = Quaternion.identity;
//												if(Motion_vec != Vector3.zero){
//													New_rot = Quaternion.LookRotation(1*Motion_vec);
//													//Gameobj_instances[counter_regsitered].rotation = Quaternion.Slerp(Gameobj_instances[counter_regsitered].rotation,New_rot,Time.deltaTime*16);
//													Gameobj_instances[counter_regsitered].rotation = Gameobj_instances[counter_regsitered].rotation;
//												}

										//}



									}

									if(Angled){

											float add_speed=0;
											float add_rot=0;

											if(enable_overides){
												if(LocalOverrides[counter_regsitered] == 4){
													add_speed = overide_WindSpeed;
													add_rot = overide_GrassAngle;
												}
											}

										if(Time.fixedTime-Grab_time_calcs > Delay_calcs | !Optimize_calcs){
											
											Grab_time_calcs=Time.fixedTime;

											Quaternion rot1 = Quaternion.FromToRotation(Gameobj_instances[counter_regsitered].up,Gameobj_instances[counter_regsitered].right);
											if(Asign_rot){
												if(Wind_speed>0 & Application.isPlaying){
													float timex = Time.time * (Wind_speed+add_speed) + 0.1365143f * 10*i;
													Local_rot.y  =  noise.Noise(timex+10, timex+20, timex)+add_rot;
												}
												Gameobj_instances[counter_regsitered].localRotation *= rot1*new Quaternion(Local_rot.x,Local_rot.y,Local_rot.z,1);
											}

										}

									}
									if(Grow_trees){ 
										if(Emitter_objects[counter_regsitered].localScale != Registered_initial_scale[counter_regsitered]){
											if ((Time.fixedTime-Current_Grow_time) < Grow_time ){
												Gameobj_instances[counter_regsitered].localScale = new Vector3(Gameobj_instances[counter_regsitered].localScale.x * (Emitter_objects[counter_regsitered].localScale.x/Registered_initial_scale[counter_regsitered].x),
												                                                                         Gameobj_instances[counter_regsitered].localScale.y * (Emitter_objects[counter_regsitered].localScale.y/Registered_initial_scale[counter_regsitered].y),
												                                                                         Gameobj_instances[counter_regsitered].localScale.z * (Emitter_objects[counter_regsitered].localScale.z/Registered_initial_scale[counter_regsitered].z));										}
										}									
									}//END GROW TREES
								}

							}
							#endregion
							//END GAMEOBJECTS							
							
							float FIND_Y = ParticleList[i].position.y;
							
							Vector3 FIND_moved_pos1 = Updated_Registered_paint_positions[counter_regsitered];//METHOD B																
							FIND_Y = FIND_moved_pos1.y;						
							
							if(!relaxed){
								if(Emitter_objects[counter_regsitered].gameObject.activeInHierarchy){
									ParticleList[i].position  = new Vector3(FIND_moved_pos.x,FIND_Y,FIND_moved_pos.z) ; 
								}
								
								//v1.4								
								if(follow_normals){
									
									ParticleList[i].rotation = 90;
									
									float FIX_for_Z = 90;			
									
									Vector3 FIXED_NORMAL = FIND_rotated_normal;	
																		
									if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x >=0){
										FIX_for_Z = Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); 
										
									}
									if(FIXED_NORMAL.z ==0 & FIXED_NORMAL.x ==0){
										FIX_for_Z = 90; //- Mathf.Abs((vertices[i].z-normals[i].z));
									}
									if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x >=0){
										FIX_for_Z = FIX_for_Z + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) ); 
									}
									
									if(FIXED_NORMAL.z >=0 & FIXED_NORMAL.x <0){
										FIX_for_Z = -90 + Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; 
									}
									if(FIXED_NORMAL.z <0 & FIXED_NORMAL.x <0){
										FIX_for_Z = FIX_for_Z - Vector3.Angle(new Vector3(1,0,0),new Vector3(FIXED_NORMAL.x,0,FIXED_NORMAL.z) )+180; 
									}
									
									float DOT = Vector3.Dot(new Vector3(0.0000001f,0.0000001f,1),FIXED_NORMAL);

									float ACOS = 0f; 
									if(DOT >1 | DOT <-1){										
										
										
									}else{
										ACOS= Mathf.Acos(DOT);
									}
									FIX_for_Z=90-((360*DOT)/6.28f);
				
									ParticleList[i].rotation = ACOS+FIX_for_Z;
									ParticleList[i].rotation = FIX_for_Z;
									
									if(debug_rot!=0){
										
										ParticleList[i].rotation =debug_rot;
									}									
									
									ParticleList[i].axisOfRotation =  Vector3.Cross( new Vector3(0.0000001f,0.0000001f,1), FIXED_NORMAL);

								}								
							}
							
							if(relaxed){

								if(!let_loose & !(enable_overides & (LocalOverrides[counter_regsitered]==2 | LocalOverrides[counter_regsitered]==3) )  ){
									if(Emitter_objects[counter_regsitered].gameObject.activeInHierarchy){
										if(ParticleList[i].remainingLifetime > keep_in_position_factor*ParticleList[i].startLifetime){
											ParticleList[i].position  = new Vector3(FIND_moved_pos.x,FIND_Y,FIND_moved_pos.z) ; 
										}
									}
								}
									//v1.4 - Assign Initial velocity
									if(ParticleList[i].remainingLifetime > ParticleList[i].startLifetime*keep_in_position_factor){
										if(Velocity_toward_normal){
											ParticleList[i].velocity = 1*new Vector3(FIND_rotated_normal.x*Normal_Velocity.x,FIND_rotated_normal.y*Normal_Velocity.y,FIND_rotated_normal.z*Normal_Velocity.z); 

										}
									}

								//Gravity
								if(Gravity_Mode & let_loose){
									
									float GRAV_Fac = grav_factor;

									if(enable_overides & (LocalOverrides[counter_regsitered] == 2 | LocalOverrides[counter_regsitered] == 3) ){
											GRAV_Fac=Release_Gravity;
									}

									if(Registered_paint_positions.Count > counter_regsitered){

											if(!Use_Lerp){
												ParticleList[i].position = Vector3.Slerp(ParticleList[i].position, Registered_paint_positions[counter_regsitered]+ new Vector3(i*X_offset_factor,Y_offset,i*Z_offset_factor),GRAV_Fac);
											}else{
												ParticleList[i].position = Vector3.Lerp(ParticleList[i].position, Registered_paint_positions[counter_regsitered]+ new Vector3(i*X_offset_factor,Y_offset,i*Z_offset_factor),GRAV_Fac);
											}
									}
									
										if(!Use_Lerp){
											ParticleList[i].velocity= Vector3.Slerp(ParticleList[i].velocity,Vector3.zero,0.05f);
										}else{
											ParticleList[i].velocity= Vector3.Lerp(ParticleList[i].velocity,Vector3.zero,0.05f);
										}

								}

								//GAMEOBJECTS
								#region GAMEOBJECTS
								if(gameobject_mode & (Application.isPlaying | Preview_mode) ){

									if(follow_particles){
										if(Gameobj_instances.Count > i | 1==1){ 

											Gameobj_instances[counter_regsitered].position = ParticleList[i].position;
											
											//v1.2.2
											Vector3 Motion_vec = Gameobj_instances[counter_regsitered].position -Updated_gameobject_positions[counter_regsitered];
											
											Quaternion New_rot = Quaternion.identity;

											if(Motion_vec.magnitude > 0.1f & Motion_vec.magnitude < 99999f){

												New_rot = Quaternion.LookRotation(1*Motion_vec);
												
													if(!Use_Lerp){
														Gameobj_instances[counter_regsitered].rotation = Quaternion.Slerp(Gameobj_instances[counter_regsitered].rotation,New_rot,Time.deltaTime*16);
													}else{
														Gameobj_instances[counter_regsitered].rotation = Quaternion.Lerp(Gameobj_instances[counter_regsitered].rotation,New_rot,Time.deltaTime*16);
													}
												
											}
											
											//remove colliders

												if(Gameobj_instances[counter_regsitered].gameObject.GetComponent<Collider>() !=null){
													if(Remove_colliders){
															if(Gameobj_instances[counter_regsitered].gameObject.GetComponent<Collider>().enabled){
																Gameobj_instances[counter_regsitered].gameObject.GetComponent<Collider>().enabled = false;
															}														
													}
													else if(!Remove_colliders){
															if(!Gameobj_instances[counter_regsitered].gameObject.GetComponent<Collider>().enabled){
																Gameobj_instances[counter_regsitered].gameObject.GetComponent<Collider>().enabled = true;
															}
													}
												}									
											
										}
									}
									
								}
								#endregion
								//END GAMEOBJECTS

							}																				

							if(keep_alive & !(enable_overides & (LocalOverrides[counter_regsitered] == 1 | LocalOverrides[counter_regsitered] == 3) ) & let_loose){

								ParticleList[i].startLifetime=16;
								ParticleList[i].remainingLifetime = 15;
							}

								if(Color_effects){
									if(Particle_color[counter_regsitered].w < 257){
										if(Keep_color){
											ParticleList[i].startColor=new Color(Particle_color[counter_regsitered].x,Particle_color[counter_regsitered].y,Particle_color[counter_regsitered].z,Particle_color[counter_regsitered].w);//SMv3.1
										}else{
											if(ParticleList[i].remainingLifetime > keep_in_position_factor*ParticleList[i].startLifetime){
												if(Lerp_color){
													//SMv3.1
													ParticleList[i].startColor=Color.Lerp(ParticleList[i].startColor,new Color(Particle_color[counter_regsitered].x,Particle_color[counter_regsitered].y,Particle_color[counter_regsitered].z,Particle_color[counter_regsitered].w),0.5f);

												}
												else{
													ParticleList[i].startColor=new Color(Particle_color[counter_regsitered].x,Particle_color[counter_regsitered].y,Particle_color[counter_regsitered].z,Particle_color[counter_regsitered].w);//SMv3.1
												}
											}
										}
									}
								}

							if(variant_size){
								
									//v1.6
									//Add gameobject melt too 
									if(gameobject_mode & Vary_gameobj_size){
										//if(Gameobj_instances[counter_regsitered].localScale.x>0){
										Gameobj_instances[counter_regsitered].localScale = new Vector3(1,1,1)*Registered_paint_size[counter_regsitered];
										//}else{
											//Registered_paint_size[counter_regsitered] = 0.001f;
											
										//}
									}else{
										ParticleList[i].startSize = Registered_paint_size[counter_regsitered];//SMv3.1
									}
																
								ParticleList[i].startLifetime=16;
								
								if(ParticleList[i].remainingLifetime < keep_alive_factor*ParticleList[i].startLifetime){
									ParticleList[i].remainingLifetime = 15;
								}								
							
								if(!is_ice){
									ParticleList[i].startSize = Registered_paint_size[counter_regsitered];//SMv3.1
								}else								
								if( 1==1 & (!keep_alive & (is_ice)) | (enable_overides & LocalOverrides[counter_regsitered]==1 ) ){
									
										if(1==1){

											//v1.6
											//Add gameobject melt too 
											if(gameobject_mode & Vary_gameobj_size){

												if(Gameobj_instances[counter_regsitered].localScale.x > 0.001f | 
												   Gameobj_instances[counter_regsitered].localScale.y > 0.001f | 
												   Gameobj_instances[counter_regsitered].localScale.z > 0.001f 
												   ){

													if(Gameobj_instances[counter_regsitered].localScale.x >0){
														if( 1==1 & (enable_overides & LocalOverrides[counter_regsitered]==1 ) ){
															Gameobj_instances[counter_regsitered].localScale = new Vector3(
																Registered_paint_size[counter_regsitered] - fast_melt_speed,
																Registered_paint_size[counter_regsitered] - fast_melt_speed,
																Registered_paint_size[counter_regsitered] - fast_melt_speed
																);
															}
														else{
															Gameobj_instances[counter_regsitered].localScale = new Vector3(
																Registered_paint_size[counter_regsitered] - melt_speed,
																Registered_paint_size[counter_regsitered] - melt_speed,
																Registered_paint_size[counter_regsitered] - melt_speed
																);
														}	
													}else{
														Gameobj_instances[counter_regsitered].localScale = new Vector3(0.001f,0.001f,0.001f);
														
													}

												}

											}

												if( ParticleList[i].startSize > 0){//SMv3.1

												if( 1==1 & (enable_overides & LocalOverrides[counter_regsitered]==1 ) ){
														ParticleList[i].startSize = Registered_paint_size[counter_regsitered] - fast_melt_speed ;//SMv3.1
												}
												else{
														ParticleList[i].startSize = Registered_paint_size[counter_regsitered] - melt_speed ;//SMv3.1
												}												
											}

											//v1.6
											//Add gameobject melt too 
											if(gameobject_mode & Vary_gameobj_size){

												//v1.6
												if(Parent_OBJ!=null){
													if(enable_combine){
														if(Mathf.Abs(Gameobj_instances[counter_regsitered].localScale.x - Registered_paint_size[counter_regsitered])>0.0f){
															Combiner.MakeActive=true;
														}
													}
												}

												if(Gameobj_instances[counter_regsitered].localScale.x>0){
													Registered_paint_size[counter_regsitered] = Gameobj_instances[counter_regsitered].localScale.x;
												}else{
													Registered_paint_size[counter_regsitered] = 0.001f;

												}
											}else{
													Registered_paint_size[counter_regsitered] = ParticleList[i].startSize;//SMv3.1
											}
										}								
									
								}																
								
							}

								//v1.6
								if(Parent_OBJ!=null){
									if(enable_combine){
										//if(Updated_gameobject_positions[i] != Gameobj_instances[i].position){
										if((Updated_Registered_paint_positions[counter_regsitered] - FIND_moved_pos).magnitude>0.0f){
											Combiner.MakeActive=true;
										}
									}
								}

							Updated_Registered_paint_positions[counter_regsitered] = FIND_moved_pos;							
							
													counter_regsitered=counter_regsitered+1;
													if(counter_regsitered > Registered_paint_positions.Count-1 ){
														counter_regsitered=0;
													}							
							
						}//END if NOT draw in sequence
						
					}
				}}//end particle for loop
				
				p11.SetParticles(ParticleList,p11.particleCount);

			}//END IF FIRE		
		
	}
}

}