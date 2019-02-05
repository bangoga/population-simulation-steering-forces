using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class All_Socializer : MonoBehaviour {

    private GameObject[] allSocializers; 
	void Start () {
        allSocializers = GameObject.FindGameObjectsWithTag("Socializer");
	}

    public GameObject[] getSocializers()
    {
        return allSocializers;
    }
	
}
