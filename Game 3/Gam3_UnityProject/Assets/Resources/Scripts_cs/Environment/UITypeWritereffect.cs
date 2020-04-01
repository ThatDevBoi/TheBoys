using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;
using TMPro;
public class UITypeWritereffect : MonoBehaviour
{
    public TextMeshPro text;
    public GameObject logUI;
    public bool playOnAwake = true;
    public float delayToStart;
    public float delayBetweenChars = 0.125f;
    public float delayAfterPunctuation = 0.5f;
    private GameObject player;
    private float speed;
    public bool talking=false;
    [HideInInspector]
    public string story;
    private float originDelayBetweenChars;
    private bool lastCharPunctuation = false;
    private char charComma;
    private char charPeriod;
    

    void Awake()
    {
        //text = GetComponent<Text>();
       // if (text == null)
            text = GetComponent<TextMeshPro>();
        originDelayBetweenChars = delayBetweenChars;

        charComma = Convert.ToChar(44);
        charPeriod = Convert.ToChar(46);
        player = GameObject.Find("PC");
        logUI = GameObject.Find("Log");
        if (logUI == null)
            print("tex not found");
        //else Debug.Log(logUI.name);
        if (playOnAwake)
        {            
            ChangeText(text.text, delayToStart);
            
        }   
            
        
    }

   
    //Update text and start typewriter effect
    public void ChangeText(string textContent, float delayBetweenChars = 0f)
    {

        
        StopCoroutine(PlayText()); //stop Coroutime if exist
        story = textContent;
        text.text = ""; //clean text
        Invoke("Start_PlayText", delayBetweenChars); //Invoke effect
    }

    public void Start_PlayText()
    {
        StartCoroutine(PlayText());
       
    }

    public IEnumerator PlayText()
    {
        logUI.GetComponent<LogSystem>().newContent=true;
        

        foreach (char c in story)
        {
            talking = true;
            delayBetweenChars = originDelayBetweenChars;
            

            if (lastCharPunctuation)  //If previous character was a comma/period, pause typing
            {
                yield return new WaitForSeconds(delayBetweenChars = delayAfterPunctuation);
                lastCharPunctuation = false;
            }

            if (c == charComma || c == charPeriod)
            {
                lastCharPunctuation = true;
            }

            text.text += c;
           
            yield return new WaitForSeconds(delayBetweenChars);
            player.GetComponent<Player_Controller>().speed = 0;//stop player from moving while text is beign written
        }
        talking = false;
        
        player.GetComponent<Player_Controller>().speed = player.GetComponent<Player_Controller>().currentSpeed;//restore player speed
    }
}