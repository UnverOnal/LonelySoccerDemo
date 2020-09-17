using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour , IControllableMovement
{
    private LineCreator lineCreator;

    private LineRenderer lineRenderer;

    private GameObject closestObjectToInputPosition;

    private float movementSpeed;

    private void Start() 
    {
        lineCreator = GetComponent<LineCreator>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void Update() 
    {
        #if UNITY_EDITOR
            //If there are no input then the line isn't casted
            if(Input.GetMouseButton(0))
            {
                closestObjectToInputPosition = GameManager.Instance.closestObjectToInputPosition;

                if(closestObjectToInputPosition == gameObject)
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
                //Resets the line
                lineCreator.direction = Vector3.zero;
                closestObjectToInputPosition = null;
            }

            movementSpeed = 7.5f;

        #elif UNITY_ANDROID
            //If there are no input then the line isn't casted
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if(touch.phase != TouchPhase.Ended)
                {
                    closestObjectToInputPosition = GameManager.Instance.closestObjectToInputPosition;

                    if(closestObjectToInputPosition == gameObject)
                    lineRenderer.enabled = true;
                }
                else
                {
                    lineRenderer.enabled = false;
                    //Resets the line
                    lineCreator.direction = Vector3.zero;
                    closestObjectToInputPosition = null;
                }
            }

            movementSpeed = 0.025f;
        #endif
    }
    
    //Moves line
    public void Move(Vector3 deltaPosition)
    {
        if(lineCreator != null)
        {
            lineCreator.direction.x += deltaPosition.x * movementSpeed;
        }
        else
        {
            Debug.LogError("pathCreator object is null, you must assign a PathCreator object into it");
        }
    }
}