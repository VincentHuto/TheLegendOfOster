using UnityEngine;
using System.Collections;
//using Artngame.PDM;

namespace Artngame.SKYMASTER {
[ExecuteInEditMode]
public class Circle_Around_ParticleSKYMASTER : MonoBehaviour
{
		private Transform trans;

		public float speedMult = 2.0f;
		
		public bool up_down_motion  = false;
		public bool Shock_effect = false;
		
		public float up_down_speed = 1f;
		public float up_down_multiply = 1f;
		
		public float JITTER = 5f;	

		public Transform sphereObject;
	
	void Start ()
	{
			trans = transform;
	}

	void Update ()
	{

	}

		void FixedUpdate()
		{
			if(sphereObject != null){
				//random speed
				float RAND_SPEEDA=speedMult;
				if(Shock_effect){
					
					RAND_SPEEDA=Random.Range(speedMult-1.1f,speedMult+JITTER); 
				}
				
				trans.RotateAround (sphereObject.position, Vector3.up, RAND_SPEEDA* 20 * Time.deltaTime);
				
				if(up_down_motion){
					
					//random speed
					float RAND_SPEED=up_down_speed;
					if(Shock_effect){
						RAND_SPEED=Random.Range(up_down_speed-0.1f,up_down_speed+JITTER/10); 
					}
					trans.position= new Vector3(trans.position.x, sphereObject.transform.position.y+up_down_multiply*Mathf.Cos(Time.fixedTime+RAND_SPEED), trans.position.z);
				}
			}
		}
	
	

}

}