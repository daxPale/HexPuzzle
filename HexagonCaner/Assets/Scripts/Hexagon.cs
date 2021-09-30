using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{  
    public Color GetColor()
    {
        return GetComponent<Renderer>().material.GetColor("_Color");
    } 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
