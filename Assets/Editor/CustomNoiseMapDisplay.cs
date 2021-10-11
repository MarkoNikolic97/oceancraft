using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(NoiseMapDisplay))]
public class CustomMapDisplay : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        NoiseMapDisplay display = (NoiseMapDisplay)target;

       
        if (GUILayout.Button("Generate Map"))
        {
            display.GenerateMap();
        }
    }
}
