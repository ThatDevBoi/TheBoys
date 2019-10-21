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
        if (fade_percentage != get_score.trashScore)
        {
            fade_percentage = get_score.trashScore;
            if (RenderSettings.fogDensity < 1)
                RenderSettings.fogDensity = RenderSettings.fogDensity / fade_percentage;
            else
                RenderSettings.fogDensity = 0;
        }
    }
}
