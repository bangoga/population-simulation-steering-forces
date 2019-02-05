using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Djisktra : MonoBehaviour {

    public GameObject start_position;
    public GameObject end_position;
    public GameObject end_A;
    public GameObject end_B;

    public List<GameObject> all_nodes;
    public List<GameObject> f_visited;
    public List<GameObject> visited;

    public List<GameObject> visited_B;

    public Queue<GameObject> finalpath = new Queue<GameObject>();
    float[] dist;
    float[] prev;

    public Queue<GameObject> path = new Queue<GameObject>();



    Stack<GameObject> DFS = new Stack<GameObject> ();
    Stack<GameObject> DFS_B = new Stack<GameObject>();
    private Nodes current;

    private void Start()
    {
        //runDjstkra();
     // dfsPath();
      //buildPath();
    }
 
    public void buildPath()
    {
        foreach (GameObject o in visited)
        {
            print(o);
        }
    }

    public void dfsPath()
    {
        DFS.Push(start_position);
        GameObject S;

        while (DFS.Count > 0)
        {
            S = DFS.Peek();
            DFS.Pop();


            if (!visited.Contains(S))
            {
                visited.Add(S);
            }


            S.GetComponent<Nodes>().getNeighbor();
            if (S.GetComponent<Nodes>().neighbors.Count > 0)
            {
                for (int i = 0; i < S.GetComponent<Nodes>().neighbors.Count; i++)
                {
                    if (!visited.Contains(S.GetComponent<Nodes>().neighbors[i]))
                    {
                        print(S.GetComponent<Nodes>().neighbors[i]);
                        DFS.Push(S.GetComponent<Nodes>().neighbors[i]);
                        if(S.GetComponent<Nodes>().neighbors[i]  == end_position){ break; }
                    }  
                }

            }

            if (S == end_position)
            {
                DFS.Clear(); // Destination has been found, delete everything 
            }

        }
    }

    public void dfsPath_B()
    {
        DFS_B.Push(start_position);
        GameObject S;

        while (DFS_B.Count > 0)
        {
            S = DFS_B.Peek();
            DFS_B.Pop();


            if (!visited_B.Contains(S))
            {
                visited_B.Add(S);
            }


            S.GetComponent<Nodes>().getNeighbor();
            if (S.GetComponent<Nodes>().neighbors.Count > 0)
            {
                for (int i = 0; i < S.GetComponent<Nodes>().neighbors.Count; i++)
                {
                    if (!visited_B.Contains(S.GetComponent<Nodes>().neighbors[i]))
                    {
                        //print(S.GetComponent<Nodes>().neighbors[i]);
                        DFS_B.Push(S.GetComponent<Nodes>().neighbors[i]);
                        if (S.GetComponent<Nodes>().neighbors[i] == end_B) { break; }
                    }
                }

            }

            if (S == end_B)
            {
                DFS_B.Clear(); // Destination has been found, delete everything 
            }

        }
    }
}


/*
 * 
 *      all_nodes = nodes; // These are all the nodes that can be transversed 
        unvisited = nodes; // All the unvisited nodes 

        dist = new float[all_nodes.Count]; // The same size as all nodse
        prev = new float[all_nodes.Count]; // previous nodes same size 

        int source = all_nodes.IndexOf(start_position);
        dist[source] = 0;
        while (all_nodes.Count > 0)
        {

        }



        // At the start we have viisted the position we are att 
        if (unvisited.Contains(start_position))
        {
            unvisited.Remove(start_position);
            visited.Add(start_position);
        }
 
     
*/
