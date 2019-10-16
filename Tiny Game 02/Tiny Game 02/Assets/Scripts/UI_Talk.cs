using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Talk : MonoBehaviour
{
    // Variables
    public Transform playerPosition;
    public string objectiveName;
    public string[] chatText;
    public int arrayLength;
    public int stringChanger = 0;
    public float distanceToTalk;
    public Text dialogueText;

    public GameObject DialogueCan;

    // Start is called before the first frame update
    void Start()
    {
        arrayLength = chatText.Length - 1;
        DialogueCan = GameObject.Find("DialogueCanvas");
        DialogueCan.SetActive(false);
        gameObject.name = objectiveName;
    }

    // Update is called once per frame
    void Update()
    {
        // Find the players position
        playerPosition = GameObject.Find("Player").GetComponent<Transform>();
        Interaction();
        
        if (stringChanger > chatText.Length)
        {
            stringChanger = 0;
        }

    }

    void Interaction()
    {
        // Distance
        float distance;
        distance = Vector3.Distance(transform.position, playerPosition.position);
        Debug.Log(distance);
        if (distance > distanceToTalk)
        {
            StartCoroutine(Talk());
        }
    }

    IEnumerator Talk()
    {
        DialogueCan.SetActive(true);
        int arrayChanger = chatText[stringChanger].Length;
        // Text Scroller
        dialogueText.text = chatText[stringChanger];
        // Change Sentence
        if (Input.GetKeyDown(KeyCode.E))
        {
            stringChanger++;
        }
        // Scroller Check
        else if (stringChanger >= arrayLength)
        {
            yield return new WaitForSeconds(0.50f);
            stringChanger = 0;
        }
        yield break;
    }
}
