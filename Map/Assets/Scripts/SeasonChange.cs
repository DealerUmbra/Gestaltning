using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonChange : MonoBehaviour {

    HexGrid hexGrid;

    Season season = Season.Spring;
    SeasonMaterialStorage smStorage;
    public int configLength;
    public int[] configs;
    public int[] childConfigUse;
    public int[] seasonConfigUse;

    void Start()
    {
        hexGrid = GameObject.Find("Hex Grid").GetComponent<HexGrid>();
        smStorage = GameObject.Find("Hex Grid").GetComponent<SeasonMaterialStorage>();
    }
    
	
	// Update is called once per frame
	void Update () {
		if(season != hexGrid.Season)
        {
            season = hexGrid.Season;
            for (int i=0; i<transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<MeshRenderer>().material != smStorage.getMaterial(configs[childConfigUse[i] * configLength + seasonConfigUse[(int)season]]))
                transform.GetChild(i).GetComponent<MeshRenderer>().material = smStorage.getMaterial(configs[childConfigUse[i] * configLength + seasonConfigUse[(int) season]]);

            }
        }
	}
}