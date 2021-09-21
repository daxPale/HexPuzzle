using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPoint : MonoBehaviour
{
    [SerializeField] GameObject selectSprite;

    public Neighbours neighbours;
    public EPointType type;

    public void AdjustSelectSpriteTransform(Transform selectInstance)
    {
        selectInstance.gameObject.SetActive(true);
        selectInstance.parent = this.transform;
        if (this.type == RotationPoint.EPointType.LEFT)
        {
            selectInstance.localRotation = new Quaternion(0, 0, 180, 1);
            selectInstance.localPosition = new Vector3(-0.25f, 0, -0.5f);
            selectInstance.localScale = Vector3.one;
        }
        else
        {
            selectInstance.localRotation = Quaternion.identity;
            selectInstance.localPosition = new Vector3(0.25f, 0, -0.5f);
            selectInstance.localScale = Vector3.one;
        }
    }

    public enum EPointType
    {
        LEFT,
        RIGHT
    }

    //Neighbors hold the indexes of neighbor hexagons of rotation point
    public struct Neighbours
    {
        public Vector2Int up;
        public Vector2Int down;
        public Vector2Int side;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
