using UnityEngine;
using System.Collections;
using Artngame.INfiniDy;
using System.Collections.Generic;

public class LocationToShaderINFINITREE : MonoBehaviour {

    public bool enableCirclularWind = false;
    public Transform player; //cilrulr wind control

    public List<Material> materials = new List<Material>();

    //public InfiniGRASSManager Grassmanager;
	Transform this_tranf;
	Vector3 prev_pos;
	public float InteractSpeed=2;
    public float StopMotionThreshold = 4;
    public float lerpSpeed = 1;
    public float TornadoAHeight = 1;
    public float TornadoBHeight = 1;

    public Transform TornadoA;
    public Transform TornadoB;
    //public float SpreadFrames = 2;

    // Use this for initialization
    void Start () {
		this_tranf = player.transform;
		prev_pos = this_tranf.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		//pass in late update to override Grass manager control
		if (materials.Count > 0) {

            if (Application.isPlaying)
            {

                Vector3 Direction = prev_pos - this_tranf.position;
                Vector3 SpeedVec = (Direction).normalized * ((prev_pos - this_tranf.position).magnitude / Time.deltaTime);
                SpeedVec = SpeedVec * InteractSpeed;
                prev_pos = this_tranf.position;

                for (int i = 0; i < materials.Count; i++)
                {

                    materials[i].SetVector("_InteractPos", this_tranf.position);
                    if (enableCirclularWind && TornadoA != null)//SpeedVec.magnitude == 0)
                    {
                        //materials[i].SetVector("_InteractPos1", player.position);
                        materials[i].SetVector("_InteractPosC1", new Vector4(TornadoA.position.x, TornadoA.position.y, TornadoA.position.z,1) );
                        materials[i].SetVector("_InteractAmpFreqRad1", new Vector4(TornadoA.localScale.y, TornadoA.localScale.x, TornadoA.localScale.z, TornadoAHeight));
                    }
                    else
                    {
                        materials[i].SetVector("_InteractPosC1", Vector4.zero);
                        materials[i].SetVector("_InteractAmpFreqRad1", Vector4.zero);
                    }
                    if (enableCirclularWind && TornadoB != null)//SpeedVec.magnitude == 0)
                    {
                        materials[i].SetVector("_InteractPosC2", new Vector4(TornadoB.position.x, TornadoB.position.y, TornadoB.position.z, 1));
                        materials[i].SetVector("_InteractAmpFreqRad2", new Vector4(TornadoB.localScale.y, TornadoB.localScale.x, TornadoB.localScale.z, TornadoBHeight));
                    }
                    else
                    {
                        materials[i].SetVector("_InteractPosC2", Vector4.zero);
                        materials[i].SetVector("_InteractAmpFreqRad2", Vector4.zero);
                    }
                    if (materials[i].HasProperty("_InteractSpeed"))
                    {
                        //Debug.Log(SpeedVec);
                        materials[i].SetVector ("_InteractSpeed", Vector3.Lerp (materials[i].GetVector ("_InteractSpeed"), SpeedVec, lerpSpeed * Time.deltaTime));
                        //materials[i].SetVector("_InteractSpeed", SpeedVec);
                    }
                    if (materials[i].HasProperty("_StopMotionThreshold"))
                    {
                        materials[i].SetFloat("_StopMotionThreshold", StopMotionThreshold);
                    }
                }
            }
		}
	}
}
