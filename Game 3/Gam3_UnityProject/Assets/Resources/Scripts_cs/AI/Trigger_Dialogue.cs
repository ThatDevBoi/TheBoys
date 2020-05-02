using System.Collections;
using TMPro;
using UnityEngine;
/// <summary>
/// This script works with multiple objects
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class Trigger_Dialogue : MonoBehaviour
{
    // Public Variables
    // Array of strings used for our conversation
    public bool using_VoiceActing = false;
    public string[] objectConversation;
    // Keycode can be changed in the inspector
    // The key is used to talk
    //public KeyCode KeyPressToTalk;
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
    public bool PC_Aproached_Me = false;
    // TextMeshPro component UI
    public TextMeshPro projectTextObject;
    // timer in which cools down when the player can talk
    private float keyTimer = 0.0f;
    //store player current speed
    private float speed;
    private GameObject player;
    // bool which allows timer to start ticking "keyTimer"
    bool keypressed = false;
    string startSring= "Press E";
    public bool StoryEvent = false;
    bool DoOnce = false;
    bool read = false;
    private GameObject Log;
    // Start is called before the first frame update
    void Start()
    {
        #region Find Variables Loop
        StartCoroutine(loopFind());
        Log=GameObject.Find("Log");
        player = GameObject.Find("PC");
        read = true;
        #endregion

        #region Build Collider
        //BoxCollider box = gameObject.AddComponent<BoxCollider>();
        //box.isTrigger = true;
        //box.size = boxCollider_Size;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // This is a dialogue interactable object if the bool is false
        if (!using_VoiceActing)
        {
            // if the player is being spawned by the Level Manager
            if (playerObject == null)
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
            if (PC_Aproached_Me)
            {
                // when in range
                if (Vector3.Distance(transform.position, playerObject.position) < distanceToTalk && !isTalking)
                {
                    //GameObject.Find("PlayerUIController/Panel/Exposition_Text").GetComponent<TextMeshPro>().text= startSring;
                    // text shows
                    projectTextObject.text = startSring + " ";
                    // boolean ticks 
                    isTalking = true;
                }
                else if (Vector3.Distance(transform.position, playerObject.position) > distanceToTalk)
                {
                    projectTextObject.text = startSring + " ";
                    isTalking = false;

                }

                // when boolean is true 
                if (isTalking && !StoryEvent)
                {
                    if (!keypressed)
                    {
                        startSring = "";
                        // Conversation starts
                        Conversation = objectConversation[conversationScroller];
                        if (keyTimer >= 0)
                        {
                            // when key is pressed 
                            if (Input.GetKeyDown(player.GetComponent<Player_Controller>().Player_Key_Binds[4]))
                            {
                                keypressed = true;
                                //if (conversationScroller == 1)
                                //    projectTextObject.text = " ";

                                // increase array scroller by 1
                                conversationScroller++;
                                // call function to type write
                                typeWriterScript.ChangeText(Conversation, 0.5f);
                                player.GetComponent<Player_Controller>().speed = 0;
                            }
                        }
                        else
                            keypressed = false;
                    }
                    else
                    {
                        startSring = "Press E";
                        return;
                    }
                }
            }
        }
        #endregion
        else    // the voice acting starts here 
        {
            if (isTalking == false)
            {
                StartVoiceAct();
            }

        }
    }

    public AudioClip voiceAct;  // audio clip that needs to be played when key is pressed
    private AudioSource _AS;
    public float currentduration = 0.0f;
    AudioSource pcAudio;
    public int switcher = 0;
    public void StartVoiceAct()
    {
        // component collection
        if (_AS == null)
        {
            // add Audio Source
            _AS = gameObject.GetComponent<AudioSource>();
            _AS.playOnAwake = false;
            _AS.loop = false;
            // add clip
            _AS.clip = voiceAct;
        }
        if (switcher == 0 && pcAudio.isPlaying == false)  // When in range of 3D sound
        {
            _AS.Play(); // play the clip
            // Check to see where the clip duration is 
            // Make that check a value so we can pass it on if needed
            currentduration = _AS.time;
        }
        else if (switcher == 1 && _AS.isPlaying == true)
        {
            Debug.LogWarning("PC Takes Over Now");
            // component collection
            if (player.transform.GetChild(0).GetComponent<AudioSource>() == null && _AS.isPlaying == true)
            {
                // Add component 
                pcAudio = player.transform.GetChild(0).gameObject.AddComponent<AudioSource>();
                if (pcAudio != null)
                {
                    pcAudio.clip = voiceAct;    // apply clip
                    pcAudio.playOnAwake = true; // play as soon as possible
                    pcAudio.time = _AS.time;    // play at the current time 

                    _AS.Stop(); // Stop the 3D sound
                    pcAudio.Play(); // Play the 2D sound 
                }
            }
        }

        if(pcAudio == null)
        {
            return;
        }
        else
        {
            if (pcAudio != null && pcAudio.isPlaying == false)
            {
                pcAudio.Stop();
                DestroyImmediate(pcAudio);
                pcAudio = null;
                switcher = 0;
            }
            else if (_AS.isPlaying == false)
            {
                _AS.Stop();
                switcher = 0;
            }
        }
    }

    #region Multiple Object Controller
    // Because we are using multiple objects We need a boolean to address which object we are at
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PC")
        {
            isTalking = true;
            switcher = 0;
            PC_Aproached_Me = true;

            //GameObject.Find("PlayerUIController/Panel").GetComponent<Panel_Fade>().dialouge = true;//this will tell the fade script to be deactivated DM
            if (read)
            {
                //Log.GetComponent<LogSystem>().textLog.text += objectConversation;
                read = false;
            }
            // When story event is true (On Trigger Allows code to run 1 time)
            if (StoryEvent)
            {
                startSring = "";    // Make sure the start string is nothing
                Conversation = objectConversation[conversationScroller];    // Set up what the conversation involves
                typeWriterScript.ChangeText(Conversation, 2);   // Run The Conversation at hand 
                player.GetComponent<Player_Controller>().speed = 0; // Turn off the players current speed so players are influenced to read text
            }
        }
    }
    private bool pressed = false;
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.name == "PC")
        {
            if (!pressed)
            {
                GameObject.Find("PlayerUIController/Interact/Exposition_Text").GetComponent<TextMeshPro>().enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    startSring = "";
                    Conversation = objectConversation[conversationScroller];    // Set up what the conversation involves
                    typeWriterScript.ChangeText(Conversation, 2);   // Run The Conversation at hand 
                     
                    pressed = true;
                    if (pressed)
                        GameObject.Find("PlayerUIController/Interact/Exposition_Text").GetComponent<TextMeshPro>().enabled = false;

                    ///GetComponentInChildren<TextMeshPro>().enabled = true;
                    //GetComponentInChildren<TextMeshPro>().text = Conversation;

                }
            }
        }
    }

    // We end the conversation with the object
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PC")
        {
            if (!pressed)
                GameObject.Find("PlayerUIController/Interact/Exposition_Text").GetComponent<TextMeshPro>().enabled = false;
            isTalking = false;
            switcher = 1;
            //GameObject.Find("PlayerUIController/Panel").GetComponent<Panel_Fade>().dialouge = false;//this will tell the fade script to be activated DM
            startSring = "Press E";
            PC_Aproached_Me = false;
            // set the conversation to be shown as over
            //Conversation = "Conversation Over";
            //typeWriterScript.StopCoroutine(typeWriterScript.PlayText());
            // reset the array scroller
            //conversationScroller = 0;
            // TextMesh needs to know what to print 
           // projectTextObject.text = Conversation;
        }
    }
    #endregion


    IEnumerator loopFind()
    {
        while(projectTextObject == null && typeWriterScript == null)
        {
            // Find the text
            projectTextObject =  GetComponentInChildren<TextMeshPro>();// GameObject.Find("PlayerUIController/Panel/Exposition_Text").GetComponent<TextMeshPro>();
            // Find the typewriter
            typeWriterScript =GetComponentInChildren<UITypeWritereffect>(); 
            yield break;  //GameObject.Find("PlayerUIController/Panel/Exposition_Text").GetComponent<UITypeWritereffect>();
        }
        
    }

}
