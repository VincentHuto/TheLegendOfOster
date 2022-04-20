using UnityEngine;
using System.Collections;
using System.Collections.Generic;
///using Artngame.PDM;

namespace Artngame.SKYMASTER {

public class Vortex_PDM {
	public Vector3 position;
	public Vector3 velocity;
	public int particle_ID;
	public float lifetime;
	public Vector3 angular_velocity;
	public float scale;
	public bool active;
}

[ExecuteInEditMode()]
public class AttractParticlesSKYMASTER : MonoBehaviour {


	public bool Turbulance = false;
	public float Turbulance_strength = 0.1f;
	public float Turbulance_frequency = 0.1f;
	public Vector3 Axis_affected = new Vector3(0,1,0);

	public bool splash_effect=false;
	public bool vortex_motion;
	private Vector3 Vortex_velocity;
	private Vector3 Vortex_position;
	public int Vortex_count=10;
	private List<Vortex_PDM> Vortexes;
	public float Vortex_life = 10f;
	public float Vortex_angularvelocity = 10f;
	public float Vortex_scale = 10f;

	public bool limit_influence=false;//limit influence of vortex to only close particles

	public float Vortex_center_size=15f;
	public Color Vortex_center_color=Color.blue;
	public bool Show_vortex=false;

	private int previous_vortex_count;

	public bool Color_force=false;
	public Color Force_color=Color.blue;

	public bool use_exponent=false;
	public bool gravity_pull=false;

	public bool Swirl_grav = false;
	public bool Bounce_grav = false;
	public float Bounce_factor=1f;
	public float Affect_tres =3f;
	private Vector3 Gravity_Plane_INNER = Vector3.zero;

	public bool gravity_planar=false;
	public Vector3 Gravity_plane = new Vector3(1,1,1);
	public float Gravity_factor=0.1f;
	
	public float Dist_Factor =1f; 

	public bool enable_paint = false;
	

   	 ParticleSystem[] p2;
	 //ParticleEmitter[] p3; //v3.4.6

	//public Particle[] particles;  //v3.4.6
    public float affectDistance =10;
    float sqrDist;
    Transform thisTransform;
	public float dumpen = 2;

	ParticleSystem.Particle[] ParticleList;

	public bool smoothattraction = true;
	public bool Lerp_velocity = false;
	public bool repel = false;

	public bool make_moving_star = false; 
	public float star_trail_dist=10;
	
	public bool limit_to_Y_axis = false; 


		//v1.4
		public bool vary_turbulence=false;
		public bool perlin_enable=false;
		public bool splash_noise=false;
		private PerlinSKYMASTER  noise;
		private SimplexNoiseSKYMASTER SIMPLEXnoise;
		public float noise_str=20;
		public float noise_turb_str=0.1f;
		public Vector3 axis_deviation=new Vector3(1,1,1);
		public float splash_noise_tres = 0.05f;

	
    void Start()
    {
	    thisTransform = transform;

			//v1.4
			if(Affect_specific!=null){

			

				if(Affect_specific.Count>0){

					p2 = new ParticleSystem[Affect_specific.Count];
					//p3 = new ParticleEmitter[0]; //v3.4.6

					for(int i = 0; i<Affect_specific.Count; i++){
						p2[i] = Affect_specific[i].GetComponent(typeof(ParticleSystem)) as ParticleSystem;
					}
					//Debug.Log (p2.Length);
				}else{

					p2 = GameObject.FindObjectsOfType<ParticleSystem>();
					//p3 = GameObject.FindObjectsOfType<ParticleEmitter>(); //v3.4.6
				}

			}else{
				Affect_specific = new List<GameObject>();

				p2 = GameObject.FindObjectsOfType<ParticleSystem>();
				//p3 = GameObject.FindObjectsOfType<ParticleEmitter>(); //v3.4.6
			}

	    sqrDist = affectDistance*affectDistance;

		Vortexes = new List<Vortex_PDM>();

		

		previous_vortex_count = Vortex_count;

    }
     
	

	//particle paint parameters
    public float hit_dist = 10;
	
	private int counter=0;


	//particle trails (when repeling)
	public int star_trails = 6;
	public float trail_distance = 1;
	public float speed_of_trail = 3;
	public float distance_of_trail = 0.5f;
	public float trail_length_out = 5.0f; //trail length
	public float size_of_trail_out = 0.05f;
	public float distance_between_trail = 0.5f;
	public float vertical_trail_separation = 2.1f;
	public float smooth_trail = 4.1f;
	//end particle trails

	public bool affect_by_tag=false;//v1.2
	public string[] tags;//v1.2
	public string[] Exclude_tags;//v1.4
	public List<GameObject> Affect_specific; //v1.4

	private bool Affect_new=false; 
	private float recheck_time;
	public float Time_to_update=2f;

	public bool MultiThread=false;
	private bool MultiThreaded=false;

	private bool init_vortexes=false;
	public bool Enable_Preview=false;

		//v1.6
		public bool End_LifeOf_Affected=false;
		public float End_Life=0.01f;

