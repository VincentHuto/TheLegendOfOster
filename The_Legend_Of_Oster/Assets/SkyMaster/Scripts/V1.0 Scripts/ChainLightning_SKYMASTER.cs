using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;
//using System.Reflection;

namespace Artngame.SKYMASTER {

//	public static class CloneComponentSKYMASTER
//	{
//		public static T GetCopyOf<T>(this Component comp, T other) where T : Component
//		{
//			Type type = comp.GetType();
//			if (type != other.GetType()) return null; // type mis-match
//			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
//			PropertyInfo[] pinfos = type.GetProperties(flags);
//			foreach (var pinfo in pinfos) {
//				if (pinfo.CanWrite) {
//					try {
//						pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
//					}
//					catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
//				}
//			}
//			FieldInfo[] finfos = type.GetFields(flags);
//			foreach (var finfo in finfos) {
//				finfo.SetValue(comp, finfo.GetValue(other));
//			}
//			return comp as T;
//		}
//	}
	
	public class ChainLightning_SKYMASTER : MonoBehaviour
	{
		public Transform target;
		private GameObject[] target1;
		public int zigs = 100;
		public float speed = 1f;
		public float scale = 1f;
		public Light startLight;
		public Light endLight;
		
		PerlinSKYMASTER noise;
		//float oneOverZigs;//v3.4.6
		
		//private Particle[] particles;
		
		//v1.4
		public bool Energized = false; //Activate externally to signal a parent has send lighting to this object
		public bool is_parent = false;
		public int current_depth; //filled externally, a non parent must search for targets if lower than max_depth and re-transmit it minus one;
		public int max_depth; //filled externally, a non parent must search for targets if lower than max_depth and re-transmit
		private int current_target_count;
		public int max_target_count=3;
		
		void Start()
		{
			if(zigs ==0){zigs=1;}
			//oneOverZigs = 1f / (float)zigs;//v3.4.6
			
			//v3.4.6
//			GetComponent<ParticleEmitter>().emit = false;
//			Emitter =GetComponent<ParticleEmitter>(); 
//			
//			GetComponent<ParticleEmitter>().Emit(zigs);
//			particles = GetComponent<ParticleEmitter>().particles;
			
			target1 = GameObject.FindGameObjectsWithTag("Conductor");
			
			if(endLight){endLight.enabled=false; endLight.gameObject.SetActive(false);}
			if(startLight){startLight.enabled=false; startLight.gameObject.SetActive(false);}
			
			Target_pos = transform.position;
			Saved_start_pos = Target_pos; //v1.7
			this_transform = transform;
			
			Target_delay = Change_target_delay;
			
			if(Random_delay){
				Target_delay = UnityEngine.Random.Range(Change_target_delay - Delay_offset, Change_target_delay + Delay_offset);
			}
		}
		
		//v1.7
		Transform this_transform;
		//ParticleEmitter Emitter;//v3.4.6
		
		public bool Random_target;
		public float Affect_dist = 10f;
		
		private float Time_count;
		public float Change_target_delay=0.5f;
		
		//v1.7
		public bool Random_delay=false;
		public float Delay_offset=1;
		float Target_delay;
		public bool Moving_Source = false;
		
		public float Particle_energy=1f;
		
		public int optimize_factor=5;
		public float ParticleSize=5;
		
		public bool Branches = true;
		public Vector3 Branch_Offset;
		public Vector3 offset_bias;
		
		Vector3 Target_pos;
		Vector3 Saved_start_pos; //v1.7
		
		public float Downward_speed = 4;
		public float Stop_dist = 1; //stop lightning if reached
		bool cast_branches = false;
		public Color Branch_color = Color.white;
		
		[Range(1,6)]
		public float Zigs_branching_divider = 2;
		
		public bool offset_noise=false;
		public bool reset_noise=false;
		
		public bool Slow_on_end=false;
		public float Slow_down_height=20;
		public float Slow_divider=40;
		
		//v1.7
		public Vector2 Light_delay = new Vector2(0.0f,0.3f);
		bool particle_on = true;
		public bool line_on = false;
		LineRenderer Liner;
		float stop_time;
		public Vector2 Line_delay = new Vector2(0.1f,0.4f);
		
