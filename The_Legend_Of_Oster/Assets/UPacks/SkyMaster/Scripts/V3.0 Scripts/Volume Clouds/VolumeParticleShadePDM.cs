using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class VolumeParticleShadePDM : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	public Light Sun;
	public Material Particle_Mat;
	// Update is called once per frame
	void Update () {
		if(Sun != null & Particle_Mat !=null){
			Particle_Mat.SetVector("_SunColor",Sun.color);
			Particle_Mat.SetFloat("_SunLightIntensity",Sun.intensity);
		}
	}
}

