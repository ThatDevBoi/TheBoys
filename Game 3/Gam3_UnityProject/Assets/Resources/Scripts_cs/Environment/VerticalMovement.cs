using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    private float startTime=0;
    public Vector3 endPosition;
    [Range(0.0f, 10.0f)]
    public float speed;
    private bool peak=false;
    private Vector3 startPosition;

    public bool xAxis;
    public bool yAxis;
    public bool zAxis;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        endPosition += startPosition;
       // Debug.Log("moving cube" + startPosition.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (peak)
        {
        transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.deltaTime* speed);
            //Debug.Log(startPosition);
            
        }

        else 
            transform.position = Vector3.MoveTowards(transform.position, endPosition, Time.deltaTime*speed);

        if (xAxis&&!yAxis&&!zAxis)
            moveHorizontal();
        else if (yAxis)
            moveVertical();
        else if (zAxis)
            moveDepth();
    }

    void moveVertical()
    {
        if (transform.position.y == endPosition.y)
        {
            peak = true;

        }
        else if (transform.position.y == startPosition.y)
        {
            peak = false;

        }
    }
    void moveHorizontal()
    {
        if (transform.position.x == endPosition.x)
        {
            peak = true;

        }
        else if (transform.position.x == startPosition.x)
        {
            peak = false;

        }
    }
    void moveDepth()
    {
        if (transform.position.z == endPosition.z)
        {
            peak = true;

        }
        else if (transform.position.z == startPosition.z)
        {
            peak = false;

        }
    }
}
