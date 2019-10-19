using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trash : MonoBehaviour
{


    // Variables
    
    // public float distanceToTalk;
    public Text dialogueText;
    //public BoxCollider dialouge_collider;
    private bool dialouge = false;
    public GameObject DialogueCan;
    GameObject player;

    // Start is called before the first frame update
    private void Start()
    {
        //DialogueCan = GameObject.Find("DialogueCanvas");
        DialogueCan.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // Change Sentence
        if (dialouge)
            PickUp(player);       

    }

    void PickUp(GameObject other)
    {
        DialogueCan.SetActive(true);
        // Text 
        dialogueText.text = ("Pick up [E]");
        
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
            other.transform.LookAt(this.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Camera cam;

        if (other.gameObject.name == "Player")
        {
            Debug.Log("exit_talk");
            dialouge = false;
            DialogueCan.SetActive(false);
            cam = other.GetComponentInChildren<Camera>();
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, 0.3f);
        }
    }

}

