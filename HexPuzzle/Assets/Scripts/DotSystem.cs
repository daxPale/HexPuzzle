﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RotationPoint;

public class DotSystem : MonoBehaviour
{
    [SerializeField] private GridSystem grids;
    [SerializeField] private Transform rotatePointPrefab;
    public List<Transform> dotTransforms = new List<Transform>();
    public RotationPoint[,] dots;

    private float dotsWidth;
    private float dotsHeight;
    public float DotsWidth { get { return dotsWidth; } private set { } }
    public float DotsHeight { get { return dotsHeight; } private set { } }
    private Transform InstantiateTopRight(Transform hex)
    {
        float posX, posY;
        posX = hex.position.x + grids.HexWidth / 4;
        posY = hex.position.y + grids.HexHeight / 2;

        Transform instance = Instantiate(rotatePointPrefab) as Transform;
        instance.name = "RotationPoint";
        instance.position = new Vector3(posX, posY, 0);
        instance.parent = this.transform;
        dotTransforms.Add(instance);
        return instance;
    }
    private Transform InstantiateTopLeft(Transform hex)
    {
        float posX, posY;
        posX = hex.position.x - grids.HexWidth / 4;
        posY = hex.position.y + grids.HexHeight / 2;

        Transform instance = Instantiate(rotatePointPrefab) as Transform;
        instance.name = "RotationPoint";
        instance.position = new Vector3(posX, posY, 0);
        instance.parent = this.transform;
        dotTransforms.Add(instance);
        return instance;
    }

    private void FindNeighbours(RotationPoint point, int column, int row)
    {
        /* 
         * Each rotation point has 3 different hexagon around itself
         * Neighbours can be found by rotation point indexes
         * But the calculation of the indexes depends on which column that rotation point is in
         */

        Neighbours neighbours = point.neighbours;
        int type = column % 4;
        Debug.Log(column);
        switch (type)
        {
            case 0:
                point.type = EPointType.RIGHT;
                neighbours.up = grids.Hexagons[column / 2, row + 1];
                neighbours.down = grids.Hexagons[column / 2, row];
                neighbours.side = grids.Hexagons[(column / 2) + 1, row + 1];
                break;
            case 1:
                point.type = EPointType.LEFT;
                neighbours.up = grids.Hexagons[(column + 1) / 2, row + 1];
                neighbours.down = grids.Hexagons[(column + 1) / 2, row];
                neighbours.side = grids.Hexagons[((column + 1) / 2) - 1, row];
                break;
            case 2:
                point.type = EPointType.RIGHT;
                neighbours.up = grids.Hexagons[column / 2, row + 1];
                neighbours.down = grids.Hexagons[column / 2, row];
                neighbours.side = grids.Hexagons[column / 2, row];
                break;
            case 3:
                point.type = EPointType.LEFT;
                neighbours.up = grids.Hexagons[(column + 1) / 2, row + 1];
                neighbours.down = grids.Hexagons[(column + 1) / 2, row];
                neighbours.side = grids.Hexagons[((column + 1) / 2) - 1, row + 1];  
                break;
            default:
                break;
        } 
    }

    private void CreateRotatePoints()
    {
        /*
         *  1) Instantiate rotation points by using hexagons' positions on the grid
         *  2) Find all three neighbour hexagon of the point
         *  3) Insert rotation point to the array
         */

        for (int y = 0; y < grids.GridHeight - 1; y++)
        {
            for (int x = 0; x < grids.GridWidth; x++)
            {
                //Find the next hexagon's position
                int hexIndex = (int)(y * grids.GridWidth + x);
                int dotX;
                Transform hexPos = grids.HexTransforms[hexIndex];
                Transform dotPos;

                if (x == 0) //Fist coloum hexagons have only top right rotation point 
                {
                    dotX = 0;
                    dotPos = InstantiateTopRight(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbours(point, dotX, y);
                    dots[dotX, y] = point;
                }
                else if (x == grids.GridWidth - 1) //Last coloum hexagons have only top left rotation point 
                {
                    dotX = 2 * x - 1;
                    dotPos = InstantiateTopLeft(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbours(point, dotX, y);
                    dots[dotX, y] = point;
                }
                else // Other hexagons -except top row- have both rotation points 
                {
                    dotX = 2 * x - 1;
                    dotPos = InstantiateTopLeft(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbours(point, dotX, y);
                    dots[dotX, y] = point;

                    dotX = 2 * x;
                    dotPos = InstantiateTopRight(hexPos);
                    RotationPoint point2 = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbours(point2, dotX, y);
                    dots[dotX, y] = point2;
                }
            }
        }
    }
    private void Awake()
    {
        // Calculate width and height of dots area by using dimensions of grid
        dotsHeight = grids.GridHeight - 1;
        dotsWidth =  2 * grids.GridWidth - 2;
        dots = new RotationPoint[(int)dotsWidth, (int)dotsHeight];
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
