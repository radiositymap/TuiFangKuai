using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhotoCamera))]
public class PhotoCameraEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Take Picture"))
            TakePicture();
    }
    
    void TakePicture() {
        PhotoCamera photoCamera = (PhotoCamera)target;
        Camera camera = photoCamera.GetComponent<Camera>();
        RenderTexture currTexture = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.Render();

        int width = camera.targetTexture.width;
        int height = camera.targetTexture.height;
        Texture2D image = new Texture2D(width, height);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();

        RenderTexture.active = currTexture;

        byte[] bytes = image.EncodeToPNG();
        DestroyImmediate(image);

        string path =
            Application.dataPath + "/" + photoCamera.fileName + ".png";

        File.WriteAllBytes(path, bytes);
        Debug.Log("Picture written to " + path + "!");
    }
}
