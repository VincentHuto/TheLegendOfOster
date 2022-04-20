using UnityEngine;
using System.Collections;
using Artngame.INfiniDy;


public class LocationToShaderINFINIGRASS : MonoBehaviour {

    //pass light v0.2
    public bool passSunLight = false;
    public Transform sun;


    public bool enableCirclularWind = false;
    public Transform helicopter; //cilrulr wind control


    public InfiniGRASSManager Grassmanager;
	Transform this_tranf;
	Vector3 prev_pos;
	public float InteractSpeed=2;

	//public float SpreadFrames = 2;

	// Use this for initialization
	void Start () {
		this_tranf = this.transform;
		prev_pos = this_tranf.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		//pass in late update to override Grass manager control
		if (Grassmanager != null) {		
			//Grassmanager.Interactor = this_tranf;
			//Grassmanager.player =this.gameObject;
			if(Application.isPlaying){
				//if (Random.Range (0, 2) == 1) {
					//pass speed
					Vector3 Direction = prev_pos - this_tranf.position;
					Vector3 SpeedVec = (Direction).normalized * ((prev_pos - this_tranf.position).magnitude / Time.deltaTime);
					prev_pos = this_tranf.position;

                for (int i = 0; i < Grassmanager.ExtraGrassMaterials.Count; i++)
                {
                    for (int j = 0; j < Grassmanager.ExtraGrassMaterials[i].ExtraMaterials.Count; j++)
                    {
                        if (Grassmanager.ExtraGrassMaterials[i].ExtraMaterials[j].HasProperty("_InteractPos"))
                        {
                            Grassmanager.ExtraGrassMaterials[i].ExtraMaterials[j].SetVector("_InteractPos", this_tranf.position);
                        }
                        if (Grassmanager.ExtraGrassMaterials[i].ExtraMaterials[j].HasProperty("_InteractPos1") && enableCirclularWind)
                        {
                            Grassmanager.ExtraGrassMaterials[i].ExtraMaterials[j].SetVector("_InteractPos1", helicopter.position);
                        }

                        if(passSunLight && sun != null)
                        {
                            Grassmanager.ExtraGrassMaterials[i].ExtraMaterials[j].SetVector("_LightDirection", sun.forward);
                        }
                    }
                }
                for (int i = 0; i < Grassmanager.GrassMaterials.Count; i++)
                {
                    Grassmanager.GrassMaterials[i].SetVector("_InteractPos", this_tranf.position);
                    if (enableCirclularWind)//SpeedVec.magnitude == 0)
                    {
                        Grassmanager.GrassMaterials[i].SetVector("_InteractPos1", helicopter.position);
                    }
                    if (Grassmanager.GrassMaterials[i].HasProperty("_InteractSpeed"))
                    {
                        //Grassmanager.GrassMaterials[i].SetVector("_InteractSpeed",SpeedVec);
                        Grassmanager.GrassMaterials[i].SetVector("_InteractSpeed", Vector3.Lerp(Grassmanager.GrassMaterials[i].GetVector("_InteractSpeed"), SpeedVec, InteractSpeed * Time.deltaTime));
                    }
                    if (passSunLight && sun != null)
                    {
                        Grassmanager.GrassMaterials[i].SetVector("_LightDirection", sun.forward);
                    }
                }
				//}
			}
			//pass this as interactor in all grass !!!! (once per x frames)
		}
	}
}
