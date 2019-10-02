using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Manager : MonoBehaviour
{

    public Canvas StartMenu;

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
        
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        StartMenu.enabled = false;
    }


}