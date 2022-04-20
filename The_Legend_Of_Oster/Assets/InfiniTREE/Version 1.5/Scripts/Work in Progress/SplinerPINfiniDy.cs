using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.INfiniDy {

[System.Serializable()]

	public class SplinerPINfiniDy : MonoBehaviour {

	List<SplineNode[]> Keep_Curve;
	SplineNode[] Curve1;

	[SerializeField]
	public Vector3 last_object_position_inspector_saw;

	public List<GameObject> control_points_children;

	[SerializeField]
	public Vector3 last_object_rotation_inspector_saw;

	[SerializeField]
	public Vector3 last_object_scale_inspector_saw;

	[SerializeField]
	public float Handle_scale=6.5f;

	void Start () {

			if(CurveQuality <1){

				CurveQuality =1;

			}

			keep_quality=CurveQuality;
	
		Start_Point = this.transform.position;

		if(parent_moving !=null){
		Start_Point_object = parent_moving.transform.position;
		}

		Keep_last_pos = this.transform.position;

		Keep_last_rot = this.transform.eulerAngles;

		Sphere_particles = this.gameObject.GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;

		//Legacy_particles = this.gameObject.GetComponentInChildren(typeof(ParticleEmitter)) as ParticleEmitter;

		if(last_object_position_inspector_saw != this.gameObject.transform.position ){ 
			
			for (int i=0;i<this.SplinePoints.Count;i++) 
			{
				Vector3 Distance = Keep_last_pos - last_object_position_inspector_saw;
				SplinePoints[i].position = SplinePoints[i].position + Distance;
			}

			last_object_position_inspector_saw = this.gameObject.transform.position;

		}

		if(last_object_rotation_inspector_saw != this.gameObject.transform.eulerAngles ){ 
			
			for (int i=0;i<this.SplinePoints.Count;i++) 
			{
				Vector3 Distance =Keep_last_rot -  last_object_rotation_inspector_saw;
				
				Vector3 Move_to_0 = SplinePoints[i].position - transform.position;
				SplinePoints[i].position = Quaternion.AngleAxis(Distance.y, new Vector3(0,1,0) ) * Move_to_0 + transform.position;
				
				Move_to_0 = SplinePoints[i].position - transform.position;
				SplinePoints[i].position = Quaternion.AngleAxis(Distance.z, new Vector3(0,0,1) ) * Move_to_0 + transform.position;
				
				Move_to_0 = SplinePoints[i].position - transform.position;
				SplinePoints[i].position = Quaternion.AngleAxis(Distance.x, new Vector3(1,0,0) ) * Move_to_0 + transform.position;
			}

			last_object_rotation_inspector_saw = this.gameObject.transform.eulerAngles;
		}

		#region CALC CURVE
		SplineNode[]  SplinePoints1 = SplinePoints.ToArray();
		int Control_points_Count = SplinePoints1.Length;




			if(Overide_seg_detail.Count < SplinePoints.Count-1){
				Overide_seg_detail = new List<int>(); 
				for (int i=0;i<SplinePoints.Count-1;i++) 
				{
					
					Overide_seg_detail.Add(0);

				}
				
			}
			Keep_Overide_seg_detail=new List<int>();

			for(int i=0;i<Overide_seg_detail.Count;i++){
				
				Keep_Overide_seg_detail.Add(Overide_seg_detail[i]);
				
			}
			




		if (Keep_Curve == null ){Keep_Curve = new List<SplineNode[]>();}

		Curve.Clear();
		for (int i=0;i<Control_points_Count;i++) {

			if (i<=Control_points_Count-2) {
							
					float detail = CurveQuality;


					if(Overide_seg_detail[i] > 0){
						
						detail = Overide_seg_detail[i];
						
					}

					
					Vector3 Handler_start  = SplinePoints1[i].position;
					Vector3 Handler_end  = SplinePoints1[i+1].position;
					
					Vector3 Vector_along_direction=Handler_start-Handler_end;
					Vector3 Vector_along_direction_INV=Handler_end-Handler_start;
					Vector3 Vector_along_direction_normalized  = (Vector_along_direction).normalized;
					
					Vector3 Curve_starting_direction = Vector3.zero;
					Vector3 Curve_ending_direction  = Vector3.zero;
					
					if (i==0 | i <= Control_points_Count-3 ) {
						if (i==0){
							Curve_starting_direction  = Vector_along_direction_INV.normalized;
							SplinePoints1[i].direction = Curve_starting_direction;
						}
						else{
							Curve_starting_direction = SplinePoints1[i].direction;
						}
						Curve_ending_direction = -1.0f*((SplinePoints1[i+2].position-Handler_end).normalized - Vector_along_direction_normalized).normalized;
						SplinePoints1[i+1].direction = -1.0f*Curve_ending_direction;
						
					} else {
						Curve_starting_direction = SplinePoints1[i].direction;
						Curve_ending_direction = Vector_along_direction.normalized;
						SplinePoints1[i+1].direction = -1.0f*Curve_ending_direction;
					} 
					
					List<SplineNode> a = new List<SplineNode>();
					
					for (float j = 0;j<1;j+= (1/detail)) {
						
						Vector3 a1 = Handler_start;
						Vector3 b1 = Vector_along_direction.magnitude*Curve_starting_direction/2;
						Vector3 c1 = Vector_along_direction.magnitude*Curve_ending_direction/2;
						Vector3 d1 = Handler_end;
						Vector3 C1 = ( d1 - (3.0f * (c1+d1)) + (3.0f * (b1+a1)) - a1 );
						Vector3 C2 = ( (3.0f * (c1+d1)) - (6.0f * (b1+a1)) + (3.0f * a1) );
						Vector3 C3 = ( (3.0f * (b1+a1)) - (3.0f * a1) );
						Vector3 C4 = ( a1 );
						Vector3 p = C1*j*j*j + C2*j*j + C3*j + C4;
						
						SplineNode NEW_POINT = new SplineNode(Vector3.zero,Quaternion.identity);
						NEW_POINT.position = p;
						
						a.Add (NEW_POINT);
					}
					
					Curve1 = a.ToArray();
					List<SplineNode> Curve_Temp =  new List<SplineNode>();
					Curve_Temp.AddRange(Curve1);
					Curve_Temp.Add(SplinePoints1[i+1]);
					Curve1 = Curve_Temp.ToArray();
					Keep_Curve.Add(Curve1);
					
				Curve.AddRange(Curve1);

			}

		}
		#endregion

	}

	public bool show_controls_play_mode=false;
	public bool show_controls_edit_mode=true;

	public int Initialized =0;

	public ParticleSystem Sphere_particles;
	//public ParticleEmitter Legacy_particles;

	public bool loop = false; 
	public GameObject parent_moving;

	[SerializeField]
	public List<SplineNode> Curve = new List<SplineNode>(); 

	private int count_steps;
		public int count_steps2; // NEW, to check for branch growth

	public bool follow_curve=true;
	public Vector3 Start_Point_object;

	public Vector3 Keep_last_pos;
	public Vector3 Keep_last_rot;

	[SerializeField]
	public float Scale_curve=1f;

	[SerializeField]
	public float Keep_last_scale;
	
	public float Motion_Speed=1;

	public bool Game_Mode_On=false;

	public bool Alter_mode=false;
	public bool Alter_mode2=false;
	public bool Accelerate=false;

	[SerializeField]
	public bool Always_on=false;

		public bool Add_point=false;
		public bool With_manip=false;


		public bool Add_mid_point=false;
		public int Add_in_Segment=1;
		
		public float Node_toggle_dist =1f;


		//infinidy
		public bool repainted=true;

	void Update () {


			if(CurveQuality <1){
				
				CurveQuality =1;
				
			}



			if(Overide_seg_detail.Count < SplinePoints.Count-1){
				Overide_seg_detail = new List<int>(); 
				for (int i=0;i<SplinePoints.Count-1;i++) 
				{
					
					Overide_seg_detail.Add(0);
					
					
				}
				Keep_Overide_seg_detail=new List<int>();
				Keep_Overide_seg_detail = Overide_seg_detail;
			}



		#region CALC CURVE
		if(Game_Mode_On){

			bool repaint=false;



				for (int i=0;i<Overide_seg_detail.Count-1;i++) 
				{

					
					if(Keep_Overide_seg_detail[i] != Overide_seg_detail[i]){
						
						Keep_Overide_seg_detail[i] = Overide_seg_detail[i];
						

						
						repaint=true;
					}
				}



				//if quality changes
				if(keep_quality!=CurveQuality){

					keep_quality=CurveQuality;
					repaint=true;

				}


				if(Add_point){
					
					Add_point=false;
					repaint=true;

					
					Vector3 Vector_along_last_two_points = (SplinePoints[SplinePoints.Count-1].position-SplinePoints[SplinePoints.Count-2].position);
					
					Vector3  Moved_vector_to_last_point = SplinePoints[SplinePoints.Count-1].position + 1*Vector_along_last_two_points;
					
					SplinePoints.Add(new SplineNode(Moved_vector_to_last_point,Quaternion.identity));


					Overide_seg_detail.Add(0);
					Keep_Overide_seg_detail.Clear();
					for(int i=0;i<Overide_seg_detail.Count;i++){
						
						Keep_Overide_seg_detail.Add(Overide_seg_detail[i]);
						
					}
										
					GameObject SPHERE_SMALL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					control_points_children.Add(SPHERE_SMALL);

					
					control_points_children[control_points_children.Count-1].transform.position = Moved_vector_to_last_point;
					control_points_children[control_points_children.Count-1].transform.localScale = (1/gameObject.transform.localScale.x)*0.3f*new Vector3(Handle_scale,Handle_scale,Handle_scale);
					
					if(With_manip){
						control_points_children[control_points_children.Count-1].AddComponent(typeof(DragTransformINfiniDy));
					}
					control_points_children[control_points_children.Count-1].transform.parent = transform;
					
				}



				if(SplinePoints.Count > 2 ){
					
					
					if (Add_mid_point) {
						
						Add_mid_point=false;
						
						
						if(Add_in_Segment > 0 & Add_in_Segment < SplinePoints.Count){
							
							
							repaint=true;
							
							
							Vector3 Moved_vector_to_last_point = SplinePoints[Add_in_Segment-1].position -(SplinePoints[Add_in_Segment-1].position-SplinePoints[Add_in_Segment].position)/2;
							
							
							SplinePoints.Insert(Add_in_Segment, new SplineNode(Moved_vector_to_last_point,Quaternion.identity));
							
							GameObject SPHERE_SMALL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							
							control_points_children.Insert(Add_in_Segment,SPHERE_SMALL);
							
							

							Overide_seg_detail.Insert(Add_in_Segment-1,0);
							Keep_Overide_seg_detail.Clear();
							for(int i=0;i<Overide_seg_detail.Count;i++){
								
								Keep_Overide_seg_detail.Add(Overide_seg_detail[i]);
								
							}
							
							
							
							control_points_children[Add_in_Segment].transform.position = Moved_vector_to_last_point;
							control_points_children[Add_in_Segment].transform.localScale = (1/gameObject.transform.localScale.x)*0.3f*new Vector3(Handle_scale,Handle_scale,Handle_scale);
							
							if(With_manip){
								control_points_children[Add_in_Segment].AddComponent(typeof(DragTransformINfiniDy));
							}
							
							control_points_children[Add_in_Segment].transform.parent = transform;
							
							
						}
					}
				}


			for(int i=control_points_children.Count-1;i>=0;i--){
				
				if(control_points_children[i] ==null){
									

						if(i>0){
							Overide_seg_detail.RemoveAt(i-1);
						}


					SplinePoints.RemoveAt(i);
					control_points_children.RemoveAt(i);

					repaint=true;
				}
				
			}

			if(control_points_children.Count !=0){
				for (int i=0;i<SplinePoints.Count;i++){

					if(SplinePoints[i].position == control_points_children[i].transform.position)
					{
						//do nothing 
					}
					else{
						SplinePoints[i].position = control_points_children[i].transform.position;
						repaint=true;

					}
				}
			}

			//repainted=false;

			if(repaint){

					repainted=true;

			SplineNode[]  SplinePoints1 = SplinePoints.ToArray();
			int Control_points_Count = SplinePoints1.Length;
			
		
			Curve.Clear();
			for (int i=0;i<Control_points_Count;i++) {
				
					if (i<=Control_points_Count-2) {

						float detail = CurveQuality;


							if(Overide_seg_detail[i] > 0){
								
								detail = Overide_seg_detail[i];
								
							}
													
						Vector3 Handler_start  = SplinePoints1[i].position;
						Vector3 Handler_end  = SplinePoints1[i+1].position;
						
						Vector3 Vector_along_direction=Handler_start-Handler_end;
						Vector3 Vector_along_direction_INV=Handler_end-Handler_start;
						Vector3 Vector_along_direction_normalized  = (Vector_along_direction).normalized;
						
						Vector3 Curve_starting_direction = Vector3.zero;
						Vector3 Curve_ending_direction  = Vector3.zero;
						
						if (i==0 | i <= Control_points_Count-3 ) {
							if (i==0){
								Curve_starting_direction  = Vector_along_direction_INV.normalized;
								SplinePoints1[i].direction = Curve_starting_direction;
							}
							else{
								Curve_starting_direction = SplinePoints1[i].direction;
							}
							Curve_ending_direction = -1.0f*((SplinePoints1[i+2].position-Handler_end).normalized - Vector_along_direction_normalized).normalized;
							SplinePoints1[i+1].direction = -1.0f*Curve_ending_direction;
							
						} else {
							Curve_starting_direction = SplinePoints1[i].direction;
							Curve_ending_direction = Vector_along_direction.normalized;
							SplinePoints1[i+1].direction = -1.0f*Curve_ending_direction;
						} 
						
						List<SplineNode> a = new List<SplineNode>();
						
						for (float j = 0;j<1;j+= (1/detail)) {
							
							Vector3 a1 = Handler_start;
							Vector3 b1 = Vector_along_direction.magnitude*Curve_starting_direction/2;
							Vector3 c1 = Vector_along_direction.magnitude*Curve_ending_direction/2;
							Vector3 d1 = Handler_end;
							Vector3 C1 = ( d1 - (3.0f * (c1+d1)) + (3.0f * (b1+a1)) - a1 );
							Vector3 C2 = ( (3.0f * (c1+d1)) - (6.0f * (b1+a1)) + (3.0f * a1) );
							Vector3 C3 = ( (3.0f * (b1+a1)) - (3.0f * a1) );
							Vector3 C4 = ( a1 );
							Vector3 p = C1*j*j*j + C2*j*j + C3*j + C4;
							
							SplineNode NEW_POINT = new SplineNode(Vector3.zero,Quaternion.identity);
							NEW_POINT.position = p;
							
							a.Add (NEW_POINT);
						}

							a.Add(SplinePoints1[i+1]);
							Curve.AddRange(a);
						
					}
				
			 }
		   }
		}
		#endregion


		if(parent_moving !=null){

			if(Sphere_particles != null){
				//v1.8
				ParticleSystem.EmissionModule em = Sphere_particles.emission;
				em.enabled = true;
				//Sphere_particles.enableEmission = true;
			}
			//if(Legacy_particles != null){
				//Legacy_particles.enabled = true;
				//Legacy_particles.emit = true;
			//}

			if(follow_curve ){

				if(!Alter_mode & !Alter_mode2){
					if((count_steps2+1) <= Curve.Count-1 )
					{
						parent_moving.transform.position =parent_moving.transform.position+(Curve[count_steps2+1].position - Curve[count_steps2].position).normalized*5*0.35f*Motion_Speed;
					
						if( Vector3.Distance(Curve[count_steps2+1].position,parent_moving.transform.position) < 7 ){ 
							count_steps2=count_steps2+1;
							parent_moving.transform.position = Curve[count_steps2].position;
						}

						else if(Vector3.Distance(Curve[count_steps2+1].position,parent_moving.transform.position) > (5*Vector3.Distance(Curve[count_steps2+1].position,Curve[count_steps2].position)) )
						{	count_steps2=count_steps2+1;
							parent_moving.transform.position = Curve[count_steps2].position;
						}

					}

					if(count_steps2 > Curve.Count-2 )
					{
						if(Sphere_particles != null){
							//v1.8
							ParticleSystem.EmissionModule em = Sphere_particles.emission;
							em.enabled = false;
							//Sphere_particles.enableEmission = false;
						}
						
						//if(Legacy_particles != null){
							//Legacy_particles.emit = false;
							//Legacy_particles.enabled=false;							
						//}
						count_steps2=0;parent_moving.transform.position = Curve[count_steps2].position;
					}
				}

				if(Alter_mode & !Alter_mode2){
					if((count_steps2+1) <= Curve.Count-1 )
					{

						if( (-parent_moving.transform.position+Curve[count_steps2+1].position).magnitude >0.0001f){
							
						Quaternion rotation = Quaternion.LookRotation( -Curve[count_steps2+1].position + parent_moving.transform.position);
						parent_moving.transform.rotation = Quaternion.Slerp(parent_moving.transform.rotation, rotation, 50f*Motion_Speed * Time.deltaTime);
						}else
						{
							
						}

						parent_moving.transform.position =parent_moving.transform.position+(Curve[count_steps2+1].position - Curve[count_steps2].position).normalized*5*0.35f*Motion_Speed;

						
							if( Vector3.Distance(Curve[count_steps2+1].position,parent_moving.transform.position) < Node_toggle_dist ){ 
							count_steps2=count_steps2+1;
							parent_moving.transform.position = Curve[count_steps2].position;
						}
						
						else if(Vector3.Distance(Curve[count_steps2+1].position,parent_moving.transform.position) > (3*Vector3.Distance(Curve[count_steps2+1].position,Curve[count_steps2].position)) )
						{	count_steps2=count_steps2+1;
							parent_moving.transform.position = Curve[count_steps2].position;
						}
						
					}
					if(count_steps2 > Curve.Count-2 )
					{
						if(Sphere_particles != null){
							//v1.8
							ParticleSystem.EmissionModule em = Sphere_particles.emission;
							em.enabled = false;
							//Sphere_particles.enableEmission = false;
						}
						
						//if(Legacy_particles != null){
							//Legacy_particles.emit = false;
							//Legacy_particles.enabled=false;							
						//}
						count_steps2=0;parent_moving.transform.position = Curve[count_steps2].position;
					}
				}

				if(Alter_mode2 ){
					if((count_steps2+1) <= Curve.Count-1 )
					{
				
						if( (Curve[count_steps2+1].position-parent_moving.transform.position).magnitude >0.0001f){
							
							Quaternion rotation = Quaternion.LookRotation( -Curve[count_steps2+1].position + parent_moving.transform.position);
							
							if(Curve[count_steps2+1].position == Curve[count_steps2].position)
							{
								if(count_steps2 <= Curve.Count-2 )
								{
									
								}
							}

							parent_moving.transform.rotation = Quaternion.Slerp(parent_moving.transform.rotation, rotation, 0.5f);
						
						}

						if(Accelerate){
							parent_moving.transform.position = Vector3.Lerp(parent_moving.transform.position,(Curve[count_steps2+1].position),(Curve[count_steps2+1].position - parent_moving.transform.position).magnitude*5*0.35f*Motion_Speed);

						}
						else{
							float Speed=0.5f;
							Speed= 5*0.35f*Motion_Speed;
							if( (Curve[count_steps2+1].position - parent_moving.transform.position).sqrMagnitude !=0 ){
								Speed = 5*0.35f*Motion_Speed / (Curve[count_steps2+1].position - parent_moving.transform.position).magnitude;
							}

							if(Speed<0.01f){Speed=2.51f;}
							
							parent_moving.transform.position = Vector3.Lerp(parent_moving.transform.position,(Curve[count_steps2+1].position),Speed);
						}

							if( Vector3.Distance(Curve[count_steps2+1].position,parent_moving.transform.position) < Node_toggle_dist ){ 

							if( (count_steps2+2) < Curve.Count )
							{
								if(Curve[count_steps2+2].position == Curve[count_steps2+1].position & 1==1)
								{
									count_steps2=count_steps2+2;

									if(count_steps2 <= Curve.Count-2 )
									{
										parent_moving.transform.position = Vector3.Slerp(parent_moving.transform.position, Curve[count_steps2].position,0.5f);
									}
								
								}else{

									count_steps2=count_steps2+1;
									
									parent_moving.transform.position = Vector3.Slerp(parent_moving.transform.position, Curve[count_steps2].position,0.5f);
								}
							}else{
								
								count_steps2=count_steps2+1;
								
								parent_moving.transform.position = Vector3.Slerp(parent_moving.transform.position, Curve[count_steps2].position,0.5f);
							}
															
						}
						
						else if(Vector3.Distance(Curve[count_steps2+1].position,parent_moving.transform.position) > (3.1f*Vector3.Distance(Curve[count_steps2+1].position,Curve[count_steps2].position)) )
						{	count_steps2=count_steps2+1;
							
							
						}
						
					}
					if(count_steps2 > Curve.Count-2 )
					{
						if(Sphere_particles != null){
							//v1.8
							ParticleSystem.EmissionModule em = Sphere_particles.emission;
							em.enabled = false;
							//Sphere_particles.enableEmission = false;
						}
						
						//if(Legacy_particles != null){
							//Legacy_particles.emit = false;
							//Legacy_particles.enabled=false;
							
						//}
						count_steps2=0;
					}
				}

			}
			else{
				parent_moving.transform.localPosition   = parent_moving.transform.localPosition+	(SplinePoints[count_steps+1].position - SplinePoints[count_steps].position).normalized*Time.fixedTime*0.25f;

				if( Vector3.Distance(SplinePoints[count_steps+1].position,parent_moving.transform.localPosition)< 5    ){
				count_steps=count_steps+1;
				}
				
				if(count_steps > SplinePoints.Count-2)
				{count_steps=0;}
			}
		}
	
		if(!show_controls_play_mode & NODES_DISABLED ==0){
			for(int i=0;i<control_points_children.Count;i++){
				MeshRenderer TEMP = control_points_children[i].GetComponent(typeof(MeshRenderer)) as MeshRenderer;
				TEMP.enabled = false;
			}
			NODES_DISABLED=1; 
		}else if(show_controls_play_mode & NODES_DISABLED ==1){
			for(int i=0;i<control_points_children.Count;i++){
				MeshRenderer TEMP = control_points_children[i].GetComponent(typeof(MeshRenderer)) as MeshRenderer;
				TEMP.enabled = true;
			}
			NODES_DISABLED=0;
		}
	
	}

	
	public int NODES_DISABLED;

	[SerializeField]
	public Vector3 Start_Point;
	[SerializeField()]
	public List<SplineNode> SplinePoints = new List<SplineNode>();
	

		[SerializeField()]
		public List<int> Overide_seg_detail;
		
		private List<int> Keep_Overide_seg_detail;

	[SerializeField]
	public bool PointGizmo_On  = true;
	[SerializeField]
	public bool extended=false;
		[Range(1,100)][SerializeField]
	public int CurveQuality  = 20;
	[SerializeField]
	public bool rotate_mode = false; 

		private int keep_quality;
	
}

#region Node
[System.Serializable]
public class SplineNode {

	public SplineNode ( Vector3 pos, Quaternion rot) {
		this.position = pos;
		this.rotation = rot;
	}
	public Vector3 position   = Vector3.zero;
	public Quaternion rotation   = Quaternion.identity;
	public Vector3 direction;
}
#endregion

}
