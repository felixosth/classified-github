using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;
using System.Threading;
using UnityEngine.SceneManagement;

public class RaycastTV : MonoBehaviour {

    private NVRHand handScript;

    public LineRenderer laserLineRenderer;
    //public float laserWidth = 0.1f;
    public float laserMaxLength = 5f;

    CamPlayerManager plyMng;


    bool firstPlay = true;


    public GameObject MiniScreen;

    int lastLevel;


    string currentlyPlayingURL;

    // Use this for initialization
    void Start () {

        handScript = GetComponent<NVRHand>();
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        lastLevel = SceneManager.GetActiveScene().buildIndex;
        //laserLineRenderer.startColor = Color.blue;
        //laserLineRenderer.endColor = Color.blue;
        //laserLineRenderer.startWidth = laserWidth;
        plyMng = GameObject.Find("CameraList").GetComponent<CamPlayerManager>();
        //laserLineRenderer.endWidth = laserWidth;
    }

    bool pressedLast = false;
	void Update () {

        var level = SceneManager.GetActiveScene().buildIndex;
        if (lastLevel != level)
        {
            print("Switched from " + lastLevel + " to " + level);
            plyMng = GameObject.Find("CameraList").GetComponent<CamPlayerManager>();
            lastLevel = level;
        }

        var pressedNow = handScript.Inputs[NVRButtons.ApplicationMenu].IsPressed;

        if(pressedNow)
        {
            laserLineRenderer.SetPosition(0, transform.position);
            var endPosition = transform.position + (laserMaxLength * transform.forward);

            laserLineRenderer.enabled = true;
            laserLineRenderer.SetPosition(1, endPosition);
        }
        else
        {
            laserLineRenderer.enabled = false;

        }

        if (pressedLast && !pressedNow)
        {
            var fwd = transform.TransformDirection(Vector3.forward);

            var ray = new RaycastHit();
            if (Physics.Raycast(transform.position, fwd, out ray))
            {
                if (ray.transform.gameObject.tag == "Screen")
                {
                    if (currentlyPlayingURL != "")
                    {
                        plyMng.UnsubscribeFromStream(currentlyPlayingURL, MiniScreen, true);
                    }

                    var newUrl = ray.transform.GetComponent<FillDropdown>().PlayingURL;
                    //currentlyPlayingURL = ;

                    if (newUrl == "")
                        return;
                    plyMng.PlayOrCreate(newUrl, MiniScreen);
                    currentlyPlayingURL = newUrl;
                }
            }
        }

        pressedLast = handScript.Inputs[NVRButtons.ApplicationMenu].IsPressed;
    }
}
