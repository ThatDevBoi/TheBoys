using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade_points : MonoBehaviour
{
    
    private Level_Manager get_score;
    private float fade_percentage;
    // Start is called before the first frame update
    void Start()
    {
        get_score=GetComponent<Level_Manager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        fade_percentage = get_score.trashScore;
        RenderSettings.fogDensity = 0.01f - (fade_percentage/100);        
    }
}
