#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerNetworkManager))]
public class InspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerNetworkManager myScript = (PlayerNetworkManager)target;
        if (GUILayout.Button("Set Child Network Transform "))
        {
            myScript.SetNetworkChildTransform();
        }
    }
}
#endif