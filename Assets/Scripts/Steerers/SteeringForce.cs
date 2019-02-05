using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringForce : MonoBehaviour {
    public GameObject Destination_A;
    public GameObject Destination_B;
    private float mass = 2f;
    public Djisktra Dj_Khalid;

    public List<GameObject> path;
    
    private int currentNode = 0;

    private float max_velocity = 0.05f;
    private float max_force = 0.05f;
    private Vector3 currentVelocity;

    public Vector3 lookAhead;
    public Vector3 lookAhead2;
    public float max_see_ahead = 0.5f;

    public float MAX_AVOID_FORCE = 0.05f;
    public float MAX_Soft_AVOID_FORCE = 0.001f;

    public float max_c_velocity;
    public GameObject FOVprefab;
    public SphereCollider FOV;
    public GameObject Trav;
    public int ran;
    private GameObject[] Obs_s;
    public int bump_count = 0; // Used to see how long the traveller has been stuck if its been stuck for too long, then it changes directions
    private GameObject destination_end;
    public GameObject WSprefab;
    private WorldStates WS;

    public float slowDownFactor = 0.01f; // This is the factor by which you slow down near a obsticle.

    // Use this for initialization
    void Start () {
        Obs_s = GameObject.FindGameObjectsWithTag("Obs");                   // Get all the obsticles
        Dj_Khalid = GameObject.Find("Djkistra").GetComponent<Djisktra>();
        path = Dj_Khalid.visited;
        WSprefab = GameObject.Find("Total_Counter");
        WS = WSprefab.GetComponent<WorldStates>();

        FOV = FOVprefab.GetComponent<SphereCollider>();
        System.Random rnd = new System.Random(this.GetInstanceID());

        // Generate a random number and accordingly decide which direction to go to 
        ran = rnd.Next(-3, 3);
        if (ran <= 0)
        {
            Destination_A = GameObject.Find("Circle79");
            Destination_B = GameObject.Find("Circle83");
        }
        else
        {
            Destination_A = GameObject.Find("Circle83");
            Destination_B = GameObject.Find("Circle79");
        }

        // Randomized speed 
        max_velocity = Random.Range(0.01f, 0.05f);
        max_c_velocity = max_velocity;
    }
	
	// Update is called once per frame
	void Update () {
        // pathFollowing();
        stuff();
    }


    // seek towards the destination, if bump_count > n, aka stuck in obstacle, change direction 
    Vector3 seek()
    {
        Vector3 position = this.transform.position - currentVelocity;
        Vector3 desiredVelocity = Vector3.zero;
        if (bump_count < 700)
        {
            desiredVelocity = Vector3.Normalize(Destination_A.transform.position - position) * max_velocity; // make max velocity warying 
            destination_end = Destination_A;
        }

        else
        {
            desiredVelocity = Vector3.Normalize(Destination_B.transform.position - position) * max_velocity;
            destination_end = Destination_B;
        }// make max velocity warying 

        Vector3 steering = desiredVelocity - currentVelocity;
        steering = new Vector3(steering.x,0,steering.z);
        return steering;
    }


    // Main function with the steering and movememnt 
    public void stuff()
    {

        Vector3 steering = Vector3.zero;
        steering = seek();
        steering = steering + avoidance_travellers();
        steering = steering + avoidance();
        steering = steering + avoidance_wanderer();
        //steering = steering + avoidance_travellers();
        steering = Vector3.ClampMagnitude(steering, max_force);
        steering = steering / this.mass;



        currentVelocity = Vector3.ClampMagnitude(steering + currentVelocity, max_velocity);
        Trav.GetComponent<Rigidbody>().position= Trav.GetComponent<Rigidbody>().position + currentVelocity;

        if (this.GetComponent<CapsuleCollider>().bounds.Intersects(destination_end.GetComponent<BoxCollider>().bounds))
        {
            WS.successfuls = WS.successfuls + 1 ; // Add up all the successfuls 
            Destroy(Trav.gameObject);
        }
    }

    // If obs near by, give a nudge of max_ahead_force to avoid collision
    public Vector3 avoidance()
    {
        Vector3 avoidance_force = Vector3.zero;
        lookAhead = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead;
        lookAhead2 = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead * 0.5f;

        foreach(GameObject o in Obs_s) {
            if (o.GetComponent<MeshCollider>().bounds.Intersects(FOV.bounds))
            {
                avoidance_force = this.GetComponent<CapsuleCollider>().bounds.ClosestPoint(lookAhead) - o.GetComponent<MeshCollider>().bounds.center;
                avoidance_force = new Vector3(avoidance_force.x, 0, avoidance_force.z); // No Y componenet 
                avoidance_force = Vector3.Normalize(avoidance_force) * MAX_AVOID_FORCE;
                max_velocity = slowDownFactor; // Change velocity for a while 
               // print("avoid");
                bump_count++;
            }
            else max_velocity = max_c_velocity; // Change velocity back to normal 
        }

        return avoidance_force;
    }


    // Avoidance with other travellers 
    public Vector3 avoidance_travellers()
    {
        GameObject [] travellers = GameObject.FindGameObjectsWithTag("traveller");
        Vector3 avoidance_force = Vector3.zero;
        lookAhead = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead;
        lookAhead2 = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead * 0.5f;

        foreach (GameObject T in travellers)
        {
            if (T.GetComponent<SteeringForce>().FOV!=null &&  FOV.bounds.Intersects(T.GetComponent<SteeringForce>().FOV.bounds) && T.transform.name!=this.transform.name)
            {
                //print("intersection");
                avoidance_force = FOV.bounds.ClosestPoint(lookAhead) - T.GetComponent<CapsuleCollider>().bounds.center;
                avoidance_force = new Vector3(avoidance_force.x, 0, avoidance_force.z); // No Y componenet 
                avoidance_force = Vector3.Normalize(avoidance_force) * MAX_AVOID_FORCE;
            }
        }



        return avoidance_force;
    }


    // Go through the list of all the wanderers, if the wanderer in there is near by, avoid 
    public Vector3 avoidance_wanderer()
    {
        GameObject[] wanderer = GameObject.FindGameObjectsWithTag("Wanderer");
        Vector3 avoidance_force = Vector3.zero;
        lookAhead = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead;
        lookAhead2 = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead * 0.5f;

        foreach (GameObject T in wanderer)
        {
            if (T.GetComponent<Wanderer>().FOV != null && FOV.bounds.Intersects(T.GetComponent<Wanderer>().FOV.bounds) && T.transform.name != this.transform.name)
            {
                //print("intersection");
                avoidance_force = FOV.bounds.ClosestPoint(lookAhead) - T.GetComponent<CapsuleCollider>().bounds.center;
                avoidance_force = new Vector3(avoidance_force.x, 0, avoidance_force.z); // No Y componenet 
                avoidance_force = Vector3.Normalize(avoidance_force) * MAX_AVOID_FORCE;
                bump_count++;
            }
        }



        return avoidance_force;
    }



    //-------------------[UnUsed functions ]-------------//

    void pathFollowing()
    {
        GameObject targetNode = null;
        if (path.Count > 0)
        {
            targetNode = path[currentNode];
            if (Vector3.Distance(targetNode.transform.position, this.transform.position) < 5)
            {
                currentNode += 1;
                if (currentNode >= path.Count)
                {
                    currentNode = path.Count - 1; // change this behaviour, will make it act like its just walking around that area
                }
            }

        }
        if (targetNode != null)
        {
            seek(targetNode);
        }

    }
    void seek(GameObject goal)
    {
        Vector3 position = this.transform.position - currentVelocity;
        Vector3 desiredVelocity = Vector3.Normalize(goal.transform.position - position) * max_velocity; // make max velocity warying 
        Vector3 steering = desiredVelocity - currentVelocity;
        steering = Vector3.ClampMagnitude(steering, max_force);
        steering = steering / this.mass;
        currentVelocity = Vector3.ClampMagnitude(steering + currentVelocity, max_velocity);
        currentVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        this.transform.Translate(currentVelocity);
        if (this.GetComponent<CapsuleCollider>().bounds.Intersects(Destination_A.GetComponent<BoxCollider>().bounds))
        {
            Destroy(this.gameObject);
        }
    }

}
