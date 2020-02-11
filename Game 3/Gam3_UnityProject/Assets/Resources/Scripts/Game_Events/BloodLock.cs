using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodLock : MonoBehaviour
{
    // that need to be removed to do something
    public GameObject[] GO_Activators;
    // single object to be seen or disabled
    public GameObject single_target;
    // multiple objects that can be activated 
    public GameObject[] array_targets;
    public bool activate_Array_elements = false;
    // how many array elements are in the array targets
    private int arrayAmount = 0;
    // bools to control the behaviour
    public bool activate = false;
    public bool array_or_single = false;
    // Start is called before the first frame update
    void Start()
    {
        // if we want to activate this when we remove everything in the bloodlock
        if (activate)
            single_target.SetActive(false);  // this turns on later so it needs to be off
        else
            single_target.SetActive(true);   // if not true then this needs to be on as it reverts the behaviour

        arrayAmount = array_targets.Length;

        if (activate_Array_elements && array_or_single)
        {
            foreach (GameObject goActive in array_targets)
            {
                goActive.SetActive(false);
            }
        }
        else if (!activate_Array_elements && array_or_single)
        {
            foreach (GameObject goActive in array_targets)
            {
                goActive.SetActive(true);
            }
            print("Turning on array");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // objects in the array
        int objectsRemain = GO_Activators.Length;

        if (activate && array_or_single && activate_Array_elements)
            Debug.LogError(gameObject.name + ":" + "Do Not Have All Bools as True! You Rather Want Multiple Objects To Spawn/Despawn (The Array)" +
                "Or You Want 1 Single Obkect To Spawn");

        // Find each GameObject in the choosen array
        foreach(GameObject active in GO_Activators)
        {
            if (!active)
                objectsRemain--;
        }

        // if there is no activation GOs
        if (objectsRemain < 1)
        {
            #region Single Logic
            if (activate && !array_or_single)
            {
                single_target.SetActive(true);
            }
            else if(!activate && !array_or_single)
            {
                single_target.SetActive(false);
            }
            else if(array_or_single && activate_Array_elements && !activate)
            {
                foreach (GameObject goActive in array_targets)
                {
                    goActive.SetActive(true);
                }
            }
            else if(array_or_single && !activate_Array_elements && !activate)
            {
                foreach (GameObject goActive in array_targets)
                {
                    goActive.SetActive(false);
                }
            }
            #endregion

            GetComponent<BloodLock>().enabled = false;
        }
    }
}
