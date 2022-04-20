using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.GIPROXY {
	
	public class LightCollisionsPDM : MonoBehaviour {

        //v1.7.0 - add check if bounce path meets collider
        public bool check_secondary_collisions = false;
        public bool meet_half_way = false; //if 2ond collision found along the bounce light path, place it half way of colliders than on collision object

        //v1.6.5
        public bool Disable_on_tag = false;
		public List<string> Disable_Tags = new List<string>(); // list of tags that will not cast a light bounce
		public bool Disable_on_layer = false;
		public List<string> Disable_Layers = new List<string>();
		public bool Set_light_mask = false;
		public LayerMask Bounce_light_mask = new LayerMask();
		
		//v1.6
		public bool Cut_below_height=false;
		public float Cut_height=0;
		public bool Preview=false;
		
		//v1.5
		public bool SecondBounce = false;
		private int Cycle_ID = 0;//v1.5.1
		private int Cycle_ID1 = 0;//v1.5.1
		public bool Not_important_2ond=false;
		
		public bool use_light_sampling=false;//use when few lights are needed to smooth out color grading
		public float sampling_dist=15f;
		
		public bool use_grid=false;//use a grid to raycast, only register hits and search sources close to hero
		public int max_positions= 10;
		List<Vector3> ray_dest_positions = new List<Vector3>(); 
		List<Vector3> sphere_dest_positions = new List<Vector3>(); 
		bool got_random_offsets=false;
		public float extend_X=1f;
		public float extend_Y=1f;
		private bool randomize=false;
		Vector2[] rand_offsets;
		public bool go_random=false;
		
		public int hor_directions=5;
		public int ver_directions=5;
		public float PointLight_Radius=10f;
		public float PointLight_Radius_2ond=10f;
		public float Update_Color_Dist=1f;//distance at which point light bounced lights color will be updated, by casting ray from source to each light
		
		public bool Follow_Hero=false;
		public Transform HERO;

		//v3.0
		//PLAYER
		public bool Tag_based_player = false;//otherwise search for camera
		public string Player_tag = "Player";

		private Vector3 Hero_Dist;
		public Vector3 Hero_offset=new Vector3(0,0,0);
		private bool Hero_has_been_set=false;
		
		Transform LightTransform;
		
		void Start () {	

			//v3.0
			if (Tag_based_player) {
				if (HERO == null) {
					HERO = (GameObject.FindGameObjectWithTag (Player_tag)).transform;
				}
			} else {
				if (HERO == null) {
					HERO = Camera.main.transform;
				}
			}

			LightTransform=this.GetComponent<Light>().transform;
			Bounce_color = this.GetComponent<Light>().color;
			Start_light_pos = LightTransform.position;
			CameraTransform = Camera.main.transform;
			current_time = Time.fixedTime;
		}
		
		float current_time;
		
		[HideInInspector]
		public List<int> Lights_level;
		[HideInInspector]
		public List<Transform> Lights_source;
		
		[HideInInspector]
		public List<Transform> Lights;
		[HideInInspector]
		public List<Color> Lights_destination_color;
		
		public float Divide_init_intensity = 5f;
		
		public float Bounce_Intensity=5f;
		public float Bounce_Range = 230f;
		public float Degrade_speed = 0.6f; //speed that uneeded lights go out
		public float Appear_speed = 2f;
		
		
		public float min_dist_to_last_light=10; //minimal distance to last light bounced to keep bounced lights alive
		public float min_dist_to_source=10; //minimal distance to source light hit to keep bounced lights alive
		public float min_density_dist=1;//minimal distance to other lights to add a new one
		
		public bool Enable_follow=false; //lights will try to follow last hit, to save some new light creations
		public float Follow_dist=10f;//bounced lights will follow latest hit if distance is bigger than this value
		public float Follow_speed=10f;//speed of follow
		
		public Color Bounce_color;
		Vector3 Start_light_pos;
		public bool Jitter_directional = false;
		public float bound_up_down = 50f;
		public float HDR_drop_speed=1f;
		
		public bool get_hit_color=false;
		
		public bool get_texture_color=false;
		public bool mix_colors=false; //mix surface and light color for bouncing color
		public bool dynamic_update_color=false;
		
		public float Color_change_speed=0.5f;
		public bool Soften_color_change=false;
		public bool Use_light_color=false;//lerp from main source light color than bounce start light color
		
		public bool Debug_on=false;
		public bool Debug_2ond=false;
		public bool add_only_when_seen=false;
		private Transform CameraTransform;
		public bool close_to_camera=false;
		public float min_dist_to_camera=10f;
		public float max_hit_light_dist = 10f; //remove lights that are far away from current hit
		
		public bool Give_Offset=false;
		public float placement_offset= 5f; //offset along the hit vector, from the hit point, avoid point lights going close to surface
		
		public bool Origin_at_Projector=false; //
		public Transform Projector_OBJ;//
		
		public GameObject LightPool;//lights holder
		
		public float Update_every=0.1f;
		
		public bool Use_Height_cut_off=false;
		public float Cut_off_height=15;
		public float floor_if_no_hero=0;
		
		//v1.5
		public bool Jove_Lights=false;
		
		//v1.5.1
		public bool Grab_Skylight = false; // send raycasts back to the sky from each raycast to the ground and lerp color with it.
		public bool Rotate_grid = false; //use for spot light or when sun also moves (than just rotate)
		public float Sky_influence = 0.05f;
		public GameObject Sky;
		
		public bool offset_on_normal=false;
		
		void Update(){
			
			//Enable light pool, if disabled
			if(!LightPool.activeInHierarchy){
				LightPool.SetActive(true);
			}
			
			if(Cut_below_height){
				if(Projector_OBJ.position.y < Cut_height){
					if(LightPool.activeInHierarchy){
						LightPool.SetActive(false);
					}
					return;
				}
			}
			
			if(Time.fixedTime - current_time > Update_every){
				
				current_time=Time.fixedTime;
				
				float Color_speed = Color_change_speed;
				if(this.GetComponent<Light>().type == LightType.Point | Soften_color_change){
					Color_speed = Color_change_speed*Time.deltaTime;
				}
				
				
				Vector3 HERO_Adjust_vector=new Vector3(0,0,0);
				
				if(Follow_Hero){

					//v3.0
					if (Tag_based_player) {
						if (HERO == null) {
							HERO = (GameObject.FindGameObjectWithTag (Player_tag)).transform;
						}
					} else {
						if (HERO == null) {
							HERO = Camera.main.transform;
						}
					}

					//if(HERO==null){
						//Add custom hero in code here if required
						//HERO = ...
					//}
					
					if(HERO!=null & !Hero_has_been_set){
						
						Vector3 ORIGIN =  LightTransform.position;
						Vector3 DIRECTION = LightTransform.forward;
						
						RaycastHit hit0 = new RaycastHit();
						//Set hero dist by raycasting
						if(Physics.Raycast(ORIGIN,DIRECTION,out hit0,Mathf.Infinity))
						{
							Hero_Dist = hit0.point-HERO.position;
							Hero_has_been_set=true;
						}
					}
					
					if(HERO!=null){
						
						Vector3 ORIGIN =  LightTransform.position;
						Vector3 DIRECTION = LightTransform.forward;
						
						RaycastHit hit0 = new RaycastHit();
						//Set hero dist by raycasting
						if(Physics.Raycast(ORIGIN,DIRECTION,out hit0,Mathf.Infinity))
						{
							
							Hero_Dist = hit0.point-HERO.position;						
							
						}
						
						HERO_Adjust_vector =  -  Hero_Dist + Hero_offset;
						
					}
				}
				
				//ORIGIN 
				Vector3 Position_at_origin = LightTransform.position + HERO_Adjust_vector; //move casts around hero if Adjust not zero
				Vector3 Forward_vector = LightTransform.forward;
				
				if(Origin_at_Projector){
					if(Projector_OBJ!=null){
						
						Position_at_origin = Projector_OBJ.position + HERO_Adjust_vector;
						Forward_vector = Projector_OBJ.forward;
					}else{
						Debug.Log ("Please insert the projector gameobject to the script");
					}
				}
				
				if(this.GetComponent<Light>().type == LightType.Directional & Jitter_directional){
					
					if(Position_at_origin.y > Start_light_pos.y){
						Position_at_origin = new Vector3(Position_at_origin.x,Position_at_origin.y- (bound_up_down/10)*Time.deltaTime,Position_at_origin.z);
					}else{
						
						Position_at_origin = new Vector3(Position_at_origin.x,Position_at_origin.y+bound_up_down*Time.deltaTime,Position_at_origin.z);
					}
				}
				
				for (int i=Lights.Count-1;i>=0;i--){
					
					if(Lights[i]==null){
						
						//kill child bounce lights
						if(SecondBounce){
							for(int k=0;k<Lights.Count;k++){
								//if Light_source is same as Lights[i], fade 
								if(Lights_source[k] == Lights[i] & Lights_level[k] > 1){
									Lights[k].GetComponent<Light>().intensity=0;
									Destroy (Lights[k].gameObject);
									Lights.RemoveAt(k);//break;
									Lights_destination_color.RemoveAt(k);//break;
									Lights_level.RemoveAt(k);
									Lights_source.RemoveAt(k);
								}
							}
						}
						
						Lights.RemoveAt(i);
						Lights_destination_color.RemoveAt(i);
						Lights_level.RemoveAt(i);
						Lights_source.RemoveAt(i);
					}else				
					if(Lights[i].GetComponent<Light>().intensity ==0){					
						
						//kill child bounce lights
						if(SecondBounce){
							for(int k=0;k<Lights.Count;k++){
								//if Light_source is same as Lights[i], fade 
								if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
									Lights[k].GetComponent<Light>().intensity=0;
									Destroy (Lights[k].gameObject);
									Lights.RemoveAt(k);
									Lights_destination_color.RemoveAt(k);
									Lights_level.RemoveAt(k);
									Lights_source.RemoveAt(k);
								}
							}
						}
						
						Destroy (Lights[i].gameObject);
						Lights.RemoveAt(i);
						Lights_destination_color.RemoveAt(i);
						Lights_level.RemoveAt(i);
						Lights_source.RemoveAt(i);
					}
				}
				
				if(!use_grid){
					
					for (int i=Lights.Count-1;i>=0;i--){			
						
						//debug draw light
						if(Debug_on){
							Debug.DrawLine(Lights[i].position,new Vector3(Lights[i].position.x,Lights[i].position.y+15,Lights[i].position.z));
						}
						//Lerp to destination color
						if(use_light_sampling){
							if(Lights_destination_color[i] != Lights[i].GetComponent<Light>().color){
								Lights[i].GetComponent<Light>().color = Color.Lerp (Lights[i].GetComponent<Light>().color,Lights_destination_color[i],Color_speed);
							}
						}				
						
						//if far away from last registered light
						if(Vector3.Distance (Lights[i].position, Lights[Lights.Count-1].position) > min_dist_to_last_light){
							if(Lights[i].GetComponent<Light>().intensity >0){
								//v1.5
								if(this.GetComponent<Light>().type != LightType.Point & Lights_level[i] < 2){
									Lights[i].GetComponent<Light>().intensity-=Degrade_speed*Time.deltaTime;
									
									//fade child bounce lights
									if(SecondBounce){
										for(int k=0;k<Lights.Count;k++){
											//if Light_source is same as Lights[i], fade 
											if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
												Lights[k].GetComponent<Light>().intensity-=100*Degrade_speed*Time.deltaTime;
											}
										}
									}
								}
							}
							else{
								
								//kill child bounce lights
								if(SecondBounce){
									for(int k=0;k<Lights.Count;k++){
										//if Light_source is same as Lights[i], fade 
										if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
											Lights[k].GetComponent<Light>().intensity=0;
											Destroy (Lights[k].gameObject);
											Lights.RemoveAt(k);
											Lights_destination_color.RemoveAt(k);
											Lights_level.RemoveAt(k);
											Lights_source.RemoveAt(k);
										}
									}
								}
								
								Destroy (Lights[i].gameObject);
								Lights.RemoveAt(i);
								Lights_destination_color.RemoveAt(i);
								Lights_level.RemoveAt(i);
								Lights_source.RemoveAt(i);
							}
						}			
						else//if far away from light source
						if(Vector3.Distance (Lights[i].position, Position_at_origin) > min_dist_to_source){
							if(Lights[i].GetComponent<Light>().intensity >0){
								
								//v1.5
								if(Lights[i].GetComponent<Light>().type == LightType.Point & Lights_level[i] < 2){
									Lights[i].GetComponent<Light>().intensity-=Degrade_speed*Time.deltaTime;
									
									//fade child bounce lights
									if(SecondBounce){
										for(int k=0;k<Lights.Count;k++){
											//if Light_source is same as Lights[i], fade 
											if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
												Lights[k].GetComponent<Light>().intensity-=100*Degrade_speed*Time.deltaTime;
											}
										}
									}
								}
							}
							else{
								
								//kill child bounce lights
								if(SecondBounce){
									for(int k=0;k<Lights.Count;k++){
										//if Light_source is same as Lights[i], fade 
										if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
											Lights[k].GetComponent<Light>().intensity=0;
											Destroy (Lights[k].gameObject);
											Lights.RemoveAt(k);
											Lights_destination_color.RemoveAt(k);
											Lights_level.RemoveAt(k);
											Lights_source.RemoveAt(k);
										}
									}
								}
								
								Destroy (Lights[i].gameObject);
								Lights.RemoveAt(i);
								Lights_destination_color.RemoveAt(i);
								Lights_level.RemoveAt(i);
								Lights_source.RemoveAt(i);
							}
						}
						else 	//inrease intensity gradually
						{
							if(Lights[i].GetComponent<Light>().intensity < Bounce_Intensity & Lights_level[i] < 2){						
								Lights[i].GetComponent<Light>().intensity+=Appear_speed*Time.deltaTime;						
							}else
							if(HDR_drop_speed >0 & Lights[i].GetComponent<Light>().intensity > Bounce_Intensity+1 & Lights_level[i] < 2){						
								Lights[i].GetComponent<Light>().intensity-=HDR_drop_speed*Time.deltaTime;				
							}
							
							if(Lights[i].GetComponent<Light>().intensity < (Bounce_Intensity/Lights_level[i]) & Lights_level[i] >= 2){						
								Lights[i].GetComponent<Light>().intensity+=Appear_speed*Time.deltaTime;						
							}else
							if(HDR_drop_speed >0 & Lights[i].GetComponent<Light>().intensity > (Bounce_Intensity/2)+1 & Lights_level[i] >= 2){						
								Lights[i].GetComponent<Light>().intensity-=HDR_drop_speed*Time.deltaTime;				
							}
						}
					}
					
					if(Cycle_ID1 > Lights.Count-1){
						Cycle_ID1=0;
					}
					
					for (int i=Lights.Count-1;i>=0;i--){
						
						if(1==1){					
							//v1.5.1
							Cycle_ID1++;
							if(Cycle_ID1 > Lights.Count-1){
								Cycle_ID1=0;
							}
							
							RaycastHit hit1 = new RaycastHit();
							if(Physics.Raycast(Position_at_origin, Forward_vector, out hit1,Mathf.Infinity))
							{
								Vector3 Offseted_hit_point= new Vector3(0,0,0);
								
								if(!Give_Offset){
									Offseted_hit_point = hit1.point;
								}else{
									//Position_at_origin, LightTransform.forward
									if(offset_on_normal){
										Offseted_hit_point = hit1.point + (hit1.normal).normalized*placement_offset;
									}else{
										Offseted_hit_point = hit1.point + (Position_at_origin-hit1.point).normalized*placement_offset;
									}
								}					
								//check if light is far away from new hit point and remove if so
								if(Vector3.Distance (Lights[i].position, Offseted_hit_point) > max_hit_light_dist){
									if(Lights[i].GetComponent<Light>().intensity >0){
										//v1.5
										if(this.GetComponent<Light>().type != LightType.Point & Lights_level[i] < 2){
											Lights[i].GetComponent<Light>().intensity-=Degrade_speed*10*Time.deltaTime;
											
											//fade child bounce lights
											if(SecondBounce){
												for(int k=0;k<Lights.Count;k++){
													//if Light_source is same as Lights[i], fade 
													if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
														Lights[k].GetComponent<Light>().intensity-=100*Degrade_speed*Time.deltaTime;
													}
												}
											}
										}
									}
									else{
										//kill child bounce lights
										if(SecondBounce){
											for(int k=0;k<Lights.Count;k++){
												//if Light_source is same as Lights[i], fade 
												if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
													Lights[k].GetComponent<Light>().intensity=0;
													Destroy (Lights[k].gameObject);
													Lights.RemoveAt(k);
													Lights_destination_color.RemoveAt(k);
													Lights_level.RemoveAt(k);
													Lights_source.RemoveAt(k);
												}
											}
										}
										
										Destroy (Lights[i].gameObject);
										Lights.RemoveAt(i);
										Lights_destination_color.RemoveAt(i);
										Lights_level.RemoveAt(i);
										Lights_source.RemoveAt(i);
									}
								}					
								
							}//END raycast	
							//break;
						}
					}
				}else{ //IF GRID !!!!!
					
					for (int i=Lights.Count-1;i>=0;i--){			
						
						//debug draw light
						if(Debug_on){
							Debug.DrawLine(Lights[i].position,new Vector3(Lights[i].position.x,Lights[i].position.y+15,Lights[i].position.z));
						}
						
						//if far away from light source
						if(Vector3.Distance (Lights[i].position, Position_at_origin) > min_dist_to_source){
							if(Lights[i].GetComponent<Light>().intensity >0){
								
								//v1.5
								if(this.GetComponent<Light>().type == LightType.Point & Lights_level[i] < 2){ //dont lower 2ond+ bounce lights, their source will below
									Lights[i].GetComponent<Light>().intensity-=Degrade_speed*10*Time.deltaTime; //THIS MUST BE USED WITH A RADIUS BASED RAYCAST, otherwise lights are created and will immediately fade
									
									//fade child bounce lights
									if(SecondBounce){
										for(int k=0;k<Lights.Count;k++){				
											if(Lights_source[k] == Lights[i] & Lights_level[k] > 1){
												Lights[k].GetComponent<Light>().intensity-=100*Degrade_speed*Time.deltaTime;
												//Lights[k].light.intensity=0;
											}
										}
									}
								}
								
							}
							else{
								//v1.5
								if(this.GetComponent<Light>().type == LightType.Point){
									
									//kill child bounce lights
									if(SecondBounce){
										for(int k=0;k<Lights.Count;k++){
											//if Light_source is same as Lights[i], fade 
											if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
												Lights[k].GetComponent<Light>().intensity=0;
												Destroy (Lights[k].gameObject);
												Lights.RemoveAt(k);
												Lights_destination_color.RemoveAt(k);
												Lights_level.RemoveAt(k);
												Lights_source.RemoveAt(k);
											}
										}
									}
									
									Destroy (Lights[i].gameObject);
									Lights.RemoveAt(i);
									Lights_destination_color.RemoveAt(i);
									Lights_level.RemoveAt(i);
									Lights_source.RemoveAt(i);
								}
							}
						}
						else 	//inrease intensity gradually
						{
							if(Lights[i].GetComponent<Light>().intensity < Bounce_Intensity & Lights_level[i] < 2){							
								Lights[i].GetComponent<Light>().intensity+=Appear_speed*Time.deltaTime;
							}else							
							if(HDR_drop_speed >0 & Lights[i].GetComponent<Light>().intensity > Bounce_Intensity+1 & Lights_level[i] < 2){							
								Lights[i].GetComponent<Light>().intensity-=HDR_drop_speed*Time.deltaTime;					
							}
							
							if(Lights[i].GetComponent<Light>().intensity < (Bounce_Intensity/Lights_level[i]) & Lights_level[i] >= 2){						
								Lights[i].GetComponent<Light>().intensity+=Appear_speed*Time.deltaTime;						
							}else
							if(HDR_drop_speed >0 & Lights[i].GetComponent<Light>().intensity > (Bounce_Intensity/2)+1 & Lights_level[i] >= 2){						
								Lights[i].GetComponent<Light>().intensity-=HDR_drop_speed*Time.deltaTime;				
							}
						}
					}
					
					if(Cycle_ID1 > Lights.Count-1){
						Cycle_ID1=0;
					}
					
					for (int i=Lights.Count-1;i>=0;i--){
						
						if(1==1){						
							//v1.5.1
							Cycle_ID1++;
							if(Cycle_ID1 > Lights.Count-1){
								Cycle_ID1=0;
							}
							
							//v1.5 moved this above so it does not produce error when light is erased just before it runs
							//If point light, adjust color for each light by casting a ray from source to the light and see color
							if(this.GetComponent<Light>().type == LightType.Point){
								Vector3 DIRECTION = Lights[i].position-Position_at_origin; 
								Vector3 ORIGIN = Position_at_origin;
								
								RaycastHit hit1 = new RaycastHit();
								if(Physics.Raycast(ORIGIN, DIRECTION, out hit1,Mathf.Infinity))
								{
									Vector3 Offseted_hit_point= new Vector3(0,0,0);
									
									if(!Give_Offset){
										Offseted_hit_point = hit1.point;
									}else{
										//Position_at_origin, LightTransform.forward
										if(offset_on_normal){
											Offseted_hit_point = hit1.point + (hit1.normal).normalized*placement_offset;
										}else{
											Offseted_hit_point = hit1.point + (Position_at_origin-hit1.point).normalized*placement_offset;
										}
									}
									if(Vector3.Distance (hit1.point, Offseted_hit_point) < Update_Color_Dist){
										
										//v1.5 									
										Lights[i].gameObject.GetComponent<Light>().color = Get_color(Lights[i].gameObject,hit1,Color_speed);
										
									}
								}//END raycast
								
								if(Grab_Skylight){
									
									RaycastHit hit_sky = new RaycastHit();	
									
									if(Physics.Raycast(ORIGIN,-DIRECTION,out hit_sky,Mathf.Infinity))
									{
										bool sky_in=true;
										if(Sky != null){
											if(Sky.name != hit_sky.collider.gameObject.name){
												sky_in = false;
											}
										}
										
										if(sky_in){
											Lights[i].gameObject.GetComponent<Light>().color = Get_color(Lights[i].gameObject, hit_sky,Color_speed*Sky_influence);
										}
										
										if(Debug_on){
											//Debug.DrawLine(ORIGIN,hit_sky.point,Color.blue);
											//Debug.Log(hit_sky.point);
											//Debug.Log(final_color);
										}
									}
									
								}
							}
							
							
							bool Light_leaves=true;
							for (int k=0;k<ray_dest_positions.Count;k++)
							{
								Vector3 DIRECTION = Forward_vector; 
								Vector3 ORIGIN = Position_at_origin+ray_dest_positions[k];
								
								RaycastHit hit1 = new RaycastHit();
								if(Physics.Raycast(ORIGIN, DIRECTION, out hit1,Mathf.Infinity))
								{
									Vector3 Offseted_hit_point= new Vector3(0,0,0);
									
									if(!Give_Offset){
										Offseted_hit_point = hit1.point;
									}else{
										//Position_at_origin, LightTransform.forward
										if(offset_on_normal){
											Offseted_hit_point = hit1.point + (hit1.normal).normalized*placement_offset;
										}else{
											Offseted_hit_point = hit1.point + (Position_at_origin-hit1.point).normalized*placement_offset;
										}
									}
									if(Vector3.Distance (Lights[i].position, Offseted_hit_point) < max_hit_light_dist){
										Light_leaves=false;
									}
								}//END raycast				
							}
							
							//check if light is far away from new hit point and remove if so
							if(Light_leaves){
								
								if(Lights[i].GetComponent<Light>().intensity >0){
									
									//v1.5								
									if(this.GetComponent<Light>().type != LightType.Point & Lights_level[i] < 2){
										Lights[i].GetComponent<Light>().intensity-=Degrade_speed*10*Time.deltaTime;
										
										//fade child bounce lights
										if(SecondBounce){
											for(int k=0;k<Lights.Count;k++){
												//if Light_source is same as Lights[i], fade 
												if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
													Lights[k].GetComponent<Light>().intensity-=100*Degrade_speed*Time.deltaTime;
												}
											}
										}
									}
									
								}
								else{
									//kill child bounce lights
									if(SecondBounce){
										for(int k=0;k<Lights.Count;k++){
											//if Light_source is same as Lights[i], fade 
											if(Lights_source[k] == Lights[i]& Lights_level[k] > 1){
												Lights[k].GetComponent<Light>().intensity=0;
												Destroy (Lights[k].gameObject);
												Lights.RemoveAt(k);
												Lights_destination_color.RemoveAt(k);
												Lights_level.RemoveAt(k);
												Lights_source.RemoveAt(k);
											}
										}
									}
									
									Destroy (Lights[i].gameObject);
									Lights.RemoveAt(i);
									Lights_destination_color.RemoveAt(i);
									Lights_level.RemoveAt(i);
									Lights_source.RemoveAt(i);
								}
							}
							
							
							//break;
						}
					}
				}
				
				//////////////// ADD BOUNCE LIGHTS ////////////////
				/// 
				///// DEFINE GRID
				/// 
				if(use_grid & !got_random_offsets){
					
					ray_dest_positions.Clear();				
					int GET_X = (int)Mathf.Sqrt(max_positions);
					
					if(!got_random_offsets){
						
						got_random_offsets=true;					
						rand_offsets=new Vector2[GET_X*GET_X];
						
						int count_me=0;
						for (int m=0;m<GET_X;m++){
							for (int n=0;n<GET_X;n++){							
								rand_offsets[count_me] = new Vector2(Random.Range(0,extend_X*(GET_X/2)*m)+(1.5f*count_me), Random.Range(0,extend_Y*(GET_X/2)*n)+(1.1f*count_me)  );
								count_me=count_me+1;
							}
						}					
					}
					int count_2=0;
					for (int m=0;m<GET_X;m++){
						for (int n=0;n<GET_X;n++){
							
							float X_AXIS = (extend_X*(GET_X/2)*m);
							float Z_AXIS = (extend_Y*(GET_X/2)*n);
							if(randomize){
								X_AXIS = X_AXIS+Random.Range(0,extend_X*(GET_X/2)*m);
								Z_AXIS = Z_AXIS+Random.Range(0,extend_Y*(GET_X/2)*n);
							}
							
							if(!go_random){		
								ray_dest_positions.Add(new Vector3(- X_AXIS, 0, - Z_AXIS  ));
								ray_dest_positions.Add(new Vector3(X_AXIS, 0, - Z_AXIS ));
							}
							
							if(go_random){		
								ray_dest_positions.Add(new Vector3(- rand_offsets[count_2].x, 0,- rand_offsets[count_2].y  ));
								count_2=count_2+1;
							}						
						}
					}
				}
				
				//find light type, raycast towards the light direction and add point lights
				if(this.GetComponent<Light>()!=null){
					
					if(this.GetComponent<Light>().type == LightType.Directional | this.GetComponent<Light>().type == LightType.Spot){
						
						Vector3 ORIGIN = Position_at_origin;
						Vector3 DIRECTION = Forward_vector;
						
						if(!use_grid & ray_dest_positions.Count<1){
							ray_dest_positions.Add (new Vector3(0,0,0)); //add a dummy to enter loop
						}
						
						for (int k=0;k<ray_dest_positions.Count;k++)
						{
							if(use_grid){ //pick one position in random to sample
								
								DIRECTION = Forward_vector; 
								
								if(Rotate_grid){
									if(Origin_at_Projector){
										ORIGIN = Position_at_origin+Projector_OBJ.transform.rotation*ray_dest_positions[k];
									}else{
										ORIGIN = Position_at_origin+ LightTransform.rotation*ray_dest_positions[k];
									}
								}else{
									ORIGIN = Position_at_origin+ray_dest_positions[k];
								}
							}
							
							//start directional ray casting
							RaycastHit hit1 = new RaycastHit();
							
							if(Physics.Raycast(ORIGIN,DIRECTION,out hit1,Mathf.Infinity))
							{
								Vector3 Offseted_hit_point= new Vector3(0,0,0);
								
								if(!Give_Offset){
									Offseted_hit_point = hit1.point;
								}else{
									if(offset_on_normal){
										Offseted_hit_point = hit1.point + (hit1.normal).normalized*placement_offset;
									}else{
										Offseted_hit_point = hit1.point + (Position_at_origin-hit1.point).normalized*placement_offset;
									}
								}
								
								if(Debug_on){
									Debug.DrawLine(ORIGIN,Offseted_hit_point,Color.magenta);
								}

                                //v1.7.0 - add check if bounce path meets collider
                                if (check_secondary_collisions)
                                {
                                    RaycastHit hit11 = new RaycastHit();
                                    if (Physics.Raycast(hit1.point, Offseted_hit_point - hit1.point, out hit11, (Offseted_hit_point - hit1.point).magnitude))
                                    {
                                        if ((hit11.point - hit1.point).magnitude < (Offseted_hit_point - hit1.point).magnitude)
                                        {
                                            if (meet_half_way)
                                            {
                                                Offseted_hit_point = hit1.point + (hit11.point - hit1.point).normalized * (hit11.point - hit1.point).magnitude / 2;
                                            }
                                            else
                                            {
                                                Offseted_hit_point = hit1.point;
                                            }
                                        }
                                    }
                                }

                                bool make_light = true;
								bool first_light = false;
								
								if(!(Lights.Count>0)){
									first_light=true;
								}else{
									//move lights toward new hit point
									if(Enable_follow){
										for (int i=Lights.Count-1;i>=0;i--){
											
											if(Vector3.Distance (Lights[i].position, Offseted_hit_point) > Follow_dist){
												Lights[i].position = Vector3.Lerp(Lights[i].position, Offseted_hit_point,Follow_speed*Time.deltaTime);
											}
										}
									}
									
									//MAKE LIGHTS conditionals
									//only if camera sees the light
									if(add_only_when_seen){
										
										RaycastHit hit11 = new RaycastHit();
										if(Physics.Raycast(CameraTransform.position, Offseted_hit_point, out hit11,Mathf.Infinity))
										{	
											if(hit11.point.magnitude>0){
												if(Vector3.Distance(hit11.point,CameraTransform.position) < Vector3.Distance(Offseted_hit_point,CameraTransform.position))
												{
													
													make_light=false;
												}
											}
										}								
									}

                                    

                                    //if away from camera (now must also add a check if current lights away from new hit point and remove them)
                                    if (close_to_camera){								
										//min_dist_to_camera
										if(Vector3.Distance (CameraTransform.position, Offseted_hit_point) > min_dist_to_camera){
											make_light=false;
										}								
									}
									
									for (int i=Lights.Count-1;i>=0;i--){
										
										//MAKE LIGHTS conditionals
										
										if(Vector3.Distance (Lights[i].position, Offseted_hit_point) < min_density_dist){
											make_light=false;
										}
										
										//find where light hits NOW and if some lights are close, give them new color (for changing surfaces)
										if(dynamic_update_color & get_hit_color){
											
											if(Vector3.Distance (Lights[i].position, Offseted_hit_point) < min_density_dist){
												
												Lights[i].gameObject.GetComponent<Light>().color = Get_color(Lights[i].gameObject,hit1,Color_speed);
												
											}
											
											if(Grab_Skylight){
												
												RaycastHit hit_sky = new RaycastHit();	
												
												if(Physics.Raycast(ORIGIN,-DIRECTION,out hit_sky,Mathf.Infinity))
												{
													bool sky_in=true;
													if(Sky != null){
														if(Sky.name != hit_sky.collider.gameObject.name){
															sky_in = false;
														}
													}
													
													if(sky_in){
														Lights[i].gameObject.GetComponent<Light>().color = Get_color(Lights[i].gameObject, hit_sky,Color_speed*Sky_influence);
													}
													
													if(Debug_on){
														Debug.DrawLine(ORIGIN,hit_sky.point,Color.blue);
														//Debug.Log(hit_sky.point);
														//Debug.Log(final_color);
													}
												}
												
											}
											
										}// END DYNAMIC UPDATE
									}
								}
								
								//add only if below a certain height from hero, to cut off roof lights etc
								if(Use_Height_cut_off){
									if(Follow_Hero){
										if((hit1.point.y - HERO.position.y) > Cut_off_height){
											make_light=false;
										} 
									}else{ //use floor_if_no_hero as floor
										if((hit1.point.y - floor_if_no_hero) > Cut_off_height){
											make_light=false;
										} 
									}
								}
								
								//v1.6.5 - Tag handling
								if(Disable_on_tag){
									for (int Tag_id=0;Tag_id<Disable_Tags.Count;Tag_id++){
										if(hit1.collider.gameObject.tag == Disable_Tags[Tag_id]){
											make_light = false;
										}
									}
								}
								//v1.6.5 - Layer handling
								if(Disable_on_layer){
									for (int Layer_id=0;Layer_id<Disable_Layers.Count;Layer_id++){
										if(LayerMask.LayerToName(hit1.collider.gameObject.layer) == Disable_Layers[Layer_id]){
											make_light = false;
										}
									}
								}
								
								if( (make_light & !first_light) | first_light){ 
									
									// Make a game object
									GameObject lightGameObject = new GameObject("Bounce Light");
									
									// Add the light component
									lightGameObject.AddComponent(typeof(Light));
									// Set color and position
									
									if(!get_hit_color){
										
										if(!mix_colors){
											lightGameObject.GetComponent<Light>().color = Bounce_color;
										}else{
											lightGameObject.GetComponent<Light>().color = Color.Lerp(Bounce_color,GetComponent<Light>().color,Color_speed);
										}
									}else{
										
										lightGameObject.GetComponent<Light>().color  = Get_color(lightGameObject,hit1,Color_speed);
										
									}
									
									lightGameObject.GetComponent<Light>().intensity= Bounce_Intensity/Divide_init_intensity;
									lightGameObject.GetComponent<Light>().range = Bounce_Range;
									lightGameObject.transform.position = Offseted_hit_point;
									
									//v1.6.5
									if(Set_light_mask){
										lightGameObject.GetComponent<Light>().cullingMask = Bounce_light_mask;
									}
									
									//save first
									Lights_destination_color.Add(lightGameObject.GetComponent<Light>().color);
									
									Color final_color=lightGameObject.GetComponent<Light>().color;
									if(use_light_sampling){
										//check starting color against near lights and lerp to it, save desired color destination per light
										for (int i=0;i<Lights.Count;i++){
											if(Vector3.Distance(Lights[i].position,Offseted_hit_point)<sampling_dist){
												final_color = Color.Lerp(final_color,Lights[i].GetComponent<Light>().color,0.1f);
											}
										}
									}
									
									if(Grab_Skylight){
										
										RaycastHit hit_sky = new RaycastHit();
										
										if(Physics.Raycast(ORIGIN,-DIRECTION,out hit_sky,Mathf.Infinity))
										{
											bool sky_in=true;
											if(Sky != null){
												if(Sky.name != hit_sky.collider.gameObject.name){
													sky_in = false;
												}
											}
											
											if(sky_in){
												final_color = Get_color(lightGameObject, hit_sky,Color_speed*Sky_influence);
											}
											
											if(Debug_on){
												//Debug.DrawLine(ORIGIN,hit_sky.point,Color.blue);
												//Debug.Log(hit_sky.point);
												//Debug.Log(final_color);
											}
										}
										
									}
									
									
									lightGameObject.GetComponent<Light>().color = final_color;
									
									Lights.Add(lightGameObject.transform);
									
									if(Jove_Lights){																				
										//Add control script
										lightGameObject.AddComponent(typeof(Control_Jove_light));
									}
									
									
									Lights_level.Add(1);
									Lights_source.Add(LightTransform);
									
									lightGameObject.transform.parent = LightPool.transform;
									
								}//END distance check
							}//END raycast
						}//END cycle grid points
						
					}//END first lighttype check
					
					
					// 2ond LIGHT TYPE - POINT
					if(this.GetComponent<Light>().type == LightType.Point){
						
						Vector3 ORIGIN = Position_at_origin;
						Vector3 DIRECTION = Forward_vector;						
						
						if(sphere_dest_positions!=null){
							if(!(sphere_dest_positions.Count >0)){
								//add all directions
								
								foreach (Vector3 direction in GetSphereDirections(hor_directions*ver_directions))
								{
									
									sphere_dest_positions.Add (direction);
									
								}
								
							}
							
						}
						
						for (int k=0;k<sphere_dest_positions.Count;k++)
						{
							if(use_grid){ //pick one position in random to sample
								
								ORIGIN = Position_at_origin;
								DIRECTION = sphere_dest_positions[k];
								
							}
							
							//start directional ray casting
							RaycastHit hit1 = new RaycastHit();
							
							//if(Physics.Raycast(ORIGIN,DIRECTION,out hit1,Mathf.Infinity)) //min_dist_to_source
							if(Physics.Raycast(ORIGIN,DIRECTION,out hit1,(min_dist_to_source-0.00001f))) //min_dist_to_source - v1.5
							{
								Vector3 Offseted_hit_point= new Vector3(0,0,0);
								
								if(!Give_Offset){
									Offseted_hit_point = hit1.point;
								}else{
									//LightTransform.position, LightTransform.forward
									if(offset_on_normal){
										Offseted_hit_point = hit1.point + (hit1.normal).normalized*placement_offset;
									}else{
										Offseted_hit_point = hit1.point + (Position_at_origin-hit1.point).normalized*placement_offset;
									}
								}
								
								if(Debug_on){
									Debug.DrawLine(ORIGIN,Offseted_hit_point,Color.magenta);
								}

                                //v1.7.0 - add check if bounce path meets collider
                                if (check_secondary_collisions)
                                {
                                    RaycastHit hit11 = new RaycastHit();
                                    if (Physics.Raycast(hit1.point, Offseted_hit_point - hit1.point, out hit11, (Offseted_hit_point - hit1.point).magnitude))
                                    {
                                        if ((hit11.point - hit1.point).magnitude < (Offseted_hit_point - hit1.point).magnitude)
                                        {
                                            if (meet_half_way)
                                            {
                                                Offseted_hit_point = hit1.point + (hit11.point - hit1.point).normalized * (hit11.point - hit1.point).magnitude / 2;
                                            }
                                            else
                                            {
                                                Offseted_hit_point = hit1.point;
                                            }
                                        }
                                    }
                                }

                                bool make_light = true;
								bool first_light = false;
								
								if(!(Lights.Count>0)){
									first_light=true;
								}else{
									//move lights toward new hit point
									if(Enable_follow){
										for (int i=Lights.Count-1;i>=0;i--){
											
											if(Vector3.Distance (Lights[i].position, Offseted_hit_point) > Follow_dist){
												Lights[i].position = Vector3.Lerp(Lights[i].position, Offseted_hit_point,Follow_speed*Time.deltaTime);
											}
										}
									}
									
									//MAKE LIGHTS conditionals
									//only if camera sees the light
									if(add_only_when_seen){
										
										RaycastHit hit11 = new RaycastHit();
										if(Physics.Raycast(CameraTransform.position, Offseted_hit_point, out hit11,Mathf.Infinity))
										{	
											if(hit11.point.magnitude>0){
												if(Vector3.Distance(hit11.point,CameraTransform.position) < Vector3.Distance(Offseted_hit_point,CameraTransform.position))
												{
													
													make_light=false;
												}
											}
										}								
									}

                                    

                                    //if away from camera (now must also add a check if current lights away from new hit point and remove them)
                                    if (close_to_camera){								
										//min_dist_to_camera
										if(Vector3.Distance (CameraTransform.position, Offseted_hit_point) > min_dist_to_camera){
											make_light=false;
										}								
									}
									
									for (int i=Lights.Count-1;i>=0;i--){
										
										//MAKE LIGHTS conditionals										
										
										if(Vector3.Distance (Lights[i].position, Offseted_hit_point) < min_density_dist){
											make_light=false;
										}
										
										//find where light hits NOW and if some lights are close, give them new color (for changing surfaces)
										if(dynamic_update_color & get_hit_color){
											if(Vector3.Distance (Lights[i].position, Offseted_hit_point) < (min_density_dist/2)   ){
												
												Lights[i].gameObject.GetComponent<Light>().color = Get_color(Lights[i].gameObject,hit1,Color_speed);
												
												//Debug.Log ("IN1");
											}
										}// END DYNAMIC UPDATE
									}
								}
								
								//check distance from point
								if(PointLight_Radius<0){PointLight_Radius=0;}
								if(PointLight_Radius != 0){
									if(Vector3.Distance(hit1.point,GetComponent<Light>().transform.position)< PointLight_Radius){
										
									}else{
										make_light=false;
									}
								}
								
								//v1.6.5 - Tag handling
								if(Disable_on_tag){
									for (int Tag_id=0;Tag_id<Disable_Tags.Count;Tag_id++){
										if(hit1.collider.gameObject.tag == Disable_Tags[Tag_id]){
											make_light = false;
										}
									}
								}
								//v1.6.5 - Layer handling
								if(Disable_on_layer){
									for (int Layer_id=0;Layer_id<Disable_Layers.Count;Layer_id++){
										if(LayerMask.LayerToName(hit1.collider.gameObject.layer) == Disable_Layers[Layer_id]){
											make_light = false;
										}
									}
								}
								
								if( (make_light & !first_light) | first_light){ 
									
									// Make a game object
									GameObject lightGameObject = new GameObject("Bounce Light");
									
									// Add the light component
									lightGameObject.AddComponent(typeof(Light));
									// Set color and position
									
									if(!get_hit_color){
										
										if(!mix_colors){
											lightGameObject.GetComponent<Light>().color = Bounce_color;
										}else{
											lightGameObject.GetComponent<Light>().color = Color.Lerp(Bounce_color,GetComponent<Light>().color,Color_speed);
										}
									}else{
										
										lightGameObject.GetComponent<Light>().color = Get_color(lightGameObject,hit1,Color_speed);
										
									}
									
									lightGameObject.GetComponent<Light>().intensity= Bounce_Intensity/Divide_init_intensity;
									lightGameObject.GetComponent<Light>().range = Bounce_Range;
									lightGameObject.transform.position = Offseted_hit_point;
									
									//v1.6.5
									if(Set_light_mask){
										lightGameObject.GetComponent<Light>().cullingMask = Bounce_light_mask;
									}
									
									//save first
									Lights_destination_color.Add(lightGameObject.GetComponent<Light>().color);
									
									Color final_color=lightGameObject.GetComponent<Light>().color;
									if(use_light_sampling){
										//check starting color against near lights and lerp to it, save desired color destination per light
										for (int i=0;i<Lights.Count;i++){
											if(Vector3.Distance(Lights[i].position,Offseted_hit_point)<sampling_dist){
												final_color = Color.Lerp(final_color,Lights[i].GetComponent<Light>().color,0.1f);
											}
										}
									}
									lightGameObject.GetComponent<Light>().color = final_color;
									
									
									Lights.Add(lightGameObject.transform);
									
									if(Jove_Lights){																				
										//Add control script
										lightGameObject.AddComponent(typeof(Control_Jove_light));
									}
									
									Lights_level.Add(1);
									Lights_source.Add(LightTransform);
									
									
									lightGameObject.transform.parent = LightPool.transform;
									
								}//END distance check
							}//END raycast
						}//END cycle grid points
						
					}//END SECOND !!!! lighttype chck
					
					
					
					
					
					
					///// ADD 2ond BOUNCES from Bounce point lights
					/// 
					if(SecondBounce){
						
						if(Cycle_ID > Lights.Count-1){
							Cycle_ID=0;
						}
						
						for(int j=0;j< Lights.Count;j++){
							if(Cycle_ID == j){
								//v1.5.1
								Cycle_ID++;
								if(Cycle_ID > Lights.Count-1){
									Cycle_ID=0;
								}
								
								// 2ond LIGHT TYPE - POINT							
								if(Lights[j].GetComponent<Light>().type == LightType.Point){ //change for 2ond
									
									Vector3 ORIGIN = Lights[j].position;
									Vector3 DIRECTION = Forward_vector;						
									
									if(sphere_dest_positions!=null){
										if(!(sphere_dest_positions.Count >0)){
											//add all directions
											
											foreach (Vector3 direction in GetSphereDirections(hor_directions*ver_directions))
											{											
												sphere_dest_positions.Add (direction);											
											}										
										}									
									}
									
									for (int k=0;k<sphere_dest_positions.Count;k++)
									{
										if(use_grid){ //pick one position in random to sample
											
											ORIGIN = Lights[j].position;
											DIRECTION = sphere_dest_positions[k];
											
										}
										
										//start directional ray casting
										RaycastHit hit1 = new RaycastHit();
										
										//if(Physics.Raycast(ORIGIN,DIRECTION,out hit1,Mathf.Infinity))
										if(Physics.Raycast(ORIGIN,DIRECTION,out hit1,(PointLight_Radius_2ond)))
										{
											Vector3 Offseted_hit_point= new Vector3(0,0,0);
											
											if(!Give_Offset){
												Offseted_hit_point = hit1.point;
											}else{											
												if(offset_on_normal){
													Offseted_hit_point = hit1.point + (hit1.normal).normalized*placement_offset;
												}else{
													Offseted_hit_point = hit1.point + (Position_at_origin-hit1.point).normalized*placement_offset;
												}
											}
											
											if(Debug_2ond){
												Debug.DrawLine(ORIGIN,Offseted_hit_point,Color.magenta);
											}

                                            //v1.7.0 - add check if bounce path meets collider
                                            if (check_secondary_collisions)
                                            {
                                                RaycastHit hit11 = new RaycastHit();
                                                if (Physics.Raycast(hit1.point, Offseted_hit_point - hit1.point, out hit11, (Offseted_hit_point - hit1.point).magnitude))
                                                {
                                                    if ((hit11.point - hit1.point).magnitude < (Offseted_hit_point - hit1.point).magnitude)
                                                    {
                                                        if (meet_half_way)
                                                        {
                                                            Offseted_hit_point = hit1.point + (hit11.point - hit1.point).normalized * (hit11.point - hit1.point).magnitude / 2;
                                                        }
                                                        else
                                                        {
                                                            Offseted_hit_point = hit1.point;
                                                        }
                                                    }
                                                }
                                            }

                                            bool make_light = true;
											bool first_light = false;
											
											if(!(Lights.Count>0)){
												first_light=true;
											}else{
												//move lights toward new hit point
												if(Enable_follow){
													for (int i=Lights.Count-1;i>=0;i--){													
														if(Vector3.Distance (Lights[i].position, Offseted_hit_point) > Follow_dist){
															Lights[i].position = Vector3.Lerp(Lights[i].position, Offseted_hit_point,Follow_speed*Time.deltaTime);
														}
													}
												}
												
												//MAKE LIGHTS conditionals
												//only if camera sees the light
												if(add_only_when_seen){
													
													RaycastHit hit11 = new RaycastHit();
													if(Physics.Raycast(CameraTransform.position, Offseted_hit_point, out hit11,Mathf.Infinity))
													{	
														if(hit11.point.magnitude>0){
															if(Vector3.Distance(hit11.point,CameraTransform.position) < Vector3.Distance(Offseted_hit_point,CameraTransform.position))
															{
																
																make_light=false;
															}
														}
													}								
												}

                                               

                                                //if away from camera (now must also add a check if current lights away from new hit point and remove them)
                                                if (close_to_camera){								
													//min_dist_to_camera
													if(Vector3.Distance (CameraTransform.position, Offseted_hit_point) > min_dist_to_camera){
														make_light=false;
													}								
												}
												
												for (int i=Lights.Count-1;i>=0;i--){
													
													//MAKE LIGHTS conditionals										
													
													if(Vector3.Distance (Lights[i].position, Offseted_hit_point) < min_density_dist){
														make_light=false;
													}
													
													//find where light hits NOW and if some lights are close, give them new color (for changing surfaces)
													if(Lights_level[i] >= 2){
														if(dynamic_update_color & get_hit_color){
															if(Vector3.Distance (Lights[i].position, Offseted_hit_point) < (min_density_dist/2)   ){
																
																Lights[i].gameObject.GetComponent<Light>().color = Get_color(Lights[i].gameObject,hit1,Color_speed);
																
																//Debug.Log ("IN2");
															}
														}// END DYNAMIC UPDATE
													}
												}
											}
											
											//check distance from point
											if(PointLight_Radius_2ond<0){PointLight_Radius_2ond=0;}
											if(PointLight_Radius_2ond != 0){
												if(Vector3.Distance(hit1.point,Lights[j].GetComponent<Light>().transform.position)< PointLight_Radius_2ond){ //change for 2ond
													
												}else{
													make_light=false;
												}
											}
											
											//v1.6.5 - Tag handling
											if(Disable_on_tag){
												for (int Tag_id=0;Tag_id<Disable_Tags.Count;Tag_id++){
													if(hit1.collider.gameObject.tag == Disable_Tags[Tag_id]){
														make_light = false;
													}
												}
											}
											//v1.6.5 - Layer handling
											if(Disable_on_layer){
												for (int Layer_id=0;Layer_id<Disable_Layers.Count;Layer_id++){
													if(LayerMask.LayerToName(hit1.collider.gameObject.layer) == Disable_Layers[Layer_id]){
														make_light = false;
													}
												}
											}
											
											if( (make_light & !first_light) | first_light){ 
												
												if(Lights_level[j] < 2 & 1==1){
													// Make a game object
													GameObject lightGameObject = new GameObject("Indirect Bounce Light Level "+(Lights_level[j]+1));
													
													// Add the light component
													lightGameObject.AddComponent(typeof(Light));
													// Set color and position
													
													if(!get_hit_color){												
														if(!mix_colors){
															lightGameObject.GetComponent<Light>().color = Bounce_color;
														}else{
															lightGameObject.GetComponent<Light>().color = Color.Lerp(Bounce_color,Lights[j].GetComponent<Light>().color,Color_speed); //change for 2ond
														}
													}else{
														
														//lightGameObject.light.color = Get_color(lightGameObject,hit1,Color_speed);
														//v1.6 - get the subtle color addition of the parent light final color (not just the main light)
														lightGameObject.GetComponent<Light>().color = Color.Lerp(Get_color(lightGameObject,hit1,Color_speed),Lights[j].GetComponent<Light>().color,0.5f);
														
													}
													
													lightGameObject.GetComponent<Light>().intensity= Bounce_Intensity/Divide_init_intensity;
													lightGameObject.GetComponent<Light>().range = Bounce_Range;
													lightGameObject.transform.position = Offseted_hit_point;
													
													//v1.6.5
													if(Set_light_mask){
														lightGameObject.GetComponent<Light>().cullingMask = Bounce_light_mask;
													}
													
													//save first
													Lights_destination_color.Add(lightGameObject.GetComponent<Light>().color);
													
													Color final_color=lightGameObject.GetComponent<Light>().color;
													if(use_light_sampling){
														//check starting color against near lights and lerp to it, save desired color destination per light
														for (int i=0;i<Lights.Count;i++){
															if(Vector3.Distance(Lights[i].position,Offseted_hit_point)<sampling_dist){
																final_color = Color.Lerp(final_color,Lights[i].GetComponent<Light>().color,0.1f);
															}
														}
													}
													lightGameObject.GetComponent<Light>().color = final_color;
													
													
													//not important
													if(Not_important_2ond){
														lightGameObject.GetComponent<Light>().renderMode = LightRenderMode.ForceVertex;
													}
													
													Lights.Add(lightGameObject.transform);
													
													if(Jove_Lights){
														
														//Add control script
														lightGameObject.AddComponent(typeof(Control_Jove_light));
														
													}
													
													int decide_level=2;//put above 1 to be on the safe side
													if(Lights_level[j] >1){
														decide_level = Lights_level[j]+1;
													}
													Lights_level.Add(decide_level);
													Lights_source.Add(Lights[j]);
													
													lightGameObject.transform.parent = LightPool.transform;
												}
												
												break;
												
											}//END distance check
										}//END raycast
									}//END cycle grid points
									
								}//END each 2ond bounce point light handle 
								break;
							}
						} // END cycling of bounce lights that cast 2ondary rays
						
					}// END 2OND BOUNCES HANDLE
					
					
				}else{
					
					Debug.Log ("Add the script to a light");
					
				}
			}else{ //Reset timer
				
			}
		}// END UPDATE 
		
		
		
		/// <summary>
		/// HELPER FUNCTIONS		/// </summary>
		/// <returns>The sphere directions.</returns>
		/// <param name="numDirections">Number directions.</param>
		
		
		private Vector3[] GetSphereDirections(int numDirections)
		{
			Vector3[] pts = new Vector3[numDirections];
			float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
			float off = 2f / numDirections;
			
			
			for(int k=0;k<numDirections;k++){
				var y = k * off - 1 + (off / 2);
				var r = Mathf.Sqrt(1 - y * y);
				var phi = k * inc;
				var x = (float)(Mathf.Cos(phi) * r);
				var z = (float)(Mathf.Sin(phi) * r);
				pts[k] = new Vector3(x, y, z);
			}
			
			
			return pts;
		}
		
		private Color Get_color(GameObject lightGameObject, RaycastHit hit1,float Color_speed){
			
			Color Out_color=lightGameObject.GetComponent<Light>().color;
			
			if(hit1.transform.GetComponent<Renderer>()!=null){
				if(hit1.transform.GetComponent<Renderer>().material!=null){
					if(get_texture_color & hit1.transform.GetComponent<Renderer>().material.HasProperty("_MainTex")){
						
						if(hit1.transform.GetComponent<Renderer>().material.mainTexture!=null){														
							
							Texture2D TextureMap = hit1.transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
							
							bool has_texture_readable=true;
							try
							{
								if(TextureMap!=null){
									TextureMap.GetPixel(0, 0);
								}else{
									has_texture_readable = false;
								}
							}
							catch (UnityException e)
							{
								if(e.Message.StartsWith("Texture '" + TextureMap.name + "' is not readable"))
								{
									has_texture_readable = false;
								}
							}
							
							if(has_texture_readable){
								Vector2 pixelUV = hit1.textureCoord;
								pixelUV.x *= TextureMap.width;
								pixelUV.y *= TextureMap.height;
								if(!mix_colors){
									Out_color = TextureMap.GetPixel ((int)pixelUV.x,(int)pixelUV.y);
								}else{
									if(Use_light_color){
										Out_color= Color.Lerp(GetComponent<Light>().color,lightGameObject.GetComponent<Light>().color,Color_speed);
									}else{
										
										Out_color = Color.Lerp(lightGameObject.GetComponent<Light>().color,TextureMap.GetPixel ((int)pixelUV.x,(int)pixelUV.y),Color_speed);
										
									}
								}
							}
						}else{
							if(hit1.transform.GetComponent<Renderer>().material.HasProperty("_Color")){
								if(!mix_colors){
									Out_color = hit1.transform.GetComponent<Renderer>().material.color;
								}else{
									if(Use_light_color){
										Out_color = Color.Lerp(GetComponent<Light>().color,hit1.transform.GetComponent<Renderer>().material.color,Color_speed);
									}else{
										
										Out_color = Color.Lerp(lightGameObject.GetComponent<Light>().color,Color.Lerp(hit1.transform.GetComponent<Renderer>().material.color,GetComponent<Light>().color,0.5f),Color_speed);
										
									}
								}
							}else{
								Out_color = GetComponent<Light>().color;
							}
						}
					}else{
						if(hit1.transform.GetComponent<Renderer>().material.HasProperty("_Color")){
							if(!mix_colors){
								Out_color = hit1.transform.GetComponent<Renderer>().material.color;
							}else{
								if(Use_light_color){
									Out_color = Color.Lerp(GetComponent<Light>().color,hit1.transform.GetComponent<Renderer>().material.color,Color_speed);
								}else{
									Out_color = Color.Lerp(lightGameObject.GetComponent<Light>().color,hit1.transform.GetComponent<Renderer>().material.color,Color_speed);
								}
							}
						}else{
							Out_color = GetComponent<Light>().color;
						}
					}
				}
			}else{
				if(!mix_colors){
					Out_color = Bounce_color;
				}else{
					Out_color = Color.Lerp(Bounce_color,GetComponent<Light>().color,Color_speed);
				}
			}
			
			return Out_color;
			
		}
		
	}
}