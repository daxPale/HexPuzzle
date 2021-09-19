using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPoint : MonoBehaviour
{
    public Neighbours neighbours;
    public EPointType type;
    public enum EPointType
    {
        LEFT,
        RIGHT
    }

    public struct Neighbours
    {
        public Hexagon up;
        public Hexagon down;
        public Hexagon side;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
