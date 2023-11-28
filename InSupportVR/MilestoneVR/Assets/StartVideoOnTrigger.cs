using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVideoOnTrigger : MonoBehaviour
{
    public Transform ObjectToTrigger;
    public float MinDistanceToTrigger = 7;
    public MilestoneLiveVideo MilestoneLiveVideo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dist = GetDistanceTo(transform.position, ObjectToTrigger.position);
        //Debug.Log(dist);
        if (dist <= MinDistanceToTrigger)
        {
            MilestoneLiveVideo.StartVideo();
        }
        else
        {
            MilestoneLiveVideo.StopVideo();
        }
    }

    static float GetDistanceTo(Vector3 v1, Vector3 v2)
    {
        Vector3 difference = new Vector3(
          v1.x - v2.x,
          v1.y - v2.y,
          v1.z - v2.z);

        return (float)Math.Sqrt(
          Math.Pow(difference.x, 2f) +
          Math.Pow(difference.y, 2f) +
          Math.Pow(difference.z, 2f));
    }

}
