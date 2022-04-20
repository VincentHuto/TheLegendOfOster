using UnityEngine;
using System.Collections;
namespace Artngame.SKYMASTER {
public class LightFlash : MonoBehaviour {



	// Use this for initialization
	void Start () {
		cur_time = Time.fixedTime;
		this_light = this.GetComponent<Light>();
	}

	public float delay = 1.5f;

	float cur_time;

	Light this_light;

	float start_emit_time;
	bool emits = false;
	public float emit_time = 1;

		public float speed = 1;

	PerlinSKYMASTER noise;
	// Update is called once per frame
	void Update () {

		if (noise == null)
		{noise = new PerlinSKYMASTER();}

			float timex = Time.time * speed * 0.1365143f;
			float timey = Time.time * speed * 1.21688f;
		float timez = Time.time * speed * 2.5564f;

		if(Time.fixedTime - cur_time > delay & !emits){
			start_emit_time = Time.fixedTime;
				emits = true;
			cur_time = Time.fixedTime;

		}

		if(emits){
			if(Time.fixedTime - start_emit_time <= emit_time){
//				Vector3 offset = new Vector3(noise.Noise(timex + his_light.intensity, timex + position.y, timex + position.z),
//					                             noise.Noise(timey + his_light.intensity, timey + position.y, timey + position.z),
//					                             noise.Noise(timez + his_light.intensity, timez + position.y, timez + position.z));
//				position += ((offset+(offset_bias1*i*0.01f)) * scale * ((float)i * oneOverZigs));

					this_light.intensity = Random.Range(1,3)* noise.Noise(timex + this_light.intensity, timey + this_light.intensity, timez + this_light.intensity);

			}else{
				emits = false;
				this_light.intensity = 0;
				start_emit_time = 0;
			}

		}

	}
}
}