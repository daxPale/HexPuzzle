using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using DG.Tweening;

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
    private Transform InstantiateTopRight(Vector3 pos)
    {
        float posX, posY;
        posX = pos.x + grids.HexWidth / 4;
        posY = pos.y + grids.HexHeight / 2;

        Transform instance = Instantiate(rotatePointPrefab) as Transform;
        instance.name = "RotationPoint";
        instance.position = new Vector3(posX, posY, 0);
        instance.parent = this.transform;
        dotTransforms.Add(instance);
        return instance;
    }
    private Transform InstantiateTopLeft(Vector3 pos)
    {
        float posX, posY;
        posX = pos.x - grids.HexWidth / 4;
        posY = pos.y + grids.HexHeight / 2;

        Transform instance = Instantiate(rotatePointPrefab) as Transform;
        instance.name = "RotationPoint";
        instance.position = new Vector3(posX, posY, 0);
        instance.parent = this.transform;
        dotTransforms.Add(instance);
        return instance;
    }

    private void FindNeighbors(RotationPoint point, int column, int row)
    {
        /* 
         * Each rotation point has 3 different hexagon around itself
         * Neighbors can be found by rotation point indexes
         * But the calculation of the indexes depends on which column that rotation point is in
         */
        int type = column % 4;

        switch (type)
        {
            case 0:
                point.type = EPointType.RIGHT;
                point.neighbors.up = new Vector2Int(column / 2, row + 1);
                point.neighbors.down = new Vector2Int(column / 2, row);
                point.neighbors.side = new Vector2Int((column / 2) + 1, row + 1); 
                break;
            case 1:
                point.type = EPointType.LEFT;
                point.neighbors.up = new Vector2Int((column + 1) / 2, row + 1);
                point.neighbors.down = new Vector2Int((column + 1) / 2, row);
                point.neighbors.side = new Vector2Int(((column + 1) / 2) - 1, row);
                break;
            case 2:
                point.type = EPointType.RIGHT;
                point.neighbors.up = new Vector2Int(column / 2, row + 1);
                point.neighbors.down = new Vector2Int(column / 2, row);
                point.neighbors.side = new Vector2Int((column / 2) + 1, row);
                break;
            case 3:
                point.type = EPointType.LEFT;
                point.neighbors.up = new Vector2Int((column + 1) / 2, row + 1);
                point.neighbors.down = new Vector2Int((column + 1) / 2, row);
                point.neighbors.side = new Vector2Int(((column + 1) / 2) - 1, row + 1);
                break;
            default:
                break;
        } 
    }

    private void SetFirstColors(RotationPoint point)
    {
        //TODO: Same color check needs improvement

        int colorIndex = 0;
        Neighbors neighbors = point.neighbors;
        List<Color> colors = grids.Colors;
       
        List<Material> materials = new List<Material>()
        {
            grids.GetHexWithIndex(neighbors.up).GetComponent<Renderer>().material,
            grids.GetHexWithIndex(neighbors.down).GetComponent<Renderer>().material,
            grids.GetHexWithIndex(neighbors.side).GetComponent<Renderer>().material,
        };

        foreach (var mat in materials)
        {
            if (mat.GetColor("_Color") == Color.white)
            {
                colorIndex = Random.Range(0, colors.Count);
                mat.SetColor("_Color", colors[colorIndex]);
            }
     
        }

        if (materials[0].GetColor("_Color") == materials[1].GetColor("_Color") &&
            materials[1].GetColor("_Color") == materials[2].GetColor("_Color"))
        {
            foreach (var mat in materials)
            {
                colorIndex = Random.Range(0, colors.Count);
                mat.SetColor("_Color", colors[colorIndex]);
            }
        }
    }

    private void CreateRotatePoints()
    {
        /*
         *  1) Instantiate rotation points by using hexagons' positions on the grid
         *  2) Find all three neighbors hexagon of the point
         *  3) Insert rotation point to the array
         */
        for (int y = 0; y < grids.GridHeight - 1; y++)
        {
            for (int x = 0; x < grids.GridWidth; x++)
            {
                //Find the next hexagon's position
                int dotX;
                Vector3 hexPos = grids.HexPositions[x, y];
                Transform dotPos;

                if (x == 0) //Fist coloumn hexagons have only top right rotation point 
                {
                    dotX = 0;
                    dotPos = InstantiateTopRight(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbors(point, dotX, y);
                    SetFirstColors(point);
                    dots[dotX, y] = point;
                }
                else if (x == grids.GridWidth - 1) //Last coloumn hexagons have only top left rotation point 
                {
                    dotX = 2 * x - 1;
                    dotPos = InstantiateTopLeft(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbors(point, dotX, y);
                    SetFirstColors(point);
                    dots[dotX, y] = point;
                }
                else // Other hexagons -except top row- have both rotation points 
                {
                    dotX = 2 * x - 1;
                    dotPos = InstantiateTopLeft(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbors(point, dotX, y);
                    SetFirstColors(point);
                    dots[dotX, y] = point;

                    dotX = 2 * x;
                    dotPos = InstantiateTopRight(hexPos);
                    RotationPoint point2 = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbors(point2, dotX, y);
                    SetFirstColors(point2);
                    dots[dotX, y] = point2;
                }
            }
        }
    }

    public RotationPoint FindClosestPoint(Vector3 clickPos)
    {
        RotationPoint point = null;
        float minLenght = float.PositiveInfinity;
        Vector3 diff;

        for (int y = 0; y < dotsHeight; y++)
        {
            for (int x = 0; x < dotsWidth; x++)
            {
                diff = clickPos - dots[x, y].transform.position;
                if (Mathf.Abs(diff.magnitude) < minLenght)
                {
                    minLenght = Mathf.Abs(diff.magnitude);
                    point = dots[x, y];
                }  
            }
        }

        return point;
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
