using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {
	
	public class ParticleShadowsSM : MonoBehaviour {
		
		// Use this for initialization
		void Start () {
			//if(Start_color == null){
			Start_color = p2.main.startColor.color; //v3.4.9
			//Debug.Log (Start_color);
			//}
		}
		Color Start_color ;
		
		public Transform Ref_light;
		public ParticleSystem p2;
		
		//public Particle[] particles;  //v3.4.6
		ParticleSystem.Particle[] ParticleList;

        public bool affectAlpha = false;

		public bool Debug_on = false;
		public float Shadow_factor = 0.5f;
		public float Shadow_max_dist = 1000; //max distance to cast shadows on smoke
		public Color ShadowColor = new Color(0.5f,0.5f,0.5f,0.5f);
		// Update is called once per frame

		int counter=0;

		void Update () {
			if(p2==null){
				Debug.Log ("Please insert a particle system");
				return;
			}
			
			ParticleSystem p11=p2;
			
			
			
			ParticleList = new ParticleSystem.Particle[p11.particleCount];
			p11.GetParticles(ParticleList);

            //Debug.Log("P count = "+ ParticleList.Length);
            for (int i=0; i < ParticleList.Length;i++)
			{
				if(1==1 || counter == i){
					Vector3 Disp = ParticleList[i].position - Ref_light.position;
					Ray ray = new Ray(Ref_light.position,Disp); //Camera.main.ScreenPointToRay(Input.mousePosition);
					ray = new Ray(ParticleList[i].position,-Ref_light.forward);
					
					if(Debug_on){
						Debug.DrawRay(ParticleList[i].position,-Ref_light.forward);
						Debug.DrawLine(ParticleList[i].position,Ref_light.position);
                        //Debug.Log("P count i = " + i);
                    }
					//Vector2 Origin = new Vector2(ParticleList[i].position.x, ParticleList[i].position.y);
					//Vector2 Direction = new Vector2(ParticleList[i].velocity.x, ParticleList[i].velocity.y);
					
					//RaycastHit2D hit = Physics.Raycast(ray.origin, ray.direction,1000);
					//RaycastHit2D hit = Physics2D.Raycast(Origin, Direction);
					
					RaycastHit hit = new RaycastHit();

					if( Physics.Raycast(ray.origin, ray.direction,out hit, Shadow_max_dist) ){

                        if (affectAlpha)
                        {
                            ParticleList[i].startColor = ShadowColor * Shadow_factor;//SMv3.1
                        }
                        else
                        {
                            float saveAlpha = ParticleList[i].GetCurrentColor(p11).a;
                            ParticleList[i].startColor = ShadowColor * Shadow_factor;
                            ParticleList[i].startColor = new Color(ParticleList[i].startColor.r, ParticleList[i].startColor.g, ParticleList[i].startColor.b,saveAlpha);
                        }
						//ParticleList[i].color = new Color(ParticleList[i].color.r,ParticleList[i].color.g,ParticleList[i].color.b, Start_color.a);
						//ParticleList[i].size = 0;
						if(Debug_on){
							Debug.DrawLine(ParticleList[i].position, hit.point,Color.red,5);
                            //Debug.Log("light hit obstacle");
						}
					}else{
                        if (affectAlpha)
                        {
                            ParticleList[i].startColor = Start_color;//SMv3.1
                        }
                        else
                        {
                            float saveAlpha = ParticleList[i].GetCurrentColor(p11).a;
                            ParticleList[i].startColor = Start_color;
                            ParticleList[i].startColor = new Color(ParticleList[i].startColor.r, ParticleList[i].startColor.g, ParticleList[i].startColor.b, saveAlpha);
                        }
                    }
					
					
					//RaycastHit2D hit = Physics2D.Raycast(cameraPosition, mousePosition, distance (optional));
					//			if(hit != null && hit.collider != null){
					//				//if(Disp.magnitude > hit.distance){
					//					ParticleList[i].color = Color.black;
					//				Debug.DrawLine(ParticleList[i].position, hit.point,Color.red);
					//				//}else{
					//					//ParticleList[i].color = Color.black;
					//				//}
					//			}else{
					//				ParticleList[i].color = Color.white;
					//			}
					counter++;
				}

				if(counter > ParticleList.Length){
					counter=0;
				}
			}
			
			p2.SetParticles(ParticleList,p11.particleCount);
			
		}
	}
}