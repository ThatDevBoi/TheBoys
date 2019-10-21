using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSchool_Movement : MonoBehaviour
{
    public float speed = 2;
    float timeCounter = 0;
    float width;
    float height;
    
    // Start is called before the first frame update
    void Start()
    {
        width = 4;
        height = 2;
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime * speed;
        float x = Mathf.Cos(timeCounter) * width;
        float y = Mathf.Sin(timeCounter) * height;
        float z = 2;
        transform.position += new Vector3(x, y, transform.forward.z * z);
    }
}
