using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum CreationTools
{
    Draw,
    Select
}

public class DrawUI : MonoBehaviour {

    CreationTools activeTool = CreationTools.Draw;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setActiveTool(int tool)
    {
       activeTool = (CreationTools) tool;
       
    }

    public CreationTools getActiveTool()
    {
        return activeTool;
    }
}
