using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shooting_Mechanic : Player_Controller.GunMechanic
{
    private int tick = 0;
    public Vector3 startPosition;
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        //startPosition = gameObject.transform.localPosition;
        gunShootSound = this.gameObject.GetComponent<AudioSource>();
        gunShootSound.volume = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        // Make player gun come backwards when near wall
        if (GameManager.gunOverride == true)
        {
            // the weapon holder positions goes backwards so the gun does not give player
            // ability to climb objects.
            weaponHolder.localPosition = new Vector3(0, -1, -3);
        }

        base.Update();
    }
}
