using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GetCameraList : MonoBehaviour {

    private CamListScript camListScript;

    public string Hostname = "localhost";

    [Header("Onvif")]
    public string OnvifUsername = "admin";
    public string OnvifPassword = "admin";
    [Header("Milestone")]
    public string MilestoneUsername = "onvif";
    public string MilestonePassword = "pass";
    public bool BasicUser = true;


    [Tooltip("If the camera contains this string")]
    public string FilterCameras;


    public CamListScript GetListCamScript()
    {
        return camListScript;
    }

    private void Awake()
	{
        //var lines = File.ReadAllLines(Application.streamingAssetsPath + "/cfg.txt");

        //camListScript = new CamListScript("localhost", "admin", "admin", true);

        camListScript = new CamListScript(Hostname, MilestoneUsername, MilestonePassword, BasicUser, OnvifUsername, OnvifPassword);
        camListScript.RefreshCameraList(FilterCameras);
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
