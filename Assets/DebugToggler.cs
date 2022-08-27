using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DebugToggler : MonoBehaviour
{
    [HideInInspector] public bool isDebuggingActive;
    public DebugWindow debugWindow;


    public void Toggle()
    {
        isDebuggingActive = !isDebuggingActive;
        debugWindow.ToggleWindow();
    }
}

// [ExecuteInEditMode]
// [CustomEditor(typeof(DebugToggler))]
// public class DebugTogglerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         DebugToggler debugToggler = (DebugToggler)target;

//         if (!debugToggler.isDebuggingActive)
//         {
//             GUI.backgroundColor = Color.red;
//             if (GUILayout.Button("Debugging: disabled"))
//             {
//                 debugToggler.Toggle();
//             }
//         }
//         else
//         {
//             GUI.backgroundColor = Color.green;
//             if (GUILayout.Button("Debugging: enabled"))
//             {
//                 debugToggler.Toggle();
//             }
//         }


//     }
// }

