using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPlayerManager : MonoBehaviour {


    public GameObject PlayerPrefab;

    List<CamPlayerScript> camPlayers;

	void Start () {
        camPlayers = new List<CamPlayerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayOrCreate(string url, GameObject sender, bool addToResize = false)
    {
        //var url = camList.GetStreamURL(index);
        bool foundPlayer = false;

        foreach (CamPlayerScript camPly in camPlayers)
        {
            if (camPly.Url == url)
            {
                camPly.AddTexture(sender);
                if(addToResize)
                    camPly.AddToResize(sender);
                foundPlayer = true;
            }
        }

        if(!foundPlayer)
        {
            var ply = Instantiate(PlayerPrefab, transform);
            var media = ply.GetComponent<CamPlayerScript>();
            media.AddTexture(sender);

            media.Play(url);

            camPlayers.Add(media);
        }
    }

    public void UnsubscribeFromStream(string url, GameObject sender, bool alsoRemoveResize = false)
    {
        foreach (CamPlayerScript camPly in camPlayers)
        {
            if (camPly.Url == url)
            {
                camPly.RemoveTexture(sender, alsoRemoveResize);
            }
        }
    }
}
