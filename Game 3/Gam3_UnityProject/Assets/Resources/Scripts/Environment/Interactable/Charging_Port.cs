using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charging_Port : MonoBehaviour
{
    private GameObject player;
    public Player_Controller playerClass;
    public Shooting_Mechanic PlayerShootingClass;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "Charging Port";
        player = GameObject.Find("PC");
        playerClass = player.GetComponent<Player_Controller>();
        PlayerShootingClass = player.GetComponentInChildren<Shooting_Mechanic>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Stop Player On Charging Port
    private void OnTriggerStay(Collider other)
    {
        // if the player is on the trigger
        if (other.gameObject.name == "PC")
        {
            // if the player Health OR Ammo is not at max
            if (playerClass.currentHealth < playerClass.maxHealth | PlayerShootingClass.backUpAmmo < PlayerShootingClass.maxBackupAmmo)
            {
                playerClass.speed = 0;  // stop the player
            }
            // if not then the player has max health and then we can move
            else if (playerClass.currentHealth == playerClass.maxHealth | PlayerShootingClass.backUpAmmo == PlayerShootingClass.maxBackupAmmo)
            {
                playerClass.speed = playerClass.currentSpeed;
            }
        }
    }
    #endregion
}
