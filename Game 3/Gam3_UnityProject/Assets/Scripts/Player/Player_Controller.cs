using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Player_Controller : MonoBehaviour
{
    #region Components
    private Rigidbody playerPhysics;
    private CapsuleCollider playerCollision;
    private Transform playersEyes; // Player Camera
    #endregion

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



    // Start is called before the first frame update
    void Start()
    {
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
        playersEyes = gameObject.transform.Find("FPS_Cam").GetComponent<Transform>();
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

        public class GunMechanic : MonoBehaviour
        {
        [Header("Shooting")]
        public LayerMask whatWeCanShoot;
        public AudioSource gunShootSound;
        public Transform gun_firePoint;
        public int gunRange = 30;
        public float fireRate = 0.25f;
        float fireTimer;

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

        [Header("Aim Down Sites")]
        // The orginal position of the gun at the hip
        private Vector3 originalPosition;
        // Where the object will be when aiming
        public Vector3 aimPosition;
        public Transform weaponHolder;
        // "Aim Down Sight" Speed
        public float adsSpeed = 8f;

        [Header("Firing Type")]
        public ShootMode shootingMode;
        public bool shootInput;
        public enum ShootMode {Auto, Semi, Burst}
        // Controls the different shooting states 
        int shootModeController = 0;
        public int numOfBullets = 3;


        Camera playerEyes;  // The players eyes 
        float cameraFieldOfView;




        public virtual void Awake()
        {
            #region Value Set-Up
            gunRange = 60;
            currentAmmo = maxAmmo;
            backUpAmmo = 90;
            #endregion

            playerEyes = Camera.main;
            cameraFieldOfView = Camera.main.fieldOfView;

            // The hip location of the gun
            originalPosition = weaponHolder.localPosition;
        }

        public virtual void FixedUpdate()
        {
            // Functions
            AimDownSights();

            #region Fire Modes

            #region Shoot Mode Controller Checks
            if (shootModeController == 0)
                shootingMode = ShootMode.Auto;
            else if (shootModeController == 1)
                shootingMode = ShootMode.Semi;
            else if (shootModeController == 2)
                shootingMode = ShootMode.Burst;

            if(Input.GetKeyDown(KeyCode.Q))
            {
                shootModeController++;
                if (shootModeController > 2)
                    shootModeController = 0;
            }
            #endregion


            switch (shootingMode)
            {
                case ShootMode.Auto:
                    shootInput = Input.GetButton("Fire1");
                    fireRate = 0.25f;
                break;

                case ShootMode.Semi:
                    shootInput = Input.GetButtonDown("Fire1");
                    fireRate = 0.8f;
                break;

                case ShootMode.Burst:
                    shootInput = Input.GetButtonDown("Fire1");
                break;
            }
            #endregion

            // Depending on the inptut and fire mode we can shoot
            if (shootInput)
            {
                if(currentAmmo > 0)
                {
                    Fire();
                }
            }

            // Fire Rate
            if (fireTimer < fireRate)
                fireTimer += Time.deltaTime;


            #region Reloading
            if (currentAmmo <= 0 && backUpAmmo > 0 && !isReloading)
                StartCoroutine(Reload());
            if (isReloading)
                return;

            if(Input.GetKeyDown(KeyCode.R) && !isReloading && backUpAmmo > 0)
            {
                StartCoroutine(Reload());
            }
            #endregion
            // Are We Allowed to shoot?
            if (currentAmmo <= 0)   // No we have no ammo
                isShooting = false;
            else
                isShooting = true;  // Yes we still have bullets 
        }

        public void AimDownSights()
        {
            if (Input.GetButton("Fire2") && !isReloading)
            {
                weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, aimPosition, Time.deltaTime * adsSpeed);
            }
            else
            {
                weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * adsSpeed);
            }
            
        }

        // Casual Shooting
        public void  Fire()
        {
            // Simple Shoot
            if(isShooting)
            {
                // If the timer isnt less than the rate of fire then we dont run the code below
                if (fireTimer < fireRate) return;
                fireTimer = 0.0f;   // Reset the timer
                // Physics Driven
                RaycastHit Hit;
                if (Physics.Raycast(gun_firePoint.transform.position, gun_firePoint.transform.TransformDirection(Vector3.forward), out Hit, gunRange, whatWeCanShoot))
                {
                    if (currentAmmo > 0)
                    {
                        gunShootSound.Play();
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
