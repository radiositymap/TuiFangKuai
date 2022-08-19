using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    GameObject activeObj = null;

    public void SpawnObject(GameObject obj) {
        if (activeObj == null) {
            GameObject instantiatedObj = Instantiate(obj);
            activeObj = instantiatedObj;
        }
    }

    void Update()
    {
        if (activeObj)
            FollowMouse(activeObj);
    }

    void FollowMouse(GameObject obj) {
        Vector2 mousePos2D = Input.mousePosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePos2D.x, mousePos2D.y, 5.0f));
        obj.transform.position = mousePos;
    }
}
