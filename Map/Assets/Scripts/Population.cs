using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    [SerializeField]
    int count;

    float[] desirabilityFactors; //Sustainability | Security

    public float[] DesirabilityFactors
    {
        get { return desirabilityFactors; }
    }

    public Material[] materials;

    public int Count
    {
        get { return count; }
        set { count = value; transform.localScale = Vector3.one * (count / 450f + 8f / 9f); }
    }
    HexCell currentCell;
    float birthRate = 0.02f;
    float mortalityRate = 0.01f;

    // Use this for initialization
    public void Create(int pop)
    {
        desirabilityFactors = new float[2];
        desirabilityFactors[0] = Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f;

        desirabilityFactors[1] = Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f;
        Count = pop;
    }

    public void PlacePop(HexCell startCell)
    {
        transform.position = startCell.Position + new Vector3(0, 5, 0);
        startCell.AddPopulation(this);
        currentCell = startCell;
    }

    public void UpdatePosition(HexCell cell)
    {
        transform.position = cell.Position + new Vector3(0, 5, 0);
        cell.AddPopulation(this);
        currentCell.RemovePopulation(this);
        currentCell = cell;
    }
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Tick()
    {
            Count += (int)(birthRate * Count + currentCell.Sustainability);
        Count -= (int)(mortalityRate * Count);
    }

    void CheckMostDesirableNeighbor()
    {
        HexDirection d = HexDirection.NE;
        int dValue = 0;
        for (HexDirection i = HexDirection.NE; i <= HexDirection.NW; i++)
        {
            if (currentCell.GetNeighbor(i))
            {
                if(currentCell.GetEdgeType(i) != HexEdgeType.Slope) {
                    HexCell dCell = currentCell.GetNeighbor(i);
                    if (dCell.Desirability(desirabilityFactors) > dValue)
                    {
                        dValue = dCell.Desirability(desirabilityFactors);
                        d = i;
                    }
                }
                
            }
        }
        //Do stuff
    }
}
