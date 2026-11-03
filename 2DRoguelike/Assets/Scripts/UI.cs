using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) 
        {
            GetComponent<Canvas>().enabled = false;
        }
        if (Time.timeScale == 1)
        {
            GetComponent<Canvas>().enabled = true;
        }
    }
}
