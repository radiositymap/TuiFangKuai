using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SavedBoards : MonoBehaviour
{
    public GameObject boardItem;

    public string savePath;

    public void LoadBoardNames() {
        savePath = GameObject.FindObjectOfType<LevelEditor>().savePath;
        Debug.Log(savePath);
        string[] filePaths = Directory.GetFiles(savePath);
        foreach (string filePath in filePaths) {
            string fileName = Path.GetFileName(filePath);
            GameObject file = GameObject.Instantiate(boardItem);
            file.transform.parent = boardItem.transform.parent;
            file.GetComponentInChildren<Text>().text = fileName;
            file.SetActive(true);
        }
    }
}
