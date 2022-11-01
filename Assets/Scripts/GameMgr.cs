using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMode currMode = GameMode.PlayMode;
    public GameObject mainMenu;
    public GameObject editorMenu;
    public GameObject winScreen;
    public Transform gameModeCamPos;
    public Transform editorModeCamPos;
    public System.Action<GameMode> OnChangedMode;
    public string savePath;

    int boardSize = 10;
    LevelLoader levelLoader;
    LevelEditor levelEditor;
    SavedBoards savedBoards;

    public enum GameMode {
        PlayMode,
        EditorMode
    };

    void Start() {
        levelLoader = GameObject.FindObjectOfType<LevelLoader>();
        levelLoader.OnLevelLoaded += OnLevelLoaded;
        levelEditor = GameObject.FindObjectOfType<LevelEditor>();
        savedBoards = GameObject.FindObjectOfType<SavedBoards>();
        savePath = Application.persistentDataPath + "/SavedStates";
        levelEditor.savePath =
            levelLoader.savePath = savedBoards.savePath = savePath;
        savedBoards.gameObject.SetActive(false);
    }

    public void LoadRandomLevel() {
        BoardState state = GenerateRandomBoard();
        levelLoader.LoadLevel(state);
        SetCameraPos(gameModeCamPos);
    }

    public BoardState GenerateRandomBoard() {
        BoardState state = new BoardState();
        // prevent collisions and ban corners
        List<Vector2> occupiedPos = new List<Vector2>() {
            new Vector2(0, 0),
            new Vector2(0, boardSize-1),
            new Vector2(boardSize-1, 0),
            new Vector2(boardSize-1, boardSize-1)
        };
        Vector2 pos;

        state.selfPos = GetRandUniqueVec2(1, boardSize-1, occupiedPos);
        state.goalPos = GetRandUniqueVec2(0, boardSize, occupiedPos);

        int numTrees = (int)Random.Range(5f, 10f);
        state.treePos = new List<Vector2>();
        for (int i=0; i<numTrees; i++)
            state.treePos.Add(GetRandUniqueVec2(1, boardSize-1, occupiedPos));

        return state;
    }

    void OnLevelLoaded() {
        SetCameraPos(gameModeCamPos);
        currMode = GameMode.PlayMode;
        CubeController cubeController = FindObjectOfType<CubeController>();
        cubeController.OnGoalReached += OnGoalReached;
        cubeController.StartPlayMode();
        if (OnChangedMode != null)
            OnChangedMode(currMode);
    }

    void OnGoalReached() {
        StartCoroutine(ShowWinScreen());
    }

    IEnumerator ShowWinScreen() {
        yield return new WaitForSeconds(1.0f);
        winScreen.SetActive(true);
    }

    public void StartEditorMode() {
        levelLoader.UnloadBoard();
        StartCoroutine(AnimateCameraTo(editorModeCamPos, 1f));
        levelEditor.enabled = true;
        editorMenu.SetActive(true);
        currMode = GameMode.EditorMode;
        if (OnChangedMode != null)
            OnChangedMode(currMode);
    }

    public void ShowMainMenu() {
        levelEditor.enabled = false;
        editorMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    // helper functions

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

    void SetCameraPos(Transform newPos) {
        Camera.main.transform.position = newPos.position;
        Camera.main.transform.rotation = newPos.rotation;
    }

    IEnumerator AnimateCameraTo(Transform newPos, float duration) {
        Vector3 fromPos = Camera.main.transform.position;
        Vector3 toPos =  newPos.position;
        Quaternion fromRot = Camera.main.transform.rotation;
        Quaternion toRot = newPos.rotation;
        float startTime = Time.time;
        float endTime  = startTime + duration;

        while (Time.time < endTime) {
            float t = (Time.time - startTime)/duration;
            Camera.main.transform.position = Vector3.Lerp(fromPos, toPos, t);
            Camera.main.transform.rotation = Quaternion.Lerp(fromRot, toRot, t);
            yield return null;
        }
    }
}
