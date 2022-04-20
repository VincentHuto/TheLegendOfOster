using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {
public class RiverHandlerSM : MonoBehaviour {

	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer>().material;
		mat.shader.maximumLOD = 600;
	}

	//MATERIAL
	Material mat;

	//RIVER SRCOLLING
	public float scrollSpeed = 0.1f;

	//RIVER LIGHTING
	public Transform lightDir;

	// Update is called once per frame
	void Update () {

		//RIVER SRCOLLING
		// Scroll main texture based on time
		float offset = Time.time * scrollSpeed;
		//renderer.material.SetTextureOffset ("_LightMap", Vector2(offset/20, offset));

		mat.SetTextureOffset ("_MainTex", new Vector2(offset*0.5f, offset*1));
		mat.SetTextureOffset ("_HeightTex", new Vector2(offset/2, offset));
		mat.SetTextureOffset ("_FoamTex", new Vector2(offset/4, offset*1));

		//RIVER LIGHTING
		//Material mat = GetComponent<Renderer>().material;
		//mat.shader.maximumLOD = GameQualitySettings.water ? 600 : 300;
		if (lightDir) {
			mat.SetVector ("_WorldLightDir", lightDir.forward);
		} else {
			mat.SetVector ("_WorldLightDir", new Vector3 (0.7f, 0.7f, 0.0f));
		}

	}
}
}
