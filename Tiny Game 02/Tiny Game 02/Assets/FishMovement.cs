using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 0;
    public Vector3 movedirection;
    public Quaternion rotateWhenMove;
    public float minRotation = 10;
    public float maxRotation = 50;

    public float rotation_cooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name == "Shark")
            speed = 5;
        if (gameObject.name == "Whale")
            speed = 2;
        if (gameObject.name == "SeaTurtle")
            speed = 3;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += new Vector3(transform.position.x, transform.position.y, transform.forward.x * speed);
        Movement();

        rotation_cooldown += Time.deltaTime;

        if(rotation_cooldown > 6)
        {
            ObjectRotation();
            rotation_cooldown = 0;
        }
    }

    void Movement()
    {
        movedirection = Vector3.right;
        // move the object 
        transform.Translate(movedirection * speed);

    }

    void ObjectRotation()
    {
        // Set up positive and negative rotation
        float plusRot = Random.Range(minRotation, maxRotation);

        rotateWhenMove = new Quaternion(plusRot, gameObject.transform.rotation.y, transform.rotation.z, 0);
        transform.rotation = rotateWhenMove;
    }
}
