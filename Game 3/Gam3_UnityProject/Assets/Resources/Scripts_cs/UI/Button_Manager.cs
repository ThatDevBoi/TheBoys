using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Button_Manager : MonoBehaviour
{
    public Camera mainCamera;
    public bool helpNeeded = false;
    public GameObject player;
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject pausePanel;

    // Camera Rotation
    public Vector3 startPositionCam;
    public Transform Help_cameraPosition;
    public Transform mainMenu_Pos;
    public float rotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        optionsPanel = GameObject.Find("Options_Canvas");
        mainCamera = Camera.main;

        startPositionCam = mainCamera.transform.position;
        helpNeeded = false;
        #region Name Check
        if (this.gameObject.name == "Pause_Canvas")
            gameObject.name = "Pause_Canvas";

        

        #endregion
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(helpNeeded)
        {
            Go_To_HelpScreen();
        }
        else
        {
            Go_To_Main_Menu();
        }
        

    }
    // starts the game
    public void StartGame(string levelname)
    {
        SceneManager.LoadScene(levelname);
        
    }
    [HideInInspector]
    public Player_Controller playerLogic;
    public void Respawn()
    {
        #region Set up player values again
        GameObject.Find("PC").GetComponent<Player_Controller>().currentHealth = 100;
        GameObject.Find("PC").GetComponent<Player_Controller>().playerDead = false;

        // 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        #endregion
    }

    public void Go_To_HelpScreen()
    {
        helpNeeded = true;
        // Rotate Camera
        if (this.gameObject.name == "MainMenu")
        {
            if (SceneManager.GetSceneByName("StartMenu") != null)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, Help_cameraPosition.position, Time.deltaTime);

                mainPanel.SetActive(false);
                optionsPanel.SetActive(true);
            }
            else
                return;
        }

    }

    public void Go_To_Main_Menu()
    {
        helpNeeded = false;
        // rotate camera to new position
        if (this.gameObject.name == "MainMenu")
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, mainMenu_Pos.position, Time.deltaTime);
        //// if the scene is not the main menu scene
        //if (SceneManager.sceneCount != 0)
        //{
        //    // do nothing
        //    return;
        //}
        //else    // if it is the main menu 
        //{

        //}
       
            mainPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    public void resumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (this.gameObject.name == "Pause_Canvas")
        {
            this.gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

   

}
