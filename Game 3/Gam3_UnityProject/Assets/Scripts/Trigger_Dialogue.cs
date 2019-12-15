﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// This script works with multiple objects
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class Trigger_Dialogue : MonoBehaviour
{
    // Public Variables
    // Array of strings used for our conversation
    public string[] objectConversation;
    // Keycode can be changed in the inspector
    // The key is used to talk
    public KeyCode KeyPressToTalk;
    // value which can change and monitor how far the player needs to be until talk can be exacuted
    public float distanceToTalk = 3;
    public Vector3 boxCollider_Size = new Vector3(5, 1, 5);


    // Private Variables
    // int that scrolls through the array for conversation
    private int conversationScroller = 0;
    // string which shows the UI the next text in the array to print
    private string Conversation;
    // Script to still use type writer effect
    private UITypeWritereffect typeWriterScript;
    // Transform component for the player position so we can make a distance
    private Transform playerObject;
    // Boolean which monitors if the player is talking to the object
    private bool isTalking = false;
    private bool playerApprrachedMe = false;
    // TextMeshPro component UI
    private TextMeshPro projectTextObject;


    // Start is called before the first frame update
    void Start()
    {
        // Find the text
        projectTextObject = GameObject.Find("PlayerUIController/Panel/Exposition_Text").GetComponent<TextMeshPro>();
        // Find the typewriter
        typeWriterScript = GameObject.Find("PlayerUIController/Panel/Exposition_Text").GetComponent<UITypeWritereffect>();
        // IDE Transform belonging to the player
        playerObject = GameObject.Find("PC").GetComponent<Transform>();
        #region Build Collider
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = boxCollider_Size;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Conversation Logic
        // if the conversation is over
        if (conversationScroller >= objectConversation.Length)
        {
            // reset 
            conversationScroller = 0;
            // tick boolean back
            isTalking = false;
        }

        if(playerApprrachedMe)
        {
            // when in range
            if (Vector3.Distance(transform.position, playerObject.position) < distanceToTalk && !isTalking)
            {
                // text shows
                projectTextObject.text = "Press" + ":" + KeyPressToTalk;
                // boolean ticks 
                isTalking = true;
            }
            else if (Vector3.Distance(transform.position, playerObject.position) > distanceToTalk)
            {
                isTalking = false;
            }
            // when boolean is true 
            if (isTalking)
            {
                // Conversation starts
                Conversation = objectConversation[conversationScroller];

                // when key is pressed 
                if (Input.GetKeyDown(KeyPressToTalk))
                {
                    // increase array scroller by 1
                    conversationScroller++;
                    // call function to type write
                    typeWriterScript.ChangeText(Conversation, 0);
                }
            }
        }
        #endregion
    }
    #region Multiple Object Controller
    // Because we are using multiple objects We need a boolean to address which object we are at
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PC")
        {
            playerApprrachedMe = true;
        }
    }
    // We end the conversation with the object
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PC")
        {
            playerApprrachedMe = false;
            // set the conversation to be shown as over
            Conversation = "Conversation Over";
            typeWriterScript.StopCoroutine(typeWriterScript.PlayText());
            // reset the array scroller
            conversationScroller = 0;
            // TextMesh needs to know what to print 
            projectTextObject.text = Conversation;
        }
    }
    #endregion

}