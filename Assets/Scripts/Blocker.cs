using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // Put in the first block in the middle 
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.AddComponent<Rigidbody>();
        cube.transform.position = new Vector3(0, 0, -1.5f);

        GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.AddComponent<Rigidbody>();
        cube1.transform.position = new Vector3(1, 0, -1.5f);
        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.AddComponent<Rigidbody>();
        cube2.transform.position = new Vector3(1, 0, -0.5f);

    }
}
