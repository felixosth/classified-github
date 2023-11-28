using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetAssets : MonoBehaviour
{
    private static bool created = false;
    void Awake()
    {
        if (!created)
        {
            AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sample_assets"));
            AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sample_scenes"));
            created = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
