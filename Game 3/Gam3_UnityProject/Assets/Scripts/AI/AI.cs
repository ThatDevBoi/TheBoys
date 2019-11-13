using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

    public class AI : MonoBehaviour
    {
        #region AI Behaviour
        // AI States
        public enum NPC_States { Dormant, Hunting, Alert };
        // Start state is dormant
        [Header("AI_Behaviour")]
        public NPC_States states = NPC_States.Dormant;
        // Switch values of logic when this value increases or decreases
        [SerializeField]    // Remove after debugging
        private int typeOfState = 0;    // Cant edit
        [SerializeField]    // Remove after debugging
        public bool PCSeen = false;    // Cant edit 
        #endregion

        #region Values of logic
        // Timers which balance the states of play for NPCs
        // This value meets a random generated value which allows for the NPC to look for the player
        private float HuntingTime = 0;  // Cant edit
        // Value that agrees or disagrees if the ai is hunting
        private float TimeUntilHunting = 2;
        private float TimeUntilAlerted = 3;
        // The time that meets the random alert time
        // Its so the NPC can Behave alerted until the player is gone and hidden
        private float AlertedTime = 0;  // Cant Edit
        // How long the player can be within vision until the AI is alert
        private float playerTimeInSight = 0;  // Cannot Edit
        #endregion

        #region Random Behaviour Time Values
        [Header("AI_Behaviour_Resetters")]
        // Random values to meet
        public float random_Alert_Value;    // Cant Edit
        public float random_Hunt_Value;     // Cant Edit
        #endregion

        #region Detecting walls
        [Header("Avoiding_Walls")]
        // If the rays return something then we have to avoid the wall
        public bool wallDetected = false; // Can Edit
        #endregion

        #region Cone Of Sight Variables 
        [Header("Cone_Of_Sight")]

        public int viewDistance = 60;  // Can Edit
        public int fieldOfView = 45;    // Can Edit 
        public LayerMask AI_Detections; // Can Edit
        private Transform playerPosition;   // Cant Edit
                                            // Hit logic for the array itself
        private RaycastHit hit;
        #endregion

        #region Movement Variables
        [Header("Movement_Attributes")]
        public Vector3[] whereTheAiGoes;    // Can Edit
        // How fast the AI Will move
        public float objectSpeed = 5;   // Can Edit
        // Where the NPC will move
        Vector3 startPosition;  // Cannot edit
        private Vector3 moveDirection;  // Cant Edit
        // Main array that we are going to change overtime
        // Scroll through the main array for different movement positions
        int walkArrayScroller = 0;  // Cant Edit

        // The Physics we can manipulate
        private NavMeshAgent AI_Physics;   // Cant Edit
        // Collision we want 
        private CapsuleCollider AI_Collider;    // Cant edit
        // Rotation
        private Quaternion AI_rotation;
        [SerializeField]
        // Rate we rotate at
        private float rotationRate = 45;
    #endregion

    [Header("Health n Damage")]
    public int MaxHealth;
    public int currentHealth;
        // Slap this in AI Behaviour
        public GameObject bulletObject;
        public Transform BulletHeard;
        public bool heardNoise;
        public float changer = 0f;
        public float wallDetectionRange = 5;
        
        FieldOfView fov;

    public bool headShot = false;
    public GameObject head;

        #region Start
        // Start is called before the first frame update
        void Start()
        {
            // Find the script
            fov = this.gameObject.GetComponent<FieldOfView>();

            gameObject.layer = 11;
            gameObject.name = "Enemy";
            #region IDE Component Set-Up
            // Component Set-Up
            AI_Physics = gameObject.GetComponent<NavMeshAgent>();
        //    AI_Physics.speed = 1;
        
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
            // Health Set-up
            currentHealth = MaxHealth;
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

            #region Heard Noise
            // !! May need to be changed later !!
            if (heardNoise == false)
            {
                StartCoroutine(AI_Movement());
            }
            else if (heardNoise == true)
               StopCoroutine(AI_Movement());

            if (bulletObject = GameObject.Find("Bullet_Sound_Position"))
            {
                BulletHeard = bulletObject.GetComponent<Transform>();
                heardNoise = true;
                transform.LookAt(BulletHeard.position);
                transform.position = Vector3.MoveTowards(transform.position, BulletHeard.transform.position, Time.deltaTime * objectSpeed);
            }
            if (heardNoise == true)
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
            #endregion

            #region Hunting
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
            #endregion

            #region Alert
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

        if (currentHealth <= 0 && !headShot)
            Destroy(gameObject);
        else if (headShot && currentHealth <= 0)
        {
            // Make sure we make a seperate GameObject so we dont delete a reference Variable
            GameObject aiHead = head;
            // Detech The Head
            aiHead.transform.parent = null;
            // Destroy the parent
            Destroy(gameObject);
            // Add Rigidbody for Physics
            Rigidbody headRB = head.gameObject.AddComponent<Rigidbody>();
            // We want the head to fall to the ground
            headRB.useGravity = true;
            // Knock the head object back
            headRB.AddForce(-transform.forward * 500);
            // Destroy the head after 10 seconds 
            Destroy(aiHead, 10f);

        }


    }
    #endregion

        #region Damage Function
        public void ApplyDamage(int damage)
        {
            currentHealth -= damage;
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
        // Movement logic needs a revisit now we have a very basic wall detection system working
        // We have the issue that this now have some minor effect on the Different movement states
        IEnumerator AI_Movement()
        {
            // Simple dormant movement (Does work / Not needed)
            #region Dormant Movement
            if (states == NPC_States.Dormant)
            {
            //    #region Wall detection
            //    if (wallDetected)
            //    {
            //        Vector3 relativePos = whereTheAiGoes[walkArrayScroller] - transform.position;
            //        Quaternion rotation = Quaternion.LookRotation(relativePos);
            //        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
            //    }
            //    else
            //    {
            //        transform.position = Vector3.MoveTowards(transform.position, whereTheAiGoes[walkArrayScroller], Time.deltaTime * objectSpeed);
            //    }
            //// Vector , forward, left, right, up, down 1 float value == speed * time.
            //// Frames
            //// Seconds

            //transform.Translate(whereTheAiGoes[walkArrayScroller] * Time.deltaTime * objectSpeed / 4);
            //Transform leftRay = transform;
            //Transform rightRay = transform;

            ////Use Phyics.RayCast to detect the obstacle
            //if (Physics.Raycast(leftRay.position + (transform.right), transform.forward, out hit, wallDetectionRange) || Physics.Raycast(rightRay.position - (transform.right), transform.forward, out hit, wallDetectionRange))
            //{
            //    if (hit.collider.gameObject.CompareTag("Obstacles"))
            //    {
            //        wallDetected = true;
            //        transform.Rotate(Vector3.up * Time.deltaTime * rotationRate);
            //    }
            //}
            //// Now Two More RayCast At The End of Object to detect that object has already pass the obsatacle.
            //// Just making this boolean variable false it means there is nothing in front of object.
            //if (Physics.Raycast(transform.position - (transform.forward), transform.right, out hit, wallDetectionRange) ||
            // Physics.Raycast(transform.position - (transform.forward), -transform.right, out hit, wallDetectionRange))
            //{
            //    if (hit.collider.gameObject.CompareTag("Obstacles"))
            //    {
            //        wallDetected = false;
            //    }
            //}
            //#region Deubbing Wall Detection
            //// Use to debug the Physics.RayCast.
            //Debug.DrawRay(transform.position + (transform.right), transform.forward * wallDetectionRange, Color.red);
            //Debug.DrawRay(transform.position - (transform.right), transform.forward * wallDetectionRange, Color.red);
            //Debug.DrawRay(transform.position - (transform.forward), -transform.right * wallDetectionRange, Color.yellow);
            //Debug.DrawRay(transform.position - (transform.forward), transform.right * wallDetectionRange, Color.yellow);

            // Move the player
            // Vector3 Storage of direction
            moveDirection = (whereTheAiGoes[walkArrayScroller]);

            if(Vector3.Distance(moveDirection, transform.position) < 3)
            {
                walkArrayScroller++;
                if (walkArrayScroller >= whereTheAiGoes.Length)
                {
                    walkArrayScroller = 0;
                }
                else
                    transform.LookAt(whereTheAiGoes[walkArrayScroller]);
            }

            AI_Physics.SetDestination(moveDirection);

            //NavMeshPath path = new NavMeshPath();
            //// NavMesh Movement
            //if (AI_Physics.enabled)
            //    AI_Physics.CalculatePath(moveDirection, path);
            #endregion
        }
        #region Old Movement
        //            // Rotation Target Position - my current position
        //            //moveDirection = (whereTheAiGoes[walkArrayScroller]);

        //            //if (Vector3.Distance(moveDirection, transform.position) < 3)
        //            //{
        //            //    walkArrayScroller++;
        //            //    if (walkArrayScroller >= whereTheAiGoes.Length)
        //            //    {
        //            //        walkArrayScroller = 0;
        //            //    }
        //            //    else
        //            //        transform.LookAt(whereTheAiGoes[walkArrayScroller]);
        //            //}
        //            //transform.position = Vector3.MoveTowards(transform.position, whereTheAiGoes[walkArrayScroller], Time.deltaTime * objectSpeed);
        //        }
        #endregion

        #region Hunting Movement
        if (states == NPC_States.Hunting)
            {
                // New system
                // Store the defualt pathfinding Vectors
                // Find the players position
                // Find positions around the player where the AI can go
                // Move towards them

                // Old system
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
            if (states == NPC_States.Alert)
            {
                //   transform.LookAt(playerPosition.position);
                transform.position = Vector3.MoveTowards(transform.position, playerPosition.position, Time.deltaTime * objectSpeed);
            }
            #endregion
            yield break;
        }
       #endregion
    }