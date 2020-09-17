using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Transform firstTarget;
    private bool isBallTouched;

    private List<Vector3> pathPoints;
    private Vector3 direction;

    private int pointIndex = 0;

    private Rigidbody ballRigidbody;

    private float forceAmount = 15f;

    private void Start() 
    {
        ballRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        pathPoints = GameManager.Instance.totalPoints;
        //It works when the aiming is done
        if(GameManager.Instance.isAimingDone)
        {
            if(isBallTouched)
            {
                StopTheBall(ballRigidbody);

                direction = pathPoints[pointIndex + 1] - transform.position;
                direction.y = 2.25f;
                direction = direction.normalized;
                ballRigidbody.AddForce(direction * forceAmount , ForceMode.Impulse);

                pointIndex++;

                isBallTouched = false;
            }
        }
    }

    //Stops the ball temporarily to apply a new force
    private void StopTheBall(Rigidbody ballRigidbody)
    {
        ballRigidbody.AddForce(Vector3.zero);
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isBallTouched = true;
        }
    }
}
