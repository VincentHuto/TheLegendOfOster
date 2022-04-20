using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {
public class SM_CloudRot : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		Prender = this.gameObject.GetComponent<ParticleRenderer> ();
//
//		if (Prender != null) {
//			Prender.particleRenderMode = ParticleRenderMode.SortedBillboard;
//		} else {
//			Debug.Log("Please use the script with a particle renderer");
//		}
//		MainCam = Camera.main.transform;
//		ThisTransf = this.transform;
//
//			prev_cam_for = MainCam.forward;
	}

//	ParticleRenderer Prender;
//
//		Vector3 prev_cam_for;
//
//	bool sorted = false;
//	Transform MainCam;
//	Transform ThisTransf;
//
//	public float angle1=35;
//	public float angle2=18;
//		public float NearCloudDist = 50;
//		public float CloudBedToThisDist = 0;

		//bool has_sorted=false;

	// Update is called once per frame
	void Update () {

//		if(Prender == null){
//			Prender = this.gameObject.GetComponent<ParticleRenderer> ();
//		}
//
//			if (1 == 1) {
//
//				//if camera rotates > 180, do sort again
//				if (Vector3.Angle (prev_cam_for, MainCam.forward) > 1
//					& (MainCam.eulerAngles.z < 5 | MainCam.eulerAngles.z > 355)) {
//
//					prev_cam_for = MainCam.forward;
//					sorted = false;
//					//Debug.Log("resort");
//				}
//
//				if (Prender != null) {
//					float angle = Vector3.Angle (MainCam.forward, new Vector3 (MainCam.forward.x, 0, MainCam.forward.z));
//
//					float angleR = Vector3.Angle (MainCam.right, new Vector3 (MainCam.right.x, 0, MainCam.right.z));
//					//	Debug.Log (Vector3.Angle (MainCam.forward, new Vector3 (MainCam.forward.x, 0, MainCam.forward.z)));
//
//					if (!sorted) {
//						sorted = true;//sort at first
//						if (Prender.particleRenderMode != ParticleRenderMode.SortedBillboard) {
//							Prender.particleRenderMode = ParticleRenderMode.SortedBillboard;
//						}
//					} else {
//
//						if (Mathf.Abs (MainCam.position.y - (ThisTransf.position.y + CloudBedToThisDist)) < NearCloudDist | (angle > angle1 & angleR < angle2)) {
//							if (Prender.particleRenderMode != ParticleRenderMode.SortedBillboard) {
//								Prender.particleRenderMode = ParticleRenderMode.SortedBillboard;
//							}
//							//Debug.Log ("change");
//						} else {
//							if (Prender.particleRenderMode != ParticleRenderMode.VerticalBillboard) {
//								Prender.particleRenderMode = ParticleRenderMode.VerticalBillboard;
//							}
//						}
//		
//					}
//				}
//
//			}

	}
}
}
