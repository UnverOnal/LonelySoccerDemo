using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages user inputs 
public class UserInputController : MonoBehaviour
{
    private  Vector3 previousPosition;
    private  Vector3 currentPosition;
    private  Vector3 deltaPosition;

    private static bool isFingerMoved = false;

    //It was created for the test purposes on the editor 
    //Returns delta position of mouse cursors movement
    public Vector3 GetMouseDragValue()
    {
        //To take starting point to previousPosition...
        if(Input.GetMouseButtonDown(0))
        {
            previousPosition = ConvertScreenPositionToWorld();
        }

        //While left mouse button is held down...
        if(Input.GetMouseButton(0))
        {
            currentPosition = ConvertScreenPositionToWorld();

            deltaPosition = currentPosition - previousPosition;
            previousPosition = currentPosition;

            return deltaPosition;
        }
        else
        {
            ResetControlValues();
            return Vector3.zero;
        }
    }

    //If the platform is Android then this is called 
    //and gets delta position value of changing finger position on the screen
    public Vector3 GetTouchDragValue()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch(touch.phase)
            {
                case TouchPhase.Began:
                currentPosition = touch.position;
                previousPosition = currentPosition;
                break;

                case TouchPhase.Moved:
                currentPosition = touch.position;
                deltaPosition = currentPosition - previousPosition;
                previousPosition = touch.position;
                isFingerMoved = true;
                break;

                case TouchPhase.Ended:
                ResetControlValues();
                break;
            }
        }

        if(isFingerMoved)
        {
            isFingerMoved = false;
            return deltaPosition;
        }
        return Vector3.zero;
    }

    //Makes some values related to game control zero to prepare them a new control session
    private void ResetControlValues()
    {
        currentPosition = default(Vector3);
        deltaPosition = default(Vector3);
        previousPosition = default(Vector3);
    }

    //This ones purpose is to avoid from code repeating
    public Vector3 ConvertScreenPositionToWorld()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;//If the z value equals zero then ScreenToWorldPoint method returns cameras' position
        mousePosition = Camera.main.enabled == true ? Camera.main.ScreenToWorldPoint(mousePosition) : Vector3.zero;

        return mousePosition;
    }
}
