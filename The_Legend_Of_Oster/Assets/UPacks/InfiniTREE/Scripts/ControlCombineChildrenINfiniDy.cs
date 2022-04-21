using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.INfiniDy {
	
	public class ControlCombineChildrenINfiniDy : MonoBehaviour {
		
		public struct Meshy {
			
			public string name;
			public Vector3[] vertices;
			public Vector3[] normals;
			public Color[] colors;
			public Vector2[] uv;
			public Vector2[] uv1;
			public Vector4[] tangents;
			public int[] triangles;
			public bool thread_ended;
		}
		
		List<Meshy> MeshList = new List<Meshy>();
		List<int> MeshIDList = new List<int>();//fill with 1 when thread registered for MeshList item is done
		
		public string Save_Name = "InfiniTree1";
		public string Save_Dir = "Assets/InfiniTREE/savedMesh/";
		
		public int Added_item_count;
		public List<Transform> Added_items;
		public List<INfiniDyForest> Added_items_handles;
		public int Max_items;
		public float Deactivate_hero_dist = 60;
		public float Deactivate_hero_offset = 5;
		public float Deactivate_hero_distCUT = 80;

		public bool DeactivateLOD = false;
		public Material LODMaterial;//material that holds the LOD sprite
		
		public bool generateTriangleStrips = true;
		public bool Auto_Disable=false;		
		public int skip_every_N_frame=0;		
		bool run_once=false;
		public bool Multithreaded=false;
		public bool MTmethod2 = false;
		// ControlCombineChildrenINfiniDy.Meshy mesh;
		
		bool mesh_initalized = false;
		public bool is_moving = false;
		public bool doing_batching = false;//signal to tree scripts to not use interactive mode
		
		void Start () {
			
			if(Hero == null){
				Hero = GameObject.FindGameObjectWithTag("Hero");
			}
			
			if(Added_items == null){
				Added_items = new List<Transform>();
			}
			if(Added_items_handles == null){
				Added_items_handles = new List<INfiniDyForest>();
			}
			
			if(!mesh_initalized){
				//mesh = new ControlCombineChildrenINfiniDy.Meshy();
				mesh_initalized = true;
			}
			
			if(Destroy_list==null){
				Destroy_list = new List<GameObject>();
			}
			if(Destroy_list_MF==null){
				Destroy_list_MF = new List<MeshFilter>();
			}
			
			if(Destroy_list64==null){
				Destroy_list64 = new List<GameObject>();
			}
			if(Destroy_list_MF64==null){
				Destroy_list_MF64 = new List<MeshFilter>();
			}
			
			Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
			//v1.7
			if(Self_dynamic_enable & !run_once){
				if(Children_list!=null){
					if(filters.Length != Children_list.Count){
						
						Children_list.Clear ();
						Positions_list.Clear();
						
						if(Self_dynamic_check_rot){
							Rotations_list.Clear();
						}
						if(Self_dynamic_check_scale){
							Scale_list.Clear();
						}						
					}
				}
			}
			
			for (int i=0;i<filters.Length;i++) {
				Renderer curRenderer  = filters[i].GetComponent<Renderer>();
				
				//v1.7
				if(Self_dynamic_enable & !run_once){
					if(Children_list!=null){
						if(filters.Length != Children_list.Count){
							Children_list.Add (filters[i].gameObject.transform);
							Positions_list.Add(filters[i].gameObject.transform.position);
							Rotations_list.Add(filters[i].gameObject.transform.rotation);
							Scale_list.Add(filters[i].gameObject.transform.localScale);
						}
					}
				}
				
				if (curRenderer != null && !curRenderer.enabled ) {	
					if(curRenderer.sharedMaterial.name.Contains("LOD")){//LOD
						
						curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false; //v1.8
						curRenderer.receiveShadows = false;
					}else{
						curRenderer.enabled = true;
					}
				}
			}				
			//run_once = true;
			previous_children_count = transform.childCount; 			
			self_dyn_enable_time = Time.fixedTime;
		}
		
		public bool MakeActive=false;
		private List<GameObject> Destroy_list;
		private List<MeshFilter> Destroy_list_MF;
		private List<GameObject> Destroy_list64;
		private List<MeshFilter> Destroy_list_MF64;
		int count_frames;
		
		//v1.7
		private List<Vector3> Positions_list;
		private List<Quaternion> Rotations_list;
		private List<Vector3> Scale_list;
		private List<Transform> Children_list;
		public bool Self_dynamic_enable=false;
		public bool Self_dynamic_check_rot=false;
		public bool Self_dynamic_check_scale=false;
		
		public bool is_active=false;
		public GameObject Hero;
		
		public float Min_dist = 0.2f; //minimal move distance to trigger mesh update
		int previous_children_count;
		public float stop_self_dyn_after = 0; //let objects settle and then stop self_dyn after x secs, if > 0
		float self_dyn_enable_time;
		public bool Decombine = false;
		public bool Decombined = false;//flag a recent decombination
		public bool Erase_tree = false;
		public List<int> Erase_Tree_IDs;
		
		int threads_started = 0;
		int threads_ended = 0;
		bool all_threads_started = false;
		public bool batching_initialized = false;
		
		List<int> Splits = new List<int>();//List to keep split count per material, for multithreading handling after threads are over
		int Material_count = 0;
		List<Material> Material_list = new List<Material>();
		
		bool Hero_far=true;
		public bool LODed = false;
		
		void LateUpdate(){

						
			Hero_far=true;
			
			if(Added_items != null){
				for(int i=0;i<Added_items.Count;i++){
					if(Added_items[i] != null){
						if(Vector3.Distance(Added_items[i].position,Hero.transform.position) < Deactivate_hero_dist){
							Hero_far = false;
						}
					}
				}
			}
			
			if(Hero_far){
				//Debug.Log ("Hero Far");
			}
			
			LODed = false;
			
			//v1.1 - Fix case where handles are not available
			bool checked_handled = false;
			
			if(Added_items_handles.Count>0){
				if(Added_items_handles[Added_items_handles.Count-1].Grow_tree_ended){
					checked_handled = true;
				}
			}
			
			if(DeactivateLOD
			   & Hero_far 
			   & !is_moving 
			   & !Decombined 
			   & Added_items.Count > 0 
			   & Destroy_list.Count > 0 //){
			   & !MakeActive 
			   & Added_items.Count == Added_item_count 
			   & checked_handled
			   //& Added_items_handles[Added_items_handles.Count-1].Grow_tree_ended
			   ){
				
				bool Hero_far2=true;
				for(int i=0;i<Added_items.Count;i++){
					if(Vector3.Distance(Added_items[i].position,Hero.transform.position) < (Deactivate_hero_dist+Deactivate_hero_offset)){
						Hero_far2 = false;
					}
				}
				
				if(Hero_far2){
					//Debug.Log ("Hero Far2");
				}
				
				if(!Hero_far2){
					
					LODed = false;
					//LOD
					//re-enable meshes in the threhold region, before system gets fully activated
					if(Destroy_list_MF.Count>0){
						for(int i=0;i<Destroy_list_MF.Count;i++){
							
							if(Destroy_list_MF[i]!=null){
								if(Destroy_list_MF[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
									Destroy_list_MF[i].gameObject.SetActive(false);
								}else{
									Destroy_list_MF[i].gameObject.SetActive(true);
								}
							}
						}
					}
					if(Destroy_list_MF64.Count>0){
						for(int i=0;i<Destroy_list_MF64.Count;i++){
							
							if(Destroy_list_MF64[i]!=null){
								if(Destroy_list_MF64[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
									Destroy_list_MF64[i].gameObject.SetActive(false);
								}else{
									Destroy_list_MF64[i].gameObject.SetActive(true);
								}
							}
						}
					}
				}else{
					//deactivate and activate LODs

					//v1.5
					bool Hero_farCUT = true;
					if(Added_items != null){
						for(int i=0;i<Added_items.Count;i++){
							if(Added_items[i] != null){
								if(Vector3.Distance(Added_items[i].position,Hero.transform.position) < Deactivate_hero_distCUT){
									Hero_farCUT = false;
								}
							}
						}
					}

					if(Hero_farCUT){
						if(Destroy_list_MF.Count>0){
							for(int i=0;i<Destroy_list_MF.Count;i++){
								
								if(Destroy_list_MF[i]!=null){
									if(Destroy_list_MF[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
										Destroy_list_MF[i].gameObject.SetActive(false);
									}else{
										Destroy_list_MF[i].gameObject.SetActive(false);
									}
								}
							}
						}
						if(Destroy_list_MF64.Count>0){
							for(int i=0;i<Destroy_list_MF64.Count;i++){
								
								if(Destroy_list_MF64[i]!=null){
									if(Destroy_list_MF64[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
										Destroy_list_MF64[i].gameObject.SetActive(false);
									}else{
										Destroy_list_MF64[i].gameObject.SetActive(false);
									}
								}
							}
						}
					}else{
						if(Destroy_list_MF.Count>0){
							for(int i=0;i<Destroy_list_MF.Count;i++){
								
								if(Destroy_list_MF[i]!=null){
									if(Destroy_list_MF[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
										Destroy_list_MF[i].gameObject.SetActive(true);
									}else{
										Destroy_list_MF[i].gameObject.SetActive(false);
									}
								}
							}
						}
						if(Destroy_list_MF64.Count>0){
							for(int i=0;i<Destroy_list_MF64.Count;i++){
								
								if(Destroy_list_MF64[i]!=null){
									if(Destroy_list_MF64[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
										Destroy_list_MF64[i].gameObject.SetActive(true);
									}else{
										Destroy_list_MF64[i].gameObject.SetActive(false);
									}
								}
							}
						}
					}
					LODed = true;
				}
				//reacivate just before threshold is passed
			}else{
				
				LODed = false;
				
				is_active=false;//start with false, if active signal it
				
				if(Self_dynamic_enable){
					
					if(stop_self_dyn_after > 0){
						if(Time.fixedTime - self_dyn_enable_time > stop_self_dyn_after){
							self_dyn_enable_time = Time.fixedTime;
							Self_dynamic_enable = false;
						}
					}				
					
					if(Children_list!=null){
						//v1.7 check if items in list are null and remove from both lists.
						for(int i=Children_list.Count-1;i>=0;i--){
							if(Children_list[i]!=null){
								if(Children_list[i].gameObject == null){
									Children_list.RemoveAt(i);
									Positions_list.RemoveAt(i);
								}
							}
						}
						//if item changed position
						for(int i=Children_list.Count-1;i>=0;i--){
							if(Children_list[i]!=null){
								
								if(Vector3.Distance(Children_list[i].position,Positions_list[i]) > Min_dist){
									MakeActive=true;
									Positions_list[i] = Children_list[i].position; //save new pos
									//Debug.Log ("ID ="+i);
									//Restore();
									//v1.6
									Multithreaded = false;
									Self_dynamic_enable = true;
								}
								if(Self_dynamic_check_rot){
									if(Rotations_list[i] != Children_list[i].rotation){
										MakeActive=true;
										Rotations_list[i] = Children_list[i].rotation; //save new rot
									}
								}
								if(Self_dynamic_check_scale){
									if(Scale_list[i] != Children_list[i].localScale){
										MakeActive=true;
										Scale_list[i] = Children_list[i].localScale; //save new scale
									}
								}
							}
						}					
						
						//if item has been added					
						int child_count  = transform.childCount; //v1.8.1 Check PREVIOUS COUNT and not children count
						
						if(child_count != previous_children_count){						
							previous_children_count = child_count;
							MakeActive=true;
						}
						
					}else{
						Children_list = new List<Transform>();
						Positions_list = new List<Vector3>();
						Rotations_list = new List<Quaternion>();
						Scale_list = new List<Vector3>();

						//v1.6
						if(!run_once & Self_dynamic_enable){
							Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
							//v1.7
							if(Self_dynamic_enable & !run_once){
								if(Children_list!=null){
									if(filters.Length != Children_list.Count){
										
										Children_list.Clear ();
										Positions_list.Clear();
										
										if(Self_dynamic_check_rot){
											Rotations_list.Clear();
										}
										if(Self_dynamic_check_scale){
											Scale_list.Clear();
										}						
									}
								}
							}
							
							for (int i=0;i<filters.Length;i++) {
								Renderer curRenderer  = filters[i].GetComponent<Renderer>();
								
								//v1.7
								if(Self_dynamic_enable & !run_once){
									if(Children_list!=null){
										if(filters.Length != Children_list.Count){
											Children_list.Add (filters[i].gameObject.transform);
											Positions_list.Add(filters[i].gameObject.transform.position);
											Rotations_list.Add(filters[i].gameObject.transform.rotation);
											Scale_list.Add(filters[i].gameObject.transform.localScale);
										}
									}
								}
								
								if (curRenderer != null && !curRenderer.enabled ) {	
									if(curRenderer.sharedMaterial.name.Contains("LOD")){//LOD
										
										curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//.castShadows = false; //v1.8
										curRenderer.receiveShadows = false;
									}else{
										curRenderer.enabled = true;
									}
								}
							}
							run_once = true;
						}

						Debug.Log ("LISTS INITIALIZED");
					}
				}			
				
				if(Erase_tree){
					Erase_tree = false;
					Restore();//de combine all
					for(int i=Erase_Tree_IDs.Count-1;i>=0;i--){
						if(Erase_Tree_IDs[i] <= Added_items.Count){
							DestroyImmediate(Added_items[Erase_Tree_IDs[i]-1].gameObject);
							Added_items.RemoveAt(Erase_Tree_IDs[i]-1);
							DestroyImmediate(Added_items_handles[Erase_Tree_IDs[i]-1].transform.gameObject);
							Added_items_handles.RemoveAt(Erase_Tree_IDs[i]-1);
						}
					}
				}
				
				if(Decombine){
					
					//Debug.Log ("INNER DECOMB");
					
					Decombine = false; //disable immediately
					Decombined = true;
					
					Start (); //reenable meshfilters
					MakeActive = false;
					is_active = false;
					Self_dynamic_enable = false;
					
					//clean up
					MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
					if(filter1!=null){
						Mesh meshD = filter1.sharedMesh;
						
						DestroyImmediate(meshD,true);
						DestroyImmediate(filter1,true);
					}else{					
						if(Destroy_list.Count>0){
							for(int i=0;i<Destroy_list.Count;i++){
								MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
								if(filter11!=null){
									Mesh meshD = filter11.sharedMesh;// this.gameObject.GetComponent <MeshFilter>().sharedMesh;
									
									DestroyImmediate(meshD,true);
									DestroyImmediate(filter11,true);
								}
							}
							for(int i=Destroy_list.Count-1;i>=0;i--){
								Destroy_list_MF.RemoveAt(i);
								DestroyImmediate(Destroy_list[i]);
								Destroy_list.RemoveAt(i);
							}
						}
						if(Destroy_list64.Count>0){
							for(int i=0;i<Destroy_list64.Count;i++){
								
								if(Destroy_list_MF64[i]!=null){
									Mesh meshD = Destroy_list_MF64[i].sharedMesh;
									
									DestroyImmediate(meshD,true);
									DestroyImmediate(Destroy_list_MF64[i],true);
								}
							}
							for(int i=Destroy_list64.Count-1;i>=0;i--){
								Destroy_list_MF64.RemoveAt(i);
								DestroyImmediate(Destroy_list64[i]);
								Destroy_list64.RemoveAt(i);
							}
						}
					}				
				}
				
				
				if(MakeActive){
					
					//LOD - reenable LOD groups to count in combine
					if(Destroy_list_MF64.Count>0){
						for(int i=0;i<Destroy_list_MF64.Count;i++){
							
							if(Destroy_list_MF64[i]!=null){
								if(Destroy_list_MF64[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
									Destroy_list_MF64[i].gameObject.SetActive(true);
								}else{
									
								}
							}
						}
					}
					if(Destroy_list_MF.Count>0){
						for(int i=0;i<Destroy_list_MF.Count;i++){
							
							if(Destroy_list_MF[i]!=null){
								if(Destroy_list_MF[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
									
									Destroy_list_MF[i].gameObject.SetActive(true);
								}else{
									
								}
							}
						}
					}
					
					
					
					is_active=true;//start with false, if active signal it
					
					if(Auto_Disable){
						if(!Multithreaded){
							MakeActive=false;
						}
					}
					
					if(skip_every_N_frame>0){
						if(count_frames >= skip_every_N_frame){ 
							count_frames=0; 
							
							return;
						}else{
							count_frames=count_frames+1;
						}
						
					}
					
					//activate children
					if(1==1){
						
						
						if((!batching_initialized & Multithreaded) | !Multithreaded){
							batching_initialized = true;											
							
							if(Multithreaded & MTmethod2){ // if 2ond method, works best if vertices > 65K, but is a bit slower
								Start (); //1. reactivate all and disabled already combined in 2.
							}
							
							if(!Multithreaded){
								Start ();
								
								MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
								if(filter1!=null){
									Mesh meshD = filter1.sharedMesh;
									
									DestroyImmediate(meshD,true);
									DestroyImmediate(filter1,true);
								}else{
									
									if(Destroy_list.Count>0){
										for(int i=0;i<Destroy_list.Count;i++){
											MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
											if(filter11!=null){
												Mesh meshD = filter11.sharedMesh;
												
												DestroyImmediate(meshD,true);
												DestroyImmediate(filter11,true);
											}
										}
										for(int i=Destroy_list.Count-1;i>=0;i--){
											Destroy_list_MF.RemoveAt(i);
											DestroyImmediate(Destroy_list[i]);
											Destroy_list.RemoveAt(i);
										}
									}						
								}
							}else if(MTmethod2){ //We cant destroy them to avoid flicker from dissapearence until threads work is over, BUT must be disabled OR WILL BE ADDED to the mix !!!!
								
								//2. disable previously combined
								
								MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
								if(filter1!=null){								
									filter1.gameObject.SetActive(false);
								}else{								
									if(Destroy_list.Count>0){
										for(int i=0;i<Destroy_list.Count;i++){
											MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
											if(filter11!=null){											
												filter11.gameObject.SetActive(false);
											}
										}									
									}						
								}
							}
							
							Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));					
							
							if(Multithreaded & MTmethod2){ //3. re-enable previously combined to avoid flicker while waiting for threads to end
								MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
								if(filter1!=null){
									filter1.gameObject.SetActive(true);
								}else{								
									if(Destroy_list.Count>0){
										for(int i=0;i<Destroy_list.Count;i++){
											MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
											if(filter11!=null){
												filter11.gameObject.SetActive(true);
											}
										}
									}						
								}
							}
							
							if(filters != null | 1==0 ){
								if(filters.Length >0 | 1==0){
									
									Matrix4x4 myTransform = transform.worldToLocalMatrix;
									Hashtable materialToMesh= new Hashtable();							
									//Debug.Log ("Filters count = "+filters.Length);
									
									int Group_start = 0;
									int Group_end = filters.Length;									
									
									for (int i=Group_start;i<Group_end;i++) {									
										
										MeshFilter filter = (MeshFilter)filters[i];
										
										if(!Multithreaded | (Multithreaded & ( (filter.sharedMesh.vertexCount <= 32000 & filter.gameObject.name != "Combined mesh64")  | MTmethod2) )){
											
											//Debug.Log (filter.gameObject.name);
											
											Renderer curRenderer  = filters[i].GetComponent<Renderer>();
											
											//LOD - check if material is LOD AND has no shadows, must be combined !!! so enable it
											// then disable it AND enable shadows to mark it, when decombine disable renderer AND disable shadows in LODs
											bool is_LOD = false;
											//if(curRenderer.sharedMaterial.name.Contains("LOD") & !curRenderer.castShadows & !curRenderer.receiveShadows ){
											if(curRenderer.sharedMaterial.name.Contains("LOD") && curRenderer.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.Off && !curRenderer.receiveShadows ){ //v1.8
												curRenderer.enabled = true;
												is_LOD = true;
											}
											
											MeshCombineUtilityINfiniDy.MeshInstance instance = new MeshCombineUtilityINfiniDy.MeshInstance ();
											instance.mesh = filter.sharedMesh;
											if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
												instance.transform = myTransform * filter.transform.localToWorldMatrix;
												
												Material[] materials = curRenderer.sharedMaterials;
												for (int m=0;m<materials.Length;m++) {
													instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
													
													ArrayList objects = (ArrayList)materialToMesh[materials[m]];
													if (objects != null) {
														objects.Add(instance);
													}
													else
													{
														objects = new ArrayList ();
														objects.Add(instance);
														materialToMesh.Add(materials[m], objects);
													}
												}
												
												if(!Multithreaded | (Multithreaded & MTmethod2)){
													curRenderer.enabled = false;
												}
												
												//LOD
												if(is_LOD){
													curRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;//.castShadows = true; //v1.8
													curRenderer.receiveShadows = true;
													curRenderer.enabled = false;
												}
												
											}
										}else{
											//Debug.Log (">64k");
										}
									}								
									
									
									
									int List_increase=0;
									//int mesh_counter=0;
									int counted_entries = 1;
									foreach (DictionaryEntry de  in materialToMesh) {
										
										Material_count++;
										Material_list.Add((Material)de.Key);
										
										ArrayList elements = (ArrayList)de.Value;
										MeshCombineUtilityINfiniDy.MeshInstance[] instances = (MeshCombineUtilityINfiniDy.MeshInstance[])elements.ToArray(typeof(MeshCombineUtilityINfiniDy.MeshInstance));
										
										List<int> Split_index = new List<int>();
										Split_index.Add(0);
										
										int vertexes_count = 0;
										for(int i=0;i<instances.Length;i++){										
											
											vertexes_count = vertexes_count + instances[i].mesh.vertexCount;// filter.sharedMesh.vertexCount;
											
											if(vertexes_count > 64000){
												vertexes_count = 0;
												Split_index.Add(i);
												Debug.Log ("Split at ="+i);
											}
										}								
										//Debug.Log ("Matrial ID ="+de.Key+" "+de.Value);
										
										Splits.Add(Split_index.Count);
										for (int j=0;j<Split_index.Count;j++) {	
											
											if(!Multithreaded){
												
												if(j < Split_index.Count-1){
													Group_start = Split_index[j];
													Group_end = Split_index[j+1]-1;
												}else{
													Group_start = Split_index[j];
													Group_end =  instances.Length;
												}
												
												MeshCombineUtilityINfiniDy.MeshInstance[] instances_Split = new MeshCombineUtilityINfiniDy.MeshInstance[Group_end-Group_start];
												for (int k=0;k<(Group_end-Group_start);k++) {
													instances_Split[k] = instances[Group_start+k-0];
												}									
												
												string name = "Combined mesh";											
												if(j < Split_index.Count-1){
													name = "Combined mesh64";
												}
												
												GameObject go = new GameObject(name);
												go.transform.parent = transform;
												go.transform.localScale = Vector3.one;
												go.transform.localRotation = Quaternion.identity;
												go.transform.localPosition = Vector3.zero;
												go.AddComponent(typeof(MeshFilter));
												go.AddComponent<MeshRenderer>();
												go.GetComponent<Renderer>().material = (Material)de.Key;
												MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
												
												filter.mesh = MeshCombineUtilityINfiniDy.Combine(instances_Split, generateTriangleStrips);
												Destroy_list.Add(go);
												Destroy_list_MF.Add(filter);
												
											}else{ //MULTITHREADED											
												
												if(j < Split_index.Count-1){
													Group_start = Split_index[j];
													Group_end = Split_index[j+1]-1;
												}else{
													Group_start = Split_index[j];
													Group_end =  instances.Length;
												}
												
												MeshCombineUtilityINfiniDy.MeshInstance[] instances_Split = new MeshCombineUtilityINfiniDy.MeshInstance[Group_end-Group_start];
												for (int k=0;k<(Group_end-Group_start);k++) {
													instances_Split[k] = instances[Group_start+k-0];
												}
												
												if(!all_threads_started){
													
													int vertexCount=0;
													int triangleCount=0;
													List<int> Combine_Mesh_vertexCount = new List<int>();
													List<Vector3[]> Combine_Mesh_vertices = new List<Vector3[]>();
													List<Vector3[]> Combine_Mesh_normals = new List<Vector3[]>();
													List<Vector4[]> Combine_Mesh_tangets= new List<Vector4[]>();
													
													List<Vector2[]> Combine_Mesh_uv= new List<Vector2[]>();
													List<Vector2[]> Combine_Mesh_uv1= new List<Vector2[]>();
													List<Color[]> Combine_Mesh_colors= new List<Color[]>();
													List<int[]> Combine_Mesh_triangles= new List<int[]>();
													List<int> Has_mesh= new List<int>();
													
													int count =0;
													foreach(MeshCombineUtilityINfiniDy.MeshInstance combine in instances_Split)											
													{
														if (combine.mesh)											
														{												
															vertexCount += combine.mesh.vertexCount;												
														}
														//Combine_Mesh_vertexCount[count] = combine.mesh.vertexCount;
														Combine_Mesh_vertexCount.Add(combine.mesh.vertexCount);
														Combine_Mesh_vertices.Add(combine.mesh.vertices);
														Combine_Mesh_normals.Add(combine.mesh.normals);
														Combine_Mesh_tangets.Add(combine.mesh.tangents);
														
														Combine_Mesh_uv.Add(combine.mesh.uv);
														Combine_Mesh_uv1.Add(combine.mesh.uv2);
														Combine_Mesh_colors.Add(combine.mesh.colors);
														Combine_Mesh_triangles.Add(combine.mesh.GetTriangles(combine.subMeshIndex));
														
														count++;
													}
													
													foreach(MeshCombineUtilityINfiniDy.MeshInstance combine in instances_Split)											
													{											
														if (combine.mesh)											
														{												
															triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
															Has_mesh.Add(1);
														}else{
															Has_mesh.Add(0);
														}											
													}
													
													ControlCombineChildrenINfiniDy.Meshy mesh = new ControlCombineChildrenINfiniDy.Meshy();
													mesh.thread_ended = false;
													MeshList.Add(mesh);
													MeshIDList.Add(0); //signal this thread is not done yet
													
													int counter_mesh = List_increase;
													LoomINfiniDy.RunAsync(()=>{
														
														MeshList[counter_mesh] = MeshCombineUtilityINfiniDy.CombineM(counter_mesh,Has_mesh,all_threads_started, instances_Split, 
														                                                             generateTriangleStrips,vertexCount,triangleCount,
														                                                             Combine_Mesh_vertexCount,Combine_Mesh_vertices,Combine_Mesh_normals,
														                                                             Combine_Mesh_tangets,Combine_Mesh_uv,Combine_Mesh_uv1,Combine_Mesh_colors,Combine_Mesh_triangles );
														
														
														
													});
													
													List_increase++;
													
													//thread_started = true;
													threads_started++;
													
													
													if(counted_entries == materialToMesh.Count & j == (Split_index.Count -1) ){											
														
														all_threads_started = true;
														//mesh_counter = 0;
														//break;
													}
													
												}									
												
											}// END MULTITHREADED ELSE
											
											//Destroy_list.Add(go);
										}
										counted_entries++;
									}//END MATRIAL FOR
									
								}}//END check filters array if exists
							
						}// END if !batching_initialized
						
						int threads_ended_real = 0;
						for(int i=0;i<MeshIDList.Count;i++){
							if(MeshIDList[i] == 0){
								
							}
							if(MeshList[i].thread_ended){
								threads_ended_real++;
							}
						}
						
						if(threads_ended_real >= threads_started & Multithreaded & all_threads_started & 1==1){
							
							if(Multithreaded & 1==1){
								//Start ();
								
								MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
								if(filter1!=null){
									Mesh meshD = filter1.sharedMesh;
									
									DestroyImmediate(meshD,true);
									DestroyImmediate(filter1,true);
									
								}else{
									
									if(Destroy_list.Count>0){
										for(int i=0;i<Destroy_list.Count;i++){
											MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
											if(filter11!=null){
												Mesh meshD = filter11.sharedMesh;// this.gameObject.GetComponent <MeshFilter>().sharedMesh;
												
												DestroyImmediate(meshD,true);
												DestroyImmediate(filter11,true);											
											}
										}
										for(int i=Destroy_list.Count-1;i>=0;i--){
											Destroy_list_MF.RemoveAt(i);
											DestroyImmediate(Destroy_list[i]);
											Destroy_list.RemoveAt(i);
										}
									}						
								}
							}
							
							//disable filters NOW 
							
							Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));	
							int Group_start = 0;
							int Group_end = filters.Length;						
							
							for (int i=Group_start;i<Group_end;i++) {
								MeshFilter filter = (MeshFilter)filters[i];
								Renderer curRenderer  = filters[i].GetComponent<Renderer>();
								
								if (curRenderer != null && curRenderer.enabled){																
									
									if(filter.gameObject.name == "Combined mesh64" & !MTmethod2){
										
									}else{
										curRenderer.enabled = false;
									}						
									
								}
							}					
							
							int mesh_counter=0;
							for(int i = 0; i<Material_list.Count; i++){
								
								for (int j=0;j<Splits[i];j++) {
									
									if(MeshList[mesh_counter].vertices !=null){
										//Debug.Log ("Vertex count = "+MeshList[mesh_counter].vertices.Length);
										//Debug.Log ("Mesh counter ="+mesh_counter+" IDs = "+MeshIDList[mesh_counter]);
									}
									
									if(MeshList[mesh_counter].vertices != null & MeshIDList[mesh_counter] != 1 ){ 
										
										MeshIDList[mesh_counter] = 1;
										
										string name = "Combined mesh";									
										if(MeshList[mesh_counter].vertices.Length > 32000){
											name = "Combined mesh64";
										}
										
										GameObject go = new GameObject(name);
										go.transform.parent = transform;
										go.transform.localScale = Vector3.one;
										go.transform.localRotation = Quaternion.identity;
										go.transform.localPosition = Vector3.zero;
										go.AddComponent(typeof(MeshFilter));
										go.AddComponent<MeshRenderer>();
										go.GetComponent<Renderer>().material = Material_list[i];//(Material)de.Key;
										MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
										
										if(MeshList[mesh_counter].vertices.Length >0){
											filter.mesh.name = MeshList[mesh_counter].name;			
											filter.mesh.vertices = MeshList[mesh_counter].vertices;			
											filter.mesh.normals = MeshList[mesh_counter].normals;			
											filter.mesh.colors = MeshList[mesh_counter].colors;			
											filter.mesh.uv = MeshList[mesh_counter].uv;			
											filter.mesh.uv2 = MeshList[mesh_counter].uv1;			
											filter.mesh.tangents = MeshList[mesh_counter].tangents;			
											filter.mesh.triangles = MeshList[mesh_counter].triangles;	
											//thread_started = false;
											
											threads_ended = threads_ended+1;
										}
										
										if(MeshList[mesh_counter].vertices.Length <= 32000){
											Destroy_list.Add(go);
											Destroy_list_MF.Add(filter);
										}else{
											Destroy_list64.Add(go);
											Destroy_list_MF64.Add(filter);
										}
									}
									mesh_counter++;								
								}							
							}						
							
							Splits.Clear();
							Splits = new List<int>();
							Material_count=0;
							Material_list.Clear();
							Material_list = new List<Material>(); 						
							
							batching_initialized = false;
							
							threads_ended = 0;
							threads_started = 0;
							MeshList.Clear();
							MeshList = new List<Meshy>();
							
							MeshIDList.Clear();
							MeshIDList= new List<int>();
							//MeshList.
							all_threads_started = false;
							mesh_counter = 0;
							if(Auto_Disable){MakeActive = false;}				
							
						}			
						
						
					}//end if use bones
					//END IF BONES
					
					//LOD - reenable LOD groups to count in combine
					if(Destroy_list_MF64.Count>0){
						for(int i=0;i<Destroy_list_MF64.Count;i++){
							
							if(Destroy_list_MF64[i]!=null){
								if(Destroy_list_MF64[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
									Destroy_list_MF64[i].gameObject.SetActive(false);
								}else{
									
								}
							}
						}
					}
					if(Destroy_list_MF.Count>0){
						for(int i=0;i<Destroy_list_MF.Count;i++){
							
							if(Destroy_list_MF[i]!=null){
								if(Destroy_list_MF[i].gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("LOD")){
									Destroy_list_MF[i].gameObject.SetActive(false);
								}else{
									
								}
							}
						}
					}
					
				}//END MAKEACTIVE
				
				
			}//END hero check
		}//END UPDATE
		
		public void Restore(){
			Decombine = false; //disable immediately
			Decombined = true;
			
			//Debug.Log ("OUTER DECOMB");
			
			Start (); //reenable meshfilters
			MakeActive = false;
			is_active = false;
			Self_dynamic_enable = false;
			
			//clean up
			MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
			if(filter1!=null){
				Mesh meshD = filter1.sharedMesh;
				
				DestroyImmediate(meshD,true);
				DestroyImmediate(filter1,true);
			}else{					
				if(Destroy_list.Count>0){
					for(int i=0;i<Destroy_list.Count;i++){
						MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
						if(filter11!=null){
							Mesh meshD = filter11.sharedMesh;
							
							DestroyImmediate(meshD,true);
							DestroyImmediate(filter11,true);
						}
					}
					for(int i=Destroy_list.Count-1;i>=0;i--){
						Destroy_list_MF.RemoveAt(i);
						DestroyImmediate(Destroy_list[i]);
						Destroy_list.RemoveAt(i);
					}
				}
				if(Destroy_list64.Count>0){
					for(int i=0;i<Destroy_list64.Count;i++){
						
						if(Destroy_list_MF64[i]!=null){
							Mesh meshD = Destroy_list_MF64[i].sharedMesh;
							
							DestroyImmediate(meshD,true);
							DestroyImmediate(Destroy_list_MF64[i],true);
						}
					}
					for(int i=Destroy_list64.Count-1;i>=0;i--){
						Destroy_list_MF64.RemoveAt(i);
						DestroyImmediate(Destroy_list64[i]);
						Destroy_list64.RemoveAt(i);
					}
				}
			}				
		}
		
	}
}