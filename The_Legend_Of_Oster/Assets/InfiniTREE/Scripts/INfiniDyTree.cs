using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Artngame.INfiniDy {
	public class INfiniDyTree
	{
		public int Level;
		public List<INfiniDyTree> Leaves = new List<INfiniDyTree>();
		GameObject TreeHolder;
		public GameObject INfiniDyForestOBJ;
		GameObject Instances;	
		public INfiniDyForest INfiniDyForestC;
		public bool is_root = false;
		List<INfiniDyTree> Branch_List;
		
		public float current_time;
		public float interval = 0.5f;
		public int Height_Level;
		
		void makeBranches()
		{
			float lenA  = INfiniDyForestC.lower_length_by * Instances.transform.localScale.y;
			float radiA = INfiniDyForestC.lower_radi_by   * Instances.transform.localScale.x;
			
			int count_to=1;
			if(Level == 0){
				count_to = INfiniDyForestC.Height_levels;
			}
			Transform previous = TreeHolder.transform;
			for(int k=0;k<count_to;k++){
				
				//reduce radius by height
				if(!INfiniDyForestC.Use_height){
					Height_Level = k;
				}else{
					if(Level == 0 & INfiniDyForestC.Height_levels > 1){
						Height_Level = k;
					}
				}
				
				
				float Radi_min = INfiniDyForestC.Min_Max_Radi.x;
				if(Height_Level>0){
					Radi_min = Radi_min *(0.99f-(Height_Level*INfiniDyForestC.Height_reduce));				
				}
				
				if(radiA < Radi_min){ 
					
				}else{	
					
					if(Height_Level>0){
						lenA  = lenA *(0.99f-(Height_Level*INfiniDyForestC.Height_reduce));
						radiA = radiA*(0.99f-(Height_Level*INfiniDyForestC.Height_reduce));
					}
					
					Branch_List = new List<INfiniDyTree>();
					
					GameObject Instance = new GameObject();	
					
					//parent to height segments if height > 0 
					if(!INfiniDyForestC.Decouple_from_bark){
						if(Height_Level == 0){
							Instance.transform.parent = previous;  
							previous = Instance.transform;
						}else{
							Instance.transform.parent = previous;
							previous = Instance.transform;
						}
					}else{
						Instance.transform.parent = previous;
					}
					
					Instance.transform.localPosition = Vector3.zero;
					Instance.transform.localEulerAngles= Vector3.zero;
					Instance.name = "Node";
					
					if(Height_Level > 0 & Level < 2){
					}
					
					Instance.transform.Translate(0,Instances.transform.localPosition.y*2,0);
					
					//CHANGES - Added the below if
					if(k > 0 & Level < 1){ 
						Instance.transform.position = Instance.transform.position + (INfiniDyForestC.Height_offset*1) + new Vector3(0,k*INfiniDyForestC.Height_separation,0);
					}
					
					if(Height_Level > 0 & Level < 1){
						Instance.transform.Rotate(INfiniDyForestC.Level1Decline*(Height_Level+1),0,INfiniDyForestC.Level1Decline*(Height_Level+1));
					}
					
					int child_count = (int)(Random.Range(INfiniDyForestC.Min_Max_Branching.x,INfiniDyForestC.Min_Max_Branching.y)/(1)); 
					
					//v1.2 - max branches per level
					if(Level > 0 & INfiniDyForestC.Max_branches_per_level.Count >= Level){
						child_count = (int)(Random.Range(INfiniDyForestC.Min_Max_Branching.x,INfiniDyForestC.Max_branches_per_level[Level-1])/(1)); 
					}
					
					for(int i = 0; i < child_count; i++)
					{
						INfiniDyTree childTree = new INfiniDyTree();
						childTree.INfiniDyForestC = INfiniDyForestC;
						Branch_List.Add(childTree);
						
						if(INfiniDyForestOBJ != null){
							childTree.INfiniDyForestC = INfiniDyForestOBJ.GetComponent(typeof(INfiniDyForest)) as INfiniDyForest;
						}
						childTree.Level = Level+1;
						childTree.Height_Level = Height_Level;
						childTree.generateTree(radiA,lenA,Color.blue,Vector3.zero+new Vector3(0,k*INfiniDyForestC.Height_separation,0), Instance);
					}
					
					
					if(Level > INfiniDyForestC.Leaf_level_min_max.x & Level < INfiniDyForestC.Leaf_level_min_max.y &  (!INfiniDyForestC.Use_height | (INfiniDyForestC.Use_height & Height_Level > INfiniDyForestC.Leaf_height_min_max.x & Height_Level < INfiniDyForestC.Leaf_height_min_max.y))  ){
						for (int j=0;j<INfiniDyForestC.Leaves_per_branch;j++){
							
							INfiniDyForestC.Registered_Leaves.Add (Instance.transform.position + Instance.transform.up.normalized*INfiniDyForestC.Leaf_dist_factor*(j+1)*Instance.transform.localScale.y); 
							
							INfiniDyForestC.Registered_Leaves_Rot.Add(Quaternion.Slerp(Instance.transform.rotation, Quaternion.AngleAxis(Random.Range(-270,270),Instance.transform.right.normalized+new Vector3(Random.Range(-270,270),Random.Range(-270,270),Random.Range(-270,270))),0.5f));
							INfiniDyForestC.Leaf_belongs_to_branch.Add(INfiniDyForestC.Registered_Brances.Count-1);
						}
					}
					if(Level > 1){
						
					}				
					
					//CHANGES 1 (1 to 0 !!!!)
					if(Level > 0){
						Instances.transform.localScale = Instances.transform.localScale/INfiniDyForestC.Growth_start_div;					
					}
				}
			}
		}	
		
		public void RotTree(float angle)
		{	if( Level >1){
				TreeHolder.transform.Rotate(0,0,angle);
			}
			if(Branch_List==null | Level <= 1){
			}else{ 
				for(int i=0;i<Branch_List.Count;i++)
				{
					Branch_List[i].RotTree(angle);
				}
			}
		}
		
		public void generateTree(float radi, float len, Color Branch_Color, Vector3 Position, GameObject INfiniDyTreeParent)
		{		
			if(INfiniDyForestOBJ!=null){
				
			}
			TreeHolder = new GameObject();
			
			//parent custom leaves there
			if(Level < 1){
				INfiniDyForestC.Leaf_pool.transform.parent = TreeHolder.transform;
			}		
			
			if(INfiniDyForestC !=null){
				
			}
			
			///////////// COLOR ///////
			if(is_root){		

				//v1.5
				if(INfiniDyForestC.BarkPrefab == ""){
					GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BarkPrefabOBJ);
					Instances = Branch;	
				}else{
					GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BarkPrefab));
					Instances = Branch;	
				}
			}
			else{			
				//if(INfiniDyForestC.Brance_prefab_per_level){ //v1.2 - removed bool, check by list size
				if(INfiniDyForestC.BranchPrefabs.Count > 0){
					if(INfiniDyForestC.BranchPrefabs.Count > 1){
						if(INfiniDyForestC.BranchID_per_level != null){
							//if(INfiniDyForestC.BranchID_per_level.Count > 0 & INfiniDyForestC.BranchID_per_level.Count >= (Level+1)){
							if((INfiniDyForestC.BranchID_per_level.Count > 0) & (INfiniDyForestC.BranchID_per_level.Count >= (Level+1)) ){
								
								if(INfiniDyForestC.BranchID_per_level[Level] < INfiniDyForestC.BranchPrefabs.Count){
									GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[INfiniDyForestC.BranchID_per_level[Level]]));
									Instances = Branch;	
								}else{
									GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
									Instances = Branch;	
								}
								
							}else{
								GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
								Instances = Branch;	
							}
						}else{
							GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
							Instances = Branch;	
						}
					}else{
						GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (INfiniDyForestC.BranchPrefabs[0]));
						Instances = Branch;	
					}
				}else{
					if(INfiniDyForestC.BranchPrefabsOBJ.Count > 1){
						if(INfiniDyForestC.BranchID_per_level != null){
							//if(INfiniDyForestC.BranchID_per_level.Count > 0 & INfiniDyForestC.BranchID_per_level.Count >= (Level+1)){
							if((INfiniDyForestC.BranchID_per_level.Count > 0) & (INfiniDyForestC.BranchID_per_level.Count >= (Level+1)) ){
								
								if(INfiniDyForestC.BranchID_per_level[Level] < INfiniDyForestC.BranchPrefabsOBJ.Count){
									GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[INfiniDyForestC.BranchID_per_level[Level]]);
									Instances = Branch;	
								}else{
									GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
									Instances = Branch;	
								}
								
							}else{
								GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
								Instances = Branch;	
							}
						}else{
							GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
							Instances = Branch;	
						}
					}else{
						GameObject Branch = (GameObject)Object.Instantiate(INfiniDyForestC.BranchPrefabsOBJ[0]);
						Instances = Branch;	
					}
				}
			}
			
			if(Level == 2){
				//Instances.renderer.material.color = Color.cyan;
			}
			if(Level == 3){
				//Instances.renderer.material.color = Color.magenta;
			}
			if(Level == 4){
				//Instances.renderer.material.color = Color.grey;
			}
			if(INfiniDyTreeParent == null){ 
				TreeHolder.tag = "INfiniDyTreeRoot";
				INfiniDyForestC.root_tree = TreeHolder;
			}
			
			if(!INfiniDyForestC.is_grass | Level == 0){
				if(INfiniDyTreeParent != null){  
					TreeHolder.transform.parent = INfiniDyTreeParent.transform;
				}
				TreeHolder.transform.localEulerAngles = Vector3.zero;
				TreeHolder.transform.localPosition = Position;
			}else{
				if(INfiniDyTreeParent != null){  
					if(Level ==0){//parent everything to the first treeholder
						TreeHolder.transform.parent = INfiniDyTreeParent.transform;
					}else{
						TreeHolder.transform.parent = INfiniDyForestC.root_tree.transform;
					}
				}
				TreeHolder.transform.localEulerAngles = Vector3.zero;
				
				float find_x = Random.Range(-7,7+Random.Range(-4,4));
				float find_z = Random.Range(-7,7+Random.Range(-4,4));
				float Dist_Above_Terrain = 0.0f;
				float find_y = 0;
				if(Terrain.activeTerrain != null){
					find_y = -INfiniDyForestC.root_tree.transform.position.y+Terrain.activeTerrain.SampleHeight(INfiniDyForestC.root_tree.transform.position+new Vector3(find_x,0,find_z))+Dist_Above_Terrain+ Terrain.activeTerrain.transform.position.y;
				}
				
				TreeHolder.transform.localPosition = new Vector3(0+find_x,find_y,1+find_z);
			}
			
			Instances.transform.parent = TreeHolder.transform;
			Instances.transform.localPosition = Vector3.zero;
			Instances.transform.localEulerAngles= Vector3.zero;
			
			float RANGE_X=Random.Range (-INfiniDyForestC.Min_max_spread.x,INfiniDyForestC.Min_max_spread.y);
			float RANGE_Y=Random.Range (-INfiniDyForestC.Min_max_spread.x,INfiniDyForestC.Min_max_spread.y);
			float RANGE_YY=0;
			if(INfiniDyForestC.Spread_Z_separate){
				RANGE_Y=Random.Range (-INfiniDyForestC.Min_max_spread_Z.x,INfiniDyForestC.Min_max_spread_Z.y);
			}
			if(INfiniDyForestC.Spread_Y_separate){
				RANGE_YY=Random.Range (-INfiniDyForestC.Min_max_spread_Y.x,INfiniDyForestC.Min_max_spread_Y.y);
			}
			
			if(Level > 1){
				if(!INfiniDyForestC.Reduce_angle_with_height){
					if(!INfiniDyForestC.Grow_angles){
						TreeHolder.transform.Rotate(RANGE_X,0,RANGE_Y);
					}else{
						TreeHolder.transform.Rotate(RANGE_X/INfiniDyForestC.Growth_angle_div,RANGE_YY,RANGE_Y/INfiniDyForestC.Growth_angle_div);
					}
				}else{
					if(!INfiniDyForestC.Grow_angles){
						TreeHolder.transform.Rotate(RANGE_X*(1-INfiniDyForestC.Height_reduce*Height_Level),RANGE_YY,RANGE_Y*(1-INfiniDyForestC.Height_reduce*Height_Level));
					}else{
						TreeHolder.transform.Rotate(RANGE_X*(1-INfiniDyForestC.Height_reduce*Height_Level)/INfiniDyForestC.Growth_angle_div,RANGE_YY,RANGE_Y*(1-INfiniDyForestC.Height_reduce*Height_Level)/INfiniDyForestC.Growth_angle_div);
					}
				}
			}
			TreeHolder.name = "TreeHolder";
			Instances.name = "Instances";
			Instances.transform.Translate(0,INfiniDyForestC.Length_scale*0.5f*len,0);
			Vector3 scaler = new Vector3(radi,len,radi);
			Instances.transform.localScale = scaler/1f;
			
			if(Level > 4){
				
			}
			
			//v1.1 - changed 1 to 0 in check
			if(Level > 0){
				INfiniDyForestC.Registered_Brances.Add(Instances.transform);
				INfiniDyForestC.Branch_grew.Add(0);
				INfiniDyForestC.Start_Scales.Add(Instances.transform.localScale);
				INfiniDyForestC.scaleVectors.Add(scaler);
				
				INfiniDyForestC.Branch_levels.Add(Level);
			}
			
			if(Height_Level > 0){
			}
			
			makeBranches();
			current_time = Time.fixedTime;//mark time to start scale growth
			Leaves.Add(this);
			Instances.transform.Translate(0,-INfiniDyForestC.Length_scale*0.5f*len,0);//v1.1
		}
		
		void Update(){
			
		}
		
		
		
	}
	
	
}

