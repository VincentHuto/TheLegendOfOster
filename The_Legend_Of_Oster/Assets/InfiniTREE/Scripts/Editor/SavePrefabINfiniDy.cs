using System.Collections;	
using UnityEditor;
using UnityEngine;
using System.IO;
using Artngame.INfiniDy;
	
class SavePrefabINfiniDy : MonoBehaviour {

	const string menuName = "Window/INfiniDy/INfiniTREE/Create Tree Prefab From Selected";
		
		[MenuItem(menuName)]
		static void CreatePrefabMenu ()
		{
			
			ControlCombineChildrenINfiniDy Combiner = Selection.activeGameObject.GetComponent(typeof(ControlCombineChildrenINfiniDy)) as ControlCombineChildrenINfiniDy;
			if(Combiner == null){
				Debug.Log ("Please select a combiner that contains only one tree");return;
			}
			if(Combiner.Added_items.Count != 1){
				Debug.Log ("Please select a combiner that contains only one tree");return;
			}

			//check if directory doesn't exit
			if(!Directory.Exists(Combiner.Save_Dir))
			{
				//if it doesn't, create it
				Directory.CreateDirectory(Combiner.Save_Dir);
			}

		string File_name = "Assets/InfiniTREE/savedMesh/" + Combiner.Save_Name + ".prefab";
		if (System.IO.File.Exists(File_name))
		{
			Debug.Log ("Please select a different name for this tree.");return;
		}

			var go = Selection.activeGameObject;
			Component[] filters  = go.GetComponentsInChildren(typeof(MeshFilter));	//gather opened renderers, make sure tree is not decombined "Combined mesh64" or "Combined mesh"

			bool found_non_proper = false;

			for(int i=0;i<filters.Length;i++){
				Debug.Log (filters[i].gameObject.name);
				if(filters[i].gameObject.name != "Combined mesh64" & filters[i].gameObject.name != "Combined mesh"){
					//if not cobmined mesh, must be deactivated
					if(filters[i].GetComponent<Renderer>().enabled){
						found_non_proper = true;
					}
				}
			}

			if(found_non_proper){
				Debug.Log ("Use only combined trees");
				return;
			}

			
			GameObject SaveObj = new GameObject(Combiner.Save_Name);


			for(int i=0;i<filters.Length;i++){
				if(filters[i].GetComponent<Renderer>().enabled & (filters[i].gameObject.name != "Combined mesh64" | filters[i].gameObject.name != "Combined mesh")){
					MeshFilter filter = (MeshFilter)filters[i];
					Mesh m1 = filter.mesh;
					AssetDatabase.CreateAsset(m1, "Assets/InfiniTREE/savedMesh/" + Combiner.Save_Name +"_M" +i+ ".asset");

					filter.gameObject.transform.parent = SaveObj.transform;
					filter.gameObject.transform.position = -Combiner.Added_items[0].transform.position;
				}
			}

			var prefab = PrefabUtility.CreateEmptyPrefab("Assets/InfiniTREE/savedMesh/" + Combiner.Save_Name + ".prefab");
			
			PrefabUtility.ReplacePrefab(SaveObj,prefab);
			AssetDatabase.Refresh();

			//Restore parenting
			for(int i=0;i<filters.Length;i++){
				if(filters[i].GetComponent<Renderer>().enabled & (filters[i].gameObject.name != "Combined mesh64" | filters[i].gameObject.name != "Combined mesh")){
					MeshFilter filter = (MeshFilter)filters[i];
					
					filter.gameObject.transform.parent = Selection.activeGameObject.transform;
				}
			}

		}
		
		[MenuItem(menuName, true)]
		static bool ValidateCreatePrefabMenu ()
		{
			return Selection.activeGameObject != null;
		}
}
