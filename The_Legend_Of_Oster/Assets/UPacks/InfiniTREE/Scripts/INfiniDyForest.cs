using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Artngame.INfiniDy {
	
	[ExecuteInEditMode]
	public class INfiniDyForest : MonoBehaviour {
		
		
		//v1.6 CHOP

		public float Max_Health = 100;
		public bool Chop = false;
		public float max_fall_ammount = 80;

		
		public bool no_shadows_on_dynamic = false;
		bool removed_shadows = false;
		public int Rm_shad_above_lvl = 5;
		public bool no_leaf_shadows = false;
		public bool Rm_receive = true;
		public bool Rm_cast = true;
		
		//
		public bool Range_leaf_scale=false;
		public Vector2 Leaf_scale_range = new Vector2(0.5f,0.8f);//leaf scaling randomization and control
		//
		float fall_timer;
		public float fall_max_time=4;
		public float fall_speed = 1;
		//Vector3 direction_randomize;
		public bool Rot_toward_normal = false;
		public float Rot_speed = 5f;
		public Vector3 direction_normal = new Vector3(0,1,0);//rotate towards normal, insert normal on instantiation
		public float max_rot_time=0.5f;
		
		public GameObject Hero;
		public float Interaction_thres = 15;
		public float Interaction_offset = 5;
		public bool Enable_LOD=false;
		public bool Grow_in_Editor=false;
		bool Has_Run_once = false;//check for editor mode
		public bool InstaGrowLeaves = false;
		public bool Leaves_local_space=true;//parent leaves to brances
		
		public bool is_grass=false;//grass system activation
		
		//[HideInInspector]

		INfiniDyTree infiniDyTree;
		public float StartRadi = 0.1f;
		public float StartLength = 1;
		public float Length_scale=1;
		public float Extend_scale=1.2f;
		public Vector2 Min_max_spread = new Vector2(45,45);
		
		public bool Spread_Z_separate=false;
		public Vector2 Min_max_spread_Z = new Vector2(45,45);
		public bool Spread_Y_separate=false;
		public Vector2 Min_max_spread_Y = new Vector2(45,45);
		
		public Vector2 Min_Max_Branching = new Vector2(1,4);
		
		//v1.2 per level max
		public List<int> Max_branches_per_level;
		
		public Vector2 Min_Max_Radi = new Vector2(0.2f,0.3f);
		public float lower_length_by = 0.8f;
		public float lower_radi_by = 0.7f;	
		
		public bool Grow_tree = false;

		public float Start_tree_scale = 0.5f;
		public float End_scale = 1;
		//public float Random_scale_bound=0;//end scale randomly around 1
		public float Grow_tree_speed = 0.1f;
		
		Vector3 prev_root_pos;
		Vector3 prev_root_rot;
		Vector3 prev_root_scale;
		
		public float Update_every = 0.02f;
		float last_update_time;
		float start_time;
		
		//new
		//public Color Branch_color;
		public List<INfiniDyTree> Trees;
		//public bool Animated=false;
		Vector3 Tree_pos ;
		
		
		public string BarkPrefab;
		public GameObject BarkPrefabOBJ;//v1.5
		//[HideInInspector]
		public List<string> LeafPrefabs;
		//[HideInInspector]
		public List<string> BranchPrefabs;

		//[HideInInspector]
		public List<GameObject> LeafPrefabsOBJ;//v1.5
		//[HideInInspector]
		public List<GameObject> BranchPrefabsOBJ;//v1.5
		//	public bool Alernate_Leaves=false; // get more leaf types and alternate them - //v1.2 removed
		//	public bool Brance_prefab_per_level=false; - //v1.2 removed
		public List<int> BranchID_per_level;
		
		public int Grow_leaves_slow_factor=10;
		public float Grow_speed=0.5f;
		public float Growth_start_div = 2;
		public float Growth_angle_div = 2;
		public float Leaf_start_div = 2;
		public bool Smooth_leaf_growth = false;
		public Vector2 Leaf_level_min_max = new Vector2(4,8);
		public int Leaves_per_branch =10;
		public bool Use_height=false;//add per height properties
		public Vector2 Leaf_height_min_max = new Vector2(0,18);
		
		//v1.2 
		public float Leaf_dist_factor = 0.29f; // control leaf distance
		
		public bool CreateForest = true;
		//public int Tree_count = 2;
		
		public bool Interactive_tree=false;//put in lower count holders
		bool Tree_status_previous;
		int Max_interact_holder_items = 2;
		int Max_trees_per_group = 12;//for non interactive trees
		public float Max_tree_dist = 15;
		
		void Create_new_pool(string name, int Max_interact_holder_items){
			
			GameObject Instance = new GameObject();			
			Instance.transform.localPosition = Vector3.zero;
			Instance.transform.localEulerAngles= Vector3.zero;
			Instance.name = name;
			Instance.tag = name;
			Forest_holder = Instance;
			
			//find one with empty slots, add tree to slots number
			Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
			if(Combiner == null){
				Forest_holder.AddComponent(typeof(ControlCombineChildrenINfiniDy));
				Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
			}
			if(Combiner != null){
				Combiner.Auto_Disable=true;
				Combiner.MakeActive = false;
				Combiner.Multithreaded = Multithreaded;
				
				Combiner.Max_items = Max_interact_holder_items;
				Combiner.Added_item_count++;
				
				if(Enable_LOD){
					Combiner.DeactivateLOD = true;
				}
				
				if(Combiner.Added_items == null){
					Combiner.Added_items = new List<Transform>();
				}
				Combiner.Added_items.Add(root_tree.transform);
				
				if(Combiner.Added_items_handles == null){
					Combiner.Added_items_handles = new List<INfiniDyForest>();
				}
				Combiner.Added_items_handles.Add(this);
				
			}
			
		}
		
		void Start () {
			if(Application.isPlaying){
				StartP ();
			}
		}
		
		void StartP () {
			
			
			
			Health=Max_Health;
			
			//	if((Application.isPlaying & !Grow_in_Editor) | (!Application.isPlaying & Grow_in_Editor & !Has_Run_once & Registered_Brances.Count < 1)){
			
			//v1.2 - remove the run_once check here, in case start does not run fully in editor, so update can re-run until it is created properly
			if((Application.isPlaying & !Grow_in_Editor) | (!Application.isPlaying & Grow_in_Editor  & Registered_Brances.Count < 1)){
				
				//Has_Run_once = true; //v1.2 move to end
				
				Hero = GameObject.FindGameObjectWithTag("Hero");
				
				if(Trees == null){
					Trees = new List<INfiniDyTree>();
				}
				
				last_update_time = Time.fixedTime;
				start_time = Time.fixedTime;
				
				infiniDyTree = new INfiniDyTree();
				infiniDyTree.is_root = true;
				Tree_pos = this.gameObject.transform.position;
				infiniDyTree.INfiniDyForestC = this;
				infiniDyTree.generateTree(StartRadi,StartLength,Color.blue,Tree_pos,null);
				infiniDyTree.INfiniDyForestOBJ = this.gameObject;
				infiniDyTree.Level = 1;
				
				//check if forest holder has been defined by scene item, if not search for tag "INfiniDyForest" and if not found create object with it.
				if(Forest_holder == null){
					
					if(Interactive_tree){
						GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
						if(Forest_holder1 == null | Forest_holder1.Length < 1){
							Create_new_pool("INfiniDyForestInter", Max_interact_holder_items);
						}else{
							//find one with empty slots, add tree to slots number
							bool found = false;
							for(int i=0;i<Forest_holder1.Length;i++){
								Combiner = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
								
								//check if far away from other trees
								bool Enter = true;
								for(int k = 0;k<Combiner.Added_items.Count;k++){
									if(Vector3.Distance(root_tree.transform.position,Combiner.Added_items[k].position) > Max_tree_dist){
										Enter = false;
									}
								}
								
								if(Enter & Combiner.Added_item_count < Combiner.Max_items &  Combiner.Max_items == Max_interact_holder_items & !Combiner.LODed){
									Forest_holder = Forest_holder1[i];
									Combiner.Added_item_count++;
									
									if(Enable_LOD){
										Combiner.DeactivateLOD = true;
									}
									
									if(Combiner.Added_items == null){
										Combiner.Added_items = new List<Transform>();
									}
									Combiner.Added_items.Add(root_tree.transform);
									
									if(Combiner.Added_items_handles == null){
										Combiner.Added_items_handles = new List<INfiniDyForest>();
									}
									Combiner.Added_items_handles.Add(this);
									
									found=true;
									break;
								}
							}
							if(!found){
								Create_new_pool("INfiniDyForestInter", Max_interact_holder_items);
							}
						}
					}else{
						GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForest");
						if(Forest_holder1 == null | Forest_holder1.Length < 1){
							Create_new_pool("INfiniDyForest", Max_trees_per_group); //Debug.Log ("OOO");
						}else{
							//find one with empty slots, add tree to slots number
							bool found = false;
							for(int i=0;i<Forest_holder1.Length;i++){
								Combiner = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
								
								//check if far away from other trees
								bool Enter = true;
								for(int k = 0;k<Combiner.Added_items.Count;k++){
									if(Vector3.Distance(root_tree.transform.position,Combiner.Added_items[k].position) > Max_tree_dist){
										Enter = false;
									}
								}
								
								if(Enter & Combiner.Added_item_count < Combiner.Max_items &  Combiner.Max_items == Max_trees_per_group & !Combiner.LODed){
									Forest_holder = Forest_holder1[i];
									Combiner.Added_item_count++;
									
									if(Enable_LOD){
										Combiner.DeactivateLOD = true;
									}
									
									if(Combiner.Added_items == null){
										Combiner.Added_items = new List<Transform>();
									}
									Combiner.Added_items.Add(root_tree.transform);
									
									if(Combiner.Added_items_handles == null){
										Combiner.Added_items_handles = new List<INfiniDyForest>();
									}
									Combiner.Added_items_handles.Add(this);
									
									found=true;
									break;
								}
							}
							if(!found){
								Create_new_pool("INfiniDyForest", Max_trees_per_group);
							}
						}
					}//END ELSE
					
				}
				
				prev_root_pos = this.transform.position;
				prev_root_rot = this.transform.eulerAngles;
				prev_root_scale = this.transform.localScale;
				
				Tree_status_previous = Interactive_tree;//initial status saved to check against later
				
				if(Grow_tree){
					root_tree.transform.localScale = Start_tree_scale*Vector3.one;			
					
				}
				
				if(InstaGrowLeaves){
					for (int i = 0; i<Registered_Brances.Count;i++){
						
						if(Registered_Brances[i] != null){
							if(Grow_level == Branch_levels[i] | 1==1){
								if(Registered_Leaves.Count != Leaves.Count){
									
									//int Diff = Registered_Leaves.Count - Leaves.Count;						
									
									for (int j = 0; j<Registered_Leaves.Count;j++){
										if(Leaf_belongs_to_branch[j] == i){
											
											int Index = 0;
											GameObject Leaf = null;// = new GameObject();
											//Leaf.name = "leaf";

											//if(Alernate_Leaves ){ //v1.2 removed bool option, grab by list count
											if(LeafPrefabs.Count > 0){
												if(LeafPrefabs.Count > 1){
													Index = Random.Range(0,LeafPrefabs.Count);
												}
											}else{
												if(LeafPrefabsOBJ.Count > 1){
													Index = Random.Range(0,LeafPrefabsOBJ.Count);
												}
											}											

											if(LeafPrefabs.Count > 0){
												Leaf =(GameObject)Object.Instantiate(Resources.Load (LeafPrefabs[Index]));
												Leaf.name = "Leaf";
											}else{
												Leaf =(GameObject)Object.Instantiate(LeafPrefabsOBJ[Index]);
												Leaf.name = "Leaf";
											}

											//v1.2
											Leaf.transform.position = Registered_Leaves[j];//Registered_Leaves[Leaves.Count];
											Leaf.transform.rotation = Registered_Leaves_Rot[j];//Registered_Leaves_Rot[Leaves.Count];
											
											if(!Leaves_local_space){
												if(!is_grass){
													Leaf.transform.parent = Registered_Brances[i];
													Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
												}
												Leaf.transform.parent = Leaf_pool.transform;
											}else{
												Leaf.transform.parent = Registered_Brances[i];
												if(!is_grass){
													Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
												}
											}
											
											
											//Debug.Log ("AAA");
											//v1.5
											if(Range_leaf_scale){
												Vector3 Scale = Leaf.transform.localScale;
												float Randomizer = Random.Range(Leaf_scale_range.x,Leaf_scale_range.y);
												Leaves_scales.Add(Scale * Randomizer);
											}else{
												Leaves_scales.Add(Leaf.transform.localScale);
											}
											
											Leaf.transform.localScale = Leaf.transform.localScale / Leaf_start_div;
											Leaves.Add(Leaf.transform);								
											
											if(Random.Range(1,Grow_leaves_slow_factor)==2){
												//break;
											}
										}
										if(Smooth_leaf_growth){
											if(j < Leaves.Count){
												if(Leaves[j] != null){
													if(Leaves[j].transform.localScale.x < Leaves_scales[j].x){
														Leaves[j].transform.localScale = Leaves[j].transform.localScale + 0.1f*Grow_speed*Time.deltaTime*new Vector3(1,1,1); 
													}
												}
											}
										}
									}
								}
								Branch_grew[i] = 1;
							}}}
				}
				
				if(Grow_in_Editor){
					//register all growth over flags here, prepare combiner ? and parent tree
					if(root_tree.transform.localScale.x < End_scale){
						root_tree.transform.localScale = End_scale*Vector3.one;
					}else{
						root_tree.transform.localScale = End_scale*Vector3.one;// MAKE IT EXACTLY same, so self dynamic baching does not kick in !!!
					}
					
					root_tree.transform.parent = Forest_holder.transform;
					
					Combiner.MakeActive  =  true;
					
					Grow_tree_ended = true;
					
					growth_over = true;
				}
				
				Has_Run_once = true; //v1.2 moved to end
			}//END if app is playing check
			
			if(Grow_in_Editor){
				
				Grow_tree_ended = true;
				
				growth_over = true;
			}
		}
		
		public bool Grow_angles = false; 
		
		[HideInInspector]
		public List<int> Branch_levels;
		[HideInInspector]
		public List<Vector3> scaleVectors;
		
		[HideInInspector]
		public List<Transform> Registered_Brances;
		[HideInInspector]
		public List<Vector3> Start_Scales;
		[HideInInspector]
		public List<Vector3> Registered_Leaves; //register leaves from each Branch and instantiate them in update here, then rotate them 
		[HideInInspector]
		public List<Quaternion> Registered_Leaves_Rot;
		[HideInInspector]
		public List<int> Leaf_belongs_to_branch;
		public GameObject Leaf_pool; //parent them here
		[HideInInspector]
		public List<Transform> Leaves; //keep instantiated leaves transofrms here
		[HideInInspector]
		public List<Vector3> Leaves_scales;
		
		int Branches_done;
		 
		public int Height_levels = 1;
		public float Height_separation = 1;
		public float Height_reduce = 0.05f;
		public float Level1Decline = 0;
		public bool Reduce_angle_with_height = false;
		
		public bool Multithreaded = false;
		public Vector3 Height_offset = new Vector3(0,0,0);
		public bool Decouple_from_bark = false;
		
		void Update () {
			
			//if((Grow_in_Editor & !Application.isPlaying & !Has_Run_once)){
			//v1.2
			if((Grow_in_Editor & !Application.isPlaying & (!Has_Run_once | (Has_Run_once & root_tree == null)) )){
				//Start ();
				
				//reset dividers, full grow at start
				Leaf_start_div = 1;
				Growth_angle_div = 1;
				Growth_start_div = 1;
				Grow_tree = false;
				Smooth_leaf_growth = false;
				InstaGrowLeaves = true;
				
				StartP (); //v1.2
				
				//Debug.Log (Combiner.gameObject.name);
			}
			
			if(Application.isPlaying){
				
				if(Time.fixedTime-last_update_time > Update_every)
				{
					
					last_update_time = Time.fixedTime;
					
					bool All_Branches_done = true;
					for (int i = 0; i < Branch_grew.Count; i++){
						if(Branch_grew[i] ==0){
							All_Branches_done = false;
						}
					}
					
					if(!growth_over){
						
						for (int i = 0; i<Registered_Brances.Count;i++){
							if(Registered_Brances[i] != null){
								if(Grow_level == Branch_levels[i]){
									if(Registered_Brances[i].localScale.x < scaleVectors[i].x*Extend_scale 
									   | Registered_Brances[i].localScale.y < scaleVectors[i].y*Extend_scale 
									   |Registered_Brances[i].localScale.z < scaleVectors[i].z*Extend_scale ){
										
										if(Grow_level < 0 | 1==0){
											Registered_Brances[i].localScale = new Vector3(Registered_Brances[i].localScale.x+0.01f, Registered_Brances[i].localScale.y+0.01f, Registered_Brances[i].localScale.z+0.01f);
										}else{
											
											Registered_Brances[i].localScale = Vector3.Lerp(Registered_Brances[i].localScale, scaleVectors[i]*Extend_scale*2, Grow_speed*Time.deltaTime);
											
										}
									}else{
										bool leaves_grown=true;
										if(Registered_Leaves.Count != Leaves.Count){
											
											//int Diff = Registered_Leaves.Count - Leaves.Count;									
											
											for (int j = 0; j<Registered_Leaves.Count;j++){
												if(Leaf_belongs_to_branch[j] == i){
													
//													int Index = 0;
//													//if(Alernate_Leaves ){ //v1.2 removed bool option, grab by list count
//													if(LeafPrefabs.Count > 1){
//														Index = Random.Range(0,LeafPrefabs.Count);
//													}
//													
//													GameObject Leaf =(GameObject)Object.Instantiate(Resources.Load (LeafPrefabs[Index]));

													//v1.5
													int Index = 0;
													GameObject Leaf = null;// = new GameObject();
													//Leaf.name = "Leaf";
													
													//if(Alernate_Leaves ){ //v1.2 removed bool option, grab by list count
													if(LeafPrefabs.Count > 0){
														if(LeafPrefabs.Count > 1){
															Index = Random.Range(0,LeafPrefabs.Count);
														}
													}else{
														if(LeafPrefabsOBJ.Count > 1){
															Index = Random.Range(0,LeafPrefabsOBJ.Count);
														}
													}											
													
													if(LeafPrefabs.Count > 0){
														Leaf =(GameObject)Object.Instantiate(Resources.Load (LeafPrefabs[Index]));
														Leaf.name = "Leaf";
													}else{
														Leaf =(GameObject)Object.Instantiate(LeafPrefabsOBJ[Index]);
														Leaf.name = "Leaf";
													}
													
													//v1.2
													Leaf.transform.position = Registered_Leaves[j];
													Leaf.transform.rotation = Registered_Leaves_Rot[j];
													
													if(!Leaves_local_space){
														if(!is_grass){
															Leaf.transform.parent = Registered_Brances[i];
															Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
														}
														Leaf.transform.parent = Leaf_pool.transform;
													}else{
														Leaf.transform.parent = Registered_Brances[i];
														if(!is_grass){
															Leaf.transform.localPosition = new Vector3(0,Mathf.Abs(Leaf.transform.localPosition.y),0);
														}
													}
													
													
													
													//v1.5
													if(Range_leaf_scale){
														Vector3 Scale = Leaf.transform.localScale;
														float Randomizer = Random.Range(Leaf_scale_range.x,Leaf_scale_range.y);
														Leaves_scales.Add(Scale * Randomizer);
													}else{
														Leaves_scales.Add(Leaf.transform.localScale);
													}
													//Leaves_scales.Add(Leaf.transform.localScale);
													
													Leaf.transform.localScale = Leaf.transform.localScale / Leaf_start_div;
													Leaves.Add(Leaf.transform);
													
													if(Random.Range(1,Grow_leaves_slow_factor)==2){
														//break;
													}
												}
												
											}
										}
										//v1.2
										for (int j = 0; j<Registered_Leaves.Count;j++){
											if(Leaf_belongs_to_branch[j] == i){
												if(Smooth_leaf_growth){
													if(j < Leaves.Count){
														if(Leaves[j] != null){
															if(Leaves[j].transform.localScale.x < Leaves_scales[j].x
															   |Leaves[j].transform.localScale.y < Leaves_scales[j].y
															   |Leaves[j].transform.localScale.z < Leaves_scales[j].z
															   ){
																//Leaves[j].transform.localScale = Leaves[j].transform.localScale + 110.1f*Grow_speed*Time.deltaTime*new Vector3(1,1,1); 
																Leaves[j].transform.localScale = Vector3.Lerp(Leaves[j].transform.localScale,Leaves_scales[j]*2, 5*Grow_speed*Time.deltaTime);
																All_Branches_done = false;
																leaves_grown = false;
															}
														}
													}
												}
											}}
										if(leaves_grown){
											Branch_grew[i] = 1;
										}
									}
								}
							}
						}
						
						
						if(Grow_level>5){
							Grow_level=0;			
						}else{
							Grow_level=Grow_level+1;
						}
						
					}
					//See if leaves and branches have stopped growing
					
					if(Grow_tree & !Grow_tree_ended){
						
						if(root_tree.transform.localScale.x < End_scale){
							root_tree.transform.localScale = root_tree.transform.localScale + Time.deltaTime*Vector3.one*Grow_tree_speed;
						}else{
							root_tree.transform.localScale = End_scale*Vector3.one;// MAKE IT EXACTLY same, so self dynamic baching does not kick in !!!
						}
					}
					
					//Debug.Log ("OO = "+Leaves.Count + " AA = " + Registered_Leaves.Count + " done = "+All_Branches_done);
					
					if(Leaves.Count >= Registered_Leaves.Count & All_Branches_done & (!Grow_tree | (Grow_tree & root_tree.transform.localScale.x >= End_scale)) ){
						
						if(!Combiner.doing_batching & !batching_ended){
							Combiner.doing_batching = true;//signal other trees to stop dynamic mode						
						}
						
						Grow_tree_ended = true;
						
						
						
						//v1.2 CHOP
						if(!passed_chop){
							passed_chop = true;						
							
							//foreach(ParticleSystem ParticlesA in ParticleHolder.GetComponentsInChildren<ParticleSystem>(true))
							foreach(TreeChopCollider collider_script in root_tree.GetComponentsInChildren<TreeChopCollider>(true))
							{
								collider_script.TreeHandler = this;
							}
							
						}
						
						if(!CreateForest){
							if(!growth_over){
								if(Combiner == null){							
									root_tree.AddComponent(typeof(ControlCombineChildrenINfiniDy));
									Combiner = root_tree.GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
									Combiner.Auto_Disable=true;
									Combiner.MakeActive = true;
								}
							}
						}else{
							if(!growth_over){
								//copy tree, randomize, parent to forest pool, combine there
								root_tree.transform.parent = Forest_holder.transform;
								
								Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
								if(Combiner == null){
									Forest_holder.AddComponent(typeof(ControlCombineChildrenINfiniDy));
									Combiner = Forest_holder.GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
								}
								if(Combiner != null){
									
									Combiner.Auto_Disable=true;								
									
									Combiner.MakeActive = true;
									
									Combiner.Multithreaded = Multithreaded;
								}
							}
						}
						
						//Start wind
						if(Enable_wind){
							Leaf_mat.SetVector("_Scale",new Vector4(1-Wind_strength*Mathf.Cos (Time.fixedTime*Wind_freq),1-Wind_strength*Mathf.Sin (Time.fixedTime*Wind_freq),1-Wind_strength*Mathf.Cos (Time.fixedTime*Wind_freq),1));
						}else{
							Leaf_mat.SetVector("_Scale",new Vector4(1,1,1,1));
						}
						growth_over = true;
						
						
						//
						if(Combiner != null){
							Combiner.Auto_Disable=true;
							
							//bool rebatch = false;
							
							if(batching_ended & !Combiner.doing_batching & !Combiner.LODed){
								if( Vector3.Distance(root_tree.transform.position, prev_root_pos) > 0.25f & !Combiner.batching_initialized){
									//rebatch = true;
									prev_root_pos = root_tree.transform.position;
									
								} 
								
								if( Vector3.Distance(root_tree.transform.eulerAngles, prev_root_rot) > 0.25f & !Combiner.batching_initialized){
									//rebatch = true;
									prev_root_rot = root_tree.transform.eulerAngles;							
								} 
								if( Vector3.Distance(root_tree.transform.localScale, prev_root_scale) > 0.25f & !Combiner.batching_initialized){
									//rebatch = true;
									prev_root_scale = root_tree.transform.localScale;
								}
							}else{
								prev_root_pos = root_tree.transform.position;
								prev_root_rot = root_tree.transform.eulerAngles;
								prev_root_scale = root_tree.transform.localScale;
								//rebatch = false;
							}
							
							Combiner.Multithreaded = Multithreaded;
							
							/////check if tree status changed, decombine container, parent tree to other type container (or make one if not exist) and parent there (decombine too if exists)
							if(Tree_status_previous != Interactive_tree & !Combiner.doing_batching  & !Combiner.LODed){
								
								Tree_status_previous = Interactive_tree;
								
								if(!Interactive_tree){
									
									Combiner.Restore();
									Combiner.Decombine = false;
									Combiner.Decombined = false;
									Combiner.MakeActive = true;
									
									GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForest");
									if(Forest_holder1 == null | Forest_holder1.Length < 1){
										Create_new_pool("INfiniDyForest", Max_trees_per_group);
										root_tree.transform.parent = Forest_holder.transform;
										Combiner.MakeActive = true;Combiner.batching_initialized = false;
									}else{
										//find one with empty slots, add tree to slots number
										bool found = false;
										for(int i=0;i<Forest_holder1.Length;i++){
											Combiner = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
											
											//check if far away from other trees
											bool Enter = true;
											for(int k = 0;k<Combiner.Added_items.Count;k++){
												if(Vector3.Distance(root_tree.transform.position,Combiner.Added_items[k].position) > Max_tree_dist){
													Enter = false;
												}
											}
											
											if(Enter & Combiner.Added_item_count < Combiner.Max_items &  Combiner.Max_items == Max_trees_per_group & !Combiner.LODed){
												Forest_holder = Forest_holder1[i];
												root_tree.transform.parent = Forest_holder.transform;
												Combiner.Added_item_count++;
												
												if(Enable_LOD){
													Combiner.DeactivateLOD = true;
												}
												
												if(Combiner.Added_items == null){
													Combiner.Added_items = new List<Transform>();
												}
												Combiner.Added_items.Add(root_tree.transform);
												
												if(Combiner.Added_items_handles == null){
													Combiner.Added_items_handles = new List<INfiniDyForest>();
												}
												Combiner.Added_items_handles.Add(this);
												
												Combiner.Decombine = false;
												Combiner.Decombined = false;
												Combiner.MakeActive = true;
												Combiner.batching_initialized = false;
												found=true;
												break;
											}
										}
										if(!found){
											Create_new_pool("INfiniDyForest", Max_trees_per_group);
											root_tree.transform.parent = Forest_holder.transform;
											Combiner.MakeActive = true;Combiner.batching_initialized = false;
										}
									}
								}else{
									
								}
							}
							
						}//END check if combiner
						
						
						//tree rot
						//rotate tree towards normal
						if(Rot_toward_normal & growth_over & !Combiner.doing_batching  & !Combiner.LODed){
							
							bool found = false;//did not find decombined flag
							GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
							if(Forest_holder1 == null | Forest_holder1.Length < 1){
								
							}else{
								//find one with empty slots, add tree to slots number								
								for(int i=0;i<Forest_holder1.Length;i++){
									ControlCombineChildrenINfiniDy Combiner1 = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
									if(Combiner1.Decombined & Combiner1 != Combiner){										
										found=true; // if it finds even one, dont do it										
									}
								}							
							}						
							
							//check within the same combiner group as this tree, if one of the trees is close enough
							bool Enter = false;
							for(int k = 0;k<Combiner.Added_items.Count;k++){
								//if(Vector3.Distance(Hero.transform.position,Combiner.Added_items[k].position) < Interaction_thres){
								Enter = true;
								//}
							}
							
							if(Enter & !found){
								
								is_moving = true;
								Combiner.is_moving = is_moving;
								//start tree rotation
								if(!Combiner.Decombined){
									Combiner.Restore();
								}
								//do rotation
								
								root_tree.transform.rotation = Quaternion.Lerp(root_tree.transform.rotation, 
								                                               Quaternion.Euler(direction_normal.x,direction_normal.y,direction_normal.z),Rot_speed * Time.deltaTime); 
								
								//if(Vector3.Distance(root_tree.transform.rotation == Quaternion.Euler(direction_normal.x,direction_normal.y,direction_normal.z)){
								if(Time.fixedTime - start_time > max_rot_time){
									Rot_toward_normal = false;
									
									Restore_shadows();
									
									is_moving = false;
									Combiner.is_moving = is_moving;
									
									if(Combiner.Decombined){
										Combiner.Decombined = false;
										Combiner.MakeActive=true;
									}
									
									start_time = Time.fixedTime; //reload, if needs real time turn in game
								}
								//}
								
								//								float margin = 0.05f;
								//								if(!is_grass){
								//									if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < (Interaction_thres - Interaction_offset)){
								//										if(Mathf.Abs(0.1f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
								//											root_tree.transform.Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
								//										}else{
								//											root_tree.transform.Rotate(Vector3.up+new Vector3(0.3f,0,0),0.1f*Mathf.Cos(0.7f*Time.fixedTime));
								//										}
								//									}
								//								}
								//								
								//								if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < (Interaction_thres - Interaction_offset)){
								//									if(Mathf.Abs(0.23f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
								//										for(int i =0;i<Registered_Brances.Count;i++){
								//											if(Branch_levels[i] <2){
								//												Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
								//											}
								//										}
								//									}else{
								//										for(int i =0;i<Registered_Brances.Count;i++){
								//											if(Branch_levels[i] <2){
								//												Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.23f*Mathf.Cos(0.7f*Time.fixedTime));
								//											}
								//										}
								//									}
								//								}
								//								
								//								if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < Interaction_thres){
								//									if(Mathf.Abs(0.1f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
								//										for(int i =0;i<Leaves.Count;i++){
								//											Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
								//										}
								//									}else{	
								//										for(int i =0;i<Leaves.Count;i++){
								//											Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.1f*Mathf.Cos(0.7f*Time.fixedTime));
								//										}
								//									}
								//								}	
								
								Remove_shadows();
								
							}else{
								
								Restore_shadows();
								
								is_moving = false;
								Combiner.is_moving = is_moving;
								
								if(Combiner.Decombined){
									Combiner.Decombined = false;
									Combiner.MakeActive=true;
								}
							}
							
							
							//root_tree.transform.rotation
							//root_tree.transform.Rotate(Vector3.right+new Vector3(0.3f,Random.Range(0,0.1f),0),fall_speed * fall_ammount * Time.deltaTime);
						}
						
						
						//PUSH TREES sample code
						if(Interactive_tree & !Combiner.doing_batching  & !Combiner.LODed & 1==1){
							
							
							//v1.2 CHOP
							if(!Chop){
								bool found = false;//did not find decombined flag
								GameObject[] Forest_holder1 = GameObject.FindGameObjectsWithTag("INfiniDyForestInter");
								if(Forest_holder1 == null | Forest_holder1.Length < 1){
									
								}else{
									//find one with empty slots, add tree to slots number
									
									for(int i=0;i<Forest_holder1.Length;i++){
										ControlCombineChildrenINfiniDy Combiner1 = Forest_holder1[i].GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
										if(Combiner1.Decombined & Combiner1 != Combiner){
											
											found=true; // if it finds even one, dont do it
											
										}
									}							
								}						
								
								//check within the same combiner group as this tree, if one of the trees is close enough
								bool Enter = false;
								for(int k = 0;k<Combiner.Added_items.Count;k++){
									if(Vector3.Distance(Hero.transform.position,Combiner.Added_items[k].position) < Interaction_thres){
										Enter = true;
									}
								}
								
								if(Enter & !found){
									
									is_moving = true;
									Combiner.is_moving = is_moving;
									//start tree rotation
									if(!Combiner.Decombined){
										Combiner.Restore();
									}
									//do rotation
									float margin = 0.05f;
									if(!is_grass){
										if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < (Interaction_thres - Interaction_offset)){
											if(Mathf.Abs(0.1f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
												root_tree.transform.Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
											}else{
												root_tree.transform.Rotate(Vector3.up+new Vector3(0.3f,0,0),0.1f*Mathf.Cos(0.7f*Time.fixedTime));
											}
										}
									}
									
									if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < (Interaction_thres - Interaction_offset)){
										if(Mathf.Abs(0.23f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
											for(int i =0;i<Registered_Brances.Count;i++){
												if(Branch_levels[i] <2){
													Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
												}
											}
										}else{
											for(int i =0;i<Registered_Brances.Count;i++){
												if(Branch_levels[i] <2){
													Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.23f*Mathf.Cos(0.7f*Time.fixedTime));
												}
											}
										}
									}
									
									if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < Interaction_thres){
										if(Mathf.Abs(0.1f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
											for(int i =0;i<Leaves.Count;i++){
												Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
											}
										}else{	
											for(int i =0;i<Leaves.Count;i++){
												Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.1f*Mathf.Cos(0.7f*Time.fixedTime));
											}
										}
									}	
									
									Remove_shadows();
									
								}else{
									
									Restore_shadows();
									
									is_moving = false;
									Combiner.is_moving = is_moving;
									
									if(Combiner.Decombined){
										Combiner.Decombined = false;
										Combiner.MakeActive=true;
									}
								}
							}//END if not CHOP
							else{////v1.2 CHOP
								
								if(Health <=(8*Max_Health/10) & !start_falling){
									
									start_falling = true;
									fall_ammount = 0;
									
									//
									fall_timer = Time.fixedTime;
									//direction_randomize = Vector3.right+new Vector3(Random.Range(-5.3f,10.1f),Random.Range(0,0.1f),Random.Range(-5.3f,10.1f));
								}
								
								if(start_falling & !fall_ended){
									
									//rotate tree
									is_moving = true;
									Combiner.is_moving = is_moving;
									//start tree rotation
									if(!Combiner.Decombined){
										Combiner.Restore();
									}
									//do rotation
									float margin = 0.05f;
									
									if(root_tree.transform.eulerAngles.x < 89){
										fall_ammount = fall_ammount+1.5f+(0.01f)*Mathf.Cos(0.7f*Time.fixedTime);
									}
									
									if(!is_grass){
										
										if(root_tree.transform.eulerAngles.x < 85){
											root_tree.transform.position = root_tree.transform.position + new Vector3(0,0.9f* Time.deltaTime,0);
											root_tree.transform.Rotate(Vector3.right+new Vector3(0.3f,Random.Range(0,0.1f),0),fall_speed * fall_ammount * Time.deltaTime);
										}
									}
									
									if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < (Interaction_thres - Interaction_offset)){
										if(Mathf.Abs(0.23f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
											for(int i =0;i<Registered_Brances.Count;i++){
												if(Branch_levels[i] <2){
													Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
												}
											}
										}else{
											for(int i =0;i<Registered_Brances.Count;i++){
												if(Branch_levels[i] <2){
													Registered_Brances[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.23f*Mathf.Cos(0.7f*Time.fixedTime));
												}
											}
										}
									}
									
									if(Vector3.Distance( Hero.transform.position, root_tree.transform.position) < Interaction_thres){
										if(Mathf.Abs(0.1f*Mathf.Cos(0.7f*Time.fixedTime))<margin){
											for(int i =0;i<Leaves.Count;i++){
												Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),margin);
											}
										}else{	
											for(int i =0;i<Leaves.Count;i++){
												Leaves[i].Rotate(Vector3.up+new Vector3(0.3f,0,0),0.1f*Mathf.Cos(0.7f*Time.fixedTime));
											}
										}
									}
									
									Remove_shadows();
									
									if(fall_ammount > max_fall_ammount | (Time.fixedTime - fall_timer) > fall_max_time){
										fall_ended = true;
									}
								}
								
								if(fall_ended){
									is_moving = false;
									Combiner.is_moving = is_moving;
									
									if(Combiner.Decombined){
										Combiner.Decombined = false;
										Combiner.MakeActive=true;
									}
									
									//v1.2
									Restore_shadows();//END shadow removal
								}
								
								
								
							}//END IF CHOP
						}else{
							is_moving = false;
							Combiner.is_moving = is_moving;
							if(Combiner.Decombined){
								
							}
						}
						
						if(batching_ended){
							Combiner.doing_batching = false;//signal other trees to stop dynamic mode						
						}
						
						batching_ended = true; // use this when returns here, to enable dynamic trees, NOT BEFORE since will break batching !!!
					}// END if growh ended
					
				}//END TIME CHECK
				
			}//END if app is playing
		}// END UPDATE
		
		void Remove_shadows(){
			
			//v1.2
			if(!removed_shadows & no_shadows_on_dynamic){
				removed_shadows = true;
				//remove shadows
				if(Rm_shad_above_lvl < 0){
					Component[] filters  = root_tree.GetComponentsInChildren(typeof(MeshFilter));
					for (int i=0;i<filters.Length;i++) {
						Renderer curRenderer  = filters[i].GetComponent<Renderer>();
						if (curRenderer != null && curRenderer.enabled ) {	
							if(curRenderer.sharedMaterial.name.Contains("LOD")){//LOD
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}else{
								//curRenderer.enabled = true;
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}else{
					for(int i =0;i<Registered_Brances.Count;i++){
						if(Branch_levels[i] >= Rm_shad_above_lvl){
							//if(Rm_shad_per_lvl[Branch_levels[i]] == 1){
							Component[] filters  = Registered_Brances[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
							//}
						}
					}
					if(no_leaf_shadows){
						for(int i =0;i<Leaves.Count;i++){
							Component[] filters  = Leaves[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}
			}
			
		}
		
		void Restore_shadows(){
			
			//v1.2
			if(removed_shadows & no_shadows_on_dynamic){
				removed_shadows = false;
				//remove shadows
				if(Rm_shad_above_lvl < 0){
					Component[] filters  = root_tree.GetComponentsInChildren(typeof(MeshFilter));
					for (int i=0;i<filters.Length;i++) {
						Renderer curRenderer  = filters[i].GetComponent<Renderer>();
						if (curRenderer != null && curRenderer.enabled ) {	
							if(curRenderer.sharedMaterial.name.Contains("LOD")){//LOD													
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}else{
								//curRenderer.enabled = true;
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}else{
					for(int i =0;i<Registered_Brances.Count;i++){
						if(Branch_levels[i] >= Rm_shad_above_lvl){
							//if(Rm_shad_per_lvl[Branch_levels[i]] == 1){
							Component[] filters  = Registered_Brances[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
							//}
						}
					}
					if(no_leaf_shadows){
						for(int i =0;i<Leaves.Count;i++){
							Component[] filters  = Leaves[i].GetComponentsInChildren(typeof(MeshFilter));
							for (int j=0;j<filters.Length;j++) {
								Renderer curRenderer  = filters[j].GetComponent<Renderer>();
								if(Rm_cast){
									curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //v1.8 //.castShadows = false;
								}
								if(Rm_receive){
									curRenderer.receiveShadows = false;
								}
							}
						}
					}
				}
			}
		}
		

		public GameObject Forest_holder;
		public Material Leaf_mat;
		public bool Enable_wind=false;
		public float Wind_freq=1;
		public float Wind_strength = 0.01f;
		public 	bool batching_ended=false;
		public bool is_moving = false;

		//v1.6 DEBUG

		public float Health;

		public bool start_falling = false;
		public float fall_ammount = 0;

		public bool fall_ended = false;
		public bool passed_chop = false;

		public GameObject root_tree;
		public bool Grow_tree_ended = false;

		public List<int> Branch_grew;
		public int Grow_level = 0;
		public bool growth_over = false;
		public ControlCombineChildrenINfiniDy Combiner;
	}
}