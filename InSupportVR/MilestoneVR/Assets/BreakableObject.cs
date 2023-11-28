using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject ObjectInPieces;
    public float BreakForce = 4;
    public float AddForce = 1.1f;
    public bool SetScaleToMine;
    private bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.relativeVelocity.magnitude);
        if (!isDestroyed && collision.relativeVelocity.magnitude >= BreakForce)
        {
            isDestroyed = true;

            GameObject wrecked = Instantiate(ObjectInPieces, transform.position, transform.rotation);
            if(SetScaleToMine)
                wrecked.transform.localScale = this.transform.localScale;

            foreach (Rigidbody body in wrecked.GetComponentsInChildren<Rigidbody>())
            {
                var force = GetComponent<Rigidbody>().velocity * AddForce;
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