		public int Vertex_count = 30;
		public float Deviation = 10.2f;
		
		public Vector2 Line_width = new Vector2(10,10);// width start end

		//v2.2
		public float min_y_disp = 10;
		public bool line_glow_on = false;
		LineRenderer LinerGlow;
		public Color Glow_color = Color.blue;
		public Material Line_Lightning_mat;
		public List<LineRenderer> LineBranches = new List<LineRenderer>();
		public List<LineRenderer> LineBranchesGlow = new List<LineRenderer>();
		public int max_branches = 10;

		public bool SmoothLights = false;//lerp lights than enable/disable
		public float max_end_light_intensity = 1;
		public float max_start_light_intensity = 8;
		public float LightFadeSpeed = 5;
		public float LightSpeed = 100;
		public float LightRipple = 3;

		void Update ()
		{		

			//v3.0
			if (target == null & Random.Range (0, 2) == 1) {
				return;
			}
			//

			if (line_on) {			
				if(Liner == null){
					Liner = GetComponent<LineRenderer>();

					//v2.2
					if(Liner == null){
						this.gameObject.AddComponent<LineRenderer>();
						Liner = GetComponent<LineRenderer>();
						//Liner.SetWidth(20,10);
						Liner.startWidth = 20; //v3.4.9
						Liner.endWidth = 10; //v3.4.9
						//Liner.SetColors(Color.white,Color.white);
						Liner.startColor = Color.white; //v3.4.9
						Liner.endColor = Color.white; //v3.4.9
						Liner.material = Line_Lightning_mat;
					}
				}

				//v2.2
				if(line_glow_on){
					if(LinerGlow == null){
						GameObject Glowholder = new GameObject();
						Glowholder.name ="GlowHolder";
						//GameObject Glowholder = (GameObject)Instantiate(new GameObject());
						Glowholder.transform.parent = this.transform;
						Glowholder.transform.localPosition = Vector3.zero;
						LinerGlow = Glowholder.AddComponent<LineRenderer>();
						//LinerGlow.SetWidth(17,30);
						LinerGlow.startWidth = 17; //v3.4.9
						LinerGlow.endWidth = 30; //v3.4.9
						//LinerGlow.SetColors(Color.black,Glow_color);
						LinerGlow.startColor = Color.black; //v3.4.9
						LinerGlow.endColor = Glow_color; //v3.4.9
						LinerGlow.material = Line_Lightning_mat;
					}
				}

				//v2.2
				if(Branches){
					if(LineBranches.Count < max_branches){
						for(int i=0;i<max_branches;i++){
							GameObject Lineholder = new GameObject();
							Lineholder.name="Lineholder"+i.ToString();
							Lineholder.transform.parent = this.transform;
							Lineholder.transform.localPosition = Vector3.zero;
							LineRenderer LinerTMP = Lineholder.AddComponent<LineRenderer>();
							//LinerTMP.SetWidth(5,10);
							LinerTMP.startWidth = 5; //v3.4.9
							LinerTMP.endWidth = 10; //v3.4.9
							//LinerTMP.SetColors(Color.white,Color.white);
							LinerTMP.startColor = Color.white; //v3.4.9
							LinerTMP.endColor = Color.white; //v3.4.9
							LinerTMP.material = Line_Lightning_mat;
							LineBranches.Add(LinerTMP);
						}
					}
					if(line_glow_on && LineBranchesGlow.Count < max_branches){
						for(int i=0;i<max_branches;i++){
							GameObject Glowholder = new GameObject();
							Glowholder.name="Glowholder"+i.ToString();
							Glowholder.transform.parent = this.transform;
							Glowholder.transform.localPosition = Vector3.zero;
							LineRenderer LinerGlow1 = Glowholder.AddComponent<LineRenderer>();
							//LinerGlow1.SetWidth(9,20);
							LinerGlow1.startWidth = 9; //v3.4.9
							LinerGlow1.endWidth = 20; //v3.4.9
							//LinerGlow1.SetColors(Color.black,Glow_color);
							LinerGlow1.startColor = Color.black; //v3.4.9
							LinerGlow1.endColor = Glow_color; //v3.4.9
							LinerGlow1.material = Line_Lightning_mat;
							LineBranchesGlow.Add(LinerGlow1);
						}
					}
				}
			}
			
			
			if(particle_on){
				if (noise == null)
				{noise = new PerlinSKYMASTER();}
				
				if(is_parent | (!is_parent & Energized & current_depth < max_depth) ){
					
					target1 = GameObject.FindGameObjectsWithTag("Conductor");
					
					if(target1 !=null){
						if(target1.Length > 0 ){
							
							int Choose = UnityEngine.Random.Range(0,target1.Length);
							if(Random_target){
								
								if(Time.fixedTime-Time_count > Target_delay ){ //v1.7
									
									if(Vector3.Distance(target1[Choose].transform.position, this.transform.position) < Affect_dist){
										target= target1[Choose].transform;
										
										Saved_start_pos = this_transform.position;
										
									}else{
										//GetComponent<ParticleEmitter>().ClearParticles();//v3.4.6
									}
									Time_count = Time.fixedTime;
									
									//v1.4
									//disable after N targets are found and hit
									if(!is_parent & Energized & current_depth < max_depth & current_target_count > max_target_count ){
										Energized = false;
									}
									current_target_count++;
									
								}
								if(target!=null){
									//v1.7
									if(Vector3.Distance(target.position, this.transform.position) > Affect_dist){
										target= null;
										//GetComponent<ParticleEmitter>().ClearParticles();//v3.4.6
									}
								}
							}
							else{
								
								target=null;
								//GetComponent<ParticleEmitter>().ClearParticles();//v3.4.6
								
								int count_each=0;
								foreach(GameObject TRANS in target1){
									
									
									if( Vector3.Distance(TRANS.transform.position, this.transform.position) < Affect_dist){
										
										target= TRANS.transform;
										
									}
									count_each=count_each+1;
								}
								
								//v1.4
								//disable after N targets are found and hit
								if(!is_parent & Energized & current_depth < max_depth & current_target_count > max_target_count ){
									Energized = false;
								}
								current_target_count++;
							}
							
							float timex = Time.time * speed * 0.1365143f;
							//float timey = Time.time * speed * 1.21688f; //v3.4.6
							//float timez = Time.time * speed * 2.5564f; //v3.4.6
							
							if(target!=null){
								float DIST_TARG = Vector3.Distance(Target_pos,target.position);
								if(DIST_TARG < (Vector3.Distance(transform.position,target.position)/2) ){
									
									if(Branches){
										cast_branches = true;
									}
								}
								
								if(Target_pos == target.position | DIST_TARG < Stop_dist){
									target = null;
									Target_pos = this_transform.position;// transform.position; //restart new //v1.7
									//Saved_start_pos = Target_pos;
									cast_branches = false;
									
									//Emitter.emit = false;//v3.4.6
									//Emitter.ClearParticles();//v3.4.6
								}
								
							}
							
							
							if(target!=null){
								Vector3 offset_bias1=offset_bias;
								if(offset_noise){
									if(reset_noise){
										offset_bias1.x = offset_bias.x + Mathf.Clamp(0.1f*noise.Noise(timex+offset_bias.x),-0.15f,0.15f);
										offset_bias1.x = Mathf.Clamp(offset_bias.x, -10.25f,10.25f);
									}else{
										offset_bias1.x = offset_bias.x + Mathf.Clamp(0.1f*noise.Noise(timex+offset_bias.x),-0.15f,0.15f);
										offset_bias1.x = Mathf.Clamp(offset_bias.x, -0.25f,0.25f);
										offset_bias = offset_bias1;
									}
								}
								
								float Speedy = Downward_speed;
								if(Slow_on_end){
									float DIST_TARG1 = Vector3.Distance(Target_pos,target.position);
									if(DIST_TARG1 < Stop_dist+Slow_down_height){
										Speedy = Downward_speed/Slow_divider;
									}
								}
								
								
								//// line rendered details
								if(Moving_Source){
									Target_pos = Vector3.Lerp(this_transform.position,target.position,Speedy*Time.deltaTime);
								}else{
									Target_pos = Target_pos + Speedy*(target.position - Target_pos).normalized;
								}
								
								
								if(line_on){
									if(Liner !=null){

										//if(line_on){
											//if(Liner !=null){
												//if(Time.fixedTime - stop_time > UnityEngine.Random.Range(Line_delay.x,Line_delay.y)){
													//Liner.SetWidth(0,0);
													//LinerGlow.SetWidth(0,0);
										if(LinerGlow!=null){
													LinerGlow.enabled = true;//v3.0
										}
													Liner.enabled = true;
													if(Branches){
														for(int i=0;i<LineBranches.Count;i++){
															//LineBranches[i].SetWidth(0,0);
															//LineBranchesGlow[i].SetWidth(0,0);
															LineBranches[i].enabled = true;
															if(line_glow_on){
																LineBranchesGlow[i].enabled = true;
															}
														}
													}
												//}
											//}
										//}

										int Vertex_count = 30;
										float Deviation = 10.2f;


										//v2.2
										if(line_glow_on){
											//pass parameters to other line too
											//LinerGlow.SetWidth(Liner.
											//LinerGlow = this.GetComponent<LineRenderer>().GetCopyOf;
											//	LinerGlow.GetCopyOf(Liner);
											
											//LinerGlow.SetVertexCount(Vertex_count);
											LinerGlow.positionCount = Vertex_count; //v3.4.9
											//for(int i = 0;i< Vertex_count;i++){
											//	LinerGlow.SetPosition(i,Liner.
											//}
											
											//LinerGlow.SetWidth(17,11);
											LinerGlow.startWidth = 17; //v3.4.9
											LinerGlow.endWidth = 11; //v3.4.9
											//LinerGlow.SetColors(Color.black,Glow_color);
											LinerGlow.startColor = Color.black; //v3.4.9
											LinerGlow.endColor = Glow_color; //v3.4.9
											//LinerGlow.material = Line_Lightning_mat;
										}

										
										//Liner.SetWidth(Line_width.x,Line_width.y);
										Liner.startWidth = Line_width.x; //v3.4.9
										Liner.endWidth = Line_width.y; //v3.4.9

										List<Vector3> Branch_L1_starts = new List<Vector3>();
										List<Vector3> Branch_L2_starts = new List<Vector3>();
										if(Branches){
											//Liner.SetVertexCount(Vertex_count*1); // double count for branches
											Liner.positionCount = Vertex_count; //v3.4.9
										}else{
											//Liner.SetVertexCount(Vertex_count);
											Liner.positionCount = Vertex_count; //v3.4.9
										}

										Vector3 Current_line_pos = Saved_start_pos;
										Vector3 Current_end_pos = Vector3.zero;
										
										Vector3 Randomizer =  (target.position - this_transform.position) / Vertex_count;
										Liner.SetPosition(0,this_transform.position);
										for(int i = 0;i< Vertex_count;i++){
											Current_end_pos.x = UnityEngine.Random.Range(Current_line_pos.x - Deviation+Randomizer.x, Current_line_pos.x + Deviation+Randomizer.x);
											Current_end_pos.y = Current_line_pos.y +Randomizer.y;
											Current_end_pos.z = UnityEngine.Random.Range(Current_line_pos.z - Deviation+Randomizer.z, Current_line_pos.z + Deviation+Randomizer.z);

											//V2.2
//											if(Mathf.Abs(Current_end_pos.z - Current_line_pos.z) < min_z_disp){
//												Current_end_pos.z = Current_end_pos.z +min_z_disp;
//											}
											if(Mathf.Abs(Current_end_pos.y - Current_line_pos.y) < min_y_disp){
												Current_end_pos.y = Current_end_pos.y - min_y_disp;
											}
											
											Liner.SetPosition(i,Current_end_pos);

											//v2.2
											if(line_glow_on){
												LinerGlow.SetPosition(i,Current_end_pos);
											}

											Current_line_pos=Current_end_pos;

											if((i/5)-Mathf.RoundToInt(i/5) == 0 & i > Vertex_count/3){
												if(Branch_L1_starts.Count < max_branches/3){			//GRAB Level 1 branches, using 1 third of availble
													Branch_L1_starts.Add(Current_line_pos);
												}
											}
										}

                                        //Liner.SetPosition(Vertex_count-1,target.position); //v4.8.5
                                        Liner.SetPosition(Vertex_count - 1, new Vector3(Current_end_pos.x, target.position.y, Current_end_pos.z)); //v4.8.5

                                        if (Branches){

										//v2.2 - Branches L1
										for(int j=0;j<Branch_L1_starts.Count;j++){
											Vector3 Current_line_pos1 = Branch_L1_starts[j];
											Vector3 Current_end_pos1 = Vector3.zero;
											//LineBranches[j].SetVertexCount(Vertex_count);
												LineBranches[j].positionCount = Vertex_count; //v3.4.9
											//LineBranches[j].SetWidth(5,1);
												LineBranches[j].startWidth = 5; //v3.4.9
												LineBranches[j].endWidth = 1; //v3.4.9

											LineBranches[j].SetPosition(0,Branch_L1_starts[j]);

											if(line_glow_on){
												//LineBranchesGlow[j].SetVertexCount(Vertex_count);
													LineBranchesGlow[j].positionCount = Vertex_count; //v3.4.9
												//LineBranchesGlow[j].SetWidth(10,5);
													LineBranchesGlow[j].startWidth = 10; //v3.4.9
													LineBranchesGlow[j].endWidth = 5; //v3.4.9
												LineBranchesGlow[j].SetPosition(0,Branch_L1_starts[j]);
											}

											for(int i = 0;i< Vertex_count;i=i+1){
												Current_end_pos1.x = UnityEngine.Random.Range(Current_line_pos1.x - Deviation*1+Randomizer.x, Current_line_pos1.x + Deviation*2+Randomizer.x);
												Current_end_pos1.y = Current_line_pos1.y +Randomizer.y/(UnityEngine.Random.Range(1,2));
												Current_end_pos1.z = UnityEngine.Random.Range(Current_line_pos1.z - Deviation*1+Randomizer.z, Current_line_pos1.z + Deviation*2+Randomizer.z);


												LineBranches[j].SetPosition(i,Current_end_pos1);
												if(line_glow_on){
													LineBranchesGlow[j].SetPosition(i,Current_end_pos1);
												}
												Current_line_pos1=Current_end_pos1;

												if((i/15)-Mathf.RoundToInt(i/15) == 0 & i > Vertex_count/3){
													if(Branch_L2_starts.Count < max_branches/3){			//GRAB Level 1 branches, using 1 third of availble
														Branch_L2_starts.Add(Current_line_pos1);
													}
												}
											}
										}
										//v2.2 - Branches L2
										int L1_counter = Branch_L1_starts.Count;
										for(int j=0;j<Branch_L2_starts.Count;j++){
											Vector3 Current_line_pos1 = Branch_L2_starts[j];
											Vector3 Current_end_pos1 = Vector3.zero;
											//LineBranches[j+L1_counter].SetVertexCount(Vertex_count);
												LineBranches[j+L1_counter].positionCount = Vertex_count; //v3.4.9

											//LineBranches[j+L1_counter].SetWidth(3,1);
												LineBranches[j+L1_counter].startWidth = 3; //v3.4.9
												LineBranches[j+L1_counter].endWidth = 1; //v3.4.9
											LineBranches[j+L1_counter].SetPosition(0,Branch_L2_starts[j]);
											//LineBranches[j+L1_counter].SetColors(Color.white,new Color(0,0,0,0));
												LineBranches[j+L1_counter].startColor = Color.white; //v3.4.9
												LineBranches[j+L1_counter].endColor = new Color(0,0,0,0); //v3.4.9

											if(line_glow_on){
												//LineBranchesGlow[j+L1_counter].SetVertexCount(Vertex_count);
													LineBranchesGlow[j+L1_counter].positionCount = Vertex_count; //v3.4.9

												//LineBranchesGlow[j+L1_counter].SetWidth(6,4);
													LineBranchesGlow[j+L1_counter].startWidth = 6; //v3.4.9
													LineBranchesGlow[j+L1_counter].endWidth = 4; //v3.4.9
												LineBranchesGlow[j+L1_counter].SetPosition(0,Branch_L2_starts[j]);
												//LineBranchesGlow[j+L1_counter].SetColors(Glow_color,new Color(0,0,0,0));
													LineBranchesGlow[j+L1_counter].startColor = Glow_color; //v3.4.9
													LineBranchesGlow[j+L1_counter].endColor = new Color(0,0,0,0); //v3.4.9
											}
											
											for(int i = 0;i< Vertex_count;i=i+1){
												Current_end_pos1.x = UnityEngine.Random.Range(11,12) + UnityEngine.Random.Range(Current_line_pos1.x - Deviation*1+Randomizer.x, Current_line_pos1.x + Deviation*2+Randomizer.x);
												Current_end_pos1.y = Current_line_pos1.y +Randomizer.y/(UnityEngine.Random.Range(1,2));
												Current_end_pos1.z = UnityEngine.Random.Range(Current_line_pos1.z - Deviation*1+Randomizer.z, Current_line_pos1.z + Deviation*2+Randomizer.z);
												
												
												LineBranches[j+L1_counter].SetPosition(i,Current_end_pos1);
												if(line_glow_on){
													LineBranchesGlow[j+L1_counter].SetPosition(i,Current_end_pos1);
												}
												Current_line_pos1=Current_end_pos1;
												
//												if((i/15)-Mathf.RoundToInt(i/15) == 0 & i > Vertex_count/3){
//													if(Branch_L2_starts.Count < max_branches/3){			//GRAB Level 1 branches, using 1 third of availble
//														Branch_L2_starts.Add(Current_line_pos1);
//													}
//												}
											}
										}
									}



									}
									
								}
								stop_time = Time.fixedTime;
								
								//////////////////////////
								
								if(Branches){

									///v3.0 //v3.4.6
//									if(particles != null){
//										for (int i=0; i < particles.Length; i=i+4)
//										{
//											
//											Vector3 position = Vector3.Lerp(Saved_start_pos, Target_pos, oneOverZigs * (float)i);
//											
//											particles[i].size = ParticleSize;
//											int Branch_ID = (int)(zigs/Zigs_branching_divider)+20;
//											if(i > Branch_ID){
//												
//												if(cast_branches){
//													particles[i].size = ParticleSize/2;
//													position = Vector3.Lerp(Vector3.Lerp(Saved_start_pos, Target_pos, oneOverZigs * (float)Branch_ID), Target_pos+ Branch_Offset, oneOverZigs * (float)(i-Branch_ID));
//												}else{
//													position = Vector3.zero;
//													particles[i].size =0;
//												}
//											}
//											Branch_ID = 720;
//											
//											Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
//											                             noise.Noise(timey + position.x, timey + position.y, timey + position.z),
//											                             noise.Noise(timez + position.x, timez + position.y, timez + position.z));
//											position += ((offset+(offset_bias1*i*0.01f)) * scale * ((float)i * oneOverZigs));
//											
//											particles[i].position = position;
//											particles[i].color = Color.white;
//											particles[i].energy = Particle_energy;
//											
//										}
//									}
									
								}
								
								
								if(cast_branches & 1==1){
									
									
									
									//v3.0 //v3.4.6
//									if(particles != null){
//										for (int i=1; i < particles.Length; i=i+4)
//										{								
//											Vector3 Around_target1 = Target_pos + Branch_Offset;
//											
//											if(i > particles.Length/2){//use only half particles for branch
//												//Around_target1 = Vector3.zero;
//											}
//											
//											int Branch_ID = (int)(zigs/Zigs_branching_divider)+20;//+Random.Range(-5,5);//520
//											Vector3 Parent_offset = (((offset_bias1*Branch_ID*0.01f)) * scale * ((float)Branch_ID * oneOverZigs));
//											Vector3 position = Vector3.Lerp(  Parent_offset+ Vector3.Lerp(Saved_start_pos,Target_pos, oneOverZigs * (float)Branch_ID), Around_target1, oneOverZigs * (float)i);
//											Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
//											                             noise.Noise(timey + position.x, timey + position.y, timey + position.z),
//											                             noise.Noise(timez + position.x, timez + position.y, timez + position.z));
//											position += ((offset-(offset_bias1*i*0.01f)) * scale * ((float)i * oneOverZigs));
//											
//											particles[i].position = position;
//											particles[i].color = Branch_color;
//											particles[i].energy = Particle_energy;
//											particles[i].size = ParticleSize;
//										}
//									}
									
									if(UnityEngine.Random.Range(0,2) == 1 & 1==1){

										//v3.0 //v3.4.6
//										if(particles != null){
//											for (int i=2; i < particles.Length; i=i+4)
//											{
//												Vector3 Around_target1 = Target_pos + (1.5f*Branch_Offset);
//												
//												if(i > particles.Length/2){//use only half particles for branch
//													//Around_target1 = Vector3.zero;
//												}
//												
//												int Branch_ID = (int)(zigs/Zigs_branching_divider)+20+5;//520
//												Vector3 Parent_offset = (((offset_bias1*Branch_ID*0.01f)) * scale * ((float)Branch_ID * oneOverZigs));
//												Vector3 position = Vector3.Lerp(  Parent_offset+ Vector3.Lerp(Saved_start_pos,Target_pos, oneOverZigs * (float)Branch_ID), Around_target1, oneOverZigs * (float)i);
//												Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
//												                             noise.Noise(timey + position.x, timey + position.y, timey + position.z),
//												                             noise.Noise(timez + position.x, timez + position.y, timez + position.z));
//												position += ((offset-(offset_bias1*i*0.01f)) * scale * ((float)i * oneOverZigs));
//												
//												particles[i].position = position;
//												particles[i].color = Branch_color;
//												particles[i].energy = Particle_energy;
//												particles[i].size = ParticleSize/2;
//											}
//										}
									}
								}else{//if no branching
									
									if(!Branches){
										//v3.0 //v3.4.6
//										if(particles != null){
//											for (int i=0; i < particles.Length; i++)
//											{
//												Vector3 position = Vector3.Lerp(Saved_start_pos, target.position, oneOverZigs * (float)i);
//												Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
//												                             noise.Noise(timey + position.x, timey + position.y, timey + position.z),
//												                             noise.Noise(timez + position.x, timez + position.y, timez + position.z));
//												position += (offset * scale * ((float)i * oneOverZigs));
//												
//												particles[i].position = position;
//												particles[i].color = Color.white;
//												particles[i].energy = Particle_energy;
//											}
//										}
									}
									
								}
								
								
								//v1.4
								ChainLightning_SKYMASTER next_in_Chain = target.gameObject.GetComponent(typeof(ChainLightning_SKYMASTER)) as ChainLightning_SKYMASTER;
								//Energize target
								if(next_in_Chain ==null){
									next_in_Chain = target.gameObject.GetComponentInChildren(typeof(ChainLightning_SKYMASTER)) as ChainLightning_SKYMASTER;
									
								}
								
								if(next_in_Chain !=null){
									if(!next_in_Chain.is_parent){
										
										next_in_Chain.Energized = true;
										next_in_Chain.max_depth = max_depth;
										next_in_Chain.current_depth = current_depth+1;
									}
								}

								//v3.0 //v3.4.6
//								if(particles != null){
//									GetComponent<ParticleEmitter>().particles = particles;
//								}
								//v3.4.6
								if (1==1)//(GetComponent<ParticleEmitter>().particleCount >= 2)
								{
									//if (startLight){
									//startLight.transform.position = particles[0].position;}
									
									int get_in=1;
									get_in=UnityEngine.Random.Range(1,optimize_factor);
									if (endLight){ 
										if(get_in==1 & target!=null){
											if(!SmoothLights){
												endLight.enabled=true;
												endLight.gameObject.SetActive(true);
												endLight.transform.position = target.transform.position; //particles[particles.Length - 1].position; //v3.4.5b
											}else{
												endLight.enabled=true;
												endLight.gameObject.SetActive(true);
												endLight.intensity = Mathf.Lerp(endLight.intensity,max_end_light_intensity, Time.deltaTime * LightSpeed);
												//endLight.gameObject.SetActive(true);
												endLight.transform.position = target.transform.position; //particles[particles.Length - 1].position;//v3.4.5b
											}
										}else{
											if(!SmoothLights){
												endLight.enabled=false;
											}else{
												endLight.intensity = Mathf.Lerp(endLight.intensity,0, Time.deltaTime * LightFadeSpeed * Random.Range(1,LightRipple));
											}
										}
									}
									
									if (startLight){ 
										if(get_in==1 & target!=null){
											if(!SmoothLights){
												startLight.enabled=true;
												startLight.gameObject.SetActive(true);
												startLight.transform.position = Saved_start_pos; // particles[0].position;//v3.4.5b
											}else{
												startLight.enabled=true;
												startLight.gameObject.SetActive(true);
												startLight.intensity = Mathf.Lerp(startLight.intensity,max_start_light_intensity, Time.deltaTime * LightSpeed);
												startLight.transform.position = Saved_start_pos; //particles[0].position;//v3.4.5b
											}
										}else{
											if(!SmoothLights){
												startLight.enabled=false;
											}else{
												startLight.intensity = Mathf.Lerp(startLight.intensity,0, Time.deltaTime * LightFadeSpeed * Random.Range(1,LightRipple));
											}
										}
									}
								}
//								else
//								{
//									if(!SmoothLights){
//										endLight.enabled=false;
//										startLight.enabled=false;
//									}else{
//										endLight.intensity = Mathf.Lerp(endLight.intensity,0, Time.deltaTime * LightFadeSpeed * Random.Range(1,LightRipple));
//										startLight.intensity = Mathf.Lerp(endLight.intensity,0, Time.deltaTime * LightFadeSpeed * Random.Range(1,LightRipple));
//									}
//								}
								
							}else{// if target is null

								//v3.0 //v3.4.6
//								if(particles != null){
//									for (int i=0; i < particles.Length; i++)
//									{
//										particles[i].position =Vector3.zero;// this_transform.position;//Vector3.zero; //v1.7 - disabled
//									}
//									GetComponent<ParticleEmitter>().particles = particles; //v1.7 - disabled
//									Emitter.emit = false;
//									Emitter.ClearParticles();
//								}
								
								if(line_on){
									if(Liner !=null){
										if(Time.fixedTime - stop_time > UnityEngine.Random.Range(Line_delay.x,Line_delay.y)){
											//Liner.SetWidth(0,0);
											Liner.startWidth = 0; //v3.4.9
											Liner.endWidth = 0; //v3.4.9
											//v2.2
											if(line_glow_on){
												//LinerGlow.SetWidth(0,0);
												LinerGlow.startWidth = 0; //v3.4.9
												LinerGlow.endWidth = 0; //v3.4.9
												LinerGlow.enabled = false;//v3.0
											}
											Liner.enabled = false;
											if(Branches){
												for(int i=0;i<LineBranches.Count;i++){
													//LineBranches[i].SetWidth(0,0);
													LineBranches[i].startWidth = 0; //v3.4.9
													LineBranches[i].endWidth = 0; //v3.4.9
													if(line_glow_on){
														//LineBranchesGlow[i].SetWidth(0,0);
														LineBranchesGlow[i].startWidth = 0; //v3.4.9
														LineBranchesGlow[i].endWidth = 0; //v3.4.9
														LineBranchesGlow[i].enabled = false;
													}
													LineBranches[i].enabled = false;
												}
											}
										}
									}
								}
								
							}//end target is null check
							
							//v1.7
							if(startLight & target==null & Time.fixedTime - stop_time > UnityEngine.Random.Range(Light_delay.x,Light_delay.y)){
								if(!SmoothLights){
									startLight.enabled=false; 
									startLight.gameObject.SetActive(false);
								}else{
									startLight.intensity = Mathf.Lerp(startLight.intensity,0, Time.deltaTime * LightFadeSpeed * Random.Range(1,LightRipple));
								}
							}
							
							if(endLight & target==null){
								if(!SmoothLights){
									endLight.enabled=false; 
									endLight.gameObject.SetActive(false);
								}else{
									endLight.intensity = Mathf.Lerp(endLight.intensity,0, Time.deltaTime * LightFadeSpeed * Random.Range(1,LightRipple));
								}
							}
							//if(startLight & target==null){startLight.enabled=false; startLight.gameObject.SetActive(false);}
							
						}//END check if targets > 0
					}
				}//END check if parent
				else{

					//v3.0 //v3.4.6
//					if(particles != null){
//						for (int i=0; i < particles.Length; i++)
//						{
//							particles[i].position = Vector3.zero;;//this_transform.position;//Vector3.zero;//v1.7 - disabled
//						}
//						GetComponent<ParticleEmitter>().particles = particles;//v1.7 - disabled
//						Emitter.emit = false;
//						Emitter.ClearParticles();
//					}
				}
				
			}
			
		}//END particles


		
	}
	
}