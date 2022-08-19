using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    Renderer rend;
    GameObject cubeHighlight;
    GameMgr gameManager;

    void Start() {
        gameManager = GameObject.FindObjectOfType<GameMgr>();
        gameManager.OnChangedMode += OnChangedMode;
        rend = GetComponent<Renderer>();
        cubeHighlight = transform.GetChild(0).gameObject;
    }

    void OnChangedMode(GameMgr.GameMode mode) {
        if (mode == GameMgr.GameMode.PlayMode) { // turn on highlight
            rend.enabled = false;
            cubeHighlight.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (GameMgr.currMode == GameMgr.GameMode.EditorMode) {
            if (collider.gameObject.CompareTag("Wall")) {
                // change to cube highlight
                rend.enabled = false;
                cubeHighlight.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider collider) {
        if (GameMgr.currMode == GameMgr.GameMode.EditorMode) {
            if (collider.gameObject.CompareTag("Wall")) {
                cubeHighlight.SetActive(false);
                rend.enabled = true;
            }
        }
    }
}
