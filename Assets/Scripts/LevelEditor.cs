using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    public GameObject highlight;
    public Action OnObjPlaced;

    GameObject selectedObj = null;
    GameObject hoveredObj = null;
    Camera mainCam;
    RaycastHit hit;
    Ray ray;
    LayerMask floorMask;
    LayerMask grabbableMask;
    Vector3 mousePos;
    bool isPointingFloor;

    void Start() {
        mainCam = Camera.main;
        floorMask = LayerMask.GetMask("Floor");
        grabbableMask = LayerMask.GetMask("Grabbable");
    }

    public void SpawnObject(GameObject obj) {
        if (selectedObj == null) {
            GameObject instantiatedObj =
                Instantiate(obj, mousePos, Quaternion.identity);
            selectedObj = instantiatedObj;
        }
    }

    public void RemoveSelectedObject() {
        Destroy(selectedObj);
    }

    void FixedUpdate()
    {
        // find mouse point on floor
        ray = mainCam.ScreenPointToRay(Input.mousePosition);
        isPointingFloor = Physics.Raycast(ray, out hit, 50, floorMask);
        if (isPointingFloor) {
            mousePos = hit.point;
            if (selectedObj)
                selectedObj.transform.position = mousePos;
        }
        // select board objects
        if (selectedObj == null &&
            Physics.Raycast(ray, out hit, 50, grabbableMask)) {
            Debug.Log("Selected obj: " + hit.collider.transform.root.gameObject.name);
            hoveredObj = hit.collider.transform.root.gameObject;
            ObjHighlight highlight = hoveredObj.GetComponent<ObjHighlight>();
            if (highlight)
                highlight.Highlight();
        }
        else {
            // clear all highlights
            ObjHighlight[] highlights = FindObjectsOfType<ObjHighlight>();
            foreach (ObjHighlight h in highlights)
                h.Unhighlight();
        }
    }

    void Update() {
        if (selectedObj == null) {
            if (highlight.activeSelf)
                highlight.SetActive(false);
            return;
        }

        // snap to grid
        if (!highlight.activeSelf)
            highlight.SetActive(true);
        highlight.transform.position = new Vector3(
            Mathf.Round(mousePos.x + 0.5f) - 0.5f,
            0.01f,
            Mathf.Round(mousePos.z + 0.5f) - 0.5f
        );

        if (Input.GetMouseButtonUp(0) && isPointingFloor) {
            selectedObj.transform.position = highlight.transform.position;
            selectedObj = null;
            if (OnObjPlaced != null)
                OnObjPlaced();
        }
    }
}
