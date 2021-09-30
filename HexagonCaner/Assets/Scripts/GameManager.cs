using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static RotationPoint;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private InputManager inputManager;
    [SerializeField] private DotSystem dotSystem;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Text scoreBoard;

    private ParticleSystem explosionParticles;
    public void RotateClockwiseSelectedPoint()
    {
        if (gridSystem.InputAvailabile())
        {
            gridSystem.hexagonRotationStatus = true;
            StartCoroutine(RotateClockWise(inputManager.SelectedPoint));
        }
    }

    public void RotateAntiClockwiseSelectedPoint()
    {
        if (gridSystem.InputAvailabile())
        {
            gridSystem.hexagonRotationStatus = true;
            StartCoroutine(RotateAntiClockWise(inputManager.SelectedPoint));
        }
    }

    public IEnumerator RotateClockWise(RotationPoint point)
    {
        switch (point.type)
        {
            case EPointType.LEFT:
                /*
                 * side->up
                 * up->down
                 * down->side
                 */
                //grids.GetHexWithIndex(point.neighbors.side).transform.RotateAround(point.transform.position, Vector3.forward, -120);
                //grids.GetHexWithIndex(point.neighbors.up).transform.RotateAround(point.transform.position, Vector3.forward, -120);
                //grids.GetHexWithIndex(point.neighbors.down).transform.RotateAround(point.transform.position, Vector3.forward, -120);

                Vector3 tempL = gridSystem.GetHexWithIndex(point.neighbors.side).transform.position;
                gridSystem.GetHexWithIndex(point.neighbors.side).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.up).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.up).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.down).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.down).transform.DOMove(tempL, 0.5f);

                Hexagon tempHexL = gridSystem.GetHexWithIndex(point.neighbors.side);
                gridSystem.Hexagons[point.neighbors.side.x, point.neighbors.side.y] = gridSystem.GetHexWithIndex(point.neighbors.down);
                gridSystem.Hexagons[point.neighbors.down.x, point.neighbors.down.y] = gridSystem.GetHexWithIndex(point.neighbors.up);
                gridSystem.Hexagons[point.neighbors.up.x, point.neighbors.up.y] = tempHexL;
                break;
            case EPointType.RIGHT:
                /*
                * up->side
                * side->down
                * down->up
                */
                Vector3 tempR = gridSystem.GetHexWithIndex(point.neighbors.up).transform.position;
                gridSystem.GetHexWithIndex(point.neighbors.up).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.side).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.side).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.down).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.down).transform.DOMove(tempR, 0.5f);

                Hexagon tempHexR = gridSystem.GetHexWithIndex(point.neighbors.up);
                gridSystem.Hexagons[point.neighbors.up.x, point.neighbors.up.y] = gridSystem.GetHexWithIndex(point.neighbors.down);
                gridSystem.Hexagons[point.neighbors.down.x, point.neighbors.down.y] = gridSystem.GetHexWithIndex(point.neighbors.side);
                gridSystem.Hexagons[point.neighbors.side.x, point.neighbors.side.y] = tempHexR;
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(0.5f);

        gridSystem.hexagonRotationStatus = false;
        CheckExplosions();
    }

    public IEnumerator RotateAntiClockWise(RotationPoint point)
    {
        switch (point.type)
        {
            case EPointType.LEFT:
                /*
                 * side->down
                 * down->up
                 * up->side
                 */
                Vector3 tempL = gridSystem.GetHexWithIndex(point.neighbors.side).transform.position;
                gridSystem.GetHexWithIndex(point.neighbors.side).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.down).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.down).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.up).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.up).transform.DOMove(tempL, 0.5f);

                Hexagon tempHexL = gridSystem.GetHexWithIndex(point.neighbors.side);
                gridSystem.Hexagons[point.neighbors.side.x, point.neighbors.side.y] = gridSystem.GetHexWithIndex(point.neighbors.up);
                gridSystem.Hexagons[point.neighbors.up.x, point.neighbors.up.y] = gridSystem.GetHexWithIndex(point.neighbors.down);
                gridSystem.Hexagons[point.neighbors.down.x, point.neighbors.down.y] = tempHexL;
                break;
            case EPointType.RIGHT:
                /*
                * up->down
                * down->side
                * side->up
                */
                Vector3 tempR = gridSystem.GetHexWithIndex(point.neighbors.up).transform.position;
                gridSystem.GetHexWithIndex(point.neighbors.up).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.down).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.down).transform.DOMove(gridSystem.GetHexWithIndex(point.neighbors.side).transform.position, 0.5f);
                gridSystem.GetHexWithIndex(point.neighbors.side).transform.DOMove(tempR, 0.5f);

                Hexagon tempHexR = gridSystem.GetHexWithIndex(point.neighbors.up);
                gridSystem.Hexagons[point.neighbors.up.x, point.neighbors.up.y] = gridSystem.GetHexWithIndex(point.neighbors.side);
                gridSystem.Hexagons[point.neighbors.side.x, point.neighbors.side.y] = gridSystem.GetHexWithIndex(point.neighbors.down);
                gridSystem.Hexagons[point.neighbors.down.x, point.neighbors.down.y] = tempHexR;
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(0.5f);

        gridSystem.hexagonRotationStatus = false;
        CheckExplosions();
    }

    private void ExplosionParticles(Vector3 pos, Color color)
    {
        explosionParticles.transform.position = new Vector3(pos.x, pos.y, pos.z - 5.0f);
        var main = explosionParticles.main;
        main.startColor = color;
        explosionParticles.Play();
    }

    public void CheckExplosions()
    {
        bool isExploaded = false;
        for (int y = 0; y < dotSystem.DotsHeight; y++)
        {
            for (int x = 0; x < dotSystem.DotsWidth; x++)
            {
                //If each neighbor hexagons is valid, then check their color. Destroy them when they have a same color

                Neighbors neighbors = dotSystem.dots[x, y].neighbors;
                if ((gridSystem.GetHexWithIndex(neighbors.up) && gridSystem.GetHexWithIndex(neighbors.down) && gridSystem.GetHexWithIndex(neighbors.side)) &&
                    gridSystem.GetHexWithIndex(neighbors.up).GetColor() == gridSystem.GetHexWithIndex(neighbors.down).GetColor() &&
                    gridSystem.GetHexWithIndex(neighbors.down).GetColor() == gridSystem.GetHexWithIndex(neighbors.side).GetColor())
                {
                    ExplosionParticles(dotSystem.dots[x, y].transform.position, gridSystem.GetHexWithIndex(neighbors.up).GetColor());

                    Destroy(gridSystem.GetHexWithIndex(neighbors.up).gameObject);
                    Destroy(gridSystem.GetHexWithIndex(neighbors.down).gameObject);
                    Destroy(gridSystem.GetHexWithIndex(neighbors.side).gameObject);

                    isExploaded = true;
                }
            }
        }

        if (!isExploaded)
        {
            inputManager.SetSelectCursor(true);
        }

        gridSystem.hexagonExplosionStatus = isExploaded;
        gridSystem.hexagonProductionStatus = isExploaded;
    }

    public IEnumerator DropAndFillAfterExplosion()
    {
        gridSystem.hexagonExplosionStatus = false;
        inputManager.SetSelectCursor(false);

        yield return new WaitForSeconds(0.5f);

        for (int x = 0; x < gridSystem.GridWidth; x++)
        {
            int dropCount = 0;
            for (int y = 0; y < gridSystem.GridHeight; y++)
            {
                if (!gridSystem.Hexagons[x, y])
                {
                    dropCount++;
                }
                else if (gridSystem.Hexagons[x, y] && dropCount > 0)
                {
                    gridSystem.Hexagons[x, y].transform.DOMoveY(gridSystem.Hexagons[x, y].transform.position.y - dropCount * gridSystem.HexHeight, 0.5f);
                    gridSystem.Hexagons[x, y - dropCount] = gridSystem.Hexagons[x, y];
                    gridSystem.Hexagons[x, y] = null;
                }
            }

            //Fill the empty rows in a grid's column with new hexagons
            for (int j = dropCount; j > 0; j--)
            {
                gridSystem.SpawnNewHexagon(x, gridSystem.GridHeight - j);
            }
        }
        yield return new WaitForSeconds(0.5f);
       
        gridSystem.hexagonProductionStatus = false;

        CheckExplosions();
    }

    public void CalculateScore()
    {
        int score = 0;

        for (int y = 0; y < gridSystem.GridHeight; y++)
        {
            for (int x = 0; x < gridSystem.GridWidth; x++)
            {
                if (gridSystem.Hexagons[x, y] == null)
                    score++;
            }
        }

        int oldScore = int.Parse(scoreBoard.text);
        oldScore += (score * 5);
        scoreBoard.text = oldScore.ToString();
    }

    void Start()
    {
        explosionParticles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gridSystem.hexagonExplosionStatus)
        {
            CalculateScore();

            StartCoroutine(DropAndFillAfterExplosion());
        }
    }
}
