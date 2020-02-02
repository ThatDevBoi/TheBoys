using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Panel_Fade : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PC");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Player_Controller>().lastVelocity != Vector3.zero)
        {
            GetComponent<CanvasRenderer>().SetAlpha(0);
            GetComponentInChildren<TextMeshPro>().faceColor = new Color32(1, 1, 1, 0);
        }
        else
        {
            GetComponent<CanvasRenderer>().SetAlpha(255);
            GetComponentInChildren<TextMeshPro>().faceColor = new Color32(255, 255, 255, 255);
        }
    }

   
}
