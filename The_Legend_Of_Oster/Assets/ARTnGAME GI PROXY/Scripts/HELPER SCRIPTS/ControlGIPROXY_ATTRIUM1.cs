using UnityEngine;
using System.Collections;

namespace Artngame.GIPROXY {

	public class ControlGIPROXY_ATTRIUM1 : MonoBehaviour {

	LightCollisionsPDM GI_PROXY_script;

		LightCollisionsPDM GI_PROXY_POINT_script;
		LightCollisionsPDM GI_PROXY_SPOT_script;

		public GameObject LightPOOL;

		public GameObject MainAtrium;
		public GameObject WhiteAtrium;

		float intensity_current;
		float ambience_current;
		float rotation_current;
		float rotation_current_X;
		float position_current_Z;

		public Transform LookAtTarget;
		public Transform PointLight;
		public Transform PointLight_POOL;

		Vector3 Initial_point_light_pos;

		public Transform SpotLight;
		public Transform SpotLight_POOL;

		public Transform GUI_texts;
		public Transform Procedural_sphere;
		public Transform Intro_letters;
		public Transform Intro_letters_GI;
		public Transform Intro_letters_GI_POOL;

		public Transform Tentacles;

		public float intro_time=5;
		float current_time=2;
		bool intro_ended=false;

		bool enable_GUI=true;

	// Use this for initialization
	void Start () {

			GI_PROXY_script = this.gameObject.GetComponent(typeof(LightCollisionsPDM)) as LightCollisionsPDM;

			if(PointLight!=null){

				PointLight.gameObject.SetActive(true);

				GI_PROXY_POINT_script = PointLight.gameObject.GetComponentInChildren(typeof(LightCollisionsPDM)) as LightCollisionsPDM;

				if(GI_PROXY_POINT_script!=null){

					PointLight.gameObject.SetActive(false);
				}
			}

			if(SpotLight!=null){
				
				SpotLight.gameObject.SetActive(true);
				
				GI_PROXY_SPOT_script = SpotLight.gameObject.GetComponentInChildren(typeof(LightCollisionsPDM)) as LightCollisionsPDM;

				if(GI_PROXY_SPOT_script!=null){

					SpotLight.gameObject.SetActive(false);
				}
			}

			Initial_point_light_pos = PointLight.transform.position;
	
	}
	
		void OnGUI(){

			if(intro_ended){
				string GUI_ON = "off";
				if(!enable_GUI){GUI_ON = "on";}
				
				if(GUI.Button(new Rect(0,100,110,40),"Toggle GUI "+GUI_ON)){

					if(!enable_GUI){
						enable_GUI=true;
						GUI_texts.gameObject.SetActive(true);
					}else{
						enable_GUI=false;
						GUI_texts.gameObject.SetActive(false);
					}

				}
			}


			if(intro_ended & enable_GUI){
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


						PointLight.gameObject.SetActive(false);
						PointLight_POOL.gameObject.SetActive(false);
						PointLight.gameObject.transform.position=Initial_point_light_pos;						
						this.GetComponent<Light>().enabled=true;

						//DISABLE SPOT LIGHT
						SpotLight.gameObject.SetActive(false);
						SpotLight_POOL.gameObject.SetActive(false);
						

					}

				}

				////// SPOT light GI on/off 

				string GI_ON_SPOT = "Toggle Spot GI on";
				if(GI_PROXY_SPOT_script.enabled){GI_ON_SPOT = "Toggle Spot GI off - "+GI_PROXY_SPOT_script.Lights.Count+" Lights";}
				
				if(GUI.Button(new Rect(0,50,200,40),GI_ON_SPOT)){
					
					if(GI_PROXY_SPOT_script.enabled){
						GI_PROXY_SPOT_script.enabled=false;
						SpotLight_POOL.gameObject.SetActive(false);



					}else{
						GI_PROXY_SPOT_script.enabled=true;
						SpotLight_POOL.gameObject.SetActive(true);



						GI_PROXY_script.enabled=false;
						LightPOOL.SetActive(false);
						this.GetComponent<Light>().enabled=false;

						PointLight.gameObject.SetActive(false);
						PointLight_POOL.gameObject.SetActive(false);
						PointLight.gameObject.transform.position=Initial_point_light_pos;
						
						this.GetComponent<Light>().enabled=false;


					}
					
				}

