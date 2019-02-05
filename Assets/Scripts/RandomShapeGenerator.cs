using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomShapeGenerator : MonoBehaviour {

    public Material mat;
    float width = 1f;
    float height= 1f;

    bool isNeg = false;
    public int total_negs = 0;
    public int total_pos = 0;

    public float max_x = -10000000;
    public float max_y = -10000000;
    public float min_x = 10000000;
    public float min_y = 10000000;

    System.Random rnd;
    public float xs;
    public float zs;
    public void createObject () {
        Mesh M = new Mesh();

        System.Random rnd = new System.Random();

        int ran = rnd.Next(4, 12);

        List<Vector2> points = new List<Vector2>();

        // Size height total = 7 so each obstacle can be between the heigt 0.875 to 3.2
        // half width is 8- 2 aka 6 so points can be at most 6 width long 
        for(int i = 0;i< ran; i++)
        {
            Vector2 position = new Vector2(Random.Range(-3f, 3f), Random.Range(-0.5f, 0.5f));
            points.Add(position);
            print(position);

            if(position.x < 0)
            {
                total_negs += 1;
            }

            if (position.x >= 0)
            {
                total_pos += 1;
            }

            if (position.x <min_x)
            {
                min_x = position.x;
            }


            if (position.y < min_y)
            {
                min_y = position.y;
            }


            if (position.x > max_x)
            {
                max_x = position.x;
            }


            if (position.y > max_y)
            {
                max_y = position.y;
            }
        }





        /*
        Vector2[] vertices2D = new Vector2[] {
            new Vector2(0,1),
            new Vector2(0,2),
            new Vector2(-2,1),
            new Vector2(1,-2),
            new Vector2(2,0),
        };
        */

        Vector2[] vertices2D = points.ToArray();

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);

        // Get the triangles of the given vertices 
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, -vertices2D[i].y);
        }

        // Create the mesh
        M.vertices = vertices;

        M.triangles = indices;
        M.RecalculateNormals();
        M.RecalculateBounds();

        // -- Visualizations 

        GetComponent<MeshFilter>().mesh = M;
        GetComponent<MeshRenderer>().material = mat;
        GetComponent<MeshCollider>().sharedMesh = M;

        //print(GetComponent<MeshCollider>().bounds);
    }
    private void Start()
    {
        //Place them at random points within the 1/8 - 1/2 of the map
        rnd = new System.Random(this.GetInstanceID());
        xs = Random.Range(-1.4f, 5);
        zs = Random.Range(-0.58f, -4.2f);
        this.transform.position= new Vector3(xs, 0.2f, zs);
    }
}
