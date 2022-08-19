using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public CubeController player;
    public GameObject mainMenu;
    public GameObject winScreen;

    int boardSize = 10;
    LevelLoader levelLoader;

    void Start() {
        levelLoader = GameObject.FindObjectOfType<LevelLoader>();
        player.OnGoalReached += OnGoalReached;
    }

    public void LoadRandomLevel() {
        BoardState state = GenerateRandomBoard();
        levelLoader.LoadLevel(state);
    }

    public BoardState GenerateRandomBoard() {

        BoardState state = new BoardState();
        // prevent collisions
        List<Vector2> occupiedPos = new List<Vector2>();
        Vector2 pos;

        state.selfPos = GetRandUniqueVec2(1, boardSize-1, occupiedPos);
        state.goalPos = GetRandUniqueVec2(0, boardSize, occupiedPos);

        int numTrees = (int)Random.Range(5f, 10f);
        state.treePos = new List<Vector2>();
        for (int i=0; i<numTrees; i++)
            state.treePos.Add(GetRandUniqueVec2(1, boardSize-1, occupiedPos));

        return state;
    }

    void OnGoalReached() {
        StartCoroutine(ShowWinScreen());
    }

    IEnumerator ShowWinScreen() {
        yield return new WaitForSeconds(1.0f);
        winScreen.SetActive(true);
    }

    Vector2 GetRandUniqueVec2(float from, float to, List<Vector2> usedVecs) {
        Vector2 vec;
        do {
            vec = new Vector2(
                (int)(Random.Range(from, to)),
                (int)(Random.Range(from, to))
            );
        } while (usedVecs.Contains(vec));
        usedVecs.Add(vec);
        return vec;
    }
}
