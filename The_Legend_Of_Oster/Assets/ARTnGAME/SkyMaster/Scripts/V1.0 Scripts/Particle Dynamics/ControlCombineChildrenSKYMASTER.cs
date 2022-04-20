using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Artngame.PDM;

namespace Artngame.SKYMASTER {
/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.

Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
*/

//[AddComponentMenu("Mesh/Combine Children")]
	public class ControlCombineChildrenSKYMASTER : MonoBehaviour {
	
		public bool generateTriangleStrips = true;
		//private Vector3 hiddenPosition = new Vector3(0, -100000, 0);

	
		public bool Auto_Disable=false;

		public int skip_every_N_frame=0;
	
	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
	void Start () {

			if(Destroy_list==null){
				Destroy_list = new List<GameObject>();
			}


				Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
				//Matrix4x4 myTransform = transform.worldToLocalMatrix;
				//Hashtable materialToMesh= new Hashtable();

			//v1.7
			if(Self_dynamic_enable){
				if(Children_list!=null){
				if(filters.Length != Children_list.Count){
					//if(filters[i].gameObject != 
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
					//MeshFilter filter = (MeshFilter)filters[i];
					Renderer curRenderer  = filters[i].GetComponent<Renderer>();

				//v1.7
				if(Self_dynamic_enable){
					if(Children_list!=null){
						if(filters.Length != Children_list.Count){
							//if(filters[i].gameObject != 
							Children_list.Add (filters[i].gameObject.transform);
							Positions_list.Add(filters[i].gameObject.transform.position);

							if(Self_dynamic_check_rot){
								Rotations_list.Add(filters[i].gameObject.transform.rotation);
							}
							if(Self_dynamic_check_scale){
								Scale_list.Add(filters[i].gameObject.transform.localScale);
							}
						}
					}
				}

					if (curRenderer != null && !curRenderer.enabled ) {

						
						curRenderer.enabled = true;
					}
				}
			
	

	}

		public bool MakeActive=false;
		private List<GameObject> Destroy_list;
		int count_frames;

		//v1.7
		private List<Vector3> Positions_list;
		private List<Quaternion> Rotations_list;
		private List<Vector3> Scale_list;
		private List<Transform> Children_list;
		public bool Self_dynamic_enable=false;
		public bool Self_dynamic_check_rot=false;
		public bool Self_dynamic_check_scale=false;

		void LateUpdate(){

			if(Self_dynamic_enable){

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
				if(Children_list[i].position != Positions_list[i]){
					MakeActive=true;
					Positions_list[i] = Children_list[i].position; //save new pos
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

				int child_count  = transform.childCount;

				if(child_count != Children_list.Count){
					//if(filters[i].gameObject != 
					//Children_list.Add (filters[i].gameObject.transform);
					//Positions_list.Add(filters[i].gameObject.transform.position);
					MakeActive=true;
				}

				}else{
					Children_list = new List<Transform>();
					Positions_list = new List<Vector3>();
					if(Self_dynamic_check_rot){
						Rotations_list = new List<Quaternion>();
					}
					if(Self_dynamic_check_scale){
						Scale_list = new List<Vector3>();
					}
				}
			}


			//erase previous mesh ?
			if(MakeActive){

				//Debug.Log("INSIDE");

				if(Auto_Disable){
					MakeActive=false;
				}

				if(skip_every_N_frame>0){
					if(count_frames >= skip_every_N_frame){ 
						count_frames=0; 
						//Debug.Log ("Return"); 
						return;
					}else{
						count_frames=count_frames+1;
					}
					//return;
				}

			//activate children
				if(1==1){
					Start ();

					MeshFilter filter1  = this.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
					if(filter1!=null){
						Mesh meshD = filter1.sharedMesh;// this.gameObject.GetComponent <MeshFilter>().sharedMesh;
						//Destroy(filter1);
						DestroyImmediate(meshD,true);
						DestroyImmediate(filter1,true);
					}else{

						if(Destroy_list.Count>0){
							for(int i=0;i<Destroy_list.Count;i++){
								MeshFilter filter11  = Destroy_list[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
								if(filter11!=null){
									Mesh meshD = filter11.sharedMesh;// this.gameObject.GetComponent <MeshFilter>().sharedMesh;
									//Destroy(filter1);
									DestroyImmediate(meshD,true);
									DestroyImmediate(filter11,true);
								}
							}
							for(int i=Destroy_list.Count-1;i>=0;i--){
								DestroyImmediate(Destroy_list[i]);
								Destroy_list.RemoveAt(i);
							}
						}

					}

					Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
					Matrix4x4 myTransform = transform.worldToLocalMatrix;
					Hashtable materialToMesh= new Hashtable();
					
					for (int i=0;i<filters.Length;i++) {
						MeshFilter filter = (MeshFilter)filters[i];
						Renderer curRenderer  = filters[i].GetComponent<Renderer>();
						MeshCombineUtilitySKYMASTER.MeshInstance instance = new MeshCombineUtilitySKYMASTER.MeshInstance ();
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
							
							curRenderer.enabled = false;
						}
					}
					
					foreach (DictionaryEntry de  in materialToMesh) {
						ArrayList elements = (ArrayList)de.Value;
						MeshCombineUtilitySKYMASTER.MeshInstance[] instances = (MeshCombineUtilitySKYMASTER.MeshInstance[])elements.ToArray(typeof(MeshCombineUtilitySKYMASTER.MeshInstance));
						


							GameObject go = new GameObject("Combined mesh");
							go.transform.parent = transform;
							go.transform.localScale = Vector3.one;
							go.transform.localRotation = Quaternion.identity;
							go.transform.localPosition = Vector3.zero;
							go.AddComponent(typeof(MeshFilter));
							go.AddComponent<MeshRenderer>();
							go.GetComponent<Renderer>().material = (Material)de.Key;
							MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
						filter.mesh = MeshCombineUtilitySKYMASTER.Combine(instances, generateTriangleStrips);

							Destroy_list.Add(go);
						
					}
				}//end if use bones
				//END IF BONES
		}	



		
}

	}}