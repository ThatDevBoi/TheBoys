using System.Collections;
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
    // value in which allows the timer to reset for cooldown timer needs to meet this value
    public float time_Between_Inputs = 3;


    // Private Variables
    // int that scrolls through the array for conversation
    private int conversationScroller = 0;
    // string which shows the UI the next text in the array to print
    private string Conversation;
    // Script to still use type writer effect
    public UITypeWritereffect typeWriterScript;
    // Transform component for the player position so we can make a distance
    public Transform playerObject;
    // Boolean which monitors if the player is talking to the object
    private bool isTalking = false;
    private bool playerApprrachedMe = false;
    // TextMeshPro component UI
    public TextMeshPro projectTextObject;
    // timer in which cools down when the player can talk
    private float keyTimer = 0.0f;
    // bool which allows timer to start ticking "keyTimer"
    bool keypressed = false;
    string startSring= "Press C";

    // Start is called before the first frame update
    void Start()
    {
        #region Find Variables Loop
        StartCoroutine(loopFind());
        #endregion

        #region Build Collider
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = boxCollider_Size;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // if the player is being spawned by the Level Manager
        if(playerObject == null)
        {
            // IDE Transform belonging to the player
            playerObject = GameObject.Find("PC").GetComponent<Transform>();
        }

        // input cooldown
        if (keypressed)
        {
            keyTimer += Time.deltaTime;
            if (keyTimer >= time_Between_Inputs)
            {
                keyTimer = 0;
                keypressed = false;
            }
        }
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
                projectTextObject.text = startSring + " ";
                // boolean ticks 
                isTalking = true;
            }
            else if (Vector3.Distance(transform.position, playerObject.position) > distanceToTalk)
            {
                projectTextObject.text = startSring + " " ;
                isTalking = false;
             
            }
            // when boolean is true 
            if (isTalking)
            {
                if (!keypressed)
                {
                    startSring = "";
                    // Conversation starts
                    Conversation = objectConversation[conversationScroller];
                    if (keyTimer >= 0)
                    {
                        // when key is pressed 
                        if (Input.GetKeyDown(KeyPressToTalk))
                        {
                            keypressed = true;
                            if (conversationScroller == 1)
                                projectTextObject.text = " ";

                            // increase array scroller by 1
                            conversationScroller++;
                            // call function to type write
                            typeWriterScript.ChangeText(Conversation, 0);
                        }
                    }
                    else
                        keypressed = false;
                }
                else
                {
                    startSring = "Press C";
                    return;
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
            startSring = "Press C";
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


    IEnumerator loopFind()
    {
        while(projectTextObject == null && typeWriterScript == null)
        {
            // Find the text
            projectTextObject = GameObject.Find("PlayerUIController/Panel/Exposition_Text").GetComponent<TextMeshPro>();
            // Find the typewriter
            typeWriterScript = GameObject.Find("PlayerUIController/Panel/Exposition_Text").GetComponent<UITypeWritereffect>();
            yield break;
        }
        
    }

}
