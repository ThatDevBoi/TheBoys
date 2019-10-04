using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject layer3;
    public GameObject layer2;
    public GameObject layer1;
    public GameObject layer1plus;
    float t;
    // Start is called before the first frame update
    void Start()
    {
        //layer1plus = layer1;
        //layer1plus.transform.Translate(15f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        t = Time.smoothDeltaTime;
        layerone();
        layertwo();
    }

    void layerone()
    {
        layer1.transform.Translate(-1*t,0,0);
        layer1plus.transform.Translate(-1 * t, 0, 0);
        if (layer1.transform.position.x< -25f)
            layer1.transform.Translate(30, 0, 0);
        if (layer1plus.transform.position.x< -25f)
            layer1plus.transform.Translate(30, 0, 0);
    }
    void layertwo()
    {
        layer2.transform.Translate(-0.1f * t, 0, 0);
        if (layer2.transform.position.x < -15f)
            layer2.transform.Translate(30, 0, 0);
    }

}