				////// POINT on/off lights
				
				string GI_ON_POINT = "Toggle Point GI on";
				if(GI_PROXY_POINT_script.enabled){GI_ON_POINT = "Toggle Point GI off - "+GI_PROXY_POINT_script.Lights.Count+" Lights";}
				
				if(GUI.Button(new Rect(400,50,200,40),GI_ON_POINT)){
					
					if(GI_PROXY_POINT_script.enabled){
						GI_PROXY_POINT_script.enabled=false;
						PointLight_POOL.gameObject.SetActive(false);
						

						
					}else{
						GI_PROXY_POINT_script.enabled=true;
						PointLight_POOL.gameObject.SetActive(true);
						
				
						
						GI_PROXY_script.enabled=false;
						LightPOOL.SetActive(false);
						this.GetComponent<Light>().enabled=false;
						
						//DISABLE SPOT LIGHT
						SpotLight.gameObject.SetActive(false);
						SpotLight_POOL.gameObject.SetActive(false);
						
						
					}
					
				}



				///// Rotate sun

				
				GUI.TextArea(new Rect(0,160-28,200,25),"Sun light");
				//Debug.Log (RenderSettings.ambientLight.r);
				intensity_current = GUI.HorizontalSlider(new Rect(0,160,200,30),GetComponent<Light>().intensity,0.2f,0.65f);
				GetComponent<Light>().intensity = intensity_current;

				GUI.TextArea(new Rect(0,220-28,200,25),"Ambient light");
				//Debug.Log (RenderSettings.ambientLight.r);
				ambience_current = GUI.HorizontalSlider(new Rect(0,220,200,30),RenderSettings.ambientLight.r*255,5,60);
				RenderSettings.ambientLight = new Color(ambience_current/255,ambience_current/255,ambience_current/255,RenderSettings.ambientLight.a);

				GUI.TextArea(new Rect(0,280-28,200,25),"Rotate Sun Horizontally");
				rotation_current = GUI.HorizontalSlider(new Rect(0,280,200,30),this.gameObject.transform.root.eulerAngles.y,100-90,270);
				this.gameObject.transform.root.eulerAngles =new Vector3(this.gameObject.transform.root.eulerAngles.x,rotation_current,this.gameObject.transform.root.eulerAngles.z);

				GUI.TextArea(new Rect(0,340-28,200,25),"Rotate Sun Vertically");
				rotation_current_X = GUI.HorizontalSlider(new Rect(0,340,200,30),this.gameObject.transform.root.eulerAngles.x,45,80);
				this.gameObject.transform.root.eulerAngles =new Vector3(rotation_current_X,this.gameObject.transform.root.eulerAngles.y,this.gameObject.transform.root.eulerAngles.z);

				//point move vertical

				GUI.TextArea(new Rect(0,400-28,200,25),"Move Point Light Vertically");
				position_current_Z = GUI.HorizontalSlider(new Rect(0,400,200,30),PointLight.transform.position.z,-45,-2.5f);
				PointLight.transform.position =new Vector3(PointLight.transform.position.x,PointLight.transform.position.y,position_current_Z);

				//parent point to procedural
				string ONOFF_mount="Mount";
					
				if(PointLight.gameObject.transform.root.gameObject == Procedural_sphere.gameObject){
						ONOFF_mount="Dismount";
				}
				if(GI_PROXY_POINT_script.enabled){
						if(GUI.Button(new Rect(0,445,200,20),ONOFF_mount+ " point light")){
							if(PointLight.gameObject.transform.root.gameObject == Procedural_sphere.gameObject){

								PointLight.localScale = 1.24f*Vector3.one;
								PointLight.gameObject.transform.parent =null;

							}else{
								PointLight.gameObject.transform.parent = Procedural_sphere;
								PointLight.localPosition = Vector3.zero;
								PointLight.localScale = 1.7f*Vector3.one;
							}
						}
				}

