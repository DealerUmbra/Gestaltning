using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

    public Mesh mesh;
    public Material material;
    public int maxDepth;
    public float childScale; 

    int depth;


	// Use this for initialization
	void Start () {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        if(depth < maxDepth)
        {
            new GameObject().AddComponent<Fractal>().Initialize(this, Vector3.up);
            new GameObject().AddComponent<Fractal>().Initialize(this, Vector3.right);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Initialize(Fractal parent, Vector3 direction)
    {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        depth = parent.depth + 1;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = direction * (0.5f + 0.5f * childScale);
    }
}
