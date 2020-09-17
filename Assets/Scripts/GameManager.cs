using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private UserInputController userInputController;

    [SerializeField]private LineCreator lineCreator;

    private IControllableMovement controllableScript;

    [HideInInspector]public GameObject closestObjectToInputPosition;

    private RaycastHit inputHit;
    private RaycastHit lineHitData;

    //This is true if the aiming is succesfully done
    [HideInInspector]public bool isAimingDone;

    [HideInInspector]public List<Vector3> totalPoints;

    [SerializeField] private CameraManager cameraManager;

    [SerializeField] private ShooterController shooterController;


    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this as GameManager;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start() 
    {
        userInputController = gameObject.AddComponent<UserInputController>();
    }

    void Update()
    {
        lineHitData = lineCreator.lastHitData;

        #region Controls
        #if UNITY_EDITOR

            Vector3 dragValueMouse = userInputController.GetMouseDragValue();

            //This is necessary to get first clicks
            //First clicks are used to find gameObjects that close to clicks world position
            if(Input.GetMouseButtonDown(0))
            {
                inputHit = GetHitData(Input.mousePosition);
            }
            //Finds closest object only while left mouse button is held down
            else if(Input.GetMouseButton(0))
            {
                controllableScript = FindClosestControllableScript(inputHit.point, out closestObjectToInputPosition);
            }
            else
            {
                closestObjectToInputPosition = null;//Reset the closest object

                //If the aiming is succesfully done
                if(lineHitData.collider != null)
                {
                    if(lineHitData.collider.CompareTag("GoalPost") && !isAimingDone)
                    {
                        isAimingDone = true;
                        totalPoints.AddRange(lineCreator.currentPoints);
                        StartCoroutine(DelayBallScene());
                    }
                }
            }

            if(controllableScript != null)
            {
                controllableScript.Move(dragValueMouse);
            }

        #elif UNITY_ANDROID
        
            Vector3 dragValueTouch = userInputController.GetTouchDragValue();

            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    inputHit = GetHitData(touch.position);
                }
                //Finds closest object only while the screen is touched
                else if(touch.phase != TouchPhase.Ended)
                {
                    controllableScript = FindClosestControllableScript(inputHit.point, out closestObjectToInputPosition);
                }
                else
                {
                    closestObjectToInputPosition = null;//Reset the closest object

                    //If the aiming succesfully done
                    if(lineHitData.collider != null)
                    {
                        if(lineHitData.collider.CompareTag("GoalPost") && !isAimingDone)
                        {
                            isAimingDone = true;
                            totalPoints.AddRange(lineCreator.currentPoints);
                            StartCoroutine(DelayBallScene());
                        }
                    }
                }

                if(controllableScript != null)
                {
                    controllableScript.Move(dragValueTouch);
                }
            }
        #endif
        #endregion
    }

    //Finds the closest controllable object to hit point and returns its controller script
    private IControllableMovement FindClosestControllableScript(Vector3 inputPosition, out GameObject closestObjectToInputPosition)
    {
        IControllableMovement controllableScript = default(IControllableMovement);
        GameObject closestControllableObject = default(GameObject);

        inputPosition.y = 0f;

        //Ignore objects that are not controllable
        //Used for generating OverlapSphere
        int layerMask = 1 << 8;

        Collider[] colliders = Physics.OverlapSphere(inputPosition, 5f, layerMask);

        //If there are multiple collactables finds the closest one
        if(colliders.Length > 0)
        {
            if(colliders.Length > 1)
            {
                GameObject[] gameObjects = ConvertCollidersToGameObjects(colliders);
                closestControllableObject = GetClosestObjectToAPoint(gameObjects, inputPosition);
            }
            else
            {
                closestControllableObject = colliders[0].gameObject;
            }

            controllableScript = closestControllableObject.GetComponent<IControllableMovement>();
        }
        closestObjectToInputPosition = closestControllableObject;
        return controllableScript;
    }

    //Takes collider array as a parameter
    //Compares over collider array
    public GameObject GetClosestObjectToAPoint(GameObject[] gameObjectsToCompare, Vector3 inputPosition)
    {
        GameObject closestObject = default(GameObject);

        float minimumDistance = float.PositiveInfinity;
        float currentDistance = 0f;

        for(int i = 0; i < gameObjectsToCompare.Length; i++)
        {
            currentDistance = Vector3.Distance(inputPosition, gameObjectsToCompare[i].transform.position);
            if(currentDistance < minimumDistance)
            {
                closestObject = gameObjectsToCompare[i];
                minimumDistance = currentDistance;
            }
        }
        Debug.Log(closestObject.name);
        return closestObject;
    }

    //A helper method that converts collider arrays to GameObject arrays
    private GameObject[] ConvertCollidersToGameObjects(Collider[] colliders)
    {
        GameObject[] gameObjects = new GameObject[colliders.Length];
        for(int i = 0; i < colliders.Length ; i++)
        {
            gameObjects[i] = colliders[i].gameObject;
            Debug.Log(gameObjects[i].name);
        }

        return gameObjects;
    }

    //Creates ray and returns hit data
    private RaycastHit GetHitData(Vector3 inputPosition)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);

        if(Physics.Raycast(ray, out hit, 100f))
        {
            return hit;
        }

        return default(RaycastHit);
    }

    //Delays opening the ball movement scene
    private IEnumerator DelayBallScene()
    {
        float time = 1f;
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
        }
        cameraManager.isAimingDone = isAimingDone;
        shooterController.isAimingDone = isAimingDone;
    }
}