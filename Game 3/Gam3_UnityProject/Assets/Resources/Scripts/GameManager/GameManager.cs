using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Obsticles;
    public GameObject[] AI;
    public GameObject Player_Character;

    public GameObject PauseUI;
    public GameObject restartUI;
    public GameObject PCUI_Controller;

    private void Awake()
    {
        InitiatGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    // the player in the scene
    GameObject PC;
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
            Player_Character = GameObject.Find("PC");
        }
        #endregion
        #region Find Player Camera
        // Find the player camera
        Transform playercam_Transform;
        Camera playerCamera;
        // Find the camera in children of player
        playercam_Transform = PC.transform.GetChild(0);
        playerCamera = playercam_Transform.GetComponent<Camera>();
        // tag player
        playerCamera.tag = "MainCamera";
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
            playerUIcan_instance.worldCamera = playerCamera;
        }
        else
        {
            // Find variables
            PCUI_Controller = GameObject.Find("PlayerUIController");
        }
        #endregion


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

        Obsticles = GameObject.FindGameObjectsWithTag("Obstacles");

        foreach (GameObject obsticles in Obsticles)
        {
            obsticles.layer = 12;
        }
    }
}
