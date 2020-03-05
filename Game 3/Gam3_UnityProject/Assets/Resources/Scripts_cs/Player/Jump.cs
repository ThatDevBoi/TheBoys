using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public bool Jumpon = false;
    public float jumpHeightCheck;
    [Range(1f, 50f)]
    public float Height;
    public float heightMultiplier;
    private Rigidbody rb;
    public float gravityMultiplier;
   

    // Start is called before the first frame update
    void Start()
    {
        
    
        if (rb == null)
            gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Player_Controller>().playerPhysics;
    }
    // Update is called once per frame
    void Update()
    { 
        if (Input.GetButtonDown("Jump"))
        {

            if (Jumpon)
            {
                rb.AddForce(Vector3.up * Height * heightMultiplier * Physics.gravity.y * (-1f));
              
            }
           
        }
       
        if (!Jumpon)
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * gravityMultiplier);

        }
        CheckJump();
    }

    void CheckJump()
    {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, jumpHeightCheck))
        {
            if (hit.collider.gameObject.layer == 8)
                Jumpon = true;

        }
        else
            Jumpon = false;
    }
}
