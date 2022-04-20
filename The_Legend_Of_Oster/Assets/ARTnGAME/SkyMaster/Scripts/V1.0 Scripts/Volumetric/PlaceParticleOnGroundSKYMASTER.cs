using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {

[ExecuteInEditMode()]
	public class PlaceParticleOnGroundSKYMASTER : MonoBehaviour {
	
	void Start () {
		p11 = this.gameObject.GetComponent("ParticleSystem") as ParticleSystem;

			if(p11==null){
				Debug.Log ("Please attach the script to a particle system");
			}

	}
	
	public ParticleSystem p11;

	public bool make_circle=false;
	public float Circle_radius = 5f;
	public int loosen_circle=1; 
	public bool is_target=false;
	public float spread = 0f;
	
	//public Particle[] particles; //v3.4.6
	ParticleSystem.Particle[] ParticleList;

	Transform thisTransform;

	public Vector2 Grass_Up_Low_Threshold = new Vector2(1f,1f);
	
	public bool relaxed = true;
	public float Dist_Above_Terrain = 1;

	void LateUpdate () {

			if(p11 ==null){

				p11 = this.gameObject.GetComponent("ParticleSystem") as ParticleSystem;

				return;
			}

		if(1==1 & Terrain.activeTerrain != null){

				ParticleList = new ParticleSystem.Particle[p11.particleCount];
				p11.GetParticles(ParticleList);
				
				if(1==1){
					for (int i=0; i < ParticleList.Length;i++)
					{

						if(!make_circle ){
							int ARAND = Random.Range(1,ParticleList.Length);
							if(i == ARAND | !relaxed ){
								ParticleList[i].position  =  new Vector3(ParticleList[i].position.x,Terrain.activeTerrain.transform.position.y + Terrain.activeTerrain.SampleHeight(ParticleList[i].position)+Dist_Above_Terrain,ParticleList[i].position.z);
							}

							if(relaxed & this.gameObject.tag == "Grass"){
							
								float FIND_Y_MARGINED = Terrain.activeTerrain.transform.position.y + Terrain.activeTerrain.SampleHeight(ParticleList[i].position)+Dist_Above_Terrain;

								float UPPER_MARGIN = FIND_Y_MARGINED+Grass_Up_Low_Threshold.x;
								float LOWER_MARGIN = FIND_Y_MARGINED-Grass_Up_Low_Threshold.y;

								float FIND_PARTICLE_Y = ParticleList[i].position.y;

								if(ParticleList[i].position.y > UPPER_MARGIN){
									FIND_PARTICLE_Y=UPPER_MARGIN;
								}
								if(ParticleList[i].position.y < LOWER_MARGIN){
									FIND_PARTICLE_Y=LOWER_MARGIN;
								}

								ParticleList[i].position  =  new Vector3(ParticleList[i].position.x,FIND_PARTICLE_Y,ParticleList[i].position.z);
							
							}

						}
					
						int ARAND1 = Random.Range(1,loosen_circle);
						if(make_circle & ( (ARAND1 == 1 ) |    (is_target & i < (p11.particleCount/1))     )  ){

		
							float find_x = this.transform.position.x + Mathf.Sin (i)*(Circle_radius+(i*spread*0.01f));
							float find_z = this.transform.position.z + Mathf.Cos (i)*(Circle_radius+(i*spread*0.01f));

							float find_y = ParticleList[i].position.y;

							if(!is_target){
								find_y = Terrain.activeTerrain.SampleHeight(new Vector3(find_x,0,find_z))+Dist_Above_Terrain+ Terrain.activeTerrain.transform.position.y;
							}

							if(is_target){
								find_y = Terrain.activeTerrain.SampleHeight(new Vector3(find_x,0,find_z))+Dist_Above_Terrain+ Terrain.activeTerrain.transform.position.y;
							}

							ParticleList[i].position  =   new Vector3(find_x,find_y +  0,find_z) ;
	
						}

					}
				}
				p11.SetParticles(ParticleList,p11.particleCount);
			
		}
		
	}
}
}