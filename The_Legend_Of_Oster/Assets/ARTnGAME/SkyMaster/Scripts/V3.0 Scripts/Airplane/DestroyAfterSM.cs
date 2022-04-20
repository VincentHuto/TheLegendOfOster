using UnityEngine;
using System.Collections;

public class DestroyAfterSM : MonoBehaviour {

	public float stayTime = 10;
	float currenttime;
	bool gameStartItem=false;//flag if item is at game start, so it is not destroyed

	// Use this for initialization
	void Start () {
		if (Time.fixedTime < 1) {
			gameStartItem=true;
		}
		currenttime = Time.fixedTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.fixedTime - currenttime > stayTime & !gameStartItem) {
			Destroy(this.gameObject);
		}
	}
}
