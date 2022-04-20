using UnityEngine;
using System.Collections;
using Artngame.GIPROXY;

namespace Artngame.SKYMASTER {
public class SKYMASTER_DemoV30_AIRPLANE : MonoBehaviour {

	#pragma warning disable 414

	void Start () {

		if(SKYMASTER_OBJ!=null){
				SUNMASTER = SKYMASTER_OBJ.GetComponent(typeof(SkyMasterManager)) as SkyMasterManager;
		}
		SPEED = SUNMASTER.SPEED;
		SUNMASTER.Seasonal_change_auto = false;
		TOD = SUNMASTER.Current_Time;

		//WaterHeightHandle = SUNMASTER.water.gameObject.GetComponent<WaterHeightSM>();
		//WaterHandler = SUNMASTER.water.gameObject.GetComponent<WaterHandlerSM>();

			Dome_rot = SUNMASTER.Rot_Sun_Y;
	}

	public float Sun_time_start = 14.43f;	//at this time, rotation of sunsystem must be 62 (14, -1.525879e-05, -1.525879e-05 WORKS !!)
	public GameObject SKYMASTER_OBJ;
	SkyMasterManager SUNMASTER;
	
	public bool HUD_ON=true;
	bool set_sun_start=false;

		public Material CloudMat1;
		public Material CloudMat2;
		public Material CloudMat3;
		public Material CloudMat4;
		int cloud_mat_counter=0;

	float Dome_rot = 0;
	float Camera_up;
	float TOD;
	float SPEED;
	//WaterHeightSM WaterHeightHandle;
	//WaterHandlerSM WaterHandler;
	int windowsON = 0;
//	public Transform windowsSpot;
//	public Transform underwaterSpot;
//		public Transform AtollViewSpot;
//		public Transform oceanSpot;
//		public Transform boatSpot;
//		public Transform boatSpot2;
//
//		public Transform smokeSPOT;
//
//		public GameObject farOceanplane;
		public Transform planeVIEW;
		public Transform backVIEW;
		bool inPlane = false;
		//bool colorsON = false;

