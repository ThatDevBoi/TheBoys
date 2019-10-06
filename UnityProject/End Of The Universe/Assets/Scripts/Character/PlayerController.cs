using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Movement Variables
    //2D rigidbody Component
    private Rigidbody2D PCRB;

    // Bool that helps game know if the player is @ the top or bottom
    private bool top;
    // Allows player to go from top to bottom
    private bool canSwitch = true;
    // Makes it so you can change gravity mid air
    private int buttonCount = 0;
    // Timer that changes when we can change gravity again
    public float SwitchTimer = 2;
    //Slider variable for the cooldown display
    public Slider cooldownSlider;

    // GunPlay Variables
    // Parts of Guns
    public Transform firePoint;
    public GameObject[] Gun;
    // Int that changes weapon stats
    private int currentGun;
    // Muzzle Flash Effect
    public SpriteRenderer muzzleFlash;
    // Timer to turn off muzzle Flash
    public float turnFlashOff = .5f;
    // The layer mask that helps a raycast understand what its detecting to hit
    public LayerMask whatToShoot;

    // Gun Stats
    // Damage it does to a obsticle
    public int gunDamage;
    // Force that pushes Rigidbody Objects back
    public float impactForce;
    // How far can players shoot
    public int range;

    // Start is called before the first frame update
    void Start()
    {
        // We dont need any of the gun GameObjects on if we are only starting the game
        Gun = GameObject.FindGameObjectsWithTag("Gun");
        foreach (GameObject guns in Gun)
            guns.SetActive(false);

        // Find Physcis
        PCRB = GetComponent<Rigidbody2D>();

        //cooldownSlider.value = SwitchTimer;

        muzzleFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Functions
        Movement();
        FireGun();
        DifferentGuns();

        if (gameObject.transform.position.y > 3)
            top = true;
        else if (gameObject.transform.position.y < 3)
            top = false;


        if (buttonCount > 0)
        {
            canSwitch = false;
            SwitchTimer -= Time.deltaTime;
            cooldownSlider.value += SwitchTimer * Time.deltaTime;
            if (SwitchTimer <= 0)
            {
                canSwitch = true;
                buttonCount = 0;
                SwitchTimer = 2;
            }
        }

        if (muzzleFlash.enabled == true)
            turnFlashOff -= Time.deltaTime;

        if(turnFlashOff <= 0)
        {
            muzzleFlash.enabled = false;
            turnFlashOff = .5f;
        }
    }

    public void Movement()
    {
        if (canSwitch)
        {
            if (Input.GetButtonDown("Jump"))
            {
                SpriteFlipping();
                buttonCount += 1;
                PCRB.gravityScale *= -1;
                cooldownSlider.value = 0;
            }

            if (!top && Input.GetButtonDown("Jump"))
            {
                SpriteFlipping();
                PCRB.gravityScale *= 1;
                cooldownSlider.value = 0;
            }
        }
        else
            return;

    }

    public void SpriteFlipping()
    {
        if(!top)
        {
            top = !top;
            Vector2 temp = transform.localScale;
            temp.y *= -1;
            transform.localScale = temp;
        }
        else
        {
            top = !top;
            Vector2 temp = transform.localScale;
            temp.y *= 1;
            transform.localScale = temp;
        }
    }

    void FireGun()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // Turn off muzzle flash as a player feedback indicator
            muzzleFlash.enabled = true;

            //Vector2 firepos = new Vector2(firePoint.position.x, firePoint.position.y);  // Vector2 that holds the fire position positions we can use to shoot from
            //Vector2 direction = (top) ? Vector2.right : Vector2.left; // The direction we can shoot the raycast depending on where we are

            Vector2 direction = new Vector2(transform.position.x, 0);
            // Setup hitinfo logic
            RaycastHit2D hitinfo = Physics2D.Raycast(firePoint.position, firePoint.TransformDirection(Vector2.right), range, whatToShoot);
            // Rot allows the Game to know if the player faces right the bullet wont need rotation 
            //however needs to be flipped 180 degrees if its the opposite
            //Quaternion rot = (top) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            Debug.DrawRay(firePoint.position, firePoint.TransformDirection(Vector2.right) * hitinfo.distance, Color.yellow);
            if(hitinfo)
            {
                Debug.Log("Hit");
                HazzardHealth hazzard = hitinfo.transform.GetComponent<HazzardHealth>();

                if (hazzard != null)
                    hazzard.TakeDamage(gunDamage);
                // What have we hit. It will show in the console
                Debug.Log(hitinfo.transform.name);
            }
            else
            {
                Debug.DrawRay(firePoint.position, firePoint.TransformDirection(Vector3.right) * 1000, Color.white);
                Debug.Log("Did Not Hit");
            }

            if (hitinfo.collider != null)
                hitinfo.rigidbody.AddForce(-hitinfo.normal * impactForce);
        }
    }

    void DifferentGuns()
    {
        switch(currentGun)
        {
            case 0:
                gunDamage = 5;
                impactForce = 50;
                break;

            case 1:
                gunDamage = 20;
                impactForce = 200;
                break;

            case 2:
                gunDamage = 10;
                impactForce = 75;
                break;

            case 3:
                gunDamage = 20;
                impactForce = 150;
                break;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Death")
            Level_Manager.gameOver = true;

        if(other.gameObject.name == "Pistol Pickup")
        {
            Destroy(other.gameObject);
            currentGun = 0;
            Gun[0].SetActive(true);
            Gun[1].SetActive(false);
            Gun[2].SetActive(false);
            Gun[3].SetActive(false);
        }
        if (other.gameObject.name == "ShotGun Pickup")
        {
            Destroy(other.gameObject);
            currentGun = 1;
            Gun[0].SetActive(false);
            Gun[1].SetActive(true);
            Gun[2].SetActive(false);
            Gun[3].SetActive(false);

        }
        if (other.gameObject.name == "SMG Pickup")
        {
            Destroy(other.gameObject);
            currentGun = 2;
            Gun[0].SetActive(false);
            Gun[1].SetActive(false);
            Gun[2].SetActive(true);
            Gun[3].SetActive(false);
        }
        if (other.gameObject.name == "Assualt Rifle Pickup")
        {
            Destroy(other.gameObject);
            currentGun = 3;
            Gun[0].SetActive(false);
            Gun[1].SetActive(false);
            Gun[2].SetActive(false);
            Gun[3].SetActive(true);
        }
    }
}
