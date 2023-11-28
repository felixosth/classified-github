using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dewarp : MonoBehaviour {

    Renderer rend;

    public GameObject test;

    MediaPlayerCtrl media;

	// Use this for initialization
	void Start () {

        media = GetComponentInChildren<MediaPlayerCtrl>();
	}
	
	// Update is called once per frame
	void Update () {
        rend = GetComponent<Renderer>();


        var tex = media.GetVideoTexture();

        //rend.material
        
        Texture2D newTex = new Texture2D(tex.width,tex.height, tex.format, tex.mipmapCount > 0 ? true : false);
        newTex.SetPixels(tex.GetPixels());
        newTex.Apply();
        //newTex.SetPixels(tex.ReadPixels());
        //newTex.Apply();
        test.GetComponent<Renderer>().material.mainTexture = newTex;

        //print(tex.GetPixel(tex.width/2, tex.height/2));
        //mat.SetColor("_")
        
		
	}
}
