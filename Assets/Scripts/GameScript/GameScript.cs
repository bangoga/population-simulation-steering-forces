using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameScript : MonoBehaviour {
    public GameObject s_Prefab;
    public GameObject t_prefab;
    public GameObject w_prefab;

    public GameObject startDoor;
    public RandomShapeGenerator RS;
    public GameObject obs_prefab;
    public GameObject[] graph;


    // --------------[ Changables for testing ] ------------- //
    public int SpawnTravellerNo;   // no of travellers
    public int SpawnWandererNo;   // no of wanderes
    public int SpawnSocialNumber; // no of socials
    public int obs_number = 1;    // no of obs


    // Getting the start spawn poitns for travellers and wanderers/spcoas 
    private Vector3 t_start = new Vector3(7.69f, -1.63f, -2.28f);
    private Vector3 w_start = new Vector3(-2f, -1.647288f, -4.72f); // 
    public Djisktra DJ; // not usedd
    public int travellerCount = 0;
    bool spawned = false;   // only after spawned can travellers respaw

    int last_given_number_traveller = 0 ;
    int last_given_number_wanderer = 0;
    int last_given_number_social = 0;

    List<GameObject> all_wanderer = new List<GameObject>();
    List<GameObject> all_social = new List<GameObject>();
    
    // Use this for initialization
    // Some First make obs, --> wait--> make wanderer/socials ---> wiat--> make travellers
    void Start () {
        graph = GameObject.FindGameObjectsWithTag("Node");
        for(int i = 0; i < obs_number; i++)
        {
            GameObject obs = Instantiate(obs_prefab);
            obs.GetComponent<RandomShapeGenerator>().createObject();
        }

        // resolvePath();
        StartCoroutine(LateSpawnWanderer(1));
        StartCoroutine(LateSpawnSocial(1));

        StartCoroutine(LateStart(2));
        StartCoroutine(LateEnableSocial(3));
        //spawnTraveller(1);

    }
	
	// Update is called once per frame
	void Update () {
        updateTravelCount();
        respawn();
    }
        

    // ------------------------------------------[ Waited starts ] -------------------//
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
       // DJ.dfsPath();
       // DJ.dfsPath_B();
        spawnTraveller(SpawnTravellerNo);
        //enableWanderer();
        spawned = true;
    }

    IEnumerator LateSpawnWanderer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        spawnWanderer(SpawnWandererNo);
    }

    IEnumerator LateSpawnSocial(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        spawnSocial(SpawnSocialNumber);
    }

    IEnumerator LateEnableSocial(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        enableSocial();
    }


    void enableWanderer()
    {
        foreach(GameObject W in all_wanderer)
        {
            W.GetComponent<Wanderer>().Trav.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    void enableSocial()
    {
        foreach (GameObject S in all_social)
        {
            S.GetComponent<Socializers>().hasStarted = true ;
        }
    }


    // -----------------------------------[ Spawning functions ] -----------------------//
    void spawnWanderer(int n)

    {
        for (int i = 0; i < n; i++)
        {
            GameObject traveller = Instantiate(w_prefab);
            traveller.transform.position = w_start;
            traveller.transform.GetChild(0).name = traveller.transform.GetChild(0).name + " " + last_given_number_wanderer;
            all_wanderer.Add(this.gameObject);
            last_given_number_wanderer += 1;
        }
    }

    void respawn()
    {
        if (travellerCount < SpawnTravellerNo && spawned)
        {
            spawnTraveller(1);
        }   
    }

    void spawnTraveller(int n)
    {
        // Spawn n number of travellers 
        
        for(int i = 0;i< n; i++)
        {
            GameObject traveller = Instantiate(t_prefab);
            traveller.transform.position = t_start;
            travellerCount += 1;
            traveller.transform.GetChild(0).name = traveller.transform.GetChild(0).name + " " + last_given_number_traveller;
            last_given_number_traveller += 1;
        }
        
    }

    void spawnSocial (int n)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject social = Instantiate(s_Prefab);
            social.transform.position = w_start;
            social.transform.GetChild(0).name = social.transform.GetChild(0).name + " " + last_given_number_social;
            all_social.Add(social.transform.GetChild(0).gameObject);
            last_given_number_social += 1;
        }
    }

    void updateTravelCount()
    {
        if (GameObject.FindGameObjectsWithTag("traveller") != null)
        {
            travellerCount = GameObject.FindGameObjectsWithTag("traveller").Length;
        }
        else travellerCount = 0;


    }
    /*
    void resolvePath()
    {
       foreach(GameObject node in graph)
        {
            if (node.GetComponent<BoxCollider>().bounds.Intersects(RS.GetComponent<MeshCollider>().bounds))
            {
                print("lock");
                Destroy(node);
            }
        }
    }

    int findNode(GameObject n)
    {
        for(int i = 0; i < graph.Length; i++)
        {
            if (n == graph[i])
            {
                return i;
            }
        }
        return -1;
    }

    */
}
