﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform hexPrefab;
    [SerializeField] private List<Color> colors;
    private float hexWidth = 2;
    private float hexHeight = Mathf.Sqrt(3);

    [SerializeField] private float gridWidth = 9;
    [SerializeField] private float gridHeight = 8;
    [SerializeField] private float gap = 0.1f;

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

    private void SetHexColor(Transform hexagon)
    {
        int index = Random.Range(0, colors.Count);
        var renderer = hexagon.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", colors[index]);
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
                SetHexColor(instance);
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