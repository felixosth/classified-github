using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(VrSettingsScript))]
public class CustomVRSettings : Editor {


    private float renderScale = 1f;

    public override void OnInspectorGUI()
    {
        renderScale = EditorGUILayout.FloatField("VR Render Scale", renderScale);

        if(GUILayout.Button("Set"))
        {
            ((VrSettingsScript)target).SetRenderScale(renderScale);
        }

    }
}

#endif