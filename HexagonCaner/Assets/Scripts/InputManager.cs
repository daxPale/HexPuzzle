using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private DotSystem dotSystem;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Transform selectSprite;

    private Transform selectInstance;

    private RotationPoint selectedPoint;
    private bool selected = false;
    private bool clockwise = false;
    public void RotateClockwiseSelectedPoint() { dotSystem.RotateClockWise(selectedPoint); }
    public void RotateAntiClockwiseSelectedPoint() { dotSystem.RotateAntiClockWise(selectedPoint); } 

    void Start()
    {
        selectInstance = Instantiate(selectSprite) as Transform;
        selectInstance.gameObject.SetActive(selected);
    }

    // Update is called once per frame
    void Update()
    {
        if (gridSystem.InputAvailabile() && Input.touchCount > 0)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (worldPosition.y > -2.5f) // above buttons
            {
                selected = true;
                selectedPoint = dotSystem.FindClosestPoint(worldPosition);
                selectedPoint.AdjustSelectSpriteTransform(selectInstance);
            }
        }
    }
}
