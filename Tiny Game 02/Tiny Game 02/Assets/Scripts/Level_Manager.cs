using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_Manager : MonoBehaviour
{
    // Variables
    public Canvas StartMenu;
    public Canvas pauseMenu;
   
    public void Awake()
    {
        StartMenu = GameObject.Find("StartMenu").GetComponent<Canvas>();    // Find the start canvas in the scene
        pauseMenu = GameObject.Find("PauseMenu").GetComponent<Canvas>();    // Find the Pause menu canvas in the scene

        Time.timeScale = 0; // Pause the game while start menu is on screen
        pauseMenu.enabled = false;  // Turn off the pause menu as the game starts
        StartMenu.enabled = true;   // Turn on the start menu

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
        // Out of bounds check will go here
    }

    public void StartGame()
    {
        // Change the time so the game will play
        Time.timeScale = 1;
        // Turn off the canvas
        StartMenu.enabled = false;
    }

    public void RestartGame(string levelName)
    {

        //Reloads the scene
        Application.LoadLevel(levelName);   // For use with out of bounds
        
    }

    public void UnpauseGame()
    {
        // Change the time so the game will play
        Time.timeScale = 1;
        // Turn off the canvas
        pauseMenu.enabled = false;
    }

    public void EndApplication()
    {
        EndApplication();
    }

    public void PauseMenu()
    {
        // Change the time so the game will pause
        Time.timeScale = 0;
        // Turn on the canvas
        pauseMenu.enabled = true;
    }

}