				//Tentacles
				if(Tentacles!=null){

						if(GUI.Button(new Rect(0,425,200,20),"Toggle Tentacles")){
							if(Tentacles.gameObject.activeInHierarchy){
								Tentacles.gameObject.SetActive(false);
							}else{
								Tentacles.gameObject.SetActive(true);
							}
						}

				}

					//Quality

						
						if(GUI.Button(new Rect(0,465,200,20),"Toggle Quality")){
						if(QualitySettings.GetQualityLevel() == 4){
								QualitySettings.IncreaseLevel(false);
								

							}else{
								
								QualitySettings.DecreaseLevel(false);
							}
						}
						

					if(GUI.Button(new Rect(0,485,200,20),"Toggle Atrium")){
						if(MainAtrium.activeInHierarchy){
							MainAtrium.SetActive(false);
							WhiteAtrium.SetActive(true);
						}else{
							MainAtrium.SetActive(true);
							WhiteAtrium.SetActive(false);
						}
					}

				///////
				if(PointLight!=null){

					string ONOFF="off";
					
					if(GI_PROXY_script.enabled){
						ONOFF="on";
					}
					
					if(1==1){
						if(GUI.Button(new Rect(400,10,200,40),"Toggle Point Light "+ONOFF) & 1==1){

							if(GI_PROXY_script.enabled){
								GI_PROXY_script.enabled=false;
								LightPOOL.SetActive(false);

								PointLight.gameObject.SetActive(true);
								PointLight_POOL.gameObject.SetActive(true);
								PointLight.gameObject.transform.position=Initial_point_light_pos;

								this.GetComponent<Light>().enabled=false;

								//DISABLE SPOT LIGHT
								SpotLight.gameObject.SetActive(false);
								SpotLight_POOL.gameObject.SetActive(false);

							}else{
								GI_PROXY_script.enabled=true;
								LightPOOL.SetActive(true);

								PointLight.gameObject.SetActive(false);
								PointLight_POOL.gameObject.SetActive(false);
								PointLight.gameObject.transform.position=Initial_point_light_pos;

								this.GetComponent<Light>().enabled=true;

								
							}
						}
					}
				}
				///////
				if(SpotLight!=null){
					
					string ONOFFSPOT="off";
					
					if(!SpotLight.gameObject.activeInHierarchy){
						ONOFFSPOT="on";
					}
					
					if(1==1){
						if(GUI.Button(new Rect(0,10,200,40),"Toggle Spot Light "+ONOFFSPOT)){
							
							if(!SpotLight.gameObject.activeInHierarchy){
								GI_PROXY_script.enabled=false;
								LightPOOL.SetActive(false);
								
								PointLight.gameObject.SetActive(false);
								PointLight_POOL.gameObject.SetActive(false);
								PointLight.gameObject.transform.position=Initial_point_light_pos;
								
								this.GetComponent<Light>().enabled=false;
								
								//DISABLE SPOT LIGHT
								SpotLight.gameObject.SetActive(true);
								SpotLight_POOL.gameObject.SetActive(true);
								
							}else{
							
								
								this.GetComponent<Light>().enabled=true;

								//DISABLE SPOT LIGHT
								SpotLight.gameObject.SetActive(false);
								SpotLight_POOL.gameObject.SetActive(false);

								
							}
						}
					}
				}


				

				

				//// POINT LIGHT ENCHANCE

				//Degrade-appear 0.135 (0.035), 0.135 (0.025), 0.32(0.099) intensity
				string HIGH_POINT="high";
				
				if(GI_PROXY_POINT_script.Degrade_speed == 0.14f){
					HIGH_POINT="low";
				}
				
