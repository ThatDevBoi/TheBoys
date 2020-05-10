using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    #region Level Manager Variables
    [Header("Player and AI")]
    public GameObject[] Obsticles;  // the environment of the scene
    public GameObject[] AI;     // The AI in the current scene
    public GameObject Player_Character; // The player character that needs to spawn or be found
    public int totalNPC;
    private int currentNPC=0;
    private int updateNPC;
    private bool npcCount=false;
    [Header("User Interface")]
    public GameObject PauseUI;  // UI element for pausing the game
    public GameObject restartUI;    // Restart UI spawns on player death
    public GameObject PCUI_Controller;  // The UI the player gets visual feedback from
    public GameObject[] textMesh_AI_UI;     // Find damage Text value
    private float timercooldown = 1f;   // Value that helps textMesh_AI_UI to be found during runtime

    [Header("Finding Walls")]
    // All the Transform walls (applied manually)   --// Find solution for this later \\--
    public Transform[] wall_tranArray;
    // all the walls in the scene
    private GameObject[] wall_goArray;
    // List in which holds all the GameObjects that are walls
    public List<GameObject> goList = new List<GameObject>();
    // the wall that is nearest to the player
    private Transform nearestWall;

    // Overrides the guns position and stops anything to do with the gun
    // This involves shooting, recoil, aiming and sway
    public static bool gunOverride = false;
    #endregion

    #region Gun Upgrade Control
    [Header("Gun Upgrade Controller")]
    // bools to maintain gun
    public bool explosiveAmmo = false;
    [HideInInspector]
    public string explosiveUpgradeName = "";
    public bool fullAuto = false;
    [HideInInspector]
    public string fullAutoUpgradeName = "";
    public bool burstFire = false;
    [HideInInspector]
    public string burstFireUpgradeName = "";
    public bool singleFire = true;
    #endregion



    #region Ultamate Control
    [Header("Ultimate Control Values")]
    public int Points_until_Ult;    // need 10 points to achieve ultimate
    public float ult_Lifetime_Time;
    public float time_between_cooldown;
    [Range(90, 120)]
    public float ult_CameraRotation;
    [Space(20)]
    [Range(800, 1000)]
    public float ult_Speed;
    private float ult_Lifetime;
    private float ult_cooldown = 0;    // plays after ultimate is used so its not spammed
    public static bool ult_initiated = false;
    private bool coolDownUlt = false;
    [HideInInspector]
    public bool ultReady = false;
    private Slider ultraSlider;



    public static bool disableInputs = false;
    // Add UI later

    #endregion
    private void Awake()
    {
        // Start game correctly
        InitiatGame();
    }

    // the player in the scene
    public GameObject PC;
    // Find GameObjects in assets
    void InitiatGame()
    {
        // find all the walls
        FindObjectsWithLayer(16);
        #region Set Values
        ult_Lifetime = ult_Lifetime_Time;
        ult_cooldown = 0;
        #endregion

        #region Player Set Up
        /// Find the Player
        if (GameObject.Find("PC") == null)
        {
            // Find the player character
            Player_Character = Resources.Load<GameObject>("Prefabs_prefs/Player_PCpref/Player_Character/PC");
            // start position
            Vector3 spawnPos = new Vector3(-19.3f, -2.35f, 95);
            // spawn player at start pos
            PC = Instantiate(Player_Character, spawnPos, Player_Character.transform.rotation) as GameObject;
            // name the PC
            PC.name = "PC";
            // set the layer
            PC.gameObject.layer = 10;
        }
        else
        {
            // player is already in the scene
            PC = GameObject.Find("PC");
        }
        #endregion
        #region Find Player Camera
        if (PC == null)
        {
            // Find the player camera
            Transform playercam_Transform;
            Camera playerCamera;
            // Find the camera in children of player
            playercam_Transform = PC.transform.GetChild(0);
            playerCamera = playercam_Transform.GetComponent<Camera>();
            // tag player
            playerCamera.tag = "MainCamera";
        }
        else
        {
            Transform playerCam;
            Camera playerCamera;
            playerCam = PC.transform.GetChild(0);
            playerCamera = playerCam.GetComponent<Camera>();
            playerCamera.tag = "MainCamera";
        }

        #endregion
        #region Pause and Restart Set Up
        if (GameObject.Find("Pause_Canvas") == null)
        {
            #region Create Pause
            // find the canvas in the assets
            PauseUI = Resources.Load<GameObject>("Prefabs_prefs/UI_uipref/RuntimeUI/Pause_Canvas");
            // make an object instace for pause 
            GameObject pauseUIinstance;
            // all the data from the instance becomes what we spawned from assets
            pauseUIinstance = Instantiate(PauseUI, Vector3.zero, Quaternion.identity) as GameObject;
            // make sure the UI is named correctly
            pauseUIinstance.name = "Pause_Canvas";
            // we get the button manager from the pauseUIObject
            pauseUIinstance.GetComponent<Button_Manager>().player = GameObject.Find("PC");
            #endregion
        }
        else
        {
            // Find the variables already in the scene
            PauseUI = GameObject.Find("Pause_Canvas");
        }
        #region Create Restart
        if (GameObject.Find("RestartCanvas") == null)
        {
            // find the canvas in the assets
            restartUI = Resources.Load<GameObject>("Prefabs_prefs/UI_uipref/RuntimeUI/RestartCanvas");
            // make instace for the prefab to spawn 
            GameObject restartUIinstance;
            // prefab spawn passes through to the instace we made
            restartUIinstance = Instantiate(restartUI, Vector3.zero, Quaternion.identity) as GameObject;
            // make sure the instance object name is correct
            restartUIinstance.name = "RestartCanvas";
            // find the player
            restartUIinstance.GetComponent<Button_Manager>().player = GameObject.Find("PC");
            restartUIinstance.GetComponent<Button_Manager>().playerLogic = PC.GetComponent<Player_Controller>();
        }
        else
        {
            restartUI = GameObject.Find("RestartCanvas");
        }
        #endregion
        #endregion
        #region Player UI Controller
        // if there is no UI for the player is the scene on runtime
        if (GameObject.Find("PlayerUIController") == null)
        {
            // Find the UI for the player in the assets
            PCUI_Controller = Resources.Load<GameObject>("Prefabs_prefs/UI_uipref/RuntimeUI/PlayerUIController");
            // make instance
            GameObject playerUI_instance;
            // make canvas instance so we can access GameObject Canvas
            Canvas playerUIcan_instance;
            // Spawn prefab and pass data into instance
            playerUI_instance = Instantiate(PCUI_Controller, Vector3.zero, Quaternion.identity) as GameObject;
            // make name of instace correct
            playerUI_instance.name = "PlayerUIController";
            // Find the canvas attached to the object instace of the UI
            playerUIcan_instance = playerUI_instance.GetComponent<Canvas>();
            // allow the UI to have access to the Camera attached to the player
            // if not the canvas wont display correctly
            playerUIcan_instance.worldCamera = PC.transform.GetChild(0).GetComponent<Camera>();
            // We find the UI in here as its the only time we can access the UI being spawned as a local
            // Find UI Slider For Ultra
            // Find holder
            GameObject ultraSliderHolder = playerUI_instance.transform.GetChild(8).gameObject;
            // slider of that parent
            ultraSlider = ultraSliderHolder.transform.GetChild(0).GetComponent<Slider>();
            ultraSlider.maxValue = ult_Lifetime;
            ultraSlider.value = ultraSlider.maxValue;
        }
        else
        {
            // Find variables
            PCUI_Controller = GameObject.Find("PlayerUIController");
        }
        #endregion
        #region Find all the AI
        // Setting up what objects are where
        // Could be handy later for saving CPU process 
        AI = GameObject.FindGameObjectsWithTag("NPC");
        

        foreach (GameObject NPC in AI)
        {
            // Set up AI Layer
            NPC.layer = 11;
            // Find the AI Head
            Transform NPCHead;
            NPCHead = NPC.gameObject.transform.GetChild(2);
            // Set Head Layer
            NPCHead.gameObject.layer = 14;
            //set Total number of enemies in the scene DM
            totalNPC++;
        }
        #endregion

        Obsticles = GameObject.FindGameObjectsWithTag("Obstacles");

        foreach (GameObject obsticles in Obsticles)
        {
            obsticles.layer = 12;
        }
        updateNPC = totalNPC;
    }

    // Update is called once per frame
    void Update()
    {
        
        // call this to balance the games frames
        balanceGame();
        // find the texts
        timercooldown -= Time.deltaTime;
        GetClosestWall(wall_tranArray);
        if (nearestWall == null)
            Debug.Log("wall not found");
        if (Vector3.Distance(nearestWall.position, PC.transform.position) < 4)
        {
            //// make the player hold their gun upward
            //Debug.Log("Yield Back Gun");
            gunOverride = true;
        }
        else
        {
            //// make the player hold their gun normal
            //Debug.Log("Normal Gun Hold");
            gunOverride = false;
        }

        #region Ultimate Controller
        if (Points_until_Ult == 10)
        {
            Debug.Log("Alt Ready");
            ultReady = true;
        }

        // when we can ult
        if(ultReady)
        {
            ultraSlider.value = ult_Lifetime;
            // when ult is pressed
            if (ult_initiated && !coolDownUlt)
            {
                Time.timeScale = .4f;
                // Value which smooths timeScale
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                PC.GetComponent<Player_Controller>().speed = ult_Speed * Time.time * 300;
                PC.GetComponent<Player_Controller>().cameraRotationRate = ult_CameraRotation;
                Debug.Log("Alt Time");
                // reduce timer how long we are allowed to ult
                ult_Lifetime -= Time.deltaTime;
                if (ult_Lifetime <= 0)
                {
                    Time.timeScale = 1;
                    ult_initiated = false;
                    ultReady = false;
                    coolDownUlt = true;
                    ult_Lifetime = 5;
                    Points_until_Ult = 0;
                }
            }
        }

        if (coolDownUlt && !ult_initiated)
        {
            PC.GetComponent<Player_Controller>().speed = PC.GetComponent<Player_Controller>().currentSpeed;
            PC.GetComponent<Player_Controller>().cameraRotationRate = 45; //UI_Manager.playersSensertivity;
            ultraSlider.value = ult_cooldown;
            ultraSlider.maxValue = time_between_cooldown;
            ult_cooldown += Time.deltaTime;
            if (ult_cooldown >= time_between_cooldown)
            {
                coolDownUlt = false;
                ult_cooldown = 0;
                ultraSlider.maxValue = ult_Lifetime;
                ultraSlider.value = ultraSlider.maxValue;
            }
        }

        #endregion

        #region Enemy Counter
        if (totalNPC >=1)
        {
            if (updateNPC >= 1)
                enemyCounter();
            else if (!npcCount)
            {
                npcCount = true;
                GameObject.Find("PlayerUIController/Number").GetComponent<TextMeshProUGUI>().text = "";
                GameObject.Find("PlayerUIController/Objective").GetComponent<TextMeshPro>().text = "Get the update";
            }
        }
        else
        {
            GameObject.Find("PlayerUIController/Number").GetComponent<TextMeshProUGUI>().text = "";
        }
        #endregion
    }
    #region Enemy Counter()
    void enemyCounter()
    {

       
        var enemyList = GameObject.FindGameObjectsWithTag("NPC");
       
        foreach (GameObject NPC in enemyList)
        {
            currentNPC++;
        }

        if (currentNPC<updateNPC )
        {
            updateNPC = currentNPC;
            currentNPC = 0;
        }
        else
            currentNPC = 0;
        GameObject.Find("PlayerUIController/Number").GetComponent<TextMeshProUGUI>().text = ("Remanining " + updateNPC +"/" + (totalNPC-1));//Show in the UI how many enemies are left DM
       
    }
    #endregion
    void balanceGame()
    {
        // if value is 0 or more
        if (timercooldown <= 0)
        {
            // if there is an object with the relevent tag in the scene
            if (GameObject.FindGameObjectWithTag("AI_UI") != null)
            {
                // add to array
                textMesh_AI_UI = GameObject.FindGameObjectsWithTag("AI_UI");
                timercooldown = 1;  // reset timer
            }
        }


        // Disable Input call
        if (Time.timeScale <= 0)
            disableInputs = true;
        else
            disableInputs = false;



        // find all the current objects of text damage
        foreach (GameObject go in textMesh_AI_UI)
        {
            // destroy text object
            Destroy(go, 2);
        }
    }

    /// <summary>
    /// Finds all objects of a certain layer type
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    GameObject[] FindObjectsWithLayer(int layer)
    {
        wall_goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < wall_goArray.Length; i++)
        {
            if (wall_goArray[i].layer == layer)
            {
                goList.Add(wall_goArray[i]);
            }
        }

        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }
    /// <summary>
    /// Tranform in which gets the players closest wall near to them
    /// </summary>
    /// <param name="walls"></param>
    /// <returns></returns>
    Transform GetClosestWall(Transform[] walls)
    {
        nearestWall = null;
        float mindist = Mathf.Infinity;
        Vector3 currentPosition = PC.transform.position;
        foreach (Transform t in walls)
        {
            float dist = Vector3.Distance(t.position, currentPosition);
            if (dist < mindist)
            {
                nearestWall = t;
                mindist = dist;
            }
        }
        return nearestWall;

    }
}
