using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonChange : MonoBehaviour {
    
    HexCell hexCell;

    public Season season = Season.Spring;
    MeshRenderer mr;
    SeasonMaterialStorage smStorage;
    public int configLength;
    public int[] materialIndices;
    public int[] seasonConfigs;

    void Start()
    {
        smStorage = GameObject.Find("Hex Grid").GetComponent<SeasonMaterialStorage>();
        mr = GetComponent<MeshRenderer>();
    }

    public HexCell HexCell
    {
        set
        {
            hexCell = value;
        }
    }

	
	// Update is called once per frame
	void Update () {
		if(season != hexCell.Season)
        {
            season = hexCell.Season;
            Material[] temp = mr.materials;
            for(int i = 0; i < mr.materials.Length; i++) {
                temp[i] = smStorage.getMaterial(materialIndices[seasonConfigs[(int)season] * configLength + i]);
            }
            mr.materials = temp;
        }
    }
}