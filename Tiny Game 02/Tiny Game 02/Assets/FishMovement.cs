using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 0;
    Vector3 movedirection;

    private float minRotation = 10;
    private float maxRotation = 50;

    float minNextTimeToRot = 5;
    float maxNextTimeToRot = 20;
    public float rotation_cooldown = 0;
    // Value that changes rotation when another is greater than the value
    float cooldownMeter;
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        //if (gameObject.name == "Shark")
        //    speed = 5;
        //if (gameObject.name == "Whale")
        //    speed = 2;
        //if (gameObject.name == "SeaTurtle")
        //    speed = 3;
        //if (gameObject.name == "Sea Horse")
        //    speed = 6;
        // Random cooldown meter
        float nextTimeTimer = Random.Range(minNextTimeToRot, maxNextTimeToRot);
        cooldownMeter = nextTimeTimer;
=======
        //if (gameObject.name == "Shark")
        //    speed = 5;
        //if (gameObject.name == "Whale")
        //    speed = 2;
        //if (gameObject.name == "SeaTurtle")
        //    speed = 3;
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += new Vector3(transform.position.x, transform.position.y, transform.forward.x * speed);
        Movement();
        // Increase Value
        rotation_cooldown += Time.deltaTime;

        if(rotation_cooldown > cooldownMeter)
        {
            ObjectRotation();
            rotation_cooldown = 0;
        }
    }

    void Movement()
    {
<<<<<<< Updated upstream
        // Movement direction
        movedirection = Vector3.right;
=======
       // movedirection = Vector3.forward;
>>>>>>> Stashed changes
        // move the object 
        transform.Translate(movedirection * speed);

    }

    void ObjectRotation()
    {
        // Set up positive and negative rotation
        float plusRot = Random.Range(minRotation, maxRotation);
        float nextTimeTimer = Random.Range(minNextTimeToRot, maxNextTimeToRot);
        cooldownMeter = nextTimeTimer;
        transform.Rotate(transform.rotation.x, transform.rotation.y + plusRot, transform.rotation.z);
    }
}