				if(PointLight!=null){
					if(GUI.Button(new Rect(400,100,200,40),"Toggle Point GI power "+HIGH_POINT) & 1==1){
						
						for(int i=0;i<GI_PROXY_POINT_script.Lights.Count;i++){
							
							Destroy(GI_PROXY_POINT_script.Lights[i].gameObject);
							
						}
						
						GI_PROXY_POINT_script.Lights.Clear();
						
						if(GI_PROXY_POINT_script.Degrade_speed == 0.09f){
							GI_PROXY_POINT_script.Degrade_speed = 0.14f;//= 0.135f;
						}else{
							GI_PROXY_POINT_script.Degrade_speed = 0.09f;
						}
						
						if(GI_PROXY_POINT_script.Appear_speed == 0.08f){
							GI_PROXY_POINT_script.Appear_speed = 0.12f;//= 0.135f;
						}else{
							GI_PROXY_POINT_script.Appear_speed = 0.08f;
						}
						
						if(GI_PROXY_POINT_script.Bounce_Intensity == 0.1f){
							GI_PROXY_POINT_script.Bounce_Intensity = 0.4f;// = 0.22f;
						}else{
							GI_PROXY_POINT_script.Bounce_Intensity = 0.1f;
						}
						
						if(GI_PROXY_POINT_script.Color_change_speed == 4){
							GI_PROXY_POINT_script.Color_change_speed = 6;
						}else{
							GI_PROXY_POINT_script.Color_change_speed = 4;
						}
						
						if(GI_PROXY_POINT_script.Bounce_Range == 60){
							GI_PROXY_POINT_script.Bounce_Range = 50;
						}else{
							GI_PROXY_POINT_script.Bounce_Range = 60;
						}
						
						if(GI_PROXY_POINT_script.min_density_dist == 9){
							GI_PROXY_POINT_script.min_density_dist = 9;
						}else{
							GI_PROXY_POINT_script.min_density_dist = 9;
						}
						
						if(GI_PROXY_POINT_script.max_hit_light_dist == 7){
							GI_PROXY_POINT_script.max_hit_light_dist = 9;
						}else{
							GI_PROXY_POINT_script.max_hit_light_dist = 7;
						}

						if(GI_PROXY_POINT_script.PointLight_Radius == 9){
							GI_PROXY_POINT_script.PointLight_Radius = 10;
						}else{
							GI_PROXY_POINT_script.PointLight_Radius = 9;
						}
						
					}
				}//END point light

			}
		}
		}

	// Update is called once per frame
	void Update () {
			if(LookAtTarget!=null){
				this.gameObject.transform.LookAt(LookAtTarget.position);
			}

			if(!intro_ended){

				if(Time.fixedTime - current_time < intro_time){

					if(Time.fixedTime - current_time < 1.6f){

					}else if(Time.fixedTime - current_time < 8.5f){

						Intro_letters.position = new Vector3(Intro_letters.position.x,Intro_letters.position.y,Mathf.Lerp((Intro_letters.position.z),Intro_letters.position.z + (2.35f*Time.deltaTime),0.5f));

					}else
					if(Time.fixedTime - current_time < intro_time){

						Intro_letters.position = new Vector3(Intro_letters.position.x,Intro_letters.position.y,Mathf.Lerp((Intro_letters.position.z),Intro_letters.position.z + (19.99f*Time.deltaTime),0.5f));

					}else if(Time.fixedTime - current_time < intro_time){
						//
					}
					if(Procedural_sphere.gameObject.activeInHierarchy){
						Procedural_sphere.gameObject.SetActive(false);
					}
					GUI_texts.gameObject.SetActive(false);
				}else{
					intro_ended=true;
					Intro_letters.gameObject.SetActive(false);
					Intro_letters_GI_POOL.gameObject.SetActive(false);
					GUI_texts.gameObject.SetActive(true);

					if(!Procedural_sphere.gameObject.activeInHierarchy){
						Procedural_sphere.gameObject.SetActive(true);
					}
				}
			
			}

	}
}

}
