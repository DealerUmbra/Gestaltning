using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonMaterialStorage : MonoBehaviour {

    public Material[] materials;
    /* 1: Brown
     * 2: Dark Green
     * 3: Light Green
     */

    public Material getMaterial(int i)
    {
        return materials[i];
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
