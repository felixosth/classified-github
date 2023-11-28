using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWithObjectsScript : MonoBehaviour {

    public Texture2D CrackedScreenTexture;

	// Use this for initialization
	void Start () {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //print(collision.gameObject.tag);
        if (collision.gameObject.tag == "CrackableScreen")
        {
            collision.gameObject.GetComponent<Renderer>().materials[1].mainTexture = CrackedScreenTexture;
            collision.gameObject.GetComponentInChildren<ParticleSystem>().Play();
            //collision.gameObject.GetComponent<Renderer>().material.SetTexture("LogoMaterial", CrackedScreenTexture);
            //foreach(Material mat in collision.gameObject.GetComponent<Renderer>().materials)
            //{
            //    //print(mat.name);
            //    if (mat.name == "LogoMaterial")
            //        mat.mainTexture = CrackedScreenTexture;
            //}
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
