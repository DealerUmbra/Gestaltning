using UnityEngine;
using System.Collections;

public class LinePoint : MonoBehaviour {
    
    float width = 2;
    Vector3[] offsetVectors = new Vector3[4];

	// Use this for initialization

    public LinePoint()
    {
        offsetVectors[0] = (Vector3.left * width + Vector3.down);
        offsetVectors[1] = (Vector3.left * width + Vector3.up);
        offsetVectors[2] = (Vector3.right * width + Vector3.up);
        offsetVectors[3] = (Vector3.right * width + Vector3.down);
    }

    void Start()
    {
        offsetVectors[0] = (Vector3.left * width + Vector3.down);
        offsetVectors[1] = (Vector3.left * width + Vector3.up);
        offsetVectors[2] = (Vector3.right * width + Vector3.up);
        offsetVectors[3] = (Vector3.right * width + Vector3.down);
    }

    public void SetWidth(float width)
    {
        this.width = width;
        offsetVectors[0] = (Vector3.left * width + Vector3.down);
        offsetVectors[1] = (Vector3.left * width + Vector3.up);
        offsetVectors[2] = (Vector3.right * width + Vector3.up);
        offsetVectors[3] = (Vector3.right * width + Vector3.down);
    }


    public void SetDirection(Vector3 direction)
    {
        transform.LookAt(transform.position + direction.normalized);
    }

    public Vector3 getCurVector(int index)
    {
        Vector3 curVector = transform.position + transform.rotation * offsetVectors[index];
        return curVector;

    }
	// Update is called once per frame
	void Update () {
	
	}
}
