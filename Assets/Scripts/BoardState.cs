using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardState : MonoBehaviour
{
    public Vector2 selfPos;
    public Vector2 goalPos;
    public List<Vector2> treePos;

    public static BoardState LoadBoardState(string jsonString) {
        return JsonUtility.FromJson<BoardState>(jsonString);
    }

    public string SerialiseBoardState() {
        return JsonUtility.ToJson(this);
    }
}
