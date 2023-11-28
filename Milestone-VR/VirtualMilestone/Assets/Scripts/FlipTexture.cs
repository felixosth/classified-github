using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
