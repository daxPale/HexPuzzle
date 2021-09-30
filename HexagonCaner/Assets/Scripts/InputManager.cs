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
    public RotationPoint SelectedPoint { get { return selectedPoint; } }

    private bool selected = false;
    public void SetSelectCursor(bool active)
    {
        selected = active;
        selectInstance.gameObject.SetActive(selected);
    }

    void Start()
    {
        selectInstance = Instantiate(selectSprite) as Transform;
        SetSelectCursor(false);
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
