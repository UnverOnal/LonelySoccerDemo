using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour 
{
    [HideInInspector]public bool isAimingDone;

    [SerializeField]private Transform targetTransform;
    [SerializeField]private Transform bottomSideCamera, topSideCamera;
    
    private Vector3 cameraOffset;
    private Vector3 smoothedPosition;

    private float smoothFactor = 1f;

    private void Start()
    {
        cameraOffset = bottomSideCamera.position - targetTransform.position;
    }

    private void Update() 
    {        
        //Switches between the cameras
        if(isAimingDone)
        {
            bottomSideCamera.gameObject.SetActive(true);
        }
        else
        {
            bottomSideCamera.gameObject.SetActive(false); 
        }
    }

    private void LateUpdate()
    {
        //Provides us the camera follow the target if the bottom camera is active
        if(bottomSideCamera.gameObject.activeSelf)
        {
            smoothedPosition = Vector3.Lerp(bottomSideCamera.position, targetTransform.position + cameraOffset, smoothFactor);

            Vector3 constraintedPosition = bottomSideCamera.position;
            constraintedPosition.z = smoothedPosition.z; 

            bottomSideCamera.position = constraintedPosition;
        }
    }
}