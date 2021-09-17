using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPoints : MonoBehaviour
{
    [SerializeField] private GridSystem grids;
    [SerializeField] private Transform rotatePointPrefab;
    private List<Transform> centers = new List<Transform>();
    private float centersWidth;
    private float centersHeight;
    public float CentersWidth { get { return centersWidth; } private set { } }
    public float CentersHeight { get { return centersHeight; } private set { } }
    private void InstantiateTopRight(Transform hex)
    {
        float posX, posY;
        posX = hex.position.x + grids.HexWidth / 4;
        posY = hex.position.y + grids.HexHeight / 2;

        Transform instance = Instantiate(rotatePointPrefab) as Transform;
        instance.name = "RotationPoint";
        instance.position = new Vector3(posX, posY, 0);
        instance.parent = this.transform;
        centers.Add(instance);
    }
    private void InstantiateTopLeft(Transform hex)
    {
        float posX, posY;
        posX = hex.position.x - grids.HexWidth / 4;
        posY = hex.position.y + grids.HexHeight / 2;

        Transform instance = Instantiate(rotatePointPrefab) as Transform;
        instance.name = "RotationPoint";
        instance.position = new Vector3(posX, posY, 0);
        instance.parent = this.transform;
        centers.Add(instance);
    }

    private void CreateRotatePoints()
    {
        for (int y = 0; y < grids.GridHeight - 1; y++)
        {
            for (int x = 0; x < grids.GridWidth; x++)
            {
                //Find the next hexagon's position
                int index = (int)(y * grids.GridWidth + x);
                Transform hexPos = grids.Hexagons[index];
                if (x == 0) //Fist coloum hexagons have only top right rotation point 
                    InstantiateTopRight(hexPos);
                else if (x == grids.GridWidth - 1)  //Last coloum hexagons have only top left rotation point 
                    InstantiateTopLeft(hexPos);
                else // Other hexagons -except top row- have both rotation points 
                {
                    InstantiateTopLeft(hexPos);
                    InstantiateTopRight(hexPos);
                }
            }
        }
    }
    private void Awake()
    {
        centersHeight = 2 * grids.GridHeight - 2;
        centersWidth = grids.GridWidth - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateRotatePoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
