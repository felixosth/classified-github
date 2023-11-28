using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillDropdown : MonoBehaviour {

	CamListScript camList;
	public Dropdown dropDown;
	public GameObject Canvas;

    CamPlayerManager playerManager;
    public GameObject Screen;

    public string PlayingURL;

	//public MediaPlayerCtrl myMediaTex;

	// Use this for initialization
	void Start () {
        //mediaTex = GetComponentInChildren<MediaPlayerCtrl>();
        playerManager = GameObject.Find("CameraList").GetComponent<CamPlayerManager>();
		camList = GameObject.Find("CameraList").GetComponent<GetCameraList>().GetListCamScript();

		//while(camList.Cameras == null) { }

		dropDown.ClearOptions();

		foreach (MilestoneCamera cam in camList.Cameras)
		{
			dropDown.options.Add(new Dropdown.OptionData() { text = cam.Name });
		}

		dropDown.RefreshShownValue();

        //myMediaTex.OnReady += OnReady;
	}

    void OnReady()
    {
        //myMediaTex.Play();
    }

    void OnDestroy()
	{
		//myMediaTex.Stop();
	}

	public void StartClick()
	{
        var url = camList.GetStreamURL(dropDown.value);
        PlayingURL = url;
        //print(url);
        //myMediaTex.Load(url);
        //tex.Initialize();
        ////tex.Setup(camList.GetStreamURL(dropDown.value), 1, -1);
        //tex.Setup(camList.GetStreamURL(dropDown.value), 0, -1); //def
        ////tex.m_URI = camList.GetStreamURL(dropDown.value);
        //tex.Play();
        playerManager.PlayOrCreate(url, Screen);

		Canvas.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
