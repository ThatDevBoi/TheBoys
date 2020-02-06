using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class Panel_Fade : MonoBehaviour
{
    private GameObject player;
    private TextMeshPro storeText;
    private float stoppedMoving;
    public bool dialouge;
    private Color32 fade=Color.clear;
  // private bool reading=false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PC");
    }

    // Update is called once per frame
    void Update()
    {

        
            
            
                
           
         if (player.GetComponent<Player_Controller>().playerPhysics.velocity != Vector3.zero|| Input.GetButtonDown("Fire1"))// When the players Rigidbody Velocity is not moving
            {
         GetComponent<CanvasRenderer>().SetAlpha(0);
            GetComponentInChildren<TextMeshPro>().faceColor = new Color32(1, 1, 1, 0);//hide the text panel
                                                                                      //storeText.text= GetComponentInChildren<TextMeshPro>().text;
            stoppedMoving = Time.time;
        }
        else  if(Time.time- stoppedMoving>=5)
            {
            
            //timer += Time.deltaTime / 10;
            fade = Color32.Lerp(fade, Color.white, Time.time / 100);
            GetComponent<CanvasRenderer>().SetAlpha(fade.a);
            GetComponentInChildren<TextMeshPro>().faceColor = fade;
        }




}
    
   
}
