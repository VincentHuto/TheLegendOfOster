using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER {
public class RadialOceanSM : MonoBehaviour {

		public Material oceanMaterial;
		public int oceanDetailx = 64;
		public int oceanDetaily = 64;
		GameObject RadialOcean;	
		public float WaveHeightFactor = 2;
				
		void Start()
		{
			CreateOcean();
		}
		
		public void CreateOcean()
		{
			//create holder object
			RadialOcean = new GameObject("RadialOcean_SkyMaster");

			//camera based stretch
			float Clip_far_Camera = Camera.main.farClipPlane;
			RadialOcean.transform.localScale = new Vector3(Clip_far_Camera,WaveHeightFactor,Clip_far_Camera);

			//create mesh
			int x_resolution = oceanDetailx;
			int y_resolution = oceanDetaily;
			float XreS = x_resolution - 1;
			float YreS = y_resolution - 1;
			Vector3[] vertices = new Vector3[x_resolution*y_resolution];
			Vector3[] normals  = new Vector3[x_resolution*y_resolution];				
			Vector2[] uvs = new Vector2[vertices.Length]; 
			
			for (int x = 0; x < x_resolution; x++)
			{
				for (int y = 0; y < y_resolution; y++)
				{	
					float radius = (float)(x/(XreS));
					float Fre = 2* Mathf.PI * (float)(y / YreS);
					vertices[x + y * x_resolution] = new Vector3(Mathf.Cos(Fre)*radius, 0, Mathf.Sin(Fre)*radius);
					normals [x + y * x_resolution] = Vector3.up;
				}
			}
			
			int[] triangles = new int[6*x_resolution*y_resolution];
			
			int counter = 0;
			int uv_counter = 0;
			for (int x = 0; x < XreS; x++)
			{
				for (int y = 0; y < YreS; y++)
				{
					float offset1 = y * x_resolution;
					triangles[counter++] = (int)(x + offset1+0);
					triangles[counter++] = (int)(x + offset1+x_resolution);
					triangles[counter++] = (int)(x + offset1+1);								
					triangles[counter++] = (int)(x + offset1+x_resolution);
					triangles[counter++] = (int)(x + offset1+x_resolution+1);
					triangles[counter++] = (int)(x + offset1+1);						
					uvs[uv_counter]  = new Vector2((float)(x / x_resolution), (float)(y / y_resolution));  
					uv_counter++;
				}
			}
			
			Mesh mesh = new Mesh();				
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs; 				
			mesh.triangles = triangles;
			Mesh OceanMesh = mesh;
			//assign mesh
			RadialOcean.AddComponent<MeshFilter>();			
			RadialOcean.GetComponent<MeshFilter>().mesh = OceanMesh;
			RadialOcean.AddComponent<MeshRenderer>();
			RadialOcean.GetComponent<Renderer>().material = oceanMaterial;		
		}	
		
				
	}
}