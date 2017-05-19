using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    [SerializeField]
    int count = 0;
    
    float birthRate = 0.02f;
    float mortalityRate = 0.01f;

    HexCell currentCell;

    public Transform popPrefab;
    Transform pTracker;

    HexCell CurrentCell
    {
        get
        {
            return currentCell;
        }
        set
        {
            currentCell = value;
        }
    }

    float[] desirabilityFactors; //Sustainability | Security

    public float dFactor(int i)
    {
        return desirabilityFactors[i];
    }

    List<Population> subPopulations = new List<Population>();

    public Population() {
        subPopulations = new List<Population>();
    }

    void Awake()
    {
        desirabilityFactors = GenerateFactors();
    }

    void Start() {
        subPopulations = new List<Population>();
        currentCell = GetComponent<HexCell>();
        pTracker = Instantiate(popPrefab, transform.position + new Vector3(0, 5, 0), Quaternion.identity);
        pTracker.parent = this.transform;
        pTracker.gameObject.SetActive(false);
    }

    public Population(float[] dFactors, int pop)
    {
        subPopulations = new List<Population>();
        desirabilityFactors = dFactors;
        count = pop;
    }

    public void Create(float[] dFactors, int pop, HexCell startCell, Transform pPrefab)
    {
        subPopulations = new List<Population>();
        desirabilityFactors = dFactors;
        this.popPrefab = pPrefab;
        Count = pop;
    }

    public float[] DesirabilityFactors
    {
        get { return desirabilityFactors; }
    }

    public Material[] materials;

    public int Count
    {
        get {
            if(subPopulations.Count == 0)
            {
                return count;
            }
            else
            {
                int totalCount = 0;
                for (int i = 0; i < subPopulations.Count; i++)
                {
                    totalCount += subPopulations[i].Count;
                }
                return count + totalCount;
            }
            
        }
        set {count = value;
            pTracker.localScale = Vector3.one * (count / 450f + 8f / 9f); }
    }

    // Use this for initialization
    public void Create(float[] dFactors, int pop)
    {
        desirabilityFactors = dFactors;
        count = pop;
    }

    public static float[] GenerateFactors()
    {
        float[] dFactors = new float[2];
        dFactors[0] = Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f;
        dFactors[1] = Mathf.Round(Random.Range(0f, 1f) * 100f) / 100f;
        return dFactors;
    }

    public void AddPopulation(Population pop)
    {
        subPopulations.Add(pop);
    }

    public void RemovePopulation(Population pop)
    {
        subPopulations.Remove(pop);
    }

    void CreateSubgroup(Population population)
    {
        Population newPop = new Population(desirabilityFactors, 50);
        population.Count -= 50;
        AddPopulation(newPop);
    }

    public void UpdatePosition(HexCell cell)
    {
        transform.position = cell.Position + new Vector3(0, 5, 0);
        cell.CellPopulation.AddPopulation(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Tick()
    {
        if(Count > 0)
        {
            pTracker.gameObject.SetActive(true);
        }
        else { pTracker.gameObject.SetActive(false); }

        Count += (int)(birthRate * Count + GetComponent<HexCell>().Sustainability);
        Count -= (int)(mortalityRate * Count);

        for (int i = 0; i < subPopulations.Count; i++)
        {
            subPopulations[i].Count += (int)(birthRate * Count + GetComponent<HexCell>().Sustainability);
            subPopulations[i].Count -= (int)(mortalityRate * Count);
        }
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
