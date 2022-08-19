using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    Renderer rend;
    GameObject cubeHighlight;

    void Start() {
        rend = GetComponent<Renderer>();
        cubeHighlight = transform.GetChild(0).gameObject;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Wall")) {
            // change to cube highlight
            rend.enabled = false;
            cubeHighlight.SetActive(true);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.CompareTag("Wall")) {
            cubeHighlight.SetActive(false);
            rend.enabled = true;
        }
    }
}
