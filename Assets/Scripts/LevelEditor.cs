using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public EditorMenu editorMenu;
    public GameObject highlight;
    public GameObject boardNameMenu;
    public Text savedBoardName;
    public Text saveBoardError;
    public Action OnObjPickedUp;
    public Action OnObjPlaced;

    [HideInInspector]
    public string savePath;

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

    public void SaveBoard() {
        GameObject cube = GameObject.FindGameObjectWithTag("Cube");
        GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

        Vector2 offset = new Vector2(4.5f, 4.5f);
        BoardState boardState = new BoardState();
        boardState.selfPos = offset +
            new Vector2(cube.transform.position.x, cube.transform.position.z);
        boardState.goalPos = offset +
            new Vector2(goal.transform.position.x, goal.transform.position.z);
        boardState.treePos = new List<Vector2>();
        foreach (GameObject tree in trees) {
            boardState.treePos.Add(offset + new Vector2(
                tree.transform.position.x, tree.transform.position.z));
        }
        string stateStr = boardState.SerialiseBoardState();
        Directory.CreateDirectory(savePath);
        if (savedBoardName.text == null || savedBoardName.text.Length <= 0)
            saveBoardError.text = "Cannot save with empty board name!";
        else {
            string path = savePath + "/" + savedBoardName.text + ".txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.Write(stateStr);
            writer.Close();
            boardNameMenu.SetActive(false);
            Debug.Log("Saved board to " + path + "!");
        }
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
            hoveredObj = hit.collider.transform.root.gameObject;
            ObjHighlight highlight = hoveredObj.GetComponent<ObjHighlight>();
            if (highlight)
                highlight.Highlight();
        }
        else {
            hoveredObj = null;
            // clear all highlights
            ObjHighlight[] highlights = FindObjectsOfType<ObjHighlight>();
            foreach (ObjHighlight h in highlights)
                h.Unhighlight();
        }
        if (GameObject.FindGameObjectsWithTag("Goal").Length > 0)
            editorMenu.menuBtns[1].interactable = false;
        if (GameObject.FindGameObjectsWithTag("Cube").Length > 0)
            editorMenu.menuBtns[2].interactable = false;
    }

    void Update() {
        if (selectedObj == null) {
            // turn off grid highlight
            if (highlight.activeSelf)
                highlight.SetActive(false);

            // pick up object
            if (Input.GetMouseButtonUp(0) && hoveredObj != null) {
                selectedObj = hoveredObj;
                if (OnObjPickedUp != null)
                    OnObjPickedUp();
            }
        } else {
            // snap to grid
            if (!highlight.activeSelf)
                highlight.SetActive(true);
            highlight.transform.position = new Vector3(
                Mathf.Round(mousePos.x + 0.5f) - 0.5f,
                0.01f,
                Mathf.Round(mousePos.z + 0.5f) - 0.5f
            );

            // drop object
            if (Input.GetMouseButtonUp(0) && isPointingFloor) {
                selectedObj.transform.position = highlight.transform.position;
                selectedObj = null;
                if (OnObjPlaced != null)
                    OnObjPlaced();
            }
        }
    }
}
