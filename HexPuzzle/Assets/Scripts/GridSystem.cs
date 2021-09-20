using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform hexPrefab;
    [SerializeField] private List<Color> colors;
    private float hexWidth = 2;
    private float hexHeight = Mathf.Sqrt(3);
    private List<Transform> hexTransforms = new List<Transform>();
    private Hexagon[,] hexagons;

    [SerializeField] private float gridWidth = 9;
    [SerializeField] private float gridHeight = 8;
    [SerializeField] private float gap = 0.1f;

    public List<Transform> HexTransforms { get { return hexTransforms; } private set { } }
    public Hexagon[,] Hexagons { get { return hexagons; } }
    public List<Color> Colors { get { return colors; } private set { } }
    public float HexWidth { get { return hexWidth; } private set { } }
    public float HexHeight { get { return hexHeight; } private set { } }
    public float GridWidth { get { return gridWidth; } private set { } }
    public float GridHeight { get { return gridHeight; } private set { } }

    public Hexagon GetHexWithIndex(Vector2Int indexes)
    {
        return hexagons[indexes.x, indexes.y];
    }

    private void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    private Vector3 CalculateHexPosition(float x, float y)
    {
        float posX, posY;

        posX = x * hexWidth * 0.75f;
        posY = y * hexHeight;

        if (x % 2 != 0)
        {
            posY -= hexHeight/2;
        }

        return new Vector3(posX, posY, 0);
    }

    private void CreateGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Transform instance = Instantiate(hexPrefab) as Transform;
                instance.name = "Hexagon" + "(" + x + "," + y + ")";
                instance.position = CalculateHexPosition(x, y);
                instance.parent = this.transform;
                hexTransforms.Add(instance);
                hexagons[x,y] = instance.GetComponent<Hexagon>();
            }
        }
    }

    private void AdjustCameraPos()
    {
        var height = 7.0f; //Calculate exact positions according to grid sizes
        var width = 7.0f;

        mainCamera.orthographicSize = 10.0f;
        mainCamera.GetComponent<Transform>().position += new Vector3(width, height, 0);
    }

    private void Awake()
    {
        hexagons = new Hexagon[(int)gridWidth, (int)gridHeight];
    }

    void Start()
    {
        AddGap();
        CreateGrid();
        AdjustCameraPos();
    }

    void Update()
    {
        
    }
}
