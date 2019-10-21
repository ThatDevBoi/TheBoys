using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_StarFish_Animation : MonoBehaviour
{
    int animationChanger = 0;
    public float nextAnim = 6f;
    public Animator animatorController;
    // Start is called before the first frame update
    void Start()
    {
        nextAnim = 6f;
        animatorController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Decline Value
        nextAnim -= Time.deltaTime;
        // Value is 0 or more
        if(nextAnim <= 0)
        {
            // Randomise the int set the new anim
            RandomiseAnim();
            // Reset the timer
            nextAnim = 6f;
        }
    }

    void RandomiseAnim()
    {
        // Random Number Generation
        animationChanger = Random.Range(0, 2);
        // Random value takes control of animator controller paramater
        animatorController.SetInteger("animationChoice", animationChanger);
    }
}
