﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    #region Variables

    #endregion
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

    // Transform Component of the player character in the game world
    private Transform playerPosition;

    [Header("Overall Movement Variables")]
    // PUBLIC

    // How fast will the Object Move
    public float AI_movement_Speed;
    // Where the NPC starts in the world
    Vector3 startPosition;

    // PRIVATE

    // The Physics we can manipulate
    private NavMeshAgent AI_Physics;
    private CapsuleCollider AI_Collider;

    [Header("Searching Behaviour Variables")]
    // PUBLIC
    public float searchRadius = 10;
    // The new path the AI will follow
    Vector3 new_AI_Path;
    // PRIVATE

    // A way to get the maunal set up patrol variable
    private Vector3[] pathFindingStorage;

    #region Behaviour Timers
    // Timers which balance the states of play for NPCs
    // This value meets a random generated value which allows for the NPC to look for the player
    private float HuntingTime = 0;  
    // Value that agrees or disagrees if the ai is hunting
    private float TimeUntilSearching = 2;
    private float TimeUntilAlerted = 3;
    // The time that meets the random alert time
    // Its so the NPC can Behave alerted until the player is gone and hidden
    private float AlertedTime = 0; 
    // How long the player can be within vision until the AI is alert
    private float playerTimeInSight = 0;
    #endregion

    [Header("Time Until Reset Behaviour")]
    // PUBLIC
    // Random values to meet
    public float random_Alert_Value;    // Cant Edit
    public float random_Hunt_Value;     // Cant Edit
    // PRIVATE


    [Header("Dormant Behaviour Variables")]
    // PUBLIC
    public Vector3[] Patrol;    // Can Edit
    public Vector3 moveDirection;
    // PRIVATE

    // Int scrolls through an array of Vector3
    int patrolArrayScroller = 0;


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
    // The Sound was detected within this vector
    Vector3 playersLastPosition;
    // How far can the enemy hear
    public float detectionSoundRaduis = 6;
    // PRIVATE

    [Header("Shooting Variables")]
    // PUBLIC
    public bool headShot = false;
    public GameObject head;

    public int viewDistance = 60;  // Can Edit
    public int fieldOfView = 45;    // Can Edit 
    public LayerMask AI_Detections; // Can Edit
    // PRIVATE

    // Hit logic for the array itself
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        #region Find / Make Components
        // Component Set-Up
        // Find the navmeshagent
        AI_Physics = gameObject.AddComponent<NavMeshAgent>();
        // Make a collider
        AI_Collider = gameObject.AddComponent<CapsuleCollider>();
        #endregion

        #region Set up Components and variables || Object Setup

        // object layer is always 11 Dont change unless needed
        gameObject.layer = 11;
        // object name will be enemy
        gameObject.name = "Enemy";

        // Find the component that belongs to player
        playerPosition = GameObject.Find("PC").GetComponent<Transform>();

        // NavMeshAgent will adopt the spped variable
        AI_Physics.speed = AI_movement_Speed;

        // Vector or Position set-up
        // Home position
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        // First position in the array needs to be the startPosition
        Patrol[0] = startPosition;

        viewDistance = 60;
        fieldOfView = 45;
        // Health Set-up
        currentHealth = MaxHealth;
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

        #region Behaviour Change
        #region Searching
        if (states == AI_States.Searching)
        {
            Debug.Log(AI_States.Searching + ":" + "I'm Now Searching for The Player");
            HuntingTime += Time.deltaTime;
            if (HuntingTime > random_Hunt_Value)
            {
                random_Hunt_Value = 0;
                HuntingTime = 0;
                states = AI_States.Dormant;
            }
        }
        #endregion

        #region Alert
        if (states == AI_States.Alert)
        {
            Debug.Log(AI_States.Alert + ":" + "I'm Now Alerted and Will Hurt The Player . . .");
            AlertedTime += Time.deltaTime;
            if (AlertedTime > random_Alert_Value)
            {
                random_Alert_Value = 0;
                AlertedTime = 0;
                states = AI_States.Dormant;
            }
        }
        #endregion
        #endregion

        #region Timer Stop On Alert
        if (states == AI_States.Alert)
        {
            playerTimeInSight = 0;
        }
        #endregion

        #region Death Monitor
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
        #endregion
    }

    #region Damage Function
    public void ApplyDamage(int damage)
    {
        // reduce health with the damage value that gets passed through by the player (Its in the shooting mechanic)
        currentHealth -= damage;
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
                random_Hunt_Value = 0;
                random_Alert_Value = 0;
                HuntingTime = 0;
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
                random_Hunt_Value = Random.Range(minHuntTime, maxHuntTIme);
                GenerateNewPath();
                break;
            #endregion

            #region Alerted
            case AI_States.Alert:
                // Get A Random Value when we pass the value then we allow the AI to go back to its home and becomes dormant
                Debug.Log(gameObject.transform.name + ":" + "Alerted . . .");
                HuntingTime = 0;
                random_Hunt_Value = 0;
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
            moveDirection = (Patrol[patrolArrayScroller]);

            if (Vector3.Distance(moveDirection, transform.position) < 3)
            {
                patrolArrayScroller++;
                if (patrolArrayScroller >= Patrol.Length)
                {
                    patrolArrayScroller = 0;
                }
                else
                    transform.LookAt(Patrol[patrolArrayScroller]);
            }

            AI_Physics.SetDestination(moveDirection);
        }
        #endregion
        // Searching for the player
        #region Hunting Movement
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
            AI_Physics.SetDestination(playerPosition.position);
            yield return new WaitForSeconds(5);
            transform.LookAt(playerPosition.position);
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
                // Fire weapon with bad aim or at random stuff
                break;

            case AI_States.Alert:
                // Shoot PC with more accuracy

                Debug.Log("Make NPC go to Cover: Yes you still need to do that");
                break;
        }
        yield break;
    }
    #endregion
}