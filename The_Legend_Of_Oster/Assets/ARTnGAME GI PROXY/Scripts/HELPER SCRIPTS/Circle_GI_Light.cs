using UnityEngine;
using System.Collections;
//using Jove;  // Enable Jove namespace here

namespace Artngame.GIPROXY {

	public class Circle_GI_Light : MonoBehaviour {
		
	//Transform trans;
	//int dir  = 1;
	public float speedMult  = 2.0f;

	public bool up_down_motion  = false;
	public bool Shock_effect = false;

	public float up_down_speed  = 1f;
	public float up_down_multiply = 1f;

	public float JITTER  = 5f;

	// Use this for initialization
	void Awake () {
		//trans = transform;
	}

	public Transform sphereObject;

	void FixedUpdate()
	{
		if(sphereObject != null){
			//random speed
			var RAND_SPEEDA=speedMult;
			if(Shock_effect){
				RAND_SPEEDA=Random.Range(speedMult-1.1f,speedMult+JITTER); 
			}

			transform.RotateAround (sphereObject.position, Vector3.up, RAND_SPEEDA* 20 * Time.deltaTime);

			if(up_down_motion){

				//random speed
				var RAND_SPEED=up_down_speed;
				if(Shock_effect){
					RAND_SPEED=Random.Range(up_down_speed-0.1f,up_down_speed+JITTER/10); 
				}
				transform.position = new Vector3(transform.position.x, sphereObject.transform.position.y+up_down_multiply*Mathf.Cos(Time.fixedTime+RAND_SPEED), transform.position.z);
			}
		}
	}

}

}
