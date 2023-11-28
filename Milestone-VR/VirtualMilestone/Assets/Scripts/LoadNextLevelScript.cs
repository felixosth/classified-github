using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevelScript : MonoBehaviour {

    public string SceneToLoad = "vrmilestone";
    public LoadSceneMode LoadSceneMode;

    public DontDestroy dontDestroy;

	// Use this for initialization
	void Start () {

        dontDestroy.IsRight = dontDestroy.RightHand.activeInHierarchy;
        dontDestroy.IsLeft = dontDestroy.LeftHand.activeInHierarchy;

        Debug.Log(dontDestroy.IsRight);
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
