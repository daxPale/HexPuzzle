using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        int type = column % 4;

        switch (type)
        {
            case 0:
                point.type = EPointType.RIGHT;
                point.neighbours.up = grids.Hexagons[column / 2, row + 1];
                point.neighbours.down = grids.Hexagons[column / 2, row];
                point.neighbours.side = grids.Hexagons[(column / 2) + 1, row + 1];
                break;
            case 1:
                point.type = EPointType.LEFT;
                point.neighbours.up = grids.Hexagons[(column + 1) / 2, row + 1];
                point.neighbours.down = grids.Hexagons[(column + 1) / 2, row];
                point.neighbours.side = grids.Hexagons[((column + 1) / 2) - 1, row];
                break;
            case 2:
                point.type = EPointType.RIGHT;
                point.neighbours.up = grids.Hexagons[column / 2, row + 1];
                point.neighbours.down = grids.Hexagons[column / 2, row];
                point.neighbours.side = grids.Hexagons[(column / 2) + 1, row];
                break;
            case 3:
                point.type = EPointType.LEFT;
                point.neighbours.up = grids.Hexagons[(column + 1) / 2, row + 1];
                point.neighbours.down = grids.Hexagons[(column + 1) / 2, row];
                point.neighbours.side = grids.Hexagons[((column + 1) / 2) - 1, row + 1];  
                break;
            default:
                break;
        } 
    }

    private void SetFirstColors(RotationPoint point)
    {
        //TODO: Same color check needs improvement

        int colorIndex = 0;
        Neighbours hexagons = point.neighbours;
        List<Color> colors = grids.Colors;
       
        List<Material> materials = new List<Material>()
        {
            hexagons.up.GetComponent<Renderer>().material,
            hexagons.down.GetComponent<Renderer>().material,
            hexagons.side.GetComponent<Renderer>().material,
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
                    SetFirstColors(point);
                    dots[dotX, y] = point;
                }
                else if (x == grids.GridWidth - 1) //Last coloum hexagons have only top left rotation point 
                {
                    dotX = 2 * x - 1;
                    dotPos = InstantiateTopLeft(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbours(point, dotX, y);
                    SetFirstColors(point);
                    dots[dotX, y] = point;
                }
                else // Other hexagons -except top row- have both rotation points 
                {
                    dotX = 2 * x - 1;
                    dotPos = InstantiateTopLeft(hexPos);
                    RotationPoint point = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbours(point, dotX, y);
                    SetFirstColors(point);
                    dots[dotX, y] = point;

                    dotX = 2 * x;
                    dotPos = InstantiateTopRight(hexPos);
                    RotationPoint point2 = dotPos.GetComponentInParent<RotationPoint>();
                    FindNeighbours(point2, dotX, y);
                    SetFirstColors(point2);
                    dots[dotX, y] = point2;
                }
            }
        }
    }

    public void RotateClockWise(RotationPoint point)
    {
        switch (point.type)
        {
            case EPointType.LEFT:
                /*
                 * side->up
                 * up->down
                 * down->side
                 */
                Vector3 upPosL = point.neighbours.up.transform.position;
                Vector3 downPosL = point.neighbours.down.transform.position;
                Vector3 sidePosL = point.neighbours.side.transform.position;

                Hexagon tempL = point.neighbours.side;
                point.neighbours.side = point.neighbours.down;
                point.neighbours.down = point.neighbours.up;
                point.neighbours.up = tempL;
                
                point.neighbours.up.transform.position = upPosL;
                point.neighbours.down.transform.position = downPosL;
                point.neighbours.side.transform.position = sidePosL;
                break;
            case EPointType.RIGHT:
                /*
                * up->side
                * side->down
                * down->up
                */
                Vector3 upPosR = point.neighbours.up.transform.position;
                Vector3 downPosR = point.neighbours.down.transform.position;
                Vector3 sidePosR = point.neighbours.side.transform.position;
             
                Hexagon tempR = point.neighbours.up;
                point.neighbours.up = point.neighbours.down;
                point.neighbours.down = point.neighbours.side;
                point.neighbours.side = tempR;

                point.neighbours.up.transform.position = upPosR;
                point.neighbours.down.transform.position = downPosR;
                point.neighbours.side.transform.position = sidePosR;
                break;
            default:
                break;
        }
    }

    public void RotateAntiClockWise(RotationPoint point)
    {
        switch (point.type)
        {
            case EPointType.LEFT:
                /*
                 * side->down
                 * down->up
                 * up->side
                 */
                Vector3 upPosL = point.neighbours.up.transform.position;
                Vector3 downPosL = point.neighbours.down.transform.position;
                Vector3 sidePosL = point.neighbours.side.transform.position;

                Hexagon tempL = point.neighbours.side;
                point.neighbours.side = point.neighbours.up;
                point.neighbours.up = point.neighbours.down;
                point.neighbours.down = tempL;

                point.neighbours.up.transform.position = upPosL;
                point.neighbours.down.transform.position = downPosL;
                point.neighbours.side.transform.position = sidePosL;
                break;
            case EPointType.RIGHT:
                /*
                * up->down
                * down->side
                * side->up
                */
                Vector3 upPosR = point.neighbours.up.transform.position;
                Vector3 downPosR = point.neighbours.down.transform.position;
                Vector3 sidePosR = point.neighbours.side.transform.position;

                Hexagon tempR = point.neighbours.up;
                point.neighbours.up = point.neighbours.side;
                point.neighbours.side = point.neighbours.down;
                point.neighbours.down = tempR;

                point.neighbours.up.transform.position = upPosR;
                point.neighbours.down.transform.position = downPosR;
                point.neighbours.side.transform.position = sidePosR;
                break;
            default:
                break;
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
