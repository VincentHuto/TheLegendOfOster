using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (firstPersonCamera.isActiveAndEnabled){
                firstPersonCamera.enabled = false;
                thirdPersonCamera.enabled = true;
            }
            else if(thirdPersonCamera.isActiveAndEnabled)
            {
                firstPersonCamera.enabled = true;
                thirdPersonCamera.enabled = false;
            }
        
        }
       
    }

    public void ShowOverheadView()
    {
        firstPersonCamera.enabled = false;
        thirdPersonCamera.enabled = true;
    }

    public void ShowFirstPersonView()
    {
        firstPersonCamera.enabled = true;
        thirdPersonCamera.enabled = false;
    }
}
