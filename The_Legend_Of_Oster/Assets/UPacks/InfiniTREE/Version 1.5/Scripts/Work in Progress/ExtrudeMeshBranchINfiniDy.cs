using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.INfiniDy {
	public class ExtrudedBranchSection
{
	public Vector3 point ;
	public Matrix4x4 matrix ;
	public float time ;
}

public class ExtrudeMeshBranchINfiniDy : MonoBehaviour {

	public float time = 2.0f;
	public bool autoCalculateOrientation = true;
	public float minDistance = 0.1f;
	public bool invertFaces = false;
	//private Mesh srcMesh ;
	private SplineMeshExtrusionINfiniDy.Edge[] precomputedEdges ;
	
		public List<ExtrudedBranchSection> sections; 

		//GROW ONCE
		public SplinerPINfiniDy SplinePath;
		public SplinerPINfiniDy SplineSection;
		Vector3 Initial_target_pos;//grab target position, to move the system with respect to the change from this position
		//bool translate_system = false;

	MeshFilter MESH;
		public float Base_angle = 90;
		public bool is_root = false;//mark root branch

		public bool self_growth = false;//allow child branch creation within the branch script, otherwise take over with tree manager for L-trees. 
		public int branch_level=0;//define level of branch in growth tree
		public int parent_section=0;//define the section the branch will be connected to in its parent branch.
		public ExtrudeMeshBranchINfiniDy Parent_branch;//keep link to parent structures, grab section in real time
		public int max_growth_level = 3;//stop growth in this level
		public List<string> BranchPrefab;
		GameObject Instances;
		List<Transform> Registered_Brances;
		public int max_branches = 2;
		public int grow_after = 25;//grow after this section count


	// Use this for initialization
	void Awake () {

			Registered_Brances = new List<Transform>();

		MESH = GetComponent(typeof(MeshFilter)) as MeshFilter;
		
			//srcMesh = new Mesh();

			sections = new List<ExtrudedBranchSection>();

			if(TargetTranform==null){
				TargetTranform = transform;
			}else{
				//translate_system = true;//if external target, move system
			}
			Initial_target_pos = TargetTranform.localPosition;
	}
	

		public bool  Bark_growth_ended = false;
		public Transform TargetTranform;
		Vector3 Keep_last_pos;
		bool Finished_bark = false;

		public Transform Level1_branches_Tranforms;

	// Update is called once per frame
	void LateUpdate () {

			Vector3 position = TargetTranform.position;
		float now = Time.time;	
	
		
		// Add a new trail section to beginning of array
		if (sections.Count == 0 || (sections[0].point - position).sqrMagnitude > minDistance * minDistance)
		{
				ExtrudedBranchSection section = new ExtrudedBranchSection ();
			section.point = position;
				section.matrix = TargetTranform.localToWorldMatrix;
			section.time = now;
			
			sections.Insert(0, section);
		}
		
		// We need at least 2 sections to create the line
		if (sections.Count < 2)
			return;

			//Self child branch growth
			if(self_growth){
			 if(branch_level < max_growth_level 
				   	& Registered_Brances.Count < max_branches 
				   	//& sections.Count > grow_after
				   & sections.Count > (grow_after*(Registered_Brances.Count+1))
				){

					//pick random branch
					string BranchPrefabS = BranchPrefab[Random.Range(0,BranchPrefab.Count)];

				//instantiate a prefab, pass this.level+1, pass section number and this.script to get section points
				GameObject Branch = (GameObject)Object.Instantiate(Resources.Load (BranchPrefabS));
				Instances = Branch;
				
				Instances.transform.localEulerAngles = Vector3.zero;

					Instances.transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(Vector3.up),Instances.transform.rotation,Random.Range(0.0f,1f));


					Instances.transform.position = sections[0].point;


				Instances.name = "Instances";
				
				Registered_Brances.Add(Instances.transform);

				//grab script
				ExtrudeMeshBranchINfiniDy Child_branch_grower = Instances.GetComponentInChildren(typeof(ExtrudeMeshBranchINfiniDy)) as ExtrudeMeshBranchINfiniDy;

				if(Child_branch_grower !=null){
						Child_branch_grower.Parent_branch = this;
						Child_branch_grower.branch_level = this.branch_level+1;//increase level
						Child_branch_grower.is_root = false;
				}
			 }			
			}

			if(SplinePath.count_steps2 >= SplinePath.Curve.Count-2 & !Finished_bark){
				Bark_growth_ended = true;
				SplinePath.parent_moving = null;

				Keep_last_pos = TargetTranform.position;
				Finished_bark = true;
			}

			if(Vector3.Distance(TargetTranform.position,Keep_last_pos) > minDistance){
				Keep_last_pos = TargetTranform.position;
				Bark_growth_ended = false;
			}else{
				Bark_growth_ended = true;
			}
		
		if(!Bark_growth_ended){

				Matrix4x4 worldToLocal = TargetTranform.worldToLocalMatrix;
		Matrix4x4[] finalSections = new Matrix4x4[sections.Count];
		Quaternion previousRotation = Quaternion.identity;
		
		for (var i=0;i<sections.Count;i++)
		{
			if (autoCalculateOrientation)
			{
				Vector3 direction = Vector3.zero;
				Quaternion rotation = Quaternion.identity;

				if (i == 0)
				{
					direction = sections[0].point - sections[1].point;
					rotation = Quaternion.LookRotation(direction, Vector3.up);
					previousRotation = rotation;
					finalSections[i] = worldToLocal * Matrix4x4.TRS(position, rotation, Vector3.one);	
				}
				// all elements get the direction by looking up the next section
				else if (i != sections.Count - 1)
				{	
					direction = sections[i].point - sections[i+1].point;
					rotation = Quaternion.LookRotation(direction, Vector3.up);
					
					// When the angle of the rotation compared to the last segment is too high
					// smooth the rotation a little bit. Optimally we would smooth the entire sections array.
					if (Quaternion.Angle (previousRotation, rotation) > 20){
						rotation = Quaternion.Slerp(previousRotation, rotation, 0.5f);
					}
					
					previousRotation = rotation;
					finalSections[i] = worldToLocal * Matrix4x4.TRS(sections[i].point, rotation, Vector3.one);
				}
				// except the last one, which just copies the previous one
				else
				{
					finalSections[i] = finalSections[i-1];
				}
			}
			else
			{
				if (i == 0)
				{
					finalSections[i] = Matrix4x4.identity;
				}
				else
				{
					finalSections[i] = worldToLocal * sections[i].matrix;
				}
			}
		}

		//////////////////////////////////////

		List<Vector2> sourceMeshData = CreateCircle(1,8);
		
		Vector2 sourceMeshCenter = CalculateCentroid(sourceMeshData);
		
		//List<Vector3> levelVerts = new List<Vector3>();
		//List<Vector2> levelUVBary = new List<Vector2>();
		List<Vector2> levelUVs = new List<Vector2>();
		List<int> levelTris = new List<int>();
		
		levelTris.Clear();
		
		//int verticesPerNode = 4;
		int edgeCount = sourceMeshData.Count;
		
		List<Vector3> sourceVerts = new List<Vector3>();
		
		for (int i = 0; i < edgeCount; i++)
		{			
			sourceVerts.Add(new Vector3(sourceMeshData[i].x, sourceMeshData[i].y, 0));

					if(sections.Count >= 2){
						//Vector3 vertex = sourceMeshData[i];
						//Vector3 normal = Vector3.zero;						

						if(i>0){
							bool Use_curve=false;
							if(Use_curve){
								
							}else{
								//normal = Vector3.Cross(TargetTranform.position-Keep_last_pos,sourceMeshData[i]);
							}
						}						

						//Vector3 u1 = (new Vector3(-normal.z, 0, normal.x)).normalized;
						//Vector3 v1 = Vector3.Cross(u1, normal);						 
						//float Height = 0;
						//float C1 = Vector3.Dot(v1, vertex + v1 * (Height - vertex.y)/v1.y);						
						//Vector2 uv = new Vector2(Vector3.Dot(vertex, u1), Vector3.Dot(v1, vertex) - C1);

						levelUVs.Add(new Vector2(10, 1));
					}else{
						levelUVs.Add(new Vector2(10, 1));
					}
		}
		
		sourceVerts.Add(new Vector3(sourceMeshCenter.x, sourceMeshCenter.y, 0));
		levelUVs.Add(new Vector2(10, 0));
		
		for (int i = 0; i < edgeCount - 1; i++)
		{                                       //0, 1, 2, 3
			levelTris.Add(sourceVerts.Count - 1); //4, 4, 4, 4 
			levelTris.Add(i);                   //0, 1, 2, 
			levelTris.Add(i + 1);               //1, 2, 3,
		}
		
		levelTris.Add(sourceVerts.Count - 1);
		levelTris.Add(edgeCount - 1);
		levelTris.Add(0);		
		
		Mesh temp = new Mesh();
		temp.vertices = sourceVerts.ToArray(); //first define vertices, or the below wont work
		temp.triangles = levelTris.ToArray();
		temp.uv = levelUVs.ToArray();
		

		for(int i=0;i<sourceMeshData.Count;i++){
			//Debug.Log ("Edge count = "+sourceMeshData[i]);
		}
		
			precomputedEdges = SplineMeshExtrusionINfiniDy.BuildManifoldEdges(temp);		
		
				Vector3[] insert_cap_to_bark = new Vector3[sourceMeshData.Count];

				Vector3 Dist = Initial_target_pos-transform.localPosition;


				for(int i=0;i<sourceMeshData.Count;i++){
					insert_cap_to_bark[i] = Vector3.zero+new Vector3((-50+Dist.x)/transform.localScale.x,Dist.y/transform.localScale.x,Dist.z/transform.localScale.x)
					+Quaternion.AngleAxis(Base_angle,Vector3.right)*new Vector3(0,sourceMeshData[i].x,sourceMeshData[i].y);
				}
			
				if(is_root){
					SplineMeshExtrusionINfiniDy.ExtrudeMesh (temp, MESH.mesh, finalSections, precomputedEdges, invertFaces, true,insert_cap_to_bark);
				}else{
					SplineMeshExtrusionINfiniDy.ExtrudeMesh (temp, MESH.mesh, finalSections, precomputedEdges, invertFaces, false,insert_cap_to_bark);
				}		
		}
	}//END UPDATE


	public static List<Vector2> CreateCircle (double radius, int sides)
	{
		List<Vector2> vectors = new List<Vector2> ();
		
		const float max = 2.0f * Mathf.PI;
		float step = max / sides;
		
		for (float theta = 0.0f; theta < max; theta += step) {
			vectors.Add (new Vector2 ((float)(radius * Mathf.Cos (theta)), (float)(radius * Mathf.Sin (theta))));
		}	
		
		return vectors;
	}
	
	public static Vector2 CalculateCentroid(List<Vector2> vectorList)
	{		
		float fArea = 0.0f, fDistance = 0.0f;
		Vector2 vCenter = Vector2.zero;
		int nIndex = 0, nLastPointIndex = vectorList.Count - 1;
		
		// Run through the list of positions.
		for (int i = 0; i <= nLastPointIndex; ++i)
		{
			//////////////////////////////////////////////////////////////////////////
			// Cacluate index.
			nIndex = (i + 1) % (nLastPointIndex + 1);
			
			// Calculate distance.
			fDistance = vectorList[i].x * vectorList[nIndex].y - vectorList[nIndex].x * vectorList[i].y;
			
			// Acculmate area.
			fArea += fDistance;
			
			// Move center positions based on positions and distance.
			vCenter.x += (vectorList[i].x + vectorList[nIndex].x) * fDistance;
			vCenter.y += (vectorList[i].y + vectorList[nIndex].y) * fDistance;
		}
		//
		//////////////////////////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////////
		// Calculate the final center position.
		fArea *= 0.5f;
		vCenter.x *= 1.0f / (6.0f * fArea);
		vCenter.y *= 1.0f / (6.0f * fArea);
		//
		//////////////////////////////////////////////////////////////////////////
		
		return vCenter;
	}
}

}