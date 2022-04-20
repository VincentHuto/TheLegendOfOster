using UnityEngine;
using System.Collections;

namespace Artngame.GIPROXY {

public class ControlGIPROXY : MonoBehaviour {

	LightCollisionsPDM GI_PROXY_script;
		public GameObject LightPOOL;

	// Use this for initialization
	void Start () {

			GI_PROXY_script = this.gameObject.GetComponent(typeof(LightCollisionsPDM)) as LightCollisionsPDM;
	
	}
	
		void OnGUI(){

			if(GI_PROXY_script!=null){

				string GI_ON = "Toggle GI on";
				if(GI_PROXY_script.enabled){GI_ON = "Toggle GI off - "+GI_PROXY_script.Lights.Count+" Lights";}

				if(GUI.Button(new Rect(200,10,200,80),GI_ON)){

					if(GI_PROXY_script.enabled){
						GI_PROXY_script.enabled=false;
						LightPOOL.SetActive(false);
					}else{
						GI_PROXY_script.enabled=true;
						LightPOOL.SetActive(true);
					}

				}
				//Degrade-appear 0.135 (0.035), 0.135 (0.025), 0.32(0.099) intensity
				string HIGH="high";

				if(GI_PROXY_script.Degrade_speed == 0.135f){
					HIGH="low";
				}

				if(GUI.Button(new Rect(200,180,200,80),"Toggle GI power "+HIGH)){

					for(int i=0;i<GI_PROXY_script.Lights.Count;i++){

						Destroy(GI_PROXY_script.Lights[i].gameObject);

					}

					GI_PROXY_script.Lights.Clear();

					if(GI_PROXY_script.Degrade_speed == 0.035f){
						GI_PROXY_script.Degrade_speed = 0.005f;//= 0.135f;
					}else{
						GI_PROXY_script.Degrade_speed = 0.035f;
					}

					if(GI_PROXY_script.Appear_speed == 0.025f){
						GI_PROXY_script.Appear_speed = 0.009f;//= 0.135f;
					}else{
						GI_PROXY_script.Appear_speed = 0.025f;
					}

					if(GI_PROXY_script.Bounce_Intensity == 0.099f){
						GI_PROXY_script.Bounce_Intensity = 0.22f;// = 0.22f;
					}else{
						GI_PROXY_script.Bounce_Intensity = 0.099f;
					}

					if(GI_PROXY_script.Color_change_speed == 16){
						GI_PROXY_script.Color_change_speed = 7;
					}else{
						GI_PROXY_script.Color_change_speed = 16;
					}
					
				}

			}

		}

	// Update is called once per frame
	void Update () {
	
	}
}

}