	void Update()
	{	

			if(1==1){

				if(dumpen == 0){
					dumpen=0.000000001f;
				}

			}


			//v1.4
			if(noise ==null){
				noise = new PerlinSKYMASTER ();
			}
			if(SIMPLEXnoise ==null){
				SIMPLEXnoise = new SimplexNoiseSKYMASTER();
			}



	  if(Application.isPlaying | Enable_Preview){

		if(!Application.isPlaying){
					MultiThreaded = MultiThread;
			//MultiThreaded=false;
		}else{ MultiThreaded = MultiThread;}

		if(Time.fixedTime-recheck_time>Time_to_update){
			Affect_new = true;
			recheck_time = Time.fixedTime;
		}
		else{Affect_new = false;}

		if(Affect_new & !(Affect_specific.Count>0)){//v1.4

				p2 = GameObject.FindObjectsOfType<ParticleSystem>();
					//p3 = GameObject.FindObjectsOfType<ParticleEmitter>(); //v3.4.6
		}else{
					//p2 = Affect_specific.ToArray();
		}

		if(!MultiThreaded){
			Vortexes = new List<Vortex_PDM>();
		}
	
		trail_distance = (2 * Mathf.PI) / star_trails;
		sqrDist = affectDistance*affectDistance;
		float dist;
		
		Vector3 SPHERE_POS = thisTransform.position;

		// SHURIKEN PARTICLES
		foreach(ParticleSystem p11 in p2){

			if(p11 != null){ //VERSION 1.2 CHANGE

				int has_tag=0;
				if(tags!=null){
					for(int i=0;i<tags.Length;i++){
						if(p11.gameObject.tag == tags[i]){
							has_tag=1;
						}
					}
				}

				bool is_in_exclude_list=false;

						if(Exclude_tags !=null){
							if(Exclude_tags.Length > 0)
							{
								for(int i=0;i<Exclude_tags.Length;i++){
									if(p11.gameObject.tag == Exclude_tags[i]){
										is_in_exclude_list=true;
									}
								}
							}
						}

				//if(!affect_by_tag | (affect_by_tag & has_tag==1) ){ //VERSION 1.2 CHANGE
				if( (!affect_by_tag & !is_in_exclude_list) | (affect_by_tag & has_tag==1) ){ //VERSION 1.4 CHANGE

		if(Vector3.Distance(thisTransform.position, p11.transform.position) < (sqrDist*Dist_Factor) ){
	
		ParticleList = new ParticleSystem.Particle[p11.particleCount];
		p11.GetParticles(ParticleList);

		#region MultiThreaded

						string p11_tag = p11.gameObject.tag;

		
		if(MultiThreaded){
							Vector3 p11_transform_pos = p11.transform.position;


							bool World_sim = false;
							if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){ //v3.4.9
								World_sim=true;
							}
							float Time_delta = Time.deltaTime;
							float Time_fixed = Time.fixedTime;

							


							//predefine vortexes, redefine if number changes
							if(Vortex_count != previous_vortex_count | !init_vortexes){

								Vortexes = new List<Vortex_PDM>();

								for(int i=0;i<Vortex_count;i++){
								
									Vortex_PDM AA = new Vortex_PDM();
									AA.active=false;
									Vortexes.Add(AA);

									
								}
								
								init_vortexes=true;
								previous_vortex_count = Vortex_count;
							}

						

							LoomSKYMASTER.RunAsync(()=>{
				
								#region RUN
								if(!make_moving_star){
									
									//v1.4
									int count_vortex=0;
									for (int i=0; i < (ParticleList.Length/2);i++)
									{
										
										//if local simulation is selected in particle
										Vector3 WORLD_POS = ParticleList[i].position + p11_transform_pos;
										
										
										if(World_sim){
											WORLD_POS = ParticleList[i].position;
										}
										
										dist = Vector3.SqrMagnitude(SPHERE_POS - WORLD_POS);
										if (dist < sqrDist) {
											
											float distance = dist;
											if (smoothattraction == false ){
												distance=0;
											}
											
											Vector3 SPHERE_POS_FIXED = SPHERE_POS;
											if(repel == true)
											{
												//find opposite to sphere-particle vector
												SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11_transform_pos;
												
												if(limit_to_Y_axis | p11_tag == "Grass"){ //dont take height into account
													
													SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11_transform_pos;
													SPHERE_POS_FIXED = new Vector3(SPHERE_POS_FIXED.x,WORLD_POS.y,SPHERE_POS_FIXED.z);
													
													//or push only downwards
													SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11_transform_pos;
													SPHERE_POS_FIXED = new Vector3(WORLD_POS.x,SPHERE_POS_FIXED.y,WORLD_POS.z);
													
												}
												
											}
											
											
											if(Turbulance){ //Version 1.2 ADDITION
												
												Vector3 POS_TURB = WORLD_POS;
												
												if(Axis_affected.y >0){
													POS_TURB.y=Turbulance_strength* Mathf.Cos (Time_fixed* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.y;
												}
												if(Axis_affected.x >0){
													POS_TURB.x=Turbulance_strength* Mathf.Cos (Time_fixed* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.x;
												}
												if(Axis_affected.z >0){
													POS_TURB.z=Turbulance_strength* Mathf.Cos (Time_fixed* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.z;
												}
												
												if(vortex_motion & p11_tag != "VortexSafe" & p11_tag != "Grass"){

													int count_active=0;
													for(int j=0;j< Vortexes.Count;j++){

														if(Vortexes[j].active){count_active=count_active+1;}
													}

													
													if(count_active<Vortex_count){


												

														Vortexes[count_active].position = ParticleList[i].position;
														Vortexes[count_active].velocity = ParticleList[i].velocity;
														Vortexes[count_active].particle_ID=i;
														Vortexes[count_active].lifetime = Vortex_life;
														Vortexes[count_active].angular_velocity = new Vector3(Vortex_angularvelocity,Vortex_angularvelocity,Vortex_angularvelocity);
														Vortexes[count_active].scale = Vortex_scale;
														
													Vortexes[count_active].active = true;

														ParticleList[i].remainingLifetime = Vortex_life;
														

													}
													
													
													for(int j=0;j< Vortexes.Count;j++){
													if(j< Vortexes.Count & Vortexes[j].active){
														if(Vortexes[j].particle_ID == i){
															
															Vortexes[j].position = ParticleList[i].position;
															Vortexes[j].velocity = ParticleList[i].velocity;
															
															if(Show_vortex){
																ParticleList[i].startSize = Vortex_center_size;
																ParticleList[i].startColor = Vortex_center_color;
															}
															

															if(Vortexes[j].lifetime >Vortex_life){
																	Vortexes[j].active=false;
															}

															
														}else { 
															

																Vector3 rVORT=ParticleList[i].position-Vortexes[j].position;

																if(limit_influence & (rVORT.magnitude > Vortexes[j].scale)  )
																{
																	//do nothing
																}
																else{
																	Vector3 vVORT=Vector3.Cross(Vortexes[j].angular_velocity, rVORT);
																	
																	float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortexes[j].scale)  );
																	
																	float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
																	float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
																	float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;
																	
																	
																		if(Lerp_velocity){
																			ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
																		}
																		else{
																			ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
																		}

																}
															
															}
														}
														
													}
												}
												
												SPHERE_POS_FIXED = new Vector3(  POS_TURB.x,POS_TURB.y,POS_TURB.z);
												
												
											}
											
											float DOUBLE_DIST=1f;
											float BOOST_DIST=1f;
											if(use_exponent){
												
												DOUBLE_DIST =0.000000001f*dist*(dist*dist*dist*dist);
												BOOST_DIST =100000000f;
												
												float SPEEDY = BOOST_DIST*Time_delta / (dumpen+0.1f*distance*DOUBLE_DIST);
												SPEEDY=(1/dist)+dumpen;
												
												SPEEDY=(affectDistance-Mathf.Sqrt(distance))/(affectDistance+(dumpen/100))*Time_delta;
												
												if(!gravity_pull){
													ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11_transform_pos;
													
													
													if(World_sim){
														ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
													}
												}
												
												
												if(gravity_pull & !gravity_planar){
													
													Vector3 rVORT=WORLD_POS-SPHERE_POS_FIXED;
													
													if(Swirl_grav){

															if(Affect_tres < rVORT.magnitude){
																ParticleList[i].velocity = ParticleList[i].velocity - rVORT.normalized*Gravity_factor/dist; 
																Gravity_Plane_INNER = ParticleList[i].velocity;
															}
															else{
																if(Bounce_grav){
																	ParticleList[i].velocity = ParticleList[i].velocity + Bounce_factor*rVORT.normalized*Gravity_factor/dist;
																}
																else{
																	Vector3 vVORT=Vector3.Cross(Gravity_Plane_INNER*Vortex_angularvelocity, rVORT);
																	
																	float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortex_scale));
																	
																	float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
																	float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
																	float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;

																	

																	if(Lerp_velocity){
																		ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
																	}
																	else{
																		ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
																	}
																}
															}
													}
													else
													{
														ParticleList[i].velocity = ParticleList[i].velocity - rVORT.normalized*Gravity_factor/dist; 
													}





													ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11_transform_pos;
													
													
													if(World_sim){
														ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
													}
													
													if (rVORT.magnitude <= 0.1f)
													{
														ParticleList[i].remainingLifetime = 0;
													}
													
												}
												
												if(gravity_pull & gravity_planar){
													
													Vector3 rVORT=WORLD_POS-SPHERE_POS_FIXED;
													
													Vector3 vVORT=Vector3.Cross(Gravity_plane*Vortex_angularvelocity, rVORT);
													
													float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortex_scale));
													
													float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
													float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
													float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;
													
													ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11_transform_pos;
													
													
													if(World_sim){
														ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
													}
													
													

														if(Lerp_velocity){
															ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
														}
														else{
															ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
														}
												}
											}
											else
											{
												DOUBLE_DIST=1f;
												BOOST_DIST=1f;
												
												ParticleList[i].position  = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,BOOST_DIST*Time_delta / (dumpen+0.1f*distance*DOUBLE_DIST)) - p11_transform_pos;
												
												
												if(World_sim){
													ParticleList[i].position  = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,Time_delta / (dumpen+0.1f*distance*DOUBLE_DIST));
												}
											}
											
											if(Color_force){	//Version 1.2 ADDITION
												
														ParticleList[i].startColor = Color.Lerp(ParticleList[i].startColor, Force_color,0.1f);
												
											}
													//v1.6
													if(End_LifeOf_Affected){
														ParticleList[i].remainingLifetime=End_Life;
													}
											
										}
										
										
										if(splash_effect){
											if(ParticleList[i].remainingLifetime < 0.05f){
												ParticleList[i].remainingLifetime = ParticleList[i].startLifetime;
												ParticleList[i].position = p11_transform_pos;
											}
										}

												//v1.4
												if(vary_turbulence){				
													float X_VEC = ParticleList[i].position.x + (2*noise.Noise(ParticleList[i].position.z*noise_str,ParticleList[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time_delta;
													float Y_VEC = ParticleList[i].position.y + (2*noise.Noise(ParticleList[i].position.z*noise_str,ParticleList[i].position.x*noise_str*0.77f+1)-0)*noise_turb_str*Time_delta;
													float Z_VEC = ParticleList[i].position.z + (2*noise.Noise(ParticleList[i].position.x*noise_str,ParticleList[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time_delta;
													
													double X_VEC1 = 0; 
													double Y_VEC1 = 0; 
													double Z_VEC1 = 0; 												
													if(perlin_enable){
														X_VEC1 = ParticleList[i].velocity.x + noise.Noise( ParticleList[i].position.y * 10*axis_deviation.y+1, ParticleList[i].position.z * 1*axis_deviation.z)*Time_delta*noise_str*noise_turb_str; 
														Y_VEC1 = ParticleList[i].velocity.y + noise.Noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.z * 1*axis_deviation.z)*Time_delta*noise_str*noise_turb_str; 
														Z_VEC1 = ParticleList[i].velocity.z + noise.Noise(ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.y * 1*axis_deviation.y)*Time_delta*noise_str*noise_turb_str; 
													}else{
														X_VEC1 = ParticleList[i].velocity.x + SIMPLEXnoise.noise( ParticleList[i].position.y * 10*axis_deviation.y+1, ParticleList[i].position.z * 1*axis_deviation.z,Time_fixed*noise_str)*Time_delta*noise_str*noise_turb_str; 
														Y_VEC1 = ParticleList[i].velocity.y + SIMPLEXnoise.noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.z * 1*axis_deviation.z,Time_fixed*noise_str)*Time_delta*noise_str*noise_turb_str; 
														Z_VEC1 = ParticleList[i].velocity.z + SIMPLEXnoise.noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.y * 1*axis_deviation.y,Time_fixed*noise_str)*Time_delta*noise_str*noise_turb_str; 
													}												
													ParticleList[i].velocity = new Vector3((float)X_VEC1,(float)Y_VEC1,(float)Z_VEC1);	
													
													ParticleList[i].position = new Vector3(X_VEC,Y_VEC,Z_VEC);
													
													if(splash_noise){
														if(ParticleList[i].remainingLifetime < splash_noise_tres){
															
															if(count_vortex < Vortexes.Count){
																ParticleList[i].position = Vortexes[count_vortex].position;
																ParticleList[i].remainingLifetime = ParticleList[i].startLifetime;
																count_vortex++;
															}else{															
																count_vortex=0;
															}
															
														}
													}
													
												}
										
										
									}
								}
								#endregion
		
				
			});

							#region RUN the rest
							if(!make_moving_star & 1==1){

								//v1.4
								int count_vortex=0;

								for (int i=(ParticleList.Length/2); i < (ParticleList.Length);i++)
								{
									
									
									Vector3 WORLD_POS = ParticleList[i].position + p11_transform_pos;
									
									
									if(World_sim){
										WORLD_POS = ParticleList[i].position;
									}
									
									dist = Vector3.SqrMagnitude(SPHERE_POS - WORLD_POS);
									if (dist < sqrDist) {
										
										float distance = dist;
										if (smoothattraction == false ){
											distance=0;
										}
										
										Vector3 SPHERE_POS_FIXED = SPHERE_POS;
										if(repel == true)
										{
											//find opposite to sphere-particle vector
											SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11_transform_pos;
											
											if(limit_to_Y_axis | p11_tag == "Grass"){ //dont take height into account
												
												SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11_transform_pos;
												SPHERE_POS_FIXED = new Vector3(SPHERE_POS_FIXED.x,WORLD_POS.y,SPHERE_POS_FIXED.z);
												
												//or push only downwards
												SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11_transform_pos;
												SPHERE_POS_FIXED = new Vector3(WORLD_POS.x,SPHERE_POS_FIXED.y,WORLD_POS.z);
												
											}
											
										}
										
										
										if(Turbulance){ //Version 1.2 ADDITION
											
											Vector3 POS_TURB = WORLD_POS;
											
											if(Axis_affected.y >0){
												POS_TURB.y=Turbulance_strength* Mathf.Cos (Time_fixed* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.y;
											}
											if(Axis_affected.x >0){
												POS_TURB.x=Turbulance_strength* Mathf.Cos (Time_fixed* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.x;
											}
											if(Axis_affected.z >0){
												POS_TURB.z=Turbulance_strength* Mathf.Cos (Time_fixed* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.z;
											}
											
											if(vortex_motion & p11_tag != "VortexSafe" & p11_tag != "Grass"){

												int count_active=0;
												for(int j=0;j< Vortexes.Count;j++){
													
													if(Vortexes[j].active){count_active=count_active+1;}
												}
												
												if(count_active<Vortex_count){
													
													
													Vortexes[count_active].position = ParticleList[i].position;
													Vortexes[count_active].velocity = ParticleList[i].velocity;
													Vortexes[count_active].particle_ID=i;
													Vortexes[count_active].lifetime = Vortex_life;
													Vortexes[count_active].angular_velocity = new Vector3(Vortex_angularvelocity,Vortex_angularvelocity,Vortex_angularvelocity);
													Vortexes[count_active].scale = Vortex_scale;

													Vortexes[count_active].active = true;

													ParticleList[i].remainingLifetime = Vortex_life;
													
													
												}
												
												
												for(int j=0;j< Vortexes.Count;j++){
													if(j< Vortexes.Count & Vortexes[j].active){
													if(Vortexes[j].particle_ID == i){
														
														Vortexes[j].position = ParticleList[i].position;
														Vortexes[j].velocity = ParticleList[i].velocity;
														
														if(Show_vortex){
															ParticleList[i].startSize = Vortex_center_size;
																		ParticleList[i].startColor = Vortex_center_color;
														}
														

															if(Vortexes[j].lifetime >Vortex_life){
																Vortexes[j].active=false;
															}
														
													}else { 
														
														
														Vector3 rVORT=ParticleList[i].position-Vortexes[j].position;

														if(limit_influence & (rVORT.magnitude > Vortexes[j].scale)  )
														{
															//do nothing
														}
														else{
															Vector3 vVORT=Vector3.Cross(Vortexes[j].angular_velocity, rVORT);
															
															float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortexes[j].scale)  );
															
															float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
															float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
															float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;
															
															

																	if(Lerp_velocity){
																		ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
																	}
																	else{
																		ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
																	}
														}
														
													}
												}
													
												}
											}
											
											SPHERE_POS_FIXED = new Vector3(  POS_TURB.x,POS_TURB.y,POS_TURB.z);
											
											
										}
										
										float DOUBLE_DIST=1f;
										float BOOST_DIST=1f;
										if(use_exponent){
											
											DOUBLE_DIST =0.000000001f*dist*(dist*dist*dist*dist);
											BOOST_DIST =100000000f;
											
											float SPEEDY = BOOST_DIST*Time_delta / (dumpen+0.1f*distance*DOUBLE_DIST);
											SPEEDY=(1/dist)+dumpen;
											
											SPEEDY=(affectDistance-Mathf.Sqrt(distance))/(affectDistance+(dumpen/100))*Time_delta;
											
											if(!gravity_pull){
												ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11_transform_pos;
												
												
												if(World_sim){
													ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
												}
											}
											
											
											if(gravity_pull & !gravity_planar){
												
												Vector3 rVORT=WORLD_POS-SPHERE_POS_FIXED;
												
												if(Swirl_grav){
														
														if(Affect_tres < rVORT.magnitude){
															ParticleList[i].velocity = ParticleList[i].velocity - rVORT.normalized*Gravity_factor/dist; 
															Gravity_Plane_INNER = ParticleList[i].velocity;
														}
														else{
															if(Bounce_grav){
																ParticleList[i].velocity = ParticleList[i].velocity + Bounce_factor*rVORT.normalized*Gravity_factor/dist;
															}
															else{
																Vector3 vVORT=Vector3.Cross(Gravity_Plane_INNER*Vortex_angularvelocity, rVORT);
																
																float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortex_scale));
																
																float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
																float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
																float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;
																
																

																if(Lerp_velocity){
																	ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
																}
																else{
																	ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
																}
															}
														}
												}
												else
												{
													ParticleList[i].velocity = ParticleList[i].velocity - rVORT.normalized*Gravity_factor/dist; 
												}

												ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11_transform_pos;
												
												
												if(World_sim){
													ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
												}
												
												if (rVORT.magnitude <= 0.1f)
												{
													ParticleList[i].remainingLifetime = 0;
												}
												
											}
											
											if(gravity_pull & gravity_planar){
												
												Vector3 rVORT=WORLD_POS-SPHERE_POS_FIXED;
												
												Vector3 vVORT=Vector3.Cross(Gravity_plane*Vortex_angularvelocity, rVORT);
												
												float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortex_scale));
												
												float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
												float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
												float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;
												
												ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11_transform_pos;
												
												
												if(World_sim){
													ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
												}
												
												

													if(Lerp_velocity){
														ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
													}
													else{
														ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
													}
											}
										}
										else
										{
											DOUBLE_DIST=1f;
											BOOST_DIST=1f;
											
											ParticleList[i].position  = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,BOOST_DIST*Time_delta / (dumpen+0.1f*distance*DOUBLE_DIST)) - p11_transform_pos;
											
											
											if(World_sim){
												ParticleList[i].position  = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,Time_delta / (dumpen+0.1f*distance*DOUBLE_DIST));
											}
										}
										
										if(Color_force){	//Version 1.2 ADDITION
											
													ParticleList[i].startColor = Color.Lerp(ParticleList[i].startColor, Force_color,0.1f);
											
										}
												//v1.6
												if(End_LifeOf_Affected){
													ParticleList[i].remainingLifetime=End_Life;
												}
										
									}
									
									
									if(splash_effect){
										if(ParticleList[i].remainingLifetime < 0.05f){
											ParticleList[i].remainingLifetime = ParticleList[i].startLifetime;
											ParticleList[i].position = p11_transform_pos;
										}
									}


											//v1.4
											if(vary_turbulence){				
												float X_VEC = ParticleList[i].position.x + (2*noise.Noise(ParticleList[i].position.z*noise_str,ParticleList[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time_delta;
												float Y_VEC = ParticleList[i].position.y + (2*noise.Noise(ParticleList[i].position.z*noise_str,ParticleList[i].position.x*noise_str*0.77f+1)-0)*noise_turb_str*Time_delta;
												float Z_VEC = ParticleList[i].position.z + (2*noise.Noise(ParticleList[i].position.x*noise_str,ParticleList[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time_delta;
												
												double X_VEC1 = 0; 
												double Y_VEC1 = 0; 
												double Z_VEC1 = 0; 												
												if(perlin_enable){
													X_VEC1 = ParticleList[i].velocity.x + noise.Noise( ParticleList[i].position.y * 10*axis_deviation.y+1, ParticleList[i].position.z * 1*axis_deviation.z)*Time_delta*noise_str*noise_turb_str; 
													Y_VEC1 = ParticleList[i].velocity.y + noise.Noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.z * 1*axis_deviation.z)*Time_delta*noise_str*noise_turb_str; 
													Z_VEC1 = ParticleList[i].velocity.z + noise.Noise(ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.y * 1*axis_deviation.y)*Time_delta*noise_str*noise_turb_str; 
												}else{
													X_VEC1 = ParticleList[i].velocity.x + SIMPLEXnoise.noise( ParticleList[i].position.y * 10*axis_deviation.y+1, ParticleList[i].position.z * 1*axis_deviation.z,Time_fixed*noise_str)*Time_delta*noise_str*noise_turb_str; 
													Y_VEC1 = ParticleList[i].velocity.y + SIMPLEXnoise.noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.z * 1*axis_deviation.z,Time_fixed*noise_str)*Time_delta*noise_str*noise_turb_str; 
													Z_VEC1 = ParticleList[i].velocity.z + SIMPLEXnoise.noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.y * 1*axis_deviation.y,Time_fixed*noise_str)*Time_delta*noise_str*noise_turb_str; 
												}												
												ParticleList[i].velocity = new Vector3((float)X_VEC1,(float)Y_VEC1,(float)Z_VEC1);	
												
												ParticleList[i].position = new Vector3(X_VEC,Y_VEC,Z_VEC);
												
												if(splash_noise){
													if(ParticleList[i].remainingLifetime < splash_noise_tres){
														
														if(count_vortex < Vortexes.Count){
															ParticleList[i].position = Vortexes[count_vortex].position;
															ParticleList[i].remainingLifetime = ParticleList[i].startLifetime;
															count_vortex++;
														}else{															
															count_vortex=0;
														}
														
													}
												}
												
											}
									
									
								}
							}
							#endregion



		} //END !multithreaded
		#endregion
		
		#region Non MultiThreaded

		if(!MultiThreaded){
		if(!make_moving_star){
		for (int i=0; i < ParticleList.Length;i++)
		{

			//if local simulation is selected in particle
			Vector3 WORLD_POS = ParticleList[i].position + p11.transform.position;

				if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){
					WORLD_POS = ParticleList[i].position;
				}
			
				dist = Vector3.SqrMagnitude(SPHERE_POS - WORLD_POS);
			if (dist < sqrDist) {
				
					float distance = dist;
					if (smoothattraction == false ){
						distance=0;
					}

					Vector3 SPHERE_POS_FIXED = SPHERE_POS;
					if(repel == true)
					{
						//find opposite to sphere-particle vector
						SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11.transform.position;

							if(limit_to_Y_axis | p11_tag == "Grass"){ //dont take height into account
						
							SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11.transform.position;
							SPHERE_POS_FIXED = new Vector3(SPHERE_POS_FIXED.x,WORLD_POS.y,SPHERE_POS_FIXED.z);

							//or push only downwards
							SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p11.transform.position;
							SPHERE_POS_FIXED = new Vector3(WORLD_POS.x,SPHERE_POS_FIXED.y,WORLD_POS.z);

						}

					}


								if(Turbulance){ //Version 1.2 ADDITION

										Vector3 POS_TURB = WORLD_POS;

										if(Axis_affected.y >0){
											POS_TURB.y=Turbulance_strength* Mathf.Cos (Time.fixedTime* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.y;
										}
										if(Axis_affected.x >0){
											POS_TURB.x=Turbulance_strength* Mathf.Cos (Time.fixedTime* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.x;
										}
										if(Axis_affected.z >0){
											POS_TURB.z=Turbulance_strength* Mathf.Cos (Time.fixedTime* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.z;
										}

										if(vortex_motion & p11_tag != "VortexSafe" & p11_tag != "Grass"){
											if(Vortexes.Count<Vortex_count){
													Vortex_PDM AA = new Vortex_PDM();
													AA.position = ParticleList[i].position;
													AA.velocity = ParticleList[i].velocity;
													AA.particle_ID=i;
													AA.lifetime = Vortex_life;
												AA.angular_velocity = new Vector3(Vortex_angularvelocity,Vortex_angularvelocity,Vortex_angularvelocity);
												AA.scale = Vortex_scale;

													ParticleList[i].remainingLifetime = Vortex_life;

													Vortexes.Add(AA);
												}


												
												for(int j=0;j< Vortexes.Count;j++){
													if(j< Vortexes.Count){
													if(Vortexes[j].particle_ID == i){
														
														Vortexes[j].position = ParticleList[i].position;
														Vortexes[j].velocity = ParticleList[i].velocity;
														
														if(Show_vortex){
																		ParticleList[i].startSize = Vortex_center_size;
																		ParticleList[i].startColor = Vortex_center_color;
														}
														
														if(Vortexes[j].lifetime >Vortex_life){
															Vortexes.Remove(Vortexes[j]);break;
														}
														
													}else { 
														
														
														Vector3 rVORT=ParticleList[i].position-Vortexes[j].position;

														if(limit_influence & (rVORT.magnitude > Vortexes[j].scale)  )
														{
															//do nothing
														}
														else{
															Vector3 vVORT=Vector3.Cross(Vortexes[j].angular_velocity, rVORT);
															
															float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortexes[j].scale)  );
															
															float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
															float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
															float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;
															
															

																	if(Lerp_velocity){
																		ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
																	}
																	else{
																		ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
																	}
														}
														
													}
												}

												}



										}

										SPHERE_POS_FIXED = new Vector3(  POS_TURB.x,POS_TURB.y,POS_TURB.z);

								
								}

									float DOUBLE_DIST=1f;
									float BOOST_DIST=1f;
									if(use_exponent){

										DOUBLE_DIST =0.000000001f*dist*(dist*dist*dist*dist);
										BOOST_DIST =100000000f;

										float SPEEDY = BOOST_DIST*Time.deltaTime / (dumpen+0.1f*distance*DOUBLE_DIST);
										SPEEDY=(1/dist)+dumpen;

										SPEEDY=(affectDistance-Mathf.Sqrt(distance))/(affectDistance+(dumpen/100))*Time.deltaTime;

										if(!gravity_pull){
											ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11.transform.position;
											
											if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){
												ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
											}
										}


										if(gravity_pull & !gravity_planar){

											Vector3 rVORT=WORLD_POS-SPHERE_POS_FIXED;

											if(Swirl_grav){
														
														if(Affect_tres < rVORT.magnitude){
															ParticleList[i].velocity = ParticleList[i].velocity - rVORT.normalized*Gravity_factor/dist; 
															Gravity_Plane_INNER = ParticleList[i].velocity;
														}
														else{
															if(Bounce_grav){
																ParticleList[i].velocity = ParticleList[i].velocity + Bounce_factor*rVORT.normalized*Gravity_factor/dist;
															}
															else{
																Vector3 vVORT=Vector3.Cross(Gravity_Plane_INNER*Vortex_angularvelocity, rVORT);
																
																float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortex_scale));
																
																float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
																float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
																float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;
																
																

																if(Lerp_velocity){
																	ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
																}
																else{
																	ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
																}
															}
														}
											}
											else
											{
											ParticleList[i].velocity = ParticleList[i].velocity - rVORT.normalized*Gravity_factor/dist; 
											}

											ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11.transform.position;
											
											if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){
												ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
											}

											if (rVORT.magnitude <= 0.1f)
											{
												ParticleList[i].remainingLifetime = 0;
											}

										}

										if(gravity_pull & gravity_planar){

											Vector3 rVORT=WORLD_POS-SPHERE_POS_FIXED;

											Vector3 vVORT=Vector3.Cross(Gravity_plane*Vortex_angularvelocity, rVORT);

											float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/Vortex_scale));

											float dvx = ParticleList[i].velocity.x + (vVORT.x - ParticleList[i].velocity.x)*factor;
											float dvy = ParticleList[i].velocity.y + (vVORT.y - ParticleList[i].velocity.y)*factor;
											float dvz = ParticleList[i].velocity.z + (vVORT.z - ParticleList[i].velocity.z)*factor;

											ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY) - p11.transform.position;
											
