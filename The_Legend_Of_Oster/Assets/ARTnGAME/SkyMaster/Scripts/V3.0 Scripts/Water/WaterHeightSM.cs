using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.SKYMASTER {
public class WaterHeightSM : MonoBehaviour {

		//v3.2 - control local waves
		public bool UseLocalWaves = true;
		public float LocalHeightAdjust = 4.5f;
		public float LocalYCutoff = 3;
		public float LocalFreq = 0.3f;
		public float LocalStartRadius = 5;

		public bool followCamera;//follow camera toggle
		public bool followBoat = false;
		public bool lerpFollow = false;
		public bool constantFollow = false;//constant motion, no smooth in/out
		public float lerpFollowSpeed = 0.1f;

		public List<Transform> ThrowObjects = new List<Transform>();
		public List<Vector3> ThrowObjectsWaterPos = new List<Vector3>();
		public List<Vector3> ThrowObjectsStartPos = new List<Vector3>();
		//public List<Vector3> ThrowObjectsStartPos;

		public List<Transform> WaterObjects = new List<Transform>();
		public List<Vector3> WaterObjectsWaterPos = new List<Vector3>();
		public List<Vector3> WaterObjectsStartPos = new List<Vector3>();
		public GameObject ThrowItem;
		public float ThrowPower = 1150;

	public bool ShiftHorPosition = true;
	public bool LerpMotion = false;
	public float lerpSpeed = 1;
		public float lerpRotSpeed = 0.05f;
	public float BoatSpeed = 0.22f;
		public float BoatRotSpeed = 2.2f;
		public bool controlBoat=false;

	public Transform SampleCube;
	public Material WaterMaterial;
	public WaterHandlerSM Waterhandler;
	// Use this for initialization
	void Start () {
		start_pos = SampleCube.transform.position;

//		for(int i=0;i<ThrowObjects.Count;i++){
//			ThrowObjectsStartPos[i] = ThrowObjects[i].position;
//		}
		for(int i=0;i<WaterObjects.Count;i++){
			WaterObjectsStartPos[i] = WaterObjects[i].position; //also define when instantiating a new one
		}

	}

	public Vector3 start_pos;
	Vector3 prev_pos;
		//float currentAngle=0;
	float CurrentRot = 0;
		//float CurrentRandRot = 0;

	// Update is called once per frame
	void Update () {
	
		if (SampleCube != null && WaterMaterial!=null) {
		
			float PosX = SampleCube.transform.position.x;
			float PosZ = SampleCube.transform.position.z;
			float PosY = SampleCube.transform.position.y;

			Vector4 _GAmplitude = WaterMaterial.GetVector("_GAmplitude");												// amplitude
			Vector4 _GFrequency = WaterMaterial.GetVector("_GFrequency");												// frequency
			Vector4 _GSteepness = WaterMaterial.GetVector("_GSteepness");												// steepness
			Vector4 _GSpeed = WaterMaterial.GetVector("_GSpeed");													// speed
			Vector4 _GDirectionAB = WaterMaterial.GetVector("_GDirectionAB");												// direction # 1, 2
			Vector4 _GDirectionCD = WaterMaterial.GetVector("_GDirectionCD");	

			//FIND HEIGHT

				///// MORE ITEMS

				float heightFactor = 1;
				if(Waterhandler!=null){
					heightFactor = (Waterhandler.waterScaleFactor.y/1.0f  + Waterhandler.waterScaleOffset.y)/1.0f;//v3.2 - changed from 1.5f and 1.2f
				}

				float heightFactor1 = 1;
				if(Waterhandler!=null){
					//heightFactor1 = (Waterhandler.waterScaleFactor.y/1.0f  + Waterhandler.waterScaleOffset.y)/1.0f;
					heightFactor1 = this.transform.localScale.y*1.1f;
				}

				for(int i=WaterObjects.Count-1;i>=0;i--){
					if(WaterObjects[i] == null){
						WaterObjects.RemoveAt(i);
						WaterObjectsWaterPos.RemoveAt(i);
						WaterObjectsStartPos.RemoveAt(i);
					}
				}
				for(int i=ThrowObjects.Count-1;i>=0;i--){
					if(ThrowObjects[i] == null){
						ThrowObjects.RemoveAt(i);
						ThrowObjectsWaterPos.RemoveAt(i);
						ThrowObjectsStartPos.RemoveAt(i);
					}
				}

				for(int i=0;i<WaterObjects.Count;i++){
					float PosX1 = WaterObjects[i].position.x;
					float PosZ1 = WaterObjects[i].position.z;
					float PosY1 = WaterObjects[i].position.y;
					Vector3 GerstnerOffsets1 = GerstnerOffset(new Vector2(PosX1,PosZ1),PosY1,new Vector2(PosX1,PosZ1), _GAmplitude, _GFrequency, _GSteepness, _GSpeed, _GDirectionAB, _GDirectionCD  );
					WaterObjectsWaterPos[i] = new Vector3(GerstnerOffsets1.x,GerstnerOffsets1.y*heightFactor1,GerstnerOffsets1.z);
				}
				for(int i=0;i<WaterObjectsWaterPos.Count;i++){
					//if(WaterObjectsWaterPos[i].y > WaterObjects[i].position.y){
						//Debug.Log("impact with water");
					//}
					//WaterObjects[i].position = WaterObjectsStartPos[i] + WaterObjectsWaterPos[i];

					if(LerpMotion){
						WaterObjects[i].position = Vector3.Lerp(WaterObjects[i].position, new Vector3(WaterObjectsStartPos[i].x, this.transform.position.y-1, WaterObjectsStartPos[i].z) + WaterObjectsWaterPos[i],Time.deltaTime*lerpSpeed);
					}else{
						WaterObjects[i].position = new Vector3(WaterObjectsStartPos[i].x, this.transform.position.y-1, WaterObjectsStartPos[i].z) + WaterObjectsWaterPos[i];
					}
				}

				for(int i=0;i<ThrowObjects.Count;i++){
					float PosX1 = ThrowObjects[i].position.x;
					float PosZ1 = ThrowObjects[i].position.z;
					float PosY1 = ThrowObjects[i].position.y;
					Vector3 GerstnerOffsets1 = GerstnerOffset(new Vector2(PosX1,PosZ1),PosY1,new Vector2(PosX1,PosZ1), _GAmplitude, _GFrequency, _GSteepness, _GSpeed, _GDirectionAB, _GDirectionCD  );
					ThrowObjectsWaterPos[i] = new Vector3(GerstnerOffsets1.x,GerstnerOffsets1.y*heightFactor1,GerstnerOffsets1.z);
				}
				for(int i=0;i<ThrowObjectsWaterPos.Count;i++){
					if(ThrowObjectsWaterPos[i].y + this.transform.position.y > ThrowObjects[i].position.y){
						//Debug.Log("impact with water");
						//add to water list
						ThrowObjects[i].GetComponent<Rigidbody>().isKinematic = true;
						WaterObjects.Add(ThrowObjects[i]);
						WaterObjectsStartPos.Add(ThrowObjects[i].position);
						WaterObjectsWaterPos.Add(ThrowObjectsWaterPos[i]);

						ThrowObjectsWaterPos.RemoveAt(i);
						ThrowObjects.RemoveAt(i);
						ThrowObjectsStartPos.RemoveAt(i);
					}
				}
				if(Input.GetMouseButtonDown(1)){ //v3.4
					if(ThrowItem != null){
						GameObject TEMP = (GameObject)Instantiate(ThrowItem, start_pos+new Vector3(0,5,0),Quaternion.identity);
						TEMP.transform.localScale = TEMP.transform.localScale*5;
						ThrowObjects.Add(TEMP.transform);
						ThrowObjectsWaterPos.Add(new Vector3(0,0,0));
						ThrowObjectsStartPos.Add(TEMP.transform.position);
						if(TEMP != null){
							Rigidbody RGB = TEMP.GetComponent<Rigidbody>() as Rigidbody;

							if(RGB != null){
								if(Camera.main != null){
									RGB.AddForce(Camera.main.transform.forward * ThrowPower);
								}else{
									RGB.AddForce(SampleCube.transform.forward * ThrowPower);
								}
							}
						}
					}
				}

				///// END MORE ITEMS
			
				Vector3 GerstnerOffsets = GerstnerOffset(new Vector2(PosX,PosZ),PosY,new Vector2(PosX,PosZ), _GAmplitude, _GFrequency, _GSteepness, _GSpeed, _GDirectionAB, _GDirectionCD  );
					
				//Debug.Log(GerstnerOffsets);

				Vector3 ShiftPos = new Vector3(0,GerstnerOffsets.y*heightFactor,0);

				if(ShiftHorPosition){
					ShiftPos = new Vector3(GerstnerOffsets.x,GerstnerOffsets.y*heightFactor,GerstnerOffsets.z);
				}

				if(LerpMotion){
					SampleCube.transform.position = Vector3.Lerp(SampleCube.transform.position, start_pos + ShiftPos, Time.deltaTime*lerpSpeed);
				}else{
					SampleCube.transform.position = start_pos + ShiftPos;
				}

				Vector3 MotionDirection = (SampleCube.transform.position - prev_pos).normalized;
				//float Randomizer = Random.Range(-220,220);
				//SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(Mathf.Lerp(currentAngle,Random.Range(-220,220)),(Vector3.up+MotionDirection)),Time.deltaTime*0.2f );

				if(!controlBoat){
			//	SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(100*MotionDirection.y,(Vector3.up+MotionDirection)),Time.deltaTime*lerpRotSpeed );//0.18f

				//CurrentRandRot = Mathf.Lerp(CurrentRandRot, 100*MotionDirection.y,Time.deltaTime*lerpRotSpeed);
				SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(100*MotionDirection.y,(Vector3.up+MotionDirection)),Time.deltaTime*lerpRotSpeed );//0.18f

			//	SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(1010*MotionDirection.y,(MotionDirection)),Time.deltaTime*lerpRotSpeed);
				}else{
					//SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(10*MotionDirection.y,(Vector3.up+0.5f*MotionDirection.normalized)),Time.deltaTime*lerpRotSpeed );//0.18f

					//SampleCube.transform.LookAt(SampleCube.transform.position + MotionDirection);

				//	SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(10*MotionDirection.y,(Vector3.up+0.5f*MotionDirection.normalized)),Time.deltaTime*lerpRotSpeed );//0.18f
					SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(30*MotionDirection.y,(SampleCube.transform.right + SampleCube.transform.up*Random.Range(0,0.5f))),Time.deltaTime*lerpRotSpeed *12);
				}

				Vector3 Velocity = (SampleCube.transform.position - prev_pos)/Time.deltaTime; 

				prev_pos = SampleCube.transform.position ;

				if(controlBoat){
					Vector3 Forward_flat = new Vector3( SampleCube.transform.forward.x,  0, SampleCube.transform.forward.z);
					start_pos = start_pos + Forward_flat * BoatSpeed * Input.GetAxis("Vertical");
					CurrentRot = CurrentRot + Input.GetAxis("Horizontal")*BoatRotSpeed;
					SampleCube.transform.rotation = Quaternion.Lerp(SampleCube.transform.rotation,Quaternion.AngleAxis(CurrentRot,Vector3.up ),Time.deltaTime*BoatRotSpeed );

					//send to local waves
					if(UseLocalWaves && WaterMaterial.HasProperty("_LocalWaveParams")){						
						//v3.2
						WaterMaterial.SetVector("_LocalWaveParams", new Vector4(LocalYCutoff,LocalHeightAdjust*Velocity.magnitude,LocalFreq,LocalStartRadius));
						//WaterMaterial.SetVector("_LocalWaveParams", new Vector4(3f,4.5f*Velocity.magnitude,0.3f,5));
						WaterMaterial.SetVector("_LocalWavePosition", new Vector4(prev_pos.x,prev_pos.y,prev_pos.z,5));
						WaterMaterial.SetVector("_LocalWaveVelocity", new Vector4(Velocity.x,Velocity.y,Velocity.z,0));
					}
				}
		}

			//follow camera
			if (Application.isPlaying && Waterhandler != null) { //v3.2

				if(followCamera && Camera.main != null){
					Vector3 Campos = Camera.main.transform.position;
					//if(Vector3.Distance(Waterhandler.transform.position, Camera.main.transform.position) > 0.55f){
					Vector3 Target = new Vector3(Campos.x,Waterhandler.transform.position.y,Campos.z);
					if(!lerpFollow){
						Waterhandler.transform.position = Target;
					}else{
						if(constantFollow){
							if((Target-Waterhandler.transform.position).magnitude >1.5f){
								Waterhandler.transform.position += (Target - Waterhandler.transform.position).normalized * Time.deltaTime*lerpFollowSpeed;
							}
						}else{
							Waterhandler.transform.position = Vector3.Lerp(Waterhandler.transform.position, Target,Time.deltaTime*lerpFollowSpeed);
						}
					}
					//}
				}
				if(followBoat){
					//if(Vector3.Distance(Waterhandler.transform.position, SampleCube.position) > 0.55f){
					Vector3 Target = new Vector3(SampleCube.position.x,Waterhandler.transform.position.y,SampleCube.position.z);
					if(!lerpFollow){
						Waterhandler.transform.position = Target;
					}else{
						if(constantFollow){
							if((Target-Waterhandler.transform.position).magnitude >1.5f){
								Waterhandler.transform.position += (Target - Waterhandler.transform.position).normalized * Time.deltaTime*lerpFollowSpeed;
							}
						}else{
							Waterhandler.transform.position = Vector3.Lerp(Waterhandler.transform.position,Target,Time.deltaTime*lerpFollowSpeed);
						}
					}
					//}
				}
			}

	}


	public Vector3 GerstnerOffset (
			Vector2 Position, float PosY,Vector2 tileableVtx,						// offsets, nrml will be written
		//Vector4 _GAmplitude,												// amplitude
		//Vector4 _GFrequency,												// frequency
		//Vector4 _GSteepness,												// steepness
		//Vector4 _GSpeed,													// speed
		//Vector4 _GDirectionAB,												// direction # 1, 2
		//Vector4 _GDirectionCD												// direction # 3, 4


		Vector4 amplitude, Vector4 frequency, Vector4 steepness, 
		Vector4 speed, Vector4 directionAB, Vector4 directionCD 

		)
	{
		Vector3 Offsets = Vector3.zero;

			float Intensity1 = WaterMaterial.GetFloat("_GerstnerIntensity1");
			//float Intensity2 = WaterMaterial.GetFloat("_GerstnerIntensity2");

			Vector4 _GerstnerIntensities = WaterMaterial.GetVector("_GerstnerIntensities");
			Vector4 _Gerstnerfactors2 = WaterMaterial.GetVector("_Gerstnerfactors2");
			Vector4 _Gerstnerfactors = WaterMaterial.GetVector("_Gerstnerfactors");
			Vector4 _GerstnerfactorsSteep = WaterMaterial.GetVector("_GerstnerfactorsSteep");
			Vector4 _GerstnerfactorsDir = WaterMaterial.GetVector("_GerstnerfactorsDir");

		Vector2 tileableVtx_xz = new Vector2 (tileableVtx.x,tileableVtx.y);

			Offsets = GerstnerOffset4 (tileableVtx_xz, steepness, amplitude, frequency, speed, directionAB, directionCD)
				+ Intensity1*0.5f*GerstnerOffset4(tileableVtx_xz+new Vector2(0,0), steepness/36, amplitude*2.2f*4, frequency*0.002f, speed*0.1f, directionAB-new Vector4(11.1f,0,10.2f,0), directionCD+new Vector4(111.1f,0,10.2f,0))		
					//+ Intensity2*GerstnerOffset4(tileableVtx_xz+new Vector2(1,2), steepness/13, amplitude*0.5f, frequency*3, speed*0.4f, directionAB-new Vector4(111.1f,0,0.2f,0), directionCD+new Vector4(111.1f,110,0.2f,0));

			//+ _GerstnerIntensity1*0.5*GerstnerOffset4(tileableVtx.xz+float3(1,2,3), steepness/36, amplitude*2.2*4, frequency*0.002, speed*0.1, directionAB-float4(11.1,0,10.2,0), directionCD+float4(111.1,0,10.2,0))		
			//	+ _GerstnerIntensity2*GerstnerOffset4(tileableVtx.xz+float3(1,2,3), steepness/13, amplitude*0.5, frequency*3, speed*0.4, directionAB-float4(111.1,0,0.2,0), directionCD+float4(111.1,110,0.2,0));
		
//		nrml = GerstnerNormal4(tileableVtx.xz + offs.xz, amplitude, frequency, speed, directionAB, directionCD)			
//			+ _GerstnerIntensity1*0.5*GerstnerNormal4(tileableVtx.xz+float3(1,2,3) + offs.xz, amplitude*4.2*4, frequency*0.002, speed*0.1, directionAB-float4(11.1,0,10.2,0), directionCD+float4(111.1,0,10.2,0)) 			
//				+ _GerstnerIntensity2*GerstnerNormal4(tileableVtx.xz+float3(1,2,3) + offs.xz, amplitude*0.6, frequency*3, speed*0.4, directionAB-float4(1.1,0,0.2,0), directionCD+float4(111.1,110,0.2,0));			

					+ _GerstnerIntensities.x*0.1f*tileableVtx.x*GerstnerOffset4(tileableVtx, _GerstnerfactorsSteep.x*steepness, - _Gerstnerfactors2.x*amplitude, _Gerstnerfactors.x*frequency/2, -0.2f*speed, -0.2f*new Vector4(_GerstnerfactorsDir.x*directionAB.x,directionAB.y,directionAB.z,directionAB.w), 0.3f*directionCD) 
					+ _GerstnerIntensities.y*0.15f*tileableVtx.y*GerstnerOffset4(tileableVtx, _GerstnerfactorsSteep.y*steepness, - _Gerstnerfactors2.y*0.1f*amplitude, _Gerstnerfactors.y*0.1f*frequency/1.2f, -speed, -new Vector4(directionAB.x,_GerstnerfactorsDir.y*directionAB.y,directionAB.z,directionAB.w), 0.4f*directionCD)
					+ _GerstnerIntensities.z*0.05f*PosY*GerstnerOffset4(tileableVtx, _GerstnerfactorsSteep.z*steepness, - _Gerstnerfactors2.z*0.1f*amplitude, _Gerstnerfactors.z*0.2f*frequency/0.9f, -0.5f*speed, -0.5f*new Vector4(_GerstnerfactorsDir.z*directionAB.x,directionAB.y,directionAB.z,directionAB.w), directionCD)
					;

		return Offsets;

	}

	Vector3 GerstnerOffset4 (Vector2 xzVtx, Vector4 steepness, Vector4 amp, Vector4 freq, Vector4 speed, Vector4 dirAB, Vector4 dirCD) 
	{
		Vector3 offsets = Vector3.zero;

		//Vector4 steepness_xxyy = new Vector4(steepness.x,steepness.x,steepness.y,steepness.y);
		//Vector4 steepness_zzww = new Vector4(steepness.z,steepness.z,steepness.w,steepness.w);
		//Vector4 amp_xxyy = new Vector4(amp.x,amp.x,amp.y,amp.y);
		//Vector4 amp_zzww = new Vector4(amp.z,amp.z,amp.w,amp.w);
		//Vector4 dirAB_xyzw = new Vector4(dirAB.x,dirAB.y,dirAB.z,dirAB.w);
		//Vector4 dirCD_xyzw = new Vector4(dirCD.x,dirCD.y,dirCD.z,dirCD.w);

		//Vector4 freq_xyzw = new Vector4(freq.x,freq.y,freq.z,freq.w);

		Vector4 dirAB_xy = new Vector2(dirAB.x,dirAB.y);
		Vector4 dirAB_zw = new Vector2(dirAB.z,dirAB.w);
		Vector4 dirCD_xy = new Vector2(dirCD.x, dirCD.y);//HDRP - v4.9.8
        Vector4 dirCD_zw = new Vector2(dirCD.z, dirCD.w);//HDRP - v4.9.8

        //Vector4 AB = steepness.xxyy * amp.xxyy * dirAB.xyzw;
        //Vector4 CD = steepness.zzww * amp.zzww * dirCD.xyzw;
        Vector4 AB = new Vector4 (steepness.x*amp.x*dirAB.x,  steepness.x*amp.x*dirAB.y,  steepness.y*amp.y*dirAB.z,  steepness.y*amp.y*dirAB.w);  //steepness_xxyy * amp_xxyy * dirAB_xyzw;
		Vector4 CD = new Vector4 (steepness.z*amp.z*dirCD.x,  steepness.z*amp.z*dirCD.y,  steepness.w*amp.w*dirCD.z,  steepness.w*amp.w*dirCD.w);  //steepness_zzww * amp_zzww * dirCD_xyzw;

		Vector4 tempA = new Vector4 (Vector2.Dot (dirAB_xy, xzVtx), Vector2.Dot (dirAB_zw, xzVtx), Vector2.Dot (dirCD_xy, xzVtx), Vector2.Dot (dirCD_zw, xzVtx));
		Vector4 dotABCD = new Vector4(freq.x*tempA.x,freq.y*tempA.y,freq.z*tempA.z,freq.w*tempA.w);
		//Vector4 TIME = _Time.yyyy * speed;
		//Vector4 TIME = Time.fixedTime * speed;
        //HDRP - v4.9.8
        Vector4 shaderTime = Shader.GetGlobalVector("_Time");
        Vector4 TIME = shaderTime.y * speed;

        Vector4 COS = new Vector4(Mathf.Cos(dotABCD.x + TIME.x),Mathf.Cos(dotABCD.y + TIME.y),Mathf.Cos(dotABCD.z + TIME.z),Mathf.Cos(dotABCD.w + TIME.w));
		Vector4 SIN = new Vector4(Mathf.Sin(dotABCD.x + TIME.x),Mathf.Sin(dotABCD.y + TIME.y),Mathf.Sin(dotABCD.z + TIME.z),Mathf.Sin(dotABCD.w + TIME.w));
		
//		offsets.x = Vector4.Dot(COS, Vector4(AB.xz, CD.xz));
//		offsets.z = Vector4.Dot(COS, Vector4(AB.yw, CD.yw));
		offsets.x = Vector4.Dot(COS, new Vector4(AB.x,AB.z, CD.x,CD.z));
		offsets.z = Vector4.Dot(COS, new Vector4(AB.y,AB.w, CD.y,CD.w));
		offsets.y = Vector4.Dot(SIN, amp);
		
		return offsets;			
	}	


}
}