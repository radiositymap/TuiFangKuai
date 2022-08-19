using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Action OnLevelLoaded;

    [HideInInspector]
    public string savePath;

    GameObject cube;
    GameObject goal;
    GameObject tree;

    void Start() {
        cube = Resources.Load<GameObject>("Cube");
        goal = Resources.Load<GameObject>("Goal");
        tree = Resources.Load<GameObject>("Tree");
    }

    public void LoadLevel(BoardState state)
    {
        UnloadBoard();

        // load board
        Vector3 offset = new Vector3(-4.5f, 0, -4.5f);
        GameObject boardCube = Instantiate(cube,
            new Vector3(state.selfPos.x, 0f, state.selfPos.y) + offset,
            Quaternion.identity);
        GameObject boardGoal = Instantiate(goal,
            new Vector3(state.goalPos.x, 0f, state.goalPos.y) + offset,
            Quaternion.identity);
        GameObject boardTree;
        foreach (Vector2 pos in state.treePos) {
            boardTree = Instantiate(tree, new Vector3(pos.x, 0, pos.y) + offset,
                Quaternion.identity);
        }
        if (OnLevelLoaded != null)
            OnLevelLoaded();
    }

    public void LoadLevel(Text boardName) {
        string boardPath = savePath + '/' + boardName.text + ".txt";
        string boardTxt = System.IO.File.ReadAllText(boardPath);
        BoardState boardState = BoardState.LoadBoardState(boardTxt);
        LoadLevel(boardState);
    }

    void UnloadBoard() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tree"))
            Destroy(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Cube"))
            Destroy(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Goal"))
            Destroy(obj);
    }
}