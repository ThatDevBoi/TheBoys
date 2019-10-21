using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public Camera cam;
    public float speed=1;
    public float turnspeedY = 5;
    public float turnspeedX = 5;
    private Rigidbody RB_PC;
    private float rotX;
    private float rotY;
    private float startFOV=60f;
    private float speedFOV = 75f;
    public GameObject can;
    CursorMode mycursor;
    private void Start()
    {
        RB_PC = GetComponent<Rigidbody>();
        Physics.gravity *= 0.1f;//reduced gravity for emulating underwater ambient
        
    }
    private void Update()
    {
        Move();     
    }

    private void Move()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        float v = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.Space))
        {
            RB_PC.AddRelativeForce(Vector3.up * 10);
           // Debug.Log("y");
        }
        
        if (v != 0)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, speedFOV, 0.3f);//increase fov when moving
            RB_PC.AddRelativeForce(0, 0, v * speed);
            //Debug.Log("x");
        }
        else if(can.activeInHierarchy==false)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, startFOV, 0.1f);//reduce fov when not moving
        }
        //mouse input for rotation
        rotX += Input.GetAxis("Mouse X")*turnspeedX;
        rotY += Input.GetAxis("Mouse Y")*turnspeedY;
        rotY = Mathf.Clamp(rotY, -10f, 30f);
        transform.localRotation = Quaternion.Euler(-rotY, rotX,0);
    }   
}       
