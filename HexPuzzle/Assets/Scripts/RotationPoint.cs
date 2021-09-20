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

    //Neighbors hold the indexes of neighbor hexagons of rotation point
    public struct Neighbours
    {
        public Vector2Int up;
        public Vector2Int down;
        public Vector2Int side;
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