	void OnGUI() {

			float BOX_WIDTH = 100;float BOX_HEIGHT = 30;

			string HUD_gui = "Disable HUD";
			if(!HUD_ON){
				HUD_gui = "Enable HUD";
			}
			if (GUI.Button(new Rect(2, 0*BOX_HEIGHT, BOX_WIDTH-2, 22), HUD_gui)){				
				if(HUD_ON){
					HUD_ON = false;
				}else{
					HUD_ON = true;
				}				
			}

			float XOffset = 5;

			if (SUNMASTER.currentWeather != null && SUNMASTER.currentWeather.VolumeCloud != null) {

				if (GUI.Button(new Rect(7*(BOX_WIDTH+XOffset)+30, 0, BOX_WIDTH+60, 22), "Cycle cloud materials"+"("+ (cloud_mat_counter+1).ToString() +")")){
					if(cloud_mat_counter == 0){
						SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material = CloudMat1;
						SUNMASTER.currentWeather.VolumeScript.ScatterMat = SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material;

					}else if(cloud_mat_counter == 1){
						SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material = CloudMat2;
						SUNMASTER.currentWeather.VolumeScript.ScatterMat = SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material;
					}else if(cloud_mat_counter == 2){
						SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material = CloudMat3;
						SUNMASTER.currentWeather.VolumeScript.ScatterMat = SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material;
					}else if(cloud_mat_counter == 3){
						SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material = CloudMat4;
						SUNMASTER.currentWeather.VolumeScript.ScatterMat = SUNMASTER.currentWeather.VolumeCloud.GetComponent<Renderer>().material;
					}
					cloud_mat_counter++;
				}
				if(cloud_mat_counter > 3){
					cloud_mat_counter=0;
				}
			}


			if(HUD_ON){	

				GUI.TextArea( new Rect(2, 1*BOX_HEIGHT, 100-2, 20),"Sun Speed");
			SPEED = GUI.HorizontalSlider(new Rect(2, 1*BOX_HEIGHT+25, 100-2, 30),SPEED,0.01f,70f);
			SUNMASTER.SPEED = SPEED;

				GUI.TextArea( new Rect(2, 1*BOX_HEIGHT+50, 100-2, 20),"Sun Intensity");
				SUNMASTER.Max_sun_intensity = GUI.HorizontalSlider(new Rect(2, 1*BOX_HEIGHT+25+50, 100-2, 15),SUNMASTER.Max_sun_intensity,0.5f,2.50f);

			//CAMERA 
//			if (windowsON == 0 | windowsON == 3) {
//				GUI.TextArea (new Rect (6 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 25, BOX_WIDTH + 0, 20), "Camera height");
//				float min_height = 6f;
//				Camera_up = GUI.HorizontalSlider (new Rect (6 * (BOX_WIDTH + XOffset), 1 * BOX_HEIGHT + 25 + 25, BOX_WIDTH + 315, 30), Camera.main.transform.position.y, min_height, 1760);			
//				Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera_up, Camera.main.transform.position.z);
//			}
//
//			if (Camera_up > 200) {
//				if (farOceanplane.activeInHierarchy) {
//					farOceanplane.SetActive (false);
//				}
//			} else {
//				if (!farOceanplane.activeInHierarchy) {
//					farOceanplane.SetActive (true);
//				}
//			}
				string ENTER = "Front of Plane View";
				if(inPlane){
					ENTER = "Behind Plane View";
				}
				if(planeVIEW != null){
					if (GUI.Button(new Rect(6*(BOX_WIDTH+XOffset)+0, 0, BOX_WIDTH+30, 22), ENTER)){
						if(inPlane){
							inPlane = false;
							Camera.main.transform.parent = backVIEW;
							Camera.main.transform.localPosition =  new Vector3(0,12,0);
						}else{
							inPlane = true;
							Camera.main.transform.parent = planeVIEW;
							Camera.main.transform.forward = planeVIEW.forward;
							Camera.main.transform.localPosition =  new Vector3(0,17,-19);
						}
					}
				}

			//DOME CONTROL		
				GUI.TextArea( new Rect(2*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH+0, 20),"SkyDome rot");			
				Dome_rot = GUI.HorizontalSlider(new Rect(2*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH+0, 30),Dome_rot,0,360);
			SUNMASTER.Rot_Sun_Y = Dome_rot;

			//DOME CONTROL		
				GUI.TextArea( new Rect(3*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH+0, 20),"Wind direction");			
				float aurlerY = GUI.HorizontalSlider(new Rect(3*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH+0, 30),SUNMASTER.windZone.transform.eulerAngles.y,0,360);
			Vector3 angles = SUNMASTER.windZone.gameObject.transform.eulerAngles;
			SUNMASTER.windZone.gameObject.transform.eulerAngles = new Vector3(angles.x,aurlerY,angles.z);

			//DOME CONTROL		
				GUI.TextArea( new Rect(4*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH+0, 20),"Wind intensity");			
				SUNMASTER.windZone.windMain  = GUI.HorizontalSlider(new Rect(4*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH+0, 30),SUNMASTER.windZone.windMain ,0,24);		

			//time of day
				GUI.TextArea( new Rect(1*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH, 20),"Time of Day");
				TOD = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH, 20),SUNMASTER.Current_Time,0f,24);
			SUNMASTER.Current_Time = TOD;

//			//WAVE HEIGHT
//				GUI.TextArea( new Rect(5*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH, 20),"Wave height");
//				WaterHandler.waterScaleOffset.y = GUI.HorizontalSlider(new Rect(5*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.waterScaleOffset.y,0f,3);
//
//			//BOAT SPEED
//				GUI.TextArea( new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50, BOX_WIDTH, 20),"Boat Speed");
//				WaterHeightHandle.BoatSpeed = GUI.HorizontalSlider(new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50, BOX_WIDTH, 20),WaterHeightHandle.BoatSpeed,0.3f,5);
//					
//
//				//EXTRA CONTROLS
//				if(1==1){
//					if (GUI.Button(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50, BOX_WIDTH, 20), "Reset Offsets")){
//						WaterHandler.FresnelOffset = 0;
//						WaterHandler.FresnelBias = 0;
//						WaterHandler.BumpFocusOffset = 0;
//						WaterHandler.DepthColorOffset = 0;
//						WaterHandler.ShoreBlendOffset = 0;
//					}
//					if (GUI.Button(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+25, BOX_WIDTH, 20), "Toggle Offsets")){
//						if(offsetsON){
//							offsetsON = false;
//						}else{
//							offsetsON = true;
//						}
//					}
//					if(offsetsON){
//						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+1*50, BOX_WIDTH*1, 20),"Fresnel Power");
//						WaterHandler.FresnelOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+1*50, BOX_WIDTH*3, 20),WaterHandler.FresnelOffset,-0.5f,1.7905f);//
//
//						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+2*50, BOX_WIDTH*1, 20),"Fresnel Bias");
//						WaterHandler.FresnelBias = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+2*50, BOX_WIDTH*3, 20),WaterHandler.FresnelBias,-130f,240);
//
//						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+3*50, BOX_WIDTH*1, 20),"Specular focus");
//						WaterHandler.BumpFocusOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+3*50, BOX_WIDTH*3, 20),WaterHandler.BumpFocusOffset,-4,4);//
//
//						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+4*50, BOX_WIDTH*1, 20),"Depth Offset");
//						WaterHandler.ShoreBlendOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+4*50, BOX_WIDTH*3, 20),WaterHandler.ShoreBlendOffset,-0.155f,0.1f);//-0.031f
//
//						GUI.TextArea( new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+50+5*50, BOX_WIDTH*1, 20),"Depth FX");
//						WaterHandler.DepthColorOffset = GUI.HorizontalSlider(new Rect(9*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+50+50+5*50, BOX_WIDTH*3, 20),WaterHandler.DepthColorOffset,-140f,10);
//					}
//				}
//
//				if (GUI.Button(new Rect(11*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Smoke & FX")){
//				if(smokeSPOT !=null){
//					if(smokeSPOT.gameObject.activeInHierarchy){
//						smokeSPOT.gameObject.SetActive(false);
//					}else{
//						smokeSPOT.gameObject.SetActive(true);
//							WaterHandler.waterScaleOffset.y =0;
//							WaterHandler.waterType = WaterHandlerSM.WaterPreset.Atoll;
//					}		
//				}
//			}
//
//
//		//BACK TO BOAT		
//		if(windowsON==0){
//					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Enter Cave")){
//						if(Camera.main != null){
//							Camera.main.transform.parent = windowsSpot;
//							Camera.main.transform.forward = windowsSpot.forward;
//							Camera.main.transform.localPosition = new Vector3(-0.5f,1.65f,-2.4f);
//							//WaterHeightHandle.controlBoat = true;
//							//WaterHeightHandle.LerpMotion = true;
//							windowsON = 1;
//							WaterHandler.waterType = WaterHandlerSM.WaterPreset.River;	
//
//							// move water and boat to boatSpot2
//							WaterHeightHandle.BoatSpeed = 0.3f;
//							WaterHeightHandle.SampleCube.position = new Vector3(boatSpot2.position.x,WaterHeightHandle.SampleCube.position.y,boatSpot2.position.z);
//							WaterHeightHandle.start_pos = new Vector3(boatSpot2.position.x,WaterHeightHandle.SampleCube.position.y,boatSpot2.position.z);
//							WaterHandler.transform.position = new Vector3(boatSpot2.position.x,WaterHandler.transform.position.y,boatSpot2.position.z);
//
//							WaterHandler.DisableUnderwater = true;
//						}
//					}
//		}else if(windowsON==1){
//
//					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Underwater")){
//						if(Camera.main != null){
//							Camera.main.transform.parent = underwaterSpot;
//							Camera.main.transform.localPosition = new Vector3(0,0,0);
//							WaterHeightHandle.controlBoat = true;
//							WaterHeightHandle.LerpMotion = true;
//							windowsON = 2;
//							WaterHandler.DisableUnderwater = false;
//
//							WaterHandler.DisableUnderwater = false;
//						}
//					}
//			
//		}else if(windowsON==2){
//					
//					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Atoll View")){
//						if(Camera.main != null){
//							Camera.main.transform.parent = AtollViewSpot;
//							Camera.main.transform.localPosition = new Vector3(0,0,0);
//							Camera.main.transform.up = Vector3.up;
//							WaterHeightHandle.controlBoat = true;
//							WaterHeightHandle.LerpMotion = true;
//							windowsON = 3;
//							WaterHandler.DisableUnderwater = false;
//							WaterHandler.waterType = WaterHandlerSM.WaterPreset.Atoll;	
//
//							WaterHandler.DisableUnderwater = true;
//						}
//					}
//					
//		}else{
//					if(GUI.Button(new Rect(1*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22),"Board boat")){
//						if(Camera.main != null){
//							Camera.main.transform.parent = WaterHeightHandle.SampleCube;
//							Camera.main.transform.forward = WaterHeightHandle.SampleCube.forward;
//							Camera.main.transform.localPosition = new Vector3(-0.5f,1.65f,-2.4f);
//							WaterHeightHandle.controlBoat = true;
//							WaterHeightHandle.LerpMotion = true;
//							windowsON = 0;
//
//							WaterHeightHandle.start_pos = oceanSpot.position;
//							WaterHeightHandle.SampleCube.position = boatSpot.position;
//							WaterHandler.DisableUnderwater = false;
//							WaterHandler.waterType = WaterHandlerSM.WaterPreset.River;	
//
//							WaterHeightHandle.BoatSpeed = 1.2f;
//							WaterHandler.transform.position = new Vector3(oceanSpot.position.x,WaterHandler.transform.position.y,oceanSpot.position.z);
//
//							WaterHandler.DisableUnderwater = true;
//						}
//					}
//		}

