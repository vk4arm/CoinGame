using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject tutorialUI;

    void Start()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenu();
        }
    }

    public void ShowMenu()
    {
        menuUI.SetActive(true);
        tutorialUI.SetActive(false);
    }

    public void ShowTutorial()
    {
        menuUI.SetActive(false);
        tutorialUI.SetActive(true);
    }
}
