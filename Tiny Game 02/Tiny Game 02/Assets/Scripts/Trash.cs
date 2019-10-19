using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trash : MonoBehaviour
{


    // Variables
    //public Transform playerPosition;
    public string CharachterName;
    public string[] chatText;
    public int arrayLength;
    public int stringChanger = 0;
    // public float distanceToTalk;
    public Text dialogueText;
    //public BoxCollider dialouge_collider;
    private bool dialouge = false;
    public GameObject DialogueCan;
    GameObject player;

    // Start is called before the first frame update
    private void Start()
    {
        arrayLength = chatText.Length - 1;
        //DialogueCan = GameObject.Find("DialogueCanvas");
        DialogueCan.SetActive(false);
        //gameObject.name = CharachterName;
        //gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Change Sentence
        if (dialouge)
            PickUp(player);

        if (stringChanger > chatText.Length)
        {
            stringChanger = 0;
        }

    }

    void PickUp(GameObject other)
    {
        DialogueCan.SetActive(true);
        // Text Scroller
        dialogueText.text = ("Pick up [E]");
        Debug.Log(stringChanger);
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.SetActive(false);
            dialouge = false;
            DialogueCan.SetActive(false);
        }
        Camera cam;
        cam = other.GetComponentInChildren<Camera>();
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40, 0.3f);//pull player closer to the character
   
    }



    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.name == "Player")
        {
            Debug.Log("in_talk");
            dialouge = true;
            player = other.gameObject;
        }
    }
  

}

