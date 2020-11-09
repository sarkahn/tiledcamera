using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[CustomEditor(typeof(TiledCamera))]
public class TiledCameraInspector : Editor
{
    [MenuItem("GameObject/TiledCamera/Create", false, 10)]
    public static void Create(MenuCommand command)
    {
        var go = new GameObject("Tiled Camera");
        Undo.RegisterCreatedObjectUndo(go, "Create TiledCamera");
        go.AddComponent<TiledCamera>();
        go.AddComponent<AudioListener>();
        go.transform.position = new Vector3(0, 0, -10);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        var tiledCam = target as TiledCamera;

        var cam = tiledCam.Camera;

        int w = cam.pixelWidth;
        int h = cam.pixelHeight;

        var pixelRatio = tiledCam.GetComponent<PixelPerfectCamera>().pixelRatio;

        EditorGUILayout.LabelField("Camera Info", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Viewport Pixel Size: {w}, {h}");
        EditorGUILayout.LabelField($"Pixel Ratio: {pixelRatio}"); 
    }

    public override bool RequiresConstantRepaint() => true;
}
