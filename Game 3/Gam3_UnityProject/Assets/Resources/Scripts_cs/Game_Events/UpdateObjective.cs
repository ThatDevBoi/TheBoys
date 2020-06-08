using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateObjective : MonoBehaviour
{
    public string newObjective; // changes current objective 

    public string UpgradeText = ""; // text that shows players what they got before it vanishes 
    public bool changeMessage = false;  // allows there to be to messages shown

    public bool triggered = false; // are we in the trigger?

    public float timer = 2f;

    // Update is called once per frame
    void Update()
    {
        ChangeText();
    }

    void ChangeText()
    {
        // Find the text we wish to change
        TextMeshPro editingText = GameObject.Find("PlayerUIController/Objective").GetComponent<TextMeshPro>();
        if(changeMessage == true && triggered == true)
        {
            editingText.text = UpgradeText;
            if(editingText.text == UpgradeText)
            {
                timer -= Time.deltaTime;

                if(timer <= 0)
                {
                    editingText.text = newObjective;
                }
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PC")
        {
            triggered = true;

            if (changeMessage)
            {
                gameObject.GetComponent<SphereCollider>().enabled = false;
                return;
            }
            else
            {
                GameObject.Find("PlayerUIController/Objective").GetComponent<TextMeshPro>().text = newObjective;
            }

        }
    }
}
