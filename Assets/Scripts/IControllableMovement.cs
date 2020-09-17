using UnityEngine;

interface IControllableMovement
{
    //Controllable objects must have this method to do their movement
    void Move(Vector3 deltaPosition);
}