using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    int count;

    public Material[] materials;

    bool migratory = false;

    public int Count
    {
        get { return count; }
        set { count = value; transform.localScale = Vector3.one * (count / 450f + 8f / 9f); }
    }
    HexCell currentCell;
    float birthRate = 0.02f;

    // Use this for initialization
    public void Create(HexCell startCell, int pop)
    {
        Count = pop;
        currentCell = startCell;
        currentCell.AddPopulation(this);
    }

    public void UpdatePosition(HexCell cell)
    {
        transform.position = cell.Position + new Vector3(0, 5, 0);
        cell.AddPopulation(this);
        currentCell.RemovePopulation(this);
        currentCell = cell;
    }
    void Start () {
        InvokeRepeating("Tick", 0f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Tick()
    {
        if (!migratory) {
            Count += (int)(birthRate * Count + currentCell.Sustainability);
        }
        else
        {
            Count = (int)((1 - birthRate) * Count);
        }
        
        currentCell.Tick();
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
                    if (dCell.Desirability > dValue)
                    {
                        dValue = dCell.Desirability;
                        d = i;
                    }
                }
                
            }
        }
        //Do stuff
    }
}
