using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Cube : MonoBehaviour
{
    // Speed of rising gameObject
    public float ySpeed = .1f;
    // Rotation rate
    public float rotationRate = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate Object
        gameObject.transform.Rotate(transform.rotation.x, rotationRate, transform.rotation.x);
        // Move object up
        transform.position += transform.up.normalized * ySpeed;
    }
    // Destroy when off screen
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
