using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artngame.INfiniDy;

public class INfiniDyTreeDEMO : MonoBehaviour {

	void Start () {
		last_click = Time.fixedTime;
	}

	public GameObject INfiniDyTree;
	public GameObject INfiniDyTree1;
	public GameObject INfiniDyTree2;
	public GameObject INfiniDyTree3;

	public GameObject INfiniDyTree4;
	public GameObject INfiniDyTree5;
	public GameObject INfiniDyTree6;
	public GameObject INfiniDyTree7;

	public GameObject INfiniDyTree8;
	public GameObject INfiniDyTree9;

	public List<GameObject> INfiniDyTrees;

	int current_forest_type=1;


	bool alter = false;
	int current_tree_id=0;

	public int max_trees=15;
	int trees=0;
	float last_click;
	public float click_time_min = 1.0f;

	public bool Move_to_terrain=true;

	void Update () {
		if(trees < max_trees & (Time.fixedTime - last_click > click_time_min)){

			if(Input.GetMouseButtonDown(0)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit=new RaycastHit();
				if(Physics.Raycast(ray,out hit)){

					Vector3 placement = hit.point;

					if(Move_to_terrain){
						if(Terrain.activeTerrain !=null){
							float find_y = Terrain.activeTerrain.SampleHeight(new Vector3(placement.x,0,placement.z))+ Terrain.activeTerrain.transform.position.y;
							if(placement.y > find_y){
								placement.y = find_y;
							}
						}
					}
					//Instantiate(INfiniDyTrees[current_tree_id],hit.point,Quaternion.identity);
					GameObject TREE = (GameObject)Instantiate(INfiniDyTrees[current_tree_id],placement,Quaternion.identity);

					INfiniDyForest Tree_script = TREE.GetComponentInChildren(typeof(INfiniDyForest)) as INfiniDyForest;

					if(Tree_script !=null){
						if(Tree_script.Rot_toward_normal & !Tree_script.Chop){
							Tree_script.max_rot_time = 1;
							Tree_script.Rot_speed = 1f;
							float Pow = 3;
							Tree_script.direction_normal = new Vector3(Mathf.Pow(hit.normal.x,Pow),Mathf.Pow(hit.normal.y,Pow),Mathf.Pow(hit.normal.z,Pow))*1000;
						}
					}

					trees++;
					last_click = Time.fixedTime;
					if(alter){
						alter = false;
					}else{
						alter = true;
					}
				}
			}

		}
	}

	void OnGUI(){

		GUI.TextArea(new Rect(10,10,140,20),"Trees ("+trees+") out of "+max_trees);

		string forest_type ="European";

		if(current_forest_type == 2){
			forest_type ="Pines";
		}

		if(current_forest_type == 3){
			forest_type ="Redwood";
		}





		if(GUI.Button(new Rect(10+140,10,100,20),"Maple Tree")){
			current_tree_id = 0;
		}
		if(GUI.Button(new Rect(10+140+100*1,10,100,20),"Maple(Roots)")){
			current_tree_id = 1;
		}
		if(GUI.Button(new Rect(10+140+100*2,10,100,20),"Snow Tree")){
			current_tree_id = 2;
		}
		if(GUI.Button(new Rect(10+140+100*3,10,100,20),"Pine Tree")){
			current_tree_id = 3;
		}
		if(GUI.Button(new Rect(10+140+100*4,10,100,20),"Thorn Bush")){
			current_tree_id = 4;
		}
		if(GUI.Button(new Rect(10+140+100*5,10,100,20),"Thorns")){
			current_tree_id = 5;
		}


		forest_type ="Maple Tree";
		if(current_tree_id == 0){
			forest_type ="Maple Tree";
		}
		if(current_tree_id == 1){
			forest_type ="Maple(Roots)";
		}
		if(current_tree_id == 2){
			forest_type ="Snow Tree";
		}
		if(current_tree_id == 3){
			forest_type ="Pine Tree";
		}
		if(current_tree_id == 4){
			forest_type ="Thorn Bush";
		}
		if(current_tree_id == 5){
			forest_type ="Thorns";
		}
		GUI.TextArea(new Rect(10,10+20,140,20),"Current: "+forest_type);

	}
}
