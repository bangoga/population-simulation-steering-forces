using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Socializers : MonoBehaviour {

    public bool isWanderer = true;


    // Use this for initialization

    System.Random ran;
    
    //public Vector3 cirlceCenter = new Vector3(5f, 0f, 5f);
    private Vector3 c_dist = new Vector3(1f, 0, 4f);
    private Vector3 c_rad = new Vector3(4f, 0f, 4f);
    public float angle = 40f;
    private float angle_change = 10f;


    private float max_velocity = 0.01f;
    private float max_force = 0.05f;
    private float mass = 2f;
    private Vector3 currentVelocity;
    private Vector3 WanderValue;

    private Vector3 steering;
    public Vector3 group_center;

    public Vector3 last_position;
    //  public All_Socializer AL;
    public GameObject[] socializes;
    public WorldStates Ws;
    public bool cliquemode = false;
    public List<GameObject> clique = new List<GameObject>();
    private float radius = 0.2f;

    public Vector3 last_target;

    private int counter = 0;
    public int waittime = 0;

    //--------------------------------//

    public Vector3 lookAhead;
    public Vector3 lookAhead2;
    public float max_see_ahead = 1.5f;

    public Vector3 avoidance_force;
    public float MAX_AVOID_FORCE = 0.1f;
    public float MAX_HARD_AVOID_FORCE = 0.2f;


    //---------------------------------//

    public GameObject FOVprefab;
    public SphereCollider FOV;
    public GameObject Trav;

    //---------------------------------//

    public bool coolOff = false;
    public float neighbors = 0f;
    public int cooldown = 0;

    //-----//
    public bool hasStarted = false;
    // Use this for initialization


     // CoolOff --> Its in cooldown mode and neesd to recoperate 
     // isWanderer --> is not in a clique and can wander arround 


    void Start () {
        clique.Add(this.gameObject);
        FOV = FOVprefab.GetComponent<SphereCollider>(); // <-------- Field of View 

        lookAhead = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead;
        lookAhead2 = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead * 0.5f;

        ran = new System.Random(this.GetInstanceID());
        socializes =  GameObject.FindGameObjectsWithTag("Socializer");
        last_target = Vector3.zero  ;

        // Get a random distance --> These aren't used initually 
        float x = ran.Next(-10, 10);
        float z = ran.Next(-10, 10);
        c_dist = new Vector3(x, 0, z);

         x = ran.Next(-10, 10);
         z = ran.Next(-10, 10);
        c_rad = new Vector3(x, 0, z);
    }
	
	// Update is called once per frame
	void Update () {
        coolDownControl();   // Control for when to socialize and when to cooldown
        colorChange();      // Change the color on cooldown
        if (!isWanderer)
        {
            syncCliques(); // Makes sure all the  cliques have the right items 
        }
        
        stuff();
    }


    
    void wander()
    {
        if (isWanderer)
        {
            
            Vector3 CC = currentVelocity;
            CC.Normalize();
            CC.Scale(c_dist);

            Vector3 displ = new Vector3(0, 0, -1f);
            displ.Scale(c_rad);

            float len = displ.magnitude;


            displ.x = Mathf.Cos(angle * Mathf.Deg2Rad) * len;
            displ.y = Mathf.Sin(angle * Mathf.Deg2Rad) * len;


            angle += (ran.Next(0, 360) * angle_change) - (angle_change * 0.5f);
            Vector3 wanderforce = new Vector3(CC.x + displ.x, 0, CC.z + displ.y);
            //return wanderforce;

            WanderValue = wanderforce;
        }
        else
        {
            WanderValue = Vector3.zero;
        }
    }

    void addForces()
    {
        if (isWanderer)
        {
            steering = WanderValue; // get first steering value
            //print(steering);
            steering = Vector3.ClampMagnitude(steering, max_force);
            steering = steering / mass;

            if (Vector3.Distance(currentVelocity, WanderValue) < 0)
            {
                currentVelocity = Vector3.Normalize(currentVelocity) * max_velocity * ((Vector3.Distance(currentVelocity, WanderValue) / 0.5f));
            }
            // trunacate
            currentVelocity = Vector3.ClampMagnitude(steering + currentVelocity, max_velocity);
            this.transform.position += new Vector3(currentVelocity.x, 0, currentVelocity.z);
        }
    }

    void formClique()
    {
        if (!clique.Contains(this.gameObject))
        {
            clique.Add(this.gameObject);
        }
        
        foreach (GameObject o in socializes)
        {
            if (Vector3.Distance(o.transform.position, this.transform.position) < 0.5 && o.transform.name != this.transform.name)
            {
                //print(Vector3.Distance(o.transform.position, this.transform.position));
                isWanderer = false;
                o.GetComponent<Socializers>().isWanderer = false;
                if (!clique.Contains(o) && cliquemode != true)
                {
                    clique=(clique.Union(o.GetComponent<Socializers>().clique)).ToList();
                }


                if (cliquemode != true)
                {
                    cliquemode = true;
                    last_position = this.transform.position;
                }
                
            }
        }
    }

    // last_position --> currentvelocity
    void makeClique()
    {

        group_center = Vector3.zero;
        foreach (GameObject o in clique)
        {
            group_center += o.GetComponent<Socializers>().last_position;
        }

        group_center= (group_center / clique.Count);
        group_center=Vector3.Normalize(group_center);
        //print(group_center);
        foreach (GameObject o in clique)
        {
            o.GetComponent<Socializers>().group_center=group_center;
        }

    }

    Vector3 makeCohesion()
    {
        Vector3 n = Vector3.zero;

        foreach (GameObject o in clique)
        {
            n += o.transform.position;
        }

        n= (n / clique.Count);

        n = new Vector3(n.x - this.transform.position.x, 0, n.z - this.transform.position.z);
        n=Vector3.Normalize(n);

        return n;
    }

    Vector3 Separation()
    {
        Vector3 n = Vector3.zero;
        foreach (GameObject o in clique)
        {
            n += o.transform.position - this.transform.position;
        }

        n = n * -1;
        return n;
    }

    void syncCliques()
    {
        foreach (GameObject o in clique)
        {
            o.GetComponent<Socializers>().clique = clique;
        }
    }
    
    void colorChange()
    {
        if (coolOff)
        {
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
        else this.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
    }
    void seek(Vector3 goal)
    {
        Vector3 position = this.transform.position - currentVelocity;
        Vector3 desiredVelocity = Vector3.Normalize(goal - position) * max_velocity;
        Vector3 steering = desiredVelocity - currentVelocity;
        steering = Vector3.ClampMagnitude(steering, max_force);
        steering = steering / this.mass;

        if (Vector3.Distance(position, goal) < 0.8)
        {
            currentVelocity = Vector3.Normalize(currentVelocity) * max_velocity * ((Vector3.Distance(position, goal) / 0.5f));
        }
        currentVelocity = Vector3.ClampMagnitude(steering + currentVelocity, max_velocity);
        this.transform.Translate(currentVelocity);
    }

 
    // The simple wanderer used for this, random points generated and go towards those points, slow down reaching the point 
    public Vector3 simpleWander()
    {
        Vector3 target = Vector3.zero;
        //print(Vector3.Distance(Trav.transform.position, last_target));
        //if (Vector3.Distance(Trav.transform.position, last_target) <2.2)
        if (counter >500)
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

    public void stuff()
    {
        var alignment = Vector3.zero ;
        var cohesion = Vector3.zero;
        var separation = Vector3.zero;

        // Normal wandering
        if (isWanderer) {
            steering = simpleWander();
            steering = steering + avoidance();
            last_position = Trav.transform.position;
        }

        if (!coolOff && hasStarted)
        {
            alignment = computeAlignment(); // Used to decide if in a clique or not 
        }
        
        // movement towards a group middle 
        if (!isWanderer && Vector3.Distance(group_center,Trav.transform.position)> 2 && !coolOff && hasStarted)
        {
            print(this.transform.name);
            steering = movetogroup()+avoidance();
        }
        else if (!isWanderer && hasStarted) // if no longer a wanderer then you need to start your movememnt frmo the start again 
        {
            steering = Vector3.zero;
            currentVelocity = Vector3.zero;
        }


        steering = Vector3.ClampMagnitude(steering, max_force);
        steering = steering / this.mass;

        currentVelocity = Vector3.ClampMagnitude(steering + currentVelocity, max_velocity);

        Vector3 V = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        Trav.GetComponent<Rigidbody>().position = Trav.GetComponent<Rigidbody>().position + V;
        counter += 1;
    }

    public Vector3 avoidance()
    {
        avoidance_force = Vector3.zero;
        lookAhead = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead;
        lookAhead2 = this.transform.position + Vector3.Normalize(currentVelocity) * max_see_ahead * 0.5f;

        foreach(GameObject o in socializes)
        {
            if(o.GetComponent<Socializers>().FOV == null)
            {
                break;
            }
            if (o.GetComponent<Socializers>().FOV != null && FOV.bounds.Intersects(o.GetComponent<Socializers>().FOV.bounds) && o.transform.name != this.transform.name)
            {
                avoidance_force = FOV.bounds.ClosestPoint(lookAhead) - o.GetComponent<CapsuleCollider>().bounds.center;
               // print(avoidance_force);
                avoidance_force = new Vector3(avoidance_force.x, 0, avoidance_force.z); // No Y componenet 
                avoidance_force = Vector3.Normalize(avoidance_force) * MAX_AVOID_FORCE;
                
            }
        }

        return avoidance_force;
    }

    public Vector3 movetogroup()
    {
        // calculate steering force 
        Vector3 position = this.transform.position - currentVelocity;
        Vector3 desiredVelocity = group_center - position;

        if (desiredVelocity.magnitude < radius)
        {
            desiredVelocity = Vector3.Normalize(desiredVelocity) * max_velocity * ((desiredVelocity.magnitude / 0.5f));
        }
        else desiredVelocity = Vector3.Normalize(desiredVelocity) * max_velocity;

        Vector3 steering = desiredVelocity - currentVelocity;
        return steering;
    }

    public Vector3 computeAlignment()
    {
        Vector3 v = Vector3.zero;
        foreach (GameObject o in socializes)
        {
            if (o.transform.name != this.transform.name)
            {
                if (o.GetComponent<Socializers>().FOV.bounds.Intersects(FOV.bounds))
                {
                    v.x += o.GetComponent<Socializers>().Trav.transform.position.x;
                    v.z += o.GetComponent<Socializers>().Trav.transform.position.z;

                    this.isWanderer = false;
                    
                    o.GetComponent<Socializers>().isWanderer = false;
                    currentVelocity = Vector3.zero;
                    if (!clique.Contains(o))
                    {
                        clique = (clique.Union(o.GetComponent<Socializers>().clique)).ToList();
                        neighbors++;
                    }
                }
            }
        }

        if (!isWanderer)
        {
            v.x /= clique.Count;
            v.z /= clique.Count;
            
            v = Vector3.Normalize(v);
            group_center = v;
           
        }


        return v;
    }


    // Delete the neighbors once all things are done 
    public void deleteNeighbors()
    {
        foreach(GameObject o in clique)
        {
            if(o.transform.name != this.transform.name)
            {
                clique.Remove(o);
                break;
            }
        }
    }

    void coolDownControl()
    {
        // if is stationary
        if (!isWanderer)
        {
            waittime++;
            if (waittime > 200)
            {
                // Done waiting
                waittime = 0;
                // Start cooloff
                coolOff = true;
                // Go back to wandering
                isWanderer = true;
                deleteNeighbors();
                // Delete all neighbours
            }
        }

        if (coolOff)
        {
            cooldown++;
            if (cooldown > 200)
            {
                cooldown = 0;
                coolOff = false;
            }
        }
    }
}
