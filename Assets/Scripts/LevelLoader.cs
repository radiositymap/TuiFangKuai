using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    // TODO move these out to Resources
    public GameObject self;
    public GameObject goal;
    public GameObject tree;

    public void LoadLevel(BoardState state)
    {
        // load board
        Vector3 offset = new Vector3(-4.5f, 0, -4.5f);
        self.transform.position =
            new Vector3(state.selfPos.x, 0.5f, state.selfPos.y) + offset;
        goal.transform.position =
            new Vector3(state.goalPos.x, 0.52f, state.goalPos.y) + offset;
        foreach (Vector2 pos in state.treePos) {
            Instantiate(tree, new Vector3(pos.x, 0, pos.y) + offset,
                Quaternion.identity);
        }
    }
}