using UnityEngine;
using System.Collections;


namespace Artngame.GIPROXY {

public class ProceduralNoiseGIPROXY : MonoBehaviour {

	public float scale = 1.0f;
	public float speed = 1.0f;
	public bool recalculateNormals = false;
	private Vector3[] baseVertices;
		private PerlinGIPROXY  noise;

	public bool Colorize=false;
	public float Color_cycle_speed = 0.1f;

	MeshFilter meshFilter;

	void Start () {

		meshFilter = GetComponent("MeshFilter") as MeshFilter;

			noise = new PerlinGIPROXY ();

		register_time = Time.fixedTime;
	}

	public float CHANGE_FACTOR=0;

	private float register_time;
	public bool Deactivate_after=false;
	public float seconds=1f;

	void Update () {

		if(!Deactivate_after | (Deactivate_after & ((Time.fixedTime-register_time) < seconds) )){
		
		if(scale!=0){
		Mesh mesh = meshFilter.mesh;
		
		if (baseVertices == null)
		{baseVertices = mesh.vertices;}
		
		Vector3[] vertices = new Vector3[baseVertices.Length];
		
		float timex = Time.time * speed + 0.1365143f * CHANGE_FACTOR;
		
		float timey = Time.time * speed + 1.21688f* CHANGE_FACTOR;
		
		float timez = Time.time * speed + 2.5564f* CHANGE_FACTOR;
		
		for (int i=0;i<vertices.Length;i++)
			
		{
			
			Vector3 vertex = baseVertices[i];
			
			vertex.x += noise.Noise(timex + vertex.x, timex + vertex.y, timex + vertex.z) * scale;
			
			vertex.y += noise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * scale;
			
			vertex.z += noise.Noise(timez + vertex.x, timez + vertex.y, timez + vertex.z) * scale;
			
			vertices[i] = vertex;
			
		}

		mesh.vertices = vertices;

			if (recalculateNormals) {
				
				mesh.RecalculateNormals();
				
			}
			
			
			mesh.RecalculateBounds();

		}
	
		if(Colorize){
			int RAND_1 = Random.Range(0,3);
			
			if(RAND_1==0){
				this.gameObject.GetComponent<Renderer>().material.color = Color.Lerp (this.gameObject.GetComponent<Renderer>().material.color, Color.blue,Color_cycle_speed);
			}else if(RAND_1 ==1){
				this.gameObject.GetComponent<Renderer>().material.color = Color.Lerp (this.gameObject.GetComponent<Renderer>().material.color, Color.red,Color_cycle_speed);
			}else if(RAND_1 ==2){
				this.gameObject.GetComponent<Renderer>().material.color = Color.Lerp (this.gameObject.GetComponent<Renderer>().material.color, Color.magenta,Color_cycle_speed);
			}
		}


	}
		
	
	}
}

}