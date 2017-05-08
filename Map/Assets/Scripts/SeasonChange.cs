using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonChange : MonoBehaviour {

    public HexGrid hexGrid;
    public Material[] materials;
    public int[] springMaterials;
    public int[] summerMaterials;
    public int[] fallMaterials;
    public int[] winterMaterials;

    Season season = Season.Spring;

    void Start()
    {
        hexGrid = GameObject.Find("Hex Grid").GetComponent<HexGrid>();
    }
    
	
	// Update is called once per frame
	void Update () {
		if(season != hexGrid.Season)
        {
            for(int i=0; i<transform.childCount; i++)
            {
                if (hexGrid.Season == Season.Spring)
                    transform.GetChild(i).GetComponent<MeshRenderer>().material = materials[springMaterials[i]];
                else if (hexGrid.Season == Season.Summer)
                    transform.GetChild(i).GetComponent<MeshRenderer>().material = materials[summerMaterials[i]];
                else if (hexGrid.Season == Season.Fall)
                    transform.GetChild(i).GetComponent<MeshRenderer>().material = materials[fallMaterials[i]];
                else if (hexGrid.Season == Season.Winter)
                    transform.GetChild(i).GetComponent<MeshRenderer>().material = materials[winterMaterials[i]];

            }
            season = hexGrid.Season;
        }
	}
}