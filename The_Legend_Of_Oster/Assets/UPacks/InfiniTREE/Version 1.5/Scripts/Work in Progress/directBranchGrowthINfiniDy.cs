using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directBranchGrowthINfiniDy : MonoBehaviour
{
    public Transform MainBranch;

    public List<Transform> followerBranches = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }
    Vector3 prevPos;
    public float offsetX = 1; public float offsetY = 1; public float offsetZ = 1;
    // Update is called once per frame
    void Update()
    {
        if(MainBranch != null)
        {
            for (int i=0;i < followerBranches.Count; i++)
            {
                Vector3 direction = MainBranch.transform.position - prevPos;
                Vector3 posDiff = MainBranch.transform.position - followerBranches[i].position;
                Vector3 adder = Vector3.zero;
                if(direction != Vector3.zero && posDiff != Vector3.zero && direction.magnitude > 0.1f && posDiff.magnitude > 0.1f)
                {
                    adder = Vector3.Cross(direction, posDiff).normalized;
                }
                //followerBranches[i].position = MainBranch.transform.position + adder;
                followerBranches[i].position = followerBranches[i].position + direction * offsetY + posDiff*offsetX + adder * offsetZ;

            }
        }

        prevPos = MainBranch.transform.position;
    }
}
