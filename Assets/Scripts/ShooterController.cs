using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [SerializeField]private Transform ball;

    private const float speed = 1.2f; 

    private Vector3 ballsStartingPosition;

    public bool isAimingDone;

    private void Start() 
    {
        ballsStartingPosition = ball.position;
        ballsStartingPosition.y = transform.position.y;
    }

    void Update()
    {
        if(isAimingDone)
        {
            transform.Translate((ballsStartingPosition - transform.position) * Time.deltaTime * speed);
        }
    }
}
