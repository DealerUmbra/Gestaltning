using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class ProcDrawLine : MonoBehaviour {

    List<LinePoint> points;
    public Mesh mesh;
    public Material material;
    public float minDistance;
    public float width;

    Vector3[] vertices;
    int[] triangles;

    public DrawUI drawEnv;

    public Transform linePrefab;

    void Awake()
    {
        points = new List<LinePoint>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        GetComponent<MeshRenderer>().material = material;

        vertices = new Vector3[4];
        triangles = new int[2];

    }

	// Use this for initialization
	void Start () {
        vertices = new Vector3[4];
        triangles = new int[6];


	}

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            switch (drawEnv.getActiveTool())
            {
                case CreationTools.Draw:
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        startPosition.y = 0;
                        CreatePoint(startPosition, transform.forward);

                    }
                    else if (Input.GetMouseButton(0))
                    {
                        Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        curPosition.y = 0;
                        if ((curPosition - points[points.Count - 1].transform.position).magnitude >= minDistance)
                        {
                            Vector3 direction = curPosition - points[points.Count - 1].transform.position;
                            LinePoint p = CreatePoint(curPosition, direction);
                            p.SetDirection(direction);
                        }
                    }

                    else if (Input.GetMouseButtonUp(0))
                    {
                        Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        curPosition.y = 0;
                        Vector3 direction = curPosition - points[points.Count - 1].transform.position;
                        points[points.Count - 1].SetDirection(direction);
                    }

                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.RecalculateNormals();
                    break;
            }

            
        }
    }
    LinePoint CreatePoint(Vector3 position, Vector3 direction)
    {
        Transform point = (Transform)Instantiate(linePrefab, position, Quaternion.identity);
        point.name = "Point " + (points.Count + 1);

        point.transform.parent = transform;
        LinePoint p = point.GetComponent<LinePoint>();
        p.SetWidth(width);
        p.SetDirection(direction);
        AddPoint(p);
        return p;
    }

    void AddPoint(LinePoint point)
    {
        if(points.Count > 0)
        {
            int vCount = vertices.Length;
            Array.Resize(ref vertices, vertices.Length + 4);
            LinePoint prevPoint = points[points.Count - 1];

            vertices[vCount] = point.getCurVector(0);
            vertices[vCount + 1] = point.getCurVector(1);
            vertices[vCount + 2] = point.getCurVector(2);
            vertices[vCount + 3] = point.getCurVector(3);

            int tCount = triangles.Length;
            Array.Resize(ref triangles, triangles.Length + 30);

            int prevIndex = vCount - 4;
            for (int i=0; i<4; i++)
            {
           
                AddQuad(prevIndex + i, prevIndex  + (1 + i) % 4, vCount + i, vCount + (i + 1) % 4, tCount + 6 * i);
            }
            AddQuad(vCount, vCount + 1, vCount + 3, vCount + 2, tCount + 24);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            { vertices[i] = point.getCurVector(i); }
            
            AddQuad(0, 3, 1, 2, 0);
        }


        points.Add(point);
    }

    void AddQuad(int v0, int v1, int v2, int v3, int tIndex)
    {
        triangles[tIndex] = v0;
        triangles[tIndex + 3] = triangles[tIndex + 2] = v1;
        triangles[tIndex + 4] = triangles[tIndex + 1] = v2;
        triangles[tIndex + 5] = v3;
    }

    private void OnDrawGizmos()
    {
        if(vertices != null) {
            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
        
    }
}
