using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject menuUI;
    public GameObject player;
    private bool gameEnded = false;

    void Update()
    {
        if (!gameEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        SetEnableCharacter(true);

        menuUI.SetActive(false);   
        GameIsPaused = false;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        SetEnableCharacter(false);

        Cursor.visible = true;
        menuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Defeat()
    {
        SetEnableCharacter(false);
        Cursor.visible = true;
        gameEnded = true;
        StartCoroutine(DefeatCoroutine());
    }

    public void Victory()
    {
        SetEnableCharacter(false);
        player.transform.Find("Fireworks 1").gameObject.SetActive(true);
        player.transform.Find("Fireworks 2").gameObject.SetActive(true);
        Cursor.visible = true;
        gameEnded = true;
        StartCoroutine(VictoryCoroutine());
    }

    private void SetEnableCharacter(bool isEnabled)
    {
        player.GetComponent<MoveBehaviour>().enabled = isEnabled;
        player.GetComponentInChildren<ThirdPersonOrbitCamBasic>().enabled = isEnabled;
    }

    IEnumerator DefeatCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        menuUI.SetActive(true);
        menuUI.transform.Find("Defeat").gameObject.SetActive(true);
        menuUI.transform.Find("ResumeButton").gameObject.SetActive(false);
    }

    IEnumerator VictoryCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        menuUI.SetActive(true);
        menuUI.transform.Find("Victory").gameObject.SetActive(true);
        menuUI.transform.Find("ResumeButton").gameObject.SetActive(false);
    }
}
