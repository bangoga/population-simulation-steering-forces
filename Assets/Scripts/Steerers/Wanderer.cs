using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : MonoBehaviour {

	// Use this for initialization
    public Vector3 cirlceCenter = new Vector3(1f, 0f, 1f);
    private Vector3 c_dist = new Vector3(1f, 0, 1f);
    private Vector3 c_rad = new Vector3(1f, 0f, 1f);
    public float angle = 20f;
    private float angle_change = 5f;


    private float max_velocity = 0.01f;
    private float max_force = 0.05f;
    private float mass = 2f;
    private Vector3 currentVelocity;
    private Vector3 WanderValue;

    private Vector3 steering;
    private int counter = 0;
    private Vector3 last_target = Vector3.zero;
    private float radius = 0.2f;
    public GameObject FOVprefab;
    public SphereCollider FOV;
    public GameObject Trav;
    public bool blockMode = false;

    public GameObject exitA;
    public GameObject exitB;
    public GameObject blockingExit;


    public Vector3 lookAhead;
    public Vector3 lookAhead2;
    public float max_see_ahead = 0.5f;

    public float MAX_AVOID_FORCE = 0.001f;
    public float MAX_Soft_AVOID_FORCE = 0.001f;

    public float max_c_velocity;

    System.Random ran;
    public float slowDownFactor = 0.01f;

    private GameObject[] Obs_s;

    void Start () {
        ran = new System.Random(this.GetInstanceID());
        FOV = FOVprefab.GetComponent<SphereCollider>();
        exitA = GameObject.Find("Circle83");
        exitB = GameObject.Find("Circle79");
        Obs_s = GameObject.FindGameObjectsWithTag("Obs");
        max_c_velocity = max_velocity;
        // Trav.GetComponent<Rigidbody>().isKinematic = true;
        // InvokeRepeating("wander", 0.3f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        blockExit();
       // if(blockingExit != this.blockingE)
        addForces();
    }

    void wander()
    {
        System.Random ran = new System.Random();
        Vector3 CC = cirlceCenter;
        CC.Normalize();
        CC.Scale(c_dist);

        Vector3 displ = new Vector3(0,0,-1f);
        displ.Scale(c_rad);

        float len = displ.magnitude;

        
        displ.x = Mathf.Cos(angle * Mathf.Deg2Rad)*len;
        displ.y = Mathf.Sin(angle * Mathf.Deg2Rad) *len;

        
        angle += (ran.Next(0,360) * angle_change) - (angle_change * 0.5f);
        Vector3 wanderforce = new Vector3(CC.x + displ.x, 0, CC.z+displ.y);
        //return wanderforce;

        WanderValue = wanderforce;
    }
    public Vector3 simpleWander()
    {
        Vector3 target = Vector3.zero;
        //print(Vector3.Distance(Trav.transform.position, last_target));
        //if (Vector3.Distance(Trav.transform.position, last_target) <2.2)
        if (counter > 500)
        {
            float x = ran.Next(-4, 7);
            float z = ran.Next(-5, 0);
            last_target = new Vector3(x, 0, z);
            //print("simple wander (x,y) : " + "(" + x + "," + z + ")");
            counter = 0;
        }

        // calculate steering force 
        Vector3 position = this.transform.position - currentVelocity;
        Vector3 desiredVelocity = last_target - position;


        // Slow down factor 
        if (desiredVelocity.magnitude < radius)
        {
            desiredVelocity = Vector3.Normalize(desiredVelocity) * max_velocity * ((desiredVelocity.magnitude / 0.5f));
        }
        else desiredVelocity = Vector3.Normalize(desiredVelocity) * max_velocity;
        Vector3 steering = desiredVelocity - currentVelocity;

        return steering;
    }
    // Main Steering 

    void addForces()
    {
        if (!blockMode)
        {
            steering = simpleWander();
            steering = steering + avoidance();
           // print(steering);
        }
        if (blockMode)
        {
            steering = seek(blockingExit);
        }
        //print(steering);
        steering = Vector3.ClampMagnitude(steering, max_force); // for each force do clamp magnitutde 
        steering = steering / mass;
        currentVelocity = Vector3.ClampMagnitude(steering + currentVelocity, max_velocity);

        //this.transform.Translate(new Vector3(currentVelocity.x, 0,currentVelocity.y));
        Vector3 V = new Vector3(currentVelocity.x, 0, currentVelocity.z);

        Trav.GetComponent<Rigidbody>().position = Trav.GetComponent<Rigidbody>().position +V;



        counter++;
    }

    // If wanderer is near the exit and traveller is near the wanderer then block exit 

    void blockExit()
    {
        if (Vector3.Distance(FOV.transform.position, exitA.transform.position) < 2 && checkForTraveller())
        {
            blockMode = true;
            blockingExit = exitA;
        }

        else if (Vector3.Distance(FOV.transform.position, exitB.transform.position) < 2 && checkForTraveller())
        {
            blockMode = true;
            blockingExit = exitB;
        }

        else blockMode = false;
    }
    Vector3 seek(GameObject target)
    {
        Vector3 position = this.transform.position - currentVelocity;
        Vector3 target_p = new Vector3(target.transform.position.x + 0.3f, target.transform.position.y, target.transform.position.z);
        Vector3 desiredVelocity = target_p - position; // make max velocity warying 

        
        // Slow down factor 
        if (desiredVelocity.magnitude < radius)
        {
            desiredVelocity = Vector3.Normalize(desiredVelocity) * max_velocity * ((desiredVelocity.magnitude / 0.5f));
        }

        Vector3 steering = desiredVelocity - currentVelocity;

        return steering;
    }

    // Check if the traveller are bumping 
    bool checkForTraveller()
    {
        bool travelBump = false;
        GameObject [] travellers = GameObject.FindGameObjectsWithTag("traveller");
        if (travellers.Length > 1)
        {
            foreach (GameObject T in travellers)
            {
                if (T.GetComponent<SteeringForce>().FOV != null && FOV.bounds.Intersects(T.GetComponent<SteeringForce>().FOV.bounds) && T.transform.name != this.transform.name)
                {
                    travelBump = true;
                }
            }
        }
        return travelBump;
    }
    // for all travellers check for closeness
    public Vector3 avoidance()
    {
        Vector3 avoidance_force = Vector3.zero;
        lookAhead = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead;
        lookAhead2 = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead * 0.5f;

        foreach (GameObject o in Obs_s)
        {
            if (o.GetComponent<MeshCollider>().bounds.Intersects(FOV.bounds))
            {
                avoidance_force = this.GetComponent<CapsuleCollider>().bounds.ClosestPoint(lookAhead) - o.GetComponent<MeshCollider>().bounds.center;
                avoidance_force = new Vector3(avoidance_force.x, 0, avoidance_force.z); // No Y componenet 
                avoidance_force = Vector3.Normalize(avoidance_force) * MAX_AVOID_FORCE;
                max_velocity = slowDownFactor; // Change velocity for a while 
            }
            else max_velocity = max_c_velocity; // Change velocity back to normal 
        }

        return avoidance_force;
    }
}
