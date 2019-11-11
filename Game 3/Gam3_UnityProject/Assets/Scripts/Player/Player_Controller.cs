using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Player_Controller : MonoBehaviour
{
    // Components
    private Rigidbody playerPhysics;
    private CapsuleCollider playerCollision;
    private Transform playersEyes; // Player Camera

    #region Player Move
    [Header("Player Movement")]
    private float speed;
    public float runSpeed = 30;
    public float currentSpeed = 20;
    float dirX;
    float dirZ;
    Vector3 movementDirection;
    #endregion

    #region mouse Rotation
    [Header("Mouse Rotation")]
    public float cameraRotationRate = 45;   // Rate we rotate at
    [HideInInspector]
    float xRotation = 0f;   // Value that monitors X Rotation keep it at 0 for default
    #endregion

    // Ground check logic
    public LayerMask groundCheckLayers;





    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "PC";
        #region Values SetUp
        speed = currentSpeed;
        cameraRotationRate = 45f;
        xRotation = 0f;
        gameObject.layer = 10;

        #endregion
        #region IDE Set-Up
        // Add Rigidbody to this gameObject
        playerPhysics = gameObject.AddComponent<Rigidbody>();
        // Add Collision to this gameObject
        playerCollision = gameObject.AddComponent<CapsuleCollider>();
        // Set up components
        // Freeze rotation for now so no falling down
        playerPhysics.constraints = RigidbodyConstraints.FreezeRotation;
        playerCollision.height = 2;
        playerCollision.radius = 0.5f;
        #endregion
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        #region Find Components
        playersEyes = gameObject.transform.FindChild("Eyes Main Camera").GetComponent<Transform>();
        #endregion
        #region Debug Components
        if (playerPhysics == null)
            Debug.LogError("Object Name" + ":" + gameObject.transform.name + ":" + "No Rigidbody is applied to the Player Character!! Check the script when it adds the components");

        if(playerCollision == null)
            Debug.LogError("Object Name" + ":" + gameObject.transform.name + ":" + "No Capsule Collider is applied to the player character!! Check the script when it adds the components");
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // Functions
        FPSMove(speed, dirX, dirZ, movementDirection);
    }

    void FPSMove(float speed, float V, float H, Vector3 direction)
    {
        // Input
        V = Input.GetAxis("Vertical");
        H = Input.GetAxis("Horizontal");
        float r = Input.GetAxis("Run");
        // direction we are moving
        direction = (transform.forward * V + (transform.right * H));
        // Make the vector to the equal of 1
        direction = direction.normalized * speed;
        // Running
        if (Input.GetAxis("Run") != 0)
        {
            
            speed = runSpeed;
        }
        else
            speed = currentSpeed;
    
        // move with physics
        playerPhysics.velocity = direction * speed * Time.deltaTime;

        // Camera Rotation Logic
        #region Fixed Camera Rotation
        // Input storage floats
        float mouseRotX;
        float mouseRotY;

        // Camera Rotation
        // Input for rotation
        mouseRotX = Input.GetAxis("Mouse X") * cameraRotationRate * Time.deltaTime;
        mouseRotY = Input.GetAxis("Mouse Y") * cameraRotationRate * Time.deltaTime;

        // For clamping (Rotate between these values)
        float minRotX = -60;
        float maxRotX = 60;

        xRotation -= mouseRotY;

        xRotation = Mathf.Clamp(xRotation, minRotX, maxRotX);

        // Horizontal Rotation up and down
        playersEyes.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Vertical Rotation left and right
        transform.Rotate(0, cameraRotationRate * Time.deltaTime * mouseRotX, 0);
        #endregion
    }

    // Jump Logic
    //void groundCheck()
    //{
    //    // Raycast Logic
    //    RaycastHit hit;
    //    float range = .5f;
    //    if (Physics.Raycast(transform.position / 2, transform.TransformDirection(Vector3.down), out hit, range, groundCheckLayers))
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.green);
    //        if (hit.collider != null)
    //        {
    //            playerPhysics.AddForce(transform.TransformDirection(Vector3.up * 1600));
    //        }
    //        else
    //            return;
    //    }
    //} 


        public class GunMechanic : MonoBehaviour
        {
        [Header("Shooting")]
        public LayerMask whatWeCanShoot;
        public AudioSource gunShootSound;
        public Transform gun_firePoint;
        public int gunRange;
        public float fireRte = 3f;
        float nextTimeToFire;

        [Header("Reloadiing")]
        public bool isReloading = false;
        float reloadTime = 1f;
        public Animator gunAnimator;
        [Header("Ammo")]  
        // Max ammo in 1 magazine
        public int maxAmmo = 12;
        // current ammo that player has in the magazine
        public int currentAmmo;
        // the ammo the player has spare
        public int backUpAmmo;
        // Object that spawns for AI
        public GameObject BulletPosition;
        bool isShooting = true;

        public virtual void Awake()
        {
            #region Value Set-Up
            gunRange = 60;
            currentAmmo = maxAmmo;
            backUpAmmo = 90;
            #endregion
        }

        public virtual void FixedUpdate()
        {
            // Functions
            GunShoot();
            #region Reloading
            if (currentAmmo <= 0 && backUpAmmo > 0 && !isReloading)
                StartCoroutine(Reload());
            if (isReloading)
                return;
            #endregion
            if (currentAmmo <= 0)
                isShooting = false;
            else
                isShooting = true;
        }

        // Casual Shooting
        void GunShoot()
        {
            // Simple Shoot
            if(isShooting)
            {
                // Physics Driven
                RaycastHit Hit;
                if (Physics.Raycast(gun_firePoint.transform.position, gun_firePoint.transform.TransformDirection(Vector3.forward), out Hit, gunRange, whatWeCanShoot))
                {
                    if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
                    {
                        gunShootSound.Play();
                        nextTimeToFire = Time.time + 1f / fireRte;
                        GameObject BulletShot = Instantiate(BulletPosition, gun_firePoint.position, Quaternion.identity) as GameObject;
                        BulletShot.name = "Bullet_Sound_Position";
                        // Decrease Ammo
                        currentAmmo--;
                        Debug.Log("Hit" + Hit.transform.name);
                        Debug.DrawRay(gun_firePoint.transform.position, gun_firePoint.TransformDirection(Vector3.forward) * Hit.distance, Color.red);
                    }
                }
            }
        }

        // Normal Reloading
        IEnumerator Reload()
        {
            isShooting = false;
            isReloading = true;
            Debug.Log("Reloading. . . .");
            gunAnimator.SetBool("isReloading", true);

            yield return new WaitForSeconds(reloadTime - .25f);
            gunAnimator.SetBool("isReloading", false);
            yield return new WaitForSeconds(.25f);
            isReloading = false;

            var shot = maxAmmo - currentAmmo;
            if (backUpAmmo < shot)
            {
                currentAmmo = backUpAmmo;
                backUpAmmo = 0;
            }
            else
            {
                currentAmmo += shot;
                backUpAmmo -= shot;
            }
        }
    }


}
