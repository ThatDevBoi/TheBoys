using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomPropertyDrawer(typeof(HideAttributes))]
public class AI : MonoBehaviour
{
    #region Default AI Variables
    // Variables that jave or will have more than 1 use. 
    // What i mean by this is we can use the playerPosition for movement and behaviour change 
    // But we also need it for shooting 
    [Header("Multi Use Variables")]
    // PUBLIC
    // The game states of the object
    public AI_States states = AI_States.Dormant;
    // Different game states the object can be influenced by
    public enum AI_States { Dormant, Searching, Alert };
    // Is the player is the FOV sight? 
    public bool PC_in_FOV = false;
    public bool i_Heard_Something;
    // PRIVATE

    [Header("Overall Movement Variables")]
    // PUBLIC
    // How fast will the Object Move
    public float AI_movement_Speed;
    // PRIVATE

    [Header("Searching Behaviour Variables")]
    // PUBLIC
    public float searchRadius = 10;
    // PRIVATE

    [Header("Time Until Reset Behaviour")]
    // PUBLIC
    // Random values to meet
    public float random_Alert_Value;   
    public float random_Search_Value;    
    // PRIVATE

    [Header("Dormant Behaviour Variables")]
    // PUBLIC
    public Vector3[] Patrol;
    private bool agentStopStore = false;
    public bool doesAgentStop = false;
    public int stopDuringPatrol;
    public float stoppingTimeLength = 1;
    // PRIVATE
    // Value that stores orginal value for editing
    private float stoptime;

    [Header("Health n Damage")]
    // PUBLIC
    public int MaxHealth;
    public int currentHealth;
    // PRIVATE

    // Hearing Player Noise
    [Header("Sound Detection")]
    // PUBLIC
    // Audio Source Running - Attached To Player Gun
    public AudioSource playerRunning;
    // Audio Source shooting - Attached To Player
    public AudioSource playerShooting;
    // How far can the enemy hear
    public float detectionSoundRaduis = 6;
    // PRIVATE

    [Header("Shooting Variables")]
    // PUBLIC
    public bool headShot = false;
    public GameObject head;
    // Rate of fire
    public float fireRate = 0.25f;
    // How accuracte is each shot
    public int firingAccuracy = 1000;
    // The Range of shooting
    public int shootingRange = 60;
    // What the AI can shoot
    public LayerMask AI_Detections;
    public Transform firePoint;
    // PRIVATE

    // Hit logic for the array itself
    private RaycastHit hit;

    [Header("User Feedback")]
    public MeshRenderer aiMeshRend;
    private Material dormantMat;
    private Material searchingMat;
    private Material alertMat;


    #region Debugging
    [HideInInspector]
    public bool Debugging;
    // Transform Component of the player character in the game world
    [Header("$Debugging$ Where the players position Component is")]
    [Header("For QA Tester")]
    [HideAttributes("Debugging", true)]
    public Transform playerPosition;
    // Where the NPC starts in the world
    [Header("$Debugging$ Where the AI Starts in the world (The patrol array 0 element always = this position)")]
    [HideAttributes("Debugging", true)]
    public Vector3 startPosition;
    // Int scrolls through an array of Vector3
    [Header("$Debugging$ The Value that scrolls through the Patrol Array")]
    [HideAttributes("Debugging", true)]
    public int patrolArrayScroller = 0;
    // The Physics we can manipulate
    [Header("$Debugging$ The NavMeshAgent Component")]
    [HideAttributes("Debugging", true)]
    public NavMeshAgent AI_Physics;
    [Header("$Debugging$ The NPC Collision")]
    [HideAttributes("Debugging", true)]
    public CapsuleCollider AI_Collider;
    // The new path the AI will follow
    [Header("$Debugging$ The Hunting behaviour new movement")]
    [HideAttributes("Debugging", true)]
    public Vector3 new_AI_Path;
    [Header("$Debugging$ Vector3 that tells the AI where to move")]
    [HideAttributes("Debugging", true)]
    public Vector3 moveDirection;
    // The Sound was detected within this vector
    [Header("$Debugging$ Noise Detection where the player was when they made a sound")]
    [HideAttributes("Debugging", true)]
    public Vector3 playersLastPosition;
    [Header("$Debugging$ How much the AI gets damaged per bullet")]
    [HideAttributes("Debugging", true)]
    public int damage = 5;
    [Header("$Debugging$ How Fast The AI Fires There gun ")]
    [HideAttributes("Debugging", true)]
    public float FireRateTimer;

