using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SavedBoards : MonoBehaviour
{
    public GameObject boardItem;

    [HideInInspector]
    public string savePath;

    public void LoadBoardNames() {
        // clear menu
        Transform menu = boardItem.transform.parent;
        foreach (Transform menuItem in menu) {
            if (menuItem.gameObject != boardItem)
                Destroy(menuItem.gameObject);
        }

        // load file names
        savePath = GameObject.FindObjectOfType<LevelEditor>().savePath;
        string[] filePaths = Directory.GetFiles(savePath);
        foreach (string filePath in filePaths) {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject file = GameObject.Instantiate(boardItem);
            file.transform.SetParent(menu);
            file.GetComponentInChildren<Text>().text = fileName;
            file.SetActive(true);
        }
    }
}
