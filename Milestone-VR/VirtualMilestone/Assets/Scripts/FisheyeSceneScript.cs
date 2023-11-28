using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FisheyeSceneScript : MonoBehaviour {

    public GameObject Selector;
    public Dropdown CameraDropdown;
    public MediaPlayerCtrl FisheyeDewarperMedia;


    public GameObject PlayerPrefab;
	// Use this for initialization
	void Awake () {

        if(!GameObject.Find("NVRPlayer"))
        {
            Instantiate(PlayerPrefab);
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayVideo()
    {
        var camList = GameObject.Find("CameraList").GetComponent<GetCameraList>().GetListCamScript();
        FisheyeDewarperMedia.Load(camList.GetStreamURL(CameraDropdown.value));

        Destroy(Selector);

    }

	public void LoadLevel(int lvl)
	{
        SceneManager.LoadSceneAsync(lvl);
	}
}
