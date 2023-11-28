using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class VrSettingsScript : MonoBehaviour {

	// Use this for initialization
	public void SetRenderScale (float scale) {

        VRSettings.renderScale = scale;
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