											if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){
												ParticleList[i].position  = Vector3.Slerp(WORLD_POS,SPHERE_POS_FIXED,SPEEDY);
											}

											

													if(Lerp_velocity){
														ParticleList[i].velocity = Vector3.Lerp(ParticleList[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
													}
													else{
														ParticleList[i].velocity = new Vector3(dvx,dvy,dvz);
													}
										}
									}
									else
									{
										DOUBLE_DIST=1f;
										BOOST_DIST=1f;

										ParticleList[i].position  = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,BOOST_DIST*Time.deltaTime / (dumpen+0.1f*distance*DOUBLE_DIST)) - p11.transform.position;
						
										if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){
														ParticleList[i].position  = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,Time.deltaTime / (dumpen+0.1f*distance*DOUBLE_DIST));
										}
									}

									if(Color_force){	//Version 1.2 ADDITION

													ParticleList[i].startColor = Color.Lerp(ParticleList[i].startColor, Force_color,0.1f);

									}
												//v1.6
												if(End_LifeOf_Affected){
													ParticleList[i].remainingLifetime=End_Life;
												}
				
					}


								if(splash_effect){
									if(ParticleList[i].remainingLifetime < 0.05f){
										ParticleList[i].remainingLifetime = ParticleList[i].startLifetime;
										ParticleList[i].position = p11.transform.position;
									}
								}

											//v1.4
											if(vary_turbulence){				
												float X_VEC = ParticleList[i].position.x + (2*noise.Noise(ParticleList[i].position.z*noise_str,ParticleList[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time.deltaTime;
												float Y_VEC = ParticleList[i].position.y + (2*noise.Noise(ParticleList[i].position.z*noise_str,ParticleList[i].position.x*noise_str*0.77f+1)-0)*noise_turb_str*Time.deltaTime;
												float Z_VEC = ParticleList[i].position.z + (2*noise.Noise(ParticleList[i].position.x*noise_str,ParticleList[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time.deltaTime;
												
												double X_VEC1 = 0; 
												double Y_VEC1 = 0; 
												double Z_VEC1 = 0; 												
												if(perlin_enable){
													X_VEC1 = ParticleList[i].velocity.x + noise.Noise( ParticleList[i].position.y * 10*axis_deviation.y+1, ParticleList[i].position.z * 1*axis_deviation.z)*Time.deltaTime*noise_str*noise_turb_str; 
													Y_VEC1 = ParticleList[i].velocity.y + noise.Noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.z * 1*axis_deviation.z)*Time.deltaTime*noise_str*noise_turb_str; 
													Z_VEC1 = ParticleList[i].velocity.z + noise.Noise(ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.y * 1*axis_deviation.y)*Time.deltaTime*noise_str*noise_turb_str; 
												}else{
													X_VEC1 = ParticleList[i].velocity.x + SIMPLEXnoise.noise( ParticleList[i].position.y * 10*axis_deviation.y+1, ParticleList[i].position.z * 1*axis_deviation.z,Time.fixedTime*noise_str)*Time.deltaTime*noise_str*noise_turb_str; 
													Y_VEC1 = ParticleList[i].velocity.y + SIMPLEXnoise.noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.z * 1*axis_deviation.z,Time.fixedTime*noise_str)*Time.deltaTime*noise_str*noise_turb_str; 
													Z_VEC1 = ParticleList[i].velocity.z + SIMPLEXnoise.noise( ParticleList[i].position.x * 10*axis_deviation.x+1, ParticleList[i].position.y * 1*axis_deviation.y,Time.fixedTime*noise_str)*Time.deltaTime*noise_str*noise_turb_str; 
												}												
												ParticleList[i].velocity = new Vector3((float)X_VEC1,(float)Y_VEC1,(float)Z_VEC1);	
												
												ParticleList[i].position = new Vector3(X_VEC,Y_VEC,Z_VEC);
												
												if(splash_noise){
													if(ParticleList[i].remainingLifetime < splash_noise_tres){
														ParticleList[i].remainingLifetime = ParticleList[i].startLifetime;
														int RN = Random.Range(0,Vortexes.Count-1);
														if(RN < Vortexes.Count){
															ParticleList[i].position = Vortexes[RN].position;
														}
													}
												}
												
											}


				}
			}
			} //END !multithreaded
						#endregion

			// particles painter
			if (Input.GetMouseButton(0) & p11.particleCount > 2 & enable_paint)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(ray, out hit, 100)) 
				{
					if(ParticleList != null & ParticleList.Length > 3 & counter >0 & counter < ParticleList.Length ){
								ParticleList[counter].position = p11.gameObject.transform.position + p11.gameObject.transform.InverseTransformPoint((hit.normal*hit_dist)+hit.point);
					}
					if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){
								if(ParticleList != null & ParticleList.Length > 3 & counter >0 & counter < ParticleList.Length ){
								ParticleList[counter].position =  hit.point;
								}

					}else{
								if(ParticleList != null & ParticleList.Length > 3 & counter >0 & counter < ParticleList.Length ){
								ParticleList[counter].position =  p11.transform.position -  hit.point ;
								}
					}

					counter=counter+1;
					
					if (counter >= p11.particleCount-1)
					{ counter=0;}
				}
			}
			//end painter

			//particle trails when repeling
			float OBJ_DIST = Vector3.Distance(p11.transform.position,transform.position);
			
			if (make_moving_star & p11.particleCount > 2 & repel & OBJ_DIST<star_trail_dist){
				for (int i=0; i<star_trails; i++) {
					
					float angle = 0;
					float trail_rotation = i*trail_distance;
					float radius = 0;
					
					for (int j = 0;  j < (p11.particleCount/star_trails);  j++) {
						
						int countit = i * (p11.particleCount/star_trails) + j;

						float trail_length=1;
						if(OBJ_DIST >0){
							trail_length = trail_length_out/OBJ_DIST;
						}
						
						float size_of_trail = size_of_trail_out*OBJ_DIST;

						radius = trail_length + distance_of_trail * angle;
						
						Vector3 Move_trail_pos = p11.transform.localPosition;
						
						Move_trail_pos.x = radius * Mathf.Cos(angle)+Move_trail_pos.x  ;
						Move_trail_pos.z = radius * Mathf.Sin(angle) + Move_trail_pos.z ;
						
						float x_pos = Move_trail_pos.x * Mathf.Cos(trail_rotation) + Move_trail_pos.z * Mathf.Sin(trail_rotation);
						float z_pos = -Move_trail_pos.x * Mathf.Sin(trail_rotation) + Move_trail_pos.z * Mathf.Cos(trail_rotation);
						Move_trail_pos.x = x_pos;
						Move_trail_pos.z = z_pos;
						
						Move_trail_pos.y = j * vertical_trail_separation+Move_trail_pos.y ;
						
						if(p11.main.simulationSpace == ParticleSystemSimulationSpace.World){
							Move_trail_pos = p11.transform.TransformPoint(Move_trail_pos);
						}
						ParticleList[countit].position = Move_trail_pos;
						
						angle = angle + distance_between_trail;
						
						ParticleList[countit].startSize = ParticleList[countit].startSize - j * size_of_trail;
						ParticleList[countit].angularVelocity = ParticleList[countit].angularVelocity- j * smooth_trail;
						
					}      
				} 
			}
			//spin it
			if (make_moving_star){
			p11.transform.Rotate(this.transform.up * Time.deltaTime * (-speed_of_trail), Space.World);
			}
			//end particle trails

			p11.SetParticles(ParticleList,p11.particleCount);

			}
			}
		}
		}



		/////////// LEGACY PARTICLES
		//v3.4.6
