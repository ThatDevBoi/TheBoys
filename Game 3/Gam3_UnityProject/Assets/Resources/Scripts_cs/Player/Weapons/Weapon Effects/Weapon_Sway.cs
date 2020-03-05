using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used for weapon sway so the weapon has a delayed follow time so it sways 
/// This needed to be seperate from the core gun logic as it interferes with the Aim Function
/// So a parent needs to have this script so the child sways (Child should be gun)
/// </summary>
public class Weapon_Sway : MonoBehaviour
{
    [Header("Weapon Sway")]
    // PUBLIC
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothTime = 6f;
    // PRIVATE
    Vector3 def;
    Vector3 euler;

    [Header("Running Sway")]
    public Player_Controller playerScript;
    private float runSway = 20f;
    public float runSwaySpeed = 2;
    public float amountSway = 20f;
    [Space(10)]
    public float runSway_downRot = 10f;

    [Header("Loop Run Vertical")]
    public float speed = 5;
    public float height = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        // find the script
        playerScript = GameObject.Find("PC").GetComponent<Player_Controller>();

        // Weapon Sway
        def = transform.localPosition;
        euler = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        // Functions
        WeaponSway();
        RunningSway();
    }

    float _smooth;
    public void WeaponSway()
    {
        // if the player is not near a wall
        if(GameManager.gunOverride == false)
        {
            // and the player is not running
            if (playerScript.running == false)
            {
                // Time we use to Sway the weapon within a Lerp
                _smooth = smoothTime;
                // Mouse Inputs for X and Y positions
                float Mouse_MovementX = -Input.GetAxis("Mouse X") * amount;
                float Mouse_MovementY = -Input.GetAxis("Mouse Y") * amount;
                // dont let mouse movement let the gun go over its max intended sway
                Mouse_MovementX = Mathf.Clamp(Mouse_MovementX, -maxAmount, maxAmount);
                Mouse_MovementY = Mathf.Clamp(Mouse_MovementY, -maxAmount, maxAmount);
                // The final position we will lerp towards is the position of the gun with the variables controlling the clamp
                Vector3 final = new Vector3(def.x + Mouse_MovementX, def.y + Mouse_MovementY, def.z);
                // the new position of the gun when rotating the mouse up, down left or right
                this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, final, Time.deltaTime * _smooth);
            }
        }
    }

    void verticalRun()
    {
        Vector3 position = transform.localPosition;
        float newY = Mathf.Sin(Time.time * speed);
        transform.localPosition = new Vector3(position.x, newY, position.z) * height;
        
    }

    public void RunningSway()
    {
        // When the gun is not being controlled
        if(GameManager.gunOverride == false)
        {
            // when the player runs 
            if (playerScript.running == true)
            {
                // generate the number in which the gun rotates across to
                // change the values to decide how far left the gun sways when running
                runSway = Random.Range(-50 + amountSway, -50 + amountSway);
                // make rotation
                Quaternion runSwaying = Quaternion.Euler(runSway_downRot, runSway, 0);
                // rotate
                transform.localRotation = Quaternion.Slerp(transform.localRotation, runSwaying, Time.deltaTime * runSwaySpeed);
                // call function to make gun move up and down in a loop
                verticalRun();
            }
            else
            {
                // reset localRotation of the gun so it sits back when not running 
                runSway = 0;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * runSwaySpeed);
            }
        }
    }
}
