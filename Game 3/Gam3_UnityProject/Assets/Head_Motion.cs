using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Motion : MonoBehaviour
{
    //  Variables
    private float timer = 0;
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float midpoint = 0.3f;

    //  Update is called once per frame
    void Update()
    {
        // float per bob
        float waveslice = 0.0f;
        // Get the inputs that make us bob
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // localPosition of object this script is applied to
        Vector3 cSharpConversion = transform.localPosition;
        // the absolute value of both inputs getting - & +
        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + bobbingSpeed;   // timer helps define when the head bobs next
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            // Clamp the bob amount
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            // the change amount for bobbing
            translateChange = totalAxes * translateChange;
            // We bob on the y axis which takes into consideration the start and change of head position
            cSharpConversion.y = midpoint + translateChange;
        }
        else
        {
            cSharpConversion.y = midpoint;  // the midpoint is the Y position start point
        }
        // position is midpoint
        transform.localPosition = cSharpConversion;
    }

}
