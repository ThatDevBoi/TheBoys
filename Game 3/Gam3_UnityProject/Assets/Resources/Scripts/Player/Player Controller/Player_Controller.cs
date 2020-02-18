using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
#if (UNITY_EDITOR) 
[CustomPropertyDrawer(typeof(HideAttributes))]
[RequireComponent(typeof(FieldOfView))]
#endif
public class Player_Controller : MonoBehaviour
{
    #region Variablles
    [Header("Movement")]
    // PUBLIC
    public float runSpeed = 30;
    // are we holding shift
    public bool running = false;
    public float dragRB = 5;
    public float gravityModifier = 120f;
    public float currentSpeed = 20;
    public float cameraRotationRate = 45;   // Rate we rotate at
    public AudioSource walkingSound;
    public LayerMask slopCheck;
    private float runTime = 1;
    public float sensitivity_x;
    public float sensitivity_y;

    public static Vector3 savedPosition;
    public bool playerDead=false;

    [Header("Health n Damage")]
    [HideInInspector]
    public int maxHealth = 100;
    public int currentHealth;
    // This array if for the images that are on the Player Healthbar 
    public Image[] sliderArray;
    public TextMeshPro healthPercentageText;
    public Canvas respawnCan;
    public Color[] HealthBar_StageColors;

    [Header("Health Bar")]
    public Slider healthBar;

    [Header("Steps")]
    // The max height of the step that player can step up on
    public float maxStepHeight = 0.4f;
    // how much overshoot does the direction of a potential step in units prevents stepping on low steps
    public float stepSearchOvershoot = 0.01f;
    // the liost of points the player collds with
    private List<ContactPoint> allCPs = new List<ContactPoint>();
    // The Velocity of the last step
    public Vector3 lastVelocity;

    [Header("Events")]
    public KeyCode pauseGamekey;

    #region Debugging
    [HideInInspector]
    public bool Debugging;
    /// <summary>
    /// Player Components The components that the player relies on to set up
    /// </summary>
    [Header("$Debugging$ The Rigidbody Physics Component")]
    [Header("For QA Tester")]
    [HideAttributes("Debugging", true)]
    // Rigidbody for player movement
    public Rigidbody playerPhysics;
    [Header("$Debugging$ Players Collision detection")]
    [HideAttributes("Debugging", true)]
    // Collider for player detection
    public BoxCollider playerCollision;
    [Header("$Debugging$ The Main Camera Component")]
    [HideAttributes("Debugging", true)]
    // Player FPS camera
    public Transform playersEyes; // Player Camera
    /// <summary>
    /// Player Movement Variables This can be for the players position, rotation etc
    /// </summary>
    [Header("$Debugging$ The Players Movement Direction")]
    [HideAttributes("Debugging", true)]
    public Vector3 movementDirection;
    [Header("$Debugging$ The X Direction of movement")]
    [HideAttributes("Debugging", true)]
    public float dirX;
    [Header("$Debugging$ The Z Direction of movemet")]
    [HideAttributes("Debugging", true)]
    public float dirZ;
    [Header("$Debugging$ Value which monitors X Rotation")]
    [HideAttributes("Debugging", true)]
    public float xRotation = 0f;   // Value that monitors X Rotation keep it at 0 for default
    [Header("$Debugging$ The Value for movement speed")]
    [HideAttributes("Debugging", true)]
    public float speed;
    /// <summary>
    /// References to find in the assets folder
    /// </summary>
    [HideAttributes("Debugging", true)]
    private GameObject pauseCan;
    /// <summary>
    /// Scripts the player movemtn needs to access
    /// </summary>
    [HideAttributes("Debugging", true)]
    Shooting_Mechanic gunScript;

    #endregion

