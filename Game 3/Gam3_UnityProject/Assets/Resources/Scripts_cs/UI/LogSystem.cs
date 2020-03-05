using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogSystem : MonoBehaviour
{

    public GameObject PCUI;
    public TextMeshProUGUI textLog;
    public bool newContent;
    public bool pause;
    // Start is called before the first frame update
    void Start()
    {
        PCUI = GameObject.Find("Exposition_Text");
        // textLog = GameObject.Find("Pause_Canvas/ScrollArea/TextContainer/Text (TMP)").GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        if (newContent || Input.GetKeyDown(KeyCode.Pause) && textLog == null)
        {
            textLog = GameObject.Find("Pause_Canvas/ScrollArea/TextContainer/Text (TMP)").GetComponent<TextMeshProUGUI>();
            UpdateLog();

        }
    }


    void UpdateLog()
    {
        if (PCUI != null && PCUI.GetComponent<UITypeWritereffect>().talking == false)
        {

            textLog.text += PCUI.GetComponent<TextMeshPro>().text;
            newContent = false;
        }
        else
            print("not found");
       
    }
}