    #region Behaviour Timers
    // Timers which balance the states of play for NPCs
    // This value meets a random generated value which allows for the NPC to look for the player
    [Header("$Debugging$ How long the AI Searches for the player")]
    [HideAttributes("Debugging", true)]
    public float SearchingTime = 0;
    // Value that agrees or disagrees if the ai is hunting
    [Header("$Debugging$ The Time it takes for the AI to detect the player for searching (Detects when in FOV)")]
    [HideAttributes("Debugging", true)]
    public float TimeUntilSearching = 1;
    [Header("$Debugging$ The Time it takes for the AI to detect the player for Alert")]
    [HideAttributes("Debugging", true)]
    public float TimeUntilAlerted = 2;
    // The time that meets the random alert time
    // Its so the NPC can Behave alerted until the player is gone and hidden
    [Header("$Debugging$ How long the AI is Alert for the player")]
    [HideAttributes("Debugging", true)]
    public float AlertedTime = 0;
    // How long the player can be within vision until the AI is alert
    [Header("$Debugging$ How long the Player is in sight (The value monitors how long the Player has been in the FOV angle)")]
    [HideAttributes("Debugging", true)]
    public float playerTimeInSight = 0;
    #endregion
    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    { 
        #region Find / Make Components
        // Component Set-Up
        // Find the navmeshagent
        AI_Physics = gameObject.AddComponent<NavMeshAgent>();
        // NavMeshAgent will adopt the spped variable
        AI_Physics.speed = AI_movement_Speed;
        // Make a collider
        AI_Collider = gameObject.AddComponent<CapsuleCollider>();
        aiMeshRend = gameObject.GetComponent<MeshRenderer>();

        // Find the component that belongs to player
        playerPosition = GameObject.Find("PC").GetComponent<Transform>();
        // Find the players walking and running Audio Source
        playerRunning = GameObject.Find("PC").GetComponent<AudioSource>();
        // Finding the Audio Source Attached to the players Gun
        // This will change later where 1 audio source will be on the Weapon_Holder and will change audio source when player changes weapon (We can find the audio clip via Assets)
        playerShooting = GameObject.Find("PC/FPS_Cam/Weapon_Holder/Pistol Holder/Pistol").GetComponent<AudioSource>();

        // Find My Head
        head = GameObject.Find("Head");
        firePoint = gameObject.transform.Find("Gun/Shooting_Point").GetComponent<Transform>();

        // Find the materials from the assets folder
        dormantMat = Resources.Load<Material>("Material/Dormant");
        searchingMat = Resources.Load<Material>("Material/Searching");
        alertMat = Resources.Load<Material>("Material/Alert");
        // Placeholder //
        // Find Materials
        #endregion

        #region Set up Components and variables || Object Setup
        // object layer is always 11 Dont change unless needed
        gameObject.layer = 11;
        // object name will be enemy
        gameObject.name = "Enemy";

        // Vector or Position set-up
        // Home position
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // First position in the array needs to be the startPosition
        Patrol[0] = startPosition;
        // Health Set-up
        currentHealth = MaxHealth;
        // change start matieral to the dormant material
        aiMeshRend.material = dormantMat;
        // Stop value so console never forgets it / looses it
        stoptime = stoppingTimeLength;

        agentStopStore = doesAgentStop;
        #endregion
        // The Zone that makes sure the object is set up correctly
        #region Debugging Zone
        if (states != AI_States.Dormant)
            Debug.LogError(gameObject.name + ":" + "The Starting enum Behaviour is wrong");

        if (Patrol.Length < 0)
            Debug.LogWarning(gameObject.name + ":" + "Patrol Array is not been set up");
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Heard Noise
        // If we have not heard a noise
        if (i_Heard_Something == false)
        {
            // Run The relevent code now
            StartCoroutine(AI_Movement());
        }
        // if we have in fact heard a noise
        else if (i_Heard_Something == true)
            StopCoroutine(AI_Movement());   // Stop moving we need to move somewhere else now
        // We need to check if the player is in a good distance
        // Without the distance whenever the player fires their gun or runs ALL AI WILL KNOW
        if (Vector3.Distance(transform.position, playerPosition.position) < detectionSoundRaduis)
        {
            // If the volume from the audioSources are increased 
            if (playerRunning.volume > 0.3f || playerShooting.volume > 0.2f)
            {
                // Find where the player was
                playersLastPosition = new Vector3(playerPosition.position.x, transform.position.y, playerPosition.position.z);
                // tick the boolean
                i_Heard_Something = true;
                doesAgentStop = false;
                AI_Physics.speed = 1;
                stoppingTimeLength = 0.1f;
            }
        }
        // when the boolean is true
        if (i_Heard_Something)
        {
            // Look at the Vector
            transform.LookAt(playersLastPosition);
            // Move with the navmesh
            AI_Physics.SetDestination(playersLastPosition);
            // when the current position of this gameObject is at the noise position
            if(transform.position.x == playersLastPosition.x)
            {
                // cant hear any more noise
                i_Heard_Something = false;
                doesAgentStop = agentStopStore;
            }
        }
        #endregion

        #region Behaviour Conditions
        // if the player is within view of the AI
        if (PC_in_FOV)
        {
            // Alert Logic
            // We count up the Player In Sight Value
            playerTimeInSight += Time.deltaTime;
            // If the detection value is greater than the time it takes to detect
            if (playerTimeInSight > TimeUntilAlerted)
            {
                playerTimeInSight = 0;  // reset the value
                states = AI_States.Alert;
                Randomizer();
            }

            if (playerTimeInSight > TimeUntilSearching)
            {
                states = AI_States.Searching;
                Randomizer();
            }
        }
        // However
        else if (!PC_in_FOV)
        {
            playerTimeInSight = 0;
            PC_in_FOV = false;
        }
        #endregion

        #region Debuggng Key Press Logic
        // Pressing R Space Bar And Y all at once shows hidden properties (For QA or testing)
        if(Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Y))
        {
            Debugging = true;
        }
        // Pressing D and M at the same time reverts and hides the properites.
        else if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.M))
        {
            Debugging = false;
        }
        #endregion

        #region Behaviour Change
        #region Dormant
        if (states == AI_States.Dormant)
        {
            // Find FOV script
            FieldOfView FOVscript = gameObject.GetComponent<FieldOfView>();
            // Change raduis back to normal
            FOVscript.viewRadius = 6;
            // Change material
            aiMeshRend.material = dormantMat;
            #region StopDuring Patrol
            if (doesAgentStop)
            {
                // Stop patrolling until StoppingTimeLength reaches 0
                // This logic could allow AI patrols to have lerp rotation etc
                if (stopDuringPatrol == patrolArrayScroller)
                {
                    AI_Physics.speed = 0;
                    stoppingTimeLength -= Time.deltaTime;
                    Debug.Log(stoppingTimeLength);
                    if (stoppingTimeLength <= 0)
                    {
                        AI_Physics.speed = 1;
                    }
                }
                else
                {
                    // Timer equals what the edit manual settings is 
                    stoppingTimeLength = stoptime;
                }
            }
            #endregion
        }
        #endregion

        #region Searching
        // if the gameObject is Searching for the player
        if (states == AI_States.Searching)
        {
            // Change material for visual feedback
            aiMeshRend.material = searchingMat;
            // find the fov script attached to the gameObject
            FieldOfView FOVscript = gameObject.GetComponent<FieldOfView>();
            // change the radius 
            FOVscript.viewRadius = 8;
            // Debug Message so we can see the state changing
            Debug.Log(AI_States.Searching + ":" + "I'm Now Searching for The Player");      // REMOVE LATER
            // Increase the time we search for 
            SearchingTime += Time.deltaTime;
            // if the current search time is greater than the one we generate in Randomizer()
            if (SearchingTime > random_Search_Value)
            {
                // Reset the random generated value
                random_Search_Value = 0;
                // reset current time
                SearchingTime = 0;
                // We are now dormant back to patrolling
                states = AI_States.Dormant;
            }
        }
        #endregion

        #region Alert
        // if we are alert
        if (states == AI_States.Alert)
        {
            // Change material for visual feedback
            aiMeshRend.material = alertMat;
            // find the script
            FieldOfView FOVscript = gameObject.GetComponent<FieldOfView>();
            // change the radius 
            FOVscript.viewRadius = 10;
            // Show we are alert in the console
            Debug.Log(AI_States.Alert + ":" + "I'm Now Alerted and Will Hurt The Player . . .");        // REMOVE LATER
            // increase the current alert time value
            AlertedTime += Time.deltaTime;
            // if the current alert time value is greater than the random generated one in Randomizer()
            if (AlertedTime > random_Alert_Value)
            {
                // reset the generated value
                random_Alert_Value = 0;
                // reset current time for alert
                AlertedTime = 0;
                // Reset and back to dormant patrol state 
                states = AI_States.Dormant;
            }
        }
        #endregion
        #endregion

        #region Timer Stop On Alert
        // if we are alert then we never allow the value to increase 
        if (states == AI_States.Alert)
        {
            playerTimeInSight = 0;
        }
        #endregion

        #region Death Monitor
        // if the current health is 0 and the headshot bool is false
        if (currentHealth <= 0 && !headShot)
            Destroy(gameObject);    // we just remove the object
        else if (headShot && currentHealth <= 0)    // however if we have no health but the bool is true
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
        #endregion

        // Fire Rate for gun
        if (FireRateTimer < fireRate)
            FireRateTimer += Time.deltaTime;

        // We dont want to run the i heard something logic when the AI already knows where the player is 
        if (states == AI_States.Alert && i_Heard_Something)
            i_Heard_Something = false;
        else if (states == AI_States.Searching && i_Heard_Something)
            i_Heard_Something = false;
        // Fire weapon (We can call this wheenever. We use a Switch stateent in the function)
        StartCoroutine("FireWeapon");
    }

    #region Damage Function
    public void ApplyDamage(int damage)
    {
        // reduce health with the damage value that gets passed through by the player (Its in the shooting mechanic)
        currentHealth -= damage;
    }
    #endregion


    #region Shoot Weapon
    public void Shoot()
    {
        #region Accuracys
        Vector3 BulletSpread(Vector3 originVector, int accuracy, bool showDebug = false, Vector3 debugPosition = default(Vector3))
        {
            // Set random values for the accuracy
            float myIntx = (float)Random.Range(-accuracy, accuracy) / 1000;
            float myInty = (float)Random.Range(-accuracy, accuracy) / 1000;
            float myIntz = (float)Random.Range(-accuracy, accuracy) / 1000;
            // New firing vector
            Vector3 newVector = new Vector3(originVector.x + myIntx, originVector.y + myInty, originVector.z + myIntz);

            if (showDebug)
            {
                Debug.DrawRay(debugPosition, originVector * 100f, Color.cyan);
                Debug.DrawRay(debugPosition, newVector * 100f, Color.red);
            }
            return newVector;
        }
        #endregion
        // if the firerate timer is less than the acutal fire rate 
        if (FireRateTimer < fireRate) return;   // return
        FireRateTimer = 0.0f;   // if not reset the timer

        if(Physics.Raycast(firePoint.position, BulletSpread(firePoint.forward, firingAccuracy, true, firePoint.position), out hit, shootingRange, AI_Detections))
        {
            // if we hit the player
            if (hit.transform.gameObject.layer == 10)
            {
                Debug.Log("I Hit The Player");
                Player_Controller applyDamage = GameObject.Find("PC").GetComponent<Player_Controller>();// get PC script
                applyDamage.HitDetection(gameObject.GetComponent<Transform>());
                applyDamage.ApplyDamage(damage);    // apply damage to the player
            }
            
        }

    }
    #endregion

    #region Behaviour Length Calculator
    void Randomizer()
    {
        switch (states)
        {
            #region Dormant
            case AI_States.Dormant:
                // Reset everything when the AI goes from Hunting to Dormant || Alert to Dormant
                Debug.Log(gameObject.transform.name + ":" + "Dormant . . .");
                random_Search_Value = 0;
                random_Alert_Value = 0;
                SearchingTime = 0;
                AlertedTime = 0;
                // Reset Searching Path
                new_AI_Path = new Vector3(0, 0, 0);
                break;
            #endregion

            #region Searching
            case AI_States.Searching:
                // We are hunting we set random values so the AI can hunt until the value is met. 
                Debug.Log(gameObject.transform.name + ":" + "Searching . . .");
                AlertedTime = 0;
                random_Alert_Value = 0;
                // We randomise the Hunting to - Dormant so the NPC does whatever until the valid time
                float minHuntTime = 20;
                float maxHuntTIme = 30;
                // Randomise the value
                random_Search_Value = Random.Range(minHuntTime, maxHuntTIme);
                GenerateNewPath();
                break;
            #endregion

            #region Alerted
            case AI_States.Alert:
                // Get A Random Value when we pass the value then we allow the AI to go back to its home and becomes dormant
                Debug.Log(gameObject.transform.name + ":" + "Alerted . . .");
                SearchingTime = 0;
                random_Search_Value = 0;
                float minAlertTime = 30;
                float maxAlertTime = 40;
                // Generate random alert value
                random_Alert_Value = Random.Range(minAlertTime, maxAlertTime);
                break;
                #endregion
        }
    }
    #endregion

    #region Search Path Generation
    void GenerateNewPath()
    {
        // The Path will be 
        Vector3 distanceBetweenObjects = transform.position - playerPosition.position;

        // Generate Random Search Position
        Vector3 newGeneratedPath = new Vector3(Random.value, gameObject.transform.position.y, Random.value) - distanceBetweenObjects;
        // This is our new path
        new_AI_Path = newGeneratedPath;

        Debug.Log(newGeneratedPath + "New Path");
    }
    #endregion

    #region Movement Logic
    // Movement logic needs a revisit now we have a very basic wall detection system working
    // We have the issue that this now have some minor effect on the Different movement states
    IEnumerator AI_Movement()
    {
        // Simple dormant movement (Does work / Not needed)
        // Patrolling
        #region Dormant Movement
        // Move the player
        // Vector3 Storage of direction
        if (states == AI_States.Dormant)
        {
             // takes into account the Vector3 Array
            moveDirection = (Patrol[patrolArrayScroller]);
            // if the distance between the AI and movedirection is less than 3
            if (Vector3.Distance(moveDirection, transform.position) < 3)
            {
                // go to next position in the array
                patrolArrayScroller++;
                if (patrolArrayScroller >= Patrol.Length)   // if the int that scrolls through the array is greater than the length of the array
                {
                    patrolArrayScroller = 0;    // reset the scroller int so we can patrol the same manual patrol
                }
                else   // however if we still have positions to go to
                    transform.LookAt(Patrol[patrolArrayScroller]);  // Lookat the next position
            }
            // Move to the Vector3 with The NavMeshAgent
            AI_Physics.SetDestination(moveDirection);
        }
        #endregion
        // Searching for the player
        #region Searching Movement
        if (states == AI_States.Searching)
        {
            AI_Physics.SetDestination(new_AI_Path);
            // if the value between both objects is greater than our search radius we cant go any further
            if(Vector3.Distance(transform.position, playerPosition.position) > searchRadius)    // if the player is outside the search radius turn back
            {
                Debug.Log("Player Out Of Range" + ":" + AI_States.Dormant);
            }
            // When the current Position is equal to the new path
            if(transform.position.x == new_AI_Path.x)
            {
                // Make a new path
                GenerateNewPath();

                StartCoroutine("FireWeapon");
            }
        }
        #endregion

        // Found and alerted of the player
        #region Alert Movement
        if (states == AI_States.Alert)
        {
            // We only move towards the players position 
            AI_Physics.SetDestination(playerPosition.position);
            // Every 5 Seconds
            yield return new WaitForSeconds(5);
            // We look straight at the player
            transform.LookAt(playerPosition.position);
            // We also fire the AI weapon 
            StartCoroutine("FireWeapon");
            
        }
        #endregion
        yield break;
    }
    #endregion

    #region AI Fire Weapon
    IEnumerator FireWeapon()
    {
        switch(states)
        {
            case AI_States.Dormant:
                yield break;

            case AI_States.Searching:
                firingAccuracy = 100;
                Shoot();
                break;

            case AI_States.Alert:
                firingAccuracy = 30;
                Shoot();
                // Shoot PC with more accuracy

                Debug.Log("Make NPC go to Cover: Yes you still need to do that");
                break;
        }
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Fist")
        {
            ApplyDamage(damage);
        }
    }
    #endregion
}