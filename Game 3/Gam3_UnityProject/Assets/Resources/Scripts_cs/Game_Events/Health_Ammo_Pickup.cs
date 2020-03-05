using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Ammo_Pickup : MonoBehaviour
{
    // The type of pickup this object is
    public bool ammoPickup_, healthPickup_, bothactive;
    [HideAttributes("healthPickup_", true)]
    public int healthGain;
    [HideAttributes("ammoPickup_", true)]
    public int ammo_gain;

    // Start is called before the first frame update
    void Start()
    {
        // check if boxcollider is attached
        BoxCollider currentCollider = gameObject.GetComponent<BoxCollider>();
        if (currentCollider == null)    // if not attched
            currentCollider = gameObject.AddComponent<BoxCollider>();   // make one
        // if the current collider attahced is not set to a trigger 
        if (currentCollider != currentCollider.isTrigger)
            currentCollider.isTrigger = true;   // make a trigger
        #region Checks
        // Check what bools have been pressed correctly 
        // If not we stop the game and say why
        if (ammoPickup_ && healthPickup_ && bothactive)
            return;
        else if (!ammoPickup_ && !healthPickup_)
        {
            Debug.LogError("Select a boolean from" + ":" + gameObject.GetComponent<Health_Ammo_Pickup>().name +
        "If not the script wont work");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else if(ammoPickup_ && healthPickup_)
        {
            Debug.LogError("Select a boolean from" + ":" + gameObject.GetComponent<Health_Ammo_Pickup>().name +
"If not the script wont work");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PC")
        {
            // find data scripts needed
            Player_Controller pc_health = other.gameObject.GetComponent<Player_Controller>();
            Shooting_Mechanic shootingAmmo = other.gameObject.transform.GetComponentInChildren<Shooting_Mechanic>();
            if(healthPickup_)
            {
                if(pc_health != null)
                {
                    if (pc_health.currentHealth > pc_health.maxHealth)
                        return;
                    else
                    {
                        pc_health.currentHealth += healthGain;
                    }
                }
            }
            else if(ammoPickup_)
            {
                if(shootingAmmo != null)
                {
                    if (shootingAmmo.backUpAmmo > shootingAmmo.maxBackupAmmo)
                        return;
                    else
                    {
                        /////////
                        shootingAmmo.backUpAmmo += ammo_gain;
                    }
                }
            }
        }
    }
}
