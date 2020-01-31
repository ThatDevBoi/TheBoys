﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    void Update()
    {
        base.Update();
    }


}