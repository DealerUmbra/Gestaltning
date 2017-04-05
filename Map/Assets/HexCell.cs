using UnityEngine;
using System.Collections;

public class HexCell : MonoBehaviour {

    public HexCoordinates coordinates;
    public Color color;
    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor (HexDirections direction)
    {
        return neighbors[(int)direction];
    }
    public void SetNeighbor (HexDirections direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
