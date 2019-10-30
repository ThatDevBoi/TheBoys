using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    // Components
    private Rigidbody playerPhysics;
    private CapsuleCollider playerCollision;
    private Transform playersEyes; // Player Camera
    #region Player Move
    public float speed = 10;
    float dirX;
    float dirZ;
    Vector3 movementDirection;
    #endregion

    #region mouse Rotation
    public float cameraRotationRate = 45;   // Rate we rotate at
    [HideInInspector]
    float xRotation = 0f;   // Value that monitors X Rotation keep it at 0 for default
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        #region Values SetUp
        speed = 10;
        cameraRotationRate = 45f;
        xRotation = 0f;
        #endregion
        #region IDE Set-Up
        // Add Rigidbody to this gameObject
        playerPhysics = gameObject.AddComponent<Rigidbody>();
        // Add Collision to this gameObject
        playerCollision = gameObject.AddComponent<CapsuleCollider>();
        // Set up components
        // Freeze rotation for now so no falling down
        playerPhysics.constraints = RigidbodyConstraints.FreezeRotation;
        playerCollision.height = 2;
        playerCollision.radius = 0.5f;
        #endregion
        Cursor.lockState = CursorLockMode.Locked;
        #region Find Components
        playersEyes = gameObject.transform.FindChild("Eyes/Main Camera").GetComponent<Transform>();
        #endregion
        #region Debug Components
        if (playerPhysics == null)
            Debug.LogError("Object Name" + ":" + gameObject.transform.name + ":" + "No Rigidbody is applied to the Player Character!! Check the script when it adds the components");

        if(playerCollision == null)
            Debug.LogError("Object Name" + ":" + gameObject.transform.name + ":" + "No Capsule Collider is applied to the player character!! Check the script when it adds the components");
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // Functions
        FPSMove(speed, dirX, dirZ, movementDirection);

        #region Fixed Camera Rotation
        // Input storage floats
        float mouseRotX;
        float mouseRotY;

        // Camera Rotation
        // Input for rotation
        mouseRotX = Input.GetAxis("Mouse X") * cameraRotationRate * Time.deltaTime;
        mouseRotY = Input.GetAxis("Mouse Y") * cameraRotationRate * Time.deltaTime;

        // For clamping (Rotate between these values)
        float minRotX = -60;
        float maxRotX = 60;

        xRotation -= mouseRotY;

        xRotation = Mathf.Clamp(xRotation, minRotX, maxRotX);

        // Horizontal Rotation up and down
        playersEyes.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Vertical Rotation left and right
        transform.Rotate(0, cameraRotationRate * Time.deltaTime * mouseRotX, 0);
        #endregion
    }

    void FPSMove(float speed, float V, float H, Vector3 direction)
    {
        // Input
        V = Input.GetAxis("Vertical");
        H = Input.GetAxis("Horizontal");
        // direction we are moving
        direction = (transform.forward * V + (transform.right * H));
        // Make the vector to the equal of 1
        direction = direction.normalized * speed;
        // move with physics
        playerPhysics.velocity = direction * speed * Time.deltaTime;
    }
}
