using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatablePlayerController : MonoBehaviour , IControllableMovement
{
    private float angle;

    private float turnSpeed;

    private void Start() 
    {
        #if UNITY_EDITOR
            turnSpeed = 7.5f;
        #elif UNITY_ANDROID
            turnSpeed = 0.2f;
        #endif

        angle = transform.eulerAngles.y;
    }

    public void Move(Vector3 deltaPosition)
    {
        angle -= deltaPosition.x * turnSpeed;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}