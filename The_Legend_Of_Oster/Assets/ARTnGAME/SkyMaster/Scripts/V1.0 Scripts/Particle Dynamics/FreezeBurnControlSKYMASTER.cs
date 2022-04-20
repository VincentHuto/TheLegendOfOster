using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.SKYMASTER {

	public class FreezeBurnControlSKYMASTER : MonoBehaviour {
			
	void Start () {

			if(this.GetComponent<Renderer>()!=null){

				Start_color = this.GetComponent<Renderer>().material.color; 
				
			}
	}

		private Color Start_color;
		public float freeze_ammount=0;//inject/increase from Propagation script
		public float burn_ammount=0;
		public float max_burn_ammount=25; //inject from Propagation script
		public float max_freeze_ammount=25;

		public float Thaw_speed=0;
		public float Freeze_speed=0.15f;

		public float check_time=0.2f;
		public float current_time;

	void Update () {

			if(burn_ammount < (max_burn_ammount/2)){

			}else{

				if(Time.fixedTime - current_time > check_time){
				//make black
				if(this.GetComponent<Renderer>()!=null){
 
						this.GetComponent<Renderer>().material.color = Color.Lerp(this.GetComponent<Renderer>().material.color,Color.black,Freeze_speed*Time.deltaTime); 
				}
				}else{
					current_time = Time.fixedTime;
				}
			}

			if(freeze_ammount < (max_freeze_ammount/10)){

				if(Thaw_speed>0){
					//lerp back to start color
					if(Time.fixedTime - current_time > check_time){
						//make black
						if(this.GetComponent<Renderer>()!=null){
							if(this.GetComponent<Renderer>().material.color != Start_color){
								this.GetComponent<Renderer>().material.color = Color.Lerp(this.GetComponent<Renderer>().material.color,Start_color,Freeze_speed*Time.deltaTime); 
							}
						}
					}else{
						current_time = Time.fixedTime;
					}
				}
			}else{
				if(Time.fixedTime - current_time > check_time){
				//make black
				if(this.GetComponent<Renderer>()!=null){
						this.GetComponent<Renderer>().material.color = Color.Lerp(this.GetComponent<Renderer>().material.color,Color.cyan,Freeze_speed*Time.deltaTime); 
				}
				}else{
					current_time = Time.fixedTime;
				}
			}

			if(Thaw_speed>0){
				if(freeze_ammount>0){
					freeze_ammount = freeze_ammount - Thaw_speed*Time.deltaTime;
				}
			}

	}	

}
}