using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHeightMap : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 10f;
    public float heightMultiplier = 5f;

    private Mesh mesh;
    private Vector3[] vertices;

    void Start()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[(width + 1) * (height + 1)];
        int[] triangles = new int[width * height * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        // Generate vertices
        for (int z = 0, i = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float y = Mathf.PerlinNoise((float)x / width * scale, (float)z / height * scale) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);
                uvs[i] = new Vector2((float)x / width, (float)z / height);
            }
        }

        // Generate triangles
        int vert = 0, tris = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}
