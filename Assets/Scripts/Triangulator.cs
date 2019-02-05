using UnityEngine;
using System.Collections.Generic;

public class Triangulator
{
    // Saving all the points for the triangles
    private List<Vector2> points = new List<Vector2>();

    public Triangulator(Vector2[] points_a){ points = new List<Vector2>(points_a);}

    // seed the function with the points created and then output the trianngles resulting from it
    public int[] Triangulate(){
        List<int> index_points = new List<int>();

        if (points.Count< 3)
            return index_points.ToArray();

        int[] triangle_points = new int[points.Count];
        if (Surface_Area() > 0) {
            for (int i = 0; i < points.Count; i++){
              triangle_points[i] = i;
            }
        }

        else{
            for (int i = 0; i < points.Count; i++){
              triangle_points[i] = (points.Count - 1) - i;
            }


        }

        //------------------------[ Calculations ] -------------------------------------//

        int all_points = points.Count;
        int count = all_points*2;
        for (int m = 0, v = all_points - 1; all_points > 2;){

            if ((count--) <= 0){
                return index_points.ToArray();
              }
            int u = v;


            if (all_points <= u){u = 0;}
            v = u + 1;

            if (all_points <= v){v = 0;}
            int w = v + 1;

            if (all_points <= w){  w = 0;}


            // Check if it is a snippet
            if (Snippet(u, v, w, all_points, triangle_points)){

                int a, b, c, s, t;

                a = triangle_points[u];

                b = triangle_points[v];

                c = triangle_points[w];

                index_points.Add(a);

                index_points.Add(b);

                index_points.Add(c);
                m++;

             // Loop through the snipped points
                for (s = v, t = v + 1; t < all_points; s++, t++){

                    triangle_points[s] = triangle_points[t];

                }

                all_points--;
                count = 2 * all_points;
            }


        }

        index_points.Reverse(); // ReOrder the Points
        return index_points.ToArray();
    }


    //---------------------------------------------[ Helper functions ]---------------------//
    // Total surface area
    private float Surface_Area()
    {
        float A = 0.0f;
        for (int p = points.Count - 1, q = 0; q < points.Count; p = q++) {
            Vector2 pval = points[p];
            Vector2 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    // Getting the snippet of the area
    private bool Snippet(int u, int v, int w, int n, int[] V) {

        Vector2 A = points[V[u]];
        Vector2 B = points[V[v]];
        Vector2 C = points[V[w]];


        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x)))){
          return false;
        }



        for (int p = 0; p < n; p++){
            if ((p == u) || (p == v) || (p == w)){
              continue;
            }



            Vector2 P = points[V[p]];

            // Then check for the calculated points if they were part of the boundry
            if (InBoundry(A, B, C, P)){
              return false;
            }
        }
        return true;
    }

    // Check if the points are all inside the triangle or outside the triangle
    private bool InBoundry(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float boundry_c, boundry_b, boundry_a;

        ax = C.x - B.x;

        ay = C.y - B.y;

        bx = A.x - C.x;

        by = A.y - C.y;

        cx = B.x - A.x;

        cy = B.y - A.y;

        apx = P.x - A.x;

        apy = P.y - A.y;

        bpx = P.x - B.x;

        bpy = P.y - B.y;

        cpx = P.x - C.x;

        cpy = P.y - C.y;

        boundry_a = ax * bpy - ay * bpx;

        boundry_c = cx * apy - cy * apx;

        boundry_b = bx * cpy - by * cpx;

        return ((boundry_a >= 0.0f) && (boundry_b >= 0.0f) && (boundry_c >= 0.0f));
    }
}
