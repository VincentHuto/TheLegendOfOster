using UnityEngine;
using System.Collections;
//using Artngame.SKYMASTER;

namespace Artngame.SKYMASTER {
public class TimerSKYMASTER : MonoBehaviour {

		SkyMasterManager skyManager;
		public bool realWorldTime = false;	
		public bool staticReference = false;
		public bool startWithRealTime = false;

		public int startYear=2016;
		public int startMonth=6;
		public int startDay=6;
		public int startHour=12;
		float shiftTime=0;
		//float secondsCounter=0;

		//int currentGameDay;
		//int currentGameMonth;
		//int currentGameYear;

		public bool enableGUI = false;

	// Use this for initialization
	void Start () {
			skyManager = this.GetComponent<SkyMasterManager> ();
			float hour = System.DateTime.Now.Hour * 3600;
			float minutes = System.DateTime.Now.Minute * 60;
			float secs = System.DateTime.Now.Second;
			float seconds = hour + minutes + secs;
			//float fraction = seconds / 3600;

			//get real world seconds when game starts
			shiftTime = seconds;
			skyManager.Current_Time = startHour;

			startGameTime = new System.DateTime (startYear, startMonth, startDay, startHour, 0, 0);

			startRealTime = new System.DateTime (System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);

			//currentGameDay = startDay;
			//currentGameMonth = startMonth;
			//currentGameYear = startYear;
	}	

	System.DateTime	startGameTime;
	System.DateTime	startRealTime;

	System.DateTime	currentGameTime;
	System.DateTime	currentRealTime;

	bool startedWithRealTime = false;

	// Update is called once per frame
	void Update () {

			if (!startedWithRealTime && startWithRealTime) {				
				startGameTime = new System.DateTime (System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
				startedWithRealTime = true;
			}

			float hour = System.DateTime.Now.Hour * 3600;
			float minutes = System.DateTime.Now.Minute * 60;
			float secs = System.DateTime.Now.Second;
			float seconds = hour + minutes + secs;

			if(realWorldTime){
				skyManager.Auto_Cycle_Sky = true;
				skyManager.SPEED = 0.0001f;
				float fraction = seconds / 3600;
				//Debug.Log ("hour="+System.DateTime.Now.Hour + " minute="+System.DateTime.Now.Minute + " secs="+System.DateTime.Now.Second  );
				//Debug.Log (fraction);
				skyManager.Current_Time = fraction;//System.DateTime.Now.TimeOfDay.TotalSeconds
			}else{
				//override skymaster cycling to reduce problems from deltatime inconsitencies
				skyManager.Auto_Cycle_Sky = true;
				skyManager.SPEED = 0.0001f;

				if (!staticReference) {
					//float fraction = (seconds-shiftTime) / 3600;
					float secsDiff = (seconds - shiftTime) / 3600;
					skyManager.Current_Time = skyManager.Current_Time + secsDiff;

					shiftTime = seconds;
				} else {
					//use static initial time references for both skymaster and real world times to avoid incremental method errors
					currentRealTime = new System.DateTime (System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
					float secsDiff = (float)(currentRealTime - startRealTime).TotalSeconds;
					currentGameTime = startGameTime;
					currentGameTime = currentGameTime.AddSeconds (secsDiff);

					float hour1 = currentGameTime.Hour * 3600;
					float minutes1 = currentGameTime.Minute * 60;
					float secs1 = currentGameTime.Second;
					float seconds1 = hour1 + minutes1 + secs1;
					float fraction = seconds1 / 3600;
					skyManager.Current_Time = fraction;

					//currentGameDay = currentGameTime.Day;

					//Debug.Log ("startGameTime  =" + startGameTime.ToLongDateString() + " ..." + startGameTime.ToLongTimeString());
					//Debug.Log ("currentGameTime=" + currentGameTime.ToLongDateString() + " ..." + currentGameTime.ToLongTimeString());
				}
			}



	}

		void OnGUI(){
			if (enableGUI) {
				GUI.TextField (new Rect (500, 400, 400, 22), "Game Date ="+currentGameTime.ToLongDateString());
				GUI.TextField (new Rect (500, 400+22, 400, 22), "Game Time ="+currentGameTime.ToLongTimeString());
			}
		}
}
}
