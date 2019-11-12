using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting_Mechanic : Player_Controller.GunMechanic
{
    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        gunShootSound = this.gameObject.GetComponent<AudioSource>();
        gunShootSound.volume = 0.2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        if (Input.GetButtonDown("Fire2"))
            AimDownSights();
    }
}
