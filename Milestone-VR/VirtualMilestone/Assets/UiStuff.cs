using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiStuff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void NewScene()
    {
        gameObject.AddComponent<EventSystem>();
        gameObject.AddComponent<NewtonVR.NVRCanvasInput>();

    }
}
