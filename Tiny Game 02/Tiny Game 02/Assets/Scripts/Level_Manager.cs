using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_Manager : MonoBehaviour
{
    // Variables
    public Canvas StartMenu;
    public Canvas pauseMenu;
    public Canvas endMenu;
    public GameObject character;
    public GameObject whale;

    public Text uiScore;
    public int trashScore = 0;
   
    public void Awake()
    {

        StartMenu = GameObject.Find("StartMenu").GetComponent<Canvas>();    // Find the start canvas in the scene
        pauseMenu = GameObject.Find("PauseMenu").GetComponent<Canvas>();    // Find the Pause menu canvas in the scene
        endMenu = GameObject.Find("EndMenu").GetComponent<Canvas>();    // Find the end screen in the scene
        whale.SetActive(false);
        character = GameObject.Find("Player");  // Find the player in the scene
        character.GetComponent<Movement>().enabled = false;
        uiScore = GameObject.Find("Game UI/Score").GetComponent<Text>();
        pauseMenu.enabled = false;  // Turn off the pause menu as the game starts
        StartMenu.enabled = true;   // Turn on the start menu
        endMenu.enabled = false;    // Turn off the end screen

        // Debug
        if (StartMenu == null)
            Debug.LogWarning("You Have Not Applied The Relevant Canvas");
        else
            return;
    }

    // Update is called once per frame
    private void Update()
    {
        //Fade();

        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseMenu();
        }
        // Change the UI score
        uiScore.text = "Score: " + trashScore;

        if (trashScore >= 10)
        {
            whale.SetActive(true);
            endMenu.enabled = true;
            character.GetComponent<Movement>().enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        character.GetComponent<Movement>().enabled = true;
        // Turn off the canvas
        StartMenu.enabled = false;
    }

    

    public void UnpauseGame()
    {
        character.GetComponent<Movement>().enabled = true;
        // Turn off the canvas
        pauseMenu.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void EndApplication()
    {
        Application.Quit();
    }

    public void PauseMenu()
    {
        character.GetComponent<Movement>().enabled = false;
        // Turn on the canvas
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pauseMenu.enabled = true;
    }

    public void RestartGame(string levelName)
    {
        Application.LoadLevel(levelName);
    }
}
