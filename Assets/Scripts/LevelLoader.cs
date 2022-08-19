using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject tree;

    List<Vector2> treePos;

    void Start()
    {
        treePos = new List<Vector2>()
        {
            new Vector2(1f,3f),
            new Vector2(2f,3f),
            new Vector2(6f,2f),
            new Vector2(5f,5f),
        };

        // load board
        foreach (Vector2 pos in treePos) {
            Instantiate(tree, new Vector3(pos.x - 4.5f, 0f, pos.y - 4.5f),
                Quaternion.identity);
        }
    }
}
