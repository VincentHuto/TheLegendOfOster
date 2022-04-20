using UnityEngine;
 
namespace Artngame.SKYMASTER {

 [ExecuteInEditMode]
 public class TerrainDepthSM : MonoBehaviour
 {
    [Range(0f, 15f)]//5
    public float heightFactor = 0.5f;
	[Range(0f, 1.1f)]//0.1
	public float heightCutoff = 0.1f;
	[Range(0.01f, 1.15f)]//0.15
	public float contrast = 0.1f;


	public Shader DepthShader;
	Material material;
 
     private void Start ()
     {
		if (DepthShader == null) {
			DepthShader = Shader.Find ("SkyMaster/TerrainDepthSM");
		}

		if (material == null)
		{
			material = new Material(DepthShader);
			material.hideFlags = HideFlags.HideAndDontSave;
		}

		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

        //if (!SystemInfo.supportsImageEffects)
        //{
        //     Debug.Log("Image effects not available");
        //     enabled = false;
        //     return;
        //}
		if (!DepthShader.isSupported || DepthShader == null)
        {
             enabled = false;
			 Debug.Log("DepthShader " + DepthShader.name + " not available");
             return;
        }          
     }
     
     private void OnDisable()
     {
		if (material != null) {
			DestroyImmediate (material);
		}
     }
     
     private void OnRenderImage(RenderTexture source, RenderTexture destination)
     {
		if (material == null)
		{
			material = new Material(DepthShader);
			material.hideFlags = HideFlags.HideAndDontSave;
		}

		if (DepthShader != null)
         {
				material.SetFloat("_heightFactor", heightFactor);
				material.SetFloat("_heightCutoff", heightCutoff);
				material.SetFloat("_contrast", contrast);

			Graphics.Blit(source, destination, material);
         }
         else
         {
			Graphics.Blit(source, destination);
         }
     }
 }

}