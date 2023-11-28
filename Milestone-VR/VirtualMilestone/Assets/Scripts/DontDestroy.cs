using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour {

    public GameObject[] GameObjects;
    public string[] GameobjectStrings;

    public GameObject RightHand, LeftHand;
    public bool IsRight, IsLeft;


	// Use this for initialization
	void Start () {

        //Object.DontDestroyOnLoad(gameObject);
        //DontDestroyOnLoad(GameObject.Find("[SteamVR]"));

        //canvasInput.

        foreach(GameObject gameObj in GameObjects)
        {
            DontDestroyOnLoad(gameObj);
        }

        foreach(string s in GameobjectStrings)
        {
            DontDestroyOnLoad(GameObject.Find(s));
        }

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        //IsRight = RightHand.activeInHierarchy;
        //IsLeft = LeftHand.activeInHierarchy;

        //Debug.Log(IsRight + " : " + IsLeft);
        
	}

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var canvasInput = GetComponent<NVRCanvasInput>();

        foreach (GameObject gameObj in canvasInput.GameObjectsToDestroy)
        {
            Destroy(gameObj);
        }

        var cursor = canvasInput.CursorSprite;
        Destroy(canvasInput);
        canvasInput = gameObject.AddComponent<NVRCanvasInput>();
        canvasInput.CursorSprite = cursor;
        Debug.Log("Level " + arg0.name + " loaded.");
        //Debug.Log(GameObject.Find("Canvas").tag);

        if (!RightHand.activeInHierarchy)
            RightHand.SetActive(true);
        if (!LeftHand.activeInHierarchy)
            LeftHand.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void FixedUpdate()
    {

    }

    //private void OnLevelWasLoaded(int level)
    //{


    //    //RightHand.SetActive(true);

    //    //Destroy(GetComponentInChildren<Camera>().gameObject);
    //    //canvasInput.SendMessage("Start");


    //}
}
