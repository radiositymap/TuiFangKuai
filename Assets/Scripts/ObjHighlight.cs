using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHighlight : MonoBehaviour
{
    Renderer[] renderers;
    Material highlightMat;
    List<Material> originalMats = new List<Material>();
    bool isHighlighted = false;

    void Start()
    {
        highlightMat = Resources.Load<Material>("Highlight");
        renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
            originalMats.Add(rend.sharedMaterial);
    }

    public void Highlight() {
        if (!isHighlighted) {
            foreach (Renderer rend in renderers)
                rend.sharedMaterial = highlightMat;
            isHighlighted = true;
        }
    }

    public void Unhighlight() {
        if (isHighlighted) {
            for (int i=0; i<renderers.Length; i++) {
                Debug.Log(renderers[i]);
                Debug.Log(originalMats[i]);
                renderers[i].sharedMaterial = originalMats[i];
            }
            isHighlighted = false;
        }
    }
}
