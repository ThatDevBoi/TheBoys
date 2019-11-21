﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomPropertyDrawer(typeof(HideAttributes))]
public class Player_Controller : MonoBehaviour
{

    #region Variablles
    [Header("Movement")]
    // PUBLIC
    public float runSpeed = 30;
    public float currentSpeed = 20;
    public float cameraRotationRate = 45;   // Rate we rotate at
    public AudioSource walkingSound;
    public LayerMask slopCheck;
    // PRIVATE

    [Header("Health n Damage")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Health Bar")]
    public Slider healthBar;


    #region Debugging
    [HideInInspector]
    public bool Debugging;
    [Header("$Debugging$ The Rigidbody Physics Component")]
    [Header("For QA Tester")]
    [HideAttributes("Debugging", true)]
    public Rigidbody playerPhysics;
    [Header("$Debugging$ Players Collision detection")]
    [HideAttributes("Debugging", true)]
    public CapsuleCollider playerCollision;
    [Header("$Debugging$ The Main Camera Component")]
    [HideAttributes("Debugging", true)]
    public Transform playersEyes; // Player Camera
    [Header("$Debugging$ The Players Movement Direction")]
    [HideAttributes("Debugging", true)]
    public Vector3 movementDirection;
    [Header("$Debugging$ Value which monitors X Rotation")]
    [HideAttributes("Debugging", true)]
    public float xRotation = 0f;   // Value that monitors X Rotation keep it at 0 for default
    [Header("$Debugging$ The X Direction of movement")]
    [HideAttributes("Debugging", true)]
    public float dirX;
    [Header("$Debugging$ The Z Direction of movemet")]
    [HideAttributes("Debugging", true)]
    public float dirZ;
    [Header("$Debugging$ The Value for movement speed")]
    [HideAttributes("Debugging", true)]
    public float speed;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
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
        walkingSound.volume = 0.3f;
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
        HealthBar();
        // Never go over max health
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        #region Debuggng Key Press Logic
        if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Y))
        {
            Debugging = true;
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.M))
        {
            Debugging = false;
        }
        #endregion
    }

    void FPSMove(float speed, float V, float H, Vector3 direction)
    {
        // Input
        V = Input.GetAxis("Vertical");
        H = Input.GetAxis("Horizontal");
        // Run Input
        float r = Input.GetAxis("Run");
        // direction we are moving
        direction = (transform.forward * V + (transform.right * H));
        // Make the vector to the equal of 1
        direction = direction.normalized * speed;

        #region Slop Check
        // Slop Check
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, .6f, slopCheck))
        {
            direction.y = playerPhysics.velocity.y; // Cancel Gravity
            Debug.DrawRay(transform.position, Vector3.down * 50, Color.green);
        }
        else
        {
            // Player gravity
            direction.y = direction.y + Physics.gravity.y;  // Turn gravity back on
        }
        #endregion

        // Running
        if (Input.GetAxis("Run") != 0)
        {
            walkingSound.volume = 0.6f;
            speed = runSpeed;
        }
        else
        {
            walkingSound.volume = 0.3f;
            speed = currentSpeed;
        }
           
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
    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void HealthBar()
    {
        healthBar.value = currentHealth;
        #region Change Bar Color
        if (currentHealth > maxHealth | currentHealth > 60)
        {
            Image healthImage = GameObject.Find("PlayerHP_Fill").GetComponent<Image>();
            healthImage.color = Color.green;
        }
        // Make fill bar Yellow
        if(currentHealth  <= 60)
        {
            Image healthImage = GameObject.Find("PlayerHP_Fill").GetComponent<Image>();
            healthImage.color = Color.yellow;
        }

        // make fill bar Red
        if (currentHealth <= 30)
        {
            Image healthImage = GameObject.Find("PlayerHP_Fill").GetComponent<Image>();
            healthImage.color = Color.red;
        }
        #endregion

    }

    #region Charging Port (OnTriggerStay)
    // Base for charging port (For ammo and health)
    private void OnTriggerStay(Collider other)
    {
        // Find the derived class
        Shooting_Mechanic gunScript = transform.FindChild("FPS_Cam/Weapon_Holder/Pistol").GetComponent<Shooting_Mechanic>();
        // Increase Health
        if (currentHealth < maxHealth)  // check if the current health is less than the max 
        {
            if (other.gameObject.name == "Charging Port")   // is this a charging port?
            {
                // Do a thing
                currentHealth += 10;

            }
        }
        // Increase ammo
        if(gunScript.currentAmmo < gunScript.maxAmmo)
        {
            if(other.gameObject.name == "Charging Port")    // is this the port?
            {
                // do a thing
                gunScript.currentAmmo += 3;
            }
        }

        if(gunScript.backUpAmmo < gunScript.maxBackupAmmo)
        {
            if (other.gameObject.name == "Charging Port")    // is this the port?
            {
                // do a thing
                gunScript.backUpAmmo += 3;
            }
        }

    }
    #endregion

    #region Gun Functionality
    public class GunMechanic : MonoBehaviour
    {
        #region Variables
        [Header("Shooting")]
        public LayerMask whatWeCanShoot;
        public AudioSource gunShootSound;
        public Transform gun_firePoint;
        public int gunRange = 30;
        public float fireRate = 0.25f;
        public GameObject bulletHole;

        [Header("Reloadiing")]
        public bool isReloading = false;
        public Animator gunAnimator;

        [Header("Ammo")]  
        // Max ammo in 1 magazine
        public int maxAmmo = 12;
        // current ammo that player has in the magazine
        public int currentAmmo;
        // the ammo the player has spare
        public int backUpAmmo;
        public int maxBackupAmmo = 90;
        public bool isShooting = true;

        [Header("Aim Down Sites")]
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
        public int shootModeController = 0;
        // Fly Shot
        public int numShots = 3;

        [Header("Bullet Type")]
        public BulletType CurrentBulletType = BulletType.Default;
        public enum BulletType { Default, Explosive}
        // Explosive Bullets
        public float explosionRadius = 5.0f;
        public float explosiveForce = 20.0f;

        [Header("UI")]
        public Text currentAmmoText;
        public Text backUpAmmoText;


        [Header("Script Ref")]
        public AI enemyHit;
        #region Debugging
        [HideInInspector]
        public bool Debugging;
        // The Time until next fire
        [Header("$Debugging$ The Timer that determines the next fired shot")]
        [Header("For QA Tester")]
        [HideAttributes("Debugging", true)]
        public float fireTimer;
        [Header("$Debugging$ The Time It takes To Reload")]
        [HideAttributes("Debugging", true)]
        public float reloadTime = 1f;
        [Header("$Debugging$ The orginal position of the players gun")]
        [HideAttributes("Debugging", true)]
        // The orginal position of the gun at the hip
        public Vector3 originalPosition;
        [Header("$Debugging$ The Value which overrides enum BulletType")]
        [HideAttributes("Debugging", true)]
        public int bulletChange;
        [Header("$Debugging$ Damage which the player sends to the AI")]
        [HideAttributes("Debugging", true)]
        public int damage;  // This is set in the switch statement 




        #endregion
        #endregion

        public virtual void Awake()
        {
            #region Value Set-Up
            gunRange = 60;
            currentAmmo = maxAmmo;
            backUpAmmo = maxBackupAmmo;

            // The hip location of the gun
            originalPosition = weaponHolder.localPosition;
            numShots = 3;   // Burst Fire change this for more bullets to spawn

            // Fly Shooting
            if (numShots / 2 * 2 == numShots) numShots++;   // Need an odd number of shots
            if (numShots < 3) numShots = 3; // At least 3 shots are needed
            #endregion

        }

        public virtual void FixedUpdate()
        {
            // Functions
            AimDownSights();

            #region Debuggng Key Press Logic
            if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Y))
            {
                Debugging = true;
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.M))
            {
                Debugging = false;
            }
            #endregion

            if (currentAmmo > maxAmmo)
                currentAmmo = maxAmmo;

            #region Fire Modes n Bullet Types

            #region Shoot Mode Controller Checks
            if (shootModeController == 0)
                shootingMode = ShootMode.Auto;
            else if (shootModeController == 1)
                shootingMode = ShootMode.Semi;
            else if (shootModeController == 2)
                shootingMode = ShootMode.Burst;

            if (bulletChange == 0)
                CurrentBulletType = BulletType.Default;
            else if (bulletChange == 1)
                CurrentBulletType = BulletType.Explosive;

            if (Input.GetKeyUp(KeyCode.Q))
            {
                shootModeController++;
                if (shootModeController >= 3)
                    shootModeController = 0;
            }


            // Key press increases controller value
            if (Input.GetKeyUp(KeyCode.B))
            {
                bulletChange++;
                // value met resets the value
                if (bulletChange >= 2)
                    bulletChange = 0;
            }
            #endregion

            switch (shootingMode)
            {
                case ShootMode.Auto:
                    shootInput = Input.GetButton("Fire1");
                    fireRate = 0.125f;
                    damage = 9;
                break;

                case ShootMode.Semi:
                    shootInput = Input.GetButtonDown("Fire1");
                    fireRate = 0.2f;
                    damage = 10;
                break;

                case ShootMode.Burst:
                    shootInput = Input.GetButtonDown("Fire1");
                    fireRate = 0.6f;
                    damage = 20;
                break;
            }
            #endregion
            // Depending on the inptut and fire mode we can shoot
            if (shootInput)
            {
                if (currentAmmo > 0)
                {
                    // Fire Functions
                    Fire();
                    BurstShot();
                }
            }
            else
            {
                gunShootSound.volume = 0.2f;
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

            // Updating UI For Ammo
            currentAmmoText.text = currentAmmo.ToString();
            backUpAmmoText.text = backUpAmmo.ToString();
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
            // if we are not burst firing
            if(shootModeController != 2)
            {
                // Simple Shoot
                if (isShooting)
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
                            gunShootSound.volume = 0.5f;
                            gunShootSound.Play();

                            GameObject impactHole = Instantiate(bulletHole, Hit.point, Quaternion.FromToRotation(Vector3.forward, Hit.normal)) as GameObject;
                            Destroy(impactHole, 5f);
                            // We want to hit the AI Body and Head to take damage (Could be changed later for more damage when hitting head enemyHit.ApplyDamage(damage * 2);)
                            if (Hit.collider.gameObject.layer == 11 | Hit.collider.gameObject.layer == 14)
                            {
                                // Hurt the AI we hit
                                if (enemyHit != null)
                                {
                                    impactHole.transform.parent = Hit.transform;
                                    enemyHit.ApplyDamage(damage);
                                }
                            }

                            if (Hit.transform.gameObject.layer == 14)
                            {
                                enemyHit.headShot = true;
                            }
                            else
                                enemyHit.headShot = false;

                            if(CurrentBulletType == BulletType.Explosive)
                            {
                                Vector3 explosionPosition = Hit.transform.position;
                                Collider[] objectsHit = Physics.OverlapSphere(explosionPosition, explosionRadius);
                                foreach(Collider objectsInRange in objectsHit)
                                {
                                    Rigidbody otherObjectPhysics = objectsInRange.GetComponent<Rigidbody>();
                                    if (otherObjectPhysics != null)
                                        otherObjectPhysics.AddExplosionForce(explosiveForce, explosionPosition, explosionRadius);
                                }
                            }

                            // Decrease Ammo
                            currentAmmo--;
                            #region Debugging Shooting
                            Debug.Log("Hit" + Hit.transform.name);  // Show on console what we hit
                            Debug.DrawRay(gun_firePoint.transform.position, gun_firePoint.TransformDirection(Vector3.forward) * Hit.distance, Color.red);
                            #endregion
                        }
                    }
                }
            }
        }

        public void BurstShot()
        {
            if(shootModeController == 2)
            {
                if (fireTimer < fireRate) return;
                fireTimer = 0.0f;
                //nextShot = Time.time + timeBetweenShots;
                for (int i = 0; i < numShots; i++)
                {
                    if (isShooting)
                    {
                        // Physics Driven
                        RaycastHit Hit;
                        if (Physics.Raycast(gun_firePoint.transform.position, gun_firePoint.transform.TransformDirection(Vector3.forward), out Hit, gunRange, whatWeCanShoot))
                        {
                            if (currentAmmo > 0)
                            {
                                gunShootSound.Play();
                                //GameObject BulletShot = Instantiate(BulletPosition, gun_firePoint.position, Quaternion.identity) as GameObject;
                                AI enemyHit = Hit.transform.GetComponent<AI>();
                                if (enemyHit != null)
                                {
                                    enemyHit.ApplyDamage(damage);
                                }
                                //BulletShot.name = "Bullet_Sound_Position";
                                // Decrease Ammo
                                currentAmmo--;
                                Debug.Log("Hit" + Hit.transform.name);
                                Debug.DrawRay(gun_firePoint.transform.position, gun_firePoint.TransformDirection(Vector3.forward) * Hit.distance, Color.red);
                            }
                        }
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
    #endregion

}
