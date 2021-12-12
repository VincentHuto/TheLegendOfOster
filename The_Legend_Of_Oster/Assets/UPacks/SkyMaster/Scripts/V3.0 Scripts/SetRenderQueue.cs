using UnityEngine;
using System.Collections;

namespace Artngame.SKYMASTER{

[AddComponentMenu("Effects/SetRenderQueue")]
//[RequireComponent(typeof(Renderer))] //v3.4.6
public class SetRenderQueue : MonoBehaviour {
	public int queue = 1;
	
	public int[] queues;

	public bool Resort = false;
	public int SortingLayerOrder = 0;
	string sortingLayerName = "Default";
	public int sortingLayerID = 0;
	public bool is_cloud=true;

	Transform This_tranf;
	public bool above_clouds = false;

	//v3.4.1
	public int SortingLayerCloudsGround = 0;
	public int SortingLayerCloudsAbove = 1;

	public bool UseSortingLayers = true;

	protected void Start() {
		if (!GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial || queues == null)
			return;
		GetComponent<Renderer>().sharedMaterial.renderQueue = queue;
		for (int i = 0; i < queues.Length && i < GetComponent<Renderer>().sharedMaterials.Length; i++)
			GetComponent<Renderer>().sharedMaterials[i].renderQueue = queues[i];
	
	
		//v3.0
		if (UseSortingLayers) {
			//Debug.Log (GetComponent<Renderer> ());
			if (Resort) {		
				GetComponent<Renderer> ().sortingLayerName = sortingLayerName;
//				GetComponent<Renderer> ().sortingLayerID = sortingLayerID;
				GetComponent<Renderer> ().sortingOrder = SortingLayerOrder;
			} else {
				GetComponent<Renderer> ().sortingLayerName = "Default";
//				GetComponent<Renderer> ().sortingLayerID = 0;
				GetComponent<Renderer> ().sortingOrder = 0;
			}
		}
		This_tranf = this.transform;
	}

	void Update(){
		if (Resort) {		
			GetComponent<Renderer>().sortingLayerName = sortingLayerName;
//			GetComponent<Renderer> ().sortingLayerID = sortingLayerID;
			GetComponent<Renderer> ().sortingOrder = SortingLayerOrder;
			Resort = false;
		}
		if (UseSortingLayers) {
			if (is_cloud) {//toggle between 0 and 1
				if (Camera.main != null && Camera.main.transform.position.y > This_tranf.position.y) {
					//GetComponent<Renderer>().sortingLayerName = sortingLayerName;
					//GetComponent<Renderer> ().sortingLayerID = sortingLayerID;
					if (!above_clouds) {
						GetComponent<Renderer> ().sortingOrder = SortingLayerCloudsAbove;//1; //v3.4.1
						above_clouds = true;
					}
				} else {
					if (above_clouds) {
						GetComponent<Renderer> ().sortingOrder = SortingLayerCloudsGround;//0; //v3.4.1
						above_clouds = false;
					}
				}
			}
		}
	}
	
}

}