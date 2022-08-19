using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Color validColour;
    public Color invalidColour;
    public bool isInWall = false;
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

    public void UseWallHighlight() {
        rend.enabled = false;
        cubeHighlight.SetActive(true);
        isInWall = true;
    }

    void OnTriggerEnter(Collider collider) {
        if (GameMgr.currMode == GameMgr.GameMode.EditorMode) {
            if (collider.gameObject.CompareTag("Wall")) {
                // change to cube highlight
                UseWallHighlight();
            }
        }
    }

    public void UseFloorHighlight() {
        cubeHighlight.SetActive(false);
        rend.enabled = true;
        isInWall = false;
    }

    void OnTriggerExit(Collider collider) {
        if (GameMgr.currMode == GameMgr.GameMode.EditorMode) {
            if (collider.gameObject.CompareTag("Wall")) {
                UseFloorHighlight();
            }
        }
    }

    public void SetHighlightColour(bool isValid) {
        rend.sharedMaterial.color = isValid ? validColour : invalidColour;
    }
}
