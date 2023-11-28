using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPlayerScript : MonoBehaviour {

    MediaPlayerCtrl media;

    public string Url;
    int targets = 0;


    public void Play(string url)
    {
        this.Url = url;
        media.Load(url);
    }

    public void AddTexture(GameObject obj)
    {
        if (targets > media.m_TargetMaterial.Length)
            return;
        targets++;
        media.m_TargetMaterial[targets - 1] = obj;
        
    }

    public void RemoveTexture(GameObject obj, bool alsoRemoveResize)
    {
        for (int i = 0; i < media.m_TargetMaterial.Length; i++)
        {
            if(media.m_TargetMaterial[i] == obj)
            {
                media.m_TargetMaterial[i] = null;
                targets--;
                break;
            }
        }

        if(alsoRemoveResize)
        {
            for (int i = 0; i < media.m_objResize.Length; i++)
            {
                if (media.m_objResize[i] == obj)
                {
                    media.m_objResize[i] = null;
                    resizedObjects--;
                    break;
                }
            }
        }
    }

    int resizedObjects = 0;
    public void AddToResize(GameObject obj)
    {
        if (resizedObjects > media.m_objResize.Length)
            return;
        resizedObjects++;
        media.m_objResize[resizedObjects - 1] = obj;
    }

    private void Awake()
    {
        media = GetComponent<MediaPlayerCtrl>();
        //media.OnReady += OnReady;
    }
 //   // Use this for initialization
 //   void Start () {


	//}

    void OnReady()
    {
        media.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
