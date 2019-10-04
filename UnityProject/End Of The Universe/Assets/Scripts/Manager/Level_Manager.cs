using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Level_Manager : MonoBehaviour
{
    // Variables
    // UI
    public Text distanceMeter;
    public Canvas StartMenu;
    // Ints
    int currentDistance = 1;
    // Float
    float playersDistance = 1;
    
    
    

    

    public void Awake()
    {
        // Assign the canvas variable
        StartMenu = GameObject.Find("StartMenu").GetComponent<Canvas>();
      
        Time.timeScale = 0;
        StartMenu.enabled = true;

        // Debug
        if (StartMenu == null)
            Debug.LogWarning("You Have Not Applied The Relevant Canvas");
        else
            return;
    }

    public void Update()
    {
        // When the game is currently not in a play state dont call the function 
        if(Time.timeScale > 0)
        {
            StartCoroutine(DistanceCalulator());
        }
    }

    public void StartGame()
    {
        // Chnage the time so the game will play
        Time.timeScale = 1;
        // Turn off the canvas
        StartMenu.enabled = false;
    }

    public IEnumerator DistanceCalulator()
    {
        playersDistance += Time.deltaTime * currentDistance;
        // Make the float value round up to an int value so it shows in UI as a whole number
        playersDistance = Mathf.RoundToInt(Time.time * currentDistance + 1);
        // Let the UI text update when the value increases
        distanceMeter.text = "Miles: " + playersDistance;
        // Console the value
        Debug.Log(playersDistance);
        yield break;
    }


}