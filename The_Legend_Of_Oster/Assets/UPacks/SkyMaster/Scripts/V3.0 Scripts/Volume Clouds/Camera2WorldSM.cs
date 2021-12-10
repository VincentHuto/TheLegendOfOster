using UnityEngine;
using System.Collections;

public class Camera2WorldSM : MonoBehaviour {

	// Use this for initialization
	void OnPreCull () {
		Shader.SetGlobalMatrix ("_Camera2World", this.gameObject.GetComponent<Camera> ().cameraToWorldMatrix);

	}
	

}
