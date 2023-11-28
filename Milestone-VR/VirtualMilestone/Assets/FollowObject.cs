using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

    public Transform FollowTransform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = FollowTransform.position;
        transform.rotation = FollowTransform.rotation;
		
	}
}
