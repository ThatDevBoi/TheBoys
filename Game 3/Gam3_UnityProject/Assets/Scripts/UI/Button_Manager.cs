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

    public void Respawn()
    {
        Time.timeScale = 1;
        player.GetComponent<Player_Controller>().currentHealth = 100;
        player.GetComponent<Player_Controller>().playerDead = false;
    }

    public void Go_To_HelpScreen()
    {
        helpNeeded = true;
        // Rotate Camera
        mainCamera.transform.position = Vector3.RotateTowards(mainCamera.transform.position, Help_cameraPosition.position, 10, rotSpeed);
    }

    public void Go_To_Main_Menu()
    {
        helpNeeded = false;
        // rotate camera to new position
        mainCamera.transform.position = Vector3.RotateTowards(mainCamera.transform.position, mainMenu_Pos.position, 10, rotSpeed);
    }

    void Fade()
    { 

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
