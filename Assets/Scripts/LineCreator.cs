using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineCreator : MonoBehaviour
{
    private const int infinity = 8;
    private int maxReflections = 100;
    private int currentReflections = 0;
    private int layerMask;

    [SerializeField] private Vector3 startingPoint;
    public Vector3 direction;
    public List<Vector3> currentPoints;

    //It can be changed on editor
    public float rayDistance = 12f;

    private LineRenderer lineRenderer;

    public RaycastHit lastHitData;
    

    private void Start() 
    {
        currentPoints = new List<Vector3>();
        lineRenderer = GetComponent<LineRenderer>();

        //Ignore goalPost object for casting rays
        layerMask = 1 << 2;
        layerMask = ~layerMask;
    }

    private void FixedUpdate() 
    {
        RaycastHit hitData; 
        var isThereAHit = Physics.Raycast(startingPoint, (direction - startingPoint).normalized,out hitData, rayDistance, layerMask);

        currentReflections = 0;
        currentPoints.Clear();
        currentPoints.Add(startingPoint);

        if(isThereAHit)
        {
            //If there is a hit then creates reflection
            ReflectFurther(startingPoint, hitData, out lastHitData);
        }
        else
        {
            //If the ray doesn't hit anything , its length be equal to infinity
            currentPoints.Add(startingPoint + (direction - startingPoint).normalized * infinity);
        }

        lineRenderer.positionCount = currentPoints.Count;
        lineRenderer.SetPositions(currentPoints.ToArray());
    }

    //Creates reflections
    private void ReflectFurther(Vector3 origin, RaycastHit hitData, out RaycastHit lastHitData)
    {
        //Outs last hit data
        lastHitData = hitData;

        if(currentReflections > maxReflections)
        {
            return;
        }

        currentPoints.Add(hitData.point);
        currentReflections++;

        Vector3 inDirection = (hitData.point - origin).normalized;
        Vector3 newDirection = Vector3.Reflect(inDirection, hitData.normal);

        RaycastHit newHitData;
        var isThereANewHit = Physics.Raycast(hitData.point + (newDirection * 0.0001f), newDirection * 100, out newHitData, rayDistance, layerMask);
        if(isThereANewHit)
        {
            ReflectFurther(hitData.point, newHitData, out lastHitData);
        }
        else
        {
            //Doesn't add or generate new ray if the last hit is GoalPost 
            if(!hitData.collider.CompareTag("GoalPost"))
            {
                currentPoints.Add(hitData.point + newDirection * rayDistance);
            }
        }
    }

}
