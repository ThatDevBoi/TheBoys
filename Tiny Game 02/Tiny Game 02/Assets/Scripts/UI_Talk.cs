using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Talk : MonoBehaviour
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
    private bool dialouge=false;
    public GameObject DialogueCan;

    // Start is called before the first frame update
    private void Start()
    {
        arrayLength = chatText.Length - 1;
        DialogueCan = GameObject.Find("DialogueCanvas");
        DialogueCan.SetActive(false);
        gameObject.name = CharachterName;
        //gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Change Sentence
        if(dialouge)
            StartCoroutine(Talk());

        if (stringChanger > chatText.Length)
        {
            stringChanger = 0;
        }

    }

    IEnumerator Talk()
    {
        DialogueCan.SetActive(true);
        //int arrayChanger = chatText[stringChanger].Length;
        // Text Scroller
        dialogueText.text =CharachterName+ chatText[stringChanger];
        Debug.Log(stringChanger);
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("e");
            stringChanger++;
        }

        //Scroller Check
        if (stringChanger >= arrayLength)
        {
            yield return new WaitForSeconds(0.50f);
            stringChanger = 0;
        }
        yield break;
    }

    

    private void OnTriggerEnter(Collider other)
    {

        
        if (other.gameObject.name == "Player")
        {
            Debug.Log("in_talk");
            dialouge = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {


        if (other.gameObject.name == "Player")
        {
            Debug.Log("exit_talk");
            dialouge = false;
            DialogueCan.SetActive(false);
        }
    }

}
