using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Transform hexPrefab;
    [SerializeField] private List<Color> colors;
    private float hexWidth = 2;
    private float hexHeight = Mathf.Sqrt(3);
    private Vector3[,] hexPositions;
    private Hexagon[,] hexagons;

    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 9;
    [SerializeField] private float gap = 0.1f;

    public bool gameEnd;
    public bool hexagonRotationStatus;
    public bool hexagonExplosionStatus;
    public bool hexagonProductionStatus;

    public Vector3[,] HexPositions { get { return hexPositions; } private set { } }
    public Hexagon[,] Hexagons { get { return hexagons; } }
    public List<Color> Colors { get { return colors; } private set { } }
    public float HexWidth { get { return hexWidth; } private set { } }
    public float HexHeight { get { return hexHeight; } private set { } }
    public float GridWidth { get { return gridWidth; } private set { } }
    public int GridHeight { get { return gridHeight; } private set { } }

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
                hexPositions[x, y] = instance.position;
                hexagons[x, y] = instance.GetComponent<Hexagon>();
            }
        }
    }
    public void SpawnNewHexagon(int x, int y)
    {
        Transform instance = Instantiate(hexPrefab) as Transform;
        instance.name = "Hexagon";
        instance.DOMove(HexPositions[x, y], 0.5f).From(new Vector3(x * HexWidth, y * HexHeight + 10f, 0));
        instance.position = HexPositions[x, y];
        instance.parent = this.transform;
        hexagons[x, y] = instance.GetComponent<Hexagon>();
        SetRandomColor(hexagons[x, y]);
    }

    private void SetRandomColor(Hexagon hex)
    {
        int colorIndex = Random.Range(0, colors.Count);

        hex.GetComponent<Renderer>().material.SetColor("_Color", colors[colorIndex]);
    }

    public bool InputAvailabile()
    {
        return !hexagonProductionStatus && !hexagonRotationStatus && !hexagonExplosionStatus && !gameEnd;
    }

    private void Awake()
    {
        hexagons = new Hexagon[(int)gridWidth, (int)gridHeight];
        hexPositions = new Vector3[(int)gridWidth, (int)gridHeight];
    }

    void Start()
    {
        AddGap();
        CreateGrid();
    }

    void Update()
    {
        
    }
}
