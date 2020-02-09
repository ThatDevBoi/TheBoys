using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotaterRateX = 0;
    public float rotaterRateY = 0;
    public float rotaterRateZ = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotaterRateX, rotaterRateY, rotaterRateZ);
    }
}
