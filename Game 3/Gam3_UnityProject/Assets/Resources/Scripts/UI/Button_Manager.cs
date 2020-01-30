using UnityEngine;
using UnityEngine.SceneManagement;
public class Button_Manager : MonoBehaviour
{
    public Camera mainCamera;
    public bool helpNeeded = false;
    public GameObject player;

    // Camera Rotation
    public Vector3 startPositionCam;
    public Transform Help_cameraPosition;
    public Transform mainMenu_Pos;
    public float rotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        startPositionCam = mainCamera.transform.position;

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
        //Time.timeScale = 1;
        #region Set up player values again
        GameObject.Find("PC").GetComponent<Player_Controller>().currentHealth = 100;
        GameObject.Find("PC").GetComponent<Player_Controller>().playerDead = false;
        #endregion
    }

    public void Go_To_HelpScreen()
    {
        helpNeeded = true;

        if(SceneManager.sceneCount != 0)
        {
            return;
        }
        else
        {
            // Rotate Camera
            mainCamera.transform.position = Vector3.RotateTowards(mainCamera.transform.position, Help_cameraPosition.position, 10, rotSpeed);
        }
    }

    public void Go_To_Main_Menu()
    {
        helpNeeded = false;
        // if the scene is not the main menu scene
        if(SceneManager.sceneCount != 0)
        {
            // do nothing
            return;
        }
        else    // if it is the main menu 
        {
            // rotate camera to new position
            mainCamera.transform.position = Vector3.RotateTowards(mainCamera.transform.position, mainMenu_Pos.position, 10, rotSpeed);
        }
    }

    public void resumeGame()
    {
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
