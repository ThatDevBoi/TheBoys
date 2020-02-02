using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shooting_Mechanic : Player_Controller.GunMechanic
{
    private int tick = 0;
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
        if(tick == 0)
        {
            if (GameManager.gunOverride == true)
            {
                tick += 1;
                transform.localRotation = new Quaternion(gameObject.transform.rotation.x, -90, 30, 0);
            }
            else
            {
                tick = 0;
            }
        }

        base.Update();
    }


}
