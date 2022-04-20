using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {

public class RecalcBOUNDS_SM : MonoBehaviour {

	void Start () {
		 AA = this.GetComponent(typeof(MeshFilter)) as MeshFilter;
		this_transform = transform;
	}
	MeshFilter AA;
	Transform this_transform;

	void Update () {		

		if(Vector3.Distance(Camera.main.transform.position, this_transform.position) < 1000 ){

			Vector3 camPosition =Camera.main.transform.position;
			Vector3 normCamForward = Vector3.Normalize(Camera.main.transform.forward);
			float boundsDistance = (Camera.main.farClipPlane - Camera.main.nearClipPlane) / 2 + Camera.main.nearClipPlane;
			Vector3 boundsTarget = camPosition + (normCamForward * boundsDistance);			

			Vector3 realtiveBoundsTarget = this.transform.InverseTransformPoint(boundsTarget);			

			Mesh mesh = AA.mesh;
			mesh.bounds = new Bounds(realtiveBoundsTarget, Vector3.one);
		}
	}

	private void shiftMesh(){ 
		Mesh mesh = AA.sharedMesh; 
		Vector3[] vertices = mesh.vertices; 
		mesh.vertices = vertices; 
		mesh.RecalculateBounds(); 
	} 
  }
}