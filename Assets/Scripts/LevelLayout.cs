using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLayout : MonoBehaviour
{
    List<Vector2> blocks;
    List<Vector2> treePos;

    void WriteToFile(string filePath) {
        StreamWriter writer = new StreamWriter(filePath, false);
        writer.WriteLine(blocks.Count);
        for (int i=0; i<blocks.Count; i++)
            writer.WriteLine(blocks[i]);
        writer.WriteLine(treePos.Count);
        for (int i=0; i<treePos.Count; i++)
            writer.WriteLine(treePos[i]);
    }
}
