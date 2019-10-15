using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_2D_Movement : MonoBehaviour
{
    //IDE Components
    Rigidbody2D PCRB;
    BoxCollider2D PCBC;
    public Vector2 boxColliderSize;
    [SerializeField]
    bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody SetUp
        PCRB = GetComponent<Rigidbody2D>();

        // Box Collider Set Up
        PCBC = GetComponent<BoxCollider2D>();
        boxColliderSize = new Vector2(1.28f, 3.02f);
        PCBC.size = boxColliderSize;

        transform.gameObject.tag = "Player";

    }

    // Update is called once per frame
    void Update()
    {
        // Functions For Movement
        //Transform_Translate_Movement();
        //Vector_Storage_Movement();
        //Rigidbody2D_AddForce();
        //Rigidbody2D_Velocity();
    }

    void Transform_Translate_Movement()
    {
        // This allows us to move only but what if we want to see where we are going
        #region Movement Logic
        float H = 0; // What do players need to do to move?
        float speed = 2;
        Vector2 moveDirection = new Vector2(0/* What axis are we moving on in a 2D world*/, 0);
        transform.Translate(moveDirection * speed * Time.deltaTime);
        #endregion

        #region Flipping Sprites
        if (moveDirection.x > 0f && !facingRight)
        {
            Sprite_Flipping();
        }
        else if (moveDirection.x < 0f && facingRight)
            Sprite_Flipping();
        #endregion
    }

    void Vector_Storage_Movement()
    {
        float H = 0;    // What are we getting here to give the vector data
        Vector3 storage = (transform.right * H);
        transform.position += storage;

        #region Sprite Flipping
        if (storage.x > 0f && !facingRight)
        {
            Sprite_Flipping();
        }
        else if (storage.x < 0f && facingRight)
            Sprite_Flipping();
        #endregion
    }

    void Rigidbody2D_AddForce()
    {
        float force = 0; // Is this really a value? or is this a direction
        PCRB.AddForce(Vector2.right * force * 10);

        #region Sprite Flipping
        if (force > 0f && !facingRight)
        {
            Sprite_Flipping();
        }
        else if (force < 0f && facingRight)
            Sprite_Flipping();
        #endregion
    }

    void Rigidbody2D_Velocity()
    {
        float H = 0; // Values get passed into Vectors as data: So what could this be for?

        float movementSpeed = 6f;

        Vector3 moveDirection = new Vector3(0, 0);

        moveDirection = moveDirection.normalized * movementSpeed;
        
        PCRB.velocity = moveDirection;

        #region Sprite Flipping
        if (moveDirection.x > 0f && !facingRight)
        {
            Sprite_Flipping();
        }
        else if (moveDirection.x < 0f && facingRight)
            Sprite_Flipping();
        #endregion
    }

    #region Flipping Function 
    void Sprite_Flipping()
    {
        facingRight = !facingRight;
        Vector2 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }
    #endregion
}
