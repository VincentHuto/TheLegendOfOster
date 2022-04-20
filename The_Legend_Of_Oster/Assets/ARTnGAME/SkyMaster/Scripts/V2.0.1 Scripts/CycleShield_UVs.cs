using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {
	
	public class CycleShield_UVs : MonoBehaviour {
		
		void Start () {
			randomizer = Random.Range(0,random_factor);
		}
		
		Vector2 Dist = new Vector2 (0f, 0f);
		public Vector2 Speed = new Vector2 (0.04f , 0.04f);
		
		public bool randomize = false;
		public float random_factor = 5;
		float randomizer;
		void LateUpdate () {
			
			Vector2 Randomizer = new Vector2(randomizer,randomizer);
			
			//Dist = Dist+ (Speed + Randomizer) * Time.deltaTime ;
			Dist = Dist+ (new Vector2(Speed.x*Randomizer.x,Speed.y*Randomizer.y )) * Time.deltaTime ;
			GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", Dist);
			
		}
	}
}