		//VOLUME WEATHER
		if((SUNMASTER.currentWeather!=null && SUNMASTER.currentWeather.currentState != WeatherSM.Volume_Weather_State.FadeIn) | SUNMASTER.currentWeather == null){
					if (GUI.Button(new Rect(2*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Cloudy")){
				SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Cloudy;			
			}	
					if (GUI.Button(new Rect(3*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Snow")){
				SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.SnowStorm;			
			}
			if (GUI.Button(new Rect(4*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Heavy Storm")){
				SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.HeavyStorm;			
			}
			if (GUI.Button(new Rect(5*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Rain")){
				SUNMASTER.currentWeatherName = SkyMasterManager.Volume_Weather_types.Rain;		
			}
		}

		//WATER TYPE

//				if(windowsON==2){
//
//					if (GUI.Button(new Rect(5*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Turbulent")){
//						WaterHandler.underWaterType = WaterHandlerSM.UnderWaterPreset.Turbulent;			
//					}
//					if (GUI.Button(new Rect(6*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Calm")){
//						WaterHandler.underWaterType = WaterHandlerSM.UnderWaterPreset.Calm;			
//					}
//
//				}else{
//					if (GUI.Button(new Rect(6*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "Caribbean")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Caribbean;			
//					}
//					if (GUI.Button(new Rect(6*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Lake")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Lake;			
//					}
//					if (GUI.Button(new Rect(10*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Atoll")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Atoll;			
//					}
//					if (GUI.Button(new Rect(9*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Dark Ocean")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.DarkOcean;			
//						//WaterHandler.waterScaleOffset.y = 0.4f;
//						if(Camera.main != null){
//							WaterHeightHandle.SampleCube.position = oceanSpot.position;
//							WaterHeightHandle.start_pos = oceanSpot.position;
//							WaterHandler.DisableUnderwater = true;
//							Camera.main.transform.localPosition = new Vector3(-0.5f,1.65f,-2.4f);
//						}
//					}
//					if (GUI.Button(new Rect(8*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Focus Ocean")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.FocusOcean;			
//					}
//					if (GUI.Button(new Rect(7*(BOX_WIDTH+XOffset), 0, BOX_WIDTH, 22), "Muddy Water")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Muddy;			
//					}
//					if (GUI.Button(new Rect(7*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "River")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.River;			
//					}
//					if (GUI.Button(new Rect(8*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "Small Waves")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.SmallWaves;			
//					}
//					if (GUI.Button(new Rect(9*(BOX_WIDTH+XOffset), 30, BOX_WIDTH, 22), "Ocean")){
//						WaterHandler.waterType = WaterHandlerSM.WaterPreset.Ocean;
//
//						if(Camera.main != null){
//							WaterHeightHandle.SampleCube.position = oceanSpot.position;
//							WaterHeightHandle.start_pos = oceanSpot.position;
//							WaterHandler.DisableUnderwater = true;
//							Camera.main.transform.localPosition = new Vector3(-0.5f,1.65f,-2.4f);
//						}
//					}
//	
////					if(WaterHandler.waterType == WaterHandlerSM.WaterPreset.DarkOcean){
////						//WaterHandler.waterScaleOffset.y = -6.4f;
////						//WaterHandler.BelowWater = false;
////					}
//
//				}
//
//				//WAVE HEIGHT
//				GUI.TextArea( new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH, 20),"Water detail");
//				WaterHandler.bumpTilingXoffset = GUI.HorizontalSlider(new Rect(11*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.bumpTilingXoffset,0.02f,0.3f);
//				WaterHandler.bumpTilingYoffset = WaterHandler.bumpTilingXoffset; 
//
//				GUI.TextArea( new Rect(11*(BOX_WIDTH+XOffset), 2*BOX_HEIGHT+20, BOX_WIDTH, 20),"Refraction");
//				WaterHandler.RefractOffset = GUI.HorizontalSlider(new Rect(11*(BOX_WIDTH+XOffset), 2*BOX_HEIGHT+25+20, BOX_WIDTH, 20),WaterHandler.RefractOffset,-0.2f,2f);
//
//				GUI.TextArea( new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT, BOX_WIDTH, 20),"Extra Waves");
//				WaterHandler.ExtraWavesFactor.x = GUI.HorizontalSlider(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.ExtraWavesFactor.x,0f,4f);
//				WaterHandler.ExtraWavesFactor.y = GUI.HorizontalSlider(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+25, BOX_WIDTH, 20),WaterHandler.ExtraWavesFactor.y,0f,1f);
//				WaterHandler.ExtraWavesFactor.z = GUI.HorizontalSlider(new Rect(10*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+25+25+25, BOX_WIDTH, 20),WaterHandler.ExtraWavesFactor.z,0f,1f);
//
//
//
//
//				//ATOLL COLORS
//				if (GUI.Button(new Rect(0*(BOX_WIDTH+XOffset), 1*BOX_HEIGHT+50+20+25, BOX_WIDTH, 20), "Atoll colors")){
//					if(colorsON){
//						colorsON = false;
//					}else{
//						colorsON = true;
//					}
//				}
//				if(colorsON){
//					GUI.TextArea( new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT, BOX_WIDTH+32, 20),"Base Color (RGBA)");
//					WaterHandler.AtollWaterColor.r = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.r,0f,4f);
//					WaterHandler.AtollWaterColor.g = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.g,0f,1f);
//					WaterHandler.AtollWaterColor.b = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.b,0f,1f);
//					WaterHandler.AtollWaterColor.a = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 5*BOX_HEIGHT+25+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollWaterColor.a,0f,1f);
//
//					GUI.TextArea( new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT, BOX_WIDTH+32, 20),"Reflect Color (RGBA)");
//					WaterHandler.AtollReflectColor.r = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.r,0f,4f);
//					WaterHandler.AtollReflectColor.g = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.g,0f,1f);
//					WaterHandler.AtollReflectColor.b = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.b,0f,1f);
//					WaterHandler.AtollReflectColor.a = GUI.HorizontalSlider(new Rect(1*(BOX_WIDTH+XOffset), 9*BOX_HEIGHT+25+25+25+25, BOX_WIDTH, 20),WaterHandler.AtollReflectColor.a,0f,1f);
//				}



		if(SPEED < 1){
			//SPEED =0;
		}

		if(SUNMASTER.Current_Time!=Sun_time_start & !set_sun_start){				
			set_sun_start=true;
		}
					
							
	
		


	
					
	}
  }// END OnGUI	
}
}