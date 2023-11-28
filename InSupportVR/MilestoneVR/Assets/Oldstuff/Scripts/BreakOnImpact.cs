using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakOnImpact : MonoBehaviour {

    public float BreakForce = 4;
    public GameObject BrokenObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude >= BreakForce)
        {
            GameObject wrecked = Instantiate(BrokenObject, transform.position, transform.rotation);
            
            foreach(Rigidbody body in wrecked.GetComponentsInChildren<Rigidbody>())
            {
                var force = GetComponent<Rigidbody>().velocity * 1.1f;
                body.AddForce(force, ForceMode.VelocityChange);
            }
            //foreach (MeshFilter meshFilter in wrecked.GetComponentsInChildren<MeshFilter>())
            //{
            //    var gObj = meshFilter.gameObject;
            //    gObj.AddComponent<Rigidbody>();
            //    gObj.AddComponent<MeshCollider>();
            //    var meshCol = gObj.GetComponent<MeshCollider>();
            //    meshCol.convex = true;
            //    meshCol.sharedMesh = meshFilter.mesh;
            //    var force = GetComponent<Rigidbody>().velocity * 1.1f;
            //    gObj.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
            //}
            Destroy(gameObject);
        }
    }
}
