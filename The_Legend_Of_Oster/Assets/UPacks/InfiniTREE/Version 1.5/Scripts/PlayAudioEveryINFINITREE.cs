using UnityEngine;
using System.Collections;

public class PlayAudioEveryINFINITREE : MonoBehaviour {

	// Use this for initialization
	void Start () {
		timer = Time.fixedTime;Source.Play();
	}
	float timer;
	public AudioSource Source;
	public float Delay = 1;
	public Animator AnimationAxe;

	public Animation AnimationAxeLegacy;
	// Update is called once per frame
	void Update () {

		if(Time.fixedTime - timer > Delay){
			if(Input.GetMouseButton(1)) {

				if(AnimationAxe!=null){
					AnimationAxe.Play("AXE ANIM");
				}

				//Debug.Log ("aaa");
				AnimationAxeLegacy.Play("AXE ANIM");
				Source.Play();
				timer = Time.fixedTime;
			}
		}
	}
}
