using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//Set the requirements
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent (typeof (MeshRenderer))]

public class TerrainGenerator : MonoBehaviour {

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private int squareCount = 0;

    private List<Vector3> colVertices = new List<Vector3>();
    private List<int> colTriangles = new List<int>();
    private int colCount;

    private Mesh mesh;
    private MeshCollider col;

    public float seed = 0;
    public float scale = 100;
    public float power = 2;
    public int height = 100;
    public int width = 255;
    public float pixelsPerUnit = 1;

    public Text timeText;

	// Use this for initialization
	void Start () {
	
        //Get mesh
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();

        UpdateTerrain();

	}

    void CreateRect(float x, float y) {

        //Create the verticies
        vertices.Add(new Vector3(x, y, 0));
        vertices.Add(new Vector3(x + (1 / pixelsPerUnit), y, 0));
        vertices.Add(new Vector3(x + (1 / pixelsPerUnit), -1, 0));
        vertices.Add(new Vector3(x, -1, 0));

        //Two triangles
        triangles.Add(squareCount * 4);
        triangles.Add(squareCount * 4 + 1);
        triangles.Add(squareCount * 4 + 3);
        triangles.Add(squareCount * 4 + 1);
        triangles.Add(squareCount * 4 + 2);
        triangles.Add(squareCount * 4 + 3);

        //Add the UVs
        uvs.Add(new Vector2(0, 16));
        uvs.Add(new Vector2(8, 16));
        uvs.Add(new Vector2(8, 0));
        uvs.Add(new Vector2(0, 0));

        squareCount++;

    }

    void CreateCollider(float x, float y, float lastX, float lastY) {

        colVertices.Add(new Vector3(lastX, lastY, 1));
        colVertices.Add(new Vector3(x, y, 1));
        colVertices.Add(new Vector3(x, y, 0));
        colVertices.Add(new Vector3(lastX, lastY, 0));

        colTriangles.Add(colCount * 4);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 3);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 2);
        colTriangles.Add((colCount * 4) + 3);

        colCount++;

    }

    float Noise(float x) {
        //return (float)(Mathf.PerlinNoise(x / scale, y / scale) * mag) * (exp);
        return height * Mathf.PerlinNoise(x / scale, seed) * power;
    }

    void CreateMesh() {

        float lastX = -1;
        float lastY = -1;

        //Create the rects
        for (var i = 0; i < width * pixelsPerUnit; i++)
        {

            //Calc the new coords and mesh
            float x = i / pixelsPerUnit;
            float y = Noise(x);
            CreateRect(x, y);

            //Create the new collider
            if(lastX > -1 && lastY > -1) { CreateCollider(x, y, lastX, lastY); }
            lastX = x;
            lastY = y;

        }

    }

    public void UpdateTerrain() {

        float startTime = Time.realtimeSinceStartup;

        //Create the mesh
        CreateMesh();

        //Regen the mesh
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        squareCount = 0;
         
        //Collision mesh
        Mesh newMesh = new Mesh();
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();
        colCount = 0;

        timeText.text = (Time.realtimeSinceStartup - startTime).ToString("f4");

    }

}
