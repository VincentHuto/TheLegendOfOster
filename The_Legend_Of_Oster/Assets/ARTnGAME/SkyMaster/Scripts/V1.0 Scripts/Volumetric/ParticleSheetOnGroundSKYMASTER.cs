using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace Artngame.SKYMASTER {

[ExecuteInEditMode()]
	public class ParticleSheetOnGroundSKYMASTER : MonoBehaviour {

	void Start () {
		


			//if(Application.isPlaying)
			{
			nbb=this.gameObject.GetComponent("ParticleSystem") as ParticleSystem;



			if(nbb!=null){
				nbb.Clear ();
				nbb.Emit(particle_count);
				aaa = new ParticleSystem.Particle[nbb.particleCount];
			}

			start_pos=this.transform.position;
			Cash_transform = transform;

		
			KTiles_X=Tiles_X;
				KTiles_Y=Tiles_Y;

			got_positions=false;
			}

			Main_cam_transform = Camera.main.transform;
			if(Application.isPlaying){
				particle_count_init = particle_count;
			}
	}

	Transform Main_cam_transform;

	void OnEnable(){

			nbb=this.gameObject.GetComponent("ParticleSystem") as ParticleSystem;

			if(nbb==null){
				Debug.Log ("Please attach the script to a particle system");
			}
			
			if(nbb!=null){
				nbb.Clear ();
				nbb.Emit(particle_count);
				aaa = new ParticleSystem.Particle[nbb.particleCount];
			}
			
			start_pos=this.transform.position;
			Cash_transform = transform;

			got_positions=false;

			KTiles_X=Tiles_X;
			KTiles_Y=Tiles_Y;
	}

	public Vector3 start_pos;
	public int particle_count = 100;
	Transform Cash_transform;
		int particle_count_init;

	private bool got_positions=false;

	private Vector3[] positions;
	int[] tile;

	private ParticleSystem.Particle[] aaa;
	public ParticleSystem nbb;
	
	public bool letloose=false;
	private bool let_loose=false;
	private int place_start_pos;

	public bool Gravity_Mode=false;

		public int Tiles_X=4;
		public int Tiles_Y=4;

		private int KTiles_X=4;
		private int KTiles_Y=4;

		public float Y_offset=0;

		public bool conform_to_terrain=false;
		public float Grav_speed = 0.05f;
		public float ext_rot = 90;

		public bool Use_formation = false;
		public bool Use_mesh = false;
		public bool Use_explicit = false;
		public Vector3 Explicit_Axis = new Vector3(0,0,0);
		public bool keep_alive =false;

		public bool  auto_rot_mesh=false;
		public bool use_cut_off=false;
		public float cut_off_height = 20;//keep on terrain, if terrain is below this height

	void Update () {

			if(nbb ==null){
				
				nbb = this.gameObject.GetComponent("ParticleSystem") as ParticleSystem;				
				return;
			}

			#if UNITY_EDITOR
			if(Camera.current!=null){
				Main_cam_transform = Camera.current.transform;
			}
			#else
			Main_cam_transform = Camera.main.transform;
			#endif

			if(aaa.Length != particle_count | KTiles_X!=Tiles_X | KTiles_Y!=Tiles_Y){
				//Start ();
				if(nbb!=null){
					nbb.Clear ();
					nbb.Emit(particle_count);
					aaa = new ParticleSystem.Particle[nbb.particleCount];
				}				
				//start_pos=this.transform.position;
				//Cash_transform = transform;				
				
				KTiles_X=Tiles_X;
				KTiles_Y=Tiles_Y;				
				got_positions=false;
			}

			ParticleSystem.MainModule MainMod = nbb.main; //v3.4.9
			if( MainMod.maxParticles < particle_count_init & Application.isPlaying){
				particle_count = MainMod.maxParticles;
			}else if( MainMod.maxParticles > particle_count_init & Application.isPlaying){
				particle_count = particle_count_init;
			}

		let_loose = letloose;
		if(!Application.isPlaying ){
			
			if(positions == null)
			{
				positions = new Vector3[particle_count];
				got_positions = false;
			}else{
				if(positions.Length>0){
					if(positions[0] == Vector3.zero){
						got_positions = false;
					}
				}
			}
			let_loose = false;

			aaa = new ParticleSystem.Particle[nbb.particleCount];
		}
			if(!let_loose){
			nbb.Clear();
			}

		nbb.Emit(particle_count);

		int tileCount = Tiles_X*Tiles_Y-1;  //15;
	
		nbb.GetParticles(aaa);

		if(!got_positions ){
				positions = new Vector3[aaa.Length];
				tile = new int[aaa.Length];
			got_positions = true;
			
			for(int i=0;i<aaa.Length;i++){
				positions[i] = aaa[i].position;
				tile[i] = Random.Range(0,15);
			}
		}

		//CACHE camera outside loop
		Vector3 CAM_UP = Camera.main.transform.up;
		Vector3 CAM_RIGHT = Camera.main.transform.right;

		int count_3s = 0;

		for(int i=0;i<aaa.Length;i++){
		  if(i < tile.Length){

					if(keep_alive){
			aaa[i].remainingLifetime = tileCount + 1 - tile[i];
			aaa[i].startLifetime = tileCount;
					}

			if(!let_loose){
				aaa[i].position = positions[i]; 
			}

			float find_x =aaa[i].position.x;
			float find_z =aaa[i].position.z;
			float Dist_Above_Terrain = Y_offset;

			Vector3 DIST_vector = Cash_transform.position-start_pos;

				float find_y = Cash_transform.position.y;//v1.2.5

				if(Terrain.activeTerrain != null){
					find_y=Terrain.activeTerrain.SampleHeight(new Vector3(find_x,0,find_z))+Dist_Above_Terrain+ Terrain.activeTerrain.transform.position.y;
				}
		
				Vector3 FINAL_POS = new Vector3(find_x,find_y,find_z)+new Vector3(DIST_vector.x,0,DIST_vector.z);

			if(!let_loose | (place_start_pos<1)){
				aaa[i].position= FINAL_POS;
			}
					//Gravity
					if(let_loose & Gravity_Mode){
						
						aaa[i].position = Vector3.Slerp(aaa[i].position, new Vector3(aaa[i].position.x,find_y,aaa[i].position.z),Grav_speed*Time.deltaTime);

					}

				if(conform_to_terrain){

						if(use_cut_off){
							if(find_y > cut_off_height){

							}else
							{
								aaa[i].position = new Vector3(aaa[i].position.x,find_y,aaa[i].position.z);
							}
						}else{
							aaa[i].position = new Vector3(aaa[i].position.x,find_y,aaa[i].position.z);
						}

					aaa[i].angularVelocity =0;

					if(Use_mesh){

						aaa[i].axisOfRotation = CAM_UP;
						aaa[i].rotation = Vector3.Angle(Vector3.right,CAM_RIGHT);

						if(Use_formation){
								//Random.seed = 3; //v3.4.6
								if(count_3s==0){
									aaa[i].axisOfRotation = new Vector3(1+i,1,1);
									count_3s++; 
								}else if (count_3s==1){
									aaa[i].axisOfRotation = new Vector3(1,1+i,1);
									count_3s++; 
								}else if (count_3s==2){
									aaa[i].axisOfRotation = new Vector3(1,1,1+i);
									count_3s=0; 
								}
							aaa[i].rotation =1+i + ext_rot;
						}

							if(Use_explicit){
								aaa[i].axisOfRotation = Explicit_Axis;
								aaa[i].rotation = ext_rot;
							}

							if(auto_rot_mesh){
								Vector3 FIXED_NORMAL = -(Main_cam_transform.position - aaa[i].position).normalized;

								float FIX_for_Z = 90;			
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
								
								aaa[i].rotation = ACOS+FIX_for_Z;
								aaa[i].axisOfRotation =  Vector3.Cross( new Vector3(0.00000001f,0.00000001f,1), FIXED_NORMAL);

							}
					}

					aaa[i].velocity=Vector3.zero;
				}

			if(!let_loose){
				aaa[i].angularVelocity =0;
				aaa[i].rotation =0;
				aaa[i].velocity=Vector3.zero;
			}			
		  }
		}

		if(place_start_pos <1){
			place_start_pos = place_start_pos+1;
		}

		nbb.SetParticles(aaa,aaa.Length);
	}
}
}