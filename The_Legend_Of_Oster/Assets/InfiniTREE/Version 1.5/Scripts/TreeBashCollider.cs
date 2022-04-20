using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artngame.INfiniDy;

public class TreeBashCollider : MonoBehaviour {

	public INfiniDyForest TreeHandler;
	public GameObject Chips;
	// Use this for initialization
	void Start () {	
	}

	List<GameObject> To_erase = new List<GameObject>();
	List<float> To_erase_timing = new List<float>();

	bool done=false;
	public AudioClip Crash;
	public AudioSource Basher;
	public bool played = false;

	public float Remove_after = 3;

	void OnCollisionEnter(Collision collision){
	
			if(Chips!=null & !done){
				done = true;
				GameObject AA = (GameObject)Instantiate(Chips,transform.position,transform.rotation);
				AA.SetActive(true);

				AA.name = "ChipsBash";
				AA.transform.parent = this.gameObject.transform;

				To_erase.Add(AA);
				To_erase_timing.Add(Time.fixedTime);
			}

		if(Basher!=null){
			if(Crash !=null & !played){
				Basher.clip = Crash;
				Basher.Play();
				played=true;
			}
		}
	}

	// Update is called once per frame
	void Update () {	

		for(int i = To_erase.Count - 1;i>=0;i--){
			if(Time.fixedTime - To_erase_timing[i] > Remove_after){
				To_erase.RemoveAt(i);
				To_erase_timing.RemoveAt(i);
			}
		}

	}
}
