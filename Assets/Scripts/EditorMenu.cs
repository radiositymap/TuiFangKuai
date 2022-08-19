using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorMenu : MonoBehaviour
{
    public List<Button> menuBtns = new List<Button>();
    public Button removeBtn;

    LevelEditor levelEditor;

    void Start()
    {
        levelEditor = FindObjectOfType<LevelEditor>();
        levelEditor.OnObjPlaced += ResetMenu;
        for (int i=0; i<menuBtns.Count; i++) {
            // some C# problem
            // https://stackoverflow.com/questions/11524883/why-does-not-this-delegate-work-inside-the-loop
            int idx = i;
            menuBtns[i].onClick.AddListener(()=> OnObjSelected(idx));
        }
        removeBtn.onClick.AddListener(ResetMenu);
    }

    void OnObjSelected(int idx) {
        for (int i=0; i<menuBtns.Count; i++) {
            if (idx != i)
                menuBtns[i].interactable = false;
        }
    }

    void ResetMenu() {
        foreach (Button btn in menuBtns)
            btn.interactable = true;
        EventSystem.current.SetSelectedGameObject(null);
    }
}
