using UnityEngine;
using System.Collections;
//using Jove;  // Enable Jove namespace here

namespace Artngame.GIPROXY {

	public class Control_Jove_light : MonoBehaviour {


	// Use this for initialization
	void Start () {

			//Enable the below if you have Jove
			/*
			this.gameObject.AddComponent(typeof(JoveLight));

			//grab Jove light
			This_Jove_light = this.gameObject.GetComponent(typeof(JoveLight)) as JoveLight;

			This_Jove_light._type = JoveLightType.Point;
			This_Jove_light._range = this.light.range;
			This_Jove_light._radius = 0; // i dont know what this is exactly, assuming it is the physical size of the point light, need this to be zero so the light source is not visible
			This_Jove_light._color = this.light.color;
			This_Jove_lightt._intensity = this.light.intensity;


			*/
			this.GetComponent<Light>().enabled=false;


	}
	
	//Enable the below if you have Jove
	//JoveLight This_Jove_light;

	// Update is called once per frame
	void Update (){

			//Enable the below if you have Jove
			/*
			This_Jove_light._range = this.light.range;
			This_Jove_light._radius = 0; // i dont know what this is exactly, assuming it is the physical size of the point light, need this to be zero so the light source is not visible
			This_Jove_light._color = this.light.color;
			This_Jove_light._intensity = this.light.intensity * 5000;
			*/
	}

}

}
