using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Level Manager Variables
    /// <summary>
    /// GameObjects that need to be gathered/found
    /// </summary>
    public GameObject[] Obsticles;  // the environment of the scene
    public GameObject[] AI;     // The AI in the current scene
    public GameObject Player_Character; // The player character that needs to spawn or be found
    /// <summary>
    /// UI Variables
    /// </summary>
    public GameObject PauseUI;  // UI element for pausing the game
    public GameObject restartUI;    // Restart UI spawns on player death
    public GameObject PCUI_Controller;  // The UI the player gets visual feedback from
    /// <summary>
    /// Find Objects of layer
    /// </summary>
    [HideInInspector]
    // all the walls in the scene
    private GameObject[] wall_goArray;
    // All the Transform walls (applied manually)   --// Find solution for this later \\--
    public Transform[] wall_tranArray;
    // List in which holds all the GameObjects that are walls
    public List<GameObject> goList = new List<GameObject>();
    [HideInInspector]
    // the wall that is nearest to the player
    private Transform nearestWall;
    /// <summary>
    ///  Static bools ints float passed into scripts for balance or functionality reasons
    /// </summary>
    /// 
    // Balcne Game Variables
    public GameObject[] textMesh_AI_UI;
    public float timercooldown = 1f;

    // Overrides the guns position and stops anything to do with the gun
    // This involves shooting, recoil, aiming and sway
    public static bool gunOverride = false;
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
        #region Player Set Up
        /// Find the Player
        if (GameObject.Find("PC") == null)
        {
            // Find the player character
            Player_Character = Resources.Load<GameObject>("Prefabs/Player/Player Character/PC");
            // start position
            Vector3 spawnPos = new Vector3(-19.3f, -2.35f, 95);
            // spawn player at start pos
            PC = Instantiate(Player_Character, spawnPos, Player_Character.transform.rotation)as GameObject;
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
        if(PC == null)
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
            // we need access to the button manager so the manager can find the Player being spawned into runtime
            Button_Manager variableAccess;
            #region Create Pause
            // find the canvas in the assets
            PauseUI = Resources.Load<GameObject>("Prefabs/UI/RuntimeUI/Pause_Canvas");
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
            restartUI = Resources.Load<GameObject>("Prefabs/UI/RuntimeUI/RestartCanvas");
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
            PCUI_Controller = Resources.Load<GameObject>("Prefabs/UI/RuntimeUI/PlayerUIController");
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
        }
        #endregion

        Obsticles = GameObject.FindGameObjectsWithTag("Obstacles");

        foreach (GameObject obsticles in Obsticles)
        {
            obsticles.layer = 12;
        }
        // find all the walls
        FindObjectsWithLayer(16);
    }

    // Update is called once per frame
    void Update()
    {
        GetClosestWall(wall_tranArray);
        if (nearestWall == null)
            Debug.Log("wall not found");
        if(Vector3.Distance(nearestWall.position, PC.transform.position) < 4)
        {
            // make the player hold their gun upward
            Debug.Log("Yield Back Gun");
            gunOverride = true;
        }
        else
        {
            // make the player hold their gun normal
            Debug.Log("Normal Gun Hold");
            gunOverride = false;
        }
        // call this to balance the games frames
        balanceGame();
    }


    void balanceGame()
    {
        // find the texts
        timercooldown -= Time.deltaTime;
        if (timercooldown <= 0)
        {
            if (GameObject.FindGameObjectsWithTag("AI_UI") == null)
            {
                timercooldown = 1;
                return;
            }
            else
            {
                textMesh_AI_UI = GameObject.FindGameObjectsWithTag("AI_UI");
                timercooldown = 1;
            }
        }

        // find all the current objects of text damage
        foreach(GameObject go in textMesh_AI_UI)
        {
            Destroy(go, 2);
        }
    }
    GameObject[] FindObjectsWithLayer(int layer)
    {
        wall_goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for(int i = 0; i < wall_goArray.Length; i++)
        {
            if (wall_goArray[i].layer == layer)
            {
                goList.Add(wall_goArray[i]);
            }
        }

        if(goList.Count == 0)
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
        foreach(Transform t in walls)
        {
            float dist = Vector3.Distance(t.position, currentPosition);
            if(dist < mindist)
            {
                nearestWall = t;
                mindist = dist;
            }
        }
        return nearestWall;

    }
}
