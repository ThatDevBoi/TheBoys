using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    private Collider hit;
    private float startTime;
    public float wait;
    private bool exit=false;
    // Start is called before the first frame update
    void Start()
    {
        hit = GetComponent<Collider>();
        exit = false;
    }

    // Update is called once per frame
    void Update()
    {
       
        if(exit)
            if (Time.time - startTime > wait)
                Destroy(gameObject);
        
    }

    private void OnTriggerExit(Collider other)
    {
        startTime = Time.time;
        exit = true;
    }
}
