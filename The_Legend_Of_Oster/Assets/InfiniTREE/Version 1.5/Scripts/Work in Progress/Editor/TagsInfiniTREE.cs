using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad]
public class TagsInfiniTREE{
	
	//STARTUP
	static TagsInfiniTREE()
	{
		CreateTag("Hero");
		CreateTag("INfiniDyForest");
		CreateTag("INfiniDyForestInter");
		CreateTag("INfiniDyTree");
		CreateTag("INfiniDyTreeRoot");
		CreateTag("Axe");
	}
	
	static bool SM_layer_already_changed = false; 

	static void CreateTag(string tag_name){
		
		bool changed = false;

		bool SM_layer_changed = false;
		
		if(!SM_layer_already_changed){
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			
			SerializedProperty it1 = tagManager.GetIterator();
			bool showChildren = true;

			while (it1.NextVisible(showChildren))
			{
				if(it1.propertyType == SerializedPropertyType.String){
					if(it1.stringValue == tag_name & it1.name.Contains("data")){
						SM_layer_changed=true;
						//Debug.Log (it1.stringValue);  
						break; 
					}
				}
			}			
			
			SerializedProperty it = tagManager.GetIterator();    
			showChildren = true;	
			
			while (it.NextVisible(showChildren))
			{
				
				//set your tags here
				if (!SM_layer_changed  )
				{
					if(it.propertyType == SerializedPropertyType.String){
						if(it.stringValue == "" & it.name.Contains("data")){
							it.stringValue = tag_name;
							changed = true;
							SM_layer_changed=true;
							//Debug.Log ("222");
							break;
						}
					}					
				}				
			}			
			
			if(changed){
				tagManager.ApplyModifiedProperties();
				//changed = false;
			}else{
				if(SM_layer_already_changed){					
				}else{
					if(!SM_layer_changed){
						Debug.Log ("Sky Master warning - Please add the '" + tag_name + "' tag");
					}
				}
			}
		}
	}

	//creates a new layer
	static void CreateLayer(){
		
		bool changed = false;
		
		if(!SM_layer_already_changed){
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			
			SerializedProperty it1 = tagManager.GetIterator();
			bool showChildren = true;	
			
			while (it1.NextVisible(showChildren))
			{
				if(it1.propertyType == SerializedPropertyType.String){
					if(it1.stringValue == "SkyMaster" & it1.name.Contains("User Layer")){
						SM_layer_already_changed=true;
						//Debug.Log (it.stringValue);  
						break; 
					}
				}
			}
			
			
			SerializedProperty it = tagManager.GetIterator();    
			showChildren = true;	
			
			while (it.NextVisible(showChildren))
			{
				
				//set your tags here
				if (!SM_layer_already_changed  )
				{
					if(it.propertyType == SerializedPropertyType.String){
						if(it.stringValue == "" & it.name.Contains("User Layer")){
							it.stringValue = "SkyMaster";
							changed = true;
							SM_layer_already_changed=true;
							//Debug.Log ("222");
							break;
						}
					}
					
				}
				
			}
			
			
			if(changed){
				tagManager.ApplyModifiedProperties();
				//changed = false;
			}else{
				if(SM_layer_already_changed){
					
				}else{
					Debug.Log ("Sky Master warning - Please add the 'SkyMaster' layer to use the reflection probe functionality with dome sky");
				}
			}
		}
	}
}