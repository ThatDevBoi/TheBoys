using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

#if (UNITY_EDITOR) 
[CustomPropertyDrawer(typeof(HideAttributes))]
[RequireComponent(typeof(FieldOfView))]
#endif
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
    public float AI_Movement_dormantSpeed = 1.0f;
    public float AI_movement_searchingSpeed = 1.5f;
    public float AI_movement_alertSpeed = 2.0f;
    public float AI_Stop_Distance = 7;
    float speed;

    // PRIVATE
    // How fast will the Object Move
    private float AI_movement_Speed;

    [Header("Searching Behaviour Variables")]
    // PUBLIC
    public float searchRadius = 10;
    public float TimeUntilAlerted = 2;
    public float TimeUntilSearching = 1;

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
    public int howmanyHits = 0;
    public float howManyHitsReset = 3f;
    

    [Header("Health n Damage")]
    // PUBLIC
    public int MaxHealth;
    public int currentHealth;
    // PRIVATE

    // Hearing Player Noise
    [Header("Sound Detection")]
    // PUBLIC
    // Audio Source Running - Attached To Player Gun
    public AudioSource playerRunning_Audio;
    // Audio Source shooting - Attached To Player
    public AudioSource playerShooting_Audio;
    // How far can the enemy hear
    public float detectionSoundRaduis = 6;
    // PRIVATE

    [Header("Shooting Variables")]
    // PUBLIC
    public bool headShot = false;
    public Transform head;
    // Rate of fire
    public float fireRate = 0.25f;
    // How accuracte is each shot
    public int firingAccuracy = 1000;
    // How much damage the AI Does per shot
    public int damage = 5;
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
    private GameObject damageText;
    [SerializeField]
    private GameObject searchingGO;
    [SerializeField]
    private GameObject AlertGO;

    [Header("FOV")]
    public float dormantFOVRadius = 9;
    public float searchingFOVRadius = 11;
    public float alertFOVRadius = 13;

    [Header("Knockback")]
    public float force = 8;
    public float knockbackTime = 0.4f;
    public bool pushback = false;
    // direction of push back
    Vector3 direction = Vector3.forward;

    public static int ult_Counter = 1;


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
    public Quaternion playersLastRotation;
    [HideAttributes("Debugging", true)]
    public Vector3 playersLastPosition;
    [Header("$Debugging$ How Fast The AI Fires There gun ")]
    [HideAttributes("Debugging", true)]
    public float FireRateTimer;
    [Header("$Debugging$ Boolean to tell the AI that it has stopped and reached the distance between the player and itself where it can stop")]
    [HideAttributes("Debugging", true)]
    public bool stoppingDistance = false;
    #region Behaviour Timers
    // Timers which balance the states of play for NPCs
    // This value meets a random generated value which allows for the NPC to look for the player
    [Header("$Debugging$ How long the AI Searches for the player")]
    [HideAttributes("Debugging", true)]
    public float SearchingTime = 0;
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
        if (gameObject.GetComponent<NavMeshAgent>() != null)
            AI_Physics = gameObject.GetComponent<NavMeshAgent>();
        else
            // Find the navmeshagent
            AI_Physics = gameObject.AddComponent<NavMeshAgent>();
        // NavMeshAgent will adopt the spped variable
        AI_Physics.speed = AI_Movement_dormantSpeed;

        // Make a collider
        //AI_Collider = gameObject.AddComponent<CapsuleCollider>();
        AI_Collider = this.gameObject.GetComponent<CapsuleCollider>();
        //aiMeshRend = gameObject.GetComponent<MeshRenderer>();
        // Finding the Audio Source Attached to the players Gun
        if (GameObject.Find("PC") == null)
        {
            Debug.LogWarning("Level Manager Will Find Player and its variables");
        }
        else
        {
            // This will change later where 1 audio source will be on the Weapon_Holder and will change audio source when player changes weapon (We can find the audio clip via Assets)
            playerShooting_Audio = GameObject.Find("PC/FPS_Cam/Weapon_Holder/Pistol Holder/Pistol").GetComponent<AudioSource>();
            // Find the component that belongs to player
            playerPosition = GameObject.Find("PC").GetComponent<Transform>();
            // Find the players walking and running Audio Source
            playerRunning_Audio = GameObject.Find("PC").GetComponent<AudioSource>();
        }

        // Find My Head
        head = gameObject.transform.GetChild(2);
        firePoint = gameObject.transform.Find("Gun/Shooting_Point").GetComponent<Transform>();
        #region Find Variables Thorugh Assets
        // Find the materials from the assets folder
        dormantMat = Resources.Load<Material>("Material/Dormant");
        searchingMat = Resources.Load<Material>("Material/Searching");
        alertMat = Resources.Load<Material>("Material/Alert");

        damageText = Resources.Load<GameObject>("Prefabs/AI/Feedback/FloatingText");
        searchingGO = Resources.Load<GameObject>("Prefabs/AI/Feedback/QuestionMark");
        AlertGO = Resources.Load<GameObject>("Prefabs/AI/Feedback/!");
        #endregion
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
        //aiMeshRend.material = dormantMat;
        // Stop value so console never forgets it / looses it
        stoptime = stoppingTimeLength;

        agentStopStore = doesAgentStop;
        #endregion
        // The Zone that makes sure the object is set up correctly
        #region Debugging Zone
        if (states != AI_States.Dormant)
            Debug.LogError(gameObject.name + ":" + "The Starting enum Behaviour is wrong");

        if (Patrol.Length == 0)
            Debug.LogWarning(gameObject.name + ":" + "Patrol Array is not been set up");
        #endregion

        #region Spawning
        Vector3 GO_Search_instance_Pos = new Vector3(head.transform.position.x, head.transform.position.y + 0.5f, head.transform.position.z);
        GameObject searchGO_instance = Instantiate(searchingGO, GO_Search_instance_Pos, Quaternion.identity, gameObject.transform) as GameObject;
        searchGO_instance.name = "QuestionMark";
        searchGO_instance.SetActive(false);

        Vector3 GO_Alert_instance_Pos = new Vector3(head.transform.position.x, head.transform.position.y + 0.5f, head.transform.position.z);
        GameObject AlertGO_instance = Instantiate(AlertGO, GO_Search_instance_Pos, Quaternion.identity, gameObject.transform) as GameObject;
        AlertGO_instance.name = "!";
        AlertGO_instance.SetActive(false);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        AI_Physics.updateRotation = true;
        // Find the mesh 
        gameObject.transform.GetChild(4).gameObject.SetActive(true);

        // Knocback Pushback
        if (pushback)
        {
            int i = Random.Range(1, 100) - 50;
            Debug.Log("Pushback Chance" + ":" + i);
            if (i >= 40)
            {
                AI_Physics.updateRotation = false;
                // push navmesh back
                AI_Physics.velocity = -direction * force;
            }
            else
                pushback = false;

        }

        // this line of code runs when the level manager is setting everything up
        if (playerPosition == null && playerRunning_Audio == null && playerShooting_Audio == null)
        {
            // Find the component that belongs to player
            playerPosition = GameObject.Find("PC").GetComponent<Transform>();
            // Find the players walking and running Audio Source
            playerRunning_Audio = GameObject.Find("PC").GetComponent<AudioSource>();
            // This will change later where 1 audio source will be on the Weapon_Holder and will change audio source when player changes weapon (We can find the audio clip via Assets)
            playerShooting_Audio = GameObject.Find("PC/FPS_Cam/Weapon_Holder/Pistol Holder/Pistol").GetComponent<AudioSource>();
        }

        // Functions
        enumMonitor();
        HearingDetection();

        // The Navmesh agent speed property teams with AI_Movement_Speed Variable
        if (!stoppingDistance)
        {
            AI_Physics.speed = AI_movement_Speed;
        }
        else
            AI_Physics.speed = 0;

        if (GameObject.Find("PC").GetComponent<Player_Controller>().playerDead == true)
        {
            AI_Physics.speed = AI_movement_Speed;
            stoppingDistance = false;
            // reset the generated value
            random_Alert_Value = 0;
            // reset current time for alert
            AlertedTime = 0;
            states = AI_States.Dormant;
        }


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

            if (GameObject.Find("PC").GetComponent<Player_Controller>().playerDead == true)
            {
                states = AI_States.Dormant;
                PC_in_FOV = false;
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
        if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Y))
        {
            Debugging = true;
        }
        // Pressing D and M at the same time reverts and hides the properites.
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.M))
        {
            Debugging = false;
        }
        #endregion
        #region Death Monitor
        // if the current health is 0 and the headshot bool is false
        if (currentHealth <= 0 && !headShot)
        {
            GameManager script = GameObject.Find("Level Manager").GetComponent<GameManager>();
            script.Points_until_Ult += ult_Counter;
            Destroy(gameObject);    // we just remove the object
        }
        else if (headShot && currentHealth <= 0)    // however if we have no health but the bool is true
        {
            // Make sure we make a seperate GameObject so we dont delete a reference Variable
            Transform aiHead = head;
            // Find the head child as a object
            GameObject GO_Head = this.gameObject.transform.GetChild(2).gameObject;
            // Detech The Head from its body
            aiHead.transform.parent = null;
            // Destroy the parent
            Destroy(gameObject);
            //// Add Rigidbody for Physics
            Rigidbody headRB = head.gameObject.AddComponent<Rigidbody>();
            //// We want the head to fall to the ground
            headRB.useGravity = true;
            //// Knock the head object back
            headRB.AddForce(-transform.forward * 500);
            GameManager script = GameObject.Find("Level Manager").GetComponent<GameManager>();
            script.Points_until_Ult += ult_Counter;
            //// Destroy the head after 10 seconds 
            Destroy(GO_Head, 10f);

        }
        #endregion

        #region Behaviour Conditions
        // Fire Rate for gun
        if (FireRateTimer < fireRate)
            FireRateTimer += Time.deltaTime;
        // when the hits is 1 we need to search and we need to make sure we are not alert too
        if (howmanyHits == 1 && states != AI_States.Alert && states != AI_States.Searching)
        {
            // We are searching
            states = AI_States.Searching;
            Randomizer();
            howManyHitsReset = 3;
        }
        // if we are not searching then we need to make this check
        else if (states == AI_States.Searching)
        {
            // Countdown the reset
            howManyHitsReset -= Time.deltaTime;
            // if the howmanyHits is less than the required value and the reset has reached 0 or more
            if (howManyHitsReset <= 0)
            {
                // reset the hit
                howmanyHits = 0;
                // reset the cooldown
                howManyHitsReset = 3;
            }
        }
        // Detect how many hits have been received and then change states
        if (howmanyHits == 2)
        {
            // change AI state to alert as the AI has been shot more than once
            states = AI_States.Alert;
            Randomizer();
            howManyHitsReset = 3;
            howmanyHits = 0;
        }
        else if (states != AI_States.Alert)
        {
            // Countdown the reset
            howManyHitsReset -= Time.deltaTime;
            // if the howmanyHits is less than the required value and the reset has reached 0 or more
            if (howManyHitsReset <= 0)
            {
                // reset the hit
                howmanyHits = 0;
                // reset the cooldown
                howManyHitsReset = 3;
            }
        }

        // if we are alert then we never allow the value to increase 
        if (states == AI_States.Alert)
        {
            playerTimeInSight = 0;
        }

        // We dont want to run the i heard something logic when the AI already knows where the player is 
        if (states == AI_States.Alert && i_Heard_Something)
            i_Heard_Something = false;
        else if (states == AI_States.Searching && i_Heard_Something)
            i_Heard_Something = false;

        // Fire weapon (We can call this wheenever. We use a Switch stateent in the function)
        StartCoroutine("FireWeapon");
        #endregion
    }

    #region Detectors
    void enumMonitor()
    {
        #region Behaviour Change
        #region Dormant
        if (states == AI_States.Dormant)
        {
            if(gameObject.transform.GetChild(5).gameObject.activeSelf == true || gameObject.transform.GetChild(6).gameObject.activeSelf == true)
            {
                gameObject.transform.GetChild(5).gameObject.SetActive(false);
                gameObject.transform.GetChild(6).gameObject.SetActive(false);
            }
            if (!pushback)
                AI_movement_Speed = AI_Movement_dormantSpeed;
            else
                AI_Physics.SetDestination(direction);

            // Find FOV script
            FieldOfView FOVscript = gameObject.GetComponent<FieldOfView>();
            // Change raduis back to normal
            FOVscript.viewRadius = dormantFOVRadius;
            // Change material
            //aiMeshRend.material = dormantMat;
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
            gameObject.transform.GetChild(5).gameObject.SetActive(false);

            GameObject searchMesh = gameObject.transform.GetChild(5).gameObject;
            searchMesh.SetActive(true);
            if (states == AI_States.Dormant || states == AI_States.Alert)
                searchMesh.SetActive(false);
            if (!pushback)
                AI_movement_Speed = AI_movement_searchingSpeed;
            else
                AI_Physics.SetDestination(direction);
            // Change material for visual feedback
            //aiMeshRend.material = searchingMat;
            // find the fov script attached to the gameObject
            FieldOfView FOVscript = gameObject.GetComponent<FieldOfView>();
            // change the radius 
            FOVscript.viewRadius = searchingFOVRadius;
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
            gameObject.transform.GetChild(5).gameObject.SetActive(false);
            searchingGO.SetActive(false);
            GameObject alertMesh = gameObject.transform.GetChild(6).gameObject;
            alertMesh.SetActive(true);
            if (!pushback)
                AI_movement_Speed = AI_movement_alertSpeed;
            else
                AI_Physics.SetDestination(direction);


            // Change material for visual feedback
            //aiMeshRend.material = alertMat;
            // find the script
            FieldOfView FOVscript = gameObject.GetComponent<FieldOfView>();
            // change the radius 
            FOVscript.viewRadius = alertFOVRadius;
            // Show we are alert in the console
            Debug.Log(AI_States.Alert + ":" + "I'm Now Alerted and Will Hurt The Player . . .");        // REMOVE LATER
            // increase the current alert time value
            AlertedTime += Time.deltaTime;
            // if the current alert time value is greater than the random generated one in Randomizer()
            if (AlertedTime > random_Alert_Value)
            {
                //// need to reset how many hits the ai registered before we can transition back to dormant
                //howmanyHits = 0;
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
    }

    void HearingDetection()
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
        {
            gameObject.transform.GetChild(5).gameObject.SetActive(true);
            StopCoroutine(AI_Movement());   // Stop moving we need to move somewhere else now
        }
        // We need to check if the player is in a good distance
        // Without the distance whenever the player fires their gun or runs ALL AI WILL KNOW
        if (Vector3.Distance(transform.position, playerPosition.position) < detectionSoundRaduis)
        {
            // If the volume from the audioSources are increased 
            if (playerRunning_Audio.volume > 0.3f || playerShooting_Audio.volume > 0.2f)
            {
                // Find where the player was
                playersLastPosition = new Vector3(playerPosition.position.x, transform.position.y, playerPosition.position.z);
                // Rotate towards the player data
                playersLastRotation = Quaternion.LookRotation(new Vector3(playersLastPosition.x, 0, playersLastPosition.z));
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
            transform.rotation = Quaternion.Slerp(transform.rotation, playersLastRotation, Time.deltaTime * 1);
            if (!pushback)
            {
                // Move with the navmesh
                AI_Physics.SetDestination(playersLastPosition);
            }
            else
                AI_Physics.SetDestination(direction);
            // when the current position of this gameObject is at the noise position
            if (transform.position.x == playersLastPosition.x)
            {
                // cant hear any more noise
                i_Heard_Something = false;
                doesAgentStop = agentStopStore;
            }
        }
        #endregion
    }
    #endregion

    #region Damage Function
    public void ApplyDamage(int damage)
    {
        // reduce health with the damage value that gets passed through by the player (Its in the shooting mechanic)
        currentHealth -= damage;
        howmanyHits++;
        #region Pop up text spawn
        // rotation to make sure the text GameObject always spawns facing the player 
        Quaternion enumRotation_Changer = Quaternion.Slerp(transform.rotation, playerPosition.rotation, Time.time);

        // spawn text feedback
        GameObject instanceText = Instantiate(damageText, new Vector3(transform.position.x - 1, transform.position.y + 4, transform.position.z), enumRotation_Changer) as GameObject;
        
        // Set the tag
        instanceText.tag = "AI_UI";
        // print the health
        instanceText.GetComponent<TextMesh>().text = currentHealth.ToString();
        #endregion
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
                //
                //Canvas hitdetecter = playerPosition.gameObject.GetComponent<Canvas>();
                //hitdetecter.enabled = true;
                // 
                Player_Controller applyDamage = GameObject.Find("PC").GetComponent<Player_Controller>();// get PC script
                applyDamage.HitDetection(gameObject.GetComponent<Transform>());
                //applyDamage.HitDetection(gameObject.GetComponent<Transform>(), false);
                applyDamage.ApplyDamage(damage);    // apply damage to the player
            }
        }

    }
    #endregion

    #region Knockback
    public IEnumerator Knockback()
    {
        pushback = true;    // push back happens now

        yield return new WaitForSeconds(knockbackTime);

        pushback = false;
        AI_Physics.speed = AI_movement_Speed;
        AI_Physics.angularSpeed = 180;
        AI_Physics.acceleration = 10;
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
        Vector3 distanceBetweenObjects = transform.position + playerPosition.position;

        // Generate Random Search Position
        Vector3 newGeneratedPath = new Vector3(Random.value, gameObject.transform.position.y, Random.value) - distanceBetweenObjects;
        // This is our new path
        new_AI_Path = newGeneratedPath;

        //Debug.Log(newGeneratedPath + "New Path");
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
            }

            if (!pushback)
                // Move to the Vector3 with The NavMeshAgent
                AI_Physics.SetDestination(moveDirection);
            else
                AI_Physics.SetDestination(direction);

        }
        #endregion
        // Searching for the player
        #region Searching Movement
        if (states == AI_States.Searching)
        {
            moveDirection = new_AI_Path;
            if (!pushback)
                AI_Physics.SetDestination(moveDirection);
            else
                AI_Physics.SetDestination(direction);

            // if the value between both objects is greater than our search radius we cant go any further
            if (Vector3.Distance(transform.position, playerPosition.position) > searchRadius)    // if the player is outside the search radius turn back
            {
                Debug.Log("Player Out Of Range" + ":" + AI_States.Dormant);
                states = AI_States.Dormant;
            }

            // When the current Position is equal to the new path
            if (transform.position.x == new_AI_Path.x)
            {
                // Make a new path
                GenerateNewPath();
                Debug.Log(new_AI_Path);
                StartCoroutine("FireWeapon");
            }
        }
        #endregion

        // Found and alerted of the player
        #region Alert Movement
        if (states == AI_States.Alert)
        {
            moveDirection = playerPosition.position;
            if (!pushback)
                // We only move towards the players position 
                AI_Physics.SetDestination(moveDirection);
            else
                AI_Physics.SetDestination(direction);

            // Stopping Distance
            if (Vector3.Distance(transform.position, playerPosition.position) < AI_Stop_Distance)
            {
                stoppingDistance = true;
            }
            else
                stoppingDistance = false;
            // Every 2 Seconds
            yield return new WaitForSeconds(2);
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