//		foreach(ParticleEmitter p in p3){
//			if(p != null){ // VERSION 1.2 CHANGE
//
//				int has_tag=0;
//				if(tags!=null){
//					for(int i=0;i<tags.Length;i++){
//						if(p.gameObject.tag == tags[i]){
//							has_tag=1;
//						}
//					}
//				}
//				
//						bool is_in_exclude_list=false;
//						
//						if(Exclude_tags !=null){
//							if(Exclude_tags.Length > 0)
//							{
//								for(int i=0;i<Exclude_tags.Length;i++){
//									if(p.gameObject.tag == Exclude_tags[i]){
//										is_in_exclude_list=true;
//									}
//								}
//							}
//						}
//
//
//		if((!affect_by_tag & !is_in_exclude_list) | (affect_by_tag & has_tag==1) ){ //VERSION 1.2 CHANGE
//
//		if(Vector3.Distance(thisTransform.position, p.transform.position) < (sqrDist*Dist_Factor) ){
//
//		particles = p.particles;
//		for (int i=0; i < p.particleCount;i++)
//		{
//
//				Vector3 WORLD_POS = particles[i].position;
//				if(!p.useWorldSpace){
//					WORLD_POS = particles[i].position + p.transform.position;
//				}
//
//				dist = Vector3.SqrMagnitude(SPHERE_POS - WORLD_POS);
//			if (dist < sqrDist) {
//
//				float distance = dist;
//				if (smoothattraction == false ){
//					distance=0;
//				}
//
//					Vector3 SPHERE_POS_FIXED = SPHERE_POS;
//					if(repel == true)
//					{
//						//find opposite to sphere-particle vector
//						SPHERE_POS_FIXED = ((WORLD_POS-SPHERE_POS)/(dumpen+0.1f*distance))+p.transform.position;
//					}
//
//								if(Turbulance & !MultiThreaded){ //Version 1.2 ADDITION, does not support multithreaded turbulance for legacy
//
//									Vector3 POS_TURB = WORLD_POS;
//									
//									if(Axis_affected.y >0){
//										POS_TURB.y=Turbulance_strength* Mathf.Cos (Time.fixedTime* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.y;
//									}
//									if(Axis_affected.x >0){
//										POS_TURB.x=Turbulance_strength* Mathf.Cos (Time.fixedTime* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.x;
//									}
//									if(Axis_affected.z >0){
//										POS_TURB.x=Turbulance_strength* Mathf.Cos (Time.fixedTime* Turbulance_frequency)*SPHERE_POS_FIXED.y * Axis_affected.z;
//									}
//
//									if(vortex_motion & p.gameObject.tag != "VortexSafe" & p.gameObject.tag != "Grass"){
//										if(Vortexes.Count<Vortex_count){
//											Vortex_PDM AA = new Vortex_PDM();
//											AA.position = particles[i].position;
//											AA.velocity = particles[i].velocity;
//											AA.particle_ID=i;
//											AA.lifetime = Vortex_life;
//											AA.angular_velocity = new Vector3(Vortex_angularvelocity,Vortex_angularvelocity,Vortex_angularvelocity);
//											AA.scale = Vortex_scale;
//											particles[i].energy = Vortex_life;
//											
//											Vortexes.Add(AA);
//										}
//										
//										foreach(Vortex_PDM BB in Vortexes){
//											if(BB.particle_ID == i){
//												
//												BB.position = particles[i].position;
//												BB.velocity = particles[i].velocity;
//												
//												if(Show_vortex){
//													particles[i].size = Vortex_center_size;
//													particles[i].color = Vortex_center_color;
//												}
//												
//												if(BB.lifetime >Vortex_life){
//													Vortexes.Remove(BB);break;
//												}
//												
//											}else { 
//												
//
//												
//												
//												Vector3 rVORT=particles[i].position-BB.position;
//												Vector3 vVORT=Vector3.Cross(BB.angular_velocity, rVORT);
//												
//												float factor = 1/( 1+ ((rVORT.x*rVORT.x + rVORT.y*rVORT.y+rVORT.z*rVORT.z)/BB.scale)  );
//												
//
//												float dvx = particles[i].velocity.x + (vVORT.x - particles[i].velocity.x)*factor;
//												float dvy = particles[i].velocity.y + (vVORT.y - particles[i].velocity.y)*factor;
//												float dvz = particles[i].velocity.z + (vVORT.z - particles[i].velocity.z)*factor;
//												
//												
//
//													if(Lerp_velocity){
//															particles[i].velocity = Vector3.Lerp(particles[i].velocity,new Vector3(dvx,dvy,dvz),0.5f);
//													}
//													else{
//															particles[i].velocity = new Vector3(dvx,dvy,dvz);
//													}
//												
//
//											}
//											
//											
//										}
//									}
//									
//									SPHERE_POS_FIXED = new Vector3(  POS_TURB.x,POS_TURB.y,POS_TURB.z);
//									
//									
//								}
//
//					particles[i].position = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,Time.deltaTime / (dumpen+0.1f*distance));
//
//					if(!p.useWorldSpace){
//						particles[i].position = Vector3.Lerp(WORLD_POS,SPHERE_POS_FIXED,Time.deltaTime / (dumpen+0.1f*distance)) - p.transform.position;
//					}
//
//								if(Color_force){	//Version 1.2 ADDITION
//									
//									particles[i].color = Color.Lerp(particles[i].color, Force_color,0.1f);
//									
//								}
//										//v1.6
//										if(End_LifeOf_Affected){
//											//particles[i].lifetime=End_Life;
//										}
//
//			}
//									//v1.4 - SKYMASTER
//									if(vary_turbulence){				
//										float X_VEC = particles[i].position.x + (2*noise.Noise(particles[i].position.z*noise_str,particles[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time.deltaTime;
//										float Y_VEC = particles[i].position.y + (2*noise.Noise(particles[i].position.z*noise_str,particles[i].position.x*noise_str*0.77f+1)-0)*noise_turb_str*Time.deltaTime;
//										float Z_VEC = particles[i].position.z + (2*noise.Noise(particles[i].position.x*noise_str,particles[i].position.y*noise_str*0.77f+1)-0)*noise_turb_str*Time.deltaTime;
//										
//										double X_VEC1 = 0; 
//										double Y_VEC1 = 0; 
//										double Z_VEC1 = 0; 												
//										if(perlin_enable){
//											X_VEC1 = particles[i].velocity.x + noise.Noise( particles[i].position.y * 10*axis_deviation.y+1, particles[i].position.z * 1*axis_deviation.z)*Time.deltaTime*noise_str*noise_turb_str; 
//											Y_VEC1 = particles[i].velocity.y + noise.Noise( particles[i].position.x * 10*axis_deviation.x+1, particles[i].position.z * 1*axis_deviation.z)*Time.deltaTime*noise_str*noise_turb_str; 
//											Z_VEC1 = particles[i].velocity.z + noise.Noise(particles[i].position.x * 10*axis_deviation.x+1, particles[i].position.y * 1*axis_deviation.y)*Time.deltaTime*noise_str*noise_turb_str; 
//										}else{
//											X_VEC1 = particles[i].velocity.x + SIMPLEXnoise.noise( particles[i].position.y * 10*axis_deviation.y+1, particles[i].position.z * 1*axis_deviation.z,Time.fixedTime*noise_str)*Time.deltaTime*noise_str*noise_turb_str; 
//											Y_VEC1 = particles[i].velocity.y + SIMPLEXnoise.noise( particles[i].position.x * 10*axis_deviation.x+1, particles[i].position.z * 1*axis_deviation.z,Time.fixedTime*noise_str)*Time.deltaTime*noise_str*noise_turb_str; 
//											Z_VEC1 = particles[i].velocity.z + SIMPLEXnoise.noise( particles[i].position.x * 10*axis_deviation.x+1, particles[i].position.y * 1*axis_deviation.y,Time.fixedTime*noise_str)*Time.deltaTime*noise_str*noise_turb_str; 
//										}												
//										particles[i].velocity = new Vector3((float)X_VEC1,(float)Y_VEC1,(float)Z_VEC1);	
//										
//										particles[i].position = new Vector3(X_VEC,Y_VEC,Z_VEC);
//										
////										if(splash_noise){
////											if(particles[i].lifetime < splash_noise_tres){
////												particles[i].lifetime = particles[i].startLifetime;
////												int RN = Random.Range(0,Vortexes.Count-1);
////												if(RN < Vortexes.Count){
////													particles[i].position = Vortexes[RN].position;
////												}
////											}
////										}
//										
//									}
//			
//		}
//
//			// particles painter
//			if (Input.GetMouseButton(0) & p.particleCount > 2 & enable_paint & particles != null)
//			{
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
//				RaycastHit hit = new RaycastHit();
//				if (Physics.Raycast(ray, out hit, 10)) 
//				{
//							if(particles != null & particles.Length > 3 & counter >0 & counter < particles.Length ){
//							particles[counter].position = p.transform.InverseTransformPoint((hit.normal*hit_dist)+hit.point);
//							}
//
//					if(!p.useWorldSpace){
//								if(particles != null & particles.Length > 3 & counter >0 & counter < particles.Length ){
//								particles[counter].position = p.transform.position - hit.point;
//								}
//					}
//					else{
//								if(particles != null & particles.Length > 3 & counter >0 & counter < particles.Length ){
//								particles[counter].position =hit.point; 
//								}
//					}
//					
//					counter=counter+1;
//					
//					if (counter >= p.particleCount-2)
//					{ counter=0;}
//				}
//			}
//			//end painter
//
//			//particle trails when repeling
//			float OBJ_DIST = Vector3.Distance(p.transform.position,transform.position);
//
//			if (make_moving_star & p.particleCount > 2 & repel & OBJ_DIST<star_trail_dist){
//				for (int i=0; i<star_trails; i++) {
//					
//					float angle = 0;
//					float trail_rotation = i*trail_distance;
//					float radius = 0;
//					
//					for (int j = 0;  j < (p.particleCount/star_trails);  j++) {
//						
//						int countit = i * (p.particleCount/star_trails) + j;
//
//						float trail_length=1;
//						if(OBJ_DIST >0){
//							trail_length = trail_length_out/OBJ_DIST;
//						}
//
//						float size_of_trail = size_of_trail_out*OBJ_DIST;
//
//						radius = trail_length + distance_of_trail * angle;
//						
//						Vector3 Move_trail_pos = p.transform.localPosition;
//
//						Move_trail_pos.x = radius * Mathf.Cos(angle)+Move_trail_pos.x  ;
//						Move_trail_pos.z = radius * Mathf.Sin(angle) + Move_trail_pos.z ;
//
//						float x_pos = Move_trail_pos.x * Mathf.Cos(trail_rotation) + Move_trail_pos.z * Mathf.Sin(trail_rotation);
//						float z_pos = -Move_trail_pos.x * Mathf.Sin(trail_rotation) + Move_trail_pos.z * Mathf.Cos(trail_rotation);
//						Move_trail_pos.x = x_pos;
//						Move_trail_pos.z = z_pos;
//
//						Move_trail_pos.y = j * vertical_trail_separation+Move_trail_pos.y ;
//
//						if (p.useWorldSpace){
//
//							Move_trail_pos = p.transform.TransformPoint(Move_trail_pos);
//						}
//						particles[countit].position = Move_trail_pos;
//
//						angle = angle + distance_between_trail;
//
//						particles[countit].size = particles[countit].size - j * size_of_trail;
//						particles[countit].energy = particles[countit].energy - j * smooth_trail;
//					}      
//				} 
//			}
//			//spin it
//			if (make_moving_star){
//			p.transform.Rotate(this.transform.up * Time.deltaTime * (-speed_of_trail), Space.World);
//			}
//			//end particle trails
//
//			p.particles = particles;
//		
//			}
//			}
//		}
//		}

	
		
    }
}
}

}