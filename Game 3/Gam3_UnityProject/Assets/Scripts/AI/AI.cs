using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    // Game stats 
    #region AI Behaviour
    // AI States
    public enum NPC_States { Dormant, Hunting, Alert};
    // Start state is dormant
    public NPC_States states = NPC_States.Dormant;
    // Switch values of logic when this value increases or decreases
    public int typeOfState = 0;
    public bool PCSeen = false;
    #endregion

    #region Move Logic
    // Where the NPC will move
    private Vector3 moveDirection;
    // How fast the AI Will move
    public float objectSpeed = 5;
    private Rigidbody AI_Physics;
    private CapsuleCollider AI_Collider;
    #endregion

    #region Values of logic
    // Value increases when alert (Its a timer)
    private float playerInSight = 0;
    // Timers which balance the states of play for NPCs
    [SerializeField]
    private float HuntingTime = 0;
    [SerializeField]
    private float AlertedTime = 0;
    #endregion

    #region Movement Logic
    // Home 
    Vector3 startPosition;
    [SerializeField]
    public float rotationRate = 2.0f;

    int walkArrayScroller = 0;  // Scroll through the movement array
    public Vector3[] whereTheAiGoes;    // Set where the AI Goes
    #endregion

    #region Cone Of Sight Variables
    private Transform playerPosition;
    private int viewDistance = 60;  // Range
    private int fieldOfView = 45;
    private Vector3 rayDirection;
    #region Ray Logics
    public LayerMask AI_Detections;
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 11;
        #region IDE Component Set-Up
        // Component Set-Up
        AI_Physics = gameObject.AddComponent<Rigidbody>();
        AI_Physics.constraints = RigidbodyConstraints.FreezeRotation;
        AI_Collider = gameObject.AddComponent<CapsuleCollider>();

        #endregion

        #region Value Set-Up
        // Vector or Position set-up
        // Home position
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // Value settings
        objectSpeed = 5f;
        viewDistance = 60;
        fieldOfView = 45;
        #endregion

        #region Finding Objects.Components
        // Find the component that belongs to player
        playerPosition = GameObject.Find("PC").GetComponent<Transform>();
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Types of behaviour states
        if (typeOfState == 0)
            states = NPC_States.Dormant;

        if (typeOfState == 1)
            states = NPC_States.Hunting;

        if (typeOfState == 2)
            states = NPC_States.Alert;
        #endregion
        // Functions
        ConeView(); // We always want to draw the rays that will detect the player
        //StartCoroutine(AI_Movement());

        #region Remove After the logic works
        if (!PCSeen)
        {
            rotationRate = 2;
            gameObject.transform.Rotate(Vector3.up * rotationRate);
        }
        else
        {
            rotationRate = 0;   // Set to 0 so we have the affect of vision
            gameObject.transform.Rotate(Vector3.up * rotationRate);
        }
        #endregion

        if (PCSeen)
            AlertedTime += Time.deltaTime;

        if (AlertedTime > 5)
        {
            states = NPC_States.Alert;
            Debug.Log(NPC_States.Alert + ":" + "I'm Now Alerted and Will Hurt The Player . . .");
        }
    }

    // The View of the AIs priferal sight
    void ConeView()
    {
       
        // Hit logic for the array itself
        RaycastHit hit;
        // Vector that knows the difference from the players position and the current position of gameObject
        rayDirection = playerPosition.position - transform.position;

        // Point a ray at the players position
        Debug.DrawLine(transform.position, playerPosition.position, Color.red);
        // The position of where the front ray will be
        Vector3 frontRaypoint = transform.position + (transform.forward * viewDistance);

        // Approximate perspective visualization
        Vector3 leftRayPoint = frontRaypoint;
        leftRayPoint.x += fieldOfView * 0.5f;

        Vector3 rightRayPoint = frontRaypoint;
        rightRayPoint.x -= fieldOfView * 0.5f;

        // DrawLines    
        #region Front Ray Logic
        Debug.DrawLine(transform.position, frontRaypoint, Color.green);
        Physics.Raycast(transform.position, transform.TransformDirection(frontRaypoint), out hit, viewDistance, AI_Detections);
        #endregion

        #region Left Ray Logic
        Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        Physics.Raycast(transform.position, transform.TransformDirection(leftRayPoint), out hit, viewDistance, AI_Detections);
        #endregion

        #region RightRay Logic
        Debug.DrawLine(transform.position, rightRayPoint, Color.green);
        Physics.Raycast(transform.position, transform.TransformDirection(frontRaypoint + rightRayPoint + leftRayPoint), out hit, viewDistance, AI_Detections);
        #endregion
        // Value that holds the angle we want to detect
        float angle = Vector3.Angle(rayDirection, transform.forward);
        if (angle < fieldOfView * 0.5f)  // If the angle is less than the field of view 
        {
            #region Is Player Behind a wall
            // Condition that checks where the ray is going from point A to point B
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, viewDistance))
            {
                // Do we hit the players layer??
                if (hit.transform.gameObject.layer == 10)
                    PCSeen = true;  // Yeah we see the player
                else
                    PCSeen = false;     // No so we dont see the player

            }
            #endregion
        }
    }

    void AI_Behaviour()
    {
        if(states == NPC_States.Dormant)
        {
            

            // Movement

            // Find a good way to move
            // Move between values near the AI
        }

        if (states == NPC_States.Hunting)
        {
            // Allow for different movement
            // Move towards the recorded alerted area
            // Start to look around 
            // Wait for a value to reach a randomly generated one
            // change gamestate again
        }

        if(states == NPC_States.Alert)
        {
            // Follow player
            // When player is not in sight or hidden again
            // Hunt the player but always be close to the player (Gives illusion of still alerted)
            // When a value reaches a point then go back to dormant attitudes
        }
    }

    void Randomizer()
    {
        #region Note
        // typeOfState = 0 = Dormant
        // typeOfState = 1 = Hunting
        // typeOfState = 2 = Alerted
        #endregion
        switch (typeOfState)
        {
            case 0:
                Debug.Log(gameObject.transform.name + ":" + "Dormant . . .");

            break;

            case 1:
                // We are hunting we set random values so the AI can hunt until the value is met. 
                Debug.Log(gameObject.transform.name + ":" + "Hunting . . .");
                // We randomise the Hunting to - Dormant so the NPC does whatever until the valid time
                float minHuntTime = 10;
                float maxHuntTIme = 20;
                // Randomise the value
                float randomHuntValue = Random.Range(minHuntTime, maxHuntTIme);

                /// Insert Logic to change vector destinations to positions near the player but not follow
                break;

            case 2:
                // Get A Random Value when we pass the value then we allow the AI to go back to its home and becomes dormant
                Debug.Log(gameObject.transform.name + ":" + "Alerted . . .");
                float minAlertTime = 20;
                float maxAlertTime = 40;
                float RandomAlertValue = Random.Range(minAlertTime, maxAlertTime);
                break;
        }
    }

    void AvoidObsticles()
    {

    }

    IEnumerator AI_Movement()
    {
        // This moves the object forward we want to move the object where we tell it to go
        moveDirection = Vector3.forward;
        moveDirection = moveDirection.normalized * objectSpeed;
        AI_Physics.velocity = moveDirection * objectSpeed * Time.deltaTime;

        if (PCSeen)
        {
            yield return new WaitForEndOfFrame();
            moveDirection = Vector3.zero;
        }

        if (states == NPC_States.Alert)
        {
            yield return new WaitForEndOfFrame();
            // Replace this with a slow lerp

            moveDirection = Vector3.MoveTowards(transform.position, playerPosition.position, objectSpeed * Time.deltaTime);
        }
        yield break;
    }
}
