using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    int boardSize = 10;
    LevelLoader levelLoader;

    void Start() {
        levelLoader = GameObject.FindObjectOfType<LevelLoader>();
    }

    public void LoadRandomLevel() {
        BoardState state = GenerateRandomBoard();
        levelLoader.LoadLevel(state);
    }

    public BoardState GenerateRandomBoard() {
        BoardState state = new BoardState();
        state.selfPos = new Vector2(
            (int)(Random.Range(0, boardSize)),
            (int)(Random.Range(0, boardSize))
        );
        state.goalPos = new Vector2(
            (int)(Random.Range(0, boardSize)),
            (int)(Random.Range(0, boardSize))
        );
        int numTrees = (int)Random.Range(3, 8f);
        state.treePos = new List<Vector2>();
        for (int i=0; i<numTrees; i++) {
            state.treePos.Add(new Vector2(
                (int)(Random.Range(1f, boardSize-1)),
                (int)(Random.Range(1f, boardSize-1))
            ));
        }
        return state;
    }
}