    #endregion

    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.name != "PC")
        {
            Debug.LogWarning("The PC needs to be named -PC- try not to change it");
            gameObject.name = "PC";
        }

        sensitivity_x = 20;
        sensitivity_y = 20;

        // when the player spawns it saves that spawn position as a saved place when death occurs
        savedPosition = gameObject.transform.position;
        // States on start that need changing
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Find Components / Set them up
        #region Find Compoents / Assets
        if (gameObject.GetComponent<Rigidbody>() == null)
        {
            // Add Rigidbody to this gameObject
            playerPhysics = gameObject.AddComponent<Rigidbody>();
        }
        else
        {
            Debug.LogWarning("Note the script makes the rigidbody for you, so you dont need to add it if you dont want to");
            playerPhysics = gameObject.GetComponent<Rigidbody>();
        }
        // alter the drag of the players Rigidbody drag value
        playerPhysics.drag = dragRB;
        if(gameObject.GetComponent<BoxCollider>() == null)
        {
            // Add Collision to this gameObject
            playerCollision = gameObject.AddComponent<BoxCollider>();
        }
        else
        {
            // Find the collision already attached
            playerCollision = gameObject.GetComponent<BoxCollider>();
        }
        // Find the derived class
        gunScript = transform.Find("FPS_Cam/Weapon_Holder/Pistol Holder/Pistol").GetComponent<Shooting_Mechanic>();
        // Find the Main Camera
        playersEyes = Camera.main.GetComponent<Transform>();
        walkingSound = GetComponent<AudioSource>();
        // Find Health Bar Slider
        healthBar = GameObject.Find("PlayerUIController/PC_HealthBar").GetComponent<Slider>();
        // Finds the DetectWheel 
        RectTransform DetectWheel;
        // Find UI Component
        DetectWheel = GameObject.Find("HitDetection/DetectionWheel").GetComponent<RectTransform>();
        // Finding Percentage Text
        healthPercentageText = GameObject.Find("Percentage_Health_Text").GetComponent<TextMeshPro>();
        if(GameObject.Find("Pause_Canvas") == null)
        {
            return;
        }
        else
        {
            // Find pause Canvas
            pauseCan = GameObject.Find("Pause_Canvas");
            pauseCan.SetActive(false);
        }
        respawnCan = GameObject.Find("RestartCanvas").GetComponent<Canvas>();
        // Find all the images in the slider
        sliderArray = healthBar.transform.GetComponentsInChildren<Image>();

        #region Finding variables from assets

            #endregion
        #endregion

        #region Edit Values / Variables and Properties
        // Set up components
        // Freeze rotation for now so no falling down
        playerPhysics.constraints = RigidbodyConstraints.FreezeRotation;
        // Change object layer to the player layer
        gameObject.layer = 10;
        // Current health will need to start at the max amount
        currentHealth = maxHealth;
        // the speed we walk at needs to be the current speed we are allowed to walk at
        speed = currentSpeed;
        // the sensitivy of the mouse rotation 
        cameraRotationRate = 45f;
        xRotation = 0f;
        // set up audio volume
        walkingSound.volume = 0.3f;
        DetectWheel.rotation = new Quaternion(0, 0, 180, 0);
        #endregion

        #region Debug Components
        if (playerPhysics == null)
            Debug.LogError("Object Name" + ":" + gameObject.transform.name + ":" + "No Rigidbody is applied to the Player Character!! Check the script when it adds the components");

        if(playerCollision == null)
            Debug.LogError("Object Name" + ":" + gameObject.transform.name + ":" + "No Capsule Collider is applied to the player character!! Check the script when it adds the components");

        if (gameObject.layer != 10)
            Debug.LogError("Object Name" + ":" + gameObject.transform.name + ":" + "The Player Controller GameObject Is Not Set To Its Correct Layer!! The Layer For The PC Is 10");
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Level Manager Find SetUp
        if (pauseCan == null)
        {
            // Find pause Canvas
            pauseCan = GameObject.Find("Pause_Canvas");
            pauseCan.SetActive(false);
        }
        #endregion


        if (Input.GetKey(pauseGamekey))
        {
            // turn on the canvas
            pauseCan.SetActive(true);
            // unlock the cursor
            Cursor.lockState = CursorLockMode.None;
            // make the cursor visable
            Cursor.visible = true;
            // Pause the entire scene
            Time.timeScale = 0;
        }

        // if the player has died   currentHealth >= 0
        if (playerDead)
        {
            // respawn at the old checkpoint
            transform.position = savedPosition;
        }
        else
        {
            playerDead = false;
            respawnCan.enabled = false;
            respawnCan.transform.GetChild(0).gameObject.SetActive(true);
            // Functions
            FPSMove(speed, dirX, dirZ, movementDirection);

            HealthBar();
            // Never go over max health
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            // Show on screen Health bar percentage with this text
            healthPercentageText.text = currentHealth + "%";

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
    }
    #endregion
    // This function moves the FPS character and covers all its logic
    #region FPS Movement Function
    public void FPSMove(float speed, float V, float H, Vector3 direction)
    {
        #region Core Movement
        // Input
        V = Input.GetAxis("Vertical");
        H = Input.GetAxis("Horizontal");

        // Run Input
        float r = Input.GetAxis("Run");
        // direction we are moving
        direction = (transform.forward * V + (transform.right * H));
        // Make the vector to the equal of 1
        direction = direction.normalized * speed;
        if (playerPhysics.velocity.magnitude > 2f && walkingSound.isPlaying == false && running == false)
        {
            walkingSound.volume = Random.Range(0.4f, 0.6f);
            walkingSound.pitch = Random.Range(0.4f, 0.6f);
            walkingSound.Play();
        }
        #endregion

        #region Running
        // if there is input that says te player is holding shift and w or S or arrow keys 
        if (r != 0 && Input.GetAxis("Vertical") != 0)
        {
            // we are running
            running = true;
            #region Footsteps
            // Footsteps
            if (playerPhysics.velocity.magnitude > 2f && walkingSound.isPlaying == false)
            {
                walkingSound.volume = Random.Range(0.8f, 1f);
                walkingSound.pitch = Random.Range(0.8f, 1f);
                walkingSound.Play();
            }
            #endregion
            // cooldown for not being detected on key press straight away by the NPC
            runTime -= Time.deltaTime;
            // when value is 0 or more
            if(runTime <= 0)
            {
                // change value of audio volume
                walkingSound.volume = 0.6f;
                // reset runtime
                runTime = 1;
            }
            // speed value becomes runspeed
            speed = runSpeed;
        }
        else
        {
            // no longer running
            running = false;
            // reset timer
            runTime = 1;
            // change audio trigger
            walkingSound.volume = 0.3f;

            // change speed back to the current speed we walk at
            speed = currentSpeed;
        }
        // move with physics
        playerPhysics.velocity = direction * speed * Time.deltaTime;
        #endregion           


        // Camera Rotation Logic
        #region Fixed Camera Rotation
        // Input storage floats
        float mouseRotX;
        float mouseRotY;

        // Camera Rotation
        // Input for rotation
        mouseRotX = Input.GetAxis("Mouse X") * sensitivity_x * Time.deltaTime;
        mouseRotY = Input.GetAxis("Mouse Y") * sensitivity_y * Time.deltaTime;

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
    #endregion

    #region Damage
    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
    }
    #endregion
    // This Function is for the control of the PC health bar slider in unity
    #region Health Bar Function
    public void HealthBar()
    {
        // Set up the Slider value to be the current health value
        healthBar.value = currentHealth;
        #region Player Health Bar Transitions
        // Foreach image that is in our array of images for the healthbar slider
        // The images are the slider graphics
        foreach (Image PC_HealthBarImages in sliderArray)
        {
            // We need to check that we are at max health as we will be turning the UI Alpha channels to 0
            if (currentHealth >= maxHealth)
            {
                // Change all image color alpha channels to 0
                PC_HealthBarImages.color = new Color(0, 1, 1, Mathf.Lerp(0, 0, 0));
            }
            else    // However when we are no longer at max health
            {
                // We need to turn the UI Back on
                PC_HealthBarImages.color = new Color(1, 1, 1, Mathf.Lerp(1, 1, 1));


                // Now we need to check what our current health is as the healthbar will change color
                // so when the current is near the 100 mark we have a green health bar
                if (currentHealth > maxHealth | currentHealth > 60)
                {
                    // We find the image each time as these if statements wont run often and it saves us making ir a variable in the inspector
                    Image healthImage = GameObject.Find("PlayerHP_Fill").GetComponent<Image>();
                    healthImage.color = HealthBar_StageColors[0];
                }
                // when the current health is less than 60 or is 60
                // Make fill bar Yellow
                else if (currentHealth == 60)
                {
                    // We find the image each time as these if statements wont run often and it saves us making ir a variable in the inspector
                    Image healthImage = GameObject.Find("PlayerHP_Fill").GetComponent<Image>();
                    healthImage.color = HealthBar_StageColors[1];
                }
                // When the current health is 30 or less 
                // make fill bar Red
                else if (currentHealth == 30)
                {
                    // We find the image each time as these if statements wont run often and it saves us making ir a variable in the inspector
                    Image healthImage = GameObject.Find("PlayerHP_Fill").GetComponent<Image>();
                    healthImage.color = HealthBar_StageColors[2];
                }
            }

        }

        if (currentHealth <= 0)
        {
            playerDead = true;
            respawnCan.enabled = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playerDead = false;
            respawnCan.enabled = false;
            Time.timeScale = 1;
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        #endregion
    }
    #endregion

    #region Players Hit Detection
    // This Function gets called in the AI shooting logic. Whenever the AI shoots us we need its Transform Component to find its position
    // I did it this wasy as there will be multiple Enemies and we dont want to monitor each one individually
    public GameObject bloodSplash;
    public void HitDetection(Transform targetThatShotUs)
    {
        // THE UI WE ARE USING IS IN A CERTAIN PARENT CHILD RELATIONSHIP
        #region The Order Relationship
        // CANVAS -- IN WORLD SPACE USING FPS CAM -- Parent
        // DETECTIONWHEEL -- Uses a Image for a background and is rotated 180 on the Z Axis So the pointer points in the correct position -- Child of Canvas but a sub parent to Rotator
        // ROTATER -- Rotater is used to make the pointer rotate in a 360 angle -- Child of DetectionWheel but subParent to Pointer
        // POINTER -- The 2D sprite which is pointing in  the diorection of the enemies that shoot us -- Child of Rotator
        #endregion
        
        // The layer in UI Which will rotate and show us where the enemy is 
        RectTransform hitLayer;
        // Find the UI Layer we want
        hitLayer = GameObject.Find("HitDetection/DetectionWheel/Rotater").GetComponent<RectTransform>();
        // TThe Vector3 is local so it needs to equal 0 to begin with
        Vector3 northDirection = Vector3.zero;
        // The Z axis of our local Vector3 needs to take into mind the players Euler angles for the Y axis
        northDirection.z = gameObject.transform.eulerAngles.y;
        // Rotation variable 
        Quaternion directionOfHit;
        // The Direction between the AI that shoots the player and the player pos
        Vector3 dir = transform.position - targetThatShotUs.position;
        // Rotate towards the direction of the AI from this position
        directionOfHit = Quaternion.LookRotation(dir);
        directionOfHit.z = -directionOfHit.y;
        directionOfHit.x = 0;
        directionOfHit.y = 0;
        // Make the UI Component rotate smoothly
        hitLayer.localRotation = directionOfHit * Quaternion.Euler(northDirection);
        #region Player Feedback Indication
        // Find the pointer
        RectTransform pointer = GameObject.Find("HitDetection/DetectionWheel/Rotater/Pointer").GetComponent<RectTransform>();
        // find the object we want to spawn in the assets
        if (bloodSplash == null)
        {
            bloodSplash = Resources.Load<GameObject>("Prefabs/Player/Feedback/Damage_Indicator");
        }
        // spawn object into the scene to indicate where the player got hit from
        Instantiate(bloodSplash, pointer.position, directionOfHit * Quaternion.Euler(northDirection), pointer.transform);
        #endregion

    }
    #endregion

    #region Slop & Stair Detection
    // Used to keep the orginal gravity settings in Project Settings
    public static float globalGravity = -9.81f;
    void FixedUpdate()
    {
        playerPhysics.useGravity = false;
        // get the rigibody velocity
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;
        //Filter through the ContactPoints to see if we're grounded and to see if we can step up
        ContactPoint groundCP = default(ContactPoint);
        // bool is true or false depeinging on bool function detecing ground 
        bool grounded = FindGround(out groundCP, allCPs);
        //Vector3 that is used for the step up to the stair we detect
        Vector3 stepUpOffset = default(Vector3);
        // Make bool to detect if we step up
        bool stepUp = false;
        // if we are grounded and not stepping up
        if (grounded)
        {
            // make boolean the logic of the Find step boolean function
            stepUp = FindStep(out stepUpOffset, allCPs, groundCP, velocity);
            // check if the velocity on the Characters Y is less than 0
            if(this.GetComponent<Rigidbody>().velocity.y <= 0 && Input.GetKeyDown(KeyCode.LeftShift) == null)
            {
                // have normal gravity if we are on the ground
                Physics.gravity = new Vector3(0, -9.81f, 0);
            }
            else
            {
                // if we are not on the ground add gravity to decline faster
                // THIS IS CAUSING ISSUES FOR THE HEADSHOT LOGIC
                //Physics.gravity = new Vector3(0, -60, 0);

                // New Logic
                // Needs to be called when the player rigidbody velocity is not 0 as it means we are moving
                Vector3 gravity = globalGravity * gravityModifier * Vector3.up;
                playerPhysics.AddForce(gravity, ForceMode.Acceleration);
            }
        }
        else
        {
            //// if we are not on the ground add gravity to decline faster
            //Physics.gravity = new Vector3(0, -gravityModifier, 0);

            // New Logic
            // if we are not on the ground then we need to go down 
            Vector3 gravity = globalGravity * gravityModifier * Vector3.up;
            playerPhysics.AddForce(gravity, ForceMode.Acceleration);

        }

        //if we are able to step upward to a new velocity level
        if (stepUp)
        {
            // increase the position
            this.GetComponent<Rigidbody>().position += stepUpOffset;
            // let the physics have the last known velocity
            this.GetComponent<Rigidbody>().velocity = lastVelocity;
        }
        // clear all of the List for contact points
        allCPs.Clear();
        // the last velcity the player had is the velocity of the Rigidbody now
        lastVelocity = velocity;
    }
    #region Find Contact Points
    void OnCollisionEnter(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }

    void OnCollisionStay(Collision col)
    {
        allCPs.AddRange(col.contacts);
    }
    #endregion

    /// Finds the MOST grounded (flattest y component) ContactPoint
    /// \param allCPs List to search
    /// \param groundCP The contact point with the ground
    /// \return If grounded
    bool FindGround(out ContactPoint groundCP, List<ContactPoint> allCPs)
    {
        // the ground contact point
        groundCP = default(ContactPoint);
        // boolean so we can find the ground
        bool found = false;
        foreach (ContactPoint cp in allCPs)
        {
            //Pointing with some up direction
            if (cp.normal.y > 0.0001f && (found == false || cp.normal.y > groundCP.normal.y))
            {
                groundCP = cp;
                found = true;
            }
        }
        return found;
    }

    /// Find the first step up point if we hit a step
    /// \param allCPs List to search
    /// \param stepUpOffset A Vector3 of the offset of the player to step up the step
    /// \return If we found a step
    bool FindStep(out Vector3 stepUpOffset, List<ContactPoint> allCPs, ContactPoint groundCP, Vector3 currVelocity)
    {
        // how far we can step up 
        stepUpOffset = default(Vector3);

        //No chance to step if the player is not moving
        Vector2 velocityXZ = new Vector2(currVelocity.x, currVelocity.z);
        if (velocityXZ.sqrMagnitude < 0.0001f)
            return false;

        foreach (ContactPoint cp in allCPs)
        {
            bool test = ResolveStepUp(out stepUpOffset, cp, groundCP);
            if (test)
                return test;
        }
        return false;
    }

    /// Takes a contact point that looks as though it's the side face of a step and sees if we can climb it
    /// \param stepTestCP ContactPoint to check.
    /// \param groundCP ContactPoint on the ground.
    /// \param stepUpOffset The offset from the stepTestCP.point to the stepUpPoint (to add to the player's position so they're now on the step)
    /// \return If the passed ContactPoint was a step
    bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCP, ContactPoint groundCP)
    {
        stepUpOffset = default(Vector3);
        Collider stepCol = stepTestCP.otherCollider;

        //( 1 ) Check if the contact point normal matches that of a step (y close to 0)
        if (Mathf.Abs(stepTestCP.normal.y) >= 0.01f)
        {
            return false;
        }

        //( 2 ) Make sure the contact point is low enough to be a step
        if (!(stepTestCP.point.y - groundCP.point.y < maxStepHeight))
        {
            return false;
        }

        //( 3 ) Check to see if there's actually a place to step in front of us
        //Fires one Raycast
        RaycastHit hitInfo;
        float stepHeight = groundCP.point.y + maxStepHeight + 0.0001f;
        Vector3 stepTestInvDir = new Vector3(-stepTestCP.normal.x, 0, -stepTestCP.normal.z).normalized;
        Vector3 origin = new Vector3(stepTestCP.point.x, stepHeight, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 direction = Vector3.down;
        if (!(stepCol.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight)))
        {
            return false;
        }

        //We have enough info to calculate the points
        Vector3 stepUpPoint = new Vector3(stepTestCP.point.x, hitInfo.point.y + 0.0001f, stepTestCP.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 stepUpPointOffset = stepUpPoint - new Vector3(stepTestCP.point.x, groundCP.point.y, stepTestCP.point.z);

        //We passed all the checks! Calculate and return the point!
        stepUpOffset = stepUpPointOffset;
        return true;
    }
    #endregion

    #region Charging Port (OnTriggerStay)
    // Base for charging port (For ammo and health)
    public void OnTriggerStay(Collider other)
    {
        // if we are overlapping a trigger with the name Charging port
        if (other.gameObject.name == "Charging Port")   // is this a charging port?
        {
            //increase health
            currentHealth += 2;
            // if the current Ammo is greater than the max amount
            if (gunScript.currentAmmo > gunScript.maxAmmo)
                gunScript.currentAmmo = gunScript.maxAmmo;  // make sure the current ammo cant overtake the max value
            else    // if not and the current ammo is less than the max
            {
                // increase the ammo
                gunScript.currentAmmo += 3;
            }
            // if the backup ammo we have is more then the max amount we are allowed
            if (gunScript.backUpAmmo > gunScript.maxBackupAmmo)
                gunScript.backUpAmmo = gunScript.maxBackupAmmo;    // the backup ammo we have equals to the max
            else    // However if we dont have more but instead have less
            {
                // we increase the backup ammo
                gunScript.backUpAmmo += 3;
            }
        }

        // when we hit the trigger then we save the position
        if(other.gameObject.tag == "CheckPoint")
        {
            savedPosition = other.transform.position;
        }
    }
    #endregion

    #region Gun Functionality
    public class GunMechanic : MonoBehaviour
    {
        #region Variables
        [Header("Shooting")]
        // the objects with relevent layers to shoot
        public LayerMask whatWeCanShoot;
        // audio source we use to shoot
        public AudioSource gunShootSound;
        // the players FPS camera
        public Camera cam_FirePosition;
        // the view of the camera when not aiming
        public float currentFieldOfView = 30;
        // the FOV when we are aimig down sights
        public float aimFOV = 18;
        // boolean which tells us if we are aiming
        public bool imAiming = false;
        // range of hor far we can shoot
        public int gunRange = 30;
        // the rate of fire
        public float fireRate = 0.25f;
        // the damage we deal to enemies
        public int damage;
        // bridge that should be triggered
        public GameObject bridge;
        // weapon script for recoil
        public Weapon_Recoil recoilScript;

        // Effects
        private GameObject bulletHole;
        private GameObject muzzleFlash;
        private GameObject hitMarker;
        // Changes revolver color On Pistol
        private Material defaultBulletMat;
        private Material explosiveBulletMat;

        [Header("Reloadiing")]
        // are we reloading the gun?
        public bool isReloading = false;
        public Animator gunAnimator;


        [SerializeField]
        private AudioSource audioComponent;
        [SerializeField]
        private AudioClip shootingSound;
        [SerializeField]
        private AudioClip reloadSound;

        [Header("Ammo")]  
        // Max ammo in 1 magazine
        public int maxAmmo = 12;
        // current ammo that player has in the magazine
        public int currentAmmo;
        // the ammo the player has spare
        public int backUpAmmo;
        public int maxBackupAmmo = 90;
        // are we shooting the gun?
        public bool isShooting = true;

        [Header("Aim Down Sites")]
        // Where the object will be when aiming
        public Vector3 aimPosition;
        // Changes position for aiming
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
        public float upforce = 1;

        [Header("Melee Attack")]
        public bool isPunching = false;
        public Animator punchController;

        [Header("UI")]
        public TextMeshPro currentAmmoText;
        public TextMeshPro backUpAmmoText;

        public AI enemyHit;

        private GameManager game_manager;

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




        #endregion
        #endregion
        public Animator Bridge;
        public virtual void Awake()
        {
            #region Find Components
            if (enemyHit == null)
                enemyHit = null;
            // Find the character controller camera
            cam_FirePosition = Camera.main;
            // Find the shooting Audio Source
            gunShootSound = GetComponent<AudioSource>();
            // Find the reload anim controller
            gunAnimator = GetComponent<Animator>();
            punchController = GameObject.Find("Fist").GetComponent<Animator>();
            // Find the weapon holder
            weaponHolder = GameObject.Find("Weapon_Holder").GetComponent<Transform>();
            // find recoil script
            GameObject gunHolder = GameObject.Find("Pistol Holder");
            // find the recoil script
            recoilScript = GetComponent<Weapon_Recoil>();
            if(GameObject.Find("Ammo_In_The_Mag_Text") == null && GameObject.Find("BackUp_Ammo_Text") == null)
            {
                Debug.LogWarning("Level Manager sets this up");
            }
            else
            {
                // Find text component for the current ammo variable
                currentAmmoText = GameObject.Find("Ammo_In_The_Mag_Text").GetComponent<TextMeshPro>();
                // Find text component for the back up ammo variable
                backUpAmmoText = GameObject.Find("BackUp_Ammo_Text").GetComponent<TextMeshPro>();
            }
            // find game manager
            game_manager = GameManager.FindObjectOfType<GameManager>();

            // Find the bullet hole prefab in Resources in the asset folder
            bulletHole = Resources.Load<GameObject>("GunEffects/ImpactEffect");
            // Find Muzzle Flash
            muzzleFlash = Resources.Load<GameObject>("GunEffects/MuzzleFlash");

            // Pistol color change revolver barrel
            defaultBulletMat = Resources.Load<Material>("Player/Gun/Materials/Pistol/DefaultBulletMat");
            explosiveBulletMat = Resources.Load<Material>("Player/Gun/Materials/Pistol/ExplosiveBulletMat");
            hitMarker = Resources.Load<GameObject>("Player/Gun/Prefabs/Hitmarker");

            // Find sounds
            shootingSound = Resources.Load<AudioClip>("Audio/Player/Guns/laser-gun-19sf");
            reloadSound = Resources.Load<AudioClip>("Audio/Player/Guns/laser reload");
            #endregion

            #region Value Set-Up
            // Start Range for gun
           // gunRange = 30;//this prevents the variable to be changed in ide DM
            // Start ammo in mag
            currentAmmo = maxAmmo;
            // start backup ammo
            backUpAmmo = maxBackupAmmo;

            // The hip location of the gun
            originalPosition = weaponHolder.localPosition;
            numShots = 3;   // Burst Fire change this for more bullets to spawn

            // Fly Shooting
            if (numShots / 2 * 2 == numShots) numShots++;   // Need an odd number of shots
            if (numShots < 3) numShots = 3; // At least 3 shots are needed

            // Aim Positions
            if (gameObject.name == "Pistol")
                aimPosition = new Vector3(-0.55f, -1f, 1f);
            // Start FOV for the FPS Cam
            cam_FirePosition.fieldOfView = currentFieldOfView;

            audioComponent = gameObject.GetComponent<AudioSource>();
            audioComponent.clip = shootingSound;
            #endregion
        }

        public virtual void Update()
        {
            // if the level mamager spawns our UI we need to find it again
            // This only runs when updates starts
            if (currentAmmoText == null && backUpAmmoText == null)
            {
                // Find text component for the current ammo variable
                currentAmmoText = GameObject.Find("Ammo_In_The_Mag_Text").GetComponent<TextMeshPro>();
                // Find text component for the back up ammo variable
                backUpAmmoText = GameObject.Find("BackUp_Ammo_Text").GetComponent<TextMeshPro>();
                // Set UI For Ammo
                // Text for current ammo
                currentAmmoText.text = currentAmmo.ToString();
                // Text for current Backup Ammo
                backUpAmmoText.text = backUpAmmo.ToString();
            }

            // Functions
            AimDownSights();
            Punch();

            #region Debuggng Key Press Logic
            // Press alL the keys listed together to see hidden variables
            //R + Space Bar + Y
            if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Y))
            {
                Debugging = true;
            }
            // Press all together to revert 
            // D + M
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.M))
            {
                Debugging = false;
            }
            #endregion

            // Max Ammo limit 
            // when the current ammo goes past the max amount 
            if (currentAmmo > maxAmmo)
                currentAmmo = maxAmmo;  // then the current will always = to max
            // Back Up Ammo Limit
            if (backUpAmmo > maxBackupAmmo)
                backUpAmmo = maxBackupAmmo; // Backup Ammo will always = max amount of backup ammo we are allowed

            #region Fire Modes n Bullet Types
            // ShootingModeController
            // Press Q Key to increase or scroll through shooting types

            // 0 = Auto
            // 1 = Semi
            // 2 = Burst

            // Bullet Types 
            // 0 = Defualt 
            // 1 = Explosive
            #region Shoot Mode Controller Checks
            if (shootModeController == 0 && game_manager.singleFire == true)
                shootingMode = ShootMode.Semi;
            else if (shootModeController == 1 && game_manager.fullAuto == true)
                shootingMode = ShootMode.Auto;
            else if (shootModeController == 2 && game_manager.burstFire == true)
                shootingMode = ShootMode.Burst;

            if (bulletChange == 0)
            {
                CurrentBulletType = BulletType.Default;
                GameObject revolverBarrel = GameObject.Find("Pistol/RevolverBarrel");
                Renderer rend = revolverBarrel.GetComponent<Renderer>();
                rend.material = defaultBulletMat;
            }
            else if (bulletChange == 1)
            {
                CurrentBulletType = BulletType.Explosive;
                GameObject revolverBarrel = GameObject.Find("Pistol/RevolverBarrel");
                Renderer rend = revolverBarrel.GetComponent<Renderer>();
                rend.material = explosiveBulletMat;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (shootModeController == 0 && game_manager.fullAuto == false && game_manager.burstFire == false)
                    shootModeController = 0;
                else if (shootModeController == 0 && game_manager.fullAuto == true)
                    shootModeController++;
                else if (shootModeController == 1 && game_manager.burstFire == true)
                    shootModeController++;
                else if (shootModeController == 0 && game_manager.burstFire == true)
                {
                    shootModeController = 2;
                }
                else if (shootModeController == 0 && game_manager.fullAuto == true && game_manager.burstFire == false)
                {
                    shootModeController = 1;
                }
                else if (shootModeController >= 2 && game_manager.burstFire == true)
                    shootModeController = 0;
                else if (shootModeController >= 1 && game_manager.burstFire == false)
                    shootModeController = 0;

            }


            // Key press increases controller value
            if (Input.GetKeyDown(KeyCode.E) && game_manager.explosiveAmmo == true)
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

            // Update the UI for current ammo and backup ammo per frame
            if(currentAmmoText != null && backUpAmmoText != null)
            {
                currentAmmoText.text = currentAmmo.ToString();
                backUpAmmoText.text = backUpAmmo.ToString();
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

            #region Sound Switcher
            if (isReloading && !isShooting && currentAmmo <= 0)
            {
                audioComponent.clip = reloadSound;
            }
            else if(isReloading && !isShooting && currentAmmo > 0 && Input.GetKeyDown(KeyCode.R))
            {
                audioComponent.clip = reloadSound;
            }
            else if(!isReloading && isShooting && currentAmmo > 0)
            {
                audioComponent.clip = shootingSound;
            }
            #endregion
        }
        // player conmtroller script
        Player_Controller PlayerClass;
        public void AimDownSights()
        {
            if (GameManager.gunOverride == false)
            {
                PlayerClass = GameObject.Find("PC").GetComponent<Player_Controller>();
                if (PlayerClass.running == false)
                {
                    // if we press the right mouse button and we are not reloading
                    if (Input.GetButton("Fire2") && !isReloading)
                    {
                        imAiming = true;
                        // Lerp the weapon to the aim position we set up outside the script 
                        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, aimPosition, Time.deltaTime * adsSpeed);
                        // Chnage the FOV on the camera
                        cam_FirePosition.fieldOfView = aimFOV;
                    }
                    else   // if we are not aiming or the reloading bool is true
                    {
                        imAiming = false;
                        // make the gun go back to its hip position
                        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * adsSpeed);
                        // change the camera FOV to be what it starts as
                        cam_FirePosition.fieldOfView = currentFieldOfView;
                    }
                }
                else
                {
                    imAiming = false;
                    return;
                }
            }
            else
                return;
            
        }

        // Casual Shooting
        public void  Fire()
        {
            if(!isReloading)
            {
                // if the player is near a wall we dont want to shoot
                if (GameManager.gunOverride == false)
                {
                    // if we are not burst firing
                    if (shootModeController != 2)
                    {
                        // we need to check run as we dont want to shoot the gun while running
                        if (PlayerClass.running == false)
                        {
                            // Simple Shoot
                            if (isShooting)
                            {
                                // If the timer isnt less than the rate of fire then we dont run the code below
                                if (fireTimer < fireRate) return;
                                fireTimer = 0.0f;   // Reset the timer
                                                    // Physics Driven
                                RaycastHit Hit;
                                // 
                                if (Physics.Raycast(cam_FirePosition.transform.position, cam_FirePosition.transform.forward, out Hit, gunRange, whatWeCanShoot))
                                {
                                    if (currentAmmo > 0)
                                    {
                                        // call recoil
                                        recoilScript.Fire();
                                        gunShootSound.volume = 0.5f;
                                        gunShootSound.Play();
                                        // Decrease Ammo
                                        currentAmmo--;
                                        // Muzzle Flash
                                        GameObject particle_point = GameObject.Find("Pistol/ironSights/FirePoint"); // Find the spawn position
                                        GameObject flashMuzzle = Instantiate(muzzleFlash, particle_point.transform.position, Quaternion.identity) as GameObject;
                                        Destroy(flashMuzzle, .5f);
                                        // Impact Effect
                                        GameObject impactHole = Instantiate(bulletHole, Hit.point, Quaternion.FromToRotation(Vector3.forward, Hit.normal)) as GameObject;
                                        Destroy(impactHole, 5f);

                                        // if we hit any gameObject in the scene
                                        if (Hit.collider.gameObject)
                                        {
                                            // the parent of they object becomes the hit point
                                            impactHole.gameObject.transform.parent = Hit.transform;
                                        }

                                        // We want to hit the AI Body and Head to take damage (Could be changed later for more damage when hitting head enemyHit.ApplyDamage(damage * 2);)
                                        if (Hit.collider.gameObject.layer == 11 | Hit.collider.gameObject.layer == 14)
                                        {
                                            GameObject HitMark = Instantiate(hitMarker, Hit.point, Quaternion.FromToRotation(Vector3.forward, Hit.normal)) as GameObject;
                                            HitMark.transform.parent = Hit.transform;
                                            Destroy(HitMark, .2f);

                                            if (Hit.collider.name == "Head")
                                            {
                                                enemyHit = Hit.collider.gameObject.GetComponentInParent<AI>();
                                                enemyHit.pushback = true;
                                                enemyHit.StartCoroutine(enemyHit.Knockback());
                                            }
                                            else
                                            {
                                                enemyHit = Hit.collider.gameObject.GetComponent<AI>();
                                                enemyHit.pushback = true;
                                                enemyHit.StartCoroutine(enemyHit.Knockback());
                                            }

                                            if (Hit.collider.gameObject.layer == 14)
                                            {
                                                enemyHit.headShot = true;
                                            }
                                            else if (Hit.collider.gameObject.layer != 14)
                                            {
                                                enemyHit.headShot = false;
                                            }

                                            // Hurt the AI we hit
                                            if (enemyHit != null)
                                            {
                                                impactHole.transform.parent = Hit.transform;
                                                enemyHit.ApplyDamage(damage);
                                            }
                                        }
                                        else
                                            enemyHit = null;

                                        // if we hit the trigger that belongs to the bridge 
                                        if (Hit.collider.gameObject.layer == 17)
                                        {
                                            if (bridge == null)
                                            {
                                                bridge = GameObject.Find("Bridge");
                                            }
                                            // Play the animation for the bridge to appear
                                            bridge.GetComponent<Animator>().SetBool("Activate", true);
                                        }

                                        if (CurrentBulletType == BulletType.Explosive)
                                        {
                                            Vector3 explosionPosition = Hit.transform.position;
                                            Collider[] objectsHit = Physics.OverlapSphere(explosionPosition, explosionRadius);
                                            foreach (Collider objectsInRange in objectsHit)
                                            {
                                                Rigidbody otherObjectPhysics = objectsInRange.GetComponent<Rigidbody>();
                                                if (otherObjectPhysics != null)
                                                    otherObjectPhysics.AddExplosionForce(explosiveForce, explosionPosition, explosionRadius, upforce, ForceMode.Impulse);
                                            }
                                        }
                                        #region Debugging Shooting
                                        Debug.Log("Hit" + Hit.transform.name);  // Show on console what we hit
                                        Debug.DrawRay(cam_FirePosition.transform.position, cam_FirePosition.transform.forward * Hit.distance, Color.red);
                                        #endregion
                                    }
                                }
                            }
                        }
                        else
                            return;
                    }
                }
            }
        }
        // Intergrate with Fire Function Later
        public void BurstShot()
        {
            // when the shooting mode is changes to the value of 2 which is for burst fire
            if(shootModeController == 2)
            {
                // if the firerate allows us the shoot
                if (fireTimer < fireRate) return;
                fireTimer = 0.0f;   // reset timer
                // allow us to shoot more than 1 bullet
                for (int i = 0; i < numShots; i++)
                {
                    if (PlayerClass.running == false)
                    {
                        // can we shoot?
                        if (isShooting)
                        {
                            // Physics Driven
                            RaycastHit Hit;
                            // shoot a ray at the direction we desire and the laymask we can shoot at
                            if (Physics.Raycast(cam_FirePosition.transform.position, cam_FirePosition.transform.TransformDirection(Vector3.forward), out Hit, gunRange, whatWeCanShoot))
                            {
                                if (currentAmmo > 0)
                                {
                                    // call recoil
                                    recoilScript.Fire();
                                    gunShootSound.volume = 0.5f;
                                    gunShootSound.Play();
                                    // Decrease Ammo
                                    currentAmmo--;
                                    // Muzzle Flash
                                    GameObject particle_point = GameObject.Find("Pistol/ironSights/FirePoint"); // Find the spawn position
                                    GameObject flashMuzzle = Instantiate(muzzleFlash, particle_point.transform.position, Quaternion.identity) as GameObject;
                                    Destroy(flashMuzzle, .5f);
                                    // Impact Effect
                                    GameObject impactHole = Instantiate(bulletHole, Hit.point, Quaternion.FromToRotation(Vector3.forward, Hit.normal)) as GameObject;
                                    Destroy(impactHole, 5f);

                                    // if we hit any gameObject in the scene
                                    if (Hit.collider.gameObject)
                                    {
                                        // the parent of they object becomes the hit point
                                        impactHole.gameObject.transform.parent = Hit.transform;
                                    }

                                    // We want to hit the AI Body and Head to take damage (Could be changed later for more damage when hitting head enemyHit.ApplyDamage(damage * 2);)
                                    if (Hit.collider.gameObject.layer == 11 | Hit.collider.gameObject.layer == 14)
                                    {
                                        GameObject HitMark = Instantiate(hitMarker, Hit.point, Quaternion.FromToRotation(Vector3.forward, Hit.normal)) as GameObject;
                                        HitMark.transform.parent = Hit.transform;
                                        Destroy(HitMark, .2f);
                                        if (Hit.collider.name == "Head")
                                        {
                                            enemyHit = Hit.collider.gameObject.GetComponentInParent<AI>();
                                        }
                                        else
                                        {
                                            enemyHit = Hit.collider.gameObject.GetComponent<AI>();
                                        }

                                        if (Hit.collider.gameObject.layer == 14)
                                        {
                                            enemyHit.headShot = true;
                                        }
                                        else if (Hit.collider.gameObject.layer != 14)
                                        {
                                            enemyHit.headShot = false;
                                        }

                                        // Hurt the AI we hit
                                        if (enemyHit != null)
                                        {
                                            impactHole.transform.parent = Hit.transform;
                                            enemyHit.ApplyDamage(damage);
                                        }
                                    }
                                    else
                                        enemyHit = null;

                                    // if we hit the trigger that belongs to the bridge 
                                    if (Hit.collider.gameObject.layer == 17)
                                    {
                                        if (bridge == null)
                                        {
                                            bridge = GameObject.Find("Bridge");
                                        }
                                        // Play the animation for the bridge to appear
                                        bridge.GetComponent<Animator>().SetBool("Activate", true);
                                    }

                                    if (CurrentBulletType == BulletType.Explosive)
                                    {
                                        Vector3 explosionPosition = Hit.transform.position;
                                        Collider[] objectsHit = Physics.OverlapSphere(explosionPosition, explosionRadius);
                                        foreach (Collider objectsInRange in objectsHit)
                                        {
                                            Rigidbody otherObjectPhysics = objectsInRange.GetComponent<Rigidbody>();
                                            if (otherObjectPhysics != null)
                                                otherObjectPhysics.AddExplosionForce(explosiveForce, explosionPosition, explosionRadius, upforce, ForceMode.Impulse);
                                        }
                                    }
                                    #region Debugging Shooting
                                    Debug.Log("Hit" + Hit.transform.name);  // Show on console what we hit
                                    Debug.DrawRay(cam_FirePosition.transform.position, cam_FirePosition.transform.forward * Hit.distance, Color.red);
                                    #endregion
                                }
                            }
                        }
                    }
                    else
                        return;
                }
            }
        }
        // Melle attack
        public void Punch()
        {
            // if the player presses the key P then we play the animation and we must be punching
            if (Input.GetKey(KeyCode.F))
            {
                isPunching = true;
                // Play the animation
                //Animator anim = GameObject.Find("Fist").GetComponent<Animator>();
                punchController.SetBool("isPunching", true);
            }
            else    // else if the player didnt press p then we are not punching
            {
                isPunching = false;    // cant punch 
                // dont play animations
                punchController.SetBool("isPunching", false);
            }
        }

        // Normal Reloading
        IEnumerator Reload()
        {
            // if we are at max ammo 
            if (currentAmmo >= maxAmmo)
                yield break;    // we cant reload
            else
            {
                audioComponent.clip = reloadSound;
                // cant shoot if we are reloading 
                isShooting = false;
                // We are now Reloading 
                isReloading = true;
                // Play reload anim
                gunAnimator.SetBool("isReloading", true);
                // we need to wait for the sound to kick in so the audio is synced
                yield return new WaitForSeconds(0.25f);
                audioComponent.Play();
                // Wait a few seconds
                yield return new WaitForSeconds(reloadTime - .25f);
                // We are no longer animating the gun to reload
                gunAnimator.SetBool("isReloading", false);
                // Wait more 
                yield return new WaitForSeconds(.25f);
                // We are no longer reloading
                isReloading = false;
                // var shot is a variable in which takes into considering the max ammo and current 
                // this will be to make sure we are relevant ammo in the magazin or how much we do have in the magazine
                var shot = maxAmmo - currentAmmo;
                // if the current backup ammo is less than shot
                if (backUpAmmo < shot)
                {
                    currentAmmo = backUpAmmo;
                    backUpAmmo = 0;
                }
                else    // the current ammo will take values from the backup to reload
                {
                    currentAmmo += shot;
                    backUpAmmo -= shot;
                }
                // Updating UI For Ammo
                currentAmmoText.text = currentAmmo.ToString();
                backUpAmmoText.text = backUpAmmo.ToString();
            }

        }
    }
    #endregion

}