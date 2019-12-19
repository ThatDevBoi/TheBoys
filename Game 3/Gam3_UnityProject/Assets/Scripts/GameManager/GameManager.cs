using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Obsticles;
    public GameObject[] AI;
    public GameObject Player_Character;


    private void Awake()
    {
        InitiatGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitiatGame()
    {

        Player_Character = GameObject.Find("PC");
        Player_Character.gameObject.layer = 10;
        Transform playerCam;
        playerCam = Player_Character.transform.GetChild(0);
        playerCam.tag = "MainCamera";

        // Setting up what objects are where
        // Could be handy later for saving CPU process 
        AI = GameObject.FindGameObjectsWithTag("NPC");

        foreach(GameObject NPC in AI)
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

        foreach(GameObject obsticles in Obsticles)
        {
            obsticles.layer = 12;
        }
    }
}
