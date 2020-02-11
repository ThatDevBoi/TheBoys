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
                _smooth = smoothTime;

                float MovementX = -Input.GetAxis("Mouse X") * amount;
                float MovementY = -Input.GetAxis("Mouse Y") * amount;

                MovementX = Mathf.Clamp(MovementX, -maxAmount, maxAmount);
                MovementY = Mathf.Clamp(MovementY, -maxAmount, maxAmount);

                Vector3 final = new Vector3(def.x + MovementX, def.y + MovementY, def.z);
                this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, final, Time.deltaTime * _smooth);
            }
        }
    }

    public void RunningSway()
    {
        if(GameManager.gunOverride == false)
        {
            if (playerScript.running == true)
            {
                // generate the number in which the gun rotates across to
                // change the values to decide how far left the gun sways when running
                runSway = Random.Range(-50 + amountSway, -50 + amountSway);
                // make rotation
                Quaternion runSwaying = Quaternion.Euler(runSway_downRot, runSway, 0);
                // rotate
                transform.localRotation = Quaternion.Slerp(transform.localRotation, runSwaying, Time.deltaTime * runSwaySpeed);
            }
            else
            {
                runSway = 0;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * runSwaySpeed);
            }
        }
    }
}
