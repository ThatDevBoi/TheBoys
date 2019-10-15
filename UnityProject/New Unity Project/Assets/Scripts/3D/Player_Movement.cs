using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    // Variables
    // IDE
    // Try and keep your variables that we dont need to see or edit private
    // You can expose variables by using [SerializeField] or [ShowInInspector]
    // I use these to remind me  that a variable is private and that im only exposing it for debugging or playtesting
    // if its public its to edit and find the correct values for gameplay a good way to do this is playtest until you like whats happening in your game
    // then set the values in Start() or as its defualt variable value
    // So if speed is best at being 5 set speed to 5 when you make the variable [public float speed = 5f;]

    // Remember make sensible namespaces your namespace is what you call the variable 
    /*public int --> coinValue <-- Thats the namespace*/
    // Rigidbody Reference
    Rigidbody player_RB;
    // BoxCollider Reference
    BoxCollider playerCollision;
    // Vector3 size of the BoxCollider
    public Vector3 playerBCSize;
    // Centre the BoxCollider 
    public Vector3 playerBCcentre;
    // Value of mass on the Rigidbody
    public int playerRBMass;
    // Boolean that decides if the rigidbody Use Gravity is on or off
    public bool playerRBGravity;

    private void Awake()
    {
        transform.gameObject.tag = "Player";
    }
    // Start is called before the first frame update
    void Start()
    {
        // ##Important Notice##
        // When setting values make sure you find IDE components first
        // You cant set values first then find the Component your are trying to save

        //Getting Components and setting values
        // Find the components
        player_RB = GetComponent<Rigidbody>();
        playerCollision = GetComponent<BoxCollider>();
        //// Set the values we want to use
        //player_RB.useGravity = playerRBGravity;
        //player_RB.mass = playerRBMass;
        playerCollision.center = playerBCcentre;
        playerCollision.size = playerBCSize;
        // Important Line <Only use this IF you decide to use AddForce>
        player_RB.constraints = RigidbodyConstraints.FreezeRotation;

        // Adding Components and setting values
        // A local sphere collider
        //SphereCollider SPCol;
        //// We add the sphere collider
        //SPCol = gameObject.AddComponent<SphereCollider>();

        // Now that its added we can set values booleans or whatever below it. Its good to remember this pattern
        // Add First Edit Later

    }

    // Update is called once per frame
    void Update()
    {
        // Functions

        //Vector_Storage_Movement();
        //Transfrom_Translate_Movement();
        //AddForceMovement();
        //VelocityMovement();
    }
    // You'll need to edit these functions for them to work. Pick one or all of them. Fixing one is better than none

    void Transfrom_Translate_Movement()
    {
        #region Use Regions
        // Quick tip use #region and #endregion its a good way to break lines of code apart so you can give it a sensible name
        // Mainly so you know what is where and what code does what
        #endregion

        #region Local Variable Example
        // this is a local variable 
        // Local just means you cannot see this in the inspector its ||not public|| 
        float localVarable = 10;
        #endregion
        float speed = 0; // What could this be for?
        Vector3 directionToMove = new Vector3(/* Something goes here*/0, 0, /*Something goes here*/0);
        transform.Translate(directionToMove * speed * Time.deltaTime  /*something goes here*/ /*Something goes here*/);
    }

    void Vector_Storage_Movement()
    {
        #region Use Regions
        // Quick tip use #region and #endregion its a good way to break lines of code apart so you can give it a sensible name
        // Mainly so you know what is where and what code does what
        #endregion
        Vector3 storage = (/* Think about where we are moving on this axis + */ (transform.forward * 0 /*We need something here just remember how do we move?*/)); //*  We Need A Value Here?;
        // Increase our current position
        transform.position += storage.normalized;
        
    }

    void AddForceMovement()
    {
        #region Use Regions
        // Quick tip use #region and #endregion its a good way to break lines of code apart so you can give it a sensible name
        // Mainly so you know what is where and what code does what
        #endregion
        // What are we storing? Do we really need to say where we are going?
        Vector3 storage = new Vector3(0/*Do We Need to set where we are going with position? or do we need something else*/, 0, /*How are we moving*/0);
        player_RB.AddForce(storage /* * we need a value. But for what?*/);

    }

    void VelocityMovement()
    {
        #region Use Regions
        // Quick tip use #region and #endregion its a good way to break lines of code apart so you can give it a sensible name
        // Mainly so you know what is where and what code does what
        #endregion

        float H = 0; // What does H mean?
        float V = 0; // What could V mean?  // pssst Maybe check the input manager in unity

        float movementSpeed = 5f;
        // Should we be storing something in this vector?
        // Do we need to declare what direction we want to go?
        Vector3 moveDirection = new Vector3(0, 0, 0);
        // This line you get free. normalizing a Vector is making is so its a lower valued number returning as 1
        moveDirection = moveDirection.normalized * movementSpeed;

        player_RB.velocity = moveDirection;
    }
}
