using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    int speed = 10;
    public float F=1;
    float turnspeed = 5;
    Rigidbody rigidbody;
    private void Start()
    {

        rigidbody = GetComponent<Rigidbody>();
        Physics.gravity *= 0.1f;
    }
    private void Update()
    {


        float v = Input.GetAxis("Vertical");

       float h = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(Vector3.up * 10);
            Debug.Log("y");
        }

       if(h!=0)
            rigidbody.AddRelativeTorque(0,h* F*0.01f,0);
            Debug.Log("x");
        
        if (v != 0)
        {
            rigidbody.AddRelativeForce(0,0, v * F);
            Debug.Log("x");
        }
    }

}
