using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeAnimSpeed : MonoBehaviour
{
    float speedDifference = 0;
    float minSpeed = .1f;
    float maxSpeed = 1;
    public Animator fishAnim;
    // Start is called before the first frame update
    void Start()
    {
        fishAnim = GetComponent<Animator>();

        speedDifference = Random.Range(minSpeed, maxSpeed);
        fishAnim.speed = speedDifference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
