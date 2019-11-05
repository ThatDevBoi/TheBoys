using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    #region AI Variables
    // Game stats 
    #region AI Behaviour
    // AI States
    public enum NPC_States { Dormant, Hunting, Alert};
    // Start state is dormant
    public NPC_States states = NPC_States.Dormant;
    // Switch values of logic when this value increases or decreases
    [SerializeField]    // Remove after debugging
    private int typeOfState = 0;    // Cant edit
    [SerializeField]    // Remove after debugging
    private bool PCSeen = false;    // Cant edit 
    #endregion

    #region Values of logic
    // Timers which balance the states of play for NPCs
    // This value meets a random generated value which allows for the NPC to look for the player
    [SerializeField]
    private float HuntingTime = 0;  // Cant edit
    // Value that agrees or disagrees if the ai is hunting
    private float TimeUntilHunting = 2;
    private float TimeUntilAlerted = 3;
    // The time that meets the random alert time
    // Its so the NPC can Behave alerted until the player is gone and hidden
    [SerializeField]
    private float AlertedTime = 0;  // Cant Edit
    // How long the player can be within vision until the AI is alert
    [SerializeField]
    private float playerTimeInSight = 0;  // Cannot Edit
    #endregion

    #region Movement Logic
    // AI Home (Its First Start Position)
    Vector3 startPosition;  // Cannot edit
    [SerializeField]
    // Rate we rotate at
    private float rotationRate = 45;
    // Scroll through the main array for different movement positions
    int walkArrayScroller = 0;  // Cant Edit
    // Main array that we are going to change overtime
    public Vector3[] whereTheAiGoes;    // Can Edit
    #endregion

    #region Random Behaviour Time Values
    // Random values to meet
    [SerializeField]
    public float random_Alert_Value;    // Cant Edit
    [SerializeField]
    public float random_Hunt_Value;     // Cant Edit
    #endregion

    #region Detecting walls
    // If the rays return something then we have to avoid the wall
    public bool wallDetected = false; // Can Edit
    #region The Angle we have made with rays Values
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);   // Can Edit
    // Left and right sensors at the front Edit to change positions of the rays
    public float frontSideSensorPosition = 0.2f;    // Can Edit
    // Angle Position 
    public float frontsensorAngle = 30f;    // Can Edit
    #endregion
    #endregion

    #region Cone Of Sight Variables 
    private Transform playerPosition;   // Cant Edit
    public int viewDistance = 60;  // Can Edit
    public int fieldOfView = 45;    // Can Edit 
    private Vector3 rayDirection;   // Cant edit (Re Visit)
    public LayerMask AI_Detections; // Can Edit
    #endregion

    #region Movement
    // Where the NPC will move
    private Vector3 moveDirection;  // Cant Edit
     // How fast the AI Will move
    public float objectSpeed = 5;   // Can Edit
    // The Physics we can manipulate
    private Rigidbody AI_Physics;   // Cant Edit
    // Collision we want 
    private CapsuleCollider AI_Collider;    // Cant edit
    #endregion
    #endregion
    public GameObject bulletObject;
    public Transform BulletHeard;
    public bool heardNoise;
    public float changer = 0f;

    #region Start
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
        //objectSpeed = .5f;
        viewDistance = 60;
        fieldOfView = 45;
        #endregion

        #region Finding Objects Components
        // Find the component that belongs to player
        playerPosition = GameObject.Find("PC").GetComponent<Transform>();
        BulletHeard = null;
        #endregion
    }
    #endregion

    #region Update
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
        // !! May need to be changed later !!
        if (heardNoise == false)
        {
            StartCoroutine(AI_Movement());
        }
        else if(heardNoise == true)
            StopCoroutine(AI_Movement());

        if (bulletObject = GameObject.Find("Bullet_Sound_Position"))
        {
            BulletHeard = bulletObject.GetComponent<Transform>();
            heardNoise = true;
            transform.LookAt(BulletHeard.position);
            transform.position = Vector3.MoveTowards(transform.position, BulletHeard.transform.position, Time.deltaTime * objectSpeed);
        }
        if(heardNoise == true)
        {
            changer += Time.deltaTime;
            if (changer > 5)
            {
                Destroy(bulletObject);
                changer = 0;
                heardNoise = false;
                // change this with a slerp rotation 
                transform.LookAt(whereTheAiGoes[walkArrayScroller]);
            }
        }


        // Rotation debugging logic // Delete after everything works
        #region Remove After the logic works
        //if (!PCSeen)
        //{
        //    rotationRate = 2;
        //    gameObject.transform.Rotate(Vector3.up * rotationRate);
        //}
        //else
        //{
        //    rotationRate = 0;   // Set to 0 so we have the affect of vision
        //    gameObject.transform.Rotate(Vector3.up * rotationRate);
        //}
        #endregion

        #region Behaviour Conditions
        // if the player is within view of the AI
        if (PCSeen)
        {
            // Alert Logic
            // We count up the Player In Sight Value
            playerTimeInSight += Time.deltaTime;
            // If the detection value is greater than the time it takes to detect
            if (playerTimeInSight > TimeUntilAlerted)
            {
                playerTimeInSight = 0;  // reset the value
                typeOfState = 2;
                Randomizer();
            }

            if (playerTimeInSight > TimeUntilHunting)
            {
                typeOfState = 1;
                Randomizer();
            }
        }
        // However
        else if (!PCSeen)
        {
            playerTimeInSight = 0;
            PCSeen = false;
        }
        // Hunting
        if (typeOfState == 1)
        {
            Debug.Log(NPC_States.Hunting + ":" + "I'm Now Hunting The Player");
            HuntingTime += Time.deltaTime;
            if (HuntingTime > random_Hunt_Value)
            {
                random_Hunt_Value = 0;
                HuntingTime = 0;
                typeOfState = 0;
            }
        }

        // Alert
        if (typeOfState == 2)
        {
            Debug.Log(NPC_States.Alert + ":" + "I'm Now Alerted and Will Hurt The Player . . .");
            AlertedTime += Time.deltaTime;
            if (AlertedTime > random_Alert_Value)
            {
                random_Alert_Value = 0;
                AlertedTime = 0;
                typeOfState = 0;
            }
        }
        #endregion

        #region Timer Stoppers
        if (states == NPC_States.Alert)
        {
            playerTimeInSight = 0;
        }
        #endregion
    }
    #endregion

    // The View of the AIs priferal sight
    #region Cone View Function
    void ConeView()
    {
        #region Cone View
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

        #endregion

        // Value that holds the angle we want to detect
        float angle = Vector3.Angle(rayDirection, transform.forward);
        if (angle < fieldOfView * 0.5f)  // If the angle is less than the field of view 
        {
            #region Is Player Behind a wall / Detecting the player
            // Condition that checks where the ray is going from point A to point B
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, viewDistance))
            {
                // Do we hit the players layer??
                if (hit.transform.gameObject.layer == 10)
                    PCSeen = true;
            }
            else
                // if we dont hit the player then we cant see them
                PCSeen = false;
            #endregion
        }

        #region Obsticle Detection
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        wallDetected = false;
        
        // front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if(Physics.Raycast(sensorStartPos, transform.forward, out hit, viewDistance))
        {
            if (hit.collider.CompareTag("Obsticle"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                Debug.Log("Front Right Sensor Hit");
                wallDetected = true;
                avoidMultiplier -= 1f;
                Debug.Log("Front Right Sensor Value" + ":" + avoidMultiplier);
            }
        }
        
        // front right angle sensor
        else if(Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontsensorAngle, transform.up) * transform.forward, out hit, viewDistance))
        {
            if (hit.collider.CompareTag("Obsticle"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                Debug.Log("Front Right Angle Sensor Hit");
                wallDetected = true;
                avoidMultiplier -= 0.5f;
                Debug.Log("Front Right Angle Sensor Value" + ":" + avoidMultiplier);
            }
        }

        // front left sensor 
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, viewDistance))
        {
            if (hit.collider.CompareTag("Obsticle"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                Debug.Log("Front Left Sensor Hit");
                wallDetected = true;
                avoidMultiplier += 1;
                Debug.Log("Front Left Sensor Value" + ":" + avoidMultiplier);
            }
        }
        
        // front left angle sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontsensorAngle, transform.up) * transform.forward, out hit, viewDistance))
        {
            if (hit.collider.CompareTag("Obsticle"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                Debug.Log("Front Left Angle Sensor Hit");
                wallDetected = true;
                avoidMultiplier += 0.5f;
                Debug.Log("Front Left Angle Sensor Value" + ":" + avoidMultiplier);
            }
        }

        // Front Centre Senser
        if(avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Obsticle"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    Debug.Log("Front Centre Hit");
                    wallDetected = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = -1f;
                        Debug.Log("Front Centre Senser Value" + ":" + avoidMultiplier);
                    }
                    else
                    {
                        avoidMultiplier = 1;
                        Debug.Log("Front Centre Senser Value" + ":" + avoidMultiplier);
                    }
                }
            }
        }


        if (wallDetected)
        {
            // Insert Logic
            gameObject.transform.Rotate(Vector3.up * rotationRate);
            //
        }

        #endregion
    }
    #endregion

    #region Time Behaviour Lasts
    void Randomizer()
    {
            #region Note
            // typeOfState = 0 = Dormant
            // typeOfState = 1 = Hunting
            // typeOfState = 2 = Alerted
            #endregion
            switch (typeOfState)
            {
            #region Dormant
            case 0:
                    // Reset everything when the AI goes from Hunting to Dormant || Alert to Dormant
                    Debug.Log(gameObject.transform.name + ":" + "Dormant . . .");
                    random_Hunt_Value = 0;
                    random_Alert_Value = 0;
                    HuntingTime = 0;
                    AlertedTime = 0;
                    break;
            #endregion

            #region Hunting
            case 1:
                    // We are hunting we set random values so the AI can hunt until the value is met. 
                    Debug.Log(gameObject.transform.name + ":" + "Hunting . . .");
                    AlertedTime = 0;
                    random_Alert_Value = 0;
                    // We randomise the Hunting to - Dormant so the NPC does whatever until the valid time
                    float minHuntTime = 10;
                    float maxHuntTIme = 20;
                    // Randomise the value
                    random_Hunt_Value = Random.Range(minHuntTime, maxHuntTIme);
                    break;
            #endregion

            #region Alerted
            case 2:
                    // Get A Random Value when we pass the value then we allow the AI to go back to its home and becomes dormant
                    Debug.Log(gameObject.transform.name + ":" + "Alerted . . .");
                    HuntingTime = 0;
                    random_Hunt_Value = 0;
                    float minAlertTime = 20;
                    float maxAlertTime = 40;
                    // Generate random alert value
                    random_Alert_Value = Random.Range(minAlertTime, maxAlertTime);
                    break;
            #endregion
            }
    }
    #endregion

    #region Movement Logic
    IEnumerator AI_Movement()
    {
        // Make sure we always set the start position
        whereTheAiGoes[0] = startPosition;

        // Simple dormant movement (Works)
        #region Dormant Movement
        if (states == NPC_States.Dormant)
        {
            moveDirection = whereTheAiGoes[walkArrayScroller];
            if (Vector3.Distance(moveDirection, transform.position) < 3)
            {
                walkArrayScroller++;
                if (walkArrayScroller >= whereTheAiGoes.Length)
                {
                    walkArrayScroller = 0;
                }
                else
                    transform.LookAt(whereTheAiGoes[walkArrayScroller]);
            }
            transform.position = Vector3.MoveTowards(transform.position, whereTheAiGoes[walkArrayScroller], Time.deltaTime * objectSpeed);
        }
        #endregion

        #region Hunting Movement
        if(states == NPC_States.Hunting)
        {
            Debug.Log("I'm Hunting Now");
            Vector3 playerPosCurrently = playerPosition.position;
            float playerpos = playerPosCurrently.magnitude;
            // We see the players current Position
            Debug.Log("Player Position:" + playerPosCurrently);
            float maxStalk = Random.Range(playerpos, playerpos);
            transform.position = Vector3.MoveTowards(transform.position, playerPosCurrently * maxStalk, Time.deltaTime * objectSpeed);
        }
        #endregion

        #region Alert Movement
        if(states == NPC_States.Alert)
        {
         //   transform.LookAt(playerPosition.position);
            transform.position = Vector3.MoveTowards(transform.position, playerPosition.position, Time.deltaTime * objectSpeed);
        }
        #endregion
        yield break;
    }
    #endregion
}